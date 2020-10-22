using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class ShapeCreator : EditorWindow
{
    #region Encapsulated Types

    private enum SelectionMode
    {
        Anchor,
        Tangent,
        ErrorRadius
    }
    #endregion
    
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
    static void OpenWindow() => GetWindow<ShapeCreator>().Show();
    
    private Texture gridTexture;
    
    private bool isExpanded;
    private int informationHeight;
    private float lineHeight;

    private Rect area;
    private float unit;
    private float radius;

    private int selection;
    private SelectionMode mode;
    private bool isDragging;

    private List<Tangent> currentTangents;
    
    void OnEnable()
    {
        shape = null;

        informationHeight = 1;
        selection = -42;
        currentTangents = new List<Tangent>();
        lineHeight = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
        
        gridTexture = Resources.Load<Texture2D>("Grid");
        if (Selection.activeObject is Shape activeShape) shape = activeShape;

        Undo.undoRedoPerformed += UndoRedo;
    }

    void OnSelectionChange()
    {
        if (Selection.activeObject is Shape activeShape)
        {
            shape = activeShape;
            Repaint();
        }
    }
    void UndoRedo()
    {
        currentTangents.Clear();
        Repaint();
    }

    void OnGUI()
    {
        Setup();
        DrawBackground();
        
        Handles.BeginGUI();
        DrawDiscs();

        if (shape != null)
        {
            if (currentTangents.Any()) DrawTangents();
            DrawBezier();
        }
        
        DrawBorders();
        Handles.EndGUI();

        if (shape == null) return;
        
        var Ev = Event.current;
        if (Ev.isMouse) ProcessMouse(Ev);
        if (Ev.type == EventType.KeyDown && Ev.keyCode == KeyCode.A) AddSection(Ev);
        if (Ev.type == EventType.KeyDown && Ev.keyCode == KeyCode.I) InsertSection(Ev);
        if (Ev.type == EventType.KeyDown && Ev.keyCode == KeyCode.C) ClearSelection();
        if (mode == SelectionMode.Anchor && Ev.type == EventType.KeyDown && Ev.keyCode == KeyCode.Delete) DeleteSection();
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
        
        var displacement = new Vector2(0, lineHeight * informationHeight);
        var topRect = new Rect(layout.position, new Vector2(layout.width, padding.z) + displacement);
        EditorGUI.DrawRect(topRect, standard);
        Handles.DrawLine(area.min + displacement, bottomRight + displacement);

        DrawFoldout(topRect);
        
        var bottomRect = new Rect(new Vector2(layout.x, layout.height - padding.w), new Vector2(layout.width, padding.w));
        EditorGUI.DrawRect(bottomRect, standard);
        Handles.DrawLine(area.max, topLeft);
        
        var rightRect = new Rect(new Vector2(layout.width - padding.y, layout.y), new Vector2(padding.y, layout.height));
        EditorGUI.DrawRect(rightRect, standard);
        Handles.DrawLine(bottomRight + displacement, area.max);
        
        var leftRect = new Rect(layout.position, new Vector2(padding.x, layout.height));
        EditorGUI.DrawRect(leftRect, standard);
        Handles.DrawLine(topLeft, area.min + displacement);
    }
    private void DrawDiscs()
    {
        Handles.color = backgroundShades[1].SetAlpha(0.15f);
        Handles.DrawSolidDisc(area.center, Vector3.forward, radius);
        
        Handles.color = lineShades[0];
        Handles.DrawWireDisc(area.center, Vector3.forward, radius);
    }

    private void DrawFoldout(Rect rect)
    {
        rect = new Rect(rect.position + new Vector2(padding.x, padding.z * 0.5f), new Vector2(position.width - padding.y * 2, lineHeight));
        
        var lastState = isExpanded;
        isExpanded = EditorGUI.Foldout(rect, isExpanded, new GUIContent("Information"));

        if (lastState == true && isExpanded == false) informationHeight = 1;
        else if (lastState == false && isExpanded == true) informationHeight = 6;
        
        if (!isExpanded) return;

        rect.y += lineHeight;

        GUI.enabled = false;
        EditorGUI.ObjectField(rect, shape, typeof(Shape), true);
        GUI.enabled = true;
        
        var style = new GUIStyle(EditorStyles.label);
        style.richText = true;
        
        rect.y += lineHeight;
        EditorGUI.LabelField(rect, new GUIContent("<b>Add new anchor</b> : A"), style);
        
        rect.y += lineHeight;
        EditorGUI.LabelField(rect, new GUIContent("<b>Insert new anchor between closest anchors</b> : I"), style);
        
        rect.y += lineHeight;
        EditorGUI.LabelField(rect, new GUIContent("<b>Clear selection</b> : C"), style);
        
        rect.y += lineHeight;
        EditorGUI.LabelField(rect, new GUIContent("<b>Delete selected anchor</b> : Del"), style);
    }
    #endregion

    #region Bezier

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

            var color = bezierShades[1];
            if (i == selection)
            {
                color = Color.green;
                DrawErrorRadius(p1, i);
            }
            else if (i == selection - 3 || i == selection + 3) color = Color.magenta;
            
            if (DrawButtonFor(p1, 5f, color)) SetupTangents(i);
        }
    }
    private void DrawErrorRadius(Vector2 point, int index)
    {
        var perpendicular = Vector2.zero;
        if (index + 1 == shape.Points.Count)
        {
            var p2 = Translate(shape.Points[index-1]);
            var p3 = Translate(shape.Points[index-2]);
            var p4 = Translate(shape.Points[index-3]);
                    
            var velocity = Bezier.GetVelocity(point, p2, p3, p4, 0f);
            perpendicular = Vector2.Perpendicular(velocity);
        }
        else
        {
            var p2 = Translate(shape.Points[index+1]);
            var p3 = Translate(shape.Points[index+2]);
            var p4 = Translate(shape.Points[index+3]);
                    
            var velocity = Bezier.GetVelocity(point, p2, p3, p4, 0f);
            perpendicular = Vector2.Perpendicular(velocity);
        }

        var errorRadius = shape.Points[index].errorRadius * radius;
        perpendicular = point + perpendicular.normalized * errorRadius;
        
        Handles.color = Color.cyan;
        Handles.DrawLine(point, perpendicular);
        Handles.DrawWireDisc(point, Vector3.forward, errorRadius);

        if (DrawButtonFor(perpendicular, 3f, Color.cyan))
        {
            selection = index;
            mode = SelectionMode.ErrorRadius;
        }
    }
    private void SetupTangents(int index)
    {
        selection = index;
        mode = SelectionMode.Anchor;

        currentTangents.Clear();
        if (index + 1 == shape.Points.Count)
        {
            currentTangents.Add(new Tangent(index - 1, -1));
            currentTangents.Add(new Tangent(-1, index - 2));
        }
        else if (index == 0)
        {
            currentTangents.Add(new Tangent(-1, index + 1));
            currentTangents.Add(new Tangent(index + 2, -1));
        }
        else
        {
            currentTangents.Add(new Tangent(index - 1, index + 1));
            currentTangents.Add(new Tangent(-1, index - 2));
            currentTangents.Add(new Tangent(index + 2, -1));
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

            var color = index == selection ? Color.green : bezierShades[2];
            if (DrawButtonFor(point, 3f, color))
            {
                selection = index;
                mode = SelectionMode.Tangent;
            }
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
    #endregion

    #region Processing

    private void ProcessMouse(Event Ev)
    {
        if (selection != -42 && Ev.type == EventType.MouseDrag)
        {
            isDragging = true;

            if (mode == SelectionMode.ErrorRadius)
            {
                var position = Translate(shape.Points[selection]);
                var errorRadius = (Ev.mousePosition - position).magnitude / radius;
                
                Undo.RecordObject(shape, $"{shape.GetInstanceID()} Setting Error Radius {selection}");
                shape.SetPointErrorRadius(selection, errorRadius);
            }
            else if (mode == SelectionMode.Tangent || Vector2.Distance(Ev.mousePosition, area.center) < radius)
            {
                var allowedArea = area.Enlarge(-10f);
                var mousePosition = Ev.mousePosition;

                mousePosition.x = Mathf.Clamp(mousePosition.x, allowedArea.xMin, allowedArea.xMax);
                mousePosition.y = Mathf.Clamp(mousePosition.y, allowedArea.yMin, allowedArea.yMax);
                
                Undo.RecordObject(shape, $"{shape.GetInstanceID()} Moving Point {selection}");
                shape.SetPointPosition(selection, Translate(mousePosition));
            }
            else
            {
                var goal = (Ev.mousePosition - area.center).normalized * radius / radius;
                
                Undo.RecordObject(shape, $"{shape.GetInstanceID()} Moving Point {selection}");
                shape.SetPointPosition(selection, new Vector2(goal.x, -goal.y));
            }

            Repaint();
            Ev.Use();
        }

        if (Ev.button == 0 && Ev.type == EventType.MouseUp)
        {
            isDragging = false;
            Ev.Use();
        }
    }
    
    private void AddSection(Event Ev)
    {
        var destination = Vector2.zero;
        if (Vector2.Distance(Ev.mousePosition, area.center) >= radius)
        {
            var goal = (Ev.mousePosition - area.center).normalized * radius / radius;
            destination = new Vector2(goal.x, -goal.y);
        }
        else destination = Translate(Ev.mousePosition);
      
        Undo.RecordObject(shape, $"{shape.GetInstanceID()} Adding Section");
        shape.AddNewSection(destination);
        
        Repaint();
    }
    private void InsertSection(Event Ev)
    {
        var mousePosition = Translate(Ev.mousePosition);
        
        var firstClosest = float.PositiveInfinity;
        var firstClosestIndex = -1;

        var secondClosest = float.PositiveInfinity;
        var secondClosestIndex = -1;

        for (var i = 0; i < shape.Points.Count; i++)
        {
            if (i % 3 != 0) continue;
            var distance = Vector2.Distance(mousePosition, shape.Points[i].position);

            if (distance < firstClosest)
            {
                secondClosest = firstClosest;
                secondClosestIndex = firstClosestIndex;
                
                firstClosest = distance;
                firstClosestIndex = i;
            }
            else if (distance < secondClosest)
            {
                secondClosest = distance;
                secondClosestIndex = i;
            }
        }
        
        var from = Mathf.Min(firstClosestIndex, secondClosestIndex);
        
        Undo.RecordObject(shape, $"{shape.GetInstanceID()} Inserting section at {from}");
        shape.InsertNewSection(mousePosition, from);
        
        Repaint();
    }
    private void DeleteSection()
    {
        if (selection != -42 && shape.Points.Count == 4) return;
        
        Undo.RecordObject(shape, $"{shape.GetInstanceID()} Deleting section at {selection}");
        shape.DeleteSection(selection);
        
        currentTangents.Clear();
        ClearSelection();
        
        Repaint();
    }

    private void ClearSelection()
    {
        currentTangents.Clear();
        
        isDragging = false;
        selection = -42;
        
        Repaint();
    }
    #endregion
    
    private Vector2 Translate(Point point)
    {
        var position = point.position;
        return area.center + new Vector2(position.x, -position.y) * radius;
    }
    private Vector2 Translate(Vector2 screenPoint)
    {
        var position = (screenPoint - area.center) / radius;
        return new Vector2(position.x, -position.y);
    }
}
