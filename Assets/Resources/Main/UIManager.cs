using System;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public partial class Main : MonoBehaviour
{
    public static Canvas ActivePrimaryCanvas => UIManager.MainCanvas;
    public static CanvasManager UIManager => Instance.MyUIManager;
    [SerializeField]
    private CanvasManager MyUIManager = new();
    [Serializable]
    public class CanvasManager
    {
        public List<Button> PlayButtons = new();
        public List<Button> MultiplayerButtons = new();
        public List<Button> ResumeButtons = new();
        public List<Button> ReturnToMenuButtons = new();
        public List<Button> RestartButtons = new();
        public List<Button> UnleashWaveButton = new();
        public List<Button> SettingsButton = new();
        public List<Button> QuitButtons = new();
        public List<Button> DebugButtons = new();
        public TextMeshProUGUI PauseMenuTopText, MPControls1, MPControls2;
        public void AddListeners()
        {
            foreach (Button b in PlayButtons)
                b.onClick.AddListener(Play);
            foreach (Button b in MultiplayerButtons)
                b.onClick.AddListener(Play);
            foreach (Button b in ResumeButtons)
                b.onClick.AddListener(Resume);
            foreach (Button b in ReturnToMenuButtons)
                b.onClick.AddListener(MainMenu);
            foreach (Button b in RestartButtons)
                b.onClick.AddListener(Restart);
            foreach (Button b in UnleashWaveButton)
                b.onClick.AddListener(StartGame);
            foreach (Button b in SettingsButton)
                b.onClick.AddListener(ToggleSettings);
            foreach (Button b in QuitButtons)
                b.onClick.AddListener(QuitGame);
            foreach (Button b in DebugButtons)
                b.onClick.AddListener(OpenDebugMenu);
        }
        public Canvas MainCanvas;
        public GameObject PauseMenu, SettingsMenu, DebugMenu, MultiplayerMenu;
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
            if (SettingsMenu.activeSelf)
                ToggleSettings();
            else if(DebugMenu.activeSelf)
                OpenDebugMenu();
            else
                StaticPlaySound();
            UnpauseGame();
        }
        public void MainMenu()
        {
            CoinManager.AfterDeathReset();
            UnpauseGame();
            StaticPlaySound();
            SceneManager.LoadScene("MainMenu");
        }
        public void GameOver()
        {
            Button resumeButton = null;
            foreach (Button b in SettingsButton)
                b.gameObject.SetActive(false);
            foreach (Button b in ResumeButtons)
            {
                b.gameObject.SetActive(false);
                resumeButton = b;
            }
            foreach (Button b in RestartButtons)
            {
                if(resumeButton != null)
                {
                    b.GetComponent<RectTransform>().sizeDelta = resumeButton.GetComponent<RectTransform>().sizeDelta;
                    b.GetComponent<RectTransform>().pivot = resumeButton.GetComponent<RectTransform>().pivot;
                    b.transform.localPosition = resumeButton.transform.localPosition;
                }
                b.GetComponentInChildren<TextMeshProUGUI>().text = "Try Again";
            }
            PauseMenuTopText.text = "Game Over";

            PauseMenu.SetActive(true);
            PauseGame();
        }
        public void Restart()
        {
            Play(SceneManager.GetActiveScene().buildIndex);
        }
        public void Play()
        {
            Play(1);
        }
        public void Play(int scene)
        {
            CoinManager.AfterDeathReset();
            SceneManager.LoadScene(scene);
            UnpauseGame();
            StaticPlaySound();
        }
        public void ToggleSettings()
        {
            if (!SettingsMenu.activeSelf)
                DebugMenu.SetActive(false);
            SettingsMenu.SetActive(!SettingsMenu.activeSelf);
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
        public void QuitGame()
        {
            Application.Quit();
        }
        public void EnableDebugButtons()
        {
            if(Main.DebugCheats)
            {
                foreach(Button b in DebugButtons)
                {
                    b.interactable = true;
                    b.GetComponentInChildren<TextMeshProUGUI>().color = Color.white;
                }
            }
        }
        public void OpenDebugMenu()
        {
            if (!DebugMenu.activeSelf)
                SettingsMenu.SetActive(false);
            DebugMenu.SetActive(!DebugMenu.activeSelf);
            StaticPlaySound();
        }
    }
}
