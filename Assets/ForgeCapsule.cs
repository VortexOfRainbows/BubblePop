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

        int powerType = PowerUp.RandomFromPool(0f, -1, -1);
        HeldPower.Type = powerType;
        HeldPower.Start();
        HeldPower.adornment.enabled = false;
        HeldPower.LightingMultiplier = 0.6f;
        Fluid.color = ColorHelper.RarityColors[PowerUp.Get(powerType).GetRarity() - 1].WithAlpha(0.2f);
        HasSelectedPower = true;
    }
    public void FixedUpdate()
    {
        if (!HasSelectedPower)
            InitPower();
        HeldPower.gameObject.SetActive(true);
    }
    public void DeployPower()
    {
        Vector2 spawnPos = transform.position + new Vector3(0, -0.4f);
        AudioManager.PlaySound(SoundID.ChestDrop, spawnPos, 1, 1);
        PowerUpObject p = PowerUp.Spawn(HeldPower.Type, spawnPos).GetComponent<PowerUpObject>();
        p.FinalPosition = transform.position + new Vector3(0, -2);
        p.VelocityStyle = 1;
        p.velocity = new Vector2(0, 4);
    }
}
