using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class OilHat : BubblemancerHat
{
    public override void ModifyUIOffsets(bool isBubble, ref Vector2 offset, ref float rotation, ref float scale)
    {
        offset = new Vector2(0.08f, -0.9f);
        scale *= 0.9425f;
        rotation = 5f;
    }
    public override void ModifyDescription(ref EquipDescription description)
    {
        description.RequestAbilitySlots(Ability.ID.Passive);
    }
    protected override UnlockCondition UnlockCondition => UnlockCondition.Get<KingOilUnlock>();
    protected override void ModifyPowerPool(List<PowerUp> powerPool)
    {
        powerPool.Add<Smokestack>();
        powerPool.Add<GoldenGun>();
        powerPool.Add<DiversifiedPortfolio>();
        powerPool.Add<CompoundInterest>();
        powerPool.Add<Pumpjack>();
    }
    protected override void ModifyPowerPoolForDiplayOnly(List<PowerUp> powerPool)
    {
        powerPool.Add<Futures>();
        powerPool.Add<Commodities>();
        powerPool.Add<Options>();
        powerPool.Add<Securities>();
        powerPool.Add<Windfall>();
    }
    public override void EquipUpdate()
    {
        Player.FirstChoiceIsInvestment = true;
    }
    public Transform ExhaustPipe;
    public float ExhaustAnimation { get; set; } = 0;
    protected override void AnimationUpdate()
    {
        float r = p.MoveDashRotation() * 0.4f;
        transform.localScale = new Vector3(p.Body.transform.localScale.x * (p.Body.Flipped ? -1 : 1), 0.75f + 0.225f * p.Body.transform.localScale.y, p.Body.transform.localScale.z);
        transform.localEulerAngles = Mathf.LerpAngle(transform.localEulerAngles.z, r, 0.1f) * Vector3.forward;
        transform.SetLocalXY(Vector2.Lerp((Vector2)transform.localPosition, new Vector2(0, 0.15f + 0.575f * p.Bobbing * p.Squash).RotatedBy(transform.eulerAngles.z * Mathf.Deg2Rad), 0.1f) + velocity);
        velocity *= 0.9f;

        if(Player.TotalInvestments > 0 && Player.SmokeStack > 0)
        {
            ExhaustAnimation += Mathf.Sqrt(Player.TotalInvestments);
            if(ExhaustAnimation > 75)
            {
                Color exhaustColor = Color.Lerp(Color.gray, Color.black, Utils.RandFloat(0.2f));
                Vector2 velo = Player.RB.velocity;
                if (velo.y < 0) 
                    velo.y *= 0.1f;
                ParticleManager.NewParticle((Vector2)ExhaustPipe.transform.position + Player.RB.velocity * Time.fixedDeltaTime, 1.0f, Vector2.up * 3 + velo, 0.5f, 3, ParticleManager.ID.Smoke, exhaustColor.WithAlpha(0.25f));
                ExhaustAnimation -= Utils.RandFloat(50, 75);
            }
        }
    }
}
