using UnityEngine;
using UnityEngine.Tilemaps;

public partial class World : MonoBehaviour
{
    public static void SetTile(Vector3Int pos, TileBase tile)
    {
        World.RealTileMap.Map.SetTile(pos, tile);
    }
    public static TileBase GetTile(int i, int j) => GetTile(new Vector3Int(i, j));
    public static TileBase GetTile(Vector3Int pos)
    {
        return World.RealTileMap.Map.GetTile(pos);
    }
    public static bool HasTile(int i, int j) => HasTile(new Vector3Int(i, j));
    public static bool HasTile(Vector3Int pos)
    {
        return RealTileMap.Map.HasTile(pos);
    }
    public static Vector3 CenterOfTile(Vector3Int tilePos)
    {
        return RealTileMap.Map.GetCellCenterWorld(tilePos);
    }
    public struct TileData
    {
        public DualGridTile TileType;
        public byte ProgressionNumber;
        public bool IsRoadblock;
        public float distance;
        public Vector2 direction;
        public int runID;
        //public int testID;
        public TileData(byte progressionNum = byte.MaxValue, bool roadBlock = false)
        {
            ProgressionNumber = progressionNum;
            IsRoadblock = roadBlock;
            distance = float.MaxValue;
            direction = Vector2.zero;
            runID = 0;
            TileType = null;
        }
    }
    private static Vector2Int tileDataOffset;
    private static TileData[,] tileData;
    private static TileData NoTileData = new(byte.MaxValue);
    public static readonly int Padding = 20;
    public static ref TileData GetTileData(Vector3Int pos)
    {
        Vector2Int pointPos = (Vector2Int)pos - tileDataOffset;
        if (pointPos.x < 0 || pointPos.y < 0 || pointPos.x >= tileData.GetLength(0) || pointPos.y >= tileData.GetLength(1))
            return ref NoTileData;
        return ref tileData[pointPos.x, pointPos.y];
    }
    public static ref TileData UnsafeGetTileData(Vector3Int pos) => ref UnsafeGetTileData((Vector2Int)pos);
    public static ref TileData UnsafeGetTileData(Vector2Int pos)
    {
        Vector2Int pointPos = pos - tileDataOffset;
        return ref tileData[pointPos.x, pointPos.y];
    }
    public static Vector2 GetTileDirection(Vector2Int pos) => UnsafeGetTileData(pos).direction;
    public static Vector2 GetDirection(Vector3 pos)
    {
        float x = pos.x / TilePathfinding.TileSize.x;
        float y = pos.y / TilePathfinding.TileSize.y;
        return GetTileDirection(new Vector2Int(Mathf.FloorToInt(x), Mathf.FloorToInt(y)));
    }
}
