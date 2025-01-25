using UnityEngine;

public static class EventManager
{
    private static int enemySpawnTimer = 30;
    private static int minSpawnTime = 300, maxSpawnTime = 600;
    private static float minXBound = 25, maxXBound = 30, minYBound = 15, maxYBound = 20;
    public static void Update()
    {
        enemySpawnTimer--;
        if (enemySpawnTimer <= 0)
        {
            //SpawnRandomEnemy();
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
        Vector3 stuff = new Vector3(Random.Range(minXBound, maxXBound) * (Random.Range(0, 1) * 2 - 1), Random.Range(minYBound, maxYBound) * (Random.Range(0, 1) * 2 - 1));
        if(Utils.RandFloat(1) < 0.3f)
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
        Projectile.NewProjectile(Player.Position + new Vector2(randPos.x, 30), Vector2.zero, 1, Player.Position.y + randPos.y);
    }
}
