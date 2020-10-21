using UnityEngine;

public abstract class MuzzleFlashEffect : ScriptableObject
{
    public abstract bool TryGetMuzzleFlash(out ParticleSystem effect);
}