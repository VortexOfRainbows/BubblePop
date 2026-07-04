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
        LoadCompendium();
    }
    private void LoadCompendium()
    {
        CompendiumCanvas = Instantiate(CompendiumPrefab, transform);
        Compendium c = CompendiumCanvas.transform.GetChild(0).GetComponent<Compendium>();
        Compendium.ScreenResolution = new Vector2(c.MyCanvasRectTransform.rect.width, c.MyCanvasRectTransform.rect.height); //1920, 1080 in most cases
        Compendium.HalfResolution = Compendium.ScreenResolution / 2f;
        c.MoveCompendiumUpdate(1.0f);
    }
    public void Update()
    {
        bool isMainMenu = Main.SceneMainMenu;
        if (isMainMenu)
        {
            CompendiumCanvas.SetActive(true);
            return;
        }
        if (Main.CanvasManager.PauseUIActive())
        {
            CompendiumCanvas.SetActive(true);
            if (CharacterSelect.Instance != null)
                CharacterSelect.Instance.gameObject.SetActive(false);
        }
        else
        {
            CompendiumCanvas.SetActive(false);
            if (CharacterSelect.Instance != null)
                CharacterSelect.Instance.gameObject.SetActive(true);
            if(Compendium.CurrentlySelectedPage.TierListActive)
                Compendium.CurrentlySelectedPage.ToggleTierList(Compendium.Instance.TierListText);
            Compendium.Instance.MoveCompendiumUpdate(1.0f);
        }
    }
}
