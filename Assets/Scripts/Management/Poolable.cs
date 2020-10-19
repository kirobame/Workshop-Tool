using UnityEngine;
using UnityEngine.Events;

public abstract class Poolable<T> : MonoBehaviour where T : Object
{
    public T Value => value;
    
    [SerializeField] private T value;
    [SerializeField] private UnityEvent onReboot;
    
    private Pool<T> origin;
        
    protected virtual void OnDisable()
    {
        origin.Stock(this);
        origin = null;
    }
    
    public void SetOrigin(Pool<T> origin) => this.origin = origin;
    
    public void Reboot()
    {
        onReboot.Invoke();
        OnReboot();
    }
    protected virtual void OnReboot() { }
}