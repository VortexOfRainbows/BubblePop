using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Canvas MainGameCanvas;
    public static UIManager Instance;
    [SerializeField]
    private GameObject pauseMenu;
    [SerializeField]
    private GameObject gameOverScreen;
    public static bool Paused => Main.GamePaused;
    public static bool StartingScreen = false;

    public static int highscore;
    public static int score;

    [SerializeField]
    private GameObject pauseButton;
    [SerializeField]
    private TMPro.TextMeshProUGUI scoreText;
    [SerializeField]
    private TMPro.TextMeshProUGUI highscoreText;
    [SerializeField]
    private TMPro.TextMeshProUGUI deadHighscoreText;
    [SerializeField]
    private TMPro.TextMeshProUGUI moneyText;
    [SerializeField]
    private TMPro.TextMeshProUGUI savingsText;

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        score = 0;
        highscore = PlayerData.GetInt("Highscore");
        pauseButton.SetActive(false);
        StartingScreen = true;
        Main.GameStarted = false;
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
        deadHighscoreText.text = highscoreText.text;
        moneyText.text = $"Gold: {CoinManager.Current}";
        savingsText.text = $"Savings: {CoinManager.Savings}";
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
        pauseButton.SetActive(true);
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
