using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WorldGenLoader : MonoBehaviour
{
    [SerializeField] GameObject LoadingScreen;
    [SerializeField] Image BarImage;
    [SerializeField] TextMeshProUGUI TextComponent;
    [SerializeField] float StartValue = 0f;

    public int totalSteps = 0;
    public int currentStep = 0;

    private float progressValue;
    private float targetValue;


    [SerializeField] float lerpSpeed = 0.5f;

    public void WorldLoader(int maxSteps)
    {
        totalSteps = maxSteps;
        LoadingScreen.SetActive(true);
    }

    public void NextStep()
    {
        progressValue = Mathf.Clamp01((float)currentStep / totalSteps);
        targetValue = progressValue * (1 - StartValue) + StartValue;

        if (currentStep >= totalSteps)
        {
            //LoadFinished();
        }
    }

    void Update()
    {
        float displayValue = Mathf.MoveTowards(targetValue, progressValue, lerpSpeed * Time.deltaTime);
        BarImage.fillAmount = displayValue;
        TextComponent.text = (displayValue * 100).ToString("F2") + "%";
    }

    public void LoadFinished()
    {
        LoadingScreen.SetActive(false);
    }
}