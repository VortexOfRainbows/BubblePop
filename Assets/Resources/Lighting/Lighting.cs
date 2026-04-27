using UnityEngine;
using UnityEngine.Tilemaps;

public static class Lighting
{
    public static Tile DepthTile;
    public static void Setup(Tilemap Map, Tilemap LightingFront, Tilemap LightingBack)
    {
        if(Map == null || LightingFront == null || LightingBack == null)
        {
            throw new System.Exception("ERROR: Could not find lighting tile maps");
        }
        DepthTile = World.DepthTile;
        Map.GetCorners(out int left, out int right, out int bottom, out int top);
        for (int i = left; i < right; i++)
        {
            for (int j = bottom; j < top; j++)
            {
                Vector3Int pos = new(i, j);
                if (World.SolidTile(pos)) //This is also used for occlusion so it is obtained when typically setting up the tile maps... Additionally, it could be used to check for solid tiles quicker, but im not certain if it is faster (NEEDS TESTING)
                {
                    LightingFront.SetTile(pos, DepthTile);
                    LightingBack.SetTile(pos, DepthTile);
                }
            }
        }
    }
}
