using System.Collections.Generic;
using UnityEngine;
public class BubblemancerWand : Weapon
{
    protected override void ModifyPowerPool(List<PowerUp> powerPool)
    {
        //powerPool.Add<WeaponUpgrade>();
        powerPool.Add<Shotgun>();
        powerPool.Add<ChargeShot>();
        powerPool.Add<BubbleBlast>();
        powerPool.Add<SoapySoap>();
        powerPool.Add<ShotSpeed>();
        powerPool.Add<Starshot>();
    }
    protected override string Name()
    {
        return "Bubble Wand";
    }
    protected override string Description()
    {
        return "A magical bubble-blowing wand given to The Bubblemancer by a suspicious scientist";
    }
    protected override void AnimationUpdate()
    {
        WandUpdate();
    }
    private float AttackCooldown => 20f / Player.Instance.AttackSpeedModifier;
    public override void StartAttack(bool alternate)
    {
        if (AttackLeft < -AttackCooldown && AttackRight < 0)
        {
            if (!alternate)
            {
                AudioManager.PlaySound(GlobalDefinitions.audioClips[14], transform.position, 1f, 1f);
                AttackLeft = 50;
                p.bonusBubbles = p.ShotgunPower;
            }
        }
        if (AttackRight < -AttackCooldown && AttackLeft < 0)
        {
            if (alternate)
            {
                AttackRight = 50;
            }
        }
    }
    private void TryDoingStarShot(ref int starshotNum)
    {
        if (starshotNum <= 0)
            return;
        float chance = 0.1f + 0.05f * starshotNum;
        if (Utils.RandFloat(1f) < chance)
        {
            Vector2 toMouse = Utils.MouseWorld - (Vector2)p.gameObject.transform.position;
            Vector2 awayFromWand = new Vector2(1, 0).RotatedBy(transform.eulerAngles.z * Mathf.Deg2Rad);
            float spread = Mathf.Max(90 - p.FasterBulletSpeed * 5, 0);
            float speed = Utils.RandFloat(16, 19) + 1.6f * p.FasterBulletSpeed;
            Vector2 velocity = toMouse.normalized * speed + awayFromWand * 4;
            Vector2 norm = velocity.normalized * (12 + p.FasterBulletSpeed * 0.5f) + Utils.RandCircle(4) * (10f / (10f + p.FasterBulletSpeed));
            Projectile.NewProjectile((Vector2)transform.position + awayFromWand * 2, velocity.RotatedBy(Utils.RandFloat(-spread, spread) * Mathf.Deg2Rad), type: 4, transform.position.x + norm.x, transform.position.y + norm.y);

            float chanceOfLosingAStarProc = 0.6f * (10f / (9f + p.ShotgunPower)); //0.45 chance when you have one extra power, .409f when you have another, etc
            if (Utils.RandFloat(1) < chanceOfLosingAStarProc)
                --starshotNum;
        }
    }
    private void WandUpdate()
    {
        //if(Input.GetKeyDown(KeyCode.V))
        //{
        //    DamagePower++;
        //    ShotgunPower++;
        //}

        Vector2 toMouse = Utils.MouseWorld - (Vector2)p.gameObject.transform.position;
        float dir = Mathf.Sign(toMouse.x);
        float bodyDir = Mathf.Sign(p.rb.velocity.x);
        Vector2 attemptedPosition = new Vector2(0.8f, 0.2f * dir).RotatedBy(toMouse.ToRotation()) + p.rb.velocity.normalized * 0.1f;

        //Debug.Log(attemptedPosition.ToRotation() * Mathf.Rad2Deg);

        p.PointDirOffset = -45 * dir * p.squash;
        p.MoveOffset = -5 * bodyDir * p.squash;
        p.DashOffset = 100 * dir * (1 - p.squash);

        Vector2 awayFromWand = new Vector2(1, 0).RotatedBy(transform.eulerAngles.z * Mathf.Deg2Rad);
        if (AttackLeft > 0)
        {
            bool canAttack = AttackLeft % 2 == 1 && AttackLeft >= 41;
            bool bonusBubble = p.bonusBubbles > 0 && AttackLeft >= 41;
            if (!canAttack && bonusBubble)
            {
                canAttack = true;
                p.bonusBubbles--;
            }
            if (canAttack)
            {
                int starshotNum = p.Starshot;
                float speed = Utils.RandFloat(9, 15) + 1.5f * p.FasterBulletSpeed;
                float spread = 12 + Mathf.Sqrt(p.ShotgunPower) * (10f / (10f + p.FasterBulletSpeed));
                Projectile.NewProjectile((Vector2)transform.position + awayFromWand * 2,
                    toMouse.normalized.RotatedBy(Utils.RandFloat(-spread, spread) * Mathf.Deg2Rad)
                    * speed + awayFromWand * Utils.RandFloat(2, 4) + new Vector2(Utils.RandFloat(-0.7f, 0.7f), Utils.RandFloat(-0.7f, 0.7f)));
                TryDoingStarShot(ref starshotNum);
                float odds = Mathf.Sqrt(1f / (AttackLeft - 40f));
                int attempts = p.bonusBubbles;
                while (attempts >= AttackLeft - 40)
                {
                    if (Utils.RandFloat(1) <= odds)
                    {
                        speed = Utils.RandFloat(9, 15) + 1.5f * p.FasterBulletSpeed;
                        Projectile.NewProjectile((Vector2)transform.position + awayFromWand * 2,
                            toMouse.normalized.RotatedBy(Utils.RandFloat(-spread, spread) * Mathf.Deg2Rad)
                            * speed + awayFromWand * Utils.RandFloat(2, 4) + new Vector2(Utils.RandFloat(-0.7f, 0.7f), Utils.RandFloat(-0.7f, 0.7f)));
                        p.bonusBubbles--;
                        TryDoingStarShot(ref starshotNum);
                    }
                    attempts--;
                }
            }
            float percent = AttackLeft / 50f;
            p.PointDirOffset += 165 * percent * dir * p.squash;
        }
        if (AttackRight > 0)
        {
            if ((Input.GetMouseButton(1) || AttackRight < 100) && AttackRight >= 50)
            {
                if (AttackRight == 50)
                {
                    AudioManager.PlaySound(GlobalDefinitions.audioClips[33], Player.Position, 0.3f, 1.5f);
                    AudioManager.PlaySound(GlobalDefinitions.audioClips[34], Player.Position, 0.6f, 1f);
                    Projectile.NewProjectile((Vector2)transform.position + awayFromWand, Vector2.zero, 3, 149, 0);
                }
                if (AttackRight < 250)
                {
                    AttackRight++;
                    if (AttackRight == 150)
                    {
                        AudioManager.PlaySound(GlobalDefinitions.audioClips[35], Player.Position, 0.65f, 1f);
                    }
                    if (AttackRight == 250)
                    {
                        AudioManager.PlaySound(GlobalDefinitions.audioClips[36], Player.Position, 0.7f, 1f);
                    }
                }
                p.PointDirOffset += -Mathf.Min(45f, (AttackRight - 50f) / 200f * 45f) * dir * p.squash;
            }
            else
            {
                if (AttackRight > 50)
                    AttackRight = 50;
                AttackRight--;
                float percent = AttackRight / 50f;
                p.PointDirOffset += 125 * percent * dir * p.squash;
            }
        }
        else
            AttackRight--;

        //Final Stuff
        float r = attemptedPosition.ToRotation() * Mathf.Rad2Deg - p.PointDirOffset - p.MoveOffset + p.DashOffset;
        transform.localPosition = Vector2.Lerp(transform.localPosition, attemptedPosition, 0.08f);
        gameObject.GetComponent<SpriteRenderer>().flipY = p.PointDirOffset < 0;
        WandEulerAngles.z = Mathf.LerpAngle(WandEulerAngles.z, r, 0.15f);
        transform.eulerAngles = new Vector3(0, 0, WandEulerAngles.z);
        AttackLeft--;
    }
}
