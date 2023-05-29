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
        // Pause & unpause the game when escape is pressed, but only if the game is in the playing or paused states
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if(State == GameState.Playing || State == GameState.Paused)
            {
                TogglePause();
            }
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


    // this method doubles up the logic from the escape press, which is shit and pointless
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

    // most logic stemming from game states is contained in other scripts

    private void HandlePlaying()
    {
        gamePaused = false;
        Time.timeScale = 1;
    }

    private void HandlePaused()
    {
        gamePaused = true;
        Time.timeScale = 0;
    }

    private void HandleWin()
    {
        gamePaused = true;
        Time.timeScale = 0;
    }    

    private void HandleLose()
    {
        gamePaused = true;
        Time.timeScale = 0;
    }

    private void HandleQuit()
    {
        Application.Quit();
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