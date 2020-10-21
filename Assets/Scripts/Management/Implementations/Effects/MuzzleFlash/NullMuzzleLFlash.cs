using UnityEngine;

[CreateAssetMenu(fileName = "NeNullMuzzleFlash", menuName = "Custom/Effects/Muzzle Flashes/Null")]
public class NullMuzzleLFlash : MuzzleFlashEffect
{
    public override bool TryGetMuzzleFlash(out ParticleSystem effect)
    {
        effect = null;
        return false;
    }
}