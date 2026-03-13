using System.Collections.Generic;
using UnityEngine;

public class Fizzy : Body
{
    public GameObject Skateboard;
    public bool OnSkateboard { get; private set; } = false;
    public float SkateboardMountCounter = 0;
    public float SkateboardMountTime => 100;
    protected override UnlockCondition UnlockCondition => UnlockCondition.Get<FizzyUnlock>();
    public override void Init()
    {
        Player.abilityTimer = 0;
        PrimaryColor = new Color(0.8f, 0.7f, 0.56f, 0.7f);
    }
    protected override void ModifyPowerPool(List<PowerUp> powerPool)
    {
        powerPool.Add(PowerUp.Get<WeaponUpgrade>());
        powerPool.Add(PowerUp.Get<WeaponUpgrade>());
    }
    public override void InitializeDescription(ref DetailedDescription description)
    {
        description.WithName("Fizzy").WithDescription("He's a pretty cool bubble");
    }
    public override void AbilityUpdate(ref Vector2 playerVelo, Vector2 moveSpeed)
    {
        if (Player.Control.Ability && !Player.Control.LastAbility && moveSpeed.magnitude > 0 && Player.AbilityReady)
        {
            SwapSkateboard();
        }
        if(OnSkateboard)
        {
            if(SkateboardMountCounter < SkateboardMountTime)
                SkateboardMountCounter++;
            if(SkateboardMountCounter >= 100)
            {
                Skateboard.transform.localPosition = new Vector3(0, Player.Accessory is Kicks ? -1.75f : -1.3f, 0);
            }
        }
        else
        {
            if (SkateboardMountCounter > 0)
                SkateboardMountCounter--;
        }
        float percent = SkateboardMountCounter / SkateboardMountTime;
        float sin = Mathf.Sin(percent * Mathf.PI);
        float scale = percent + sin * 0.5f;
        Skateboard.transform.LerpLocalScale(Vector2.one * scale, 0.1f);
    }
    public void SwapSkateboard()
    {
        if(SkateboardMountCounter >= 100 || SkateboardMountCounter <= 0)
        {
            OnSkateboard = !OnSkateboard;
            Skateboard.SetActive(true);
        }
    }

    public override void FaceUpdate()
    {
        Vector2 toMouse = p.LookPosition - (Vector2)transform.position;
        Vector2 toMouse2 = toMouse.normalized;
        toMouse2.x += Mathf.Sign(toMouse2.x) * 4;
        float toMouseR = toMouse2.ToRotation();
        Vector2 looking = new Vector2(0.08f, 0).RotatedBy(toMouseR);
        looking.x += 0.04f * p.Direction;
        Vector2 pos = looking;
        Face.transform.localPosition = Vector2.Lerp(Face.transform.localPosition, pos, 0.1f);
        Face.transform.eulerAngles = new Vector3(0, 0, toMouse2.y * 12.5f * p.Direction);
        Face.transform.localScale = new Vector3(p.Direction, 1, 1);
    }
}
