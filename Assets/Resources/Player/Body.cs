using System.Collections.Generic;
using UnityEngine;

public class Body : Equipment
{
    public override void ModifyLifeStats(ref int MaxLife, ref int Life, ref int MaxShield, ref int Shield)
    {
        MaxLife += 3;
        Life += 3;
    }
    public void SaveData()
    {
        PlayerData.SaveInt($"{TypeName}Hat", LastSelectedHat);
        PlayerData.SaveInt($"{TypeName}Acc", LastSelectedAcc);
        PlayerData.SaveInt($"{TypeName}Wep", LastSelectedWep);
    }
    public void LoadData()
    {
        LastSelectedHat = PlayerData.GetInt($"{TypeName}Hat");
        LastSelectedAcc = PlayerData.GetInt($"{TypeName}Acc");
        LastSelectedWep = PlayerData.GetInt($"{TypeName}Wep");
        //Debug.Log($"{LastSelectedHat}{LastSelectedAcc}{LastSelectedWep}");
        if (LastSelectedHat <= 0)
            LastSelectedHat = GetDefaultEquip(CharacterSelect.Instance.Hats);
        if (LastSelectedAcc <= 0)
            LastSelectedAcc = GetDefaultEquip(CharacterSelect.Instance.Accessories);
        if (LastSelectedWep <= 0)
            LastSelectedWep = GetDefaultEquip(CharacterSelect.Instance.Weapons);
    }
    public int GetDefaultEquip(List<GameObject> equipList)
    {
        for(int i = 0; i < equipList.Count; ++i)
        {
            Equipment e = equipList[i].GetComponent<Equipment>();
            if (e.SameUnlockAsBody(this))
            {
                //Debug.Log(e.name);
                return e.IndexInAllEquipPool;
            }
        }
        return equipList[0].GetComponent<Equipment>().IndexInAllEquipPool;
    }
    public int LastSelectedHat { get; set; } = -1;
    public int LastSelectedAcc { get; set; } = -1;
    public int LastSelectedWep { get; set; } = -1;
    public Color PrimaryColor = ParticleManager.DefaultColor;
    public GameObject Face => FaceR.gameObject;
    public SpriteRenderer FaceR;
    protected virtual float AngleMultiplier => 0.3f;
    protected virtual float RotationSpeed => 0.12f;
    public int FlipDir { get; set; } = 1;
    public bool Flipped => FlipDir == -1;
    public override void ModifyUIOffsets(bool isBubble, ref Vector2 offset, ref float rotation, ref float scale)
    {
        scale = 1.2f;
    }
    protected sealed override void AnimationUpdate()
    {
        float angleMult = 0.5f * AngleMultiplier;
        if (p.squash < 0.9f)
            angleMult = 1;
        if (p.lastVelo.sqrMagnitude > 0.10f)
        {
            float r = spriteRender.transform.eulerAngles.z;
            float angle = p.lastVelo.ToRotation() * Mathf.Rad2Deg;
            if (angle < 0)
                angle += 360;
            if (angle > 90 && angle <= 270)
            {
                angle = angle - 180;
                angle = 180 + angle * angleMult;
            }
            else
            {
                if (angle >= 270)
                    angle -= 360;
                angle *= angleMult;
            }
            r = Mathf.LerpAngle(r, angle, RotationSpeed);
            FlipDir = r >= 90 && r < 270 ? - 1 : 1;
            spriteRender.transform.eulerAngles = new Vector3(0, 0, r);
        }
        gameObject.transform.localScale = new Vector3(1 + 0.1f * (1 - p.Bobbing), p.Bobbing, 1);
        spriteRender.transform.localScale = 
            FaceR.transform.localScale = new Vector3(1 + (1 - p.squash) * 2.5f, p.squash, 1);
        spriteRender.transform.localScale = new Vector3(spriteRender.transform.localScale.x, 
            spriteRender.transform.localScale.y * FlipDir,
            spriteRender.transform.localScale.z);
        Vector2 squashReAlign = new(0, p.Bobbing * p.squash - 1);
        transform.localPosition = squashReAlign;
        gameObject.SetActive(true);
        FaceUpdate();
        if(player.Accessory is ShadyCoat && player.Body is not ThoughtBubble)
            player.Body.Face.SetActive(false);
        else
            player.Body.Face.SetActive(true);
    }
    protected override void DeathAnimation()
    {
        if (p.DeathKillTimer <= 0)
        {
            AudioManager.PlaySound(SoundID.Death.GetVariation(1), transform.position, 0.15f, 0.4f);
            for (int i = 0; i < 100; i++)
            {
                Vector2 circular = new Vector2(1, 0).RotatedBy(Mathf.PI * i / 25f);
                ParticleManager.NewParticle((Vector2)transform.position + circular * Utils.RandFloat(0, 1),
                    Utils.RandFloat(0.5f, 1.0f), circular * Utils.RandFloat(0, 24), 4f, Utils.RandFloat(1, 3), 0, Player.ProjectileColor);
            }
        }
        ModifyDeathAnimation();
    }
    public virtual void ModifyHurtAnimation()
    {
        AudioManager.PlaySound(SoundID.Death.GetVariation(1), transform.position, 0.1f, 0.6f);
        for (int i = 0; i < 15; i++)
        {
            Vector2 circular = new Vector2(1, 0).RotatedBy(Mathf.PI * i / 7.5f);
            ParticleManager.NewParticle((Vector2)transform.position + circular * Utils.RandFloat(0, 1),
                Utils.RandFloat(0.5f, 1.0f), circular * Utils.RandFloat(3, 18), 4f, Utils.RandFloat(1, 3), 0, Player.ProjectileColor);
        }
    }
    public virtual void ModifyDeathAnimation()
    {
        gameObject.SetActive(false);
    }
    public virtual void FaceUpdate()
    {
        Vector2 toMouse = p.LookPosition - (Vector2)transform.position;
        Vector2 pos = new Vector2(0.15f * p.Direction, 0) + toMouse.normalized * 0.21f;
        Face.transform.localPosition = Vector2.Lerp(Face.transform.localPosition, pos, 0.1f);
        FaceR.flipX = spriteRender.flipY;
    }
    public virtual void AbilityUpdate(ref Vector2 playerVelo, Vector2 moveSpeed)
    {

    }
    public virtual float AbilityCD => 0.5f;
    public override int GetRarity()
    {
        return 5;
    }
}
