using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

public abstract class Operation : ScriptableObject
{
    public event Action<object,Object[]> OnBeginning;
    public event Action<object,Object[]> OnPerformed;
    public event Action<object,Object[]> OnEnd;
    
    [SerializeField] private PreProcessor[] preProcessors;
    [SerializeField] private List<SubOperation> subOperations;

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
    public abstract void Execute(object args, params Object[] parameters);

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
    
    protected void Begin(object args, params Object[] parameters) => OnBeginning?.Invoke(args, parameters);
    protected void Perform(object args, params Object[] parameters) => OnPerformed?.Invoke(args, parameters);
    protected void End(object args, params Object[] parameters) => OnEnd?.Invoke(args, parameters);
}
public abstract class Operation<TArgs, TSource> : Operation where TSource : OperationHandler
{
    protected TSource source;

    public override void SetSource(OperationHandler source) => this.source = (TSource)source;
    public override void Execute(object args, params Object[] parameters)
    {
        foreach (var preProcessor in PreProcessors) preProcessor.Affect(this);
        Execute((TArgs)args, parameters);
        foreach (var preProcessor in PreProcessors.Reverse()) preProcessor.UndoFor(this);
    }

    protected abstract void Execute(TArgs args, Object[] parameters);
}