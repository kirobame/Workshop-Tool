using UnityEngine;
using UnityEngine.Events;

public class Crate : MonoBehaviour, IHittable, IResetable
{
    public bool HasAlreadyBeenHit => hasAlreadyBeenHit;

    [SerializeField] private Token acceptedToken;
    
    [SerializeField] private UnityEvent onReset;
    [SerializeField] private UnityEvent onHit;
    
    private bool hasAlreadyBeenHit;
    
    public bool Hit(Bullet bullet, RaycastHit hit)
    {
        if (hasAlreadyBeenHit) return false;

        if (bullet.TryGetComponent<PoolableBullet>(out var poolable) && poolable.Tag == acceptedToken)
        {
            hasAlreadyBeenHit = true;
            onHit.Invoke();

            return true;
        }

        return false;
    }
    
    void IResetable.Reset()
    {
        hasAlreadyBeenHit = false;
        onReset.Invoke();
    }
}