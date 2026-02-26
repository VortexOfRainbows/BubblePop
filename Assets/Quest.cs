using System;
using TMPro;
using UnityEngine;

public class Quest : MonoBehaviour
{
    public class QuestData
    {
        public QuestData(string text, Func<bool> completionCheck, Func<string> updateCheck)
        {
            Text = text;
            CompletionCheck = completionCheck;
            UpdateCheck = updateCheck;
            ProgressText = UpdateCheck.Invoke();
        }
        public readonly Func<bool> CompletionCheck;
        public readonly Func<string> UpdateCheck;
        public string Text { get; private set; }
        public string ProgressText { get; set; }
        public string CompleteText => Text + " " + ProgressText;
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
    public QuestData Data { get; set; }
    public void UpdateText()
    {
        Text.text = Data.CompleteText;
    }
    public TextMeshProUGUI Text;
    public RectTransform RectTransform { get; set; }
    public bool Complete { get; private set; } = false;
    public float CompletionCounter = -1;
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
        if(CompletionCounter < 0)
        {
            Data.UpdateCheck.Invoke();
            if(Data.CompletionCheck.Invoke())
                SetComplete();
        }
        float lerpFactor = Utils.DeltaTimeLerpFactor(0.085f);
        if (Complete)
        {
            Utils.LerpSnap(transform, new Vector2(400 - RectTransform.rect.width / 2, transform.localPosition.y), lerpFactor);
            if (CompletionCounter >= 1)
            {
                CompletionCounter += Time.deltaTime;
                if (CompletionCounter > 2)
                    Destroy(gameObject);
            }
        }
        else
        {
            if (CompletionCounter >= 0)
            {
                Text.color = Color.Lerp(Text.color, Color.green, lerpFactor);
                CompletionCounter += Time.deltaTime * 1.1f;
                float sin = Mathf.Sin(CompletionCounter * Mathf.PI);
                sin *= 0.1f;
                Utils.LerpLocalScale(transform, Vector2.one * (1 + sin), lerpFactor);
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
        if (Input.GetKeyDown(KeyCode.N))
            SetComplete();
        if (Complete)
            DoUpdate();
    }
}
