using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerBehaviour : MonoBehaviour
{
    [Header("Server Settings")]
    public float lowertimeOffset; // lower percentage offset for timers
    public float uppertimeOffset; // upper percentage offset for timers
    public float updateFrequency; // how often updates arrive
    public float downloadSpeed; // how long downloads take
    public float installSpeed; // how long installs take
    public int downloadErrorRate; // frequency of download errors
    public int installErrorRate; // frequence of install errors
    public int difficultyRating; // security difficulty modifier
    public bool serverLoggingEnabled; // enables & disables logging on this server
    public Animator animator;
    public Animator animator2;

    [Header("Server Monitoring")]
    public float updateClock; // update countdown timer
    public float downloadClock; // download progress timer
    public float installClock; // install progress timer
    public float damageClock; // clock for determining damage/score ticks
    public int serverStatus; // integer for server state, different values affect gameplay, eg. 0 = happy, 1 = needs download, 2 = downloading, 3 = awaiting install, 4 = installing, 5 = install complete, 6 = users notified
    public bool serverIsDown; // true if server is currently updating & unavailable to users
    public int pendingUpdates; // count of updates pending on this server
    public int downloadedUpdates; // updates due to install on this round

    private bool updateClockSet; // true if the update clock is set
    private bool downloadClockSet; // true if download clock is set
    private bool installClockSet; // true if install clock is set
    private bool canClick; // true during trigger enable, allows player to interract

    void Start()
    {
        serverStatus = 0;
    }

    void Update()
    {
        if (GameManager.Instance.gamePaused == true)
        {
            return;
        }

        if (!updateClockSet)
        {
            updateClock = Random.Range(lowertimeOffset * updateFrequency, uppertimeOffset * updateFrequency);
            updateClockSet = true;
            //ServerLogger("Clock set to" + updateClock);
        }

        if(updateClockSet)
        {
            updateClock -= Time.deltaTime;
        }

        if (updateClock <= 0.0f)
        {
            updateClockSet = false;
            pendingUpdates++;
            //ServerLogger($"Pending updates = {pendingUpdates}");

            if (serverStatus == 0)
            {
                NeedsUpdate();
            }
        }

        if (downloadClockSet == true && serverStatus == 2)
        {
            downloadClock -= Time.deltaTime;

            if (downloadClock <= 0.0f)
            {
                if(FailChecker(downloadErrorRate) == false)
                {
                    NeedsInstall();
                    downloadedUpdates = pendingUpdates;
                    //ServerLogger($"Downloaded updates = {downloadedUpdates}");
                }
                else
                {
                    //ServerLogger("Oops! Download Failed. Please restart.");
                    downloadClockSet = false;
                    animator.SetBool("serverError", true);
                    NeedsUpdate();
                }
            }
        }

        if (installClockSet == true && serverStatus == 4)
        {
            installClock -= Time.deltaTime;

            if (installClock <= 0.0f)
            {
                if(FailChecker(installErrorRate) == false)
                {
                    InstallComplete();
                }
                else
                {
                    //ServerLogger("Oops! Install failed. Please restart.");
                    installClockSet = false;
                    animator.SetBool("serverError", true);
                    NeedsInstall();
                }
            }
        }

        if (serverStatus == 6)
        {
            // add score etc. on level script for successful update
            if (pendingUpdates == 0)
            {
                LevelManager.Instance.BonusScore(difficultyRating, true);
                serverStatus = 0;
            }
            if (pendingUpdates != 0)
            {
                LevelManager.Instance.BonusScore(difficultyRating, false);
                NeedsUpdate();
            }
            serverIsDown = false;
            //ServerLogger("The server is reset! Success!");
        }

        damageClock += Time.deltaTime;

        if (damageClock >= LevelManager.Instance.globalDamageInterval)
        {
            LevelManager.Instance.ScoreTick(difficultyRating, pendingUpdates, serverIsDown);
            damageClock = 0;
            ServerLogger("Damage clock triggered.");
        }

        animator2.SetBool("serverDown", serverIsDown);

    }

    void OnMouseDown()
    {
        // fix this shit, use an enum for the server states

        if(GameManager.Instance.gamePaused == true)
        {
            return;
        }

        if (!canClick)
        {
            //ServerLogger("Player not near object!");
            return;
        }
        else
        {
            if (serverStatus == 0)
            {
                //ServerLogger("Server is happy, nothing happens...");
            }
            if (serverStatus == 1)
            {
                //ServerLogger("Server needs updating! Download will start...");

                StartDownload();
            }
            if (serverStatus == 2)
            {
                //ServerLogger("Download has begun, please be patient.");
            }
            if (serverStatus == 3)
            {
                //ServerLogger("Awaiting install of update! Install will start...");

                StartInstall();
            }
            if (serverStatus == 4)
            {
                //ServerLogger("Installation has begun, please be patient.");
            }
            if (serverStatus == 5)
            {
                //ServerLogger("Installation is complete! Please go to the terminal to notify users.");
            }
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

    private bool FailChecker(int errorRate)
    {
        if (Random.Range(0, 100) <= errorRate) // rolling below the error rate returns true, triggering fail behaviours
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    void NeedsUpdate()
    {
        serverStatus = 1;
        animator.SetBool("serverPending", false);
        animator.SetBool("serverAttention", true);
        //ServerLogger("Update Needed!");
    }

    void StartDownload()
    {
        serverStatus = 2;
        animator.SetBool("serverError", false);
        animator.SetBool("serverAttention", false);
        animator.SetBool("serverPending", true);
        serverIsDown = true;
        downloadClock = Random.Range(lowertimeOffset * downloadSpeed, uppertimeOffset * downloadSpeed);
        downloadClockSet = true;
        //ServerLogger("Download timer set to" + downloadClock);
    }

    void NeedsInstall()
    {
        serverStatus = 3;
        animator.SetBool("serverPending", false);
        animator.SetBool("serverInstalling", false);
        animator.SetBool("serverAttention", true);
        downloadClockSet = false;
        //ServerLogger("Awaiting install of update!");
    }

    void StartInstall()
    {
        serverStatus = 4;
        animator.SetBool("serverError", false);
        animator.SetBool("serverAttention", false);
        animator.SetBool("serverInstalling", true);
        installClock = Random.Range(lowertimeOffset * installSpeed, uppertimeOffset * installSpeed);
        installClockSet = true;
        //ServerLogger("Install timer set to" + installClock);
    }

    void InstallComplete()
    {
        serverStatus = 5;
        animator.SetBool("serverPending", false);
        animator.SetBool("serverInstalling", false);
        animator.SetBool("serverAttention", false);
        animator.SetBool("serverComplete", true);
        pendingUpdates -= downloadedUpdates;
        //ServerLogger($"{downloadedUpdates} complete, now {pendingUpdates} pending updates");
        //ServerLogger("Install complete! Please notify users.");
    }

    public void NotifyUsers()
    {
        if (serverStatus == 5)
        {
            animator.SetBool("serverComplete", false);
            serverStatus = 6;
            //ServerLogger("Users notified!");
        }
        else
        {
            //ServerLogger("No need to notify.");
        }
    }

    void ServerLogger(object message)
    {
        if (serverLoggingEnabled)
        {
            Debug.Log(message);
        }
    }

}