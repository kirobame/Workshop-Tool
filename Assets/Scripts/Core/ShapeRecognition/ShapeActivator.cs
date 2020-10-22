using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewShapeActivator", menuName = "Custom/Activators/Shape")]
public class ShapeActivator : Activator<Vector2>
{
    #region Encapsulated Types

    private struct TrackedShape
    {
        public TrackedShape(int index)
        {
            this.index = index;

            advancement = 0;
            error = 0;
        }

        public readonly int index;

        public int advancement;
        public float error;
    }
    #endregion
    
    public override string ShortName => "Shp";

    [SerializeField] private Shape[] shapes;

    private List<int> untrackedShapes = new List<int>();
    private List<TrackedShape> trackedShapes = new List<TrackedShape>();
    
    private Vector2 lastInput;

    public override void Initialize()
    {
        for (var i = 0; i < shapes.Length; i++)
        {
            shapes[i].GenerateRuntimeData();
            untrackedShapes.Add(i);
        }
    }

    protected override void OnStarted(Vector2 input) => lastInput = input;
    protected override void OnPerformed(Vector2 input)
    {
        for (var i = 0; i < untrackedShapes.Count; i++)
        {
            if (!shapes[untrackedShapes[i]].CanStartEvaluation(input)) continue;
            
            Debug.Log($"Now Tracking : {shapes[untrackedShapes[i]]}");
            
            trackedShapes.Add(new TrackedShape(untrackedShapes[i]));
            untrackedShapes.RemoveAt(i);
            i--;
        }
        
        var direction = (input - lastInput).normalized;
        for (var i = 0; i < trackedShapes.Count; i++)
        {
            var current = trackedShapes[i];
            var error = shapes[current.index].Evaluate(input, direction, current.advancement, out var next);

            Debug.Log($"Evaluating : {shapes[current.index]}");
            
            if (next)
            {
                if (current.advancement + 1 == shapes[current.index].RuntimePoints.Count - 1)
                {
                    Debug.Log($"Recognized : {shapes[current.index]}");
                    Execute(shapes[current.index]);
                    
                    untrackedShapes.Add(current.index);
                    trackedShapes.RemoveAt(i);
                    i--;
                }
                else
                {
                    current.error = 0;
                    current.advancement++;
                    
                    trackedShapes[i] = current;
                }
            }
            else
            {
                current.error += error;
                if (current.error > 10)
                {
                    Debug.Log($"Lost Tracking of : {shapes[current.index]}");
                    
                    untrackedShapes.Add(current.index);
                    trackedShapes.RemoveAt(i);
                    i--;
                }
                else trackedShapes[i] = current;
            }
        }
        
        lastInput = input;
    }
    protected override void OnCanceled(Vector2 input)
    {
        Debug.Log("END");
        
        untrackedShapes.Clear();
        for (var i = 0; i < shapes.Length; i++) untrackedShapes.Add(i);
        
        trackedShapes.Clear();
    }
}