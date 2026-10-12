using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WorldGenLoader : MonoBehaviour
{
    [SerializeField] GameObject LoadingScreen;
    [SerializeField] Image BarImage;
    [SerializeField] GameObject Spinner;
    [SerializeField] float RotationSpeed = 250f;
    [SerializeField] TextMeshProUGUI TextComponent;
    [SerializeField] float StartValue = 0f;

    public int totalSteps = 0;
    public int currentStep = 0;

    private float progressValue;
    private float targetValue;


    [SerializeField] float lerpSpeed = 20f;

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

    private void Update()
    {
        float displayValue = Mathf.MoveTowards(targetValue, progressValue, lerpSpeed);

        if (displayValue < BarImage.fillAmount) return;
        BarImage.fillAmount = displayValue;
        TextComponent.text = (displayValue * 100).ToString("F2") + "%";
        Spinner.transform.Rotate(0, 0, -RotationSpeed * Time.deltaTime);
    }

    public void LoadFinished()
    {
        LoadingScreen.SetActive(false);
    }
}