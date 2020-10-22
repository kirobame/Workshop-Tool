using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Goal : MonoBehaviour, IResetable
{
    [SerializeField] private Token roomHandlerToken;

    [SerializeField] private UnityEvent onReset;
    [SerializeField] private UnityEvent onSpawned;
    
    void OnDisable() => Enemy.OnAllKilled -= Activate;

    private void Activate() => onSpawned.Invoke();
    
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            var roomHandler = Repository.GetFirst<RoomHandler>(roomHandlerToken);
            roomHandler.ActivateNext();
        }
    }
    
    void IResetable.Reset()
    {
        Enemy.OnAllKilled += Activate;
        onReset.Invoke();
    }
}