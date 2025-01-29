using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
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
    public static PowerUp Get(int i)
    {
        if (PowerUps == null)
            InitDict();
        return PowerUps[i];
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
        if (Reverses.ContainsKey(p))
            return;
        p.MyID = typeCounter;
        PowerUps[typeCounter] = p;
        Reverses[p] = typeCounter;
        Debug.Log($"Added: {p.Name()} to the dictionary at index {typeCounter}");
        typeCounter++;
        maximumTypes++;
    }
    protected static Dictionary<int, PowerUp> PowerUps;
    protected static void InitDict()
    {
        PowerUps = new Dictionary<int, PowerUp>();
        ReflectiveEnumerator.AssembleInstances<PowerUp>();
    }
    public static Dictionary<PowerUp, int> Reverses = new Dictionary<PowerUp, int>();
    private static int typeCounter = 0;
    private static int maximumTypes = 0;
    public int Stack;
    public float Rarity;
    public int MyID = -1;
    protected PowerUp()
    {
        sprite = Resources.Load<Sprite>(GetType().Name);
        AddToDictionary(this);
        Init();
    }
    private void Reset()
    {
        Stack = 0;
        Rarity = 0;
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
