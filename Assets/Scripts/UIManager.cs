using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    public Canvas MainGameCanvas;
    public GameObject pauseMenu;
    public GameObject gameOverScreen;
    public static bool Paused => Main.GamePaused;
    public static bool StartingScreen = false;

    public static int highscore;
    public static int score;

    public TMPro.TextMeshProUGUI scoreText;
    public TMPro.TextMeshProUGUI highscoreText;
    public TMPro.TextMeshProUGUI deadHighscoreText;
    public TMPro.TextMeshProUGUI moneyText;
    public TMPro.TextMeshProUGUI waveText;
    public GameObject CurrencyIcon;
    public GameObject SavingsIcon;

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        score = 0;
        highscore = PlayerData.GetInt("Highscore");
        StartingScreen = true;
        Main.WavesUnleashed = false;
    }

    // Update is called once per frame
    void Update()
    {
        Instance = this;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if(Paused)
                Resume();
            else
                Pause();
        }

        scoreText.text = "Score: " + Mathf.FloorToInt(score);
        highscoreText.text = "Highscore: " + Mathf.FloorToInt(highscore);
        waveText.text = "Wave: " + Mathf.FloorToInt(WaveDirector.WaveNum);
        deadHighscoreText.text = highscoreText.text;

        int money = CoinManager.Current; // : CoinManager.Savings;
        moneyText.text = $"${money}";
        moneyText.enabled = true;
        CurrencyIcon.SetActive(false);
        SavingsIcon.SetActive(true);

        if (score > highscore)
        {
            highscore = score;
            PlayerData.SaveInt("Highscore", (int)highscore);
        }
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
        Main.GamePaused = false;
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
    }
    public void MainMenu()
    {
        CoinManager.AfterDeathTransfer();
        Time.timeScale = 1f;
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
        Time.timeScale = 1f;
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
