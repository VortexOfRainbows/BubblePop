using System.Collections.Generic;
using UnityEngine;

public class WaveCard 
{
    public override string ToString()
    {
        return $"[P: {Patterns.Length}, C: {Cost}, MS: {mulliganDelay}]";
    }
    public WaveCard(float cost, float startUpDelay, float endDelay, List<EnemyPattern> patterns)
    {
        this.mulliganDelay = fullDelay = startUpDelay;
        this.postPlayDelay = endDelay;
        this.Patterns = patterns.ToArray();
        this.Cost = cost;
    }
    public WaveCard(float cost, float startUpDelay, float endDelay, params EnemyPattern[] patterns)
    {
        this.mulliganDelay = fullDelay = startUpDelay;
        this.postPlayDelay = endDelay;
        this.Patterns = patterns;
        this.Cost = cost;
    }
    public EnemyPattern[] Patterns;
    public float fullDelay = 100;
    public float mulliganDelay = 100;
    /// <summary>
    /// Additional delay put on the next card drawn.
    /// Currently unimplemented
    /// </summary>
    public float postPlayDelay = 100;
    private int patternNum = 0;
    private float counter = 0;
    public bool Resolved = false;
    public float Cost = 100;
    public void Resolve()
    {
        if(++counter > mulliganDelay)
        {
            if (patternNum < Patterns.Length)
            {
                Patterns[patternNum].Release();
                counter -= Patterns[patternNum].EndDelay;
                ++patternNum;
            }
            else
            {
                if(counter > postPlayDelay + mulliganDelay)
                {
                    Finish();
                }
            }
        }
    }
    public void Finish()
    {
        Resolved = true;
    }
}
public class EnemyPattern
{
    public EnemyPattern(Vector2 location, float endDelay, float betweenEnemyDelay, params GameObject[] prefabs)
    {
        Location = location;
        EndDelay = endDelay;
        BetweenEnemyDelay = betweenEnemyDelay;
        EnemyPrefabs = prefabs;
    }
    public bool RelativeLocationToPlayer = true;
    public GameObject[] EnemyPrefabs;
    public Vector2 Location;
    public float EndDelay = 50f;
    public float BetweenEnemyDelay = 20f;
    public void Release()
    {
        Vector2 spawnPos = ShiftLocationIfOutOfBounds(RelativeLocationToPlayer ? Location + Player.Position : Location);
        Wormhole.Spawn(spawnPos, EnemyPrefabs, BetweenEnemyDelay);
    }
    public Vector2 ShiftLocationIfOutOfBounds(Vector2 stuff)
    {
        if ((stuff - Player.Position).magnitude < 12)
        {
            stuff -= Player.Position;
            stuff = Player.Position + stuff.normalized * 12;
        }
        int att = 0;
        Vector2 toPlayer = (Player.Position - stuff).normalized;
        while (!InWorld(stuff))
        {
            stuff += toPlayer * 1f + Utils.RandCircle(1.5f);
            if (++att > 200)
                return stuff;
        }
        return stuff;
    }
    public bool InWorld(Vector2 location)
    {
        bool hasWorldMap = DualGridTilemap.Instance != null && DualGridTilemap.RealTileMap != null;
        if(hasWorldMap)
        {
            int success = 0;
            for(int i = -1; i <= 1; ++i)
            {
                for(int j = -1; j <= 1; ++j)
                {
                    if (DualGridTilemap.RealTileMap.HasTile(DualGridTilemap.RealTileMap.WorldToCell(location) + new Vector3Int(i, j)))
                    {
                        success++;
                        if (success > 7)
                            return true;
                    }
                }
            }
            return success > 6;
        }
        else
        {
            return WaveDirector.InsideBathtub(location);
        }
    }
}