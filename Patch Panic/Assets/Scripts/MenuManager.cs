using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenu, failMenu, winMenu;
    [SerializeField] private TMP_Text finalScore, winningScore;

    TimeSpan uptimeScore;

    private void Awake()
    {
        GameManager.OnGameStateChanged += GameManagerOnGameStateChanged;
    }

    private void OnDestroy()
    {
        GameManager.OnGameStateChanged -= GameManagerOnGameStateChanged;
    }

    public void OnFeedbackLinkClick()
    {
        Application.OpenURL("https://forms.gle/NqLYSaumXePAK4jS9");
    }

    private void GameManagerOnGameStateChanged(GameState state)
    {
        pauseMenu.SetActive(state == GameState.Paused);
        failMenu.SetActive(state == GameState.Lose);
        winMenu.SetActive(state == GameState.Win);

        if(state == GameState.Lose)
        {
            finalScore.text = $"You scored {LevelManager.Instance.playerScore}";
            SetHighScore();
        }

        if(state == GameState.Win)
        {
            // Generate minutes and seconds values from the cumulative uptime score in the levelmanager
            uptimeScore = TimeSpan.FromSeconds(LevelManager.Instance.cumulativeUptime);
            int upMins = uptimeScore.Minutes;
            int upSecs = uptimeScore.Seconds;

            winningScore.text = $"You scored {LevelManager.Instance.playerScore} with a total uptime of {upMins}:{upSecs}!";
            SetHighScore();
        }
    }

    private void SetHighScore()
    {
        int score = LevelManager.Instance.playerScore;

        if(score >= PlayerPrefs.GetInt("highScore", 0))
        {
            PlayerPrefs.SetInt("highScore", score);
            PlayerPrefs.Save();
        }
    }
}
