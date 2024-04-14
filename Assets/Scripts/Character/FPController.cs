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
    Vector2 moveInput;
    float sprinting = 0f;
    // End Character control variables

    // Gravity
    [Header("Gravity")]
    [SerializeField]
    private float gravityConstant = -9.81f;
    Vector3 gravityForce;

    private bool isGrounded;

    #region Inputs
    private void Awake()
    {
        playerInputsAction = new PlayerInputsAction();
    }

    private void OnEnable()
    {
        EnableInputs(true);
        GameManager.GameStateMachine.OnStateEnter += (gameState) => { OnGameStateChange(gameState, true); };
        GameManager.GameStateMachine.OnStateExit += (gameState) => { OnGameStateChange(gameState, false); };
    }

    private void OnDisable()
    {
        GameManager.GameStateMachine.OnStateEnter -= (gameState) => { OnGameStateChange(gameState, true); };
        GameManager.GameStateMachine.OnStateExit -= (gameState) => { OnGameStateChange(gameState, false); };
        EnableInputs(false);
    }

    void OnGameStateChange(EGameState gameState, bool enter)
    {
        switch (gameState)
        {
            case EGameState.Pause:
                break;
            case EGameState.Victory:
                break;
            case EGameState.GameOver:
                break;
            default:
                return;
        }
        //Disable inputs if in Pause/Victory/GameOver
        EnableInputs(!enter);
    }

    void EnableInputs(bool enable)
    {
        if (enable)
        {
            playerInputsAction.PlayerMovement.Enable();
            playerInputsAction.PlayerMovement.Movement.performed += Movement;
            playerInputsAction.PlayerMovement.Movement.canceled += Movement;
            playerInputsAction.PlayerMovement.Sprint.performed += Sprint;
            playerInputsAction.PlayerMovement.Sprint.canceled += Sprint;
            playerInputsAction.PlayerMovement.Jump.performed += Jump;
        }
        else
        {
            playerInputsAction.PlayerMovement.Disable();
            playerInputsAction.PlayerMovement.Movement.performed -= Movement;
            playerInputsAction.PlayerMovement.Movement.canceled -= Movement;
            playerInputsAction.PlayerMovement.Sprint.performed -= Sprint;
            playerInputsAction.PlayerMovement.Sprint.canceled -= Sprint;
            playerInputsAction.PlayerMovement.Jump.performed -= Jump;
        }
    }

    private void Movement(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    private void Sprint(InputAction.CallbackContext context)
    {
        sprinting = context.ReadValue<float>();
    }

    private void Jump(InputAction.CallbackContext context)
    {
        if (Physics.CheckSphere(groundCheck.position, groundDistanceCheck, groundMask))
        {
            gravityForce.y = Mathf.Sqrt(jumpHeight * -2 * gravityConstant);
            controller.Move(gravityForce * Time.deltaTime);
        }
    }
    #endregion

    // Update is called once per frame
    void Update()
    {
        if (Physics.CheckSphere(groundCheck.position, groundDistanceCheck, groundMask) & gravityForce.y < 0f)
        {
            isGrounded = true;
            Move();
            gravityForce.y = - 0.5f;
        }
        else
        {
            isGrounded = false;
            if (airControl)
            {
                Move();
            }
            gravityForce.y += gravityConstant * Time.deltaTime;
        }
        controller.Move(gravityForce * Time.deltaTime);
    }

    private void Move()
    {
        float playerSpeedModifier = 1f;

        if (sprinting > 0f & isGrounded)
        {
            playerSpeedModifier *= sprintSpeedMultiplier;
        }
        moveSpeed = baseSpeed * playerSpeedModifier;

        Vector3 direction = transform.right * moveInput.x + transform.forward * moveInput.y;
        controller.Move(direction * moveSpeed * Time.deltaTime);
    }
}
