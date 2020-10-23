using UnityEngine;
using UnityEngine.Events;

// Class allowing the registration of an object into a pool without directly interfering with it. 
// The stocking of the object into the given pool is based on Unity's already given lifetime callbacks. 
public abstract class Poolable<T> : MonoBehaviour where T : Object
{
    public T Value => value;
    
    // The value targeted by this poolable
    [SerializeField] private T value;
    
    // Callback allowing more flexibility for the resetting of data for the next pull. 
    [SerializeField] private UnityEvent onReboot;
    
    private Pool<T> origin;
        
    protected virtual void OnDisable()
    {
        origin.Stock(this);
        origin = null;
    }
    
    public void SetOrigin(Pool<T> origin) => this.origin = origin;
    
    // Method used to reset any data of the targeted value. 
    public virtual void Reboot()
    {
        onReboot.Invoke();
        OnReboot();
    }
    protected virtual void OnReboot() { }
}