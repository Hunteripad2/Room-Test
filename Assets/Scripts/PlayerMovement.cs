using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerMovement : NetworkBehaviour
{
    [HideInInspector] public Vector3 velocity;
    [HideInInspector] private float inputX;
    [HideInInspector] private float inputY;
    [HideInInspector] private enum AnimationState { idle, backward, forward, left, right, backwardRight, backwardLeft, forwardRight, forwardLeft }
    [HideInInspector] private AnimationState animationState;
    [HideInInspector] public NetworkVariable<Vector3> Position = new NetworkVariable<Vector3>();
    [HideInInspector] private PlayerMovement localPlayer;

    [Header("Movement")]
    [SerializeField] private Vector3 initialPosition = new Vector3(-0.5f, 0.3f, -3f);
    [SerializeField] private CharacterController characterController;
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
        localPlayer = NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject().GetComponent<PlayerMovement>();
        Position.Value = initialPosition;
    }

    private void Update()
    {
        if (this == localPlayer)
        {
            HandleMovement();
            HandleAnimation();
        }
        else
        {
            transform.position = Position.Value;
        }
    }

    private void FixedUpdate()
    {
        if (this == localPlayer)
        {
            HandleVelocity();

            if (NetworkManager.Singleton.IsServer)
            {
                Position.Value = transform.position;
            }
            else
            {
                SubmitPositionRequestServerRpc();
            }
        }
    }

    private void HandleMovement()
    {
        inputX = Input.GetAxis("Horizontal");
        inputY = Input.GetAxis("Vertical");

        Vector3 direction = transform.right * inputX + transform.forward * inputY;

        characterController.Move(direction * speed * Time.deltaTime);
    }

    [ServerRpc]
    void SubmitPositionRequestServerRpc(ServerRpcParams rpcParams = default)
    {
        Position.Value = transform.position;
    }

    private void HandleAnimation()
    {
        if (characterController.velocity == Vector3.zero)
        {
            animator.SetInteger("state", (int)AnimationState.idle);
        }
        else
        {
            UpdateAnimation();
        }
    }

    private void UpdateAnimation()
    {
        if (inputX > 0f && inputY > 0)
        {
            animator.SetInteger("state", (int)AnimationState.backward);
        }
        else if (inputX > 0f && inputY == 0)
        {
            animator.SetInteger("state", (int)AnimationState.left);
        }
        else if (inputX > 0f && inputY < 0)
        {
            animator.SetInteger("state", (int)AnimationState.forward);
        }
        else if (inputX == 0f && inputY > 0)
        {
            animator.SetInteger("state", (int)AnimationState.backward);
        }
        else if (inputX == 0f && inputY < 0)
        {
            animator.SetInteger("state", (int)AnimationState.forward);
        }
        else if (inputX < 0f && inputY > 0)
        {
            animator.SetInteger("state", (int)AnimationState.backward);
        }
        else if (inputX < 0f && inputY == 0)
        {
            animator.SetInteger("state", (int)AnimationState.right);
        }
        else
        {
            animator.SetInteger("state", (int)AnimationState.forward);
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

        characterController.Move(velocity * Time.deltaTime);
    }

    private bool IsOnGround()
    {
        return Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundMask);
    }
}
