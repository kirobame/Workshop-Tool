using UnityEngine;
using UnityEngine.Events;

public abstract class Poolable<T> : MonoBehaviour where T : Object
{
    public T Value => value;

    [SerializeField] private UnityEvent onReboot;
    [SerializeField] private T value;
    
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