using UnityEngine;

public static class Extensions
{
    public static Vector3 Swap(this Vector2 value, float y = 0f) => new Vector3(value.x, y, value.y);
    
    public static Color SetAlpha(this Color color, float alpha)
    {
        color.a = alpha;
        return color;
    }

    public static Rect Enlarge(this Rect rect, float amount)
    {
        rect.position -= Vector2.one * amount;
        rect.size += Vector2.one * amount;

        return rect;
    }
}