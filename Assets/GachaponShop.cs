using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class GachaponShop : MonoBehaviour
{
    public RestockMachine RestockMachine;
    public int RestockRemaining { get; set; } = 3;
    public static List<GachaponShop> AllShops { get; set; } = new();
    public GameObject[] Pedastal;
    public PowerUpObject[] Stock { get; private set; }
    public static int TotalPowersPurchased { get; set; } = 0;
    public static float PriceMultiplier { get; set; } = 1f;
    public bool BlackMarketShop = false;
    public float FillStockTimer = 0;
    public int NextToFillUp = -1;
    public byte ProgressionNumber { get; set; } = 0;
    public void Start()
    {
        AllShops.Add(this);
        ProgressionNumber = World.GetTileData(World.RealTileMap.Map.WorldToCell(transform.position)).ProgressionNumber;
        RestockMachine.SetRestockAmountToShopStock(this);
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
                TryRestocking();
            }
        }
    }
    public void Restock()
    {
        ++TotalPowersPurchased;
        AddStock(NextToFillUp);
        NextToFillUp = -1;
        --RestockRemaining;
        if (RestockRemaining <= 0) //This is just for testing the restock machine
            RestockRemaining = 10;
        RestockMachine.SetRestockAmountToShopStock(this);
    }
    public void TryRestocking()
    {
        FillStockTimer += Time.fixedDeltaTime;
        if (RestockRemaining > 0)
        {
            FillStockTimer = 0;
            Restock();
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
        float bmChance = BlackMarketShop ? 1 : .005f * Player.Instance.BlackmarketMult;
        Vector3 pillowPosition = Pedastal[i].transform.position + new Vector3(0, 1.2f);
        Vector3 spawnPosition = RestockMachine != null ? RestockMachine.transform.position + new Vector3(0, 0.05f) : pillowPosition;
        PowerUpObject obj = PowerUp.Spawn(PowerUp.RandomFromPool(0.05f, bmChance), spawnPosition).GetComponent<PowerUpObject>();
        obj.Cost = Mathf.Max(0, (int)(obj.MyPower.Cost * mult));
        obj.FinalPosition = pillowPosition;
        obj.VelocityStyle = 1;
        obj.velocity = new Vector2(0, 8);
        if (RestockMachine != null)
        {
            AudioManager.PlaySound(SoundID.ChestDrop, spawnPosition, 1, 1);
            float r = Utils.rand.NextFloat(Mathf.PI * 2);
            for (int a = 0; a < 30; ++a)
            {
                Vector2 circular = new Vector2(1, 0).RotatedBy(Mathf.PI * a / 15f + r);
                ParticleManager.NewParticle(spawnPosition, Utils.RandFloat(2, 3), circular * Utils.RandFloat(2.5f, 4), 0.2f, Utils.RandFloat(0.7f, 1.5f), ParticleManager.ID.Pixel,
                    obj.glow.color * 0.7f);
            }
        }
        Stock[i] = obj;
    }
}
