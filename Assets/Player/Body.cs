using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Body : Equipment
{
    public struct EquipSaveData
    {
        public int Type;
        public int ParentCategory;
        public EquipSaveData(int type, int parent)
        {
            Type = type;
            ParentCategory = parent;
        }
    }
    public void SaveData()
    {
        PlayerData.SaveEquipmentData($"{TypeName}Hat", LastSelectedHat);
        PlayerData.SaveEquipmentData($"{TypeName}Acc", LastSelectedAcc);
        PlayerData.SaveEquipmentData($"{TypeName}Wep", LastSelectedWep);
    }
    public void LoadData()
    {
        LastSelectedHat = PlayerData.LoadEquipmentData($"{TypeName}Hat");
        LastSelectedAcc = PlayerData.LoadEquipmentData($"{TypeName}Acc");
        LastSelectedWep = PlayerData.LoadEquipmentData($"{TypeName}Wep");
    }
    public EquipSaveData LastSelectedHat;
    public EquipSaveData LastSelectedAcc;
    public EquipSaveData LastSelectedWep;
    public Color PrimaryColor = ParticleManager.DefaultColor;
    public GameObject Face;
    public SpriteRenderer FaceR;
    protected virtual float AngleMultiplier => 1f;
    protected virtual float RotationSpeed => 0.12f;
    public override void ModifyUIOffsets(ref Vector2 offset, ref float rotation, ref float scale)
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
    protected sealed override void DeathAnimation()
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
