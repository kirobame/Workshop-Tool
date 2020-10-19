using UnityEngine;

public class UpdatableActivator<TInput> : Activator<TInput>, IUpdatable
{
    private TInput input;
    private bool canBeUpdated;

    protected override void OnStarted(TInput input)
    {
        this.input = input;
        canBeUpdated = true;
    }
    protected override void OnPerformed(TInput input) => this.input = input;
    protected override void OnCanceled(TInput input)
    {
        this.input = input;
        canBeUpdated = false;
    }

    public virtual void Update()
    {
        if (!canBeUpdated) return;
        Execute(input);
    }
}