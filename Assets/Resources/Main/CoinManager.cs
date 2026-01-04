using System;
using Unity.VisualScripting.FullSerializer.Internal;
using UnityEngine;
public static class CoinManager
{
    public static void InitCoinPrefabs()
    {
        Bronze = Resources.Load<GameObject>("Money/BronzeCoin");
        Silver = Resources.Load<GameObject>("Money/SilverCoin");
        Gold = Resources.Load<GameObject>("Money/GoldCoin");
        Heart = Resources.Load<GameObject>("Money/HeartPickup");
        Key = Resources.Load<GameObject>("Money/KeyPickup");
        Chest = Resources.Load<GameObject>("Chests/Chest");
        Token = Resources.Load<GameObject>("Money/Token");
        Gem = Resources.Load<GameObject>("Money/GemPickup");
    }
    //public static void Load()
    //{
        //Savings = PlayerData.GetInt("Savings");
    //}
    //public static void Save()
    //{
        //PlayerData.SaveInt("Savings", Savings);
    //}
    public static GameObject Bronze;
    public static GameObject Silver;
    public static GameObject Gold;
    public static GameObject Heart;
    public static GameObject Key;
    public static GameObject Chest, Token, Gem;
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
    public static void SpawnHeart(Func<Vector2> func, float collectDelay) => SpawnHeart(func.Invoke(), collectDelay);
    public static void SpawnHeart(Vector2 pos, float collectDelay)
    {
        GameObject obj = GameObject.Instantiate(Heart, pos, Quaternion.identity);
        obj.GetComponent<Rigidbody2D>().velocity = Utils.RandCircle(4);
        obj.GetComponent<Coin>().BeforeCollectableTimer = collectDelay;
    }
    public static void SpawnKey(Func<Vector2> func, float collectDelay) => SpawnKey(func.Invoke(), collectDelay);
    public static void SpawnKey(Vector2 pos, float collectDelay)
    {
        GameObject obj = GameObject.Instantiate(Key, pos, Quaternion.identity);
        obj.GetComponent<Rigidbody2D>().velocity = Utils.RandCircle(4);
        obj.GetComponent<Coin>().BeforeCollectableTimer = collectDelay;
    }
    public static void SpawnGem(Func<Vector2> func, float collectDelay) => SpawnGem(func.Invoke(), collectDelay);
    public static void SpawnGem(Vector2 pos, float collectDelay)
    {
        GameObject obj = GameObject.Instantiate(Gem, pos, Quaternion.identity);
        obj.GetComponent<Rigidbody2D>().velocity = Utils.RandCircle(4);
        obj.GetComponent<Coin>().BeforeCollectableTimer = collectDelay;
    }
    public static void SpawnToken(Func<Vector2> func, float collectDelay) => SpawnToken(func.Invoke(), collectDelay);
    public static void SpawnToken(Vector2 pos, float collectDelay)
    {
        GameObject obj = GameObject.Instantiate(Token, pos, Quaternion.identity);
        obj.GetComponent<Rigidbody2D>().velocity = Utils.RandCircle(4);
        var c = obj.GetComponent<Coin>();
        c.BeforeCollectableTimer = collectDelay;
    }
    public static Chest SpawnChest(Func<Vector2> func, int type) => SpawnChest(func.Invoke(), type);
    public static Chest SpawnChest(Vector2 pos, int type)
    {
        Chest obj = GameObject.Instantiate(Chest, pos, Quaternion.identity).GetComponent<Chest>();
        obj.Init(type);
        return obj;
    }
    public static int CurrentCoins { get; private set; } = 0;
    public static int CurrentKeys { get; private set; } = 0;
    public static int CurrentTokens { get; private set; } = 0;
    public static int CurrentGems { get; private set; } = 0;
    public static int TotalEquipCost;
    public static void AfterDeathReset()
    {
        CurrentTokens = CurrentKeys = CurrentCoins = CurrentGems = 0;
    }
    public static void ModifyCoins(int amt)
    {
        CurrentCoins += amt;
        if (amt < 0)
        {
            Player.GoldSpentTotal -= amt;
            if(Player.GoldSpentTotal >= 6480)
            {
                UnlockCondition.Get<GachaponUnlock>().SetComplete();
            }
        }
    }
    public static void ModifyKeys(int amt)
    {
        CurrentKeys += amt;
    }
    public static void ModifyTokens(int amt)
    {
        int prevTokens = CurrentTokens;
        CurrentTokens += amt;
        if(CurrentTokens > Player.Instance.MaxTokens)
            CurrentTokens = Mathf.Max(prevTokens, Player.Instance.MaxTokens);
    }
    public static void ModifyGems(int amt)
    {
        CurrentGems += amt;
    }
}
