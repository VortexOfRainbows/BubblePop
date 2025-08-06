using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.VisualScripting;
using UnityEditor;
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
public class DetailedDescription
{
    public static string BastardizeText(string s, char bChar)
    {
        string ret = string.Empty;
        bool open = false;
        for(int i = 0; i < s.Length; ++i)
        {
            Char c = s[i];
            if(!open)
            {
                if (c == '<')
                {
                    open = true;
                }
                else if(char.IsLetterOrDigit(c))
                {
                    c = bChar;
                }
            }
            else if (c == '>')
                open = false;
            ret += c;
        }
        return ret;
    }
    public PowerUp owner;
    private static readonly string[] Rares = new string[] { "#CFCFFF", "#C2FFAA", "#AAD3FE", "#D4AAFE", "#FCB934" };
    private static readonly string Yellow = "#FFED75";
    private static readonly string Gray = "#999999";
    private static readonly string TabForMoreDetail = " <size=24><color=#CB8A8A>(TAB for more detail)</color></size>";
    private static readonly int NormalTextSize = 28;
    private static readonly int GrayTextSize = 24;
    public string ToRichText(string t)
    {
        string[] segments = t.Split(' ');
        string concat = string.Empty;
        int secondLast = segments.Length - 1;
        bool openBracket = false;
        for (int i = 0; i < segments.Length; ++i)
        {
            string segment = segments[i];
            concat += SegmentToRichRext(segment, ref openBracket);
            if (i != secondLast)
                concat += ' ';
        }
        return concat;
    }
    public string SegmentToRichRext(string t, ref bool waitingForEnding)
    {
        if(t.Length > 2)
        {
            string first2 = t[..2];
            char third = t[2];
            char last = t[^1];
            bool isOpening = third == '[' || third == '(';
            bool isEnding = last == ']' || last == ')';
            string start = string.Empty;
            string end = string.Empty;
            string contents = t;
            if (first2 == "G:")
                start = $"<size={GrayTextSize}><color={Gray}>";
            else if (first2 == "Y:")
                start = $"<size={NormalTextSize}><color={Yellow}>";
            if (isEnding && waitingForEnding && !isOpening) //If this string ends with a ']' and we have previously seen a '['
            {
                waitingForEnding = false;
                if (last != ')')
                    contents = t[..^1];
                end = "</color></size>";
            }
            else if (isOpening && !waitingForEnding && !isEnding) //If this string starts with a '[' and we have not seen a ']'
            {
                waitingForEnding = true;
                if (third != '(')
                    contents = t[3..];
                else
                    contents = t[2..];
            }
            else if (start != string.Empty) //there are no brackets, so we start and end at once
            {
                if(isOpening && isEnding && third != '(')
                {
                    contents = t[3..^1];
                }
                else
                    contents = t[2..];
                end = "</color></size>";
            }
            return $"{start}{contents}{end}";
        }
        return t;
    }
    public DetailedDescription(PowerUp p) 
    { 
        owner = p;
        Name = p.GetType().FullName.ToSpacedString();
        Description = "N/A";
    }
    public void WithDescription(string lines)
    {
        Description = lines;
    }
    public void WithShortDescription(string lines)
    {
        ShortDescription = lines;
    }
    public void WithDescriptionVariant<T>(string lines) where T: Body
    {
        AltDescriptions.Add(typeof(T), lines);
    }
    public void WithName(string name)
    {
        Name = name;
    }
    private readonly Dictionary<Type, string> AltDescriptions = new();
    private readonly Dictionary<Type, string> CompleteAltDescriptions = new();
    private string Name;
    private string Description;
    private string ShortDescription = null;
    private string CompleteDescription = string.Empty;
    private string CompleteShortDescription = string.Empty;
    public string FullDescription()
    {
        if(CompleteDescription == string.Empty)
            CompleteDescription = ToRichText(Description);
        if(Player.Instance != null)
        {
            Type playerBodyType = Player.Instance.Body.GetType();
            if (AltDescriptions.TryGetValue(playerBodyType, out string lines))
            {
                if (!CompleteAltDescriptions.TryGetValue(playerBodyType, out string lines2))
                {
                    CompleteAltDescriptions[playerBodyType] = ToRichText(lines);
                }
                else
                    return lines2;
            }
        }
        return CompleteDescription;
    }
    public string BriefDescription(bool withDetails = false)
    {
        if(ShortDescription == null)
            return FullDescription();
        if (CompleteShortDescription == string.Empty)
            CompleteShortDescription = ToRichText(ShortDescription);
        return CompleteShortDescription + (withDetails ? TabForMoreDetail : string.Empty);
    }
    public static string TextBoundedByRarityColor(int rare, string text)
    {
        return $"<color={Rares[rare]}>{text}</color>";
    }
    public string TextBoundedByRarityColor(string text)
    {
        return TextBoundedByRarityColor(owner.GetRarity() - 1, text);
    }
    public string GetName()
    {
        return TextBoundedByRarityColor(Name);
    }
}

public abstract class PowerUp
{
    public static readonly float Common = 1f;
    public static readonly float Uncommon = 0.7f;
    public static readonly float Rare = 0.27f;
    public static readonly float SuperRare = 0.1f;
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
    public int PickedUpCountAllRuns => AmountPickedUpAcrossAllRuns;
    public int PickedUpBestAllRuns => HighestAmountPickedUpInASingleRun;
    protected int AmountPickedUpAcrossAllRuns = 0;
    protected int HighestAmountPickedUpInASingleRun = 0;
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
    public static void ResetPowerAvailability()
    {
        AvailablePowers.Clear();
    }
    public static void AddPowerUpToAvailability(PowerUp power)
    {
        for(int i = 0; i < AvailablePowers.Count; ++i)
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
    #region Powerup Datastructure Related Stuff
    private static int typeCounter = 0;
    private static int maximumTypes = 0;
    public static GameObject Spawn<T>(Vector2 pos, int pointCost = 100) where T : PowerUp => Spawn(typeof(T).Name, pos, pointCost);
    public static GameObject Spawn(string powerTypeName, Vector2 pos, int pointCost = 100)
    {
        if (Reverses == null)
            InitDict();
        return Spawn(Reverses[powerTypeName], pos, pointCost);
    }
    public static GameObject Spawn(int powerUpID, Vector2 pos, int pointCost = 100)
    {
        PowerUpObject obj = GameObject.Instantiate(PowerDefinitions.PowerUpObj, pos, Quaternion.identity);
        obj.Type = powerUpID;
        WaveDirector.PointsSpent += pointCost;
        WaveDirector.PityPowersSpawned += pointCost / 100f;
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
        Debug.Log($"Added: {p.TrueDescription.GetName()} to the dictionary at index {typeCounter}");
        typeCounter++;
        maximumTypes++;
    }
    protected static Dictionary<int, PowerUp> PowerUps;
    public static Dictionary<string, int> Reverses;
    protected static void InitDict()
    {
        Reverses = new();
        PowerUps = new Dictionary<int, PowerUp>();
        ReflectiveEnumerator.AssembleInstances<PowerUp>();
    }
    public int MyID = -1;
    #endregion
    public static bool PickingPowerUps = false;
    public static int RandomFromPool(float bonusChoiceChance = 0.15f)
    {
        return PickRandomPower(0, bonusChoiceChance);
    }
    private static int PickRandomPower(int recursionDepth = 0, float addedChoiceChance = 0.15f)
    {
        if (Utils.RandFloat() < addedChoiceChance)
        {
            return Get<Choice>().MyID;
        }
        float weightMult = 1.0f + recursionDepth * 0.1f;
        int type = AvailablePowers[Utils.RandInt(AvailablePowers.Count)];
        if (Player.Instance.RollPerc > 0)
        {
            int rare = PowerUps[type].GetRarity();
            if (rare >= 3) //Increase odds of seeing blue, purple, yellow
                weightMult += 0.2f * Player.Instance.RollPerc;
            else //Slightly decrease odds of seeing white, green
            {
                float decreaseMult = rare == 1 ? 0.02f : 0.01f;
                weightMult -= decreaseMult * Player.Instance.RollPerc;
                if(weightMult < 0.2f)
                {
                    weightMult = 0.2f;
                }
            }
        }
        float powerupWeighting = PowerUps[type].Weighting * weightMult;
        if (powerupWeighting > Utils.RandFloat(1))
        {
            return type;
        }
        else
            return PickRandomPower(recursionDepth + 1, addedChoiceChance + 0.02f);
    }
    public static void TurnOnPowerUpSelectors()
    {
        int max = Player.Instance.BonusChoices ? 5 : 3;
        for (int i = 0; i < max; i++)
        {
            if (!PowerUpButton.buttons[i].Active)
            {
                PowerUpButton.buttons[i].TurnOn();
            }
        }
        if(PlayerData.PauseDuringPowerSelect)
            Time.timeScale = 0;
    }
    public static void TurnOffPowerUpSelectors()
    {
        int max = Player.Instance.BonusChoices ? 5 : 3;
        for (int i = 0; i < max; i++)
        {
            if (PowerUpButton.buttons[i].Active)
            {
                PowerUpButton.buttons[i].TurnOff();
            }
        }
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
    }
    public virtual Sprite GetTexture()
    {
        return Resources.Load<Sprite>($"PowerUps/{InternalName}");
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
    public virtual void InitializeDescription(ref DetailedDescription description)
    {

    }
    public static readonly string LockedName = "???";
    public static readonly string LockedDescription = "Powerup not yet discovered";
    public string UnlockedName => TrueDescription.GetName();
    public string ShortDescription => TrueDescription.BriefDescription();
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
        if (rare == 5)
            return 100;
        if (rare == 4)
            return 50;
        if (rare == 3)
            return 25;
        if (rare == 2)
            return 15;
        if (rare == 1)
            return 10;
        return (int)(10 / Weighting);
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
    public virtual int GetRarity()
    {
        return CalculateRarity();
    }
    public virtual Material GetBorder(bool thin = false)
    {
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
}
