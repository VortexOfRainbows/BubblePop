using System.Collections.Generic;
using UnityEngine;

public class GachaponShop : MonoBehaviour
{
    public static List<GachaponShop> AllShops { get; set; } = new();
    public GameObject[] Pedastal;
    public PowerUpObject[] Stock { get; private set; }
    public static int TotalPowersPurchased { get; set; } = 0;
    public static float PriceMultiplier { get; set; } = 1f;
    public float FillStockTimer = 0;
    public int NextToFillUp = -1;
    public byte ProgressionNumber { get; set; } = 0;
    public void Start()
    {
        AllShops.Add(this);
        ProgressionNumber = World.GetTileData(World.RealTileMap.Map.WorldToCell(transform.position)).ProgressionNumber;
    }
    public void FixedUpdate()
    {
        if (Main.WavesUnleashed && ProgressionNumber <= Main.PylonProgressionNumber)
        {
            if(Stock == null)
            {
                Stock = new PowerUpObject[Pedastal.Length];
                for (int i = 0; i < Pedastal.Length; ++i)
                    AddStock(i);
            }
            if(NextToFillUp == -1)
            {
                FillStockTimer = 0;
                CheckMissingStock();
            }
            else
            {
                Restock();
            }
        }
    }
    public void Restock()
    {
        FillStockTimer += Time.fixedDeltaTime;
        if (FillStockTimer > 1.5f)
        {
            ++TotalPowersPurchased;
            FillStockTimer = 0; 
            AddStock(NextToFillUp);
            NextToFillUp = -1;
        }
    }
    public bool CheckMissingStock()
    {
        for(int i = 0; i < Pedastal.Length; ++i)
        {
            GameObject p = Pedastal[i];
            PowerUpObject s = Stock[i];
            if(s == null)
            {
                NextToFillUp = i;
                return true;
            }
        }
        return false;
    }
    public void AddStock(int i)
    {
        float mult = PriceMultiplier * (1.0f + 0.05f * TotalPowersPurchased - Player.Instance.ShopDiscount);
        PowerUpObject obj = PowerUp.Spawn(PowerUp.RandomFromPool(0.05f, .005f * Player.Instance.BlackmarketMult), Pedastal[i].transform.position + new Vector3(0, 1.5f)).GetComponent<PowerUpObject>();
        obj.Cost = Mathf.Max(0, (int)(obj.MyPower.Cost * mult));
        Stock[i] = obj;
    }
}
