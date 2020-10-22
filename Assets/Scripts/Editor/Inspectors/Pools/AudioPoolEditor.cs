using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AudioPool))]
[CanEditMultipleObjects]
public class AudioPoolEditor : PoolEditor<AudioSource, PoolableAudio> { }