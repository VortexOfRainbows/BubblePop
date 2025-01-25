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

    public float highscore;
    public float score;

    [SerializeField]
    private TMPro.TextMeshProUGUI scoreText;
    [SerializeField]
    private TMPro.TextMeshProUGUI highscoreText;
    [SerializeField]
    private TMPro.TextMeshProUGUI deadHighscoreText;


    // Start is called before the first frame update
    void Start()
    {

        score = 0f;
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
        if(tutorial.activeSelf)
        {
            if(Input.GetKeyDown(KeyCode.Mouse0))
            {
                tutorial.SetActive(false);
                Time.timeScale = 1f;
                tutorialSeen = true;
            }
        }

        if(Input.GetKeyDown(KeyCode.Escape))
        {
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

        scoreText.text = "Score: " + Mathf.FloorToInt(score);
        highscoreText.text = "Highscore: " + Mathf.FloorToInt(highscore);
        deadHighscoreText.text = "Highscore: " + Mathf.FloorToInt(highscore);
        score += 10f * Time.deltaTime;
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
