using UnityEngine;

[CreateAssetMenu(fileName = "NewFireOperation", menuName = "Custom/Operations/Fire")]
public class FireOperation : Operation<Null, OperationHandler>
{
    protected override void Execute(Null args, Object[] parameters)
    {
        var pool = Repository.GetFirst<BulletPool>(parameters[0] as Token);
        var firePoint = parameters[1] as Transform;
        
        var remoteEvent = parameters[2] as RemoteEvent;
        remoteEvent.Invoke();

        var bullet = pool.RequestSingle();

        bullet.transform.position = firePoint.position;
        bullet.Fire(source.transform.forward);
      
        Perform(args, parameters);
    }
}