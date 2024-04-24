using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;

public class LookMouse : MonoBehaviour
{
    [SerializeField]
    private Transform m_playerBody;
    public Transform Playerbody => m_playerBody;

    [SerializeField]
    [Range(1f, 100f)]
    private float m_mouseSensitivity;
    public float MouseSensitivity => m_mouseSensitivity;
    public static readonly string s_sensitivityParamName = "Sensitivity";

    private PlayerInputsAction playerInputsAction;

    private float camXRotation = 0f;
    private Vector2 lookInput;

    #region Inputs
    void Awake()
    {
        playerInputsAction = new PlayerInputsAction();
        m_mouseSensitivity = PlayerPrefs.GetFloat(s_sensitivityParamName, 20);
    }

    private void OnEnable()
    {
        EnableInputs(true);
        GameManager.GameStateMachine.OnStateEnter += (gameState) => { OnGameStateChange(gameState, true); };
        GameManager.GameStateMachine.OnStateExit += (gameState) => { OnGameStateChange(gameState, false); };
        SettingsMenu.onSensibilityChanged += SetSensitivity;
    }

    private void OnDisable()
    {
        SettingsMenu.onSensibilityChanged -= SetSensitivity;
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

    private void SetSensitivity(float value)
    {
        m_mouseSensitivity = value;
        PlayerPrefs.SetFloat(s_sensitivityParamName, m_mouseSensitivity);
    }

    void EnableInputs(bool enable)
    {
        if (enable)
        {
            playerInputsAction.PlayerLook.Enable();
            playerInputsAction.PlayerLook.Look.performed += Look;
            playerInputsAction.PlayerLook.Look.canceled += Look;
        }
        else
        {
            playerInputsAction.PlayerLook.Disable();
            playerInputsAction.PlayerLook.Look.performed -= Look;
            playerInputsAction.PlayerLook.Look.canceled -= Look;
        }
    }

    private void Look(InputAction.CallbackContext context)
    {
        lookInput = context.ReadValue<Vector2>();
    }
    #endregion

    void Update()
    {
        Vector2 deltaMouse = lookInput * m_mouseSensitivity * Time.deltaTime;

        camXRotation -= deltaMouse.y;
        camXRotation = Mathf.Clamp(camXRotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(camXRotation, 0f, 0f);
        m_playerBody.Rotate(Vector3.up * deltaMouse.x);
    }
}
