using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuButtons : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene(1);
    }
    public void MenuSound()
    {
        Main.CanvasManager.StaticPlaySound();
    }
}
