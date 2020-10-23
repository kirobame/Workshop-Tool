using System;
using UnityEngine;
using UnityEngine.InputSystem;

// Base class meant to process an input in a certain way and generate callbacks when needed.
// Eg : Take Stick Vector2 and trigger a call only when the delta is approximately going up.
public abstract class Activator : ScriptableObject
{
    // Callbacks allowing complex behaviours.
    public event Action<object> OnBegan;
    public event Action<object> OnExecuted;
    public event Action<object> OnFinished;
    
    //------------------------------------------------------------------------------------------------------------------
    
    // Identifier to distinguish InputLinks processing the same InputAction.
    public abstract string ShortName { get; }
    
    // The InputAction being processed.
    protected InputAction currentInput;

    //------------------------------------------------------------------------------------------------------------------
    
    // Manual start replacement
    public virtual void Initialize() { }
    
    // Runtime binding to a specific InputAction. Allows to dynamically change the binding
    // of operations during runtime.
    public virtual void Bind(InputAction input)
    {
        if (currentInput != null) Unbind();
        currentInput = input;
    }
    public abstract void Unbind();

    // Methods allowing inheritors to trigger the different processing callbacks. 
    protected void Begin(object args) => OnBegan?.Invoke(args);
    protected void Execute(object args) => OnExecuted?.Invoke(args);
    protected void Finish(object args) => OnFinished?.Invoke(args);
}

// Generic implementation to correctly pass a casted input to the processing inheritors. 
public abstract class Activator<TInput> : Activator
{
    // Cached actions allowing easy subscribing behaviours to the InputAction events. 
    protected Action<InputAction.CallbackContext> started;
    protected Action<InputAction.CallbackContext> performed;
    protected Action<InputAction.CallbackContext> canceled;

    //------------------------------------------------------------------------------------------------------------------
    
    // Extension of the base method to cast the received input. 
    public override void Bind(InputAction input)
    {
        base.Bind(input);
        
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
    
    //------------------------------------------------------------------------------------------------------------------
    
    // Actual processing methods.
    protected virtual void OnStarted(TInput input) { }
    protected virtual void OnPerformed(TInput input) { }
    protected virtual void OnCanceled(TInput input) { }
}