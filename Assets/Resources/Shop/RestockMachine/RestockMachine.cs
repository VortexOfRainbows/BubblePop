using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RestockMachine : MonoBehaviour
{
    public SpriteRenderer[] Numbers;
    public SpriteRenderer DisplayNumber;
    public Transform UI;
    public TextMeshProUGUI RestockCost;
    public Transform GumballMachine;
    public GameObject Smile, OpenSmile;
    public float NumberSwapTimer = 0;
    public float AnimationTimer = 0;
    public void SetRestockAmountToShopStock(GachaponShop Owner)
    {
        int index = Owner.RestockRemaining;
        if (index >= Numbers.Length - 1)
            index = Numbers.Length - 2;
        if (Owner.RestockRemaining >= 99999)
            index = Numbers.Length - 1;
        DisplayNumber.sprite = Numbers[index].sprite;
        DisplayNumber.transform.localPosition = Numbers[index].transform.localPosition;
        DisplayNumber.transform.localScale = Vector3.one * 0.5f;
        DisplayNumber.transform.localRotation = Numbers[index].transform.localRotation;
        NumberSwapTimer = 0;
    }
    public void UpdateUI(GachaponShop owner)
    {
        Player.FindClosest(transform.position, out Vector2 norm, out float distance, 100);
        if (distance < 7 && owner.FillUpTimer <= 0 && owner.RestockRemaining <= 0)
        {
            UI.LerpLocalScale(Vector2.one, 0.1f);
            UI.gameObject.SetActive(true);
            if(owner.RestockRemaining <= 0)
            {
                bool canAfford = CoinManager.CurrentGems >= owner.RestockCost;
                Image i = UI.GetComponent<Image>();
                i.color = Color.Lerp(i.color, canAfford ? ColorHelper.UI.DefaultColor : ColorHelper.UI.DefaultColor, Utils.DeltaTimeLerpFactor(0.1f)).WithAlpha(0.5f);
                RestockCost.color = canAfford ? ColorHelper.UI.DefaultColor : ColorHelper.UI.RedColor;
                if (Input.GetKeyDown(KeyCode.R) && canAfford && (ChoicePowerMenu.Hide || !ChoicePowerMenu.Instance.gameObject.activeSelf))
                {
                    owner.TryAddingRemainingRestocks(owner.RestockCost);
                    UI.gameObject.SetActive(false);
                }
            }
        }
        else
        {
            UI.gameObject.SetActive(false);
            UI.LerpLocalScale(Vector2.one * 0.8f, 0.5f);
        }
        RestockCost.text = owner.RestockCost.ToString();
    }
    public void FixedUpdate()
    {
        Animate();
        AnimateNumber();
    }
    public void Animate()
    {
        AnimationTimer += Time.fixedDeltaTime * 0.8f;
        float sin = Mathf.Sin(AnimationTimer * Mathf.PI);
        float sin2 = Mathf.Sin(AnimationTimer * Mathf.PI * 0.5f);
        float sin3 = Mathf.Sin(NumberSwapTimer * Mathf.PI) * 0.04f;
        float stretch = sin * 0.025f;
        GumballMachine.transform.localScale = new Vector3(1 + stretch + sin3, 1 - stretch + sin3, 1);
        transform.localScale = new Vector3(1 - stretch * 0.3f, 1 + stretch * 0.3f, 1);
        float tilt = 2.25f * sin2;
        GumballMachine.transform.SetLocalEulerZ(tilt);
    }
    public void AnimateNumber()
    {
        NumberSwapTimer += Time.fixedDeltaTime * 2.5f;
        if (NumberSwapTimer > 1)
            NumberSwapTimer = 1;
        //bool smile = NumberSwapTimer > 0 && NumberSwapTimer < 1;
        //Smile.SetActive(!smile);
        //OpenSmile.SetActive(smile);
        float sin = Mathf.Sin(NumberSwapTimer * Mathf.PI);
        float scale = sin * sin * 0.5f + NumberSwapTimer;
        DisplayNumber.transform.localScale = Vector3.one * (0.5f + 0.5f * scale);
        Smile.transform.localScale = OpenSmile.transform.localScale = DisplayNumber.transform.localScale;
    }
}
