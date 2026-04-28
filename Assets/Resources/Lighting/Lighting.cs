using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public static class Lighting
{
    public static Tile LightTile;
    public static Tile OcclusionTile;
    public static RenderTexture LightRT;
    public static Camera LightingCamera;
    //public static Sprite LightRTSprite;
    public static void Setup(Tilemap Map, Tilemap LightingFront, Tilemap LightingBack, Tilemap OcclusionMap)
    {
        if(Map == null || LightingFront == null || LightingBack == null)
        {
            throw new System.Exception("ERROR: Could not find lighting tile maps");
        }
        LightTile = Resources.Load<Tile>("Lighting/LightTile");
        OcclusionTile = Resources.Load<Tile>("Lighting/OcclusionLightTile");
        LightRT = Resources.Load<RenderTexture>("Lighting/LightingRenderTexture");
        LightingCamera = Camera.main.transform.GetChild(0).GetComponent<Camera>();
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
    public static void Update()
    {
        ResizeLightingRenderTexture();
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
            if (Main.Instance.TileLightRenderTarget != null)
                Main.Instance.TileLightRenderTarget.SetMaterialDirty(); //Updates the texel size on the gaussian blur to make the shadows consistent on resize
        }
        if (Main.Instance.TileLightRenderTarget != null)
        {
            float baseTexelSize = 4 / 1080f;
            Main.Instance.TileLightRenderTarget.material.SetVector("_TexelScaler", new Vector2(baseTexelSize / Camera.main.aspect, baseTexelSize));
        }
    }
}
