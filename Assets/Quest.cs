using System;
using TMPro;
using UnityEngine;

public class Quest : MonoBehaviour
{
    public enum QuestType
    { 
        Dummy,
        SurviveAgainstInvaders,
        ActivatePylon,
        StabilizePylon,
    }
    public class QuestData
    {
        public QuestType Type;
        public QuestData Sequel { get; private set; }
        public QuestData(string text, string progressText, QuestType Type, QuestData sequelQuest = null)
        {
            Text = text;
            ProgressText = progressText;
            this.Type = Type; 
            Sequel = sequelQuest;
        }
        public string Text { get; private set; }
        public string ProgressText { get; set; }
        public string CompleteText => Text + "\n" + ProgressText;
        internal bool CheckForCompletion()
        {
            bool setComplete = false;
            switch(Type)
            {
                case QuestType.SurviveAgainstInvaders:
                    if (Main.CurrentPylon == null)
                        setComplete = true;
                    else if(WaveDirector.SkullEnemiesActive <= 0 && !WaveDirector.WaveActive)
                    {
                        if (Main.CurrentPylon.Complete)
                            setComplete = true;
                        else
                        {
                            string quant = Main.CurrentPylon.WavesRequired > 0 && !Main.CurrentPylon.EndlessPylon ? Main.CurrentPylon.WavesRequired.ToString() : "Endless";
                            ProgressText = $"Waves: {Main.CurrentPylon.WavesPassed}/{quant}";
                        }
                    }
                    break;
                case QuestType.StabilizePylon:
                    if (Main.CurrentPylon == null || Main.CurrentPylon.Purified)
                        setComplete = true;
                    else
                    {
                        Player.FindClosest(Main.CurrentPylon.transform.position, out _, out float dist);
                        dist -= Main.PylonActivationDist * 0.9f;
                        if (dist <= 0)
                        {
                            setComplete = Main.CurrentPylon.Purified;
                            ProgressText = "In Progress";
                        }
                        else
                            ProgressText = $"Distance: {(int)(dist + 0.9999f):#}";
                    }
                    break;
                case QuestType.ActivatePylon:
                    if (Main.NextPylon == null || (Main.NextPylon == Main.CurrentPylon && Main.PylonActive))
                        setComplete = true;
                    else
                    {
                        Player.FindClosest(Main.NextPylon.transform.position, out _, out float dist);
                        dist -= Main.PylonActivationDist * 1.0f;
                        if(dist <= 0)
                            ProgressText = "Ready";
                        else
                            ProgressText = $"Distance: {(int)(dist + 0.9999f):#}";
                    }
                    break;
            }
            if(setComplete)
                ProgressText = "Complete";
            return setComplete;
        }
    }
    public static Quest SpawnBlurb(Transform parent, QuestData data)
    {
        Quest q = Instantiate(Main.PrefabAssets.QuestPrefab, parent).GetComponent<Quest>();
        q.RectTransform = q.gameObject.GetComponent<RectTransform>();
        q.transform.localPosition = new Vector3(400 - q.RectTransform.rect.width / 2, q.transform.position.y, q.transform.position.z);
        q.Data = data;
        q.UpdateText();
        return q;
    }
    public void CheckForCompletion()
    {
        if (CompletionCounter < 0 && Data.CheckForCompletion())
            SetComplete();
    }
    public QuestData Data { get; set; }
    public void UpdateText()
    {
        Text.text = Data.CompleteText;
    }
    public TextMeshProUGUI Text;
    public RectTransform RectTransform { get; set; }
    public bool Complete { get; private set; } = false;
    public float CompletionCounter = -1;
    public bool Dead { get; set; } = false;
    public void SetComplete()
    {
        if(CompletionCounter < 0)
        {
            AudioManager.PlaySound(SoundID.CoinPickup.GetVariation(1), Camera.main.transform.position, 0.8f, 0.25f); 
            CompletionCounter = 0;
        }
    }
    public void DoUpdate()
    {
        UpdateText();
        CheckForCompletion();
        float lerpFactor = Utils.DeltaTimeLerpFactor(0.0675f);
        float lerpFactor2 = Utils.DeltaTimeLerpFactor(0.1f);
        if (Complete)
        {
            Utils.LerpSnap(transform, new Vector2(400 - RectTransform.rect.width / 2, transform.localPosition.y), lerpFactor);
            if (CompletionCounter >= 1)
            {
                CompletionCounter += Time.deltaTime * 1.5f;
                if (CompletionCounter > 2)
                {
                    Dead = true;
                }
            }
        }
        else
        {
            if (CompletionCounter >= 0)
            {
                Text.color = Color.Lerp(Text.color, Color.green, lerpFactor2);
                CompletionCounter += Time.deltaTime * 1.2f;
                float sin = Mathf.Sin(CompletionCounter * Mathf.PI);
                sin *= 0.1f;
                Utils.LerpLocalScale(transform, Vector2.one * (1 + sin), lerpFactor2);
                if (CompletionCounter >= 1)
                {
                    CompletionCounter = 1;
                    Complete = true;
                }
            }
            Utils.LerpSnap(transform, new Vector2(50 - RectTransform.rect.width / 2, transform.localPosition.y), lerpFactor);
        }
    }
    public void Update()
    {
        if (Main.DebugCheats && Input.GetKeyDown(KeyCode.N))
            SetComplete();
    }
}
