using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkstationController : MonoBehaviour
{

    public static WorkstationController Instance;

    //public static event Action<bool> WorkstationTrigger;

    private GameObject[] servers;

    private bool canClick;

    private void Awake()
    {
        Instance = this;
        servers = GameObject.FindGameObjectsWithTag("Server");
    }

    private void OnMouseDown()
    {
        if (GameManager.Instance.gamePaused == true)
        {
            return;
        }

        if (canClick)
        {
            //WorkstationTrigger?.Invoke(true);

            foreach (GameObject go in servers)
            {
                go.GetComponent<ServerController>().WorkstationTrigger();
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

}
