using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(EffectPool))]
[CanEditMultipleObjects]
public class EffectPoolEditor : PoolEditor<ParticleSystem, PoolableEffect> { }