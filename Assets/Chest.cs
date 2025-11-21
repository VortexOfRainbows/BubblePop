using System;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour
{
    public void OnCollisionEnter2D(Collision2D collision) => OnTriggerStay2D(collision.collider);
    public void OnCollisionStay2D(Collision2D collision) => OnTriggerStay2D(collision.collider);
    public void OnTriggerEnter2D(Collider2D collision) => OnTriggerStay2D(collision);
    public void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            TryOpening();
    }
    public void TryOpening()
    {
        if(OpenAnimation <= 0 && CoinManager.CurrentKeys >= 1)
        {
            RB.velocity *= 0.0f;
            CoinManager.ModifyKeys(-1);
            Open();
        }
    }
    public BoxCollider2D Collider;
    public Rigidbody2D RB;
    public Transform Visual;
    public SpriteRenderer SpriteRenderer;
    public SpriteRenderer SpriteRendererKey;
    public SpriteRenderer SpriteRendererShadow;
    public SpriteRenderer Bubble;
    private float OGShadowAlpha = 0.0f;
    public Sprite ClosedSprite { get; private set; }
    public Sprite OpenSprite { get; private set; }
    public int BounceCount { get; private set; }
    public Vector2 Velocity;
    public int ChestType = 0;
    public int StarsAllocated { get; private set; }
    public int Direction => (BounceCount % 2 * 2 - 1);
    public float BounceHeight { get; private set; } = 0.75f;
    public bool OpenVertically = false;
    public void Init(int type)
    {
        ClosedSprite = Main.TextureAssets.T3Chest;
        OpenSprite = Main.TextureAssets.T3ChestOpen;
        SpriteRendererKey.enabled = OpenVertically = false;
        SpriteRendererKey.color = SpriteRendererKey.color.WithAlpha(0);
        ChestType = type;
        if (ChestType == 0)
        {
            ClosedSprite = Main.TextureAssets.T3Chest;
            OpenSprite = Main.TextureAssets.T3ChestOpen;
            StarsAllocated = 1;
            BounceHeight = 0.7f;
            Bubble.transform.localPosition = new Vector3(0, 0.95f, -1f);
            if(Utils.RandFloat(1) < 0.05f)
            {
                ClosedSprite = Main.TextureAssets.T3ChestUma;
                OpenSprite = Main.TextureAssets.T3ChestUmaOpen;
                OpenVertically = true;
            }
        }
        else if (ChestType == 1)
        {
            ClosedSprite = Main.TextureAssets.T2Chest;
            OpenSprite = Main.TextureAssets.T2ChestOpen;
            StarsAllocated = 2;
            Bubble.transform.localPosition = new Vector3(0, 1.1f, -1f);
        }
        else if (ChestType == 2)
        {
            ClosedSprite = Main.TextureAssets.T1Chest;
            OpenSprite = Main.TextureAssets.T1ChestOpen;
            StarsAllocated = 3;
            BounceHeight = 0.6f;
            Bubble.transform.localPosition = new Vector3(0, 1.025f, -1f);
        }
        OGShadowAlpha = SpriteRendererShadow.color.a;

        SpriteRenderer.sprite = ClosedSprite;
        SpawnAnimation = SpawnAnimation = OpenAnimation = EndAnimation = 0;
        HasSpawned = false;
        HasOpened = false;
        BounceCount = 2;
        BounceCount += Utils.RandInt(2);

        if(SkipSpawnAnimation)
        {
            Bubble.enabled = false;
            Collider.enabled = true;
            HasSpawned = true;
        }
        else
        {
            Collider.enabled = false;
            Visual.transform.localScale = Vector3.one * 0.1f;
            SpriteRendererShadow.transform.localScale = new Vector3(Visual.transform.localScale.x * 3, Visual.transform.localScale.y, 1);
            Visual.transform.LerpLocalEulerZ(12 * Direction, 1);
            AudioManager.PlaySound(SoundID.SoapSlide, transform.position, 0.5f, 0.9f, 0);
        }
    }
    public void Start()
    {
        Init(ChestType);
    }
    public void FixedUpdate()
    {
        //Collider.isTrigger = CoinManager.CurrentKeys > 0;
        if (Input.GetKey(KeyCode.R) && Main.DebugCheats)
        {
            Init(ChestType);
        }
        if (HasSpawned)
        {
            Visual.transform.LerpLocalEulerZ(0, 0.1f);
            Visual.transform.localPosition = Visual.transform.localPosition * 0.9f;
            if ((Control.Tab && Main.DebugCheats) || OpenAnimation > 0)
            {
                Open();
            }
            else
            {
                Close();
            }
        }
        else
        {
            SpawnAnimation += Time.fixedDeltaTime;
            float sqrt = MathF.Min(1, Mathf.Sqrt(SpawnAnimation));

            if (SpawnAnimation * SpawnAnimation >= 4)
            {
                float animation = SpawnAnimation - 2;
                if(Bubble.enabled)
                {
                    AudioManager.PlaySound(SoundID.BubblePop, transform.position, 1.15f, 1.0f, 0);
                    Velocity = new Vector2(0, 1);
                    Bubble.enabled = false;
                    for(int i = 0; i < 14; ++i)
                    {
                        ParticleManager.NewParticle(Visual.transform.position + new Vector3(Utils.RandFloat(-2, 2), Utils.RandFloat(0, 2.5f)), Utils.RandFloat(0.5f, 1.0f), Vector2.zero, 2, Utils.RandFloat(0.4f, 0.5f), 0);
                    }
                }
                Velocity.y -= 0.25f;
                if (Visual.transform.localPosition.y <= 0)
                {
                    Visual.transform.localPosition = new Vector3(0, 0, 0);
                    if (BounceCount > 0)
                    {
                        AudioManager.PlaySound(SoundID.SoapDie, transform.position, 0.7f, 0.8f, 0);
                        Velocity.y = Mathf.Max(0, (Velocity.y + 0.3f) * -BounceHeight);
                        BounceCount--;
                    }
                    else
                        Velocity.y = 0;
                }
                if (Velocity.y != 0 || BounceCount > 0)
                    Visual.transform.LerpLocalEulerZ(Mathf.Lerp(0, (11 + BounceCount) * Direction, Mathf.Max(0, Velocity.y * 1.2f)), Mathf.Abs(Velocity.y) * Time.fixedDeltaTime * 1.25f);
                else
                {
                    HasSpawned = true;
                }
                Visual.transform.localPosition += (Vector3)Velocity * Time.fixedDeltaTime;

            }
            else
            {
                float initPercent = Mathf.Clamp(sqrt * 1.0f + 0.5f * MathF.Sin(sqrt * MathF.PI), 0.1f, 1.5f);
                Visual.transform.localScale = Vector3.one * initPercent;
                SpriteRendererShadow.color = SpriteRendererShadow.color.WithAlpha(MathF.Min(1, SpawnAnimation) * 0.8f);

                float squeezeSin = 0.08f * MathF.Sin(SpawnAnimation * MathF.PI);
                Visual.transform.localScale = new Vector3(Visual.transform.localScale.x + squeezeSin, Visual.transform.localScale.y - squeezeSin, 0.6f);

                Visual.transform.localPosition = new Vector3(MathF.Sin(SpawnAnimation * 4f) * SpawnAnimation * 0.5f * Mathf.Max(0, 2 - SpawnAnimation) * Direction,
                    5 - Mathf.Min(5, SpawnAnimation * SpawnAnimation), 0);
                Bubble.enabled = true;

                if(initPercent < 0.8f)
                    ParticleManager.NewParticle(Visual.transform.position + new Vector3(Utils.RandFloat(-2, 2), Utils.RandFloat(0, 2.5f)), Utils.RandFloat(0.5f, 1.0f), Utils.RandCircleEdge(initPercent) * 4 + Vector2.up * 2, initPercent, 1.0f - 0.5f * initPercent, 0);
            }

            SpriteRendererShadow.transform.localScale = new Vector3(Visual.transform.localScale.x * 3, sqrt, 1);
            SpriteRendererShadow.transform.localPosition = new Vector3(Visual.transform.localPosition.x * 0.5f, 0, 1);
        }
        RB.velocity *= 0.94f;
    }
    public bool SkipSpawnAnimation = false;
    public bool HasOpened { get; private set; } = false;
    public bool HasSpawned { get; private set; } = false;
    public float SpawnAnimation = 0, OpenAnimation = 0, EndAnimation = 0;
    public float OpenDir = 0;
    public void Open()
    {
        if (OpenAnimation >= 1)
            Collider.enabled = false;
        if (!HasOpened)
        {
            if(OpenDir == 0)
            {
                if (Player.Position.x < transform.position.x)
                    OpenDir = -1;
                else
                    OpenDir = 1;
            }
            if(OpenAnimation >= 1)
            {
                AudioManager.PlaySound(SoundID.PickupPower, transform.position, 0.75f, 0.6f, 0);
                SpriteRenderer.sprite = OpenSprite;
                HasOpened = true;
                GenerateLoot();
                Visual.localScale = Vector3.one;
            }
            else
            {
                OpenAnimation += Time.fixedDeltaTime * 1.0f;
                if (OpenAnimation > 1)
                    OpenAnimation = 1;
                float cubed = OpenAnimation * OpenAnimation * OpenAnimation;
                if (cubed > 0.925f)
                    SpriteRendererKey.enabled = false;
                else
                {
                    float dir = OpenDir;
                    SpriteRendererKey.transform.localScale = new Vector3(OpenVertically ? -1 : dir, 1) * (1 + 0.25f * MathF.Sin(MathF.PI * Mathf.Min(1, OpenAnimation * 4)));
                    SpriteRendererKey.transform.LerpLocalEulerZ((OpenVertically ? 90 : dir * -90), 1f);
                    if(!SpriteRendererKey.enabled)
                    {
                        AudioManager.PlaySound(SoundID.ChargePoint, transform.position, 0.5f, 1.4f, 0);
                        SpriteRendererKey.enabled = true;
                    }
                    if (OpenVertically)
                        SpriteRendererKey.transform.localPosition = new Vector3(0, 1 + 1.6f * (1 - cubed), 0);
                    else
                        SpriteRendererKey.transform.localPosition = new Vector3(dir * 1.6f * (1 - cubed), 1, 0);
                    SpriteRendererKey.color = SpriteRendererKey.color.WithAlpha(Mathf.Min(1, OpenAnimation * 4) - Mathf.Clamp(cubed - 0.5f, 0, 0.5f) * 2f);
                }

                float sin = Mathf.Sin(OpenAnimation * Mathf.PI * 2.0f) * (0.4f + 0.6f * OpenAnimation);
                Visual.LerpLocalScale(new Vector2(1 - sin * 0.1f, 1 + sin * 0.08f), 0.5f);
            }
        }
        else
        {
            EndAnimation += Time.fixedDeltaTime;
            float timeOut = 5;
            if(EndAnimation > timeOut)
            {
                float endPercent = EndAnimation - timeOut;
                SpriteRenderer.color = SpriteRenderer.color.WithAlpha(1 - endPercent);
                SpriteRendererShadow.color = SpriteRendererShadow.color.WithAlpha(OGShadowAlpha * (1 - endPercent));
                if (endPercent > 1)
                {
                    Destroy(gameObject);
                }
            }
        }
    }
    public void Close()
    {
        Collider.enabled = true;
        OpenAnimation = 0;
        SpriteRenderer.sprite = ClosedSprite;
        HasOpened = false;
    }
    public void GenerateLoot()
    {
        List<int> powers = new();
        int s = StarsAllocated;
        while(s > 0)
        {
            int rare = Math.Max(1, Utils.RandInt(1, Math.Min(s + 1, 6)));
            int power = PowerUp.RandomFromPool(0.05f, OpenVertically ? 1f : -1, rare);
            powers.Add(power);
            s -= rare;
        }
        float c = powers.Count;
        float yOffset = 1.3f;
        int spentPowers = 0;

        for (int i = 0; i < c; i++)
        {
            float otherMult = (i + 0.5f - c / 2f);
            float j = otherMult * 25f;
            Vector2 circular = new Vector2(0, 2.5f + c * 0.6f).RotatedBy(j * Mathf.Deg2Rad);
            circular.y += yOffset;
            PowerUpObject power = PowerUp.Spawn(powers[i], (Vector2)transform.position + new Vector2(0, yOffset * 0.6f), 0).GetComponent<PowerUpObject>();
            power.velocity = circular * 3.5f;
            power.finalPosition = new Vector2(transform.position.x + circular.x, transform.position.y + yOffset);
            spentPowers += power.Cost;
        }

        Vector2 pos = transform.position + new Vector3(0, yOffset);
        if(spentPowers <= StarsAllocated + 2)
        {
            if(Utils.RandFloat(1) < 0.2f * StarsAllocated)
                CoinManager.SpawnHeart(pos, 1);
            else if(Utils.RandFloat(1) < 0.2f * StarsAllocated)
                CoinManager.SpawnKey(pos, 1);
            else if (Utils.RandFloat(1) < 0.1f + 0.1f * StarsAllocated)
                CoinManager.SpawnCoin(pos, (int)(StarsAllocated * Utils.RandFloat(10, 21) * WaveDirector.WaveMult), 1);
        }
    }
}
