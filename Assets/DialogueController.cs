using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DialogueController : MonoBehaviour
{
    public static Queue<string> DialogueQueue = new();
    public static DialogueController Instance;
    public static void EnqueueDialogue(string dialogue)
    {
        DialogueQueue.Enqueue(dialogue);
        Instance.WantsOpen = true;
        Instance.AnimationCounter = 0;
    }
    public static void EnqueueDialogueChain(IEnumerable<string> dialogueChain)
    {
        foreach(string dialogue in dialogueChain)
            DialogueQueue.Enqueue(dialogue);
        Instance.WantsOpen = true;
        Instance.AnimationCounter = 0;
    }
    public static void EnqueueDialogueChain(params string[] dialogueChain)
    {
        foreach(string dialogue in dialogueChain)
            DialogueQueue.Enqueue(dialogue);
        Instance.WantsOpen = true;
        Instance.AnimationCounter = 0;
    }
    public Transform DialogueVisual;
    public TextMeshProUGUI DialogueText;
    public CanvasGroup MyGroup;
    public Canvas MyCanvas;
    public bool WantsOpen = true;
    public float AnimationCounter = 0;
    public void Start()
    {
        Instance = this;
        MyGroup.alpha = 0f;
        WantsOpen = false;
        MyGroup.blocksRaycasts = false;
    }
    public void Update()
    {
        Instance = this;
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
            UpdateVisibleDialogue();
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
    public void UpdateVisibleDialogue()
    {
        AnimationCounter += Time.deltaTime;
        if (DialogueQueue.Count > 0)
        {
            DialogueText.text = DialogueQueue.Peek();
            if((Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Space)) && AnimationCounter >= 1)
            {
                AnimationCounter = 0;
                DialogueQueue.Dequeue();
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Space))
            {
                WantsOpen = false;
            }
            DialogueText.text = "MISSING DIALOGUE";
        }
    }
}