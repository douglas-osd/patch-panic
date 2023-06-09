using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LevelManager : MonoBehaviour
{

    public static LevelManager Instance;

    [Header("UI Elements")]
    [SerializeField] private TMP_Text playerScoreText;
    [SerializeField] private TMP_Text securityHealthText;
    [SerializeField] private TMP_Text levelTimerText;
    [SerializeField] private GameObject failTimerUI;
    [SerializeField] private TMP_Text failTimerText;

    [Header("CurrentScores")]
    public int userHealth;
    public int securityHealth;
    public int playerScore;
    public int cumulativeUptime;

    [Header("Settings")]
    public int startingSecurityHealth;
    public float globalDamageInterval;
    public int scorePerDifficulty;
    public int completedUpdateBonus; // base bonus score upon each installed update
    public int fullyUpdatedBonus; // base bonus scope upon fully updating the server
    [SerializeField] private float downTimerStart = 30.0f, winTimer = 120.0f;
    [SerializeField] private AudioClip levelMusic;

    private float downTimer;

    private GameObject[] servers;
    private TimeSpan levelTimespan;
    private TimeSpan failTimespan;
    private bool allServersDown;

    void Awake()
    {
        Instance = this;
    }

    private void OnDestroy()
    {
        
    }


    void Start()
    {
        playerScore = 0;
        downTimer = downTimerStart;
        securityHealth = startingSecurityHealth;
        SoundManager.Instance.PlayMusic(levelMusic);
        servers = GameObject.FindGameObjectsWithTag("Server");
    }

    void Update()
    {
        LevelUITextUpdate();

        if(winTimer <= 0)
        {
            GameManager.Instance.UpdateGameState(GameState.Win);
        }

        // sets state to lose if lose conditions are met and the player has not already won
        if(securityHealth <= 0 || downTimer <= 0)
        {
            if(GameManager.Instance.State != GameState.Win)
            {
                GameManager.Instance.UpdateGameState(GameState.Lose);
            }
        }

        // messy logic that checks each server for their state to see if any are currently up - loop breaks on finding an active server
        // if no active servers are found, it sets allServersDown to false, allowing the fail timer to start ticking
        foreach (GameObject go in servers)
        {
            if(go.GetComponent<ServerBehaviour>().serverIsDown == false)
            {
                allServersDown = false;
                break;
            }
            else
            {
                allServersDown = true;
            }
        }

        failTimerUI.SetActive(allServersDown);

        RunTimers();

    }

    // Old method
    public void ScoreTick(int difficulty, int pendingUpdates, bool serverDown)
    {
        if (!serverDown)
        {
            playerScore += (scorePerDifficulty * difficulty);
            cumulativeUptime += (int)globalDamageInterval;
        }

        if (pendingUpdates != 0)
        {
            securityHealth -= (difficulty * pendingUpdates);
        }
    }

    // New method
    public void ScoringTick(int difficultyMod, int baseScore, ServerState state)
    {

    }

    // Old method
    public void BonusScore(int difficulty, bool fullyUpdated)
    {
        if (fullyUpdated)
        {
            playerScore += (fullyUpdatedBonus * difficulty);
        }
        if (!fullyUpdated)
        {
            playerScore += (completedUpdateBonus * difficulty);
        }
    }

    // New method
    public void AddBonusScore(int updatesCompleted, int difficultyMod, int baseScore, bool fullyUpdated)
    {
        if(fullyUpdated)
        {
            playerScore += ((updatesCompleted * difficultyMod * baseScore)*2);
            return;
        }

        playerScore = +(updatesCompleted * difficultyMod * baseScore);
    }

    // method combining all the player facing text in the UI during gameplay, called at the start of each update call
    private void LevelUITextUpdate()
    {
        playerScoreText.text = $"{playerScore}";
        securityHealthText.text = $"{securityHealth}";

        levelTimespan = TimeSpan.FromSeconds(winTimer);
        int winMins = levelTimespan.Minutes;
        int winSecs = levelTimespan.Seconds;

        levelTimerText.text = $"{winMins}:{winSecs}";
    }

    private void FailTimerTextUpdate()
    {
        failTimespan = TimeSpan.FromSeconds(downTimer);
        int failSecs = failTimespan.Seconds;
        int failMilis = failTimespan.Milliseconds;

        failTimerText.text = $"{failSecs}:{failMilis}";
    }

    private void RunTimers()
    {
        if(GameManager.Instance.gamePaused == false)
        {
            if (allServersDown)
            {
                downTimer -= Time.deltaTime;
                FailTimerTextUpdate();
            }
            if (!allServersDown)
            {
                downTimer = downTimerStart;
            }

            winTimer -= Time.deltaTime;
        }
    }

    private void CheckWinLoseConds()
    {
        if(GameManager.Instance.gamePaused == false)
        {
            if (winTimer <= 0)
            {
                GameManager.Instance.UpdateGameState(GameState.Win);
            }

            // sets state to lose if lose conditions are met and the player has not already won
            if (securityHealth <= 0 || downTimer <= 0)
            {
                if (GameManager.Instance.State != GameState.Win)
                {
                    GameManager.Instance.UpdateGameState(GameState.Lose);
                }
            }
        }
    }
}