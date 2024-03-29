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

    public bool DoesServerWantUpdates(int updateQueue)
    {
        if (updateQueue >= maxUpdatesInQueue)
        {
            return false;
        }
        else
        {
            return true;
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
