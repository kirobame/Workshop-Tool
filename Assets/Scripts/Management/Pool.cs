using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

// Based empty class allowing Editor Serialization + Display in the Inspector.
public abstract class Pool : MonoBehaviour { }

// Simple generic implementation to allow Poolable to call the Stock method.
public abstract class Pool<T> : Pool where T : Object
{
    public abstract void Stock(Poolable<T> poolable);
}

// True generic implementation allowing to return when asked the targeted Type & not the Poolable Type.
public abstract class Pool<T, TPoolable> : Pool<T> where T : Object where TPoolable : Poolable<T>
{
    // The base prefab to be instantiated in case of lack of resource. 
    [SerializeField] private TPoolable prefab;
    
    // All already cached instances directly ready for use. 
    [SerializeField] private List<TPoolable> instances = new List<TPoolable>();
    
    private Queue<TPoolable> availableInstances = new Queue<TPoolable>();
    private HashSet<TPoolable> usedInstances = new HashSet<TPoolable>();
    
    //------------------------------------------------------------------------------------------------------------------
    
    void Awake()
    {
        instances.RemoveAll(instance => instance == null);
        foreach (var instance in instances) availableInstances.Enqueue(instance);
    }
    
    //------------------------------------------------------------------------------------------------------------------
    
    // Simple request method for an instance. Spawns one if no instance are actually available.
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
    
    //------------------------------------------------------------------------------------------------------------------
    
    // Requests a specific instance which needs to answer to a predicate.
    // If no matching instance is found, the passed prefab will be instantiated as it should answer to the predicate.
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
    
    //------------------------------------------------------------------------------------------------------------------
    
    // Stocking method allowing the resetting of a Poolable & its targeted instance. 
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
    
    //------------------------------------------------------------------------------------------------------------------
    
    // Readies a poolable for use.
    private void Claim(TPoolable poolable)
    {
        poolable.SetOrigin(this);
            
        poolable.gameObject.SetActive(true);
        poolable.transform.SetParent(null);
            
        usedInstances.Add(poolable);
    }
}