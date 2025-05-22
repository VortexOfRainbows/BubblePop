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
    public float fullDelay = 1;
    public float mulliganDelay = 1;
    /// <summary>
    /// Additional delay put on the next card drawn.
    /// Currently unimplemented
    /// </summary>
    public float postPlayDelay = 1;
    private int patternNum = 0;
    public bool Resolved = false;
    public float Cost = 100;
    public float resolveCounter = 0;
    public void Resolve()
    {
        resolveCounter += Time.fixedDeltaTime;
        if (resolveCounter > 0)
        {
            if (patternNum < Patterns.Length)
            {
                Patterns[patternNum].Release();
                resolveCounter -= Patterns[patternNum].EndDelay;
                ++patternNum;
            }
            else
            {
                if(resolveCounter > postPlayDelay)
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
    public float EndDelay = 0.50f;
    public float BetweenEnemyDelay = 0.2f;
    public void Release()
    {
        Vector2 spawnPos = ShiftLocationIfOutOfBounds(RelativeLocationToPlayer ? Location + Player.Position : Location);
        Wormhole.Spawn(spawnPos, EnemyPrefabs, BetweenEnemyDelay / Time.fixedDeltaTime);
    }
    public Vector2 ShiftLocationIfOutOfBounds(Vector2 stuff)
    {
        Vector2 toPlayer = Player.Position - stuff;
        float dist = toPlayer.magnitude;
        toPlayer = toPlayer.normalized;
        if (dist < 12)
        {
            stuff = Player.Position - toPlayer * 12;
        }
        int att = 0;
        while (!InWorld(stuff))
        {
            toPlayer = (Player.Position - stuff).normalized;
            stuff += toPlayer * 1.25f + Utils.RandCircle(2f);
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
            for(int i = -1; i <= 1; ++i)
            {
                for(int j = -1; j <= 1; ++j)
                {
                    if (!DualGridTilemap.RealTileMap.HasTile(DualGridTilemap.RealTileMap.WorldToCell(location) + new Vector3Int(i, j)))
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        else
        {
            return WaveDirector.InsideBathtub(location);
        }
    }
}