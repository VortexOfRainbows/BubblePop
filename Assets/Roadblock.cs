using UnityEngine;

public class Roadblock : MonoBehaviour
{
    public Wormhole portal;
    public CircleCollider2D c2D;
    public byte ProgressionLevel = 0;
    public int counter = 0;
    public readonly Vector2[] dirs = new Vector2[] { new(1, 0), new(0, 1), new(-1, 0), new(0, -1) };
    public void FixedUpdate()
    {
        if(Main.PylonProgressionNumber > ProgressionLevel)
        {
            //The logic here should be like this:
            //If same level as portal, on

            //If one level behind the portal, on if wave is active

            //If two levels behind the portal, close
            portal.Closing = true;
            c2D.enabled = false;
        }
        else if(portal != null && portal.PlayerDistMult > 0)
        {
            ++counter;
            int i = (counter % 13) - 6;
            int j = ((counter / 13) % 13) - 6;
            i *= 2;
            j *= 2;
            var v = World.RealTileMap.Map.WorldToCell((Vector2)transform.position + new Vector2(i, -j));
            if (World.GetTileData(v).IsRoadblock && (Mathf.Abs(i) > 2 || Mathf.Abs(j) > 2))
            {
                Vector2 v2 = World.RealTileMap.Map.GetCellCenterWorld(v);
                float dist = ((Vector2)transform.position - v2).magnitude;
                float distM = (1 - dist / 12f);
                float alphaMult = distM * portal.PlayerDistMult * portal.PlayerDistMult;
                if (alphaMult > 0.05f)
                {
                    Vector2 dir = dirs[Utils.RandInt(4)];
                    Vector2 toPos = dir; // (Vector2)transform.position - v2;
                    if (World.GetTileData(v + new Vector3Int((int)toPos.x, (int)toPos.y)).IsRoadblock && distM > 0.2f)
                    {
                        var r = -toPos.ToRotation() * Mathf.Rad2Deg;
                        ParticleManager.NewParticle(v2 + dir * 0.6f, new Vector2(toPos.magnitude - 0.2f, .5f), Vector2.zero, 0, 2f, ParticleManager.ID.Line, Color.red.WithAlpha(alphaMult) * 2f, r);
                    }
                    ParticleManager.NewParticle(v2, 12, Vector2.zero, 0, 2, ParticleManager.ID.Pixel, Color.red.WithAlpha(alphaMult) * 2f);
                    if(Utils.RandFloat() < 0.3f)
                        ParticleManager.NewParticle(v2, new Vector2(7, 7), Vector2.zero, 0, 2, ParticleManager.ID.Pixel, Color.red.WithAlpha(alphaMult) * 2f, 0);
                }
            }
            //World.RealTileMap.Map.
        }
        transform.position = World.RealTileMap.Map.GetCellCenterWorld(World.RealTileMap.Map.WorldToCell(transform.position));
        if (portal == null)
            Destroy(gameObject);
    }
}
