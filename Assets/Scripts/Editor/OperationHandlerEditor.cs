using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

[CustomEditor(typeof(OperationHandler))]
[CanEditMultipleObjects]
public class OperationHandlerEditor : Editor
{
    private Color black = new Color(0.15f, 0.15f, 0.15f);
    private float size;

    void OnEnable()
    {
        size = EditorGUIUtility.singleLineHeight;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.UpdateIfRequiredOrScript();

        var iterator = serializedObject.GetIterator();
        iterator.NextVisible(true);

        GUI.enabled = false;
        EditorGUILayout.PropertyField(iterator);
        GUI.enabled = true;

        iterator.NextVisible(false);
        if (GUILayout.Button("Create New Link"))
        {
            if (iterator.arraySize == 0) iterator.InsertArrayElementAtIndex(0);
            else iterator.InsertArrayElementAtIndex(iterator.arraySize - 1);
        }

        if (iterator.arraySize > 0)
        {
            DrawSeparation(Vector2.zero, 3);
            DrawLinks(iterator);
        }

        serializedObject.ApplyModifiedProperties();
    }

    private void DrawSeparation(Vector2 margins, float yAddition = 4f, bool withSeparation = true)
    {
        if (withSeparation) EditorGUILayout.Separator();
        
        var lastRect = GUILayoutUtility.GetLastRect();
        lastRect.height = 1;
        lastRect.y += yAddition;

        lastRect.width -= margins.x;
        lastRect.x += margins.x;

        lastRect.width -= margins.y;

        EditorGUI.DrawRect(lastRect, black);
    }

    private void DrawLinks(SerializedProperty linksProperty)
    {
         for (var i = 0; i < linksProperty.arraySize; i++)
         {
            var linkProperty = linksProperty.GetArrayElementAtIndex(i);

            var inputProperty = linkProperty.FindPropertyRelative("inputReference");
            var inputReference = inputProperty.objectReferenceValue as InputActionReference;
            
            //----------------------------------------------------------------------------------------------------------
            
            var name = inputReference == null ? "Null" : inputReference.name;
            linkProperty.isExpanded = EditorGUILayout.Foldout(linkProperty.isExpanded, new GUIContent(name));

            //----------------------------------------------------------------------------------------------------------
            
            var layoutRect = GUILayoutUtility.GetLastRect();
            layoutRect.width -= EditorGUIUtility.labelWidth;
            layoutRect.x += EditorGUIUtility.labelWidth;
            
            if (GUI.Button(layoutRect, new GUIContent("Delete Link")))
            {
                linksProperty.DeleteArrayElementAtIndex(i);
                break;
            }
            
            //----------------------------------------------------------------------------------------------------------

            if (!linkProperty.isExpanded)
            {
                EndDrawLink(linksProperty, i);
                continue;
            }
            EditorGUI.indentLevel++;

            //----------------------------------------------------------------------------------------------------------
            
            EditorGUILayout.PropertyField(inputProperty, new GUIContent("Reference"));
            
            var operandsProperty = linkProperty.FindPropertyRelative("operands");
            EditorGUILayout.BeginHorizontal();
            
            EditorGUILayout.PropertyField(linkProperty.FindPropertyRelative("activator"));
            if (GUILayout.Button("Add",GUILayout.Width(40)))
            {
                if (operandsProperty.arraySize == 0) operandsProperty.InsertArrayElementAtIndex(0);
                else operandsProperty.InsertArrayElementAtIndex(operandsProperty.arraySize - 1);
            }
            EditorGUILayout.EndHorizontal();
            
            //----------------------------------------------------------------------------------------------------------

            if (operandsProperty.arraySize == 0)
            {
                EndDrawLink(linksProperty, i);
                continue;
            }
            
            operandsProperty.isExpanded = EditorGUILayout.Foldout(operandsProperty.isExpanded, new GUIContent("Operands"));
            DrawSeparation(new Vector2(80f,0f), size * 0.5f, false);
            
            if (!operandsProperty.isExpanded) return;
            EditorGUI.indentLevel++;
            
            layoutRect = GUILayoutUtility.GetLastRect();
            layoutRect.width = size;
            layoutRect.x += 4;
            
            layoutRect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            layoutRect.height = size;

            //----------------------------------------------------------------------------------------------------------
            
            DrawOperands(operandsProperty, layoutRect);
            EndDrawLink(linksProperty, i);
         }
    }
    private void EndDrawLink(SerializedProperty linksProperty, int index)
    {
        EditorGUI.indentLevel = 0;
        if (index < linksProperty.arraySize - 1) DrawSeparation(Vector2.zero, 2);
    }
    
    private Rect DrawOperands(SerializedProperty operandsProperty, Rect layoutRect)
    {
        for (var i = 0; i < operandsProperty.arraySize; i++)
        {
            var operandProperty = operandsProperty.GetArrayElementAtIndex(i);
            var operationProperty = operandProperty.FindPropertyRelative("operation");
                
            //----------------------------------------------------------------------------------------------------------
            
            EditorGUILayout.BeginHorizontal();
            
            EditorGUILayout.PropertyField(operationProperty, GUIContent.none);
            if (GUILayout.Button("Remove", GUILayout.Width(60)))
            {
                operandsProperty.DeleteArrayElementAtIndex(i);
                break;
            }
            
            var parametersProperty = operandProperty.FindPropertyRelative("parameters");
            if (GUILayout.Button(EditorGUIUtility.IconContent("CreateAddNew@2x"), EditorStyles.label,GUILayout.Width(size), GUILayout.Height(size)))
            {
                parametersProperty.InsertArrayElementAtIndex(0);
            }
            EditorGUILayout.EndHorizontal();

            //-----------------------------------------------------------------------------------

            var indexRect = layoutRect;
            GUI.Button(indexRect, new GUIContent($"- {i.ToString()}"), EditorStyles.label);
            layoutRect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            layoutRect = DrawParameters(parametersProperty, layoutRect);

            //----------------------------------------------------------------------------------------------------------
            
            if (i < operandsProperty.arraySize - 1)
            {
                DrawSeparation(new Vector2(30f,20f), 2f);
                layoutRect.y += 8;
            }
        }

        return layoutRect;
    }
    
    private Rect DrawParameters(SerializedProperty parametersProperty, Rect layoutRect)
    {
        for (var i = 0; i < parametersProperty.arraySize; i++)
        {
            var parameterProperty = parametersProperty.GetArrayElementAtIndex(i);
            EditorGUILayout.BeginHorizontal();

            var textureRect = layoutRect;
            textureRect.position += Vector2.one * 2;
            textureRect.max -= Vector2.one * 2;
            GUI.DrawTexture(textureRect, EditorGUIUtility.IconContent("UnityEditor.VersionControl").image, ScaleMode.StretchToFill, true);

            //----------------------------------------------------------------------------------------------------------
            
            DrawParameterMover(parametersProperty, layoutRect, -1, i, i - 1 >= 0);
            DrawParameterMover(parametersProperty, layoutRect, 1, i, i + 1 < parametersProperty.arraySize);
                    
            layoutRect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

            //----------------------------------------------------------------------------------------------------------
            
            EditorGUILayout.PropertyField(parameterProperty, GUIContent.none);
            if (GUILayout.Button(EditorGUIUtility.IconContent("winbtn_win_close@2x"), EditorStyles.label, GUILayout.Width(size), GUILayout.Height(size)))
            {
                parameterProperty.objectReferenceValue = null;
                parametersProperty.DeleteArrayElementAtIndex(i);
            }
            EditorGUILayout.EndHorizontal();
        }

        return layoutRect;
    }
    private void DrawParameterMover(SerializedProperty parametersProperty, Rect layoutRect, int direction, int index, bool canBeExecuted)
    {
        var halfWidth = layoutRect.width * 0.5f;
        
        var position = layoutRect.position + halfWidth * Mathf.Clamp01(direction) * Vector2.right;
        var upRect = new Rect(position, new Vector2(halfWidth, layoutRect.height));
        
        if (GUI.Button(upRect, GUIContent.none, EditorStyles.label) && canBeExecuted)
        {
            parametersProperty.MoveArrayElement(index, index + direction);
        }
    }
}