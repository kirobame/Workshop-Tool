using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Goal : MonoBehaviour, IResetable
{
    [SerializeField] private Token roomHandlerToken;
    [SerializeField] private AudioEffect soundEffect;

    [SerializeField] private UnityEvent onReset;
    [SerializeField] private UnityEvent onSpawned;

    private bool state = true;
    
    void OnDisable() => Enemy.OnAllKilled -= Activate;

    private void Activate() => onSpawned.Invoke();
    
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && state)
        {
            var roomHandler = Repository.GetFirst<RoomHandler>(roomHandlerToken);
            roomHandler.ActivateNext();

            soundEffect.Play(0);
            
            state = false;
        }
    }
    
    void IResetable.Reset()
    {
        state = true;
        
        Enemy.OnAllKilled += Activate;
        onReset.Invoke();
    }
}