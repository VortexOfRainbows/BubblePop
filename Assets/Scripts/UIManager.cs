using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private GameObject tutorial;
    [SerializeField]
    private GameObject pauseMenu;
    [SerializeField]
    private GameObject gameOverScreen;
    public static bool gamePaused = false;
    private static bool tutorialSeen = false;

    public static int highscore;
    public static int score;

    [SerializeField]
    private TMPro.TextMeshProUGUI scoreText;
    [SerializeField]
    private TMPro.TextMeshProUGUI highscoreText;
    [SerializeField]
    private TMPro.TextMeshProUGUI deadHighscoreText;
    [SerializeField]
    private TMPro.TextMeshProUGUI PowerUpOne;
    [SerializeField]
    private TMPro.TextMeshProUGUI PowerUpTwo;


    // Start is called before the first frame update
    void Start()
    {
        score = 0;
        highscore = PlayerPrefs.GetInt("Highscore", 0);

        if(tutorialSeen == false)
        {
            tutorial.SetActive(true);
            Time.timeScale = 0f;
        }
    }

    // Update is called once per frame
    void Update()
    {

        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if(gamePaused)
                Resume();
            else
                Pause();
        }

        if(Input.GetKeyDown(KeyCode.Backspace))
        {
            GameOver();
        }
        ///if player == dead
        ///{
        ///GameOver();
        ///}

        PowerUpTwo.text = Player.DamagePower.ToString();
        PowerUpOne.text = Player.ShotgunPower.ToString();
        scoreText.text = " Soapy Score: " + Mathf.FloorToInt(score);
        highscoreText.text = "Bubble Best: " + Mathf.FloorToInt(highscore);
        deadHighscoreText.text = "Bubble Best: " + Mathf.FloorToInt(highscore);
        if (score > highscore)
        {
            highscore = score;
            PlayerPrefs.SetInt("Highscore", (int)highscore);
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
        tutorialSeen = true;
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
        SceneManager.LoadScene("SampleScene");
        Time.timeScale = 1f;
    }
}
