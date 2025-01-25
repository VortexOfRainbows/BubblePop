using System;
using UnityEngine;

public static class Utils
{
    public const float PixelsPerUnit = 4;
    public static Vector2 RotatedBy(this Vector2 spinningpoint, float radians, Vector2 center = default(Vector2))
    {
        float xMult = (float)MathF.Cos(radians);
        float yMult = (float)MathF.Sin(radians);
        Vector2 vector = spinningpoint - center;
        Vector2 result = center;
        result.x += vector.x * xMult - vector.y * yMult;
        result.y += vector.x * yMult + vector.y * xMult;
        return result;
    }
    /// <summary>
    /// Converts the vector to the rotation in radians which would make the vector (magnitude, 0) become the vector.
    /// Basically runs arctan on the vector. y/x
    /// </summary>
    /// <param name="directionVector"></param>
    /// <returns></returns>
    public static float ToRotation(this Vector2 directionVector)
    {
        return Mathf.Atan2(directionVector.y, directionVector.x);
    }
    public static Quaternion ToQuaternion(this float rotation)
    {
        Quaternion relativeRotation = Quaternion.AngleAxis(rotation * Mathf.Rad2Deg, new Vector3(0, 0, 1));
        return relativeRotation;
    }
    public static float WrapAngle(this float x)
    {
        x = (x + Mathf.PI) % (2 * Mathf.PI);
        if (x < 0)
            x += Mathf.PI * 2;
        return x - Mathf.PI;
    }
    public static Vector2 MouseWorld => Camera.main.ScreenToWorldPoint(Input.mousePosition);
}
