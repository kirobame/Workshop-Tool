using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "NewFireOperation", menuName = "Custom/Operations/Fire")]
[OperationParameterTypes(typeof(Transform))]
[OperationParameterNames("Fire Point")]
public class FireOperation : Operation<Null, OperationHandler>
{
    [SerializeField] private Token bulletPoolToken;
    [SerializeField] private PoolableBullet bulletPrefab;
    [SerializeField] private MuzzleFlashEffect muzzleFlashEffect;
    [SerializeField] private float angleOffset;

    [Space, SerializeField] private AudioEffect soundEffect;

    protected override void During(Null args, Object[] parameters)
    {
        var pool = Repository.GetFirst<BulletPool>(bulletPoolToken);
        var firePoint = parameters[0] as Transform;

        var bullets = pool.RequestSpecific(bulletPrefab, 1, poolable => poolable.Tag == bulletPrefab.Tag);
        var bullet = bullets.First();
        
        bullet.transform.position = firePoint.position;
        var direction = Quaternion.AngleAxis(angleOffset, Vector3.up) * source.transform.forward;
        bullet.Fire(direction);
        
        soundEffect.Play(0);
        if (muzzleFlashEffect.TryGetMuzzleFlash(out var muzzleFlash))
        {
            muzzleFlash.transform.position = firePoint.position;
            muzzleFlash.transform.rotation = firePoint.rotation;
            muzzleFlash.Play();
        }
    }
}