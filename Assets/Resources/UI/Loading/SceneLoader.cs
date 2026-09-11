using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] GameObject LoadingScreen;
    [SerializeField] Image BarImage;
    [SerializeField] TextMeshProUGUI TextComponent;

    private void Start()
    {
        Main.CanvasManager.loadingScript = this;
    }

    public void loadScene(int sceneID)
    {
        StartCoroutine(LoadSceneAsync(sceneID));
    }

    IEnumerator LoadSceneAsync(int sceneID)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneID);
        LoadingScreen.SetActive(true);

        while (!operation.isDone)
        {
            float progressValue = Mathf.Clamp01(operation.progress / 0.9f);
            BarImage.fillAmount = progressValue;
            TextComponent.text = (progressValue * 100).ToString("F2") + "%";
            yield return null;
        }
    }
}
