using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; set; }
    public static Canvas ActivePrimaryCanvas => Instance.m_MainGameCanvas;
    [SerializeField] private Canvas m_MainGameCanvas;
    public GameObject pauseMenu;
    public GameObject gameOverScreen;
    public static bool Paused => Main.GamePaused;
    public static bool StartingScreen = false;

    public TMPro.TextMeshProUGUI deadHighscoreText;
    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        StartingScreen = true;
        Main.WavesUnleashed = false;
    }

    // Update is called once per frame
    void Update()
    {
        Instance = this;

        if (Input.GetKeyDown(KeyCode.Escape) && SceneManager.GetActiveScene().buildIndex != 0)
        {
            if(Paused)
                Resume();
            else
                Pause();
        }

        deadHighscoreText.text = $"Wave: {WaveDirector.WaveNum}";

        //if(Main.WavesUnleashed)
        //    enemyScaleText.text = $"{WaveDirector.EnemyScalingFactor:0.0}";
        //enemyScaleText.gameObject.SetActive(Main.WavesUnleashed);
    }
    public void Pause()
    {
        pauseMenu.SetActive(true);
        Main.PauseGame();
    }
    public void UnleashWaves()
    {
        StartingScreen = false;
        Main.StartGame();
    }
    public void Resume()
    {
        pauseMenu.SetActive(false);
        Main.UnpauseGame();
    }
    public void MainMenu()
    {
        CoinManager.AfterDeathTransfer();
        Main.UnpauseGame();
        SceneManager.LoadScene("MainMenu");
    }
    public void GameOver()
    {
        gameOverScreen.SetActive(true);
        Main.PauseGame();
    }
    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Main.UnpauseGame();
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
