using System.Collections.Generic;
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
        if (WaveDirector.WaveNum > 1 || WaveDirector.CardsPlayed > 5)
            possibleEnemies.Add(EnemyID.OldSoap);
        if (WaveDirector.WaveNum > 1)
            possibleEnemies.Add(EnemyID.Chicken);
        if (WaveDirector.WaveNum > 2)
            possibleEnemies.Add(EnemyID.Crow);
        if (WaveDirector.WaveNum > 3)
            possibleEnemies.Add(EnemyID.OldFlamingo);
        if (WaveDirector.WaveNum > 5 && WaveDirector.CardsPlayed > 5)
            possibleEnemies.Add(EnemyID.Gatligator);
        if (WaveDirector.WaveNum > 7 && WaveDirector.CardsPlayed > 2)
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
        float chanceForWaveSpawn = Mathf.Min(0.25f, 0.01f + WaveDirector.WaveNum * 0.01f);
        float chanceForSwarm = Mathf.Min(0.25f, 0.01f + WaveDirector.WaveNum * 0.01f);
        bool isSwarm = Utils.RandFloat() < chanceForSwarm;
        int swarmCount = 1;
        if(isSwarm)
        {
            swarmCount = 3;
            float bonusChance = 0.3f + Mathf.Sqrt(WaveDirector.WaveNum) * 0.1f;
            if (bonusChance > 0.9f)
                bonusChance = 0.9f;
            while (Utils.RandFloat() < bonusChance)
                ++swarmCount;
        }
        if(Utils.RandFloat() < chanceForWaveSpawn)
        {
            int TotalDudes = Utils.RandInt(2, 4);
            float bonusChance = 0.3f + WaveDirector.WaveNum * 0.1f;
            if (bonusChance > 0.8f)
                bonusChance = 0.8f;
            while (Utils.RandFloat() < bonusChance)
                ++TotalDudes;
            GameObject enemyType = DrawEnemy(GetPossibleEnemies());
            GameObject[] enemies = new GameObject[TotalDudes];
            for(int i = 0; i < TotalDudes; ++i)
            {
                enemies[i] = enemyType;
            }
            float time = 1.25f;
            var card = DrawMultiSpawn(RandomEdgeLocation(), 10, 5, 1 + TotalDudes * enemyType.GetComponent<Enemy>().CostMultiplier, time, enemies);
            if(isSwarm)
            {
                if (Utils.RandFloat() < 0.5f)
                    card.Patterns[0].Location = Vector2.zero;
                card = card.ToSwarmCircle(swarmCount, 10, 1.0f, 0.5f);
            }
            return card;
        }
        var card2 = DrawSingleSpawn(RandomEdgeLocation(), DrawEnemy(GetPossibleEnemies()));
        if (isSwarm)
        {
            if (Utils.RandFloat() < 0.5f)
                card2.Patterns[0].Location = Vector2.zero;
            card2 = card2.ToSwarmCircle(swarmCount, 10, 1.0f, 0.5f);
        }
        return card2;
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
        return DrawSingleSpawn(location, enemy, Utils.RandFloat(4, 10), Utils.RandFloat(1, 2), 1 + 2 * enemy.GetComponent<Enemy>().CostMultiplier);
    }
    public static WaveCard DrawSingleSpawn(Vector2 location, GameObject enemy, float charge, float delay, float cost)
    {
        return new WaveCard(
            cost,
            charge,
            delay,
            new EnemyPattern(location, 0, 0, enemy));
    }
    public static WaveCard DrawSingleSpawn(EnemyPattern p, float charge, float delay, float cost)
    {
        return new WaveCard(
            cost,
            charge,
            delay,
            p);
    }
    public static WaveCard DrawMultiSpawn(Vector2 location, float charge, float delay, float cost, float interval, params GameObject[] enemy)
    {
        return new WaveCard(
            cost,
            charge,
            delay,
            new EnemyPattern(location, 0, interval, enemy));
    }
    public static WaveCard ToSwarmCircle(this WaveCard card, int count, float radius = 10f, float costMult = 1f, float endDelay = -1f)
    {
        EnemyPattern original = card.Patterns[0];
        if(endDelay > 0)
        {
            original.EndDelay = endDelay;
        }
        card.Patterns = new EnemyPattern[count];
        for(int i = 0; i < count; ++i)
        {
            Vector2 circular = new Vector2(0, radius).RotatedBy(i / (float)count * Mathf.PI * 2f);
            card.Patterns[i] = new(original.Location + circular, original.EndDelay, original.BetweenEnemyDelay, original.EnemyPrefabs);
        }
        card.Cost = (1 + card.Cost + count) * costMult;
        return card;
    }
}
