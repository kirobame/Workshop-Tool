using System;
using UnityEngine;
using Object = UnityEngine.Object;

[Serializable]
// Container class for an Operation which allows to specify what runtime parameters from the scene will be sent to 
// the Operation instance.
public class Operand
{
    public Operand(Operation source, params Object[] parameters)
    {
        operation = source;
        this.parameters = parameters;
    }
    
    //------------------------------------------------------------------------------------------------------------------
    
    public Operation Operation => runtimeOperation;

    // Template data.
    [SerializeField] private Operation operation;
    [SerializeField] private Object[] parameters;

    // Cached callbacks for easy subscription behaviours.
    private Action<object> beginAction;
    private Action<object> performAction;
    private Action<object> endAction;
    
    // Runtime representation to avoid any data corruption.
    private Operation runtimeOperation;

    //------------------------------------------------------------------------------------------------------------------
    
    // Manual Start replacement to create runtime representation of the Operation template. 
    public void Initialize()
    {
        runtimeOperation = Object.Instantiate(operation);
        runtimeOperation.Initialize();
    }
    
    //------------------------------------------------------------------------------------------------------------------

    // Linkage to an activator based on the Listened phases of the Operation. 
    public void LinkTo(Activator activator)
    {
        beginAction = args => runtimeOperation.Begin(args, parameters);
        performAction = args => runtimeOperation.Perform(args, parameters);
        endAction = args => runtimeOperation.End(args, parameters);

        if (runtimeOperation.Phase.HasFlag(OperationPhase.Beginning)) activator.OnBegan += beginAction;
        if (runtimeOperation.Phase.HasFlag(OperationPhase.During)) activator.OnExecuted += performAction;
        if (runtimeOperation.Phase.HasFlag(OperationPhase.End)) activator.OnFinished += endAction;
    }
    public void BreakLinkFor(Activator activator)
    {
        if (runtimeOperation.Phase.HasFlag(OperationPhase.Beginning)) activator.OnBegan -= beginAction;
        if (runtimeOperation.Phase.HasFlag(OperationPhase.During)) activator.OnExecuted -= performAction;
        if (runtimeOperation.Phase.HasFlag(OperationPhase.End)) activator.OnFinished -= endAction;
    }
}