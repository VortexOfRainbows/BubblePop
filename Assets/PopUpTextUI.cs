using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopUpTextUI : MonoBehaviour
{
    public const int DefaultPopupDuration = 400;
    public static Queue<PowerUp> PowerupQueue = new();
    public static void Enable(string name, string desc, int duration = 4)
    {
        Enable(Instance, name, desc, duration);
    }
    public static void Enable(PopUpTextUI instance, string name, string desc, int duration = DefaultPopupDuration)
    {
        if (instance.Name.text != name || instance.Description.text != desc)
        {
            instance.SetName(name);
            instance.SetDescription(desc);
            //instance.ReadyToRenderFixed = false;
            instance.ReadyToRenderNormal = false;
            instance.Visual.SetActive(false);
        }
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
    //public bool ReadyToRenderFixed = false;
    public bool ReadyToRenderNormal = false;
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
        //Instance.gameObject.SetActive(false);
        if (--enabledDuration < 0)
        {
            //ReadyToRenderFixed = false;
            ReadyToRenderNormal = false;
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
            childImages.Add(PowerUpVisual.adornment);
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
    public void SnapToScreen(bool sizeOnly = false)
    {
        Vector2 DescriptionSize = Description.GetRenderedValues(false);
        Vector2 NameSize = Name.GetRenderedValues(false);
        float DefaultSize = 610;
        float height = DescriptionSize.y + NameSize.y;
        if (height < NameSize.y)
            height = NameSize.y;
        height += 30;
        float width = Mathf.Min(DefaultSize, Mathf.Max(NameSize.x, DescriptionSize.x) + 10);
        visualRect.sizeDelta = new Vector2(width, height);
        if (!sizeOnly)
        {
            //Clamp so it won't leave the boundaries of the screen
            //Debug.Log($"{transform.position}, {width}, {height}, {MainGameCanvas.scaleFactor}");
            width = Screen.width - visualRect.rect.width * MainGameCanvas.scaleFactor;
            height = visualRect.rect.height * MainGameCanvas.scaleFactor;
            transform.position = new Vector2(Mathf.Clamp(transform.position.x, 0, width), Mathf.Clamp(transform.position.y, height, 10000) + myRect.rect.height / 2 * MainGameCanvas.scaleFactor);
        }
    }
    public void Update()
    {
        if (myRect == null)
            myRect = GetComponent<RectTransform>();
        if (enabledDuration >= 0)
        {
            if (!Visual.activeSelf)
            {
                if (FollowMouse)
                {
                    transform.position = new Vector3(1920, transform.position.y, 0);
                    SnapToScreen(true);
                    Visual.SetActive(true);
                }
                else
                {
                    transform.position = new Vector3(1920, transform.position.y, 0);
                    Visual.SetActive(true);
                }
            }
            else
            {
                ReadyToRenderNormal = true;
                if(!FollowMouse)
                    transform.localPosition = new Vector3(1035, -150, 0);
            }
        }
        if(FollowMouse)
            Instance = this;
        else
        {
            UpdateMiddleInstance();
            MiddleInstance = this;
        }
        if (ReadyToRenderNormal)
        {
            if (FollowMouse)
            {
                //Debug.Log(Input.mousePosition);
                transform.position = Input.mousePosition + new Vector3(40, -40);
                SnapToScreen();
            }
        }
        if (Main.GamePaused)
            FixedUpdate();
    }
}
