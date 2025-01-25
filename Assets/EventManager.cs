using UnityEngine;

public static class EventManager
{
    private static float bathBombTimer = 0;
    public static void Update()
    {
        bathBombTimer -= Time.deltaTime;
        if (bathBombTimer < 0)
        {
            SpawnBathBomb();
            bathBombTimer = Utils.RandFloat(5, 8); //5 to 8 seconds for another bath bomb
        }
    }
    public static void SpawnBathBomb()
    {
        Vector2 randPos = new Vector2(Utils.RandFloat(-12, 12), Utils.RandFloat(-7, 7));
        Projectile.NewProjectile(Player.Position + new Vector2(randPos.x, 30), Vector2.zero, 1, Player.Position.y + randPos.y);
    }
}
