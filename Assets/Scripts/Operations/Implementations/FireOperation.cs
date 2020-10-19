using UnityEngine;

[CreateAssetMenu(fileName = "NewFireOperation", menuName = "Custom/Operations/Fire")]
public class FireOperation : Operation<Null, OperationHandler>
{
    protected override void Execute(Null args, Object[] parameters)
    {
        var pool = Repository.GetFirst<BulletPool>(parameters[0] as Token);
        var firePoint = parameters[1] as Transform;

        var bullet = pool.RequestSingle();
        
        bullet.transform.position = firePoint.position;
        bullet.Fire(source.transform.forward);
        
        Debug.DrawRay(firePoint.position, source.transform.forward * 5, Color.blue, 25f);
        
        Perform(args, parameters);
    }
}