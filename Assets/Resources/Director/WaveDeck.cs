using System.Collections.Generic;
using UnityEngine;
public static class WaveDeck
{
    public static readonly float minXBound = -30, maxXBound = 30, minYBound = -18, maxYBound = 18;
    public static List<GameObject> GetPossibleEnemies()
    {
        return WaveDirector.PossibleEnemies();
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
        Player target = Player.GetInstance(Utils.RandInt(Player.AllPlayers.Count));
        float multipleEnemiesChance = Mathf.Min(0.5f, 0.05f + WaveDirector.WaveNum * 0.025f);
        float swarmCircleChance = Mathf.Min(0.5f, 0.05f + WaveDirector.WaveNum * 0.025f);
        bool isCircle = Utils.RandFloat() < swarmCircleChance;
        int swarmCircleCount = 1;
        if(isCircle)
        {
            swarmCircleCount = 2;
            float bonusChance = 0.3f + Mathf.Sqrt(WaveDirector.WaveNum) * 0.1f;
            if (bonusChance > 0.9f)
                bonusChance = 0.9f;
            while (Utils.RandFloat() < bonusChance) //Increase the amount of portals in the swarm circle
                ++swarmCircleCount;
        }
        if(Utils.RandFloat() < multipleEnemiesChance)
        {
            int TotalDudes = 2;
            float bonusChance = 0.3f + WaveDirector.WaveNum * 0.1f;
            if (bonusChance > 0.8f)
                bonusChance = 0.8f;
            while (Utils.RandFloat() < bonusChance) //Increase the amount of enemies spawned from this circle
                ++TotalDudes;
            GameObject enemyType = DrawEnemy(GetPossibleEnemies());
            GameObject[] enemies = new GameObject[TotalDudes];
            for(int i = 0; i < TotalDudes; ++i)
            {
                enemies[i] = enemyType;
            }
            float time = 1.25f;
            var card = DrawMultiSpawn(RandomPositionOnPlayerEdge(target), 1 + TotalDudes * enemyType.GetComponent<Enemy>().CostMultiplier, time, enemies);
            if(isCircle)
            {
                if (Utils.RandFloat() < 0.5f)
                    card.Patterns[0].SpawnPattern = RightOnPlayer(target); //Circle center is on the player
                card = card.ToSwarmCircle(swarmCircleCount, 10, .75f, .5f);
            }
            return card;
        }
        var card2 = DrawSingleSpawn(RandomPositionOnPlayerEdge(target), DrawEnemy(GetPossibleEnemies()));
        if (isCircle)
        {
            if (Utils.RandFloat() < 0.5f) //50% chance for circle to spawn on top of the player. Otherwise it spawns on the originally determined location
                card2.Patterns[0].SpawnPattern = RightOnPlayer(target); //Circle center is on the player
            card2 = card2.ToSwarmCircle(swarmCircleCount, 10, .75f, .5f);
        }
        return card2;
    }
    public static ArbitrarySpawnPattern RandomPositionOnPlayerEdge(Player target)
    {
        return new ArbitrarySpawnPattern(target, () => target.Position + new Vector2(Random.Range(minXBound, maxXBound), Random.Range(minYBound, maxYBound)), true);
    }
    public static ArbitrarySpawnPattern RightOnPlayer(Player target)
    {
        return new ArbitrarySpawnPattern(target, () => Player.Instance1Pos, true);
    }
    //public static WaveCard DrawSingleSpawn(GameObject enemy)
    //{
        //return DrawSingleSpawn(RandomPositionOnPlayerEdge(), enemy);
    //}
    public static WaveCard DrawSingleSpawn(EnemySpawnPattern location, GameObject enemy)
    {
        Enemy e = enemy.GetComponent<Enemy>();
        return DrawSingleSpawn(location, enemy, 1 + e.CostMultiplier);
    }
    public static WaveCard DrawSingleSpawn(EnemySpawnPattern location, GameObject enemy, float cost)
    {
        return new WaveCard(cost, new EnemyPattern(location, 0, 0, false, enemy));
    }
    public static WaveCard DrawSingleSpawn(EnemyPattern p, float cost)
    {
        return new WaveCard(cost, p);
    }
    public static WaveCard DrawMultiSpawn(EnemySpawnPattern location, float cost, float interval, params GameObject[] enemy)
    {
        return new WaveCard(cost, new EnemyPattern(location, 0, interval, false, enemy));
    }
    public static WaveCard ToSwarmCircle(this WaveCard card, int count, float radius = 10f, float costMult = 1f, float endDelay = -1f)
    {
        EnemyPattern original = card.Patterns[0];
        if(endDelay > 0)
        {
            original.EndDelay = endDelay;
        }
        card.Patterns = new EnemyPattern[count];
        for (int i = 0; i < count; ++i)
        {
            Vector2 circular = new Vector2(0, radius).RotatedBy(i / (float)count * Mathf.PI * 2f);
            card.Patterns[i] = new(new SpawnWithOffset(original.SpawnPattern.Target, original.SpawnPattern, circular), 
                original.EndDelay, original.BetweenEnemyDelay, original.Skull, original.EnemyPrefabs);
        }
        card.Cost = card.Cost * count * costMult;
        return card;
    }
}
