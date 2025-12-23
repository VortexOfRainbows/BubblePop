using UnityEngine;
using UnityEngine.Tilemaps;

#if UNITY_EDITOR
[ExecuteAlways]
#endif

public class WorldNode : MonoBehaviour
{
    public void Start()
    {
        RoundPosition();
    }
    public void RoundPosition()
    {
        Vector3Int transformPos = new((int)transform.position.x / 2, (int)transform.position.y / 2);
        transform.position = transformPos * 2;
    }
    #if UNITY_EDITOR
    public void Update() => RoundPosition();
    #endif
    [SerializeField]
    private Tilemap TileMap;
    public Tilemap GetTileMap()
    {
        TileMap = TileMap != null ? TileMap : GetComponentInChildren<Tilemap>();
        return TileMap;
    }
    public void Generate(World world)
    {
        Vector3Int transformPos = new((int)transform.position.x / 2, (int)transform.position.y / 2);
        GetTileMap();
        TileMap.GetCorners(out int left, out int right, out int bottom, out int top);
        for(int i = left; i < right; ++i)
        {
            for(int j = bottom; j < top; ++j)
            {
                Vector3Int v = new(i, j);
                var tile = TileMap.GetTile(v);
                if(tile != null)
                {
                    world.Tilemap.Map.SetTile(v + transformPos, tile);
                }
            }
        }
        gameObject.SetActive(false);
    }
}
