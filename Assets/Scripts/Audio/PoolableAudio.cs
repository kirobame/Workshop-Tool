using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolableAudio : Poolable<AudioSource>
{
    public void Process(AudioSource template)
    {
        Value.clip = template.clip;
        Value.volume = template.volume;
        Value.pitch = template.pitch;
    }
}