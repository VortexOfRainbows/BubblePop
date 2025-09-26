using System;
using UnityEngine;
using static UnityEditor.PlayerSettings;
public static class CoinManager
{
    public static void InitCoinPrefabs()
    {
        Bronze = Resources.Load<GameObject>("Money/BronzeCoin");
        Silver = Resources.Load<GameObject>("Money/SilverCoin");
        Gold = Resources.Load<GameObject>("Money/GoldCoin");
        Heart = Resources.Load<GameObject>("Money/HeartPickup");
    }
    public static void Load()
    {
        Savings = PlayerData.GetInt("Savings");
    }
    public static void Save()
    {
        PlayerData.SaveInt("Savings", Savings);
    }
    public static GameObject Bronze;
    public static GameObject Silver;
    public static GameObject Gold;
    public static GameObject Heart;
    public static void SpawnCoin(Vector2 pos, int value = 1, float collectDelay = 0f)
    {
        int bronze = value % 5;
        int silver = value / 5 % 5;
        int gold = value / 25;
        for (; bronze > 0; --bronze)
            Spawn(Bronze, pos, collectDelay);
        for (; silver > 0; --silver)
            Spawn(Silver, pos, collectDelay);
        for (; gold > 0; --gold)
            Spawn(Gold, pos, collectDelay);
    }
    public static void SpawnCoin(Func<Vector2> func, int value = 1, float collectDelay = 0f)
    {
        int bronze = value % 5;
        int silver = value / 5 % 5;
        int gold = value / 25;
        for (; bronze > 0; --bronze)
            Spawn(Bronze, func.Invoke(), collectDelay);
        for (; silver > 0; --silver)
            Spawn(Silver, func.Invoke(), collectDelay);
        for (; gold > 0; --gold)
            Spawn(Gold, func.Invoke(), collectDelay);
    }
    private static void Spawn(GameObject coinType, Vector2 pos, float collectDelay)
    {
        GameObject obj = GameObject.Instantiate(coinType, pos, Quaternion.identity);
        obj.GetComponent<Rigidbody2D>().velocity = Utils.RandCircle(8);
        obj.GetComponent<Coin>().BeforeCollectableTimer = collectDelay;
    }
    public static void SpawnHeart(Func<Vector2> func, float collectDelay)
    {
        GameObject obj = GameObject.Instantiate(Heart, func.Invoke(), Quaternion.identity);
        obj.GetComponent<Rigidbody2D>().velocity = Utils.RandCircle(4);
        obj.GetComponent<Coin>().BeforeCollectableTimer = collectDelay;
    }
    public static void SpawnHeart(Vector2 pos, float collectDelay)
    {
        GameObject obj = GameObject.Instantiate(Heart, pos, Quaternion.identity);
        obj.GetComponent<Rigidbody2D>().velocity = Utils.RandCircle(4);
        obj.GetComponent<Coin>().BeforeCollectableTimer = collectDelay;
    }
    public static int Current { get; private set; }
    public static int Savings { get; private set; }
    public static int TotalEquipCost;
    public static void AfterDeathTransfer()
    {
        ModifySavings(Current / 10);
        Current = 0;
    }
    public static void ModifySavings(int amt)
    {
        Savings += amt;
    }
    public static void ModifyCurrent(int amt)
    {
        Current += amt;
    }
}
