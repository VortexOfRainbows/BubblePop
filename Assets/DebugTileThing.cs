using System.ComponentModel;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DebugTileThing : MonoBehaviour
{
    void Start()
    {
        var DepthTile = Resources.Load<Tile>("World/Tiles/DepthTile");
        Tilemap t = GetComponent<Tilemap>();
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
                t.SetTile(new TileChangeData(pos, DepthTile, World.RoadblockColor((byte)(255 - i)), Matrix4x4.identity), true);
            }
        }
    }
}
