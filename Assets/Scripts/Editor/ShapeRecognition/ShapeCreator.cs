using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ShapeCreator : EditorWindow
{
    private const int desiredSize = 12;
    private const int subDivision = 5;
    private const int areaRadius = 5;

    private const int bezierWidth = 2;
    
    private static Shape shape;

    private readonly Color[] backgroundShades = new Color[]
    {
        new Color(0.1647059f, 0.1647059f, 0.1647059f),
        new Color(0.2131132f, 0.2131132f, 0.2131132f),
    };
    private readonly Color[] lineShades = new Color[]
    {
        new Color(0.0868039f, 0.0868039f, 0.0868039f),
        new Color(0.1558039f, 0.1558039f, 0.1558039f),
    };
    private readonly Color[] bezierShades = new Color[]
    {
        Color.yellow,
        Color.red
    };
    
    private readonly Vector4 padding = new Vector4(4,4,4,4);
    
    [MenuItem("Tools/Custom/Shape Creator")]
    static void OpenWindow()
    {
        GetWindow<ShapeCreator>().Show();
        shape = CreateInstance<Shape>();
    }

    private Texture gridTexture;

    private Rect area;
    private Vector2Int size;
    private float unit;
    private float radius;

    private List<int> selection = new List<int>();
    private bool isDragging;

    void OnEnable()
    {
        gridTexture = Resources.Load<Texture2D>("Grid");
        Debug.Log(gridTexture);
    }

    void OnGUI()
    {
        Setup();
        EditorGUI.DrawRect(area, backgroundShades[0]);

        var size = area.width < area.height ? area.width : area.height;
        var grid = new Vector2Int(Mathf.CeilToInt(area.width / size), Mathf.CeilToInt(area.height / size));

        var step = Vector2.one * size;
        var start = area.center - grid * step * 0.5f;

        for (var x = 0; x < grid.x; x++)
        {
            for (var y = 0; y < grid.y; y++)
            {
                var position = start + new Vector2(x,y) * step;
                var rect = new Rect(position, step);
                
                GUI.DrawTexture(rect, gridTexture);
            }   
        }
        
        
        
        Handles.BeginGUI();
        
        Handles.color = backgroundShades[1].SetAlpha(0.25f);
        //Handles.DrawSolidDisc(area.center, Vector3.forward, radius);

        var hardColor = new Color(0.04f, 0.04f, 0.04f);
        Handles.color = hardColor;
        Handles.DrawWireDisc(area.center, Vector3.forward, radius);

        DrawBezier();
        DrawHandles();

        var Ev = Event.current;
        if (Ev.isMouse)
        {
            if (Ev.type == EventType.MouseDrag)
            {
                foreach (var point in selection) shape.MovePoint(point, new Vector2(Ev.delta.x, -Ev.delta.y) / radius);
                
                Repaint();
                Ev.Use();
            }

            if (Ev.button == 0 && Ev.type == EventType.MouseUp)
            {
                Debug.Log("Clearing selection");
                selection.Clear();
                
                Ev.Use();
            }
        }
        
        Handles.EndGUI();
    }

    private void Setup()
    {
        area = this.position;
        area.position = Vector2.zero;
        
        area.x += padding.x;
        area.width -= padding.y * 2;
        area.y += padding.z;
        area.height -= padding.w * 2;

        var totalSize = desiredSize * subDivision;
        unit = area.width > area.height ? area.height / totalSize : area.width / totalSize;

        var width = Mathf.RoundToInt(area.width / unit);
        var height = Mathf.RoundToInt(area.height / unit);
        size = new Vector2Int(width, height);

        radius = areaRadius * subDivision * unit;
    }

    #region Background Drawings

    private void DrawGrid()
    {
        var xOffset = size.x % 2 != 0 ? unit * 0.5f : 0f;
        var yOffset = size.y % 2 != 0 ? unit * 0.5f : 0f;
        var offset = new Vector2(xOffset, yOffset);

        var halfWidth = Mathf.FloorToInt(size.x * 0.5f);
        var halfHeight = Mathf.FloorToInt(size.y * 0.5f);
        var index = new Vector2Int(-halfWidth, halfHeight);

        for (var x = -size.x * 0.5f; x < size.x * 0.5f + 1; x++)
        {
            for (var y = -size.y * 0.5f; y < size.y * 0.5f + 1; y++)
            {
                var position = area.center + new Vector2(x, y) * unit + offset;

                DrawVerticalLine(x, position, index);
                DrawHorizontalLine(y, position, index);

                index.y++;
            }

            index.y = 0;
            index.x++;
        }
    }
    private void DrawBorders(Color color)
    {
        var bottomRight = new Vector2(area.xMax, area.yMin);
        var topLeft = new Vector2(area.xMin, area.yMax);

        Handles.color = color;
        
        Handles.DrawLine(area.min, bottomRight);
        Handles.DrawLine(bottomRight, area.max);
        Handles.DrawLine(area.max, topLeft);
        Handles.DrawLine(topLeft, area.min);
    }

    private void DrawVerticalLine(float x, Vector2 position, Vector2Int index)
    {
        if (position.x < area.min.x || position.x > area.max.x) return;
        var lineShadeIndex = Mathf.Abs(index.x) % subDivision == 0 ? 0 : 1;

        position.y = area.center.y;
        if (x > -areaRadius * subDivision && x < areaRadius * subDivision)
        {
            Handles.color = lineShades[lineShadeIndex];

            var cosinus = (position - area.center).x / radius;
            var angle = Mathf.Acos(cosinus);
            var sinus = Mathf.Abs(Mathf.Sin(angle) * radius) * Vector2.up;

            var start = position + sinus;
            var end = position - sinus;
            Handles.DrawLine(start, end);

            var color = lineShades[lineShadeIndex];
            color.r -= 0.025f;
            color.g -= 0.025f;
            color.b -= 0.025f;

            Handles.color = color;

            var top = new Vector2(position.x, area.y);
            var bottom = new Vector2(position.x, area.y + area.height);
            Handles.DrawLine(bottom, start);
            Handles.DrawLine(end, top);
        }
        else
        {
            var color = lineShades[lineShadeIndex];
            color.r -= 0.025f;
            color.g -= 0.025f;
            color.b -= 0.025f;

            Handles.color = color;

            var halfHeight = area.height * 0.5f;
            Handles.DrawLine(position + Vector2.down * halfHeight, position + Vector2.up * halfHeight);
        }
    }

    private void DrawHorizontalLine(float y, Vector2 position, Vector2Int index)
    {
        if (position.y < area.min.y || position.y > area.max.y) return;
        var lineShadeIndex = Mathf.Abs(index.y) % subDivision == 0 ? 0 : 1;

        position.x = area.center.x;
        if (y > -areaRadius * subDivision && y < areaRadius * subDivision)
        {
            Handles.color = lineShades[lineShadeIndex];

            var sinus = (position - area.center).y / radius;
            var angle = Mathf.Asin(sinus);
            var cosinus = Mathf.Abs(Mathf.Cos(angle) * radius) * Vector2.right;

            var start = position + cosinus;
            var end = position - cosinus;
            Handles.DrawLine(start, end);

            var color = lineShades[lineShadeIndex];
            color.r -= 0.025f;
            color.g -= 0.025f;
            color.b -= 0.025f;

            Handles.color = color;

            var left = new Vector2(area.x, position.y);
            var right = new Vector2(area.x + area.width, position.y);
            Handles.DrawLine(right, start);
            Handles.DrawLine(end, left);
        }
        else
        {
            var color = lineShades[lineShadeIndex];
            color.r -= 0.025f;
            color.g -= 0.025f;
            color.b -= 0.025f;

            Handles.color = color;

            var halfWidth = area.width * 0.5f;
            Handles.DrawLine(position + Vector2.left * halfWidth, position + Vector2.right * halfWidth);
        }
    }
    #endregion

    private void DrawBezier()
    {
        for (var i = 0; i < shape.Points.Count - 1; i += 3)
        {
            var p1 = Translate(shape.Points[i]);
            var p2 = Translate(shape.Points[i+1]);
            var p3 = Translate(shape.Points[i+2]);
            var p4 = Translate(shape.Points[i+3]);
            
            Handles.DrawBezier(p1,p4,p2,p3, bezierShades[0], null, bezierWidth);
        }
    }
    private void DrawHandles()
    {
        Debug.Log("Drawing handles");
        
        for (var i = 0; i < shape.Points.Count; i += 3)
        {
            var p1 = Translate(shape.Points[i]);
            
            var buttonSize = 5f * Vector2.one;
            var rect = new Rect(p1 - buttonSize * 0.5f, buttonSize);

            Handles.color = bezierShades[1];
            Handles.DrawSolidDisc(p1, Vector3.forward, 5f);
            
            var Ev = Event.current;
            if (Ev.isMouse && Ev.button == 0 && Ev.type == EventType.MouseDown)
            {
                if (rect.Contains(Ev.mousePosition))
                {
                    Debug.Log($"Adding : {i}");
                    selection.Add(i);
                    
                    Ev.Use();
                }
            }
            
            if (i + 1 == shape.Points.Count) break;
            
            var p2 = Translate(shape.Points[i+1]);
            var p3 = Translate(shape.Points[i+2]);
            var p4 = Translate(shape.Points[i+3]);
        }
    }

    private Vector2 Translate(Vector2 point) => area.center + new Vector2(point.x, -point.y) * radius;
}
