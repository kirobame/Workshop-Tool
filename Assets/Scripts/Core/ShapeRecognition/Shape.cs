using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewShape", menuName = "Custom/Shape")]
public class Shape : ScriptableObject
{
    public IReadOnlyList<Vector2> Points => points;
    [SerializeField] private Vector2[] points = new Vector2[4]
    {
        new Vector2(-1f, 0f),
        new Vector2(-0.25f, 0f),
        new Vector2(0f, 0.25f),
        new Vector2(0f, 1f)
    };

    public void MovePoint(int index, Vector2 delta) => points[index] += delta;
}
