using System.Collections.Generic;
using System.Security;
using UnityEngine;
using UnityEngine.EventSystems;

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
    public int ChargeShotDamage = 0;
    public int ShotgunPower = 0;
    public int DashSparkle = 0;
    public int FasterBulletSpeed = 0;
    public int Starbarbs = 0;
    public int SoapySoap = 0;
    public int BubbleBlast = 0;
    public int Starshot = 0;
    public float AttackSpeedModifier = 1.0f;
    public int BinaryStars = 0;
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
        ChargeShotDamage = ShotgunPower = DashSparkle = FasterBulletSpeed = Starbarbs = SoapySoap = BubbleBlast = Starshot = BinaryStars = 0;
        AttackSpeedModifier = 1.0f;
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
        UpdateFixed();
    }
    private float BinaryStarTimer = 0.0f;
    private void UpdateFixed()
    {
        if(BinaryStars > 0)
        {
            BinaryStarTimer -= Time.fixedDeltaTime;
            while(BinaryStarTimer <= 0)
            {
                BinaryStarTimer = 2f / Mathf.Pow(BinaryStars, 0.75f); //1.0, 1.25, 1.5, 1.75, 2.0
                Vector2 circular = Utils.RandCircle(1).normalized;
                float speedMax = 18 + FasterBulletSpeed;
                for (int i = 0; i < 2; i++)
                {
                    circular = circular.RotatedBy(Mathf.PI * i);
                    Vector2 target = (Vector2)transform.position + circular * (16 + FasterBulletSpeed);
                    Projectile.NewProjectile(transform.position, circular.RotatedBy(Mathf.PI * 0.9f) * speedMax, 4, target.x, target.y);
                }
            }
        }
        else
        {
            BinaryStarTimer = 0;
        }
    }
    public void OnDash(Vector2 velo)
    {
        if (DashSparkle > 0)
        {
            Vector2 norm = velo.normalized;
            int stars = 1 + DashSparkle;
            for (; stars > 0; --stars)
            {
                Vector2 target = (Vector2)transform.position + norm * 14 + Utils.RandCircle(6);
                Projectile.NewProjectile(transform.position, norm.RotatedBy(Utils.RandFloat(-135, 135) * Mathf.Deg2Rad) * -Utils.RandFloat(16f, 24f), 4, target.x, target.y);
            }
        }
    }
}

