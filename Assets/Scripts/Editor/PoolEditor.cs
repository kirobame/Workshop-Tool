using UnityEditor;
using UnityEngine;

public class PoolEditor<T,TPoolable> : Editor where T : Object where TPoolable : Poolable<T>
{
    public override void OnInspectorGUI()
    {
        serializedObject.UpdateIfRequiredOrScript();

        var iterator = serializedObject.GetIterator();
        iterator.NextVisible(true);

        GUI.enabled = false;
        EditorGUILayout.PropertyField(iterator);
        GUI.enabled = true;
        
        iterator.NextVisible(false);
        EditorGUILayout.PropertyField(iterator);
        
        iterator.NextVisible(false);
        GUI.enabled = false;
        EditorGUILayout.PropertyField(iterator);
        GUI.enabled = true;

        if (GUILayout.Button("Fetch"))
        {
            iterator.ClearArray();
            
            var source = ((Pool)target).gameObject;
            foreach (var poolable in source.GetComponentsInChildren<TPoolable>(true))
            {
                if (poolable == null) continue;
                
                iterator.InsertArrayElementAtIndex(0);

                var poolableProperty = iterator.GetArrayElementAtIndex(0);
                poolableProperty.objectReferenceValue = poolable;
            }
        }
        
        serializedObject.ApplyModifiedProperties();
    }
}