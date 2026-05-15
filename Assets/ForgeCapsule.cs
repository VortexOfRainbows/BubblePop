using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
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
    public int GemCost { get; set; } = 5;
    public void UpdateUI(object owner = null)
    {
        if (!HasSelectedPower)
            return;
        Player p = Player.FindClosest(transform.position, out Vector2 norm, out float distance, 100);
        if (distance < 7 && p != null && p.ThisIsPlayerClosestInteractable(gameObject))
        {
            UI.LerpLocalScale(Vector2.one, 0.1f);
            UI.gameObject.SetActive(true);
            //if (owner.RestockRemaining <= 0)
            {
                bool canAfford = CoinManager.CurrentGems >= GemCost;
                Image i = UI.GetComponent<Image>();
                i.color = Color.Lerp(i.color, canAfford ? ColorHelper.UI.DefaultColor : ColorHelper.UI.DefaultColor, Utils.DeltaTimeLerpFactor(0.1f)).WithAlpha(0.5f);
                RestockCost.color = canAfford ? ColorHelper.UI.DefaultColor : ColorHelper.UI.RedColor;
                if (Input.GetKeyDown(KeyCode.R) && canAfford && (ChoicePowerMenu.Hide || !ChoicePowerMenu.Instance.gameObject.activeSelf))
                {
                    MyHammer.Begin(this, GemCost);
                    UI.gameObject.SetActive(false);
                }
            }
        }
        else
        {
            UI.gameObject.SetActive(false);
            UI.LerpLocalScale(Vector2.one * 0.8f, 0.5f);
        }
        RestockCost.text = GemCost.ToString();
    }
    public void InitPower()
    {
        HeldPower.group.sortingOrder = LayerHelper.CrucibleSortingOrder;
        HeldPower.transform.parent.localPosition = new Vector3(0, 1.7f, 0);
        int powerType = PowerUp.RandomFromPool(0f, 0.04f, -1);
        HeldPower.transform.parent.localScale = new Vector3(0f, 0f, 1);
        HeldPower.Type = powerType;
        HeldPower.Start();
        HeldPower.adornment.enabled = false;
        HeldPower.LightingMultiplier = 0.45f;
        ColoredWater = Fluid.color = ColorHelper.RarityColors[PowerUp.Get(powerType).GetRarity() - 1].WithAlpha(1);
        HasSelectedPower = true;
        HeldPower.gameObject.SetActive(true);
        GemCost = HeldPower.MyPower.CrucibleGems();
        if (HeldPower.MyPower.IsBlackMarket())
            GemCost *= 2;
    }
    public void FixedUpdate()
    {
        if (MyHammer == null)
            return;
        if (Main.WavesUnleashed && MyHammer.ProgressionNumber <= Main.PylonProgressionNumber && !HasSelectedPower)
            InitPower();
        if(HasSelectedPower)
            AnimatePower();
        else
        {
            Fluid.GetComponent<SpriteRenderer>().material.SetFloat("_VerticalOffset", -3);
            HeldPower.gameObject.SetActive(false);
        }
    }
    public float PowerAnimation { get; set; } = 0;
    public void AnimatePower()
    {
        Transform powerParent = HeldPower.transform.parent;
        SpriteRenderer fluidR = Fluid.GetComponent<SpriteRenderer>();
        float lerpyLerp = fluidR.material.GetFloat("_VerticalOffset");
        if (PowerAnimation > 0)
        {
            PowerAnimation -= Time.fixedDeltaTime;
            float percent = 1 - PowerAnimation;
            percent *= percent;
            Vector3 target = new Vector3(0, -0.2f, 0);
            Vector3 startingScale = new Vector3(0.63f, 0.63f);
            powerParent.localPosition = Vector3.Lerp(new Vector3(0, 1.7f, 0), target, percent);
            powerParent.localScale = Vector3.Lerp(startingScale, new Vector2(0.3f, 0.3f), percent);
            HeldPower.LightingMultiplier = 0.45f * (1 - percent);
            if (PowerAnimation <= 0)
            {
                powerParent.localScale = Vector3.zero;
                PowerAnimation = 0;
                GivePower();
                HeldPower.gameObject.SetActive(false);
            }
            Fluid.color = ColoredWater;
            Fluid.transform.localScale = new Vector3(1, 1, 1);
            fluidR.material.SetFloat("_CrystalLerp", 0);
            fluidR.material.SetFloat("_VerticalOffset", Mathf.Lerp(lerpyLerp, -2.1f, 0.1f));
        }
        else if(PowerAnimation < 0)
        {
            PowerAnimation += Time.fixedDeltaTime;
            float percent = 1 + PowerAnimation / 2.6f;
            float sin = Mathf.Sin(1.5f * percent * Mathf.PI) + percent;
            float iSin = 1 - sin;
            iSin = iSin * iSin;
            fluidR.material.SetFloat("_VerticalOffset", Mathf.Lerp(lerpyLerp, -2.1f * iSin, 0.1f));
            if(sin > 0.5f)
            {
                fluidR.material.SetFloat("_CrystalLerp", (sin - 0.5f) * 1.5f);
            }
            if(percent > 0.3f && percent <= 0.7f)
            {
                HeldPower.gameObject.SetActive(true);
                float newPercent = (percent - 0.3f) / 0.4f;
                float newSin = Mathf.Sin(newPercent * Mathf.PI * 3) + newPercent;
                float pSqr = newPercent * newPercent * 0.5f + 0.5f * Mathf.Sqrt( newPercent);
                powerParent.LerpLocalPosition(new Vector3(0, 1.6f + 0.1f * newSin * newPercent, 0), 0.1f);
                powerParent.localScale = new Vector3(0.63f * pSqr, 0.63f * pSqr, 1);
                HeldPower.LightingMultiplier = 0.45f * newPercent;
            }
            if (PowerAnimation > 0)
                PowerAnimation = 0;
        }
        else
        {
            powerParent.LerpLocalPosition(new Vector2(0, 1.7f), 0.1f);
            powerParent.LerpLocalScale(new Vector2(0.63f, 0.63f), 0.1f);
            fluidR.material.SetFloat("_CrystalLerp", 0);
            fluidR.material.SetFloat("_VerticalOffset", Mathf.Lerp(lerpyLerp, -2.1f, 0.1f));
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
