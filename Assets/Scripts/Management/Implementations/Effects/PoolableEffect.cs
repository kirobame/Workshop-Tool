using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolableEffect : Poolable<ParticleSystem>
{
    public Token Tag => tag;
    [SerializeField] private Token tag;
}