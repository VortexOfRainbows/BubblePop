using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PowerUpUIElement : MonoBehaviour
{
    public static readonly Color DefaultBubbleColor = new(1, 1, 1, 0.627451f);
    public static readonly Color GrayBubbleColor = new(.4f, .4f, .4f, 0.627451f);
    public static readonly Color GrayColor = new(.4f, .4f, .4f, 0.8f);
    public void SetPowerType(int PowerID)
    {
        if (PickerElement)
            Index = PowerID;
        else
            throw new Exception("Cannot set the power type of a power in the inventory!");
    }
    public Canvas myCanvas = null;
    public Image outer;
    public Image inner;
    public Image adornment;
    [SerializeField] private GameObject visual;
    public PowerUp MyPower => PowerUp.Get(Type);
    public Sprite Sprite => MyPower.sprite;
    private int prevType = -1;
    public int Type => InventoryElement ? (PlayerHasPower() ? Player.Instance.GetPower(Index) : -1) : Index;
    public TMPro.TextMeshProUGUI Count;
    public int Index = 0;
    public bool InventoryElement = true;
    /// <summary>
    /// Whether this element appears in the char select screen or compendium. Determines whether it can be locked (blacked out)
    /// </summary>
    public bool MenuElement = false;
    /// <summary>
    /// Whether this element appears in the compendium. If true, does not show a description
    /// </summary>
    public bool CompendiumElement = false;
    private float Timer;
    public PowerUpLayout myLayout;
    public bool GrayOut { get; set; }
    public bool SpecialLockedSprite { get; set; } = false;
    public bool ForceUnhideElement { get; set; } = false;
    public bool CrucibleElement { get; set; } = false;
    public bool UsePlaceHolder = false;
    public TextMeshProUGUI CostText;
    public GameObject CostObj;
    public bool PickerElement
    {
        get => !InventoryElement;
        set => InventoryElement = !value;
    }
    public bool PlayerHasPower()
    {
        return Index < Player.Instance.PowerCount;
    }
    public void TurnedOn()
    {
        MyPower.AliveUpdate(inner.gameObject, outer.gameObject, true);

        inner.sprite = Sprite;
        if(SpecialLockedSprite || UsePlaceHolder)
            inner.sprite = Main.TextureAssets.PowerUpPlaceholder;
        RectTransform rect = inner.transform as RectTransform;
        Rect rectangle = inner.sprite.rect;
        rect.pivot = inner.sprite.pivot / new Vector2(rectangle.width, rectangle.height);
        inner.SetNativeSize();

        Sprite adornSprite = SpecialLockedSprite ? null : MyPower.GetAdornment();
        if (adornSprite != null)
        {
            adornment.gameObject.SetActive(true);
            adornment.sprite = adornSprite;
            rect = adornment.transform as RectTransform;
            rectangle = adornment.sprite.rect;
            rect.pivot = adornment.sprite.pivot / new Vector2(rectangle.width, rectangle.height);
            adornment.SetNativeSize();
        }
        else
            adornment.gameObject.SetActive(false);

        outer.SetNativeSize();
        visual.SetActive(true);
        SetBorderColors();
        WhileOn();
    }
    public void SetBorderColors()
    {
        outer.material = adornment.material = MyPower.GetBorder(true);
        inner.material = MyPower.GetBorder();
    }
    public void TurnedOff()
    {
        Timer = 0;
        visual.SetActive(false);
    }
    public bool AppearLocked => (MenuElement && MyPower.PickedUpCountAllRuns <= 0 && !ForceUnhideElement) || SpecialLockedSprite;
    public bool PreventHovering;
    public bool ForceHideCount = false;
    public float HoverRadius { get; set; } = 64;
    /// <summary>
    /// How much this power cost to manufacture.
    /// Either Gems or Shards depending on context.
    /// </summary>
    public int Cost { get; set; } = -1;
    public void WhileOn()
    {
        Timer += 1;
        if(ForceHideCount)
            Count.gameObject.SetActive(false);
        else if(!CompendiumElement)
            Count.text = MyPower.Stack.ToString();
        else
            Count.gameObject.SetActive(!AppearLocked && (Compendium.Instance == null || Compendium.Instance.PowerPage.ShowCounts) && !PreventHovering);
        bool canHover = !PreventHovering && (myLayout == null || !myLayout.isHovering) && (!CompendiumElement || Compendium.Instance.PowerPage.MouseInCompendiumArea) && (!CrucibleElement || PowerUpCheatUI.MouseInCompendiumArea);
        float size = CompendiumElement ? 96 + HoverRadius - outer.rectTransform.rect.width : HoverRadius * transform.localScale.x;
        bool rectangular = CompendiumElement;
        if (canHover && Utils.IsMouseHoveringOverThis(rectangular, outer.rectTransform, size, myCanvas, CompendiumElement) && (CompendiumElement || !Main.GamePaused || !(InventoryElement || MenuElement)))
        {
            if(myLayout != null)
                myLayout.isHovering = true;
            string name = AppearLocked ? DetailedDescription.TextBoundedByRarityColor(MyPower.GetRarity() - 1, PowerUp.LockedName, MyPower.IsBlackMarket()) : MyPower.UnlockedName;
            string desc = AppearLocked ? PowerUp.LockedDescription : CompendiumElement ? "" : MyPower.FullDescription;
            PopUpTextUI.Enable(name, desc);
            float scaleUP = 1.125f;
            transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one * scaleUP, 0.16f);

            if(CompendiumElement)
            {
                if (Control.LeftMouseClick)
                    Compendium.Instance.PowerPage.UpdateSelectedType(Index);
                else if (Control.RightMouseClick)
                    Compendium.Instance.PowerPage.TierList.QueueRemoval = Index;
            }
        }
        else
            transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one, 0.16f);
        if(CostObj != null)
        {
            if(CrucibleElement)
            {
                if (PowerUpCheatUI.CurrentType == 1)
                {
                    CostObj.SetActive(!Main.DebugSettings.PowerUpCheat);
                    int cost = Cost * PowerUpCheatUI.ProcessQuantity;
                    CostText.text = cost.ToString();
                    bool canAfford = cost <= CoinManager.CurrentShards || Main.DebugSettings.PowerUpCheat;
                    CostText.color = canAfford ? ColorHelper.UIDefaultColor : ColorHelper.UIRedColor;
                }
                else
                {
                    CostObj.SetActive(false);
                }
            }
            else if(ChoicePowerMenu.IsBlackMarket)
            {
                CostObj.SetActive(ChoicePowerMenu.IsBlackMarket);
                int cost = Cost;
                bool canAfford = cost <= CoinManager.CurrentGems;
                CostText.text = cost.ToString();
                CostText.color = canAfford ? ColorHelper.UIDefaultColor : ColorHelper.UIRedColor;
            }
            else
                CostObj.SetActive(false);
        }
        UpdateAppearance();
    }
    public void UpdateAppearance()
    {
        if (GrayOut && !AppearLocked)
        {
            inner.color = adornment.color = GrayColor;
            outer.color = GrayBubbleColor;
        }
        else
        {
            outer.color = DefaultBubbleColor;
            if (AppearLocked)
            {
                inner.color = adornment.color = Color.black;
                if(SpecialLockedSprite)
                {
                    outer.color = Color.black;
                }
            }
            else if (MenuElement)
            {
                inner.color = adornment.color = Color.white;
            }
        }
    }
    public void Update()
    {
        if(!InventoryElement && !MenuElement)
            OnUpdate();
    }
    public void OnUpdate()
    {
        if (Type >= 0)
        {
            if ((!visual.activeSelf || prevType != Type) && Timer > 1)
            {
                TurnedOn();
                prevType = Type;
            }
            else
                WhileOn();
        }
        else
            TurnedOff();
    }
}
