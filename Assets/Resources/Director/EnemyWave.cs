using System.Collections.Generic;
using UnityEngine;
using System;
using static UnityEngine.RuleTile.TilingRuleOutput;
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
public abstract class EnemySpawnPattern
{
    public virtual float MinimumDistanceToPlayer => 12;
    public Player Target => Player.Instance;
    public virtual Vector2 GenerateLocation()
    {
        return Target.transform.position;
    }
    /// <summary>
    /// How regenerating the location should be handled if the location was invalid, such as it was in a tile
    /// </summary>
    /// <returns></returns>
    public virtual Vector2 RegenerateLocation(int attemptNumber, bool finalGeneration = false)
    {
        return GenerateLocation();
    }
}
public class ArbitrarySpawnPattern : EnemySpawnPattern
{
    public Func<Vector2> locationFunction = null;
    public Vector2? Location = null;
    private readonly bool OnlyGenOnce;
    public ArbitrarySpawnPattern(Func<Vector2> location, bool onlyGenerateOnce = false)
    {
        locationFunction = location;
        OnlyGenOnce = onlyGenerateOnce;
    }
    public ArbitrarySpawnPattern(Vector2 location)
    {
        Location = location;
    }
    public override Vector2 GenerateLocation()
    {
        if(OnlyGenOnce)
        {
            if(Location == null)
                Location = locationFunction.Invoke();
            return Location.Value;
        }
        return locationFunction != null ? (Location = locationFunction.Invoke()).Value : Location.Value;
    }
    public override Vector2 RegenerateLocation(int attemptNumber, bool finalGeneration = false)
    {
        if(locationFunction == null)
        {
            Vector2 toPlayer = (Player.Position - Location.Value).normalized;
            Location += toPlayer * 1.25f + Utils.RandCircle(2f);
            if (finalGeneration)
                return Location.Value;
        }
        Location = null; //Since the location was invalid, we regenerate
        return GenerateLocation();
    }
}
public class CircleSpawnPattern : EnemySpawnPattern
{
    public override float MinimumDistanceToPlayer => 0;
    public Vector2 CircularOffset;
    public EnemySpawnPattern CircularCenter;
    public Vector2 Location;
    public CircleSpawnPattern(EnemySpawnPattern center, Vector2 offset)
    {
        CircularCenter = center;
        CircularOffset = offset;
    }
    public override Vector2 GenerateLocation()
    {
        return Location = CircularCenter.GenerateLocation() + CircularOffset;
    }
    public override Vector2 RegenerateLocation(int attemptNumber, bool finalGeneration = false)
    {
        Vector2 toPlayer = (Player.Position - Location).normalized;
        Location += toPlayer * 1.25f + Utils.RandCircle(2f);
        if (finalGeneration)
            return Location;
        return Location;
    }
}
public class EnemyPattern
{
    public EnemySpawnPattern SpawnPattern;
    public EnemyPattern(EnemySpawnPattern location, float endDelay, float betweenEnemyDelay, bool isSkull, params GameObject[] prefabs)
    {
        SpawnPattern = location;
        EndDelay = endDelay;
        BetweenEnemyDelay = betweenEnemyDelay;
        EnemyPrefabs = prefabs;
        Skull = isSkull;
    }
    public bool RelativeLocationToPlayer = true;
    public GameObject[] EnemyPrefabs;
    public float EndDelay = 0.50f;
    public float BetweenEnemyDelay = 0.2f;
    public bool Skull { get; set; } = false;
    public void Release()
    {
        Vector2 spawnPos = ShiftLocationIfOutOfBounds(SpawnPattern.GenerateLocation());
        Wormhole.Spawn(spawnPos, EnemyPrefabs, Skull, BetweenEnemyDelay / Time.fixedDeltaTime);
    }
    public Vector2 ShiftLocationIfOutOfBounds(Vector2 location)
    {
        Vector2 toPlayer = Player.Position - location;
        float dist = toPlayer.magnitude;
        toPlayer = toPlayer.normalized;
        float minDist = SpawnPattern.MinimumDistanceToPlayer;
        if (dist < minDist)
        {
            location = Player.Position - toPlayer * minDist;
        }
        int att = -1;
        while (!ValidLocation(location) && ++att <= 200)
        {
            location = SpawnPattern.RegenerateLocation(att, att == 200);
        }
        return location;
    }
    public bool ValidLocation(Vector2 location)
    {
        bool hasWorldMap = World.RealTileMap != null && World.RealTileMap.Map != null;
        if(hasWorldMap)
            for(int i = -1; i <= 1; ++i)
                for(int j = -1; j <= 1; ++j)
                    if (!World.ValidEnemySpawnTile(location + new Vector2(i, j) * 2))
                        return false;
        return true;
    }
}