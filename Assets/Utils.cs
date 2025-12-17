using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using static Enemy;
public static class Utils
{
    public static Unity.Mathematics.Random rand = InitRandSeed();
    private static Unity.Mathematics.Random InitRandSeed()
    {
        Unity.Mathematics.Random r = new();
        r.InitState((uint)UnityEngine.Random.Range(0, int.MaxValue));
        return r;
    }
    public static bool RollWithLuck(float odds)
    {
        float n = RollWithLuckRaw();
        return n < odds;
    }
    public static float RollWithLuckRaw()
    {
        return rand.NextFloat();
    }
    /// <summary>
    /// Lerps adjusted for delta time so it can be used consistently in Update(), rather than FixedUpdate()
    /// </summary>
    /// <param name="originalNumber">The lerp factor</param>
    /// <param name="intendedApplicationRate">How many times per frame it is inteded to be applied. Default is 100, as the fixed delta time is 0.01.</param>
    /// <returns></returns>
    public static float DeltaTimeLerpFactor(float originalNumber, float intendedApplicationRate = 100f)
    {
        return 1 - Mathf.Pow(1 - originalNumber, Time.unscaledDeltaTime * intendedApplicationRate);
    }
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
    public static Vector2 RandCircle(float r = 1)
    {
        return UnityEngine.Random.insideUnitCircle * r;
    }
    public static Vector2 RandCircleEdge(float r = 1)
    {
        return UnityEngine.Random.insideUnitCircle.normalized * r;
    }
    public static Vector2 RandCircle(float min, float max)
    {
        return RandCircleEdge(RandFloat(min, max));
    }
    public const int AlternativeCameraPosX = 5000;
    public const int AlternativeCameraPosY = 1000;
    public static Vector3 PositionAdjustedByCanvas(Vector3 pos, Canvas canvas)
    {
        pos.x -= canvas.transform.position.x;
        pos.y -= canvas.transform.position.y;
        pos /= canvas.GetComponent<RectTransform>().lossyScale.x;
        pos *= Main.ActivePrimaryCanvas.scaleFactor;
        pos += Main.ActivePrimaryCanvas.transform.position;
        return pos;
    }
    public static bool IsMouseHoveringOverThis(bool rectangular, RectTransform transform, float radius, Canvas canvas = null, bool ignoreScale = false)
    {
        if (Main.ActivePrimaryCanvas == null)
            return false;
        Vector3 pos = transform.position;
        float scale = Main.ActivePrimaryCanvas.scaleFactor;
        if (canvas == null)
            canvas = Main.ActivePrimaryCanvas;
        else
        {
            pos = PositionAdjustedByCanvas(pos, canvas);
        }
        if (rectangular)
        {
            //Debug.Log(pos);
            scale *= ignoreScale ? 1 : transform.localScale.x;
            Rect rect = transform.rect;
            float width = (rect.width + radius) * scale;
            float height = (rect.height + radius) * scale ;
            rect = new Rect(pos.x - width * transform.pivot.x, pos.y - height * transform.pivot.y, width, height);
            if (rect.Contains(Input.mousePosition))
                return true;
        }
        else
        {
            pos += radius * scale * new Vector3(1 - 2 * transform.pivot.x, 1 - 2 * transform.pivot.y);
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
    public static Color WithAlpha(this Color color, float alphaMultiplier)
    {
        color.a = alphaMultiplier;
        return color;
    }
    public static Transform LerpLocalPosition(this Transform transform, Vector2 newPosition, float t)
    {
        transform.localPosition = Vector3.Lerp(transform.localPosition, new Vector3(newPosition.x, newPosition.y, transform.localPosition.z), t);
        return transform;
    }
    public static Transform LerpLocalScale(this Transform transform, Vector2 newScale, float t)
    {
        transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(newScale.x, newScale.y, transform.localScale.z), t);
        return transform;
    }
    public static Transform LerpLocalEulerZ(this Transform transform, float r, float t)
    {
        transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, Mathf.LerpAngle(transform.localEulerAngles.z, r, t));
        return transform;
    }
    public static Transform SetEulerZ(this Transform transform, float r)
    {
        transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, r);
        return transform;
    }
    public static Vector3 SetXY(this Vector3 v, float x, float y)
    {
        v.x = x;
        v.y = y;
        return v;
    }
    public static Vector3 SetXY(this Vector3 v, Vector2 v2)
    {
        v.x = v2.x;
        v.y = v2.y;
        return v;
    }
    public static string ToHexString(this Color color)
    {
        return
            ((byte)(color.r * 255)).ToString("X2") +
            ((byte)(color.g * 255)).ToString("X2") +
            ((byte)(color.b * 255)).ToString("X2") +
            ((byte)(color.a * 255)).ToString("X2");
    }
    public static float Distance(this Vector3 v, Vector3 v2)
    {
        return (v - v2).magnitude;
    }
    public static float Distance(this Vector2 v, Vector2 v2)
    {
        return (v - v2).magnitude;
    }
    public static float Distance(this Vector2 v, Vector2 v2, float xContribution, float yContribution)
    {
        Vector2 v3 = (v - v2);
        v3.x *= xContribution;
        v3.y *= yContribution;
        return v3.magnitude;
    }
    public static Vector3 ClampToRect(Vector3 v, Rect boundingRect, float padding = 0)
    {
        float left = boundingRect.xMin + padding;
        float right = boundingRect.xMax - padding;
        float bot = boundingRect.yMin + padding;
        float top = boundingRect.yMax - padding;
        if (v.x < left)
            v.x = left;
        if (v.x > right)
            v.x = right;
        if (v.y < bot)
            v.y = bot;
        if (v.y > top)
            v.y = top;
        return v;
    }
    public static float SignNoZero(this float x)
    {
        if (x < 0)
            return -1;
        return 1;
    }
    public static Vector2 LerpSnap(Transform target, Vector2 position, float amt = 0.1f, float tolerance = 0.5f)
    {
        if (target.localPosition.Distance(position) < tolerance)
            target.localPosition = position;
        else
            target.localPosition = target.localPosition.Lerp(position, amt);
        return target.localPosition;
    }
    public static Vector2 LerpSnapNotLocal(Transform target, Vector2 position, float amt = 0.1f, float tolerance = 0.5f)
    {
        if (target.position.Distance(position) < tolerance)
            target.position = position;
        else
            target.position = target.position.Lerp(position, amt);
        return target.position;
    }
    public static string WithSizeAndColor(this string s, int size, string color)
    {
        return $"<size={size}><color={color}>{s}</color></size>";
    }
    public static string WithColor(this string s, string color)
    {
        return $"<color={color}>{s}</color>";
    }
    public static TileBase GetTile(this Tilemap map, int i, int j)
    {
        return map.GetTile(new Vector3Int(i, j));
    }
    public static float LerpAngleRadians(float a, float b, float t)
    {
        float num = Mathf.Repeat(b - a, MathF.PI * 2);
        if (num > MathF.PI)
        {
            num -= MathF.PI * 2;
        }
        return a + num * Mathf.Clamp01(t);
    }
    public static int Rand1OrMinus1()
    {
        return RandInt(2) * 2 - 1;
    }
}
