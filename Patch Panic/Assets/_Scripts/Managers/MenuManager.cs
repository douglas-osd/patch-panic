using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance;

    [SerializeField] private GameObject pauseMenu, failMenu, winMenu;
    [SerializeField] private TMP_Text finalScore, winningScore, failureCause;
    [SerializeField] private AudioClip buttonAudio;

    TimeSpan uptimeScore;

    private void Awake()
    {
        Instance = this;
        GameManager.OnGameStateChanged += GameManagerOnGameStateChanged;
    }

    private void OnDestroy()
    {
        GameManager.OnGameStateChanged -= GameManagerOnGameStateChanged;
    }

    private void GameManagerOnGameStateChanged(GameState state)
    {
        pauseMenu.SetActive(state == GameState.Paused);
        failMenu.SetActive(state == GameState.Lose);
        winMenu.SetActive(state == GameState.Win);

        if(state == GameState.Lose)
        {
            finalScore.text = $"You scored {LevelManager.Instance._player.playerScore}";
            failureCause.text = LevelManager.Instance.failCause;
            SetHighScore();
        }

        if(state == GameState.Win)
        {
            // Generate minutes and seconds values from the cumulative uptime score in the levelmanager
            uptimeScore = TimeSpan.FromSeconds(LevelManager.Instance.cumulativeUptime);
            int upMins = uptimeScore.Minutes;
            int upSecs = uptimeScore.Seconds;

            winningScore.text = $"You scored {LevelManager.Instance._player.playerScore} with a total uptime of {upMins}:{upSecs}!";
            SetHighScore();
        }
    }

    private void SetHighScore()
    {
        int score = LevelManager.Instance._player.playerScore;

        if(score >= PlayerPrefs.GetInt("highScore", 0))
        {
            PlayerPrefs.SetInt("highScore", score);
            PlayerPrefs.Save();
        }
    }

    public void QuitButtonPress()
    {
        SoundManager.Instance.PlaySound(buttonAudio);
        GameManager.Instance.UpdateGameState(GameState.Quit);
    }

    public void ResumeButtonPress()
    {
        SoundManager.Instance.PlaySound(buttonAudio);
        GameManager.Instance.UpdateGameState(GameState.Playing);
    }

    public void MenuButtonPress()
    {
        SoundManager.Instance.PlaySound(buttonAudio);
        SceneManager.LoadScene(0);
    }

    public void FeedbackButtonPress()
    {
        SoundManager.Instance.PlaySound(buttonAudio);
        Application.OpenURL("https://forms.gle/NqLYSaumXePAK4jS9");
    }
}
