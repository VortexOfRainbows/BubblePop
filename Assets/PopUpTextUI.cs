using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

public class PopUpTextUI : MonoBehaviour
{
    public static void Enable(string name, string desc)
    {
        Instance.SetName(name);
        Instance.SetDescription(desc);
        Instance.Visual.SetActive(true);
        enabledDuration = 2;
    }
    private static float enabledDuration = 0;
    public static PopUpTextUI Instance;
    public Canvas MainGameCanvas;
    public GameObject Visual;
    public TMPro.TextMeshProUGUI Name;
    public TMPro.TextMeshProUGUI Description;
    public RectTransform myRect;
    public void SetName(string name) => Name.text = name;
    public void SetDescription(string desc) => Description.text = desc;
    public void Start() => Instance = this;
    public void FixedUpdate()
    {
        if (myRect == null)
            myRect = GetComponent<RectTransform>();
        RectTransform visualRect = Visual.GetComponent<RectTransform>();
        visualRect.sizeDelta = new Vector2(myRect.sizeDelta.x, Description.renderedHeight + Name.renderedHeight + 20);
        Instance = this;
        transform.position = Input.mousePosition + new Vector3(40, -40);
        //Instance.gameObject.SetActive(false);
        if(--enabledDuration < 0)
        {
            Visual.SetActive(false);
        }

        float width = Screen.width - visualRect.rect.width * MainGameCanvas.scaleFactor;
        float height = visualRect.rect.height * MainGameCanvas.scaleFactor;
        //Clamp so it won't leave the boundaries of the screen
        //Debug.Log($"{transform.position}, {width}, {height}");
        transform.position = new Vector2(Mathf.Clamp(transform.position.x, 0, width), Mathf.Clamp(transform.position.y, height, 10000) + myRect.rect.height / 2 * MainGameCanvas.scaleFactor);
    }
}
