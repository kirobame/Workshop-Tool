using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

// Asset class representing an action to be executed on specific Activator or other Operation callbacks.
public abstract class Operation : ScriptableObject
{
    
    // An Operation also declares complex behaviours in order to creation action chains. 
    public event Action<object,Object[]> OnBegan;
    public event Action<object,Object[]> OnPerformed;
    public event Action<object,Object[]> OnEnd;

    //------------------------------------------------------------------------------------------------------------------

    // Set of modifications modifying the data of the behaviour. 
    [SerializeField] private PreProcessor[] preProcessors;
    
    // The phases to which this will class will listen to in order to execute its behaviour.
    public OperationPhase Phase => phase;
    [SerializeField] private OperationPhase phase;
    
    // Containers of other Operations being executed with the callback of this instance. 
    [SerializeField] protected List<SubOperation> subOperations;

    // Runtime interpretations of assets in order to avoid completely corruption of data. 
    protected IEnumerable<PreProcessor> PreProcessors => runtimePreProcessors;
    private HashSet<PreProcessor> runtimePreProcessors;

    //------------------------------------------------------------------------------------------------------------------
    
    // Creation of runtime interpretations & initialization of chained Operations. 
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

    // Defines the Component owner of this instance. 
    public abstract void SetSource(OperationHandler source);

    //------------------------------------------------------------------------------------------------------------------
    
    // Addition & Removal of runtime data. 
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
    
    //------------------------------------------------------------------------------------------------------------------
    
    // Root Start method used for subscription to Activator or other Operation
    public void Begin(object args, params Object[] parameters)
    {
        foreach (var preProcessor in PreProcessors) preProcessor.Affect(this);
        AtStart(args, parameters);
        foreach (var preProcessor in PreProcessors.Reverse()) preProcessor.UndoFor(this);
    }
    // Abstract opening for inheritors to not disturb the effect of PreProcessors.
    protected abstract void AtStart(object args, params Object[] parameters);

    // Root Update method used for subscription to Activator or other Operation
    public void Perform(object args, params Object[] parameters)
    {
        foreach (var preProcessor in PreProcessors) preProcessor.Affect(this);
        During(args, parameters);
        foreach (var preProcessor in PreProcessors.Reverse()) preProcessor.UndoFor(this);
    }
    protected abstract void During(object args, params Object[] parameters);

    // Root End method used for subscription to Activator or other Operation
    public void End(object args, params Object[] parameters)
    {
        foreach (var preProcessor in PreProcessors) preProcessor.Affect(this);
        AtEnd(args, parameters);
        foreach (var preProcessor in PreProcessors.Reverse()) preProcessor.UndoFor(this);
    }
    protected abstract void AtEnd(object args, params Object[] parameters);

    //------------------------------------------------------------------------------------------------------------------
    
    protected void CallStart(object args, params Object[] parameters) => OnBegan?.Invoke(args, parameters);
    protected void CallPerform(object args, params Object[] parameters) => OnPerformed?.Invoke(args, parameters);
    protected void CallEnd(object args, params Object[] parameters) => OnEnd?.Invoke(args, parameters);
}

// Generic implementation to remove object primitivity. 
public abstract class Operation<TArgs, TSource> : Operation where TSource : OperationHandler
{
    protected TSource source;
    // Passing the source down the action chain. 
    public override void SetSource(OperationHandler source)
    {
        this.source = (TSource) source;
        foreach (var subOperation in subOperations) subOperation.Value.SetSource(source);
    }
    
    //------------------------------------------------------------------------------------------------------------------

    // Casting Activator arguments to remove ambiguity. 
    protected override void AtStart(object args, params Object[] parameters) => AtStart((TArgs) args, parameters);
    protected virtual void AtStart(TArgs args, Object[] parameters) { }
    
    protected override void During(object args, params Object[] parameters) => During((TArgs) args, parameters);
    protected virtual void During(TArgs args, Object[] parameters) { }
    
    protected override void AtEnd(object args, params Object[] parameters) => AtEnd((TArgs) args, parameters);
    protected virtual void AtEnd(TArgs args, Object[] parameters) { }
}