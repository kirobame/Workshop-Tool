using UnityEngine;

public abstract class PreProcessor : ScriptableObject
{
    public abstract void Affect(Operation operation);
    public abstract void UndoFor(Operation operation);
}
public abstract class PreProcessor<TOp> : PreProcessor where TOp : Operation
{
    public override void Affect(Operation operation) => Affect(operation as TOp);
    protected abstract void Affect(TOp operation);

    public override void UndoFor(Operation operation) => UndoFor(operation as TOp);
    protected abstract void UndoFor(TOp operation);
}