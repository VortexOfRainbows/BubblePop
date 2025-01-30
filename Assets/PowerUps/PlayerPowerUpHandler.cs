using System.Collections.Generic;
using UnityEngine;

public partial class Player : Entity
{
    public int PowerCount => powers.Count;
    public int GetPower(int i) => powers[i];
    public void PickUpPower(int Type)
    {
        if (powers.Contains(Type))
            return;
        else
            powers.Add(Type);
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

