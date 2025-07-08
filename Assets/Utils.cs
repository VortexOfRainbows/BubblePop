using System;
using System.Collections.Generic;
using UnityEngine;
using static Enemy;

public static class Utils
{
    public static List<PowerUp> Add<T>(this List<PowerUp> list) where T : PowerUp
    {
        list.Add(PowerUp.Get<T>());
        return list;
    }
    public static List<PowerUp> Remove<T>(this List<PowerUp> list) where T : PowerUp
    {
        list.Remove(PowerUp.Get<T>());
        return list;
    }
    public static string ToSpacedString(this string str)
    {
        for(int i = str.Length - 1; i > 0; --i)
        {
            if (char.IsUpper(str[i]))
                str = str.Insert(i, " ");
        }
        return str;
    }
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
    public static float RandFloat(float max = 1)
    {
        return UnityEngine.Random.Range(0, max);
    }
    public static float RandFloat(float min, float max)
    {
        return UnityEngine.Random.Range(min, max);
    }
    public static int RandInt(int max = 1)
    {
        return UnityEngine.Random.Range(0, max);
    }
    public static int RandInt(int min, int maxExclusive)
    {
        return UnityEngine.Random.Range(min, maxExclusive);
    }
    public static Vector2 RandCircle(float r)
    {
        return UnityEngine.Random.insideUnitCircle * r;
    }
    public const int AlternativeCameraPosX = 5000;
    public const int AlternativeCameraPosY = 1000;
    public static bool IsMouseHoveringOverThis(bool rectangular, RectTransform transform, float radius = 64, Canvas canvas = null)
    {
        Vector3 pos = transform.position;
        float scale = UIManager.Instance.MainGameCanvas.scaleFactor;
        if (canvas == null)
            canvas = UIManager.Instance.MainGameCanvas;
        else
        {
            pos.x -= canvas.transform.position.x;
            pos.y -= canvas.transform.position.y;
            pos /= canvas.GetComponent<RectTransform>().lossyScale.x;
            pos *= UIManager.Instance.MainGameCanvas.scaleFactor;
            pos += UIManager.Instance.MainGameCanvas.transform.position;
        }
        if (rectangular)
        {
            //Debug.Log(pos);
            scale *= transform.localScale.x;
            Rect rect = transform.rect;
            float width = rect.width * scale;
            float height = rect.height * scale;
            rect = new Rect(pos.x - width * transform.pivot.x, pos.y - height * transform.pivot.y, width, height);
            if (rect.Contains(Input.mousePosition))
                return true;
        }
        else
        {
            pos += new Vector3(1 - 2 * transform.pivot.x, 1 - 2 * transform.pivot.y) * scale * radius;
            if (((Vector2)pos - (Vector2)Input.mousePosition).magnitude < radius * scale)
                return true;
        }
        return false;
    }
    public static bool Contains(this List<ImmunityData> list, Projectile proj)
    {
        foreach(ImmunityData data in list)
            if (data.attacker == proj)
                return true;
        return false;
    }
    public static Vector3 Lerp(this Vector3 vector3, Vector3 other, float amt)
    {
        return vector3 = Vector3.Lerp(vector3, other, amt);
    }
    public static Color PastelRainbow(float radians, float centerColor = 0.75f, Color overrideColor = default)
    {
        float center = centerColor;
        float spread = 1 - center;

        float circlePalette = Mathf.Cos(radians);
        float width = spread * circlePalette;
        float red = center + width;

        circlePalette = Mathf.Cos(radians + 2.094f);
        width = spread * circlePalette;
        float grn = center + width;

        circlePalette = Mathf.Cos(radians + 4.1888f);
        width = spread * circlePalette;
        float blu = center + width;

        if (overrideColor == default)
            return new Color(red, grn, blu);
        else
            return new Color(red, grn, blu) * overrideColor;
    }
    public static Color WithAlphaMultiplied(this Color color, float alphaMultiplier)
    {
        color.a *= alphaMultiplier;
        return color;
    }
}
