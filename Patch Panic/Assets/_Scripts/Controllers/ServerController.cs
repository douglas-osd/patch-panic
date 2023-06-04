using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerController : MonoBehaviour
{

    [SerializeField] ServerType _serverType;

    public ServerState State;

    private void Start()
    {
        
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
            case ServerState.NeedsInstall:
                break;
            case ServerState.Installing:
                break;
            case ServerState.InstallFailed:
                break;
            case ServerState.InstallComplete:
                break;
            case ServerState.UsersNotified:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
        }

    }


}

public enum ServerState
{
    Idle,
    NeedsUpdate,
    Downloading,
    DownloadFailed,
    NeedsInstall,
    Installing,
    InstallFailed,
    InstallComplete,
    UsersNotified
}