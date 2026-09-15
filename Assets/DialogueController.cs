using TMPro;
using UnityEngine;

public class DialogueController : MonoBehaviour
{
    public Transform DialogueVisual;
    public TextMeshProUGUI DialogueText;
    public CanvasGroup MyGroup;
    public Canvas MyCanvas;
    public bool WantsOpen = true;
    public void Start()
    {
        MyGroup.alpha = 0f;
        WantsOpen = false;
        MyGroup.blocksRaycasts = false;
    }
    public void Update()
    {
        if(Main.DebugCheats && Input.GetKeyDown(KeyCode.N))
        {
            WantsOpen = !WantsOpen;
        }
        Vector2 defaultPosition = new(transform.localPosition.x, 0);
        float lerpT = Utils.DeltaTimeLerpFactor(0.1f);
        if(WantsOpen)
        {
            transform.LerpLocalPosition(defaultPosition, lerpT);
            MyGroup.alpha += Time.unscaledDeltaTime * 10f;
            DialogueVisual.gameObject.SetActive(true);
        }
        else
        {
            defaultPosition.y -= 20;
            transform.LerpLocalPosition(defaultPosition, lerpT);
            MyGroup.alpha -= Time.unscaledDeltaTime * 10f;
        }
        MyGroup.alpha = Mathf.Clamp01(MyGroup.alpha);
        if(MyGroup.alpha <= 0)
        {
            DialogueVisual.gameObject.SetActive(false);
        }
    }
}