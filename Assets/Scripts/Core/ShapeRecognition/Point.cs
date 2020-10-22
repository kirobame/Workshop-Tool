using System;
using UnityEngine;

[Serializable]
public struct Point
{
    public Point(Vector2 position, float errorRadius)
    {
        this.position = position;
        this.errorRadius = errorRadius;
    }
    
    public Vector2 position;
    public float errorRadius;
}