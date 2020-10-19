using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using Object = UnityEngine.Object;

[Serializable]
public class InputLink
{
    public Activator Activator => runtimeActivator;
    public IEnumerable<Operation> Operations => runtimeOperands.Select(operand => operand.Operation);
    
    [SerializeField] private InputActionReference inputReference;
    [SerializeField] private Activator activator;
    [SerializeField] private Operand[] operands;

    private Activator runtimeActivator;
    private HashSet<Operand> runtimeOperands;

    public void Enable() => runtimeActivator.Bind(inputReference.action);
    public void Disable() => runtimeActivator.Unbind();
    
    public void Initialize()
    {
        runtimeActivator = Object.Instantiate(activator);

        runtimeOperands = new HashSet<Operand>();
        foreach (var operand in operands)
        {
            operand.Initialize();
            runtimeOperands.Add(operand);
            
            operand.LinkTo(runtimeActivator);
        }
    }

    public bool AddOperand(Operand operand)
    {
        if (runtimeOperands.Add(operand))
        {
            operand.LinkTo(runtimeActivator);
            return true;
        }

        return false;
    }
    public bool RemoveOperand(Operand operand)
    {
        if (runtimeOperands.Remove(operand))
        {
            operand.BreakLinkFor(runtimeActivator);
            return true;
        }

        return false;
    }
}