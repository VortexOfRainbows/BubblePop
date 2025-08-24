using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WaveMeter : MonoBehaviour
{
    public Image Fill;
    public TextMeshProUGUI WaveNumber;
    //public static WaveMeter Instance;
    //public void Start()
    //{
        //Instance = this;
    //}
    //public void Update()
    //{
        //Instance = this;
    //}
    public void FixedUpdate()
    {
        WaveNumber.text = WaveDirector.WaveNum.ToString();
        Fill.fillAmount = Mathf.Lerp(Fill.fillAmount, WaveDirector.WaveProgressPercent, 0.02f);
    }
}
