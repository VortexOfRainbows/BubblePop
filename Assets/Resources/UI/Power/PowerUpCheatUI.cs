using UnityEngine;
using UnityEngine.UI;

public class PowerUpCheatUI : MonoBehaviour
{
    public static PowerUpCheatUI Instance { get; set; }
    public static int ProcessQuantity { get; set; } = 1;
    public PowerUpButton ChoiceTemplate;
    public GridLayoutGroup GridParent;
    public SliderInputField QuantitySlider;
    public Button QuantityUp, QuantityDown;
    public bool CheatCanvas { get; set; } = false;
    public void Start()
    {
        Instance = this;
        if (CheatCanvas)
        {
            for (int i = 0; i < PowerUp.Reverses.Count; ++i)
            {
                PowerUpButton p = Instantiate(ChoiceTemplate, GridParent.transform);
                p.SetType(i);
                p.gameObject.SetActive(true);
                p.CheatButton = true;
                p.NonChoiceButton = true;
            }
        }
        QuantityUp.onClick.AddListener(UpQuantity);
        QuantityDown.onClick.AddListener(DownQuantity);
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
    public void Init()
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

            p.CheatButton = true;
            p.NonChoiceButton = true;
        }
        gameObject.SetActive(true);
    }
    public void Disable()
    {
        foreach(PowerUpButton t in GridParent.GetComponentsInChildren<PowerUpButton>(false))
        {
            Destroy(t.gameObject);
        }
        gameObject.SetActive(false);
    }
    public void Update()
    {
        Instance = this;
        UpdateContentSize();
    }
    public void UpdateContentSize()
    {
        int c = GridParent.transform.childCount;
        if (c <= 0)
            return;
        Vector3 lastElement = GridParent.transform.GetChild(c - 1).localPosition;
        RectTransform r = GridParent.GetComponent<RectTransform>();
        float dist = -lastElement.y -GridParent.padding.bottom * 3;
        r.sizeDelta = new Vector2(r.sizeDelta.x, dist);
    }
}
