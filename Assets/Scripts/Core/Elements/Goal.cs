using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Goal : MonoBehaviour
{
    [SerializeField] private Token roomHandlerToken;
    //
    [SerializeField] private UnityEvent onSpawned;
    
    void Awake() => Enemy.OnAllKilled += Activate;
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
}