using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.FilePathAttribute;

public class EnemyWave
{
    public List<SubWave> subWaves = new List<SubWave>();
    public int waveCounter = 0;
    public bool Resolved = false;
    public float endDelay = 1000; //currently unused
    public void Resolve()
    {
        if(waveCounter >= subWaves.Count)
        {
            Finish();
        }
        else
        {
            subWaves[waveCounter].Resolve();
            if (subWaves[waveCounter].Resolved)
            {
                ++waveCounter;
            }
        }
    }
    public void Finish()
    {
        Resolved = true;
    }
}
public class SubWave
{
    public SubWave(float startUpDelay, float endDelay, List<EnemyPattern> patterns)
    {
        this.startUpDelay = startUpDelay;
        this.endDelay = endDelay;
        this.Patterns = patterns.ToArray();
    }
    public SubWave(float startUpDelay, float endDelay, params EnemyPattern[] patterns)
    {
        this.startUpDelay = startUpDelay;
        this.endDelay = endDelay;
        this.Patterns = patterns;
    }
    public EnemyPattern[] Patterns;
    public float startUpDelay = 100;
    public float endDelay = 100;
    private int patternNum = 0;
    private float counter = 0;
    public bool Resolved = false;
    public void Resolve()
    {
        if(++counter > startUpDelay)
        {
            if (patternNum < Patterns.Length)
            {
                Patterns[patternNum].Finish();
                counter -= Patterns[patternNum].EndDelay;
                ++patternNum;
            }
            else
            {
                if(counter > endDelay + startUpDelay)
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
    public GameObject[] EnemyPrefabs;
    public Vector2 Location;
    public float EndDelay = 50f;
    public float BetweenEnemyDelay = 20f;
    public void Finish()
    {
        Wormhole.Spawn(Location, EnemyPrefabs, BetweenEnemyDelay);
    }
}