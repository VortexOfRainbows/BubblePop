using UnityEngine;
using UnityEngine.UI;

public class WarpUI : MonoBehaviour
{
    public static WarpUI Instance { get; private set; }
    public Transform Visual;
    public Canvas MyCanvas;
    public StandardButton ContinueButton, FinalizeButton;
    public static readonly Color DefaultColor = ColorHelper.Cornflower.WithAlpha(0.9f);
    public static readonly Color GreyColor = (ColorHelper.Cornflower * 0.5f).WithAlpha(0.9f);
    public bool ChooseContinueButton { get; private set; } = true;
    public float HasBeenOpenForMoreThan1Second = 0f;
    public void Start()
    {
        Instance = this;
        ContinueButton.onClick.AddListener(OnContinue);
        FinalizeButton.onClick.AddListener(OnEnd);
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
        StandardButton SelectedButton = ChooseContinueButton ? ContinueButton : FinalizeButton;
        StandardButton OtherButton = !ChooseContinueButton ? ContinueButton : FinalizeButton;
        if(HasBeenOpenForMoreThan1Second < 1)
        {
            SelectedButton.transform.GetChild(0).gameObject.SetActive(false);
            SelectedButton.transform.LerpLocalScale(Vector2.one * 0.95f, 1);
            SelectedButton.targetGraphic.color = GreyColor;
            OtherButton.transform.GetChild(0).gameObject.SetActive(false);
            OtherButton.transform.LerpLocalScale(Vector2.one * 0.95f, 1);
            OtherButton.targetGraphic.color = GreyColor;
            ContinueButton.interactable = FinalizeButton.interactable = false;
            return;
        }
        ContinueButton.interactable = FinalizeButton.interactable = true;
        SelectedButton.transform.GetChild(0).gameObject.SetActive(true);
        SelectedButton.transform.LerpLocalScale(Vector2.one, lerpFactor);
        SelectedButton.targetGraphic.color = SelectedButton.targetGraphic.color.Lerp(ColorHelper.UI.Yellow, lerpFactor);

        OtherButton.transform.GetChild(0).gameObject.SetActive(false);
        OtherButton.transform.LerpLocalScale(Vector2.one * 0.95f, lerpFactor);
        OtherButton.targetGraphic.color = OtherButton.targetGraphic.color.Lerp(DefaultColor, lerpFactor);
        
        if(Utils.IsMouseHoveringOverThis(true, OtherButton.targetGraphic.rectTransform, 0, MyCanvas))
        {
            ChooseContinueButton = !ChooseContinueButton;
            OtherButton.transform.GetChild(0).gameObject.SetActive(true);
            SelectedButton.transform.GetChild(0).gameObject.SetActive(false);
        }
        bool input = Input.GetKeyDown(KeyCode.E);
        //if (!input && Utils.IsMouseHoveringOverThis(true, SelectedButton.targetGraphic.rectTransform, 0, MyCanvas))
        //    input = Input.GetMouseButtonDown(0); // Input.GetMouseButtonUp(0);
        if (ChooseContinueButton)
        {
            if (input)
                OnContinue();
            else if (Input.GetKeyDown(KeyCode.S))
                ChooseContinueButton = false;
        }
        else
        {
            if (input)
                OnEnd();
            else if (Input.GetKeyDown(KeyCode.W))
                ChooseContinueButton = true;
        }
    }
    public void OnContinue()
    {
        Close();
        Player.GameWin();
        //Reset world
        World.Instance.ResetWorld(false);
    }
    public void OnEnd()
    {
        Close();
        Player.GameWin();
        //Return to Main Menu
        Main.CanvasManager.MainMenu();
    }
}
