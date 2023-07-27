using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerController : MonoBehaviour
{

    [SerializeField] ServerType _serverType;

    private Animator animator;

    public ServerState State;

    public static event Action<ServerState> OnServerStateChanged;

    private float updateTimer;
    private bool updateTimerSet;
    private bool wantsUpdate;
    private float downloadTimer;
    private bool downloadTimerSet;
    private float installTimer;
    private bool installTimerSet;
    public bool serverUp;
    private bool canClick;

    public int updateQueue;

    public int downloadedUpdates;

    private void Start()
    {
        UpdateServerState(ServerState.Idle);
        animator = GetComponent<Animator>();
    }

    private void Awake()
    {
        LevelManager.GlobalScoringTick += LevelManagerGlobalScoringTick;
        //WorkstationController.WorkstationTrigger += WorkstationTrigger;
    }

    private void OnDestroy()
    {
        LevelManager.GlobalScoringTick -= LevelManagerGlobalScoringTick;
        //WorkstationController.WorkstationTrigger -= WorkstationTrigger;
    }

    // Runs on event: runs the logic for adding score based on this server's status.
    private void LevelManagerGlobalScoringTick(bool ticked)
    {
        LevelManager.Instance.ScoringTick(_serverType.difficultyModifier, _serverType.baseScore, _serverType.baseDamage, serverUp, updateQueue);
        Debug.Log("Score ticked.");
    }

    public void WorkstationTrigger()
    {
        if(State == ServerState.WaitingToNotify)
        {
            UpdateServerState(ServerState.UsersNotified);
            Debug.Log("Users notified by workstation.");
        }
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
                HandleIdle();
                break;
            case ServerState.NeedsUpdate:
                HandleNeedsUpdate();
                break;
            case ServerState.Downloading:
                HandleStartDownload();
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
                HandleStartInstall();
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

    private void HandleWaitForUpdate()
    {
        if (_serverType.DoesServerWantUpdates(updateQueue))
        {
            wantsUpdate = true;

            if(HandleUpdateTimer())
            {
                updateQueue++;
            }
        }

        if (State == ServerState.Idle && updateQueue > 0)
        {
            UpdateServerState(ServerState.NeedsUpdate);
        }
    }

    private void HandleIdle()
    {
        serverUp = true;
    }

    private void HandleNeedsUpdate()
    {
        // Animation & sound.
        animator.SetInteger("animState", 1);
    }

    private void HandleStartDownload()
    {
        // Animation & sound.
        animator.SetInteger("animState", 2);
        serverUp = false;
    }

    // If the download timer logic retuns true (timer has ended), check for a download error, then either fail or complete the download.
    private void HandleDownloading()
    {
        if(HandleDownloadTimer())
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
        animator.SetInteger("animState", 3);
    }

    private void HandleDownloadComplete()
    {
        downloadedUpdates = updateQueue;
        animator.SetInteger("animState", 4);

        UpdateServerState(ServerState.NeedsInstall);
    }    

    private void HandleNeedsInstall()
    {
        // Animation & sound?
    }

    private void HandleStartInstall()
    {
        // Animation & sound.
        animator.SetInteger("animState", 5);
    }

    private void HandleInstalling()
    {
        if (HandleInstallTimer())
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
        animator.SetInteger("animState", 6);
    }

    private void HandleInstallComplete()
    {
        animator.SetInteger("animState", 7);
        UpdateServerState(ServerState.WaitingToNotify);
    }

    private void HandleWaitingToNotify()
    {
        // Animation & sound?
        // If workstation is clicked in this state, set state to UsersNotified. Probably need to listen to an event here?
    }

    private void HandleUsersNotified()
    {
        // Add sound effect and animation for updating the server!
        animator.SetTrigger("notified");
        updateQueue -= downloadedUpdates;
        LevelManager.Instance.AddBonusScore(downloadedUpdates, _serverType.difficultyModifier, _serverType.baseScore, _serverType.CheckFullyUpdated(updateQueue));
        UpdateServerState(ServerState.Idle);
    }

    private void OnMouseDown()
    {
        if (GameManager.Instance.gamePaused == true)
        {
            return;
        }

        if(!canClick)
        {
            return;
        }

        switch (State)
        {
            case ServerState.Idle:
                break;
            case ServerState.NeedsUpdate:
                UpdateServerState(ServerState.Downloading);
                break;
            case ServerState.Downloading:
                break;
            case ServerState.DownloadFailed:
                UpdateServerState(ServerState.Downloading);
                break;
            case ServerState.DownloadComplete:
                break;
            case ServerState.NeedsInstall:
                UpdateServerState(ServerState.Installing);
                break;
            case ServerState.Installing:
                break;
            case ServerState.InstallFailed:
                UpdateServerState(ServerState.Installing);
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            canClick = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            canClick = false;
        }
    }

    public bool HandleUpdateTimer()
    {
        if (!wantsUpdate)
        {
            updateTimerSet = false;
            updateTimer = 0.0f;
            return false;
        }

        if (!updateTimerSet)
        {
            updateTimer = UnityEngine.Random.Range(_serverType.updateFrequency.minValue, _serverType.updateFrequency.maxValue);
            updateTimerSet = true;
        }

        updateTimer -= Time.deltaTime;

        if (updateTimer <= 0.0f)
        {
            updateTimerSet = false;
            updateTimer = 0.0f;
            return true;
        }

        return false;
    }

    public bool HandleDownloadTimer()
    {
        if (!downloadTimerSet)
        {
            downloadTimer = UnityEngine.Random.Range(_serverType.downloadSpeed.minValue, _serverType.downloadSpeed.maxValue);
            downloadTimerSet = true;
        }

        downloadTimer -= Time.deltaTime;

        if (downloadTimer <= 0.0f)
        {
            downloadTimerSet = false;
            downloadTimer = 0.0f;
            return true;
        }

        return false;
    }

    public bool HandleInstallTimer()
    {
        if (!installTimerSet)
        {
            installTimer = UnityEngine.Random.Range(_serverType.installSpeed.minValue, _serverType.installSpeed.maxValue);
            installTimerSet = true;
        }

        installTimer -= Time.deltaTime;

        if (installTimer <= 0.0f)
        {
            installTimerSet = false;
            installTimer = 0.0f;
            return true;
        }

        return false;
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