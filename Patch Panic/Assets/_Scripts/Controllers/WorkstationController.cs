using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkstationController : MonoBehaviour
{

    public static WorkstationController Instance;

    public static event Action<bool> WorkstationTrigger;

    private bool canClick;

    private void Awake()
    {
        Instance = this;
    }

    private void OnMouseDown()
    {
        if(canClick)
        {
            WorkstationTrigger?.Invoke(true);
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
