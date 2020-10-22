using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PoolableAudio))]
[CanEditMultipleObjects]
public class PoolableAudioEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        
        var iterator = serializedObject.GetIterator();
        iterator.NextVisible(true);

        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.PropertyField(iterator);
        EditorGUI.EndDisabledGroup();
        
        iterator.NextVisible(false);
        EditorGUILayout.PropertyField(iterator);

        var audioSource = iterator.objectReferenceValue as AudioSource;
        
        iterator.NextVisible(false);
        EditorGUILayout.PropertyField(iterator);

        if (GUILayout.Button("Play") && audioSource != null) audioSource.Play();
        
        serializedObject.ApplyModifiedProperties();
    }
}