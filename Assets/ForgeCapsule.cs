using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ForgeCapsule : MonoBehaviour
{
    public bool HasSelectedPower { get; set; } = false;
    public Transform UI;
    public TextMeshProUGUI RestockCost;
    public PowerUpObject HeldPower;
    public SpriteRenderer Fluid;
    public ForgeHammer MyHammer { get; set; }
    public Color ColoredWater { get; set; }
    public void UpdateUI(object owner = null)
    {
        Player p = Player.FindClosest(transform.position, out Vector2 norm, out float distance, 100);
        if (distance < 7 && p != null && p.ThisIsPlayerClosestInteractable(gameObject))
        {
            UI.LerpLocalScale(Vector2.one, 0.1f);
            UI.gameObject.SetActive(true);
            //if (owner.RestockRemaining <= 0)
            {
                bool canAfford = CoinManager.CurrentGems >= 5;
                Image i = UI.GetComponent<Image>();
                i.color = Color.Lerp(i.color, canAfford ? ColorHelper.UI.DefaultColor : ColorHelper.UI.DefaultColor, Utils.DeltaTimeLerpFactor(0.1f)).WithAlpha(0.5f);
                RestockCost.color = canAfford ? ColorHelper.UI.DefaultColor : ColorHelper.UI.RedColor;
                if (Input.GetKeyDown(KeyCode.R) && canAfford && (ChoicePowerMenu.Hide || !ChoicePowerMenu.Instance.gameObject.activeSelf))
                {
                    MyHammer.Begin(this, 5);
                    UI.gameObject.SetActive(false);
                }
            }
        }
        else
        {
            UI.gameObject.SetActive(false);
            UI.LerpLocalScale(Vector2.one * 0.8f, 0.5f);
        }
        RestockCost.text = 5.ToString();
    }
    public void InitPower()
    {
        HeldPower.group.sortingOrder = LayerHelper.CrucibleSortingOrder;
        HeldPower.gameObject.SetActive(false);
        HeldPower.transform.parent.localPosition = new Vector3(0, 1.9f, 0);
        int powerType = PowerUp.RandomFromPool(0f, -1, -1);
        HeldPower.Type = powerType;
        HeldPower.Start();
        HeldPower.adornment.enabled = false;
        HeldPower.LightingMultiplier = 0.45f;
        ColoredWater = Fluid.color = ColorHelper.RarityColors[PowerUp.Get(powerType).GetRarity() - 1].WithAlpha(1);
        HasSelectedPower = true;
    }
    public void FixedUpdate()
    {
        if (!HasSelectedPower)
            InitPower();
        AnimatePower();
    }
    public float PowerAnimation { get; set; } = 0;
    public void AnimatePower()
    {
        Transform powerParent = HeldPower.transform.parent;
        HeldPower.gameObject.SetActive(true);
        if (PowerAnimation > 0)
        {
            PowerAnimation -= Time.fixedDeltaTime;
            float percent = 1 - PowerAnimation;
            percent *= percent;
            Vector3 target = new Vector3(0, -0.2f, 0);
            Vector3 startingScale = new Vector3(0.6f, 0.6f);
            powerParent.localPosition = Vector3.Lerp(new Vector3(0, 1.9f, 0), target, percent);
            powerParent.localScale = Vector3.Lerp(startingScale, new Vector2(0.3f, 0.3f), percent);
            HeldPower.LightingMultiplier = 0.45f * (1 - percent);
            if (PowerAnimation <= 0)
            {
                powerParent.localScale = Vector3.zero;
                PowerAnimation = 0;
                GivePower();
                HeldPower.gameObject.SetActive(false);
            }
            Fluid.color = Color.Lerp(ColoredWater, ColoredWater * 0.7f, percent).WithAlpha(1);
            Fluid.transform.localScale = new Vector3(1, 1 - percent, 1);
        }
        else if(PowerAnimation < 0)
        {
            PowerAnimation += Time.fixedDeltaTime;
            if(PowerAnimation > -1)
            {
                float percent = 1 + PowerAnimation;
                float pSqr = percent * 0.5f + 0.5f * percent * percent;
                powerParent.localPosition = new Vector3(0, 1.9f, 0);
                powerParent.localScale = new Vector3(0.6f * pSqr, 0.6f * pSqr, 1);
                Fluid.color = Color.Lerp(Color.green * 0.5f, ColoredWater, Mathf.Sqrt(percent)).WithAlpha(1);
                HeldPower.LightingMultiplier = 0.45f * percent;
            }
            else
            {
                float percent = 2 + PowerAnimation;
                percent *= percent;
                Fluid.color = Color.Lerp(ColoredWater * 0.7f, Color.green * 0.5f, percent).WithAlpha(1);
                Fluid.transform.localScale = new Vector3(1, percent * percent, 1);
            }
            if (PowerAnimation > 0)
                PowerAnimation = 0;
        }
    }
    public void GivePower()
    {
        Vector2 spawnPos = transform.position + new Vector3(0, -0.4f);
        AudioManager.PlaySound(SoundID.ChestDrop, spawnPos, 1, 1);
        PowerUpObject p = PowerUp.Spawn(HeldPower.Type, spawnPos).GetComponent<PowerUpObject>();
        p.FinalPosition = transform.position + new Vector3(0, -2);
        p.VelocityStyle = 1;
        p.velocity = new Vector2(0, 4);
    }
    public void DeployPower()
    {
        PowerAnimation = 1;
    }
}
