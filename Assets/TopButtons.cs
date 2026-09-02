using TMPro;
using UnityEngine;

public class TopButtons : MonoBehaviour
{
    public StandardButton ShardButton;
    public StandardButton ChoiceButton;
    public TextMeshProUGUI ShardButtonText;
    public TextMeshProUGUI ChoiceButtonText;
    void Update()
    {
        // move buttons off screen if menu is open (menu must be hidden for buttons to appear)
        bool showShard = PowerUpCheatUI.ShardInstance.Hide && PowerUpCheatUI.ShardInstance.CanOpenMenu();
        bool showChoice = ChoicePowerMenu.Hide && ChoicePowerMenu.Instance.gameObject.activeSelf;

        // move positions of buttons relative to each other if both menus are accessible (and interactable)
        ShardButton.interactable = showShard;
        ChoiceButton.interactable = showChoice;
        bool shiftPositions = showShard && showChoice;

        float lerpT = Utils.DeltaTimeLerpFactor(0.125f);

        // move buttons onto screen if the player should be able to access them
        // TODO: change these Y values after the UI rework to make more sense with the new layout
        ShardButton.transform.LerpLocalPosition(new Vector2(shiftPositions ? 125 : 0, showShard ? -55 : 55), lerpT);
        ChoiceButton.transform.LerpLocalPosition(new Vector2(shiftPositions ? -125 : 0, showChoice ? -55 : 55), lerpT);

        // (maybe) make buttons have icons to show what they do (like a shard icon for the shard button, and a choice icon for the choice button)
        ShardButtonText.text = !Main.DebugCheats ? "Show Shards" : "Open Cheat Menu";
        ChoiceButtonText.text = "Show Choices";
    }
}
