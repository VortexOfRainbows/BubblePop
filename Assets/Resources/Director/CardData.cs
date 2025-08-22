using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardData
{
    /* 
     * CardData ought to contain
     * 1. Information on the enemy that it adds to the pool
     * 2. Information on other effects if the enemy is already added
     * 3. Information on the rewards the card provides
     */
    public float AvailablePoints;
    public float SpentPoints = 0;
    public EnemyClause Enemies;
    public ModifierClause Modifiers;
    public RewardClause Rewards;
    public void GetPointsAllowed()
    {
        AvailablePoints = 100; //This should be tied to the wave number in some way
    }
    public void Generate()
    {
        GetPointsAllowed();
        Generate<EnemyClause>(AvailablePoints);
        Generate<ModifierClause>(AvailablePoints);
        Generate<RewardClause>(SpentPoints);
    }
    public T Generate<T>(float points) where T : CardClause, new()
    {
        T c = CardClause.Generate<T>(points);
        if (c is EnemyClause e)
            Enemies = e;
        else if (c is ModifierClause m)
            Modifiers = m;
        if (c is RewardClause r)
        {
            Rewards = r;
            SpentPoints = c.Points;
        }
        else
        {
            SpentPoints += AvailablePoints - c.Points;
            AvailablePoints = c.Points;
        }
        return c;
    }
}
public abstract class CardClause
{
    /// <summary>
    /// Enemy cards cost points to spawn
    /// Modifier cards cost points to add
    /// Based on the amount of points spent, reward cards are added
    /// </summary>
    public float Points { get; set; }
    public static T Generate<T>(float AvailablePoints) where T : CardClause, new()
    {
        T clause = new();
        clause.Points = AvailablePoints;
        clause.GenerateData();
        return clause;
    }
    public override string ToString()
    {
        return "";
    }
    public abstract void GenerateData();
}
public class EnemyClause : CardClause
{
    public EnemyPoolAddition EnemyPoolChange;
    public ModifierClause AlternateModifier;
    public override void GenerateData()
    {
        AlternateModifier = Generate<ModifierClause>(Points);
    }
    //public override string ToString()
    //{
    //    return $"Adds {EnemyPoolChange.ToString()} to the enemy pool";
    //}
}
public class ModifierClause : CardClause
{
    public List<DirectorModifier> Modifiers = new();
    public override void GenerateData()
    {
        while(Points > 0)
        {
            --Points;
        }
    }
}
public class RewardClause : CardClause
{
    public List<Reward> Rewards = new();
    public override void GenerateData()
    {
        while (Points > 0)
        {
            --Points;
        }
    }
}