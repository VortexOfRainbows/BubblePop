using UnityEngine;

public static class EventManager
{
    public static int Point
    {
        get => UIManager.score;
        set => UIManager.score = value;
    }
    public static bool InsideBathtub(Vector2 pos)
    {
        pos.x *= 0.3f;
        return pos.magnitude <= 20f;
    }
    public static bool CanSpawnPower()
    {
        return PointsSpent < Point && Utils.RandFloat(1.0f) < 0.25f;
    }
    public static float PointsSpent = 0;
    private static float PointTimer = 0;
    private static float SoapTimer, DuckTimer, FlamingoTimer, MadLadTimer;
    private static float minXBound = -30, maxXBound = 30, minYBound = -20, maxYBound = 20;
    public static void Update()
    {
        if(Input.GetKeyDown(KeyCode.T))
        {
            Point += 100;
        }
        PointTimer += Time.deltaTime;
        if (PointTimer > 1)
        {
            Point++;
            PointTimer--;
        }
        bathBombTimer -= Time.deltaTime;
        if (bathBombTimer < 0)
        {
            SpawnBathBomb();
            float minTime = GetSpawnTime(6, 1, 0, 5000);
            float maxTime = GetSpawnTime(8, 2, 0, 10000);
            bathBombTimer = Utils.RandFloat(minTime, maxTime); //5 to 8 seconds for another bath bomb
        }
        EnemySpawning(ref DuckTimer, 1f, GetSpawnTime(10, 5, 10, 1000), GlobalDefinitions.Ducky);
        EnemySpawning(ref SoapTimer, 1f, GetSpawnTime(20, 7, 100, 1100), GlobalDefinitions.Soap);
        EnemySpawning(ref FlamingoTimer, 1f, GetSpawnTime(30, 20, 300, 1800), GlobalDefinitions.flamingoFloatie);
        EnemySpawning(ref MadLadTimer, 1f, GetSpawnTime(90, 30, 1000, 10000), GlobalDefinitions.FinalDuck);
    }
    private static float GetSpawnTime(float max, float min, float minimumThreshhold, float maxThreshold)
    {
        if(Point < minimumThreshhold)
        {
            return -1;
        }
        float dist = maxThreshold - minimumThreshhold;
        float percent = (Point - minimumThreshhold) / dist;
        return Mathf.Lerp(max, min, percent);
    }
    private static void EnemySpawning(ref float SpawnTimer, float SpawnTimerSpeedScaling, float respawnTime, GameObject Enemy)
    {
        if(SpawnTimer > respawnTime && respawnTime > 0)
        {
            Vector2 stuff = Player.Position + new Vector2(Random.Range(minXBound, maxXBound), Random.Range(minYBound, maxYBound));
            if ((stuff - Player.Position).magnitude < 24)
            {
                stuff -= Player.Position;
                stuff = Player.Position + stuff.normalized * 24;
            }
            int att = 0;
            while (!InsideBathtub(stuff))
            {
                stuff = Player.Position + new Vector2(Random.Range(minXBound, maxXBound), Random.Range(minYBound, maxYBound));
                stuff *= 1f - (att / 100f);
                if ((stuff - Player.Position).magnitude < 24)
                {
                    stuff -= Player.Position;
                    stuff = Player.Position + stuff.normalized * 24;
                }
                if (++att > 100)
                {
                    Debug.Log("Fail to spawn");
                    return;
                }
            }
            GameObject.Instantiate(Enemy, stuff, Quaternion.identity);
            SpawnTimer = Utils.RandFloat(-0.5f, 0.5f) * respawnTime;
        }
        else
        {
            SpawnTimer += Time.deltaTime * SpawnTimerSpeedScaling;
        }
    }
    private static float bathBombTimer = 0;
    public static int GetBathBombType()
    {
        float[] weights = new float[] { 2, 1, 0.1f, 0, 0, 0, 0 };
        if (Point < 200)
        {
            if (Point > 100)
                weights = new float[] { 2, 2, 1, 0.2f, 0, 0, 0 };
        }
        else if (Point < 400)
            weights = new float[] { 1, 2, 2, 1, 0.2f, 0.1f, 0 };
        else if (Point < 700)
            weights = new float[] { 0.1f, 1, 2, 2, 1, 0.1f, 0 };
        else if (Point < 1000)
            weights = new float[] { 0.1f, 1, 2, 2, 2, 1, 0.1f };
        else if (Point < 1400)
            weights = new float[] { 0.1f, 0.2f, 1, 2, 2, 2, 1 };
        else if (Point < 2000)
            weights = new float[] { 0.1f, 0.2f, 0.3f, 1, 2, 2, 1 };
        else if (Point < 3000)
            weights = new float[] { 0.1f, 0.2f, 0.3f, 1, 2, 3, 2 };
        else
            weights = new float[] { 0.1f, 0.2f, 0.3f, 1, 2, 3, 7 };
        float total = 0f;
        for (int i = 0; i < weights.Length; i++)
        {
            total += weights[i];
        }
        //Debug.Log("t: " + total);
        float rand = Utils.RandFloat(total);
        //Debug.Log("rand: " + rand);
        for (int i = 0; i < weights.Length; i++)
        {
            if (rand < weights[i])
            {
                ///Debug.Log(rand + ": " + i);
                return i;
            }
            else
                rand -= weights[i];
        }
        return 0;
    }
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
        Projectile.NewProjectile(Player.Position + new Vector2(randPos.x, 30), Vector2.zero, 1, Player.Position.y + randPos.y, GetBathBombType());
    }
}
