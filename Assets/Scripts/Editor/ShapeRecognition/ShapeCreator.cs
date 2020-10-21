using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        new Color(0.04f, 0.04f, 0.04f),
        new Color(0.0868039f, 0.0868039f, 0.0868039f),
        new Color(0.1558039f, 0.1558039f, 0.1558039f),
    };
    private readonly Color[] bezierShades = new Color[]
    {
        Color.yellow,
        Color.red,
        Color.white
    };
    
    private readonly Color standard = new Color(0.2196079f, 0.2196079f, 0.2196079f);
    private readonly Vector4 padding = new Vector4(4,4,4,4);
    
    [MenuItem("Tools/Custom/Shape Creator")]
    static void OpenWindow()
    {
        GetWindow<ShapeCreator>().Show();
        shape = CreateInstance<Shape>();
    }

    private Texture gridTexture;

    private Rect area;
    private float unit;
    private float radius;

    private List<int> selection = new List<int>();
    private List<Tangent> currentTangents = new List<Tangent>();
    private bool isDragging;

    void OnEnable() => gridTexture = Resources.Load<Texture2D>("Grid");

    void OnGUI()
    {
        Setup();
        DrawBackground();
        
        Handles.BeginGUI();
        
        DrawDiscs();
        DrawBorders();
        
        if (currentTangents.Any()) DrawTangents();
        DrawBezier();

        Handles.EndGUI();
        
        var Ev = Event.current;
        if (Ev.isMouse)
        {
            if (Ev.type == EventType.MouseDrag)
            {
                isDragging = true;
                var delta = Ev.delta / radius;

                foreach (var point in selection)
                {
                    var position = Translate(shape.Points[point]);
                    if (Vector2.Distance(position + delta * 2, area.center) >= radius && Vector2.Dot((area.center - position).normalized, delta.normalized) < 0) continue;
                    
                    shape.MovePoint(point, new Vector2(delta.x, -delta.y));
                }
                
                Repaint();
                Ev.Use();
            }

            if (Ev.button == 0 && Ev.type == EventType.MouseUp)
            {
                isDragging = false;
                selection.Clear();
                
                Ev.Use();
            }
        }
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

        radius = areaRadius * subDivision * unit;
    }

    #region Aesthetics

    private void DrawBackground()
    {
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
    }
    private void DrawBorders()
    {
        var bottomRight = new Vector2(area.xMax, area.yMin);
        var topLeft = new Vector2(area.xMin, area.yMax);

        var layout = position;
        layout.position = Vector2.zero;
        
        Handles.color = lineShades[0];
        
        var bottomRect = new Rect(new Vector2(layout.x, layout.height - padding.w), new Vector2(layout.width, padding.w));
        EditorGUI.DrawRect(bottomRect, standard);
        Handles.DrawLine(area.min, bottomRight);
        
        var rightRect = new Rect(new Vector2(layout.width - padding.y, layout.y), new Vector2(padding.y, layout.height));
        EditorGUI.DrawRect(rightRect, standard);
        Handles.DrawLine(bottomRight, area.max);
        
        var topRect = new Rect(layout.position, new Vector2(layout.width, padding.z));
        EditorGUI.DrawRect(topRect, standard);
        Handles.DrawLine(area.max, topLeft);
        
        var leftRect = new Rect(layout.position, new Vector2(padding.x, layout.height));
        EditorGUI.DrawRect(leftRect, standard);
        Handles.DrawLine(topLeft, area.min);
    }
    private void DrawDiscs()
    {
        Handles.color = backgroundShades[1].SetAlpha(0.15f);
        Handles.DrawSolidDisc(area.center, Vector3.forward, radius);
        
        Handles.color = lineShades[0];
        Handles.DrawWireDisc(area.center, Vector3.forward, radius);
    }
    #endregion
    
    private void DrawBezier()
    {
        for (var i = 0; i < shape.Points.Count; i += 3)
        {
            var p1 = Translate(shape.Points[i]);

            if (i + 1 != shape.Points.Count)
            {
                var p2 = Translate(shape.Points[i+1]);
                var p3 = Translate(shape.Points[i+2]);
                var p4 = Translate(shape.Points[i+3]);
            
                Handles.DrawBezier(p1,p4,p2,p3, bezierShades[0], null, bezierWidth);
            }

            var color = selection.Contains(i) ? Color.blue : bezierShades[1];
            if (DrawButtonFor(p1, 5f, color))
            {
                selection.Add(i);

                currentTangents.Clear();
                if (i + 1 == shape.Points.Count)
                {
                    currentTangents.Add(new Tangent(i - 1, -1));
                    currentTangents.Add(new Tangent(-1, i - 2));
                }
                else if (i == 0)
                {
                    currentTangents.Add(new Tangent(-1, i + 1));
                    currentTangents.Add(new Tangent(i + 2, -1));
                }
                else
                {
                    currentTangents.Add(new Tangent(i - 1, i + 1));
                    currentTangents.Add(new Tangent(-1, i - 2));
                    currentTangents.Add(new Tangent(i + 2, -1));
                }
            }
        }
    }
    private void DrawTangents()
    {
        foreach (var tangent in currentTangents)
        {
            if (tangent.TryGetFirst(out var firsIndex)) DrawFor(firsIndex, + 1);
            if (tangent.TryGetSecond(out var secondIndex)) DrawFor(secondIndex, - 1);
        }

        void DrawFor(int index, int direction)
        {
            Handles.color = bezierShades[2];
            
            var point = Translate(shape.Points[index]);
            Handles.DrawLine(point, Translate(shape.Points[index + direction]));

            var color = selection.Contains(index) ? Color.blue : bezierShades[2];
            if (DrawButtonFor(point, 3f, color)) selection.Add(index);
        }
    }
    
    private bool DrawButtonFor(Vector2 point, float size, Color color)
    {
        var buttonSize = size * Vector2.one;
        var rect = new Rect(point - buttonSize * 0.5f, buttonSize);

        Handles.color = color;
        Handles.DrawSolidDisc(point, Vector3.forward, size);
            
        var Ev = Event.current;
        if (Ev.isMouse && Ev.button == 0 && Ev.type == EventType.MouseDown)
        {
            if (!rect.Enlarge(size * 4).Contains(Ev.mousePosition)) return false;
            
            Ev.Use();
            return true;
        }

        return false;
    }
    private Vector2 Translate(Vector2 point) => area.center + new Vector2(point.x, -point.y) * radius;
}
