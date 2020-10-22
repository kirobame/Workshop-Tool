using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Shape))]
public class ShapeEditor : Editor
{
    private const float occupiance = 0.9f;
    
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        
        var iterator = serializedObject.GetIterator();
        iterator.NextVisible(true);

        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.PropertyField(iterator);
        EditorGUI.EndDisabledGroup();
        
        EditorGUILayout.Separator();
        
        var rect = GUILayoutUtility.GetRect(EditorGUIUtility.currentViewWidth, EditorGUIUtility.currentViewWidth);
        EditorGUI.DrawRect(rect, new Color(0.175f,0.175f,0.175f));
        
        Handles.BeginGUI();

        Handles.color = new Color(0.3f, 0.3f, 0.3f).SetAlpha(0.15f);
        Handles.DrawSolidDisc(rect.center, Vector3.forward, rect.width * 0.5f * occupiance);

        Handles.color = new Color(0.1f, 0.1f, 0.1f);
        Handles.DrawWireDisc(rect.center, Vector3.forward, rect.width * 0.5f * occupiance);
        
        var topRight = new Vector2(rect.xMax, rect.yMin);
        var bottomLeft = new Vector2(rect.xMin, rect.yMax);
        
        var halfY =  new Vector2(0f, rect.height * 0.5f);
        Handles.DrawLine(rect.min + halfY, topRight + halfY);

        var halfX = new Vector2(rect.width * 0.5f, 0f);
        Handles.DrawLine(rect.max - halfX, topRight - halfX);
        
        Handles.DrawLine(rect.min, topRight);
        Handles.DrawLine(topRight, rect.max);
        Handles.DrawLine(rect.max, bottomLeft);
        Handles.DrawLine(bottomLeft, rect.min);

        iterator.NextVisible(false);
        Handles.color = Color.yellow;

        for (var i = 0; i < iterator.arraySize; i += 3)
        {
            var p1 = GetPosition(iterator, i, rect);

            if (i + 1 != iterator.arraySize)
            {
                var p2 = GetPosition(iterator, i + 1, rect);
                var p3 = GetPosition(iterator, i + 2, rect);
                var p4 = GetPosition(iterator, i + 3, rect);
            
                Handles.DrawBezier(p1,p4, p2,p3, Color.yellow,null, 2);
            }

            var size = rect.width * 0.005f;
            Handles.DrawSolidDisc(p1, Vector3.forward, rect.width * 0.005f);

            size *= 5f;
            var indexRect = new Rect(p1 - Vector2.one * size, Vector2.one * (size * 0.5f) + Vector2.right  * 50f + Vector2.up * (size * 0.25f));
            EditorGUI.LabelField(indexRect, Mathf.RoundToInt((float)i / 3).ToString());
        }
        Handles.EndGUI();
        
        serializedObject.ApplyModifiedProperties();
    }

    private Vector2 GetPosition(SerializedProperty arrayProperty, int index, Rect rect)
    {
        var pointProperty = arrayProperty.GetArrayElementAtIndex(index);
        pointProperty.NextVisible(true);

        var position = pointProperty.vector2Value;
        return rect.center + new Vector2(position.x, -position.y) * (rect.width * 0.5f * occupiance);
    }

    private void DisplayRuntimeData(Rect rect)
    {
        var shape = serializedObject.targetObject as Shape;
        if (shape.RuntimePoints != null)
        {
            for (var i = 0; i < shape.RuntimePoints.Count - 1; i++)
            {
                var p1 = shape.RuntimePoints[i].position;
                p1 = rect.center + new Vector2(p1.x, -p1.y) * (rect.width * 0.5f * occupiance);
                
                var p2 = shape.RuntimePoints[i + 1].position;
                p2 = rect.center + new Vector2(p2.x, -p2.y) * (rect.width * 0.5f * occupiance);
                
                Handles.color = Color.cyan;
                Handles.DrawLine(p1,p2);
                
                Handles.DrawSolidDisc(p1, Vector3.forward, rect.width * 0.005f);
                Handles.DrawSolidDisc(p2, Vector3.forward, rect.width * 0.005f);
            }
        }
    }
}