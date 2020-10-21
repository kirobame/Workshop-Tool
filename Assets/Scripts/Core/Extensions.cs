using UnityEngine;

public static class Extensions
{
    public static Vector3 Swap(this Vector2 value, float y = 0f) => new Vector3(value.x, y, value.y);
    
    public static Color SetAlpha(this Color color, float alpha)
    {
        color.a = alpha;
        return color;
    }
}