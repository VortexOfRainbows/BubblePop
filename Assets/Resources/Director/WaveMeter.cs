using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WaveMeter : MonoBehaviour
{
    public static WaveMeter Instance;
    public float FillAmt { get; set; } = 1;
    public float StartTimer { get; set; } = 0;
    public float AnimationTimer { get; set; } = 0;
    public RectTransform NonMeterStats;
    public TextMeshProUGUI HighscoreWaveText;
    public RectTransform Meter;
    public TextMeshProUGUI WaveNumber;
    public Transform DeckPosition;
    public Transform Mask;

    public GameObject SkullTick => Resources.Load<GameObject>("Director/SkullTick");
    public void Update()
    {
        Instance = this;
        AnimationTimer += Time.deltaTime;
        AnimationUpdate();
    }
    public void Start()
    {
        Instance = this;
        Meter.sizeDelta = new Vector2(20, Meter.sizeDelta.y);
        AnimationTimer = StartTimer = 0;
        transform.localPosition = new Vector2(transform.localPosition.x, 150);
    }
    public void AnimationUpdate()
    {
        float targetPosition = -50 + Mathf.Sin(AnimationTimer * Mathf.Deg2Rad * 40) * 5;
        float defaultPosition = 150;
        Utils.LerpSnap(transform, new Vector2(transform.localPosition.x, Main.WavesUnleashed ? targetPosition : defaultPosition), Utils.DeltaTimeLerpFactor(0.02f), 0.1f);
        Utils.LerpSnap(NonMeterStats.transform, new Vector2(NonMeterStats.transform.localPosition.x, Main.WavesUnleashed ? defaultPosition : targetPosition), Utils.DeltaTimeLerpFactor(0.02f), 0.1f);
        if (Main.WavesUnleashed && WaveDirector.WaveActive)
        {
            if (StartTimer > 0.5f)
            {
                WaveNumber.text = WaveDirector.WaveNum.ToString();
                FillAmt = Mathf.Lerp(FillAmt, WaveDirector.WaveProgressPercent, Utils.DeltaTimeLerpFactor(0.025f));
                float iFill = 1 - FillAmt;
                float defaultSize = 400;
                Meter.sizeDelta = new Vector2(defaultSize * iFill + 20, Meter.sizeDelta.y);
            }
            else
                StartTimer += Time.deltaTime;
        }
        else
        {
            HighscoreWaveText.text = $"Highscore: {WaveDirector.HighScoreWaveNum}";
            StartTimer = 0;
        }
        UpdateSkullsRemaining();
        UpdateNextWaveButton();
        UpdateTicks();
    }
    public RectTransform NextWaveButton;
    public Image NextWaveBG, Pylon;
    public RectTransform SkullEnemyRemaining;
    public TextMeshProUGUI SkullEnemyRemainingText;
    public bool SkullEnemiesActive { get; set; } = false;
    public void UpdateSkullsRemaining()
    {
        SkullEnemiesActive = WaveDirector.SkullEnemiesActive > 0;
        float defaultPosition = 50;
        Utils.LerpSnap(SkullEnemyRemaining, new Vector2(SkullEnemyRemaining.localPosition.x, SkullEnemiesActive ? (Mathf.Sin(AnimationTimer * Mathf.Deg2Rad * 60) * 4 -80) : defaultPosition), Utils.DeltaTimeLerpFactor(0.05f), 0.1f);
        SkullEnemyRemainingText.text = WaveDirector.SkullEnemiesActive.ToString();
    }
    public void UpdateNextWaveButton()
    {
        bool AwaitingNextCard = !WaveDirector.WaveActive && WaveDirector.WaitingForCard;
        float defaultPosition = 200;
        Utils.LerpSnap(NextWaveButton, new Vector2(NextWaveButton.localPosition.x, AwaitingNextCard ? -50 : defaultPosition), Utils.DeltaTimeLerpFactor(0.05f), 0.1f);
        Color targetColor = !Main.PlayerNearPylon ? new Color(0.9f, 0.5f, 0.5f, 1f) : Color.white;
        if (Main.PlayerNearPylon)
        {
            if(AwaitingNextCard)
            {
                bool press = Control.Interact;
                if(Utils.IsMouseHoveringOverThis(true, NextWaveBG.rectTransform, 0, CardManager.Instance.MyCanvas))
                {
                    if (Control.LeftMouseClick)
                        press = true;
                    targetColor = Color.yellow;
                }
                if(press)
                {
                    CardManager.DrawCards();
                    WaveDirector.WaitingForCard = false;
                }
            }
        }
        if (!AwaitingNextCard || Main.PlayerNearPylon)
            Pylon.color = Color.Lerp(Pylon.color, Color.white, Utils.DeltaTimeLerpFactor(0.2f));
        else
            Pylon.color = Color.Lerp(Pylon.color, new Color(0.75f, 0.75f, 0.75f, 0.75f), Utils.DeltaTimeLerpFactor(0.2f));
        NextWaveBG.color = Color.Lerp(NextWaveBG.color, targetColor, Utils.DeltaTimeLerpFactor(0.2f));
    }

    public List<SkullTick> SkullTicks = new();
    public void SetTicks(int amt = 0)
    {
        foreach(SkullTick g in SkullTicks)
        {
            if (g != null)
                Destroy(g.gameObject);
        }
        SkullTicks.Clear();
        for (int i = 0; i < amt; ++i)
        {
            float interval = (i + 1f) / (amt + 1f);
            var tick = Instantiate(SkullTick, Mask).GetComponent<SkullTick>();
            tick.transform.localPosition = new Vector3(-400 * interval, 25);
            tick.MyPercent = interval;
            SkullTicks.Add(tick);
        }
    }
    public void UpdateTicks()
    {
        for(int i = 0; i < SkullTicks.Count; ++i)
        {
            SkullTick tick = SkullTicks[i];
            tick.UpdateSkull(1 - FillAmt);
        }
    }
}
