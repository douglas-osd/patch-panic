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

    public int updateQueue;

    public int downloadedUpdates;

    private void Start()
    {
        UpdateServerState(ServerState.Idle);
    }

    private void Update()
    {
        if(GameManager.Instance.gamePaused == true)
        {
            return;
        }

        _serverType.DoesServerWantUpdates(updateQueue, wantsUpdate);

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
                break;
            case ServerState.NeedsUpdate:
                break;
            case ServerState.Downloading:
                break;
            case ServerState.DownloadFailed:
                break;
            case ServerState.DownloadComplete:
                HandleDownloadComplete();
                break;
            case ServerState.NeedsInstall:
                break;
            case ServerState.Installing:
                break;
            case ServerState.InstallFailed:
                break;
            case ServerState.InstallComplete:
                HandleInstallComplete();
                break;
            case ServerState.WaitingToNotify:
                break;
            case ServerState.UsersNotified:
                HandleUsersNotified();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
        }

        OnServerStateChanged?.Invoke(newState);

    }

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

    private void HandleDownloadComplete()
    {
        downloadedUpdates = updateQueue;

        UpdateServerState(ServerState.NeedsInstall);
    }    

    private void HandleInstalling()
    {
        if (_serverType.HandleInstallTimer(downloadTimer, downloadTimerSet))
        {
            if (_serverType.ErrorCheck("install"))
            {
                UpdateServerState(ServerState.InstallFailed);
                return;
            }

            UpdateServerState(ServerState.InstallComplete);
        }
    }

    private void HandleInstallComplete()
    {
        UpdateServerState(ServerState.WaitingToNotify);
    }

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