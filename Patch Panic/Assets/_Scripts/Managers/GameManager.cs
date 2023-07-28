using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameState State;

    public bool gamePaused;

    public static event Action<GameState> OnGameStateChanged;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        UpdateGameState(GameState.Playing);
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    public void UpdateGameState(GameState newState)
    {
        State = newState;

        switch (newState)
        {
            case GameState.Playing:
                HandlePlaying();
                break;
            case GameState.Paused:
                HandlePaused();
                break;
            case GameState.Win:
                HandleWin();
                break;
            case GameState.Lose:
                HandleLose();
                break;
            case GameState.Quit:
                HandleQuit();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
        }

        OnGameStateChanged?.Invoke(newState);
    }


    // Pauses & unpauses the game & music when called.
    private void TogglePause()
    {
        if(State == GameState.Playing)
        {
            UpdateGameState(GameState.Paused);
            return;
        }
        if(State == GameState.Paused)
        {
            UpdateGameState(GameState.Playing);
            return;
        }
    }

    // Most logic leveraging game states exists in other scripts.

    private void HandlePlaying()
    {
        GamePauser(false);
    }

    private void HandlePaused()
    {
        GamePauser(true);
    }

    private void HandleWin()
    {
        GamePauser(true);
    }    

    private void HandleLose()
    {
        GamePauser(true);
    }

    private void HandleQuit()
    {
        Application.Quit();
    }

    private void GamePauser(bool shouldPause)
    {
        if(shouldPause)
        {
            gamePaused = true;
            Time.timeScale = 0;
            SoundManager.Instance.MusicPauser(true);
        }
        if(!shouldPause)
        {
            gamePaused = false;
            Time.timeScale = 1;
            SoundManager.Instance.MusicPauser(false);
        }
    }

}

public enum GameState
{
    Playing,
    Paused,
    Win,
    Lose,
    Quit
}