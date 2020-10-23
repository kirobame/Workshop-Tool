using System;
using UnityEngine;
using Object = UnityEngine.Object;

[Serializable]
// Container class allowing ease of use for Operation chaining. 
public class SubOperation : IEquatable<SubOperation>
{
    public SubOperation(Operation source, OperationPhase overridingPhase = OperationPhase.None)
    {
        value = source;
        this.overridingPhase = this.overridingPhase;
    }

    //------------------------------------------------------------------------------------------------------------------
    
    public Operation Value => runtimeValue;
    
    // The contained value.
    [SerializeField] private Operation value;
    
    // Supplementary data allowing to call the targeted Operation differently than in the way it was originally set up.
    [SerializeField] private OperationPhase overridingPhase;

    // Cached callbacks for easy subscription behaviours.
    private Action<object, Object[]> beginAction;
    private Action<object, Object[]> performAction;
    private Action<object, Object[]> endAction;
    
    // Runtime representation to avoid any data corruption.
    private Operation runtimeValue;
        
    //------------------------------------------------------------------------------------------------------------------
    
    // Chaining methods based on the listened phases or the Overriding phases for this Operation instance.
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

    //------------------------------------------------------------------------------------------------------------------
    
    // IEquatable implementation. 
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