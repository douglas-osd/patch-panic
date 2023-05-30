using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Server : ScriptableObject
{

    // This class of scriptable object defines the basic components for all servers for the game.
    // Further assets derived from this scriptable object can be used to create server variants.
    // Attach the relevant scriptable object to the server's monobehaviour script to give it access to data.

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

}
