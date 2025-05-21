using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class WaveDeck
{
    public static readonly float minXBound = -30, maxXBound = 30, minYBound = -18, maxYBound = 18;
    public static List<GameObject> GetPossibleEnemies()
    {
        List<GameObject> possibleEnemies = new()
        {
            EnemyID.OldDuck
        };
        if (WaveDirector.WaveNum > 1 || WaveDirector.CardsPlayed > 4)
            possibleEnemies.Add(EnemyID.OldSoap);
        if (WaveDirector.WaveNum > 1 && WaveDirector.CardsPlayed > 2)
            possibleEnemies.Add(EnemyID.Chicken);
        if (WaveDirector.WaveNum > 2 || WaveDirector.CardsPlayed > 4)
            possibleEnemies.Add(EnemyID.OldFlamingo);
        if (WaveDirector.WaveNum > 3 && WaveDirector.CardsPlayed > 2)
            possibleEnemies.Add(EnemyID.OldLeonard);
        return possibleEnemies;
    }
    public static GameObject DrawEnemy(List<GameObject> possibleEnemies)
    {
        return possibleEnemies[Utils.RandInt(possibleEnemies.Count)];
    }
    public static GameObject TryDrawingExpensiveEnemy(List<GameObject> possibleEnemies, int tries = 2)
    {
        List<GameObject> considerations = new List<GameObject>();
        while (tries-- > 0)
        {
            considerations.Add(DrawEnemy(possibleEnemies));
        }
        int bestIndex = 0;
        float bestCost = considerations[bestIndex].GetComponent<Enemy>().CostMultiplier;
        for(int i = 1; i < considerations.Count; ++i)
        {
            float cost = considerations[i].GetComponent<Enemy>().CostMultiplier;
            if(cost > bestCost)
            {
                bestIndex = i;
                bestCost = cost;
            }
        }
        return considerations[bestIndex];
    }
    public static GameObject TryDrawingCheapEnemy(List<GameObject> possibleEnemies, int tries = 2)
    {
        List<GameObject> considerations = new List<GameObject>();
        while (tries-- > 0)
        {
            considerations.Add(DrawEnemy(possibleEnemies));
        }
        int bestIndex = 0;
        float bestCost = considerations[bestIndex].GetComponent<Enemy>().CostMultiplier;
        for (int i = 1; i < considerations.Count; ++i)
        {
            float cost = considerations[i].GetComponent<Enemy>().CostMultiplier;
            if (cost < bestCost)
            {
                bestIndex = i;
                bestCost = cost;
            }
        }
        return considerations[bestIndex];
    }
    public static WaveCard DrawCard()
    {
        return DrawSingleSpawn(RandomEdgeLocation(), DrawEnemy(GetPossibleEnemies()));
    }
    public static Vector2 RandomEdgeLocation()
    {
        return new(Random.Range(minXBound, maxXBound), Random.Range(minYBound, maxYBound));
    }
    public static WaveCard DrawSingleSpawn(GameObject enemy)
    {
        return DrawSingleSpawn(RandomEdgeLocation(), enemy);
    }
    public static WaveCard DrawSingleSpawn(Vector2 location, GameObject enemy)
    {
        return DrawSingleSpawn(location, enemy, Utils.RandFloat(3, 9), Utils.RandFloat(1, 2), 1 + 2 * enemy.GetComponent<Enemy>().CostMultiplier);
    }
    public static WaveCard DrawSingleSpawn(Vector2 location, GameObject enemy, float charge, float delay, float cost)
    {
        return new WaveCard(
            cost,
            charge, //Since single spawn cards are cheap, they have a long mulligan cooldown
            delay, //The cards drawn after this card also get a longer mulligan
            new EnemyPattern(location, 0, 0, enemy));
    }
}
