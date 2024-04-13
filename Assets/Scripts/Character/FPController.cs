using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    private CharacterController controller;
    private PlayerInputsAction playerInputsAction;

    // Ground check
    [Header("Ground Check")]
    [SerializeField]
    private Transform groundCheck;
    [SerializeField]
    private LayerMask groundMask;
    [SerializeField]
    private float groundDistanceCheck;
    [Space]
    // End Ground check

    // Character control variables
    [Header("Movement tunables")]
    [SerializeField, Range(1f, 10f)]
    private float baseSpeed = 5f;
    [SerializeField]
    [Range(1.5f, 3f)]
    private float sprintSpeedMultiplier;
    [SerializeField]
    [Range(0.25f, 0.75f)]
    private float crouchSpeedMultiplier;
    [SerializeField]
    [Range(1f, 10f)]
    private float jumpHeight;
    [SerializeField]
    private bool airControl;
    [Space]
    [HideInInspector]
    public float speedBoonMultiplier = 1f;
    private float moveSpeed;
    // End Character control variables

    // Gravity
    [Header("Gravity")]
    [SerializeField]
    private float gravity = -9.81f;
    Vector3 velocity;

    private void Start()
    {
        playerInputsAction = new PlayerInputsAction();
        playerInputsAction.PlayerMovement.Enable();
        playerInputsAction.PlayerMovement.Jump.performed += Jump;
    }

    // Update is called once per frame
    void Update()
    {
        if (Physics.CheckSphere(groundCheck.position, groundDistanceCheck, groundMask))
        {
            Move();
            velocity.y = -0.5f;
        }
        else
        {
            if(airControl)
            {
                Move();
            }
            velocity.y += gravity * Time.deltaTime;            
        }
        // Reset the velocity to very low value just to stick player to the ground
        controller.Move(velocity * Time.deltaTime);
    }

    private void Move()
    {
        float transversalMove = playerInputsAction.PlayerMovement.Movement.ReadValue<Vector2>().x;
        float longitudinalMove = playerInputsAction.PlayerMovement.Movement.ReadValue<Vector2>().y;
        float playerSpeedModifier = 1f;

        if (playerInputsAction.PlayerMovement.Crouch.ReadValue<float>() > 0f & playerInputsAction.PlayerMovement.Sprint.ReadValue<float>() == 0f)
        {
            playerSpeedModifier *= crouchSpeedMultiplier;
        }
        if (playerInputsAction.PlayerMovement.Crouch.ReadValue<float>() == 0f & playerInputsAction.PlayerMovement.Sprint.ReadValue<float>() > 0f)
        {
            playerSpeedModifier *= sprintSpeedMultiplier;
        }
        moveSpeed = baseSpeed * playerSpeedModifier;

        Vector3 direction = transform.right * transversalMove + transform.forward * longitudinalMove;
        controller.Move(direction * moveSpeed * Time.deltaTime);
    }

    private void Jump(InputAction.CallbackContext context)
    {
        if(Physics.CheckSphere(groundCheck.position, groundDistanceCheck, groundMask))
        {
            Debug.Log("Jumping");
            velocity.y = Mathf.Sqrt(jumpHeight * -2 * gravity);
            controller.Move(velocity * Time.deltaTime);
        }
    }
}
