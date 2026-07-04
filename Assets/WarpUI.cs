using UnityEngine;
using UnityEngine.UI;

public class WarpUI : MonoBehaviour
{
    public static WarpUI Instance { get; private set; }
    public Transform Visual;
    public Canvas MyCanvas;
    public Image ContinueButton;
    public Image FinalizeButton;
    public bool ChooseContinueButton { get; private set; } = true;
    public float HasBeenOpenForMoreThan1Second = 0f;
    public void Start()
    {
        Instance = this;
    }
    public static void Open()
    {
        Main.PauseGame();
        Instance.Visual.gameObject.SetActive(true);
        Instance.Update();
    }
    public static void Close()
    {
        Main.UnpauseGame();
        Instance.Visual.gameObject.SetActive(false);
        Instance.Update();
    }
    public static bool IsCurrentlyOpen => Instance.Visual.gameObject.activeSelf;
    public void Update()
    {
        if (!Visual.gameObject.activeSelf)
        {
            HasBeenOpenForMoreThan1Second = 0;
            return;
        }
        HasBeenOpenForMoreThan1Second += Time.unscaledDeltaTime;
        float lerpFactor = Utils.DeltaTimeLerpFactor(0.1f);
        Image SelectedButton = ChooseContinueButton ? ContinueButton : FinalizeButton;
        Image OtherButton = !ChooseContinueButton ? ContinueButton : FinalizeButton;
        if(HasBeenOpenForMoreThan1Second < 1)
        {
            SelectedButton.transform.GetChild(0).gameObject.SetActive(false);
            SelectedButton.transform.LerpLocalScale(Vector2.one * 0.95f, 1);
            SelectedButton.color = ColorHelper.UI.GreyColor;
            OtherButton.transform.GetChild(0).gameObject.SetActive(false);
            OtherButton.transform.LerpLocalScale(Vector2.one * 0.95f, 1);
            OtherButton.color = ColorHelper.UI.GreyColor;
            return;
        }
        SelectedButton.transform.GetChild(0).gameObject.SetActive(true);
        SelectedButton.transform.LerpLocalScale(Vector2.one, lerpFactor);
        SelectedButton.color = SelectedButton.color.Lerp(ColorHelper.UI.SelectColor, lerpFactor);

        OtherButton.transform.GetChild(0).gameObject.SetActive(false);
        OtherButton.transform.LerpLocalScale(Vector2.one * 0.95f, lerpFactor);
        OtherButton.color = OtherButton.color.Lerp(ColorHelper.UI.GreyColor, lerpFactor);
        
        if(Utils.IsMouseHoveringOverThis(true, OtherButton.rectTransform, 0, MyCanvas))
        {
            ChooseContinueButton = !ChooseContinueButton;
            OtherButton.transform.GetChild(0).gameObject.SetActive(true);
            SelectedButton.transform.GetChild(0).gameObject.SetActive(false);
        }
        bool input = Input.GetKeyDown(KeyCode.E);
        if (!input && Utils.IsMouseHoveringOverThis(true, SelectedButton.rectTransform, 0, MyCanvas))
            input = Input.GetMouseButtonDown(0);
        if (ChooseContinueButton)
        {
            if (input)
            {
                Close();
                Player.GameWin();
                //Reset world
                World.Instance.ResetWorld(false);
            }
            else if (Input.GetKeyDown(KeyCode.S))
                ChooseContinueButton = false;
        }
        else
        {
            if (input)
            {
                Close();
                Player.GameWin();
                //Return to Main Menu
                Main.CanvasManager.MainMenu();
            }
            else if (Input.GetKeyDown(KeyCode.W))
                ChooseContinueButton = true;
        }
    }
}
