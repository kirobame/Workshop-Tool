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
        rect.size += Vector2.one * amount * 2;

        return rect;
    }

    public static bool TryProjectOnto(this Vector2 pt, Vector2 p1, Vector2 p2, out Vector2 result)
    {
        var U = (pt.x - p1.x) * (p2.x - p1.x) + (pt.y - p1.y) * (p2.y - p1.y);
        var UDenom = Mathf.Pow(p2.x - p1.x, 2) + Mathf.Pow(p2.y - p1.y, 2);

        U /= UDenom;

        result.x = p1.x + U * (p2.x - p1.x);
        result.y = p1.y + U * (p2.y - p1.y);

        float minX, maxX, minY, maxY;

        minX = Mathf.Min(p1.x, p2.x);
        maxX = Mathf.Max(p1.x, p2.x);
        
        minY =  Mathf.Min(p1.y, p2.y);
        maxY =  Mathf.Max(p1.y, p2.y);

        if (result.x >= minX && result.x <= maxX && result.y >= minY && result.y <= maxY) return true;
        else return false;
    }
}