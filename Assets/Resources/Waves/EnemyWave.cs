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
        this.mulliganDelay = startUpDelay;
        this.postPlayDelay = endDelay;
        this.Patterns = patterns.ToArray();
        this.Cost = cost;
    }
    public WaveCard(float cost, float startUpDelay, float endDelay, params EnemyPattern[] patterns)
    {
        this.mulliganDelay = startUpDelay;
        this.postPlayDelay = endDelay;
        this.Patterns = patterns;
        this.Cost = cost;
    }
    public EnemyPattern[] Patterns;
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
        Wormhole.Spawn(RelativeLocationToPlayer ? Location + Player.Position : Location, EnemyPrefabs, BetweenEnemyDelay);
    }
}