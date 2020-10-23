using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using Object = UnityEngine.Object;

[Serializable]
// Container class specifying a complex link between :
// An InputAction towards an Activator...
// ...An Activator towards many Operations. 
public class InputLink
{
    public Activator Activator => runtimeActivator;
    public IEnumerable<Operation> Operations => runtimeOperands.Select(operand => operand.Operation);
    
    // Template data.
    [SerializeField] private InputActionReference inputReference;
    [SerializeField] private Activator activator;
    
    // Operation containers allowing a more easy linkage.
    [SerializeField] private Operand[] operands;

    // Runtime representations to avoid any data corruption.
    private Activator runtimeActivator;
    private HashSet<Operand> runtimeOperands;

    //------------------------------------------------------------------------------------------------------------------
    
    // Links to an OperationHandler or any other Component's enabled state in order to stop or resume the linkage.
    public void Enable() => runtimeActivator.Bind(inputReference.action);
    public void Disable() => runtimeActivator.Unbind();
    
    // Manual Start replacement allowing creation of runtime representations & Base linkage between Activator & Operations.
    public void Initialize()
    {
        runtimeActivator = Object.Instantiate(activator);
        runtimeActivator.Initialize();

        runtimeOperands = new HashSet<Operand>();
        foreach (var operand in operands)
        {
            operand.Initialize();
            runtimeOperands.Add(operand);
            
            operand.LinkTo(runtimeActivator);
        }
    }

    //------------------------------------------------------------------------------------------------------------------
    
    // Addition & Removal of runtime data. 
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