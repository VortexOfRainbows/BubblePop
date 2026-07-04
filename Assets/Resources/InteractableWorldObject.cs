using UnityEngine;

public class InteractableWorldObject : MonoBehaviour
{
    public Transform PopupUI;
    public virtual void EnableUI(bool fixedTime = false)
    {
        PopupUI.GetChild(0).gameObject.SetActive(true);
        PopupUI.GetChild(0).LerpLocalScale(Vector2.one, fixedTime ? 0.1f : Utils.DeltaTimeLerpFactor(0.1f));
    }
    public virtual void DisableUI(bool fixedTime = false)
    {
        PopupUI.GetChild(0).gameObject.SetActive(false);
        PopupUI.GetChild(0).LerpLocalScale(Vector2.one * 0.8f, fixedTime ? 0.5f : Utils.DeltaTimeLerpFactor(0.5f));
    }
}
