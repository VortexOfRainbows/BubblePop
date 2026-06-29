using UnityEngine;
using UnityEngine.SceneManagement;

public class PersistentGameObjectLoader : MonoBehaviour
{
    public static GameObject CompendiumPrefab => Resources.Load<GameObject>("UI/Compendium/CompendiumCanvas");
    public static PersistentGameObjectLoader Instance { get; private set; }
    private GameObject CompendiumCanvas;
    private void Start()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadPersistentObjects();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void LoadPersistentObjects()
    {
        CompendiumCanvas = Instantiate(CompendiumPrefab, transform);
    }
    private void Update()
    {
        bool isMainMenu = SceneManager.GetActiveScene().buildIndex == 0;
        if (isMainMenu)
            return;

        if(Main.CanvasManager.PauseUIActive())
        {
            CompendiumCanvas.SetActive(true);
        }
        else
        {
            CompendiumCanvas.SetActive(false);
        }
    }
}
