using System;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour
{
    public Transform Visual;
    public SpriteRenderer SpriteRenderer;
    public SpriteRenderer SpriteRendererKey;
    public SpriteRenderer SpriteRendererShadow;
    private float OGShadowAlpha = 0.0f;
    public Sprite ClosedSprite { get; private set; }
    public Sprite OpenSprite { get; private set; }
    public int ChestType = 0;
    public int StarsAllocated = 3;
    public void Init(int type)
    {
        ClosedSprite = Main.TextureAssets.T3Chest;
        OpenSprite = Main.TextureAssets.T3ChestOpen;
        SpriteRendererKey.enabled = false;
        SpriteRendererKey.color = SpriteRendererKey.color.WithAlpha(0);
        ChestType = type;
        if (ChestType == 0)
        {
            ClosedSprite = Main.TextureAssets.T2Chest;
            OpenSprite = Main.TextureAssets.T2ChestOpen;
            StarsAllocated = 5;
        }
        if (ChestType == 1)
        {
            ClosedSprite = Main.TextureAssets.T3Chest;
            OpenSprite = Main.TextureAssets.T3ChestOpen;
            StarsAllocated = 3;
        }
        OGShadowAlpha = SpriteRendererShadow.color.a;
    }
    public void Start()
    {
        Init(ChestType);
    }
    public void FixedUpdate()
    {
        if(Control.Tab || OpenAnimation > 0)
        {
            Open();
        }
        else
        {
            Close();
        }
    }
    public bool HasOpened = false;
    public float OpenAnimation = 0;
    public float EndAnimation = 0;
    public float OpenDir = 0;
    public void Open()
    {
        if(!HasOpened)
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
                    SpriteRendererKey.transform.localScale = new Vector3(ChestType == 1 ? -1 : dir, 1) * (1 + 0.25f * MathF.Sin(MathF.PI * Mathf.Min(1, OpenAnimation * 4)));
                    SpriteRendererKey.transform.LerpLocalEulerZ((ChestType == 1 ? 90 : dir * -90), 1f);
                    SpriteRendererKey.enabled = true;
                    if(ChestType == 1)
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
            int power = PowerUp.RandomFromPool(0.05f, -1f, rare);
            powers.Add(power);
            s -= rare;
        }
        float c = powers.Count;
        float yOffset = 1.3f;
        for (int i = 0; i < c; i++)
        {
            float otherMult = (i + 0.5f - c / 2f);
            float j = otherMult * 25f;
            Vector2 circular = new Vector2(0, 2.5f + c * 0.6f).RotatedBy(j * Mathf.Deg2Rad);
            circular.y += yOffset;
            PowerUpObject power = PowerUp.Spawn(powers[i], (Vector2)transform.position + new Vector2(0, yOffset * 0.6f), 0).GetComponent<PowerUpObject>();
            power.velocity = circular * 3.5f;
            power.finalPosition = new Vector2(transform.position.x + circular.x, transform.position.y + yOffset);
        }
    }
}
