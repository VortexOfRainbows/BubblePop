using UnityEngine;
using UnityEngine.UI;

public class CompendiumPowerUpElement : CompendiumElement
{
    public static GameObject Prefab => Resources.Load<GameObject>("UI/Compendium/CompendiumPowerUpElement");
    public PowerUpUIElement MyElem;
    public int Style { get; set; }
    protected bool Selected { get; set; }
    public Button BlackMarketButton = null;
    public GameObject BlackMarketVisual, NormalVisual;
    public void Start()
    {
        if(BlackMarketButton != null)
            BlackMarketButton.onClick.AddListener(ToggleBlackMarket);
    }
    public void ToggleBlackMarket()
    {
        Compendium.Instance.PowerPage.ToggleBlackMarketMode();
        BlackMarketVisual.SetActive(!Compendium.Instance.PowerPage.BlackMarketMode);
        NormalVisual.SetActive(Compendium.Instance.PowerPage.BlackMarketMode);
        Debug.Log("Black Market Compendium Mode".WithColor(ColorHelper.RarityColors[5].ToHexString()));
    }
    public override void Init(int i, Canvas canvas)
    {
        MyElem.SetPowerType(TypeID = i);
        MyElem.myCanvas = canvas;
        MyElem.Count.text = PowerUp.Get(TypeID).PickedUpCountAllRuns.ToString();
        MyElem.GrayOut = GrayOut;
        int forceInitUpdates = 1;
        if (Style == 2)
        {
            BG.enabled = false;
            MyElem.ForceHideCount = true;
            transform.localScale = Vector3.one * 0.8f;
            transform.GetComponent<RectTransform>().pivot = Vector2.one * 0.5f;
            forceInitUpdates += 2;
        }
        for(int a = 0; a < forceInitUpdates; ++a)
            MyElem.OnUpdate();
    }
    public void Update()
    {
        if (TypeID == -1 && gameObject.activeSelf)
            Destroy(gameObject);
        MyElem.GrayOut = GrayOut;
        MyElem.OnUpdate();
        if(!MyElem.PreventHovering)
        {
            Selected = TypeID == Compendium.Instance.PowerPage.SelectedType;
            if (Style != 2)
            {
                Color target = Selected ? new Color(1, 1, .4f, 0.431372549f) : new Color(0, 0, 0, 0.431372549f);
                BG.color = Color.Lerp(BG.color, target, 0.125f);
            }
        }
        else if(BlackMarketButton != null)
        {
            bool canAppearAsBlackMarket = (MyElem.MyPower.IsBlackMarket() || MyElem.MyPower.HasBlackMarketAlternate)
                && (MyElem.MyPower.PickedUpCountAllRuns > 0 &&
                (MyElem.MyPower.BlackMarketVariantUnlockCondition == null || MyElem.MyPower.BlackMarketVariantUnlockCondition.Unlocked));
            BlackMarketButton.gameObject.SetActive(canAppearAsBlackMarket);
            if (BlackMarketButton.isActiveAndEnabled)
            {
                RectTransform r = BlackMarketButton.GetComponent<RectTransform>();
                if (Utils.IsMouseHoveringOverThis(true, r, 56, MyElem.myCanvas, true))
                {
                    PopUpTextUI.Enable(Compendium.Instance.PowerPage.BlackMarketMode ?
                        "Return to Normal".WithColor(ColorHelper.RarityColors[0].ToHexString()) : 
                        "Black Market Mode".WithColor(ColorHelper.RarityColors[5].ToHexString()), "");
                    BlackMarketButton.transform.LerpLocalScale(new Vector2(1.1f, 1.1f), Utils.DeltaTimeLerpFactor(.1f));
                }
                else
                {
                    BlackMarketButton.transform.LerpLocalScale(new Vector2(1.0f, 1.0f), Utils.DeltaTimeLerpFactor(.1f));
                }
            }
        }
    }
    public override void SetHovering(bool canHover)
    {
        MyElem.PreventHovering = !canHover;
    }
    public override CompendiumElement Instantiate(TierList parent, TierCategory cat, Canvas canvas, int i, int position)
    {
        CompendiumPowerUpElement cpue = Instantiate(Prefab).GetComponent<CompendiumPowerUpElement>();
        parent.InsertIntoTransform(cat.Grid.transform, cpue, position);
        cpue.Style = 2;
        cpue.MyElem.PreventHovering = true;
        cpue.SetGrayOut(true);
        cpue.MyElem.HoverRadius = 0;
        cpue.Init(i, canvas);
        return cpue;
    }
    public override bool IsLocked()
    {
        return MyElem.AppearLocked;
    }
    public override int GetRare(bool reverse = false)
    {
        var p = PowerUp.Get(TypeID);
        return p.GetRarity() + (p.IsBlackMarket() ? (Compendium.Instance.PowerPage.BlackMarketMode != reverse ? -5 : 5) : 0);
    }
    public override int GetCount()
    {
        return PowerUp.Get(TypeID).PickedUpCountAllRuns;
    }
}
