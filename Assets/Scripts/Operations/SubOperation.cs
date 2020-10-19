using System;
using UnityEngine;
using Object = UnityEngine.Object;

[Serializable]
public class SubOperation : IEquatable<SubOperation>
{
    public SubOperation(Operation source, OperationPhase activationPhase)
    {
        value = source;
        this.activationPhase = activationPhase;
    }
    
    [SerializeField] private Operation value;
    [SerializeField] private OperationPhase activationPhase;

    private Action<object, Object[]> action;
    private Operation runtimeValue;
        
    public void Link(Operation source)
    {
        action = (args, parameters) => runtimeValue.Execute(args, parameters);
        runtimeValue = Object.Instantiate(value);
            
        switch (activationPhase)
        {
            case OperationPhase.Beginning:
                source.OnBeginning += action;
                break;
                
            case OperationPhase.During:
                source.OnPerformed += action;
                break;
                
            case OperationPhase.End:
                source.OnEnd += action;
                break;
        }
    }
    public void BreakLinkage(Operation source)
    {
        switch (activationPhase)
        {
            case OperationPhase.Beginning:
                source.OnBeginning -= action;
                break;
                
            case OperationPhase.During:
                source.OnPerformed -= action;
                break;
                
            case OperationPhase.End:
                source.OnEnd -= action;
                break;
        }
    }

    public bool Equals(SubOperation other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Equals(value, other.value) && activationPhase == other.activationPhase;
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
            hashCode = (hashCode * 397) ^ (int) activationPhase;
            return hashCode;
        }
    }
}