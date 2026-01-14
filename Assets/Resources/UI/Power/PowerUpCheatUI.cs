using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PowerUpCheatUI : MonoBehaviour
{
    public static bool Hide { get; set; } = false;
    public static PowerUpCheatUI Instance { get; set; }
    public static int ProcessQuantity { get; set; } = 1;
    public static int CurrentType = -1;
    public static bool MouseInCompendiumArea { get; private set; }
    public static void TurnOn(int type = 0)
    {
        if(!Instance.gameObject.activeSelf || CurrentType != type)
        {
            Instance.Init(type);
        }
    }
    public static void TurnOff()
    {
        if (Instance.gameObject.activeSelf)
        {
            Instance.Disable();
        }
    }
    public PowerUpButton ChoiceTemplate;
    public GridLayoutGroup GridParent;
    public SliderInputField QuantitySlider;
    public Button QuantityUp, QuantityDown, HideButton;
    public Crucible CurrentCrucible { get; set; }
    public TextMeshProUGUI Title;
    public TextMeshProUGUI Description, HideButtonTextUI;
    public RectTransform SelectionArea;
    public Canvas MyCanvas;
    public void Start()
    {
        Instance = this;
        gameObject.SetActive(false);
        QuantityUp.onClick.AddListener(UpQuantity);
        QuantityDown.onClick.AddListener(DownQuantity);
        HideButton.onClick.AddListener(ToggleHide);
    }
    public void ToggleHide()
    {
        Hide = !Hide;
        if (!Hide && PlayerData.PauseDuringPowerSelect)
            Main.PauseGame();
        else
            Main.UnpauseGame();
    }
    public void UpQuantity()
    {
        int amt = 1;
        if (Input.GetKey(KeyCode.LeftShift))
            amt *= 5;
        if (Input.GetKey(KeyCode.LeftControl))
            amt *= 20;
        QuantitySlider.TryParseInput((ProcessQuantity + amt).ToString(), true);
    }
    public void DownQuantity()
    {
        int amt = 1;
        if (Input.GetKey(KeyCode.LeftShift))
            amt *= 5;
        if (Input.GetKey(KeyCode.LeftControl))
            amt *= 20;
        QuantitySlider.TryParseInput((ProcessQuantity - amt).ToString(), true);
    }
    public void Init(int type = 0)
    {
        Disable();
        gameObject.SetActive(true);
        CurrentType = type;
        transform.localScale = 0.9f * Vector3.one;
        if (type == 1)
        {
            //Using a couroutine here to make it less immediately laggy by spreading out the initalization of the stuff over frames.
            //This could maybe be done without a couroutine too, but would be more arduous
            Title.text = "Shards of Power";
            Description.text = "Use Shards to Clone Any Power";
            StartCoroutine(InitCheatButtons());
        }
        else
        {
            //Using a couroutine here to make it less immediately laggy by spreading out the initalization of the stuff over frames.
            //This could maybe be done without a couroutine too, but would be more arduous
            Title.text = "Crucible";
            Description.text = "Convert Powers to Gems";
            StartCoroutine(InitCrucibleButtons());
        }
    }
    public IEnumerator InitCheatButtons()
    {
        for (int i = 0; i < PowerUp.Reverses.Count; ++i)
        {
            PowerUpButton p = Instantiate(ChoiceTemplate, GridParent.transform);
            p.SetType(i);
            p.gameObject.SetActive(true);
            p.CheatButton = true;
            p.NonChoiceButton = true;
            p.PowerUI.CrucibleElement = true;
            yield return new WaitForSeconds(Time.fixedUnscaledDeltaTime);
        }
        yield return null;
    }
    public IEnumerator InitCrucibleButtons()
    {
        Player player = Player.Instance;
        for (int i = 0; i < player.Powers.Count; i++)
        {
            PowerUp power = PowerUp.Get(player.Powers[i]);
            PowerUpButton p = Instantiate(ChoiceTemplate, GridParent.transform);
            p.SetType(power.Type);
            p.gameObject.SetActive(true);
            p.PowerUI.Count.gameObject.SetActive(power.Stack > 1);
            p.PowerUI.Count.text = power.Stack.ToString();
            p.Crucible = CurrentCrucible;

            p.CheatButton = false;
            p.NonChoiceButton = true;
            p.PowerUI.CrucibleElement = true;
            yield return new WaitForSeconds(Time.fixedUnscaledDeltaTime);
        }
        yield return null;
    }
    public void Disable()
    {
        foreach(PowerUpButton t in GridParent.GetComponentsInChildren<PowerUpButton>(false))
            Destroy(t.gameObject);
        gameObject.SetActive(false);
    }
    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.C))
            ToggleHide();
        if (!ChoicePowerMenu.Hide && ChoicePowerMenu.Instance.gameObject.activeSelf)
        {
            if (!Hide)
                ToggleHide();
            HideButton.interactable = false;
        }
        else
            HideButton.interactable = true;
        Instance = this;
        UpdateContentSize();
        MouseInCompendiumArea = Utils.IsMouseHoveringOverThis(true, SelectionArea, 0, MyCanvas);
        float lerpT = Utils.DeltaTimeLerpFactor(0.125f);
        transform.LerpLocalScale(Vector2.one, Utils.DeltaTimeLerpFactor(0.1f));
        if (Hide)
        {
            transform.LerpLocalPosition(new Vector2(Main.ActivePrimaryCanvas.GetComponent<RectTransform>().rect.width / 2, 440), lerpT);
            HideButton.transform.LerpLocalPosition(new Vector2(ChoicePowerMenu.Hide && ChoicePowerMenu.Instance.gameObject.activeSelf ? 710 : 600, -205), lerpT);
            HideButtonTextUI.text = "Show Selector";
            return;
        }
        else
        {
            transform.LerpLocalPosition(new Vector2(Main.ActivePrimaryCanvas.GetComponent<RectTransform>().rect.width / 2, -Main.ActivePrimaryCanvas.GetComponent<RectTransform>().rect.height / 2), lerpT);
            HideButton.transform.LerpLocalPosition(new Vector2(600, -65), lerpT);
            HideButtonTextUI.text = "Hide Selector";
        }
    }
    public void UpdateContentSize()
    {
        int c = GridParent.transform.childCount;
        if (c <= 0)
            return;
        Vector3 lastElement = GridParent.transform.GetChild(c - 1).localPosition;
        RectTransform r = GridParent.GetComponent<RectTransform>();
        float dist = -lastElement.y + GridParent.padding.bottom * 3;
        r.sizeDelta = new Vector2(r.sizeDelta.x, dist - 600); //600 is the size of the canvas height
    }
}
