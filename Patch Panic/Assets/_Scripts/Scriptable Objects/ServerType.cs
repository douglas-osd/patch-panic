using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ServerType", menuName = "Server Type")]
public class ServerType : ScriptableObject
{

    // This class of scriptable object defines the basic components for all servers for the game.
    // Further assets derived from this scriptable object can be used to create server variants.
    // Attach the relevant scriptable object to the server's monobehaviour script to give it access to data.

    // Include animations for:
    // Idle/Server Running, Please Trigger Download, Downloading, Download Failed, Download Succeeded, Please Trigger Install, Installing, Install Failed, Install Succeeded/Please Notify Users

    [MinMaxRange(0, 30)] public RangedFloat updateFrequency, downloadSpeed, installSpeed;

    [SerializeField] public float downloadErrorRate, installErrorRate;
    [SerializeField] public int baseScore, baseDamage, difficultyModifier, maxUpdatesInQueue;



    public bool ErrorCheck(string errorType)
    {
        return errorType switch
        {
            "download" => Random.Range(0, 100) <= downloadErrorRate * difficultyModifier,
            "install" => Random.Range(0, 100) <= installErrorRate * difficultyModifier,
            _ => false
        };
    }

    // Handles logic for the update queue timer: checks if updates are wanted, sets the clock, ticks the timer and returns true or false.
    // Returns true when the timer hits zero after being set and ticking down. Returns false if the server doesn't want an update.
    public bool HandleUpdateTimer(float timer, bool clockSet, bool wantsUpdate)
    {
        // If the server doesn't currently want an update, the clock is not set and at 0, then return false
        if(!wantsUpdate)
        {
            clockSet = false;
            timer = 0.0f;
            return false;
        }

        // If the clock is not set, set the clock to a random value in the range
        if(!clockSet)
        {
            timer = Random.Range(updateFrequency.minValue, updateFrequency.maxValue);
            clockSet = true;
        }

        // Tick timer
        timer -= Time.deltaTime;

        // When the timer hits 0, return true.
        if(timer <= 0.0f)
        {
            clockSet = false;
            timer = 0.0f;
            return true;
        }

        return false;
    }

    // Handles download timer logic. If the clock isn't set, set the clock, tick the clock, and return true when the timer hits 0 or false if not at 0.
    public bool HandleDownloadTimer(float timer, bool clockSet)
    {
        if(!clockSet)
        {
            timer = Random.Range(downloadSpeed.minValue, downloadSpeed.maxValue);
            clockSet = true;
        }

        timer -= Time.deltaTime;

        if(timer <= 0.0f)
        {
            clockSet = false;
            timer = 0.0f;
            return true;
        }

        return false;
    }

    // Handles install timer logic. If the clock isn't set, set the clock, tick the clock, and return true when the timer hits 0 or false if not at 0.
    public bool HandleInstallTimer(float timer, bool clockSet)
    {
        if(!clockSet)
        {
            timer = Random.Range(installSpeed.minValue, installSpeed.maxValue);
            clockSet = true;
        }

        timer -= Time.deltaTime;

        if(timer <= 0.0f)
        {
            clockSet = false;
            timer = 0.0f;
            return true;
        }

        return false;
    }

    // Runs logic determining if the server wants updates.
    // Checks the update queue against the max number allowed in the queue, and returns true or false.
    public void DoesServerWantUpdates(int updateQueue, bool wantsUpdate)
    {
        if (updateQueue >= maxUpdatesInQueue)
        {
            wantsUpdate = false;
        }
        else
        {
            wantsUpdate = true;
        }
    }

    // Logic for if the server is fully up to date. Runs when update is complete.
    // If the update queue is 0, returns true, meaning it is fully updates.
    public bool CheckFullyUpdated(int updateQueue)
    {
        if(updateQueue == 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

}
