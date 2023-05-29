using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkstationBehaviour : MonoBehaviour
{

    [Header("Settings")]
    public bool workstationLoggingEnabled; // enables logging from this workstation

    private GameObject[] servers;
    private bool canClick;

    void Start()
    {

            servers = GameObject.FindGameObjectsWithTag("Server");
            WorkstationLogger("Servers added to array.");
        
    }

    private void OnMouseDown()
    {
        if(GameManager.Instance.gamePaused == true)
        {
            return;
        }

        if(!canClick)
        {

            WorkstationLogger("Player not near object!");

        }
        else
        {

            foreach (GameObject go in servers)
            {
                go.GetComponent<ServerBehaviour>().NotifyUsers();
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

    void WorkstationLogger(object message)
    {
        if (workstationLoggingEnabled)
        {
            Debug.Log(message);
        }
    }

}