using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopUpTextUI : MonoBehaviour
{
    public const int DefaultPopupDuration = 400;
    public static Queue<PowerUp> PowerupQueue = new();
    public static void Enable(string name, string desc, int duration = 3)
    {
        Enable(Instance, name, desc, duration);
    }
    public static void Enable(PopUpTextUI instance, string name, string desc, int duration = DefaultPopupDuration)
    {
        instance.SetName(name);
        instance.SetDescription(desc);
        instance.Visual.SetActive(true);
        instance.enabledDuration = duration;
    }
    public static void UpdatePowerUp(PopUpTextUI instance, int type)
    {
        if(instance.PowerUpVisual != null)
        {
            instance.PowerUpVisual.SetPowerType(type);
        }
    }
    public RectTransform visualRect;
    public float enabledDuration = 0;
    public static PopUpTextUI Instance;
    public static PopUpTextUI MiddleInstance;
    public PowerUpUIElement PowerUpVisual;
    public Canvas MainGameCanvas;
    public GameObject Visual;
    public TMPro.TextMeshProUGUI Name;
    public TMPro.TextMeshProUGUI Description;
    public RectTransform myRect;
    public bool FollowMouse = false;
    public void SetName(string name) => Name.text = name;
    public void SetDescription(string desc) => Description.text = desc;
    public void Start()
    {
        if (FollowMouse)
            Instance = this;
        else
            MiddleInstance = this;
    }
    public void FixedUpdate()
    {
        if (myRect == null)
            myRect = GetComponent<RectTransform>();
        if (FollowMouse)
        {
            Instance = this;
            //Debug.Log(Input.mousePosition);
            transform.position = Input.mousePosition + new Vector3(40, -40);
            SnapToScreen();
        }
        else
        {
            MiddleInstance = this;
            UpdateMiddleInstance();
        }
        //Instance.gameObject.SetActive(false);
        if (--enabledDuration < 0)
        {
            Visual.SetActive(false);
        }
    }
    private List<Color> defaultColors = null;
    private List<Image> childImages = null;
    public StarUI[] stars;
    public void UpdateStars(float per, float grow)
    {
        int i = PowerUpVisual.MyPower.GetRarity() - 1;
        for(int j = 0; j < stars.Length; ++j)
        {
            stars[j].gameObject.SetActive(i == j);
        }
        stars[i].transform.localScale = 1.1f * grow * Vector3.one;
        stars[i].SetAlpha(per * per);
    }
    public void UpdateMiddleInstance()
    {
        SetHeightMiddle();
        float percent = 1 - enabledDuration / DefaultPopupDuration;
        float growPercent = 1 + 0.036f * Mathf.Sin(Mathf.Min(1, percent * 15) * Mathf.PI);
        transform.localScale = Vector3.one * growPercent;
        float halfPercent = Mathf.Max(0, percent * 2 - 1);
        float colPer = percent < 0.5f ? Mathf.Min(1, percent * 20) : Mathf.Sqrt(halfPercent > 1 ? 0 : Mathf.Max(1 - halfPercent));
        if(defaultColors == null)
        {
            defaultColors = new List<Color>();
            childImages = new List<Image>();
            GetComponentsInChildren(false, childImages);
            childImages.Add(PowerUpVisual.inner);
            foreach (Image i in childImages)
            {
                defaultColors.Add(i.color);
            }
            defaultColors.Add(Name.color);
            defaultColors.Add(Description.color);
        }
        else
        {
            int i = 0;
            for (; i < childImages.Count; ++i)
            {
                childImages[i].color = defaultColors[i].WithAlphaMultiplied(colPer);
            }
            Name.color = defaultColors[i++].WithAlphaMultiplied(colPer);
            Description.color = defaultColors[i++].WithAlphaMultiplied(colPer);
            UpdateStars(colPer, 1 + (growPercent - 1) * 3);
        }
        if(percent >= 1.05f)
        {
            if(PowerupQueue.Count > 0)
            {
                //swap to next in queue
                PowerUp p = PowerupQueue.Dequeue();
                UpdatePowerUp(this, p.Type);
                Enable(this, p.UnlockedName, p.ShortDescription);
            }
        }
    }
    private void SetHeightMiddle()
    {
        float height = Mathf.Max(155, Description.renderedHeight + Name.renderedHeight + 30);
        visualRect.sizeDelta = new Vector2(Mathf.Max(760, myRect.sizeDelta.x), height);
    }
    public void SnapToScreen()
    {
        visualRect.sizeDelta = new Vector2(myRect.sizeDelta.x, Description.renderedHeight + Name.renderedHeight + 30);
        float width = Screen.width - visualRect.rect.width * MainGameCanvas.scaleFactor;
        float height = visualRect.rect.height * MainGameCanvas.scaleFactor;
        //Clamp so it won't leave the boundaries of the screen
        //Debug.Log($"{transform.position}, {width}, {height}, {MainGameCanvas.scaleFactor}");
        transform.position = new Vector2(Mathf.Clamp(transform.position.x, 0, width), Mathf.Clamp(transform.position.y, height, 10000) + myRect.rect.height / 2 * MainGameCanvas.scaleFactor);
    }
    public void Update()
    {
        if (Main.GamePaused || Time.timeScale == 0)
            FixedUpdate();
    }
}
