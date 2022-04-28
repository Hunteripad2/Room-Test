using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [HideInInspector] private CharacterController controller;
    [HideInInspector] public Vector3 velocity;
    [HideInInspector] private enum AnimationState { idle, backward, forward, left, right, backwardRight, backwardLeft, forwardRight, forwardLeft }
    [HideInInspector] private AnimationState animationState;

    [Header("Movement")]
    [SerializeField] private float speed = 5f;
    [SerializeField] private float gravityForce = -9.81f;
    [SerializeField] public float defaultVelocityY = -1f;
    [SerializeField] private Animator animator;

    [Header("Environment")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.25f;
    [SerializeField] private LayerMask groundMask;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        HandleMovement();
    }

    private void FixedUpdate()
    {
        HandleVelocity();
    }

    private void HandleMovement()
    {
        float dirX = Input.GetAxis("Horizontal");
        float dirY = Input.GetAxis("Vertical");

        Vector3 direction = transform.right * dirX + transform.forward * dirY;

        controller.Move(direction * speed * Time.deltaTime);

        if (controller.velocity == Vector3.zero)
        {
            animator.SetInteger("state", (int)AnimationState.idle);
        }
        else
        {
            UpdateAnimation(dirX, dirY);
        }
    }

    private void UpdateAnimation(float dirX, float dirY)
    {
        if (dirX > 0f && dirY > 0)
        {
            animator.SetInteger("state", (int)AnimationState.backward);
        }
        else if (dirX > 0f && dirY == 0)
        {
            animator.SetInteger("state", (int)AnimationState.left);
        }
        else if (dirX > 0f && dirY < 0)
        {
            animator.SetInteger("state", (int)AnimationState.forward);
        }
        else if (dirX == 0f && dirY > 0)
        {
            animator.SetInteger("state", (int)AnimationState.backward);
        }
        else if (dirX == 0f && dirY < 0)
        {
            animator.SetInteger("state", (int)AnimationState.forward);
        }
        else if (dirX < 0f && dirY > 0)
        {
            animator.SetInteger("state", (int)AnimationState.backward);
        }
        else if (dirX < 0f && dirY == 0)
        {
            animator.SetInteger("state", (int)AnimationState.right);
        }
        else
        {
        }
    }

    private void HandleVelocity()
    {
        if (IsOnGround())
        {
            velocity.y = defaultVelocityY;
        }
        else
        {
            velocity.y += gravityForce * Time.deltaTime;
        }

        controller.Move(velocity * Time.deltaTime);
    }

    private bool IsOnGround()
    {
        return Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundMask);
    }
}
