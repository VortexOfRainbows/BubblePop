using UnityEngine;
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
    }
    public static void Update() //Runs on normal delta time, not fixed
    {
        ResizeLightingRenderTexture();
        UpdateSun();
    }
    public static float DayProgress = 0;
    public static readonly float TimeInADay = 60; //1 minutes per day, for now
    public static Vector2 SunVector = new(1, 0);
    public static bool SunRise => SunVector.x > 0.8f && SunVector.y > 0;
    public static bool MidDay => SunVector.x > -0.2f && SunVector.x < 0.2f && SunVector.y > 0;
    public static bool SunSet => SunVector.x < -0.8f && SunVector.y > 0;
    public static bool Nightwake => SunVector.x < -0.8f && SunVector.y < 0;
    public static bool Midnight => SunVector.x > -0.2f && SunVector.x < 0.2f && SunVector.y < 0;
    public static bool Twilight => SunVector.x > 0.8f && SunVector.y < 0;
    public static void UpdateSun()
    {
        DayProgress += Time.deltaTime;
        if (DayProgress > TimeInADay)
            DayProgress -= TimeInADay;
        float dayPercent = DayProgress / TimeInADay;
        SunVector = new Vector2(1, 0).RotatedBy(dayPercent * Mathf.PI * 2);
        if (FrontLight != null && BackLight != null)
        {
            Vector2 Sun = -SunVector; //TODO: Calculate this with tangent for more accurate shadows!
            Sun.x *= 5;
            Sun.y -= 1;
            if (Sun.x == 0)
                Sun.x = 0.001f; //CANNOT LET SUN.X BE 0
            FrontLight.SetVector("_Sun", Sun);
            BackLight.SetVector("_Sun", Sun);
        }
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
        }
    }
}
