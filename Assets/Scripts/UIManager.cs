using System;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public partial class Main : MonoBehaviour
{
    public static Canvas ActivePrimaryCanvas => UIManager.MainCanvas;
    public static CanvasManager UIManager => Instance.MyUIManager;
    public static bool StartingScreen = false;

    [SerializeField]
    private CanvasManager MyUIManager = new();
    [Serializable]
    public class CanvasManager
    {
        public List<Button> ResumeButtons;
        public List<Button> ReturnToMenuButtons;
        public List<Button> RestartButtons;
        public List<Button> UnleashWaveButton;
        public void AddListeners()
        {
            foreach(Button b in ResumeButtons)
                b.onClick.AddListener(Resume);
            foreach (Button b in ReturnToMenuButtons)
                b.onClick.AddListener(MainMenu);
            foreach (Button b in RestartButtons)
                b.onClick.AddListener(Restart);
            foreach (Button b in UnleashWaveButton)
                b.onClick.AddListener(StartGame);
        }
        public Canvas MainCanvas;
        public GameObject PauseMenu;
        public GameObject GameOverScreen;
        public TextMeshProUGUI DeadHighscoreText;
        public void Pause()
        {
            PauseMenu.SetActive(true);
            PauseGame(); 
            StaticPlaySound();
        }
        public void Resume()
        {
            PauseMenu.SetActive(false);
            UnpauseGame();
            StaticPlaySound();
        }
        public void MainMenu()
        {
            CoinManager.AfterDeathTransfer();
            UnpauseGame();
            StaticPlaySound();
            SceneManager.LoadScene("MainMenu");
        }
        public void GameOver()
        {
            GameOverScreen.SetActive(true);
            PauseGame();
        }
        public void Restart()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            UnpauseGame();
            StaticPlaySound();
        }
        public void PlaySound()
        {
            StaticPlaySound();
        }
        public static void StaticPlaySound()
        {
            Vector3 pos = Vector3.zero;
            if (Player.Instance != null)
                pos = Player.Position;
            AudioManager.PlaySound(SoundID.BubblePop, Vector3.zero, 1f, 1.0f);
        }
    }
}
