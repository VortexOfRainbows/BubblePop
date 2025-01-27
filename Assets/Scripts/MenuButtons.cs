using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuButtons : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("SampleScene");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void MenuSound()
    {
        AudioManager.PlaySound(GlobalDefinitions.audioClips[Random.Range(0, 8)], Vector3.zero, 1f, 1.0f);
    }
}
