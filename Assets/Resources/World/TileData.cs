using UnityEngine;
using UnityEngine.Tilemaps;

public partial class World : MonoBehaviour
{
    public static bool ValidEnemySpawnTile(Vector3 pos)
    {
        Vector3Int posi = RealPosToTilePos(pos);
        var data = SafeGetTileData(posi);
        bool currentlyOnThisProgressionTier = data.ProgressionNumber == Main.PylonProgressionNumber;
        bool validSpawnTile = RealTileMap.Map.GetTile(posi) != TileID.DarkGrass.FloorTileType && !SafeGetTileData(posi).IsRoadblock;
        return !data.IsSolid && validSpawnTile && currentlyOnThisProgressionTier;
    }
    public static bool NonSolidTileSafe(Vector3 position)
    {
        return !SafeGetTileData(RealPosToTilePos(position)).IsSolid;
    }
    public static bool SolidTile(Vector3 worldPosition) => SolidTile(RealPosToTilePos(worldPosition));
    public static bool SolidTile(Vector3Int pos) => SolidTile(pos.x, pos.y);
    public static bool SolidTile(int x, int y) => UnsafeGetTileData(x, y).IsSolid;
    public static bool AreaIsClear(Vector3Int area, int squareRadius = 0)
    {
        for (int i = -squareRadius; i <= squareRadius; ++i)
            for (int j = -squareRadius; j <= squareRadius; ++j)
                if (HasTile(area.x + i, area.y + j))
                    return false;
        return true;
    }
    public static bool WithinBorders(Vector3 position, bool IncludeProgressionBounds)
    {
        bool roadblock = IncludeProgressionBounds && IsRoadblocked(position);
        return NonSolidTileSafe(position) && !roadblock;
    }
    public static bool IsRoadblocked(Vector3 position)
    {
        var data = SafeGetTileData(RealPosToTilePos(position));
        bool currentlyOnThisProgressionTier = data.ProgressionNumber > Main.PylonProgressionNumber;
        bool roadblock = currentlyOnThisProgressionTier || (data.IsRoadblock && Main.PylonActive);
        return roadblock;
    }
    public static void SetTile(Vector3Int pos, DualGridTile tile, bool solid)
    {
        //if (!Instance.ApproximateSize.Contains(pos))
        //    throw new System.Exception($"GENERROR: Tried placing tile: {pos}, worldbounds: {Instance.ApproximateSize}");
        ref TileData data = ref SafeGetTileData(pos);
        data.IsSolid = solid;
        data.TileType = tile;
        data.HasTile = true;
    }
    //public static void SetTilePlusProperties(Vector3Int pos, TileBase tile)
    //{
    //    World.RealTileMap.Map.SetTile(pos, tile);
    //}
    public static DualGridTile GetTile(int i, int j) => UnsafeGetTileData(i, j).TileType;
    public static DualGridTile GetTile(Vector3Int pos) => GetTile(pos.x, pos.y);
    public static bool HasTile(int i, int j) => UnsafeGetTileData(i, j).HasTile;
    public static bool HasTile(Vector3Int pos) => HasTile(pos.x, pos.y);
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
        public bool IsSolid;
        public bool HasTile;
        public TileData(byte progressionNum = byte.MaxValue, bool roadBlock = false)
        {
            ProgressionNumber = progressionNum;
            IsRoadblock = roadBlock;
            distance = float.MaxValue;
            direction = Vector2.zero;
            runID = 0;
            TileType = null;
            IsSolid = false;
            HasTile = false;
        }
    }
    private static Vector2Int tileDataOffset;
    private static TileData[,] tileData;
    private static TileData NoTileData = new(byte.MaxValue);
    public static readonly int Padding = 20;
    public static ref TileData SafeGetTileData(Vector3Int pos)
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
    public static ref TileData UnsafeGetTileData(int i, int j) => ref tileData[i - tileDataOffset.x, j - tileDataOffset.y];
    public static ref TileData GetTileData(int i, int j)
    {
        i -= tileDataOffset.x;
        j -= tileDataOffset.y;
        if (i < 0 || j < 0 || i >= tileData.GetLength(0) || j >= tileData.GetLength(1))
            return ref NoTileData;
        return ref tileData[i, j];
    }
    public static Vector2 GetTileDirection(Vector2Int pos) => UnsafeGetTileData(pos).direction;
    public static Vector2 GetDirection(Vector3 pos)
    {
        float x = pos.x / TilePathfinding.TileSize.x;
        float y = pos.y / TilePathfinding.TileSize.y;
        return GetTileDirection(new Vector2Int(Mathf.FloorToInt(x), Mathf.FloorToInt(y)));
    }
}
