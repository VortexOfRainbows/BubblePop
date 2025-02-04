using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Weapon : Equipment
{
    public Vector3 WandEulerAngles = new Vector3(0, 0, 0);
    protected override void AnimationUpdate()
    {
        WandUpdate();
    }
    private void WandUpdate()
    {
        //if(Input.GetKeyDown(KeyCode.V))
        //{
        //    DamagePower++;
        //    ShotgunPower++;
        //}
        if (p.AttackLeft < -20 && p.AttackRight < 0)
        {
            if (Input.GetMouseButton(0))
            {
                AudioManager.PlaySound(GlobalDefinitions.audioClips[14], transform.position, 1f, 1f);
                p.AttackLeft = 50;
                p.bonusBubbles = p.ShotgunPower;
            }
        }
        if (p.AttackRight < -20 && p.AttackLeft < 0)
        {
            if (Input.GetMouseButton(1))
            {
                p.AttackRight = 50;
            }
        }

        Vector2 toMouse = Utils.MouseWorld - (Vector2)p.gameObject.transform.position;
        float dir = Mathf.Sign(toMouse.x);
        float bodyDir = Mathf.Sign(p.rb.velocity.x);
        Vector2 attemptedPosition = new Vector2(0.8f, 0.2f * dir).RotatedBy(toMouse.ToRotation()) + p.rb.velocity.normalized * 0.1f;

        //Debug.Log(attemptedPosition.ToRotation() * Mathf.Rad2Deg);

        p.PointDirOffset = -45 * dir * p.squash;
        p.MoveOffset = -5 * bodyDir * p.squash;
        p.DashOffset = 100 * dir * (1 - p.squash);

        Vector2 awayFromWand = new Vector2(1, 0).RotatedBy(transform.eulerAngles.z * Mathf.Deg2Rad);
        if (p.AttackLeft > 0)
        {
            bool canAttack = p.AttackLeft % 2 == 1 && p.AttackLeft >= 41;
            bool bonusBubble = p.bonusBubbles > 0 && p.AttackLeft >= 41;
            if (!canAttack && bonusBubble)
            {
                canAttack = true;
                p.bonusBubbles--;
            }
            if (canAttack)
            {
                float speed = Utils.RandFloat(9, 15) + 1.5f * p.FasterBulletSpeed;
                float spread = 12 + Mathf.Sqrt(p.ShotgunPower) * (10f / (10f + p.FasterBulletSpeed));
                Projectile.NewProjectile((Vector2)transform.position + awayFromWand * 2,
                    toMouse.normalized.RotatedBy(Utils.RandFloat(-spread, spread) * Mathf.Deg2Rad)
                    * speed + awayFromWand * Utils.RandFloat(2, 4) + new Vector2(Utils.RandFloat(-0.7f, 0.7f), Utils.RandFloat(-0.7f, 0.7f)));
                float odds = Mathf.Sqrt(1f / (p.AttackLeft - 40f));
                int attempts = p.bonusBubbles;
                while (attempts >= p.AttackLeft - 40)
                {
                    if (Utils.RandFloat(1) <= odds)
                    {
                        speed = Utils.RandFloat(9, 15) + 1.5f * p.FasterBulletSpeed;
                        Projectile.NewProjectile((Vector2)transform.position + awayFromWand * 2,
                            toMouse.normalized.RotatedBy(Utils.RandFloat(-spread, spread) * Mathf.Deg2Rad)
                            * speed + awayFromWand * Utils.RandFloat(2, 4) + new Vector2(Utils.RandFloat(-0.7f, 0.7f), Utils.RandFloat(-0.7f, 0.7f)));
                        p.bonusBubbles--;
                    }
                    attempts--;
                }
            }
            float percent = p.AttackLeft / 50f;
            p.PointDirOffset += 165 * percent * dir * p.squash;
        }
        if (p.AttackRight > 0)
        {
            if ((Input.GetMouseButton(1) || p.AttackRight < 100) && p.AttackRight >= 50)
            {
                if (p.AttackRight == 50)
                {
                    AudioManager.PlaySound(GlobalDefinitions.audioClips[33], Player.Position, 0.3f, 1.5f);
                    AudioManager.PlaySound(GlobalDefinitions.audioClips[34], Player.Position, 0.6f, 1f);
                    Projectile.NewProjectile((Vector2)transform.position + awayFromWand, Vector2.zero, 3, 0);
                }
                if (p.AttackRight < 250)
                {
                    p.AttackRight++;
                    if (p.AttackRight == 150)
                    {
                        AudioManager.PlaySound(GlobalDefinitions.audioClips[35], Player.Position, 0.65f, 1f);
                    }
                    if (p.AttackRight == 250)
                    {
                        AudioManager.PlaySound(GlobalDefinitions.audioClips[36], Player.Position, 0.7f, 1f);
                    }
                }
                p.PointDirOffset += -Mathf.Min(45f, (p.AttackRight - 50f) / 200f * 45f) * dir * p.squash;
            }
            else
            {
                if (p.AttackRight > 50)
                    p.AttackRight = 50;
                p.AttackRight--;
                float percent = p.AttackRight / 50f;
                p.PointDirOffset += 125 * percent * dir * p.squash;
            }
        }
        else
            p.AttackRight--;

        //Final Stuff
        float r = attemptedPosition.ToRotation() * Mathf.Rad2Deg - p.PointDirOffset - p.MoveOffset + p.DashOffset;
        transform.localPosition = Vector2.Lerp(transform.localPosition, attemptedPosition, 0.08f);
        gameObject.GetComponent<SpriteRenderer>().flipY = p.PointDirOffset < 0;
        WandEulerAngles.z = Mathf.LerpAngle(WandEulerAngles.z, r, 0.15f);
        transform.eulerAngles = new Vector3(0, 0, WandEulerAngles.z);
        p.AttackLeft--;
    }
    protected override void DeathAnimation()
    {
        transform.localPosition = Vector3.Lerp(transform.localPosition, new Vector3(0.4f, -0.6f), 0.1f);
        transform.eulerAngles = new Vector3(0, 0, Mathf.LerpAngle(transform.transform.eulerAngles.z, Mathf.Sign(p.lastVelo.x) == 1 ? 0 : 180, 0.1f));
    }
}
