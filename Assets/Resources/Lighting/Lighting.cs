using System.Collections.Generic;
using UnityEditor.Build;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public static class Lighting
{
    public static Tile LightTile;
    public static Tile OcclusionTile;
    public static RenderTexture LightRT;
    public static Camera LightingCamera;
    public static Material FrontLight;
    public static Material BackLight;
    public static RawImage ShadowRenderTexture => Main.Instance.TileLightRenderTarget;
    public static Light2D GlobalLight => Main.Instance.GlobalLight;
    public static void Setup(Tilemap Map, Tilemap LightingFront, Tilemap LightingBack, Tilemap OcclusionMap)
    {
        if (Map == null || LightingFront == null || LightingBack == null)
        {
            throw new System.Exception("ERROR: Could not find lighting tile maps");
        }
        LightTile = Resources.Load<Tile>("Lighting/LightTile");
        OcclusionTile = Resources.Load<Tile>("Lighting/OcclusionLightTile");
        LightRT = Resources.Load<RenderTexture>("Lighting/LightingRenderTexture");
        LightingCamera = Camera.main.transform.GetChild(0).GetComponent<Camera>();
        FrontLight = LightingFront.GetComponent<TilemapRenderer>().material;
        BackLight = LightingBack.GetComponent<TilemapRenderer>().material;
        //LightRTSprite = Sprite.Create(LightRT, new Rect(0, 0, LightRT.width, LightRT.height), new Vector2(0.5f, 0.5f));
        Map.GetCorners(out int left, out int right, out int bottom, out int top);
        for (int i = left; i < right; i++)
        {
            for (int j = bottom; j < top; j++)
            {
                Vector3Int pos = new(i, j);
                if (World.SolidTile(pos)) //This is also used for occlusion so it is obtained when typically setting up the tile maps... Additionally, it could be used to check for solid tiles quicker, but im not certain if it is faster (NEEDS TESTING)
                {
                    LightingFront.SetTile(pos, LightTile);
                    LightingBack.SetTile(pos, LightTile);
                    OcclusionMap.SetTile(pos, OcclusionTile);
                }
            }
        }
        DayProgress = TimeInADay * 0.1f;
        Update();
    }
    public static void Update() //Runs on normal delta time, not fixed
    {
        ResizeLightingRenderTexture();
        UpdateSun();
    }
    public static float DayProgress = 0;
    public static readonly float TimeInADay = 480; //6 minutes per day, for now (night progresses faster, so 4 + 2 = 6)
    public static Vector2 SunVector = new(1, 0);
    public static bool IsDay => DayProgress < TimeInADay / 2;
    public static bool IsNight => !IsDay;
    public static float DaySin = 0;
    public static float NightSin = 0;
    public static void UpdateSun()
    {
        float factor = 1.0f;
        if (IsNight)
            factor = 2.0f;
        if (Main.DebugCheats && Input.GetKey(KeyCode.K))
            factor *= 20;
        DayProgress += Time.deltaTime * factor;
        if (DayProgress > TimeInADay)
            DayProgress -= TimeInADay;
        float dayPercent = DayProgress / TimeInADay;
        SunVector = new Vector2(1, 0).RotatedBy(dayPercent * Utils.TWOPI);
        DaySin = SunVector.y > 0 ? SunVector.y : 0;
        NightSin = SunVector.y < 0 ? -SunVector.y : 0;
        if (FrontLight != null && BackLight != null)
        {
            float maxWidth = 12;
            float width;
            if (SunVector.y == 0)
                width = 0;
            else
                width = SunVector.x / Mathf.Abs(SunVector.y); //TANGENT
            width = Mathf.Clamp(width, -maxWidth, maxWidth);
            Vector2 Sun = new(-width, Mathf.Abs(SunVector.y) * 2 - 3.5f); 
            if (Sun.x == 0)
                Sun.x = 0.001f; //CANNOT LET SUN.X BE 0
            FrontLight.SetVector("_Sun", Sun);
            BackLight.SetVector("_Sun", Sun);

            float alphaMult = Mathf.Sqrt(Mathf.Abs(SunVector.y)) * (maxWidth - Mathf.Abs(width)) / maxWidth;
            alphaMult = Mathf.Clamp01(alphaMult);
            if (IsNight) //nightTime)
                alphaMult *= 0.4f;
            ShadowRenderTexture.color = new Color(0, 0, 0, 0.95f * alphaMult);
        }
        GetSunlightColor();
    }
    internal class ColorWithRange
    {
        public float Start;
        public float End;
        public Color Color;
        public float blendFront;
        public float blendBack;
        public ColorWithRange(Color color, float startRange, float endRange, float blendBack = 0.05f, float blendFront = 0.05f)
        {
            Color = color;
            Start = startRange + blendBack * 0.5f;
            End = endRange - blendFront * 0.5f;
            this.blendBack = blendBack;
            this.blendFront = blendFront;
        }
        public Color Get(float currentValue)
        {
            if (currentValue >= Start && currentValue < End)
                return Color;
            else
            {
                float p = 0;
                if(currentValue < Start)
                {
                    currentValue -= Start;
                    p = 1 + currentValue / blendBack;
                }
                else //currentValue >= End
                {
                    currentValue -= End;
                    p = 1 - currentValue / blendFront;
                }
                if (p <= 0 || p >= 1)
                    return Color.clear;
                return Color * p;
            }
        }
    }
    public static void GetSunlightColor()
    {
        Color nightColor = new(.15f, .1f, 0.3f);
        Color dayBreak = new(.35f, .15f, .35f);
        Color sunRise = new(.5f, .35f, .15f);
        Color dayColor = new(1, 1, 1);
        List<ColorWithRange> ColorRange = new();
        Color final = Color.clear;
        if (IsDay)
        {
            ColorRange.Add(new(dayBreak, -0.1f, 0.05f));
            ColorRange.Add(new(sunRise, 0.05f, 0.10f));
            ColorRange.Add(new(dayColor, 0.1f, 0.9f));
            ColorRange.Add(new(sunRise, 0.9f, 0.95f));
            ColorRange.Add(new(dayBreak, 0.95f, 1.1f));
            float percent = DayProgress / TimeInADay * 2;
            foreach(ColorWithRange c in ColorRange)
                final += c.Get(percent);
        }
        else if(IsNight)
        {
            ColorRange.Add(new(dayBreak, -0.1f, 0.05f));
            //ColorRange.Add(new(nightWake, 0.05f, 0.10f));
            ColorRange.Add(new(nightColor, 0.05f, 0.95f));
            //ColorRange.Add(new(nightWake, 0.9f, 0.95f));
            ColorRange.Add(new(dayBreak, 0.95f, 1.1f));
            float percent = DayProgress / TimeInADay * 2 - 1;
            foreach (ColorWithRange c in ColorRange)
                final += c.Get(percent);
        }
        GlobalLight.color = final;//.WithAlpha(1.0f);
    }
    public static float PortionOfRange(float percent, float startingThresh, float endThresh)
    {
        percent -= startingThresh;
        if(percent > 0 && percent < endThresh)
        {
            float p = percent / endThresh;
            if (p > 1)
                p = 1;
            return p;
        }
        return 0;
    }
    public static void ResizeLightingRenderTexture()
    {
        //Debug.Log($"{Screen.width}, {Screen.height}");
        if(LightRT.width != Screen.width || LightRT.height != Screen.height)
        {
            //// Set the provided RenderTexture as active
            //var prev = RenderTexture.active;
            //RenderTexture.active = LightRT;

            //// Create a new Texture2D with the same dimensions
            //Texture2D tex = new Texture2D(LightRT.width, LightRT.height);

            //tex.ReadPixels(new Rect(0, 0, LightRT.width, LightRT.height), 0, 0);
            //tex.Apply(); // Apply changes;
            //System.IO.File.WriteAllBytes("texture.png", tex.EncodeToPNG());
            //RenderTexture.active = prev;

            LightRT.Release();
            LightRT.width = Screen.width;
            LightRT.height = Screen.height;
            LightRT.Create();
            LightingCamera.ResetAspect();
            if (ShadowRenderTexture != null)
                ShadowRenderTexture.SetMaterialDirty(); //Updates the texel size on the gaussian blur to make the shadows consistent on resize
        }
        if (ShadowRenderTexture != null)
        {
            float baseTexelSize = 4 / 1080f;
            ShadowRenderTexture.material.SetVector("_TexelScaler", new Vector2(baseTexelSize / Camera.main.aspect, baseTexelSize));
            ShadowRenderTexture.gameObject.SetActive(true);
        }
    }
}
