using System.Collections;
using System.Collections.Generic;
using UnityEditor.Timeline;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Player Components")]
    [SerializeField]
    private Transform mainCamera;
    [SerializeField]
    private CharacterController controller;
    [SerializeField]
    private Transform objectsSummonPoint;
    private PlayerInputsAction playerInputsAction;

    // Ground check
    [Header("Ground Check")]
    [SerializeField]
    private Transform groundCheck;
    [SerializeField]
    private LayerMask groundMask;
    [SerializeField, Range(0.1f, 0.25f)]
    private float groundDistanceCheck = 0.1f;
    [Space]
    // End Ground check

    // Character control variables
    [Header("Movement tunables")]
    [SerializeField, Range(1f, 10f)]
    private float baseSpeed = 5f;
    [SerializeField, Range(1.5f, 3f)]
    private float sprintSpeedMultiplier;
    [SerializeField, Range(0.25f, 0.75f)]
    private float crouchSpeedMultiplier;
    [SerializeField, Range(1f, 10f)]
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
    private float gravityConstant = -9.81f;
    Vector3 gravityForce;

    // Defaults settings
    private float defaultControllerHeight;
    private float defaultControllerCenterY;
    private float defaultCameraPosY;
    private float defaultObjectSummonPosY;

    private bool isCrouching;

    private void Start()
    {
        playerInputsAction = new PlayerInputsAction();
        playerInputsAction.PlayerMovement.Enable();
        playerInputsAction.PlayerMovement.Jump.performed += Jump;

        defaultControllerHeight = controller.height;
        defaultControllerCenterY = controller.center.y;
        defaultCameraPosY = mainCamera.position.y;
        defaultObjectSummonPosY = objectsSummonPoint.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        if (Physics.CheckSphere(groundCheck.position, groundDistanceCheck, groundMask) & gravityForce.y < 0f)
        {
            Move();
            gravityForce.y = - 0.5f;
        }
        else
        {
            if(airControl)
            {
                Move();
            }
            gravityForce.y += gravityConstant * Time.deltaTime;
        }
        // Reset the gravityForce to very low value just to stick player to the ground
        controller.Move(gravityForce * Time.deltaTime);
    }

    private void Move()
    {
        float transversalMove = playerInputsAction.PlayerMovement.Movement.ReadValue<Vector2>().x;
        float longitudinalMove = playerInputsAction.PlayerMovement.Movement.ReadValue<Vector2>().y;
        float crouching = playerInputsAction.PlayerMovement.Crouch.ReadValue<float>();
        float sprinting = playerInputsAction.PlayerMovement.Sprint.ReadValue<float>();
        float playerSpeedModifier = 1f;

        if (crouching > 0f & sprinting == 0f)
        {
            playerSpeedModifier *= crouchSpeedMultiplier;
        }
        if (crouching == 0f & sprinting > 0f)
        {
            playerSpeedModifier *= sprintSpeedMultiplier;
        }
        moveSpeed = baseSpeed * playerSpeedModifier;

        if (crouching > 0)
        {
            isCrouching = true;
        }
        else
        {
            isCrouching = false;
        }

        Vector3 direction = transform.right * transversalMove + transform.forward * longitudinalMove;
        controller.Move(direction * moveSpeed * Time.deltaTime);
    }

    private void Jump(InputAction.CallbackContext context)
    {
        Debug.Log("JUMPING");
        if(Physics.CheckSphere(groundCheck.position, groundDistanceCheck, groundMask))
        {
            gravityForce.y = Mathf.Sqrt(jumpHeight * -2 * gravityConstant);
            controller.Move(gravityForce * Time.deltaTime);
        }
    }
}
