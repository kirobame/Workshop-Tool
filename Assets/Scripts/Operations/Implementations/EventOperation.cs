using UnityEngine;

[CreateAssetMenu(fileName = "NewEventOperation", menuName = "Custom/Operations/Event")]
public class EventOperation : Operation<Null,OperationHandler>
{
    [SerializeField] private RemoteEvent[] remoteEvents;

    protected override void During(Null args, Object[] parameters) { foreach (var remoteEvent in remoteEvents) remoteEvent.Invoke(); }
}