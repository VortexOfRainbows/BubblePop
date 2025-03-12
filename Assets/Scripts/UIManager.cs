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
    private GameObject tutorial;
    [SerializeField]
    private GameObject pauseMenu;
    [SerializeField]
    private GameObject gameOverScreen;
    public static bool gamePaused = false;
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

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        score = 0;
        highscore = PlayerData.GetInt("Highscore");
        tutorial.SetActive(true);
        pauseButton.SetActive(false);
        StartingScreen = true;
        Time.timeScale = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        Instance = this;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if(gamePaused)
                Resume();
            else
                Pause();
        }

        //DamagePowerUp.text = Player.Instance.DamagePower.ToString();
        //ShotgunPowerUp.text = Player.Instance.ShotgunPower.ToString();
        scoreText.text = "Score: " + Mathf.FloorToInt(score);
        highscoreText.text = "Highscore: " + Mathf.FloorToInt(highscore);
        deadHighscoreText.text = highscoreText.text;
        if (score > highscore)
        {
            highscore = score;
            PlayerData.SaveInt("Highscore", (int)highscore);
        }

    }

    public void Pause()
    {
        gamePaused = true;
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
    }

    public void CloseTutorial()
    {
        tutorial.SetActive(false);
        Time.timeScale = 1f;
        StartingScreen = false;
        pauseButton.SetActive(true);
    }

    public void Resume()
    {
        gamePaused = false;
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
    }

    public void MainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    public void GameOver()
    {
        gameOverScreen.SetActive(true);
        Time.timeScale = 0f;
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
