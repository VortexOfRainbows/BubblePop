using Unity.VisualScripting;
using UnityEngine;

public static class CoinManager
{
    public static void InitCoinPrefabs()
    {
        Bronze = Resources.Load<GameObject>("Money/BronzeCoin");
        Silver = Resources.Load<GameObject>("Money/SilverCoin");
        Gold = Resources.Load<GameObject>("Money/GoldCoin");
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
    public static void SpawnCoin(Vector2 pos, int value = 1)
    {
        int bronze = value % 5;
        int silver = value / 5 % 5;
        int gold = value / 25;
        for (; bronze > 0; --bronze)
            Spawn(Bronze, pos);
        for (; silver > 0; --silver)
            Spawn(Silver, pos);
        for (; gold > 0; --gold)
            Spawn(Gold, pos);
    }
    private static void Spawn(GameObject coinType, Vector2 pos)
    {
        GameObject obj = GameObject.Instantiate(coinType, pos, Quaternion.identity);
        obj.GetComponent<Rigidbody2D>().velocity = Utils.RandCircle(8);
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
