using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "NewShape", menuName = "Custom/Shape")]
public class Shape : ScriptableObject
{
    public const float standardErrorRadius = 0.1f;
    private const int curveSubdivision = 6;

    private const float headingAffect = 0.075f;
    private const float spacingAffect = 0.1f;
    private const float radiusForgiveness = 2.5f;
    
    public static readonly Vector2 errorRange = new Vector2(0.05f, 0.35f);
    
    public IReadOnlyList<Point> Points => points;
    [SerializeField] private List<Point> points = new List<Point>()
    {
        new Point(new Vector2(-1f, 0f), standardErrorRadius),
        new Point(new Vector2(-0.25f, 0f), standardErrorRadius),
        new Point(new Vector2(0f, 0.25f), standardErrorRadius),
        new Point(new Vector2(0f, 1f), standardErrorRadius),
    };

    public IReadOnlyList<Point> RuntimePoints => runtimePoints;
    private Point[] runtimePoints;
    
    #region Edition

    public void SetPointPosition(int index, Vector2 position)
    {
        var point = points[index];
        point.position = position;

        points[index] = point;
    }
    public void SetPointErrorRadius(int index, float radius)
    {
        radius = Mathf.Clamp(radius, errorRange.x, errorRange.y);

        var point = points[index];
        point.errorRadius = radius;

        points[index] = point;
    }

    public void AddNewSection(Vector2 destination)
    {
        var lastPoint = points.Last();
        var direction = destination - lastPoint.position;

        var firstTangent = lastPoint.position + direction * 0.25f;
        points.Add(new Point(firstTangent, standardErrorRadius));

        var secondTangent = lastPoint.position + direction * 0.75f;
        points.Add(new Point(secondTangent, standardErrorRadius));
        
        points.Add(new Point(destination, standardErrorRadius));
    }
    public void InsertNewSection(Vector2 destination, int from)
    {
        var section = new Point[3];
        
        var firstTangent = destination + (points[from + 1].position - destination) * 0.25f;
        section[0] = new Point(firstTangent, standardErrorRadius);
        section[1] = new Point(destination, standardErrorRadius);
        
        var secondTangent = destination + (points[from + 2].position - destination) * 0.25f;
        section[2] = new Point(secondTangent, standardErrorRadius);
        
        points.InsertRange(from + 2, section);
    }
    public void DeleteSection(int index)
    {
        var range = Vector2Int.zero;
        
        if (index == 0) range = new Vector2Int(0,3);
        else if (index == points.Count - 1) range = new Vector2Int(index - 2, 3);
        else range = new Vector2Int(index - 1, 3);
        
        points.RemoveRange(range.x, range.y);
    }
    #endregion

    public void GenerateRuntimeData()
    {
        var results = new List<Point>();

        var index = 0;
        for (var i = 0; i < points.Count - 1; i += 3)
        {
            var p1 = points[i].position;
            var p2 = points[i+1].position;
            var p3 = points[i+2].position;
            var p4 = points[i+3].position;

            for (float j = 0; j < curveSubdivision; j++)
            {
                var ratio = j / curveSubdivision;
                
                var position = Bezier.GetPoint(p1, p2, p3, p4, ratio);
                var errorRadius = Mathf.Lerp(points[i].errorRadius, points[i + 3].errorRadius, ratio);
                
                results.Add(new Point(position, errorRadius * radiusForgiveness));
            }
        }

        var last = points.Last();
        last.errorRadius *= radiusForgiveness;
        
        results.Add(last);
        runtimePoints = results.ToArray();
    }
    public bool CanStartEvaluation(Vector2 position)
    {
        var firstPoint = points.First();
        var distance = Vector2.Distance(firstPoint.position, position);

        return distance <= firstPoint.errorRadius;
    }
    public float Evaluate(Vector2 position, Vector2 direction, int index, out bool next)
    {
        next = false;
        
        var p1 = runtimePoints[index].position;
        var p2 = runtimePoints[index + 1].position;
        
        var p2Distance = Vector2.Distance(position, p2);
        if (p2Distance <= runtimePoints[index + 1].errorRadius)
        {
            next = true;
            return 0;
        }
        
        var headingError = Mathf.Clamp01(Vector2.Dot(direction, (p2 - p1).normalized) * -1) * headingAffect;
        
        if (position.TryProjectOnto(p1, p2, out var result))
        {
            var ratio = (result - p1).magnitude / (p2 - p1).magnitude;
            var errorMargin = Mathf.Lerp(runtimePoints[index].errorRadius, runtimePoints[index + 1].errorRadius, ratio);

            var distance = Vector2.Distance(position, p2);
            if (distance <= errorMargin) return headingError;
            else
            {
                var spacingError = (distance - errorMargin) * spacingAffect;
                return (headingError + spacingError) / 2f;
            }
        }
        else
        {
            var p1Distance = Vector2.Distance(position, p1);
            if (p1Distance < p2Distance)
            {
                if (p1Distance <= runtimePoints[index].errorRadius) return headingError;
                else
                {
                    var spacingError = (p1Distance - runtimePoints[index].errorRadius) * spacingAffect;
                    return (headingError + spacingError) / 2f;
                }
            }
            else
            {
                var spacingError = (p1Distance - runtimePoints[index + 1].errorRadius) * spacingAffect;
                return (headingError + spacingError) / 2f;
            }
        }
    }
}
