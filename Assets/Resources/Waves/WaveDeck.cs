using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class WaveDeck
{
    private static float minXBound = -30, maxXBound = 30, minYBound = -20, maxYBound = 20;
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
        Vector2 stuff = new Vector2(Random.Range(minXBound, maxXBound), Random.Range(minYBound, maxYBound));
        return DrawSingleSpawn(stuff, DrawEnemy(EnemyID.AllEnemyList));
    }
    public static WaveCard DrawSingleSpawn(Vector2 location, GameObject enemy)
    {
        return new WaveCard(
            1 + 2 * enemy.GetComponent<Enemy>().CostMultiplier,
            Utils.RandFloat(2, 5), //Since single spawn cards are cheap, they have a long mulligan cooldown
            Utils.RandFloat(1, 2), //The cards drawn after this card also get a longer mulligan
            new EnemyPattern(location, 0, 0, enemy));
    }
}
