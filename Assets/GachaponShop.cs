using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.UI;

public class GachaponShop : MonoBehaviour
{
    public GameObject[] Pedastal;
    protected PowerUpObject[] Stock;
    public int TotalPowersPurchased;
    public float PriceMultiplier = 1f;
    public float FillStockTimer = 0;
    public int NextToFillUp = -1;
    public void FixedUpdate()
    {
        if(Main.WavesUnleashed)
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
        if (FillStockTimer > 2)
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
        float mult = PriceMultiplier * (1.0f + 0.1f * TotalPowersPurchased);
        PowerUpObject obj = PowerUp.Spawn(PowerUp.RandomFromPool(0.05f, .005f), Pedastal[i].transform.position + new Vector3(0, 1.5f), 0).GetComponent<PowerUpObject>();
        obj.Cost = (int)(obj.MyPower.Cost * mult);
        Stock[i] = obj;
    }
}
