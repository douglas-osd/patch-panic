using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D body;

    private float horizontal;
    private float vertical;
    private readonly float moveLimiter = 0.7f;

    [Header("Settings")]
    [SerializeField] private float runSpeed = 20.0f;

    private Animator animator;

    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (GameManager.Instance.gamePaused == true)
        {
            return;
        }

        SetShockedFace();

        // Gives a value between -1 and 1
        horizontal = Input.GetAxisRaw("Horizontal"); // -1 is left
        vertical = Input.GetAxisRaw("Vertical"); // -1 is down

        animator.SetInteger("direction", (int)horizontal);
    }

    void FixedUpdate()
    {
        if (horizontal != 0 && vertical != 0) // Check for diagonal movement
        {
            // limit movement speed diagonally, so you move at 70% speed
            horizontal *= moveLimiter;
            vertical *= moveLimiter;
        }

        body.velocity = new Vector2(horizontal * runSpeed, vertical * runSpeed);
    }

    private void SetShockedFace()
    {
        if (LevelManager.Instance.allServersDown)
        {
            animator.SetBool("shocked", true);
        }
        else
        {
            animator.SetBool("shocked", false);
        }
    }
}
