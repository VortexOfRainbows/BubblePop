using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WorldGenLoader : MonoBehaviour
{
    [SerializeField] GameObject LoadingScreen;
    [SerializeField] Image BarImage;
    [SerializeField] TextMeshProUGUI TextComponent;

    private int totalSteps = 0;
    private int currentStep = 0;

    public void WorldLoader(int maxSteps)
    {
        totalSteps = maxSteps;
        LoadingScreen.SetActive(true);
    }

    public void NextStep()
    {
        currentStep++;
        float progressValue = Mathf.Clamp01((float)currentStep / totalSteps);

        BarImage.fillAmount = progressValue;
        TextComponent.text = (progressValue * 100).ToString("F2") + "%";

        if (progressValue >= 1f)
        {
            LoadingScreen.SetActive(false);
        }
    }
}