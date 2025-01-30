using System.Collections.Generic;
using UnityEngine;

public partial class Player : Entity
{
    public int PowerCount => powers.Count;
    /// <summary>
    /// Returns the PowerUpID of the power at the given index
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public int GetPower(int index) => powers[index];
    public void PickUpPower(int Type)
    {
        if (powers.Contains(Type))
            return;
        else
            powers.Add(Type);
    }
    public void RemovePower(int Type, int num = 1)
    {
        int index= powers.IndexOf(Type);
        if (index != -1)
        {
            PowerUp p = PowerUp.Get(GetPower(index));
            p.Stack -= num;
            if(p.Stack <= 0)
                powers.RemoveAt(index);
        }
    }
    public int DamagePower = 0;
    public int ShotgunPower = 0;
    private List<int> powers;
    private void PowerInit()
    {
        powers = new List<int>();
        PowerUp.ResetAll();
    }
    private void ResetPowers()
    {
        powers.Clear();
    }
    private void ClearPowerBonuses()
    {
        DamagePower = ShotgunPower = 0;
    }
    private void UpdatePowerUps()
    {
        ClearPowerBonuses();
        for(int i = 0; i < powers.Count; i++)
        {
            PowerUp power = PowerUp.Get(powers[i]);
            if(power.Stack > 0)
            {
                power.HeldEffect(this);
                //Debug.Log($"Doing held effect for {power.Stack}");
            }
        }
    }
}

