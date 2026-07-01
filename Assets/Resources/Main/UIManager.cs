using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public partial class Main : MonoBehaviour
{
    public static Canvas ActivePrimaryCanvas => UIManager.ScalingHelperCanvas;
    public static CanvasManager UIManager => Instance.MyUIManager;
    [SerializeField]
    private CanvasManager MyUIManager = new();
    [Serializable]
    public class CanvasManager
    {
        public static bool PauseUIActive()
        {
            return UIManager.PauseMenu.activeSelf;
        }
        public GameObject MPMenu1, MPMenu2, SPMenu;
        public TextMeshProUGUI PauseMenuTopText, MPControls1, MPControls2, SPControls;
        public Canvas MainCanvas, ScalingHelperCanvas;
        public GameObject PauseMenu, SettingsMenu, DebugMenu, MultiplayerMenu;
        public TextMeshProUGUI DeadHighscoreText;
        public void OpenMultiplayerMenu(bool pause)
        {
            if (pause)
                PauseGame();
            UIManager.MPMenu1.SetActive(Player.AllPlayers.Count > 1);
            UIManager.MPMenu2.SetActive(Player.AllPlayers.Count > 1);
            UIManager.SPMenu.SetActive(Player.AllPlayers.Count <= 1);
            UIManager.MultiplayerMenu.SetActive(true);
        }
        public static void CloseMultiplayerMenu()
        {
            if(!PauseUIActive() && Main.GamePaused)
                UnpauseGame();
            UIManager.MultiplayerMenu.SetActive(false);
        }
        public static void Pause()
        {
            UIManager.PauseMenu.SetActive(true);
            PersistentGameObjectLoader.Instance.Update();
            PauseGame();
            StaticPlaySound();
        }
        public static void Resume()
        {
            UIManager.PauseMenu.SetActive(false);
            PersistentGameObjectLoader.Instance.Update();
            if (UIManager.SettingsMenu.activeSelf)
                ToggleSettings();
            else if(UIManager.DebugMenu.activeSelf)
                OpenDebugMenu();
            else
                StaticPlaySound();
            UnpauseGame();
        }
        public static void MainMenu()
        {
            CoinManager.AfterDeathReset();
            UnpauseGame();
            StaticPlaySound();
            SceneManager.LoadScene("MainMenu");
        }
        public static void GameOver()
        {
            UIManager.PauseMenuTopText.text = "Game Over";

            UIManager.PauseMenu.SetActive(true);
            PauseGame();
        }
        public static void Restart()
        {
            Play(SceneManager.GetActiveScene().buildIndex);
        }
        public static void Play()
        {
            Play(1);
        }
        public static void PlayMult()
        {
            Play(2);
        }
        public static void Play(int scene)
        {
            CoinManager.AfterDeathReset();
            SceneManager.LoadScene(scene);
            UnpauseGame();
            StaticPlaySound();
        }
        public static void ToggleSettings()
        {
            if (!UIManager.SettingsMenu.activeSelf)
                UIManager.DebugMenu.SetActive(false);
            UIManager.SettingsMenu.SetActive(!UIManager.SettingsMenu.activeSelf);
            StaticPlaySound();
        }
        public void PlaySound()
        {
            StaticPlaySound();
        }
        /// <summary>
        /// TODO: Replace this with smarter behavior in the standard button system
        /// </summary>
        public static void StaticPlaySound()
        {
            //Vector3 pos = Vector3.zero;
            //if (Player.Instance != null)
            //    pos = Player.Instance1Pos;
            AudioManager.PlaySound(SoundID.BubblePop, Vector3.zero, 1f, 0.8f, 4);
        }
        public static void QuitGame()
        {
            Application.Quit();
        }
        public static void OpenDebugMenu()
        {
            if (!UIManager.DebugMenu.activeSelf)
                UIManager.SettingsMenu.SetActive(false);
            UIManager.DebugMenu.SetActive(!UIManager.DebugMenu.activeSelf);
            StaticPlaySound();
        }
    }
}
