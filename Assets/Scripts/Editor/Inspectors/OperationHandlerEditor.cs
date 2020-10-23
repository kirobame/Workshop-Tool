using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

[CustomEditor(typeof(OperationHandler))]
[CanEditMultipleObjects]
public class OperationHandlerEditor : Editor
{
    // Drawing data
    private Color black = new Color(0.15f, 0.15f, 0.15f);
    private float size;

    void OnEnable() => size = EditorGUIUtility.singleLineHeight;

    //------------------------------------------------------------------------------------------------------------------
    
    public override void OnInspectorGUI()
    {
        // Draw base values & then draw InputLinks.
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
            //Begin the drawing chain. 
            
            DrawSeparation(Vector2.zero, 3);
            DrawLinks(iterator);
        }

        serializedObject.ApplyModifiedProperties();
    }
    
    //------------------------------------------------------------------------------------------------------------------
    
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

    // Root method for the drawing chain.
    private void DrawLinks(SerializedProperty linksProperty)
    {
         for (var i = 0; i < linksProperty.arraySize; i++)
         {
             // Get root property.
            var linkProperty = linksProperty.GetArrayElementAtIndex(i);

            var inputProperty = linkProperty.FindPropertyRelative("inputReference");
            var inputReference = inputProperty.objectReferenceValue as InputActionReference;
            
            //----------------------------------------------------------------------------------------------------------

            // Draw header with the appropriate unique name. 
            var activatorProperty = linkProperty.FindPropertyRelative("activator");
            var activator = activatorProperty.objectReferenceValue as Activator;
            
            var name = inputReference == null ? "Null" : inputReference.name;
            if (activator != null) name += $"-{activator.ShortName}";
            else name += "-N";    
            
            linkProperty.isExpanded = EditorGUILayout.Foldout(linkProperty.isExpanded, new GUIContent(name));

            //----------------------------------------------------------------------------------------------------------
            
            // Allow the deletion of the link.
            var layoutRect = GUILayoutUtility.GetLastRect();
            layoutRect.width -= EditorGUIUtility.labelWidth;
            layoutRect.x += EditorGUIUtility.labelWidth;
            
            if (GUI.Button(layoutRect, new GUIContent("Delete Link")))
            {
                linksProperty.DeleteArrayElementAtIndex(i);
                break;
            }
            
            //----------------------------------------------------------------------------------------------------------

            // If not expanded skip.
            if (!linkProperty.isExpanded)
            {
                EndDrawLink(linksProperty, i);
                continue;
            }
            EditorGUI.indentLevel++;

            //----------------------------------------------------------------------------------------------------------
            
            // Draw the main values : InputAction & Activator.
            EditorGUILayout.PropertyField(inputProperty, new GUIContent("Reference"));
            
            var operandsProperty = linkProperty.FindPropertyRelative("operands");
            EditorGUILayout.BeginHorizontal();
            
            EditorGUILayout.PropertyField(activatorProperty);
            if (GUILayout.Button("Add",GUILayout.Width(40)))
            {
                if (operandsProperty.arraySize == 0) operandsProperty.InsertArrayElementAtIndex(0);
                else operandsProperty.InsertArrayElementAtIndex(operandsProperty.arraySize - 1);
            }
            EditorGUILayout.EndHorizontal();
            
            //----------------------------------------------------------------------------------------------------------

            // If there are no operands skip.
            if (operandsProperty.arraySize == 0)
            {
                EndDrawLink(linksProperty, i);
                continue;
            }
            
            operandsProperty.isExpanded = EditorGUILayout.Foldout(operandsProperty.isExpanded, new GUIContent("Operands"));
            DrawSeparation(new Vector2(80f,0f), size * 0.5f, false);

            if (!operandsProperty.isExpanded)
            {
                EndDrawLink(linksProperty, i);
                continue;
            }
            EditorGUI.indentLevel++;
            
            // Prepare layout Rect for use by Operands.
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
    // Skip method.
    private void EndDrawLink(SerializedProperty linksProperty, int index)
    {
        EditorGUI.indentLevel = 0;
        if (index < linksProperty.arraySize - 1) DrawSeparation(Vector2.zero, 2);
    }
    
    private Rect DrawOperands(SerializedProperty operandsProperty, Rect layoutRect)
    {
        for (var i = 0; i < operandsProperty.arraySize; i++)
        {
            // Get root property.
            var operandProperty = operandsProperty.GetArrayElementAtIndex(i);
            var operationProperty = operandProperty.FindPropertyRelative("operation");
                
            //----------------------------------------------------------------------------------------------------------
            
            // Draw root property & allow its removal.
            EditorGUILayout.BeginHorizontal();
            
            EditorGUILayout.PropertyField(operationProperty, GUIContent.none);
            if (GUILayout.Button("Remove", GUILayout.Width(60)))
            {
                operandsProperty.DeleteArrayElementAtIndex(i);
                break;
            }
            EditorGUILayout.EndHorizontal();

            //-----------------------------------------------------------------------------------

            // Display the index of the current Operand for better dissociation multiple ones. 
            var indexRect = layoutRect;
            GUI.Button(indexRect, new GUIContent($"- {i.ToString()}"), EditorStyles.label);
            layoutRect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            
            if (operationProperty.objectReferenceValue == null)
            {
                End();
                continue;
            }
            
            // Gets needed attribute to draw parameters.
            // Allows to get a name & type for each objectField with each being associated to one parameter.
            var typesAttribute = operationProperty.objectReferenceValue.GetType().GetCustomAttribute<OperationParameterTypesAttribute>();
            var namesAttribute = operationProperty.objectReferenceValue.GetType().GetCustomAttribute<OperationParameterNamesAttribute>();
            
            if (typesAttribute == null || namesAttribute == null)
            {
                End();
                continue;
            }
            
            // Get parameters & Draw them.
            var parametersProperty = operandProperty.FindPropertyRelative("parameters");
            parametersProperty.arraySize = typesAttribute.Types.Count;
            layoutRect = DrawParameters(parametersProperty, layoutRect, typesAttribute.Types, namesAttribute.Names);

            //----------------------------------------------------------------------------------------------------------
            
            End();
            void End()
            {
                if (i < operandsProperty.arraySize - 1)
                {
                    DrawSeparation(new Vector2(30f,20f), 2f);
                    layoutRect.y += 8;
                }
            }
        }

        return layoutRect;
    }
    private Rect DrawParameters(SerializedProperty parametersProperty, Rect layoutRect, IReadOnlyList<Type> types, IReadOnlyList<string> names)
    {
        for (var i = 0; i < parametersProperty.arraySize; i++)
        {
            var parameterProperty = parametersProperty.GetArrayElementAtIndex(i);
            layoutRect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

            //----------------------------------------------------------------------------------------------------------

            var guiContent = new GUIContent(names[i]);
            parameterProperty.objectReferenceValue = EditorGUILayout.ObjectField(guiContent, parameterProperty.objectReferenceValue, types[i], true);
        }

        return layoutRect;
    }
}