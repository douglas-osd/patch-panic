using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ServerType", menuName = "Server Type")]
public class ServerType : ScriptableObject
{

    // This class of scriptable object defines the basic components for all servers for the game.
    // Further assets derived from this scriptable object can be used to create server variants.
    // Attach the relevant scriptable object to the server's monobehaviour script to give it access to data.

    //[MinMaxRange(0,30)]
    //public RangedFloat updateFrequency;

    public float lowerTimerOffset;
    public float upperTimerOffset;
    public float updateFrequency;
    public float downloadSpeed;
    public float installSpeed;
    public int downloadErrorRate;
    public int installErrorRate;
    public int difficultyModifier;

    public bool FailChecker(bool isDownload)
    {
        int errorRate = (isDownload) ? downloadErrorRate : installErrorRate;

        if (Random.Range(0, 100) <= errorRate)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void HandleUpdateTimer(float timer, bool clockSet)
    {
        if(!clockSet)
        {
            timer = Random.Range(lowerTimerOffset * updateFrequency, upperTimerOffset * updateFrequency);
            clockSet = true;
        }

        timer -= Time.deltaTime;

        if(timer <= 0.0f)
        {
            clockSet = false;
            timer = 0.0f;
            //TriggerUpdate();
        }
    }

    public void HandleDownloadTimer(float timer, bool clockSet)
    {
        if(!clockSet)
        {
            timer = Random.Range(lowerTimerOffset * updateFrequency, upperTimerOffset * updateFrequency);
            clockSet = true;
        }

        timer -= Time.deltaTime;

        if(timer <= 0.0f)
        {
            clockSet = false;
            timer = 0.0f;
            //TriggerDownloadComplete();
        }
    }

    public void HandleInstallTimer(float timer, bool clockSet)
    {
        if(!clockSet)
        {
            timer = Random.Range(lowerTimerOffset * updateFrequency, upperTimerOffset * updateFrequency);
        }

        timer -= Time.deltaTime;

        if(timer <= 0.0f)
        {
            clockSet = false;
            timer = 0.0f;
            //TriggerInstallComplete();
        }
    }

}
