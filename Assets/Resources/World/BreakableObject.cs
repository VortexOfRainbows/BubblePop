using System;
using System.Collections.Generic;
using UnityEngine;

public class BreakableObject : MonoBehaviour, IImpactedByProjIFrames
{
    public static GameObject CratePrefab => WorldTilemap.CratePrefab;
    public static GameObject BarrelPrefab => WorldTilemap.BarrelPrefab;
    public static GameObject UrnPrefab => WorldTilemap.UrnPrefab;
    public static BreakableObject SpawnCrucibleVersion(Collider2D[] CrucibleColliders, Vector2 pos, Func<Vector2> velocity, int value = 1)
    {
        int rand = Utils.RandInt(4);
        GameObject prefab = rand <= 1 ? CratePrefab : rand == 2 ? BarrelPrefab : UrnPrefab;
        BreakableObject obj = Instantiate(prefab, pos, Quaternion.identity, Main.GenericSuperParent).GetComponent<BreakableObject>();
        obj.OverrideLootCoins = value;
        obj.RB.velocity = velocity.Invoke();
        obj.targetScale = Utils.RandFloat(0.7f, 0.8f);
        obj.transform.localScale = Vector2.zero;
        Physics2D.IgnoreCollision(obj.Collider, CrucibleColliders[0]);
        Physics2D.IgnoreCollision(obj.Collider, CrucibleColliders[1]);
        Physics2D.IgnoreCollision(obj.Collider, CrucibleColliders[2]);
        //int layerToClear = LayerMask.GetMask("WorldObj");
        //obj.Collider.includeLayers &= ~layerToClear;
        //obj.RB.includeLayers &= ~layerToClear;
        //obj.Collider.excludeLayers |= layerToClear;
        //obj.RB.excludeLayers |= layerToClear;
        return obj;
    }
    public enum BreakableObjectType
    {
        Crate = 0,
        Barrel = 1,
        Urn = 2,
    }
    public BreakableObjectType Type;
    public SpriteRenderer SpriteRenderer;
    public Rigidbody2D RB;
    public float targetScale = -1;
    public int OverrideLootCoins { get; set; } = -1;
    public int HitsRequiredToKill { get; set; } = 3;
    public void OnCollisionEnter2D(Collision2D collision) => OnTriggerStay2D(collision.collider);
    public void OnCollisionStay2D(Collision2D collision) => OnTriggerStay2D(collision.collider);
    public void OnTriggerEnter2D(Collider2D collision) => OnTriggerStay2D(collision);
    public void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Proj") && collision.gameObject.TryGetComponent(out Projectile p))
            ReceiveProjectileImpact(p);
    }
    public void ReceiveProjectileImpact(Projectile p)
    {
        if (AlreadyBroken)
            return;
        if (!p.SpecializedImmuneFrames.Contains(this) && ((p.Damage > 0 && p.Friendly) || p.Hostile))
        {
            if (p.Penetrate != -1 && --p.Penetrate == 0)
                p.Kill();
            else
                p.SpecializedImmuneFrames.Add(new Projectile.ImmunityData(this, p.immunityFrames));
            HitsRequiredToKill -= (int)Mathf.Max(1, p.Damage);
            if (HitsRequiredToKill <= 0 || p is MeleeHitbox)
            {
                Break();
            }
            else
            {
                if (Type == BreakableObjectType.Crate || Type == BreakableObjectType.Barrel)
                    AudioManager.PlaySound(SoundID.WoodBreak, transform.position, 0.6f, Utils.RandFloat(1.55f, 1.65f));
                else if (Type == BreakableObjectType.Urn)
                    AudioManager.PlaySound(SoundID.WoodBreak, transform.position, 0.6f, Utils.RandFloat(2.55f, 2.65f), 0);
                SpriteRenderer.color = Color.Lerp(SpriteRenderer.color, Color.red, 0.5f);
            }
        }
    }
    public bool AlreadyBroken { get; set; } = false;
    public CapsuleCollider2D Collider;
    public void Break()
    {
        AlreadyBroken = true;
        Color c = Color.white;
        float particleScale = 0.7f;
        if (Type == BreakableObjectType.Crate)
        {
            c = ColorHelper.WoodColor;
            AudioManager.PlaySound(SoundID.WoodBreak, transform.position, 1, Utils.RandFloat(0.875f, 0.925f));
        }
        else if (Type == BreakableObjectType.Barrel)
        {
            c = ColorHelper.WoodColor;
            AudioManager.PlaySound(SoundID.WoodBreak, transform.position, 1, Utils.RandFloat(0.9f, 0.975f));
        }
        else if (Type == BreakableObjectType.Urn)
        {
            AudioManager.PlaySound(SoundID.WoodBreak, transform.position, 1, Utils.RandFloat(1.875f, 1.925f));
            c = Color.gray;
            particleScale = 0.6f;
        }
        GenerateLoot();
        for (int i = 0; i < 30; ++i)
        {
            Vector2 randPos = Collider.bounds.min + new Vector3(Collider.bounds.extents.x * Utils.RandFloat(2f), Collider.bounds.extents.y * Utils.RandFloat(2f));
            ParticleManager.NewParticle(randPos, particleScale * Utils.RandFloat(0.8f, 1.0f), Utils.RandCircle(6) + Vector2.up * Utils.RandFloat(5, 10), 5, Utils.RandFloat(1, 1.2f), 1,
                Color.Lerp(c, Color.black, Utils.RandFloat(0.2f)));
        }
        Destroy(gameObject);
    }
    public void GenerateLoot()
    {
        int shields = 0;
        int hearts = 0;
        int keys = 0;
        int gems = 0;
        int coins = 0;
        int totalRolls = 1;
        float chanceOfBonusRoll = Player.Instance.BonusRecycleLoot;
        if (OverrideLootCoins > 0)
        {
            coins = (int)(OverrideLootCoins * (1 + chanceOfBonusRoll));
            totalRolls = -1;
        }
        else
        {
            if (chanceOfBonusRoll > 0)
            {
                int solidValue = (int)chanceOfBonusRoll;
                float remainingBonus = chanceOfBonusRoll - solidValue;
                if (Utils.RandFloat(1) < remainingBonus)
                    ++solidValue;
                else //Fallback to extra coins if the extra loot roll is failed
                    coins += (int)(1 + WaveDirector.WaveNum * remainingBonus);
                totalRolls += solidValue;
            }
        }
        int i;
        for (i = 0; i < totalRolls; ++i)
        {
            if (Type == BreakableObjectType.Crate)
            {
                float dropRand = Utils.RandFloat();
                if (dropRand < 0.02f)
                    ++shields;
                else if (dropRand < 0.04f)
                    ++hearts;
                else if (dropRand < 0.12f)
                    ++keys;
                else if (dropRand < 0.25f)
                    gems += Utils.RandInt(1, 2 + WaveDirector.WaveNum / 3);
                else if (dropRand < 0.75f)
                    coins += Utils.RandInt(1, 6 + WaveDirector.WaveNum);
            }
            else if (Type == BreakableObjectType.Barrel)
            {
                float dropRand = Utils.RandFloat();
                if (dropRand < 0.025f)
                    ++hearts;
                else if (dropRand < 0.1f)
                    gems += Utils.RandInt(1, 2 + WaveDirector.WaveNum / 3);
                else
                    coins += Utils.RandInt(1, 4 + WaveDirector.WaveNum);
            }
            else if (Type == BreakableObjectType.Urn)
            {
                float dropRand = Utils.RandFloat();
                if (dropRand < 0.025f)
                    ++shields;
                else if (dropRand < 0.1f)
                    ++keys;
                else
                    coins += Utils.RandInt(3, 6 + WaveDirector.WaveNum);
            }
        }
        for (i = 0; i < shields; ++i)
            CoinManager.SpawnShield(transform.position, .5f);
        for (i = 0; i < hearts; ++i)
            CoinManager.SpawnHeart(transform.position, .5f);
        for (i = 0; i < keys; ++i)
            CoinManager.SpawnKey(transform.position, .5f);
        if(gems > 0)
            CoinManager.SpawnGem(transform.position, .5f, gems);
        if (coins > 0)
            CoinManager.SpawnCoin(transform.position, coins, .5f, true);
    }
    public void FixedUpdate()
    {
        if(targetScale > 0)
            transform.LerpLocalScale(new Vector2(targetScale, targetScale), 0.1f);
        if(Type == BreakableObjectType.Crate)
            RB.velocity *= 0.94f;
        else if(Type == BreakableObjectType.Barrel || Type == BreakableObjectType.Urn)
            RB.velocity *= 0.95f;
        SpriteRenderer.color = Color.Lerp(SpriteRenderer.color, Color.white, 0.07f);
    }
}
