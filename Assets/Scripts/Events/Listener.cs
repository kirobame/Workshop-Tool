using UnityEngine;
using UnityEngine.Events;

public class Listener : MonoBehaviour
{
    [SerializeField] private RemoteEvent[] remoteEvents;
    [SerializeField] private UnityEvent callback;

    private void OnEnable() { foreach (var remoteEvent in remoteEvents) remoteEvent.action += callback.Invoke; }
    private void OnDisable() { foreach (var remoteEvent in remoteEvents) remoteEvent.action -= callback.Invoke; }
}