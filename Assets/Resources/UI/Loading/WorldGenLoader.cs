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

    public void WorldLoader(int maxSteps)
    {
        totalSteps = maxSteps;
        LoadingScreen.SetActive(true);
    }

    public void NextStep()
    {
        float progressValue = Mathf.Clamp01((float)currentStep / totalSteps);
        BarImage.fillAmount = progressValue * (1 - StartValue) + StartValue;
        TextComponent.text = (progressValue * 100).ToString("F2") + "%";

        if (currentStep >= totalSteps)
        {
            LoadingScreen.SetActive(false);
        }
    }
}