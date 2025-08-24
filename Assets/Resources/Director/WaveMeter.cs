using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WaveMeter : MonoBehaviour
{
    public float FillAmt { get; set; } = 1;
    public float StartTimer { get; set; } = 0;
    public float AnimationTimer { get; set; } = 0;
    public RectTransform Meter;
    public TextMeshProUGUI WaveNumber;
    public void Update()
    {
        AnimationTimer += Time.deltaTime;
    }
    public void Start()
    {
        Meter.sizeDelta = new Vector2(25, Meter.sizeDelta.y);
        AnimationTimer = StartTimer = 0;
        transform.localPosition = new Vector2(transform.localPosition.x, 150);
    }
    public void FixedUpdate()
    {
        float targetPosition = -50 + Mathf.Sin(AnimationTimer * Mathf.Deg2Rad * 40) * 5;
        Utils.LerpSnap(transform, new Vector2(transform.localPosition.x, Main.WavesUnleashed ? targetPosition : 150), 0.05f, 0.1f);
        if(Main.WavesUnleashed && WaveDirector.WaveActive)
        {
            if (StartTimer > 0.5f)
            {
                WaveNumber.text = WaveDirector.WaveNum.ToString();
                FillAmt = Mathf.Lerp(FillAmt, WaveDirector.WaveProgressPercent, 0.025f);
                float iFill = 1 - FillAmt;
                float defaultSize = 400;
                Meter.sizeDelta = new Vector2(defaultSize * iFill + 25, Meter.sizeDelta.y);
            }
            else
                StartTimer += Time.fixedDeltaTime;
        }
        else
        {
            StartTimer = 0;
        }
    }
}
