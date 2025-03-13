using System.Collections.Generic;
using UnityEngine;

public class Body : Equipment
{
    public void SaveData()
    {
        PlayerData.SaveInt($"{TypeName}Hat", LastSelectedHat);
        PlayerData.SaveInt($"{TypeName}Acc", LastSelectedAcc);
        PlayerData.SaveInt($"{TypeName}Wep", LastSelectedWep);
    }
    public void LoadData()
    {
        LastSelectedHat = PlayerData.GetInt($"{TypeName}Hat", -1);
        LastSelectedAcc = PlayerData.GetInt($"{TypeName}Acc", -1);
        LastSelectedWep = PlayerData.GetInt($"{TypeName}Wep", -1);
        //Debug.Log($"{LastSelectedHat}, {LastSelectedAcc}, {LastSelectedWep}");
        if (LastSelectedHat < 0)
            LastSelectedHat = GetDefaultEquip(CharacterSelect.Instance.Hats);
        if (LastSelectedAcc < 0)
            LastSelectedAcc = GetDefaultEquip(CharacterSelect.Instance.Accessories);
        if (LastSelectedWep < 0)
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
                return e.IndexInTheAllEquipPool;
            }
        }
        return equipList[0].GetComponent<Equipment>().IndexInTheAllEquipPool;
    }
    public int LastSelectedHat = -1;
    public int LastSelectedAcc = -1;
    public int LastSelectedWep = -1;
    public Color PrimaryColor = ParticleManager.DefaultColor;
    public GameObject Face;
    public SpriteRenderer FaceR;
    protected virtual float AngleMultiplier => 1f;
    protected virtual float RotationSpeed => 0.12f;
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
            float r = transform.eulerAngles.z;
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
            spriteRender.flipY = r >= 90 && r < 270;
            bool flip = !spriteRender.flipY;
            transform.eulerAngles = new Vector3(0, 0, r);
        }
        transform.localScale = new Vector3(1 + (1 - p.squash) * 2.5f + 0.1f * (1 - p.Bobbing), p.Bobbing * p.squash, 1);
        Vector2 squashReAlign = new Vector2(0, p.Bobbing * p.squash - 1);
        transform.localPosition = squashReAlign;
        gameObject.SetActive(true);
        FaceUpdate();
    }
    protected override void DeathAnimation()
    {
        if (p.DeathKillTimer <= 0)
        {
            AudioManager.PlaySound(SoundID.Death.GetVariation(1), transform.position, 0.21f, 0.4f);
            for (int i = 0; i < 100; i++)
            {
                Vector2 circular = new Vector2(1, 0).RotatedBy(Mathf.PI * i / 25f);
                ParticleManager.NewParticle((Vector2)transform.position + circular * Utils.RandFloat(0, 1),
                    Utils.RandFloat(0.5f, 1.1f), circular * Utils.RandFloat(0, 24) + new Vector2(0, Utils.RandFloat(-2, 4)), 4f, Utils.RandFloat(1, 3), 0, Player.ProjectileColor);
            }
            gameObject.SetActive(false);
        }
    }
    public virtual void FaceUpdate()
    {
        Vector2 toMouse = Utils.MouseWorld - (Vector2)transform.position;
        toMouse *= Mathf.Sign(p.lastVelo.x);
        Vector2 pos = new Vector2(0.15f, 0) + toMouse.normalized * 0.25f;
        Face.transform.localPosition = Vector2.Lerp(Face.transform.localPosition, pos, 0.1f);
        FaceR.flipY = spriteRender.flipY;
    }
    public virtual void AbilityUpdate(ref Vector2 playerVelo, Vector2 moveSpeed)
    {

    }
}
