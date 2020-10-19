using System;
using UnityEngine;
using UnityEngine.InputSystem;

public abstract class Activator : ScriptableObject
{
    public event Action<object> OnExecuted;
    
    protected InputAction currentInput;

    public virtual void Bind(InputAction input)
    {
        if (currentInput != null) Unbind();
        currentInput = input;
    }
    public abstract void Unbind();

    public void Execute(object args) => OnExecuted?.Invoke(args);
}
public abstract class Activator<TInput> : Activator
{
    protected Action<InputAction.CallbackContext> started;
    protected Action<InputAction.CallbackContext> performed;
    protected Action<InputAction.CallbackContext> canceled;

    public override void Bind(InputAction input)
    {
        if (currentInput != null) Unbind();
        currentInput = input;
        
        started = ctxt => OnStarted((TInput)Convert.ChangeType(ctxt.ReadValueAsObject(), typeof(TInput)));
        input.started += started;
        
        performed = ctxt => OnPerformed((TInput)Convert.ChangeType(ctxt.ReadValueAsObject(), typeof(TInput)));
        input.performed += performed;
        
        canceled = ctxt => OnCanceled((TInput)Convert.ChangeType(ctxt.ReadValueAsObject(), typeof(TInput)));
        input.canceled += canceled;
    }
    public override void Unbind()
    {
        currentInput.started -= started;
        started = null;
        
        currentInput.performed -= performed;
        performed = null;
        
        currentInput.canceled -= canceled;
        canceled = null;

        currentInput = null;
    }
    
    protected virtual void OnStarted(TInput input) { }
    protected virtual void OnPerformed(TInput input) { }
    protected virtual void OnCanceled(TInput input) { }
}