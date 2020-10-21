using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "NewStandardMuzzleFlash", menuName = "Custom/Effects/Muzzle Flashes/Standard")]
public class StandardMuzzleFlash : MuzzleFlashEffect
{
    [SerializeField] private Token effectPoolToken;
    [SerializeField] private PoolableEffect muzzleFlashPrefab;

    public override bool TryGetMuzzleFlash(out ParticleSystem effect)
    {
        var effectPool = Repository.GetFirst<EffectPool>(effectPoolToken);
        var results = effectPool.RequestSpecific(muzzleFlashPrefab, 1, item => item.Tag == muzzleFlashPrefab.Tag);

        effect = results.First();
        return true;
    }
}