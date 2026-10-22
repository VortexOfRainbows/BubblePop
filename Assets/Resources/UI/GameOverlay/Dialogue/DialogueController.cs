using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static NPC;

public class DialogueController : MonoBehaviour
{
    public class DialogueEvent
    {
        public bool SpeakerIsPlayer = false;
        public string text;
        public NPCType LeftType = NPCType.None;
        public NPCType RightType = NPCType.None;
        public int LeftExpression = -1;
        public int RightExpression = -1;
        public DialogueEvent(string text, NPCType LeftCharacter = NPCType.None, int LeftExpression = -1, NPCType RightCharacter = NPCType.None, int RightExpression = -1, bool speakerIsPlayer = false)
        {
            this.text = text;
            LeftType = LeftCharacter;
            RightType = RightCharacter;
            this.LeftExpression = LeftExpression;
            this.RightExpression = RightExpression;
            SpeakerIsPlayer = speakerIsPlayer;
        }
    }
    public static Queue<DialogueEvent> DialogueQueue = new();
    public static DialogueController Instance;
    public static void EnqueueDialogue(DialogueEvent dialogue)
    {
        DialogueQueue.Enqueue(dialogue);
        Instance.WantsOpen = true;
        Instance.AnimationCounter = 0;
    }
    public static void EnqueueDialogueChain(IEnumerable<DialogueEvent> dialogueChain)
    {
        foreach(DialogueEvent dialogue in dialogueChain)
            DialogueQueue.Enqueue(dialogue);
        Instance.WantsOpen = true;
        Instance.AnimationCounter = 0;
    }
    public static void EnqueueDialogueChain(params DialogueEvent[] dialogueChain)
    {
        foreach(DialogueEvent dialogue in dialogueChain)
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
    public Portrait LeftPortrait;
    public Portrait RightPortrait;
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
            WantsOpen = !WantsOpen;

        LeftPortrait.enabled = RightPortrait.enabled = WantsOpen;

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
            DialogueEvent dEvent = DialogueQueue.Peek();
            DialogueText.text = dEvent.text;

            LeftPortrait.SetExpression(dEvent.LeftType, dEvent.LeftExpression);
            RightPortrait.SetExpression(dEvent.RightType, dEvent.RightExpression);

            if ((Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Space)) && AnimationCounter >= 1)
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