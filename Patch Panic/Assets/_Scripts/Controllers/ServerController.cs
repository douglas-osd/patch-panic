using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerController : MonoBehaviour
{

    [SerializeField] ServerType _serverType;

    public ServerState State;

    public static event Action<ServerState> OnServerStateChanged;

    private float updateTimer;
    private bool updateTimerSet;
    private float downloadTimer;
    private bool downloadTimerSet;
    private float installTimer;
    private bool installTimerSet;
    private bool wantsUpdate;
    private bool serverUp;

    public int updateQueue;

    public int downloadedUpdates;

    private void Start()
    {
        UpdateServerState(ServerState.Idle);
    }

    private void Awake()
    {
        LevelManager.GlobalScoringTick += LevelManagerGlobalScoringTick;
    }

    private void OnDestroy()
    {
        LevelManager.GlobalScoringTick -= LevelManagerGlobalScoringTick;
    }

    // Runs on event: runs the logic for adding score based on this server's status.
    private void LevelManagerGlobalScoringTick(bool ticked)
    {
        LevelManager.Instance.ScoringTick(_serverType.difficultyModifier, _serverType.baseScore, _serverType.baseDamage, serverUp, updateQueue);
    }

    private void Update()
    {
        // Stop running further logic if game is paused.
        if(GameManager.Instance.gamePaused == true)
        {
            return;
        }

        _serverType.DoesServerWantUpdates(updateQueue, wantsUpdate);

        // Updates the server state when an update is needed.
        HandleWaitForUpdate();

        switch (State)
        {
            case ServerState.Idle:
                break;
            case ServerState.NeedsUpdate:
                break;
            case ServerState.Downloading:
                HandleDownloading();
                break;
            case ServerState.DownloadFailed:
                break;
            case ServerState.DownloadComplete:
                break;
            case ServerState.NeedsInstall:
                break;
            case ServerState.Installing:
                HandleInstalling();
                break;
            case ServerState.InstallFailed:
                break;
            case ServerState.InstallComplete:
                break;
            case ServerState.WaitingToNotify:
                break;
            case ServerState.UsersNotified:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(State), State, null);
        }

    }

    // In each state, include a trigger for the relevant animation state & relevant sound effects
    // 'Handle' methods are for logic only
    public void UpdateServerState(ServerState newState)
    {
        State = newState;

        switch (newState)
        {
            case ServerState.Idle:
                HandleIdle();
                break;
            case ServerState.NeedsUpdate:
                HandleNeedsUpdate();
                break;
            case ServerState.Downloading:
                break;
            case ServerState.DownloadFailed:
                HandleDownloadFailed();
                break;
            case ServerState.DownloadComplete:
                HandleDownloadComplete();
                break;
            case ServerState.NeedsInstall:
                HandleNeedsInstall();
                break;
            case ServerState.Installing:
                break;
            case ServerState.InstallFailed:
                HandleInstallFailed();
                break;
            case ServerState.InstallComplete:
                HandleInstallComplete();
                break;
            case ServerState.WaitingToNotify:
                HandleWaitingToNotify();
                break;
            case ServerState.UsersNotified:
                HandleUsersNotified();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
        }

        OnServerStateChanged?.Invoke(newState);

    }

    // Runs continually to accumulate additional updates in the queue or move the server to the next state.
    // If the update timer logic returns true, count the update queue up by one.
    // If the server is idle and with a queue over 0, the server moves to NeedsUpdate.
    private void HandleWaitForUpdate()
    {
        if(_serverType.HandleUpdateTimer(updateTimer, updateTimerSet, wantsUpdate))
        {
            updateQueue++;
        }

        if(State == ServerState.Idle && updateQueue > 0)
        {
            UpdateServerState(ServerState.NeedsUpdate);
        }
    }

    private void HandleIdle()
    {
        // Animation & sound.
        serverUp = true;
    }

    private void HandleNeedsUpdate()
    {
        // Animation & play a sound.
        // If clicked in this state, switch state to Downloading & set server to DOWN.
        serverUp = false;
    }

    // If the download timer logic retuns true (timer has ended), check for a download error, then either fail or complete the download.
    private void HandleDownloading()
    {
        if(_serverType.HandleDownloadTimer(downloadTimer, downloadTimerSet))
        {
            if(_serverType.ErrorCheck("download"))
            {
                UpdateServerState(ServerState.DownloadFailed);
                return;
            }

            UpdateServerState(ServerState.DownloadComplete);
        }
    }

    private void HandleDownloadFailed()
    {
        // Animation & sound.
        // If clicked in this state, switch state to Downloading.
    }

    // When a download completes, update the downloaded queue to match the total updates at that time, then move to NeedsInstall
    private void HandleDownloadComplete()
    {
        downloadedUpdates = updateQueue;

        UpdateServerState(ServerState.NeedsInstall);
    }    

    private void HandleNeedsInstall()
    {
        // Animation & sound.
        // If clicked in this state, transition to Installing.
    }

    // Runs the install timer logic. If the timer returns true (timer has ended), then check for an install error, and either fail or complete the install.
    private void HandleInstalling()
    {
        if (_serverType.HandleInstallTimer(installTimer, installTimerSet))
        {
            if (_serverType.ErrorCheck("install"))
            {
                UpdateServerState(ServerState.InstallFailed);
                return;
            }

            UpdateServerState(ServerState.InstallComplete);
        }
    }

    private void HandleInstallFailed()
    {
        // Animation & sound.
        // If clicked in this state, transition to Installing.
    }

    // Install has completed, now move to wait for the player to notify users.
    private void HandleInstallComplete()
    {
        UpdateServerState(ServerState.WaitingToNotify);
    }

    private void HandleWaitingToNotify()
    {
        // Animation & sound.
        // If workstation is clicked in this state, set state to UsersNotified. Probably need to listen to an event here?
    }

    // When users are notified, count the update queue down by the number of updates accumulated in the download queue (that have now been installed)
    // Then run AddBonusScore logic and update the server state to idle.
    private void HandleUsersNotified()
    {
        updateQueue -= downloadedUpdates;
        LevelManager.Instance.AddBonusScore(downloadedUpdates, _serverType.difficultyModifier, _serverType.baseScore, _serverType.CheckFullyUpdated(updateQueue));
        UpdateServerState(ServerState.Idle);
    }

}

public enum ServerState
{
    Idle,
    NeedsUpdate,
    Downloading,
    DownloadFailed,
    DownloadComplete,
    NeedsInstall,
    Installing,
    InstallFailed,
    InstallComplete,
    WaitingToNotify,
    UsersNotified
}