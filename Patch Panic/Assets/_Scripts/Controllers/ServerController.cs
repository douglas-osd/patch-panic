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
                break;
            case ServerState.NeedsInstall:
                break;
            case ServerState.Installing:
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