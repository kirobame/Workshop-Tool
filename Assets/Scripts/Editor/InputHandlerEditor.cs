using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

[CustomEditor(typeof(InputHandler))]
[CanEditMultipleObjects]
public class InputHandlerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.UpdateIfRequiredOrScript();
        
        var iterator = serializedObject.GetIterator();
        iterator.NextVisible(true);

        GUI.enabled = false;
        EditorGUILayout.PropertyField(iterator);
        GUI.enabled = true;
        
        EditorGUI.BeginChangeCheck();
        
        iterator.NextVisible(false);
        EditorGUILayout.PropertyField(iterator);

        var assetHasChanged = false;
        if (EditorGUI.EndChangeCheck())
        {
            assetHasChanged = true;
            var asset = iterator.objectReferenceValue as InputActionAsset;
            
            iterator.NextVisible(false);
            iterator.ClearArray();

            if (asset != null)
            {
                foreach (var actionMap in asset.actionMaps)
                {
                    iterator.InsertArrayElementAtIndex(iterator.arraySize);
                    
                    var linkProperty = iterator.GetArrayElementAtIndex(iterator.arraySize - 1);
                    var nameProperty = linkProperty.FindPropertyRelative("name");

                    nameProperty.stringValue = actionMap.name;
                }
            }
        }
        
        if (!assetHasChanged) iterator.NextVisible(false);

        for (var i = 0; i < iterator.arraySize; i++)
        {
            var linkProperty = iterator.GetArrayElementAtIndex(i);
            var nameProperty = linkProperty.FindPropertyRelative("name");
            var stateProperty = linkProperty.FindPropertyRelative("state");

            EditorGUILayout.PropertyField(stateProperty, new GUIContent(nameProperty.stringValue));
        }

        serializedObject.ApplyModifiedProperties();
    }
}
