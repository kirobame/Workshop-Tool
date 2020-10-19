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

    private Action<object> action;
    private Operation runtimeOperation;

    public void Initialize()
    {
        runtimeOperation = Object.Instantiate(operation);
        runtimeOperation.Initialize();
    }

    public void LinkTo(Activator activator)
    {
        action = args => runtimeOperation.Execute(args, parameters);
        activator.OnExecuted += action;
    }
    public void BreakLinkFor(Activator activator) =>  activator.OnExecuted -= action;
}