using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using Unity.VisualScripting;
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
    #region Powerup Datastructure Related Stuff
    private static int typeCounter = 0;
    private static int maximumTypes = 0;
    public static GameObject Spawn<T>(Vector2 pos, int pointCost = 100) where T : PowerUp => Spawn(typeof(T).Name, pos, pointCost);
    public static GameObject Spawn(string powerTypeName, Vector2 pos, int pointCost = 100) => Spawn(Reverses[powerTypeName], pos, pointCost);
    public static GameObject Spawn(int powerUpID, Vector2 pos, int pointCost = 100)
    {
        PowerUpObject obj = GameObject.Instantiate(PowerDefinitions.PowerUpObj, pos, Quaternion.identity);
        obj.Type = powerUpID;
        EventManager.PointsSpent += 100;
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
        string typeName = p.GetType().Name;
        if (Reverses.ContainsKey(typeName))
            return;
        p.MyID = typeCounter;
        PowerUps[typeCounter] = p;
        Reverses[typeName] = typeCounter;
        Debug.Log($"Added: {p.Name()} to the dictionary at index {typeCounter}");
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
    public static int RandomFromPool(float bonusChoiceChance = 0.2f)
    {
        return PickRandomPower(0, bonusChoiceChance);
    }
    private static int PickRandomPower(int recursionDepth = 0, float addedChoiceChance = 0.2f)
    {
        if (Utils.RandFloat() < addedChoiceChance)
        {
            return Get<Choice>().MyID;
        }
        float recursionModifier = 1.0f + recursionDepth * 0.5f;
        int type = UnityEngine.Random.Range(0, 6);
        if (PowerUps[type].Weighting * recursionModifier > Utils.RandFloat(1))
        {
            return type;
        }
        else
            return PickRandomPower(recursionDepth + 1, addedChoiceChance + 0.02f);
    }
    public static void TurnOnPowerUpSelectors()
    {
        for (int i = 0; i < 3; i++)
        {
            if (!PowerUpButton.buttons[i].Active)
            {
                PowerUpButton.buttons[i].TurnOn();
            }
        }
    }
    public static void TurnOffPowerUpSelectors()
    {
        for (int i = 0; i < 3; i++)
        {
            if (PowerUpButton.buttons[i].Active)
            {
                PowerUpButton.buttons[i].TurnOff();
            }
        }
    }
    public int Stack;
    public float Weighting = 1;
    //Returns the MyID of this power
    public int Type => MyID;
    protected PowerUp()
    {
        sprite = Resources.Load<Sprite>(GetType().Name);
        AddToDictionary(this);
        Init();
    }
    private void Reset()
    {
        Stack = 0;
        Weighting = 1;
        Init();
    }
    public void PickUp()
    {
        AddToPlayer(1);
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
    public virtual string Name()
    {
        return "Unnamed Power Up";
    }
    public virtual string Description()
    {
        return "Power Up";
    }
    public virtual void HeldEffect(Player p)
    {

    }
    public Sprite sprite;
}
