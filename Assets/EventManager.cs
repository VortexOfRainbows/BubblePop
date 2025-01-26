using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public static class EventManager
{
    public static bool InsideBathtub(Vector2 pos)
    {
        pos.x *= 0.28f;
        return pos.magnitude <= 22f;
    }
    private static int enemySpawnTimer = 30;
    private static int minSpawnTime = 300, maxSpawnTime = 600;
    private static float minXBound = -30, maxXBound = 30, minYBound = -20, maxYBound = 20;
    public static void Update()
    {
        enemySpawnTimer--;
        if (enemySpawnTimer <= 0)
        {
            SpawnRandomEnemy();
            enemySpawnTimer = Random.Range(minSpawnTime, maxSpawnTime);
        }
        bathBombTimer -= Time.deltaTime;
        if (bathBombTimer < 0)
        {
            SpawnBathBomb();
            bathBombTimer = Utils.RandFloat(5, 8); //5 to 8 seconds for another bath bomb
        }
    }
    public static void SpawnRandomEnemy()
    {
        // Spawn instance of random enemy
        Vector2 stuff = Player.Position + new Vector2(Random.Range(minXBound, maxXBound), Random.Range(minYBound, maxYBound));
        if ((stuff - Player.Position).magnitude < 20)
        {
            stuff -= Player.Position;
            stuff = Player.Position + stuff.normalized * 20;
        }
        int att = 0;
        while (!InsideBathtub(stuff))
        {
            stuff = Player.Position + new Vector2(Random.Range(minXBound, maxXBound), Random.Range(minYBound, maxYBound));
            stuff *= 1 - (att / 100f);
            if((stuff - Player.Position).magnitude < 20)
            {
                stuff -= Player.Position;
                stuff = Player.Position + stuff.normalized * 20;
            }
            if (++att > 100)
            {
                Debug.Log("Fail to spawn");
                return;
            }
        }
        if (Utils.RandFloat(1) < 0.3f)
        {
            GameObject.Instantiate(GlobalDefinitions.Ducky, stuff, Quaternion.identity);
        }
        else
            GameObject.Instantiate(GlobalDefinitions.Soap, stuff, Quaternion.identity);
    }
    private static float bathBombTimer = 0;
    public static void SpawnBathBomb()
    {
        Vector2 randPos = new Vector2(Utils.RandFloat(-12, 12), Utils.RandFloat(-7, 7));
        Vector2 predictedPosition = Player.Position + randPos;
        int att = 0;
        while (!InsideBathtub(predictedPosition))
        {
            predictedPosition = Player.Position + new Vector2(Utils.RandFloat(-12, 12), Utils.RandFloat(-7, 7));
            if (++att > 100)
            {
                Debug.Log("Fail to spawn");
                break;
            }
        }
        Projectile.NewProjectile(Player.Position + new Vector2(randPos.x, 30), Vector2.zero, 1, Player.Position.y + randPos.y);
    }
}
