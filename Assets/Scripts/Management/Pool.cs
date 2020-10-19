using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

public abstract class Pool : MonoBehaviour { }
public abstract class Pool<T> : Pool where T : Object
{
    public abstract void Stock(Poolable<T> poolable);
}
public abstract class Pool<T, TPoolable> : Pool<T> where T : Object where TPoolable : Poolable<T>
{
    [SerializeField] private TPoolable prefab;
    [SerializeField] private List<TPoolable> instances = new List<TPoolable>();
    
    private Queue<TPoolable> availableInstances = new Queue<TPoolable>();
    private HashSet<TPoolable> usedInstances = new HashSet<TPoolable>();
    
    void Awake()
    {
        instances.RemoveAll(instance => instance == null);
        foreach (var instance in instances) availableInstances.Enqueue(instance);
    }
    
    public T RequestSingle() => Request(1).First();
    public T[] Request(int count)
    {
        for (var i = 0; i < count - availableInstances.Count; i++)
        {
            var instance = Instantiate(prefab, transform);

            instances.Add(instance);
            availableInstances.Enqueue(instance);
        }
        
        var request = new T[count];
        for (var i = 0; i < count; i++)
        {
            var instance = availableInstances.Dequeue();
                
            Claim(instance);
            request[i] = instance.Value;
        }

        return request;
    }
    
    public T[] RequestSpecific(int count, Func<TPoolable, bool> predicate) => RequestSpecific(prefab, count, predicate);
    public T[] RequestSpecific(TPoolable prefab, int count, Func<TPoolable, bool> predicate)
    {
        var request = new T[count];
        var index = 0;

        var toRequeue = new List<TPoolable>();
        while (availableInstances.Count > 0 && index < count)
        {
            var instance = availableInstances.Dequeue();
                
            if (predicate(instance) == true)
            {
                Claim(instance);
                request[index] = instance.Value;
                    
                index++;
            }
            else toRequeue.Add(instance);
        }

        foreach (var instance in toRequeue) availableInstances.Enqueue(instance);

        for (var i = 0; i < count - index; i++)
        {
            var instance = Instantiate(prefab);
                
            instance.gameObject.SetActive(true);
            instance.SetOrigin(this);
                
            instances.Add(instance);
            usedInstances.Add(instance);

            request[index + i] = instance.Value;
        }

        return request;
    }
    
    public override void Stock(Poolable<T> poolable) => Stock(poolable as TPoolable);
    public void Stock(TPoolable poolable)
    {
        if (this == null || !gameObject.activeInHierarchy) return;

        poolable.Reboot();
        poolable.gameObject.SetActive(false);
        
        StartCoroutine(ParentingRoutine(poolable));

        availableInstances.Enqueue(poolable);
        if (!usedInstances.Remove(poolable)) instances.Add(poolable);
    }
    private IEnumerator ParentingRoutine(TPoolable poolable)
    {
        yield return new WaitForEndOfFrame();
        poolable.transform.SetParent(transform);
    }
    
    private void Claim(TPoolable poolable)
    {
        poolable.SetOrigin(this);
            
        poolable.gameObject.SetActive(true);
        poolable.transform.SetParent(null);
            
        usedInstances.Add(poolable);
    }
}