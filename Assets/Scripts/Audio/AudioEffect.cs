using System;
using UnityEngine;

[Serializable]
public class AudioEffect
{
    [SerializeField] private Token audioPoolToken;
    [SerializeField] private PoolableAudio[] templates;

    public void Play(int index)
    {
        var audioPool = Repository.GetFirst<AudioPool>(audioPoolToken);
        
        var audioSource = audioPool.RequestSingle();
        audioSource.GetComponent<PoolableAudio>().Process(templates[index].Value);
        
        audioSource.Play();
    }
}