using System;
using UnityEngine;
using Object = UnityEngine.Object;

[Serializable]
public class SubOperation : IEquatable<SubOperation>
{
    public SubOperation(Operation source, OperationPhase overridingPhase = OperationPhase.None)
    {
        value = source;
        this.overridingPhase = this.overridingPhase;
    }

    public Operation Value => runtimeValue;
    
    [SerializeField] private Operation value;
    [SerializeField] private OperationPhase overridingPhase;

    private Action<object, Object[]> beginAction;
    private Action<object, Object[]> performAction;
    private Action<object, Object[]> endAction;
    private Operation runtimeValue;
        
    public void Link(Operation source)
    {
        beginAction = (args, parameters) => runtimeValue.Begin(args, parameters);
        performAction = (args, parameters) => runtimeValue.Perform(args, parameters);
        endAction = (args, parameters) => runtimeValue.End(args, parameters);
        
        runtimeValue = Object.Instantiate(value);
        runtimeValue.Initialize();

        var phase = overridingPhase == OperationPhase.None ? value.Phase : overridingPhase;
        
        if (phase.HasFlag(OperationPhase.Beginning)) source.OnBegan += beginAction;
        if (phase.HasFlag(OperationPhase.During)) source.OnPerformed += performAction;
        if (phase.HasFlag(OperationPhase.End)) source.OnEnd += endAction;
    }
    public void BreakLinkage(Operation source)
    {
        var phase = overridingPhase == OperationPhase.None ? value.Phase : overridingPhase;
        
        if (phase.HasFlag(OperationPhase.Beginning)) source.OnBegan -= beginAction;
        if (phase.HasFlag(OperationPhase.During)) source.OnPerformed -= performAction;
        if (phase.HasFlag(OperationPhase.End)) source.OnEnd -= endAction;
    }

    public bool Equals(SubOperation other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Equals(value, other.value) && overridingPhase == other.overridingPhase;
    }
    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((SubOperation) obj);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            var hashCode = (value != null ? value.GetHashCode() : 0);
            hashCode = (hashCode * 397) ^ (int) overridingPhase;
            return hashCode;
        }
    }
}