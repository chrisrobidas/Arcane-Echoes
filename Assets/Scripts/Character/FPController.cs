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
    [Range(1f, 3f)]
    private float jumpHeight;
    [Space]
    [HideInInspector]
    public float speedBoon;
    private float moveSpeed;
    // End Character control variables

    // Gravity
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
        float longitudinalMove = playerInputsAction.PlayerMovement.Movement.ReadValue<Vector2>().x;
        float transversalMove = playerInputsAction.PlayerMovement.Movement.ReadValue<Vector2>().y;

        //if (Input.GetKey(KeyCode.LeftShift))
        //{
        //    moveSpeed = sprintSpeed;
        //}
        //else if (Input.GetKey(KeyCode.LeftControl))
        //{
        //    moveSpeed = crouchSpeed;
        //}
        //else
        //{
        //    moveSpeed = baseSpeed;
        //}

        // Vector3 direction = transform.right * horizontalMov + transform.forward * verticalMov;
        // controller.Move(direction * moveSpeed * Time.deltaTime);

        if (!Physics.CheckSphere(groundCheck.position, groundDistanceCheck, groundMask))
        {
            // Apply gravity
            velocity.y += gravity * Time.deltaTime;
            controller.Move(velocity * Time.deltaTime);
        }
        else
        {
            velocity.y = -0.5f; // Reset the velocity to very low value just to stick player to the ground
        }
        
    }

    private void Jump(InputAction.CallbackContext context)
    {
        if(Physics.CheckSphere(groundCheck.position, groundDistanceCheck, groundMask))
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2 * gravity);
        }
    }
}
