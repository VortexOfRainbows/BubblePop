using UnityEngine;

public class SettingsUI : MonoBehaviour
{
    public void Enable()
    {
        gameObject.SetActive(!gameObject.activeSelf);
    }
}
