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
        GetTileMap();
        TileMap.CompressBounds();
        Vector3Int transformPos = new((int)transform.position.x / 2, (int)transform.position.y / 2);
        var bounds = TileMap.cellBounds;
        int left = bounds.x;
        int right = left + bounds.size.x;
        int bottom = bounds.y;
        int top = bottom + bounds.size.y;
        for(int i = left; i < right; ++i)
        {
            for(int j = bottom; j < top; ++j)
            {
                Vector3Int v = new(i, j);
                var tile = TileMap.GetTile(v);
                if(tile != null)
                {
                    var change = new TileChangeData() { position = v + transformPos, tile = tile };
                    world.m_RealTileMap.m_RealTileMap.SetTile(change, true);
                }
            }
        }
        gameObject.SetActive(false);
    }
}
