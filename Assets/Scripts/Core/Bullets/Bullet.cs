using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float lifetime;
    [SerializeField] private float disappearanceTime;
    [SerializeField] private float speed;
    [SerializeField] private LayerMask collisionMask;

    [Space, SerializeField] private AudioEffect soundEffect;
    
    [SerializeField] private UnityEvent onFire;
    [SerializeField] private HitEvent onHit;

    private Coroutine lifetimeRoutine;
    private bool hasHit;
    
    public void Fire(Vector3 direction)
    {
        transform.rotation = Quaternion.LookRotation(direction);
        lifetimeRoutine = StartCoroutine(KillRoutine(lifetime));
        
        onFire.Invoke();
    }

    void OnDisable()
    {
        if (lifetimeRoutine == null) return;
        
        StopCoroutine(lifetimeRoutine);
        lifetimeRoutine = null;
    }
    
    void FixedUpdate()
    {
        if (hasHit) return;
        
        var delta = transform.forward * (Time.fixedDeltaTime * speed);
        var ray = new Ray(transform.position, delta);
        
        if (Physics.Raycast(ray, out var hit, Time.fixedDeltaTime * speed, collisionMask, QueryTriggerInteraction.Ignore))
        {
            StopCoroutine(lifetimeRoutine);
            lifetimeRoutine = null;

            StartCoroutine(KillRoutine(disappearanceTime));
            soundEffect.Play(0);
            
            var hittable = hit.transform.GetComponent<IHittable>();
            hittable?.Hit(this, hit);

            onHit.Invoke(hit);
            transform.position = hit.point;

            hasHit = true;
        }
        else transform.position += delta;
    }

    private IEnumerator KillRoutine(float time)
    {
        yield return new WaitForSeconds(time);
        
        gameObject.SetActive(false);
        hasHit = false;
    }
}