using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public static class ReflectiveEnumerator
{
    static ReflectiveEnumerator() { }
    public static void AssembleInstances<T>()
    {
        foreach (Type type in
             Assembly.GetAssembly(typeof(T)).GetTypes()
             .Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(T))))
        {
           object v = ((T)Activator.CreateInstance(type));
        }
    }
}
public abstract class PowerUp
{
    public static readonly float Common = 1f;
    public static readonly float Uncommon = 0.7f;
    public static readonly float Rare = 0.27f;
    public static readonly float SuperRare = 0.1f;
    public static float Epic => SuperRare;
    public static readonly float Legendary = 0.05f;
    public static readonly Material WhiteOutline = Resources.Load<Material>("Materials/OutlineShader/OutlineShaderWhite");
    public static readonly Material GreenOutline = Resources.Load<Material>("Materials/OutlineShader/OutlineShaderGreen");
    public static readonly Material BlueOutline = Resources.Load<Material>("Materials/OutlineShader/OutlineShaderBlue");
    public static readonly Material PurpleOutline = Resources.Load<Material>("Materials/OutlineShader/OutlineShaderPurple");
    public static readonly Material GoldOutline = Resources.Load<Material>("Materials/OutlineShader/OutlineShaderGold");
    public static readonly Material WhiteOutlineThin = Resources.Load<Material>("Materials/OutlineShader/OutlineShaderWhiteThin");
    public static readonly Material GreenOutlineThin = Resources.Load<Material>("Materials/OutlineShader/OutlineShaderGreenThin");
    public static readonly Material BlueOutlineThin = Resources.Load<Material>("Materials/OutlineShader/OutlineShaderBlueThin");
    public static readonly Material PurpleOutlineThin = Resources.Load<Material>("Materials/OutlineShader/OutlineShaderPurpleThin");
    public static readonly Material GoldOutlineThin = Resources.Load<Material>("Materials/OutlineShader/OutlineShaderGoldThin");
    public static readonly Material RedOutline = Resources.Load<Material>("Materials/OutlineShader/OutlineShaderRed");
    public static readonly Material RedOutlineThin = Resources.Load<Material>("Materials/OutlineShader/OutlineShaderRedThin");
    public int PickedUpCountAllRuns => AmountPickedUpAcrossAllRuns;
    public int PickedUpBestAllRuns => HighestAmountPickedUpInASingleRun;
    protected int AmountPickedUpAcrossAllRuns = 0;
    protected int HighestAmountPickedUpInASingleRun = 0;
    public bool IsInPowerPool { get; private set; }
    public static void SaveAllData()
    {
        if (PowerUps == null)
            InitDict();
        for (int i = 0; i < maximumTypes; ++i)
            PowerUps[i].SaveData();
    }
    public static void LoadAllData()
    {
        if (PowerUps == null)
            InitDict();
        for (int i = 0; i < maximumTypes; ++i)
            PowerUps[i].LoadData();
    }
    public void SaveData()
    {
        PlayerData.SaveInt(InternalName + "Total", AmountPickedUpAcrossAllRuns);
        PlayerData.SaveInt(InternalName + "Best", HighestAmountPickedUpInASingleRun);
    }
    public void LoadData()
    {
        AmountPickedUpAcrossAllRuns = PlayerData.GetInt(InternalName + "Total");
        HighestAmountPickedUpInASingleRun = PlayerData.GetInt(InternalName + "Best");
    }
    public static List<int> AvailablePowers = new();
    public static List<int> AvailableBlackMarketPowers = new();
    public static void ResetPowerAvailability()
    {
        for (int i = 0; i < maximumTypes; ++i)
            Get(i).IsInPowerPool = false;
        AvailablePowers.Clear();
        AvailableBlackMarketPowers.Clear();
        AddUniversalPowerups();
    }
    public static void AddUniversalPowerups()
    {
        //Statistical powers
        AddPowerUpToAvailability<Overclock>();
        AddPowerUpToAvailability<WeaponUpgrade>();
        AddPowerUpToAvailability<FocusFizz>();
        AddPowerUpToAvailability<CloudWalker>();
        AddPowerUpToAvailability<BubbleBirb>();

        //Economy powers
        AddPowerUpToAvailability<Magnet>();
        AddPowerUpToAvailability<Coupons>();

        //Powers that give powers
        AddPowerUpToAvailability<Choice>();
        AddPowerUpToAvailability<PiratesBooty>();
        AddPowerUpToAvailability<BubbleMitosis>();
        AddPowerUpToAvailability<BlackMarketDelivery>();
        AddPowerUpToAvailability<ShardsOfPower>();
    }
    private static void AddPowerUpToAvailability<T>() where T: PowerUp => AddPowerUpToAvailability(Get<T>());
    public static void AddPowerUpToAvailability(PowerUp power)
    {
        power.IsInPowerPool = true;
        for (int i = 0; i < AvailablePowers.Count; ++i)
        {
            PowerUp currentPower = Get(AvailablePowers[i]);
            if (power.Weighting > currentPower.Weighting)
            {
                AvailablePowers.Insert(i, power.MyID);
                return;
            }
            else if(power.Weighting == currentPower.Weighting)
            {
                if(power.MyID < currentPower.MyID)
                {
                    AvailablePowers.Insert(i, power.MyID);
                    return;
                }
            }
        }
        //Debug.Log($"Insert {power.Name()} at end");
        AvailablePowers.Add(power.MyID);
    }
    public static void AddBlackMarketPowerToPool(PowerUp power)
    {
        AvailableBlackMarketPowers.Add(power.MyID);
    }
    #region Powerup Datastructure Related Stuff
    private static int typeCounter = 0;
    private static int maximumTypes = 0;
    public static GameObject Spawn<T>(Vector2 pos) where T : PowerUp => Spawn(typeof(T).Name, pos);
    public static GameObject Spawn(string powerTypeName, Vector2 pos)
    {
        if (Reverses == null)
            InitDict();
        return Spawn(Reverses[powerTypeName], pos);
    }
    public static GameObject Spawn(int powerUpID, Vector2 pos)
    {
        PowerUpObject obj = GameObject.Instantiate(Main.PrefabAssets.PowerUpObj, pos, Quaternion.identity);
        obj.Type = powerUpID;
        obj.finalPosition = pos;
        WaveDirector.TotalPowersSpawned += 1;
        return obj.gameObject;
    }
    public static PowerUp Get(string powerTypeName)
    {
        if (Reverses == null)
            InitDict();
        return Get(Reverses[powerTypeName]);
    }
    public static PowerUp Get<T>() where T : PowerUp => Get(typeof(T).Name);
    public static PowerUp Get(int powerUpID)
    {
        if (PowerUps == null)
            InitDict();
        return PowerUps[powerUpID];
    }
    public static void ResetAll()
    {
        for(int i = 0; i < maximumTypes; i++)
        {
            PowerUps[i].Reset();
        }
    }
    private static void AddToDictionary(PowerUp p)
    {
        if (Reverses.ContainsKey(p.InternalName))
            return;
        p.MyID = typeCounter;
        PowerUps[typeCounter] = p;
        Reverses[p.InternalName] = typeCounter;
        Debug.Log($"Added: {p.UnlockedName} to the dictionary at index {typeCounter}");
        typeCounter++;
        maximumTypes++;
    }
    public static Dictionary<int, PowerUp> PowerUps { get; private set; }
    public static Dictionary<string, int> Reverses;
    protected static void InitDict()
    {
        Reverses = new();
        PowerUps = new Dictionary<int, PowerUp>();
        ReflectiveEnumerator.AssembleInstances<PowerUp>();
    }
    public int MyID = -1;
    #endregion
    public bool ForceBlackMarket { get; set; } = false;
    public static bool PickingPowerUps { get; set; }
    public static int RandomFromPool(float bonusChoiceChance = 0.15f, float blackMarketChance = -1f, int rarity = -1)
    {
        return PickRandomPower(0, bonusChoiceChance, Utils.RandFloat(1) < blackMarketChance, rarity);
    }
    private static int PickRandomPower(int recursionDepth = 0, float addedChoiceChance = 0.15f, bool BlackMarket = false, int rarity = -1)
    {
        float flowerChance = Player.Instance.RainbowFlowers * 0.05f;
        if(Utils.RollWithLuck(flowerChance))
        {
            return Get<RainbowFlower>().MyID;
        }
        if (Utils.RandFloat() < addedChoiceChance && (rarity == -1 || rarity == 1))
        {
            return BlackMarket ? Get<Contract>().MyID : Get<Choice>().MyID;
        }
        if (rarity == 1 || rarity == 0)
            rarity = -1;
        List<int> avail = BlackMarket ? AvailableBlackMarketPowers : AvailablePowers;
        float highestWeight = 1.0f;
        float highestSeen = 0.0f;
        if(rarity != -1)
        {
            List<int> temp = new();
            foreach(int i in avail)
            {
                if (PowerUps[i].GetRarity() >= rarity)
                {
                    highestSeen = PowerUps[i].Weighting > highestSeen ? PowerUps[i].Weighting : highestSeen;
                    temp.Add(i);
                }
            }    
            if (temp.Count > 0)
                avail = temp;
        }
        if (highestSeen != 0)
            highestWeight = highestSeen;
        float weightMult = 1.0f / highestWeight + recursionDepth * 0.1f;
        int type = avail[Utils.RandInt(avail.Count)];
        if (Player.Instance.RollPerc > 0)
        {
            int rare = PowerUps[type].GetRarity();
            if (rare >= 3) //Increase odds of seeing blue, purple, yellow
                weightMult += 0.2f * Player.Instance.RollPerc;
            else //Slightly decrease odds of seeing white, green
            {
                float decreaseMult = rare == 1 ? 0.02f : 0.01f;
                weightMult -= decreaseMult * Player.Instance.RollPerc;
                if(weightMult < 0.5f)
                {
                    weightMult = 0.5f;
                }
            }
        }
        float powerupWeighting = PowerUps[type].Weighting * weightMult;
        if ((rarity != -1 || powerupWeighting > Utils.RandFloat(1)) && 
            (addedChoiceChance != -1 || (type != Get<Choice>().Type && type != Get<Contract>().Type)))
        {
            return type;
        }
        else
        {
            return PickRandomPower(recursionDepth + 1, addedChoiceChance < 0 ? -1 : addedChoiceChance + 0.02f, BlackMarket, rarity);
        }
    }
    public static void TurnOnPowerUpSelectors()
    {
        if (Player.Instance.ResearchNoteBonuses > 0)
        {
            Player.Instance.BonusChoices = true;
            --Player.Instance.ResearchNoteBonuses;
        }
        bool BlackMarket = false;
        if(Player.Instance.ChoiceContract > 0)
        {
            BlackMarket = true;
            --Player.Instance.ChoiceContract;
        }
        ChoicePowerMenu.TurnOn(Player.Instance.BonusChoices, BlackMarket);
        if (PlayerData.PauseDuringPowerSelect)
            Time.timeScale = 0;
    }
    public static void TurnOffPowerUpSelectors()
    {
        ChoicePowerMenu.TurnOff();
        Time.timeScale = 1;
    }
    public int Stack;
    public float Weighting = 1;
    //Returns the MyID of this power
    public int Type => MyID;
    private string InternalName;
    protected PowerUp()
    {
        TrueDescription = new(this);
        InitializeDescription(ref TrueDescription);
        InternalName = GetType().Name;
        sprite = GetTexture();
        AddToDictionary(this);
        Init();
        TrueDescription.Rarity = GetRarity() - 1;
    }
    public virtual Sprite GetTexture()
    {
        var s = Resources.Load<Sprite>($"PowerUps/{InternalName}");
        return s != null ? s : Main.TextureAssets.PowerUpPlaceholder;
    }
    public virtual Sprite GetAdornment()
    {
        return null;
    }
    private void Reset()
    {
        Stack = 0;
        Weighting = 1;
        Init();
    }
    public void PickUp(int amt = 1)
    {
        AddToDisplayQueue();
        AddToPlayer(amt);
        OnPickup(amt);
        Player.Instance.MostRecentPower = this;
        AmountPickedUpAcrossAllRuns += amt;
        if (Stack > HighestAmountPickedUpInASingleRun)
            HighestAmountPickedUpInASingleRun = Stack;
        if(Player.Instance.PowerCount >= 20 && Player.Instance.Body is ThoughtBubble)
            UnlockCondition.Get<ThoughtBubbleArsenal>().SetComplete();
    }
    public void AddToDisplayQueue()
    {
        if (!PopUpTextUI.PowerupQueue.Contains(this))
            PopUpTextUI.PowerupQueue.Enqueue(this);
    }
    private int AddToPlayer(int count = 1)
    {
        Player.Instance.PickUpPower(MyID);
        Stack = Stack + count;
        return Stack;
    }
    public virtual void Init()
    {

    }
    public virtual void OnPickup(int count)
    {

    }
    private DetailedDescription TrueDescription;
    public DetailedDescription DetailedDescription => TrueDescription;
    public virtual void InitializeDescription(ref DetailedDescription description)
    {

    }
    public static readonly string LockedName = "???";
    public static readonly string LockedDescription = "Powerup not yet discovered";
    public string UnlockedName => TrueDescription.GetName(false, IsBlackMarket());
    public string ShortDescription => TrueDescription.BriefDescription();
    public bool HasBriefDescription => TrueDescription.HasBriefDescription;
    public string TrueFullDescription => TrueDescription.FullDescription();
    public string FullDescription => (Control.Tab || !PlayerData.BriefDescriptionsByDefault) ? TrueFullDescription : TrueDescription.BriefDescription(true);
    public virtual void HeldEffect(Player p)
    {

    }
    public virtual void AliveUpdate(GameObject inner, GameObject outer, bool UI = false)
    {
        inner.transform.localPosition = Vector3.zero;
        inner.transform.eulerAngles = Vector3.zero;
        inner.transform.localScale = Vector3.one;
    }
    public Sprite sprite;
    public virtual int Cost => GetDefaultCost();
    public int GetDefaultCost()
    {
        int rare = GetRarity();
        int cost = 15;
        if (rare == 5)
            cost = 250;
        else if (rare == 4)
            cost = 100;
        else if (rare == 3)
            cost = 50;
        else if (rare == 2)
            cost = 25;
        if (IsBlackMarket())
            cost *= 3;
        return cost;
    }
    public int CalculateRarity()
    {
        if(Weighting <= 0.05f)
            return 5;
        if (Weighting <= 0.1f)
            return 4;
        if (Weighting <= 0.4f)
            return 3;
        if (Weighting <= 0.8f)
            return 2;
        return 1;
    }
    public virtual int CrucibleGems(bool dissolve = false)
    {
        int rare = GetRarity();
        int gems = dissolve ? 3 : 5;
        if (rare == 5)
            gems = dissolve ? 20 : 25;
        else if (rare == 4)
            gems = dissolve ? 15 : 20;
        else if (rare == 3)
            gems = dissolve ? 10 : 15;
        else if (rare == 2)
            gems = dissolve ? 5 : 10;
        //if (IsBlackMarket())
        //    gems *= 2;
        return gems;
    }
    public virtual int ShardReplicationCost(int stackSize = 1)
    {
        int shards = 1;
        int rare = GetRarity();
        if (rare == 5)
            shards = 3;
        else if (rare == 3 || rare == 4)
            shards = 2;
        if (IsBlackMarket())
            shards *= 2;
        return shards * stackSize;
    }
    public virtual int GetRarity()
    {
        return CalculateRarity();
    }
    public virtual Material GetBorder(bool thin = false)
    {
        if (IsBlackMarket())
            return thin ? RedOutlineThin : RedOutline;
        int rare = GetRarity();
        if (rare == 5)
            return thin ? GoldOutlineThin : GoldOutline;
        if (rare == 4)
            return thin ? PurpleOutlineThin : PurpleOutline;
        if (rare == 3)
            return thin ? BlueOutlineThin : BlueOutline;
        if (rare == 2)
            return thin ? GreenOutlineThin : GreenOutline;
        return thin ? WhiteOutlineThin : WhiteOutline;
    }
    public virtual bool IsBlackMarket()
    {
        if (ForceBlackMarket)
            return true;
        bool NoAltsForCompendium = Compendium.Instance != null && Compendium.Instance.PageNumber == 0;
        if (Main.GameFinishedLoading && !IsInPowerPool && HasBlackMarketAlternate && BlackMarketVariantUnlockCondition.Unlocked && !NoAltsForCompendium)
            return true;
        if (Compendium.Instance != null && Compendium.Instance.PageNumber == 3)
            return true; //All powers on the achievement page are going to show up as black market powers, so this should make sense as an extra fail-safe
        return false;
    }
    public bool CountsAsBlackMarketForCompendium()
    {
        return IsBlackMarket() || (HasBlackMarketAlternate
                            && PickedUpCountAllRuns > 0
                            && BlackMarketVariantUnlockCondition.Unlocked);
    }
    public bool HasBlackMarketAlternate => BlackMarketVariantUnlockCondition != null;
    public virtual UnlockCondition BlackMarketVariantUnlockCondition => null;
}
