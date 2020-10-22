using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

public abstract class Operation : ScriptableObject
{
    public event Action<object,Object[]> OnBegan;
    public event Action<object,Object[]> OnPerformed;
    public event Action<object,Object[]> OnEnd;

    public OperationPhase Phase => phase;
    
    [SerializeField] private PreProcessor[] preProcessors;
    [SerializeField] private OperationPhase phase;
    [SerializeField] protected List<SubOperation> subOperations;

    protected IEnumerable<PreProcessor> PreProcessors => runtimePreProcessors;
    private HashSet<PreProcessor> runtimePreProcessors;

    public virtual void Initialize()
    {
        runtimePreProcessors = new HashSet<PreProcessor>();
        foreach (var preProcessor in preProcessors)
        {
            var runtimePreProcessor = Instantiate(preProcessor);
            runtimePreProcessors.Add(runtimePreProcessor);
        }

        foreach (var subOperation in subOperations) subOperation.Link(this);
    }

    public abstract void SetSource(OperationHandler source);

    public bool AddPreProcessor(PreProcessor preProcessor) => runtimePreProcessors.Add(preProcessor);
    public bool RemovePreProcessor(PreProcessor preProcessor) => runtimePreProcessors.Remove(preProcessor);

    public void AddSubOperation(SubOperation subOperation)
    {
        subOperation.Link(this);
        subOperations.Add(subOperation);
    }
    public void RemoveSubOperation(SubOperation subOperation)
    {
        if (!subOperations.Remove(subOperation)) return;
        subOperation.BreakLinkage(this);
    }
    
    public void Begin(object args, params Object[] parameters)
    {
        foreach (var preProcessor in PreProcessors) preProcessor.Affect(this);
        AtStart(args, parameters);
        foreach (var preProcessor in PreProcessors.Reverse()) preProcessor.UndoFor(this);
    }
    protected abstract void AtStart(object args, params Object[] parameters);

    public void Perform(object args, params Object[] parameters)
    {
        foreach (var preProcessor in PreProcessors) preProcessor.Affect(this);
        During(args, parameters);
        foreach (var preProcessor in PreProcessors.Reverse()) preProcessor.UndoFor(this);
    }
    protected abstract void During(object args, params Object[] parameters);

    public void End(object args, params Object[] parameters)
    {
        foreach (var preProcessor in PreProcessors) preProcessor.Affect(this);
        AtEnd(args, parameters);
        foreach (var preProcessor in PreProcessors.Reverse()) preProcessor.UndoFor(this);
    }
    protected abstract void AtEnd(object args, params Object[] parameters);

    protected void CallStart(object args, params Object[] parameters) => OnBegan?.Invoke(args, parameters);
    protected void CallPerform(object args, params Object[] parameters) => OnPerformed?.Invoke(args, parameters);
    protected void CallEnd(object args, params Object[] parameters) => OnEnd?.Invoke(args, parameters);
}
public abstract class Operation<TArgs, TSource> : Operation where TSource : OperationHandler
{
    protected TSource source;

    public override void SetSource(OperationHandler source)
    {
        this.source = (TSource) source;
        foreach (var subOperation in subOperations) subOperation.Value.SetSource(source);
    }

    protected override void AtStart(object args, params Object[] parameters) => AtStart((TArgs) args, parameters);
    protected virtual void AtStart(TArgs args, Object[] parameters) { }
    
    protected override void During(object args, params Object[] parameters) => During((TArgs) args, parameters);
    protected virtual void During(TArgs args, Object[] parameters) { }
    
    protected override void AtEnd(object args, params Object[] parameters) => AtEnd((TArgs) args, parameters);
    protected virtual void AtEnd(TArgs args, Object[] parameters) { }
}