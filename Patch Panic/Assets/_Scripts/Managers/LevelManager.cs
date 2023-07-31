using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LevelManager : MonoBehaviour
{

    public static LevelManager Instance;

    public Player _player;

    [Header("UI Elements")]
    [SerializeField] private TMP_Text playerScoreText;
    [SerializeField] private TMP_Text playerHealthText;
    [SerializeField] private TMP_Text levelTimerText;
    [SerializeField] private GameObject failTimerUI;
    [SerializeField] private TMP_Text failTimerText;

    [Header("CurrentScores")]
    public int cumulativeUptime;

    [Header("Settings")]
    [SerializeField] private int startingPlayerHealth;
    [SerializeField] private float globalScoringInterval;
    [SerializeField] private float failTimerStart = 30.0f, winTimer = 120.0f;
    [SerializeField] private AudioClip levelMusic, updatedSound;
    [SerializeField] private string outOfTimeLoseText, downLoseText, hackedLoseText;

    private float failTimer;

    private GameObject[] servers;
    private TimeSpan levelTimespan;
    private TimeSpan failTimespan;
    [HideInInspector] public bool allServersDown;
    private float intervalTimer;
    [HideInInspector] public string failCause;

    public static event Action<bool> GlobalScoringTick;

    void Awake()
    {
        Instance = this;
    }

    private void OnDestroy()
    {
        
    }

    void Start()
    {
        _player.playerScore = 0;
        failTimer = failTimerStart;
        intervalTimer = globalScoringInterval;
        _player.playerHealth = startingPlayerHealth;
        SoundManager.Instance.PlayMusic(levelMusic);
        servers = GameObject.FindGameObjectsWithTag("Server");
        TriggerUpdateAtRandom();
    }

    void Update()
    {
        LevelUITextUpdate();

        CheckWinLoseConds();

        // TO DO: Allow this to fetch possible server types from an array so that this isn't hard coded.
        ServerDownChecker("Default");

        failTimerUI.SetActive(allServersDown);

        RunTimers();

    }

    public void ScoringTick(int difficultyMod, int baseScore, int baseDamage, bool serverUp, int updateQueue)
    {
        if(serverUp)
        {
            _player.playerScore += (baseScore * difficultyMod);
        }

        if(updateQueue != 0)
        {
            _player.playerHealth -= (baseDamage * updateQueue);
        }
    }

    // If the server is fully updated, double the score output, if it isn't, add the score without the multiplier.
    public void AddBonusScore(int updatesCompleted, int difficultyMod, int baseScore, bool fullyUpdated)
    {
        if(fullyUpdated)
        {
            _player.playerScore += updatesCompleted * difficultyMod * baseScore * 2;
            SoundManager.Instance.PlaySound(updatedSound);
            return;
        }

        _player.playerScore += updatesCompleted * difficultyMod * baseScore;
        SoundManager.Instance.PlaySound(updatedSound);
    }

    // method combining all the player facing text in the UI during gameplay, called at the start of each update call
    private void LevelUITextUpdate()
    {
        playerScoreText.text = $"{_player.playerScore}";
        playerHealthText.text = $"{_player.playerHealth}";

        levelTimespan = TimeSpan.FromSeconds(winTimer);
        int winMins = levelTimespan.Minutes;
        int winSecs = levelTimespan.Seconds;

        levelTimerText.text = $"{winMins}:{winSecs}";
    }

    private void FailTimerTextUpdate()
    {
        failTimespan = TimeSpan.FromSeconds(failTimer);
        int failSecs = failTimespan.Seconds;
        int failMilis = failTimespan.Milliseconds;

        failTimerText.text = $"{failSecs}:{failMilis}";
    }

    private void RunTimers()
    {
        if(GameManager.Instance.gamePaused == false)
        {
            intervalTimer -= Time.deltaTime;

            if(intervalTimer <= 0)
            {
                GlobalScoringTick?.Invoke(true);
                intervalTimer = globalScoringInterval;
                AddCumulativeUptime();
            }

            if (allServersDown)
            {
                failTimer -= Time.deltaTime;
                FailTimerTextUpdate();
            }
            if (!allServersDown)
            {
                DownTimerReset();
            }

            winTimer -= Time.deltaTime;
        }
    }

    private void DownTimerReset()
    {
        failTimer = failTimerStart;
    }

    private void ServerDownChecker(string serverVariant)
    {
        // Messy logic: loops over objects tagged 'server' to get the Controller & triggers the failTimer if none of them are up.
        // To do: Alter this to handle multiple server variants. This may involve reworking how this timer functions entirely!
        foreach (GameObject go in servers)
        {
            if (go.GetComponent<ServerController>().serverVariant != serverVariant)
            {
                break;
            }
            if (go.GetComponent<ServerController>().serverUp == true)
            {
                allServersDown = false;
                break;
            }
            else
            {
                allServersDown = true;
            }
        }
    }

    private void CheckWinLoseConds()
    {
        if(GameManager.Instance.gamePaused == false)
        {
            if(winTimer <= 0 && allServersDown)
            {
                failCause = outOfTimeLoseText;
                TriggerLose();
                return;
            }

            if (winTimer <= 0)
            {
                TriggerWin();
                return;
            }

            if (_player.playerHealth <= 0)
            {
                failCause = hackedLoseText;
                TriggerLose();
                return;
            }

            if (failTimer <= 0)
            {
                failCause = downLoseText;
                TriggerLose();
            }
        }
    }

    private void AddCumulativeUptime()
    {
        if (!allServersDown)
        {
            cumulativeUptime += (int)globalScoringInterval;
        }
    }

    private void TriggerWin()
    {
        GameManager.Instance.UpdateGameState(GameState.Win);
    }

    private void TriggerLose()
    {
        GameManager.Instance.UpdateGameState(GameState.Lose);
    }

    private void TriggerUpdateAtRandom()
    {
        GameObject go = servers[UnityEngine.Random.Range(0, servers.Length)];
        go.GetComponent<ServerController>().updateQueue++;
    }
}