using System;
using UnityEngine;
using Object = UnityEngine.Object;

[Serializable]
public class Operand
{
    public Operand(Operation source, params Object[] parameters)
    {
        operation = source;
        this.parameters = parameters;
    }
    
    public Operation Operation => runtimeOperation;

    [SerializeField] private Operation operation;
    [SerializeField] private Object[] parameters;

    private Action<object> beginAction;
    private Action<object> performAction;
    private Action<object> endAction;
    
    private Operation runtimeOperation;

    public void Initialize()
    {
        runtimeOperation = Object.Instantiate(operation);
        runtimeOperation.Initialize();
    }

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