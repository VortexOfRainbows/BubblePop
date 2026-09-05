using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class MoneyIcon : MonoBehaviour
{
    public enum MoneyType
    {
        Money = 0,
        Keys = 1,
        Tokens = 2,
        Gems = 3,
        Ability = 4,
        Shard = 5,
    }
    public MoneyType Type = MoneyType.Money;
    public static float ScaleFactor = 1.1f;
    public Canvas myCanvas;
    public Image Icon;
    public GameObject Sparkle;
    public float SparkleTimer = 0;
    public Vector3 InitialScale = Vector3.zero;
    public Transform VisualParent;
    public void Start()
    {
        InitialScale = Icon.gameObject.transform.localScale;
        if (Type == MoneyType.Shard)
            VisualParent.gameObject.SetActive(false);
    }
    private void FixedUpdate()
    {
        if(Type == MoneyType.Shard && !VisualParent.gameObject.activeSelf)
        {
            if(CoinManager.CurrentShards > 0)
                VisualParent.gameObject.SetActive(true);
            return;
        }
        if (Utils.IsMouseHoveringOverThis(true, Icon.rectTransform, 0, myCanvas))
        {
            Icon.gameObject.transform.localScale = Vector3.Lerp(Icon.gameObject.transform.localScale, InitialScale * ScaleFactor, 0.1f);
            if(Type == MoneyType.Money)
                PopUpTextUI.Enable("Money", "Can be used to purchase fancy powerups");
            else if(Type == MoneyType.Keys)
                PopUpTextUI.Enable("Keys", "Can be used to open chests");
            else if(Type == MoneyType.Tokens)
                PopUpTextUI.Enable("Tokens", "Can be used to play Gacha Slots instead of money");
            else if(Type == MoneyType.Gems)
                PopUpTextUI.Enable("Gems", "Can be used to reroll Choices and power the Forge");
            else if(Type == MoneyType.Ability)
                PopUpTextUI.Enable("Ability", Player.Instance.Body.GetAbility().First((Ability x) => x.Type == Ability.ID.Ability).Blurb);
            else if (Type == MoneyType.Shard)
                PopUpTextUI.Enable("Shards", "Can be used to duplicate powers you own"); 
            if (Type != MoneyType.Ability)
                UpdateSparkle(Sparkle, true);
        }
        else
        {
            Icon.gameObject.transform.localScale = Vector3.Lerp(Icon.gameObject.transform.localScale, InitialScale * 1.0f, 0.1f);
            if(Type != MoneyType.Ability)
                UpdateSparkle(Sparkle, false);
        }
    }
    private void UpdateSparkle(GameObject obj, bool on)
    {
        obj.SetActive(on);
        if(on)
        {
            SparkleTimer++;
            obj.transform.localScale = Vector2.one * (1.2f + 0.1f * Mathf.Sin(SparkleTimer * 4 * Mathf.Deg2Rad));
        }
    }
}
