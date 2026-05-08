using System.ComponentModel;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DebugTileThing : MonoBehaviour
{
    void Start()
    {
        var DepthTile = Resources.Load<Tile>("World/Tiles/DepthTile");
        Tilemap t = GetComponent<Tilemap>();
        double sum = 0;
        double increment = 1.0 / 255.0;
        for (int i = 0; i < 256; ++i)
        {
            for (int j = 0; j < 5; ++j)
            {
                Vector3Int pos = new(i, j);
                t.SetTile(new TileChangeData(pos, DepthTile, World.RoadblockColor((byte)i), Matrix4x4.identity), true);
            }
            for (int j = 0; j < 5; ++j)
            {
                Vector3Int pos = new(i, j + 5);
                t.SetTile(new TileChangeData(pos, DepthTile, new Color((float)sum, 0, 0), Matrix4x4.identity), true);
            }
            sum += increment;
        }
    }
}
