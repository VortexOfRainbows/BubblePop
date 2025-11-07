using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.UI;

public class GachaponShop : MonoBehaviour
{
    public static GachaponShop Instance;
    public GameObject[] Pedastal;
    public PowerUpObject[] Stock { get; private set; }
    public int TotalPowersPurchased;
    public float PriceMultiplier = 1f;
    public float FillStockTimer = 0;
    public int NextToFillUp = -1;
    public void FixedUpdate()
    {
        Instance = this;
        if (Main.WavesUnleashed)
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
        float mult = PriceMultiplier * (1.0f + 0.1f * TotalPowersPurchased - Player.Instance.ShopDiscount);
        PowerUpObject obj = PowerUp.Spawn(PowerUp.RandomFromPool(0.05f, .005f * Player.Instance.BlackmarketMult), Pedastal[i].transform.position + new Vector3(0, 1.5f), 0).GetComponent<PowerUpObject>();
        obj.Cost = Mathf.Max(0, (int)(obj.MyPower.Cost * mult));
        Stock[i] = obj;
    }
}
