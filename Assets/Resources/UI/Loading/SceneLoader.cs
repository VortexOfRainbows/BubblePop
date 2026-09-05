using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    private GameObject loadingScreen;
    private Image loadingBarFill;
    private TextMeshPro fillPercentage;

    private void Start()
    {
        loadingScreen = transform.Find("LoadingContainer").gameObject;
        loadingBarFill = loadingScreen.transform.Find("Bar").GetComponent<Image>();
        fillPercentage = loadingScreen.transform.Find("Bar").GetComponent<TextMeshPro>();
        Main.CanvasManager.loadingScript = this;
    }

    public void loadScene(int sceneID)
    {
        StartCoroutine(LoadSceneAsync(sceneID));
    }

    IEnumerator LoadSceneAsync(int sceneID)
    {
        print("LSA: Load scene called");
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneID);
        loadingScreen.SetActive(true);

        while (!operation.isDone)
        {
            float progressValue = Mathf.Clamp01(operation.progress / 0.9f);
            loadingBarFill.fillAmount = progressValue;
            fillPercentage.text = (progressValue * 100).ToString("F2") + "%";
            yield return null;
        }
    }
}
