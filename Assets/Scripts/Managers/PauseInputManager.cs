using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PauseInputManager : MonoBehaviour
{
    private PlayerInputsAction playerInputsAction;

    private void Awake()
    {
        playerInputsAction = new PlayerInputsAction();
        EnablePauseInput(false);
    }
    private void OnEnable()
    {
        GameManager.GameStateMachine.OnStateEnter += OnGameStateEnter;
        GameManager.GameStateMachine.OnStateExit += OnGameStatExit;
    }

    private void OnDisable()
    {
        GameManager.GameStateMachine.OnStateEnter -= OnGameStateEnter;
        GameManager.GameStateMachine.OnStateExit -= OnGameStatExit;
    }

    void OnGameStateEnter(EGameState gameState)
    {
        if (gameState.HasFlag(EGameState.Game))
        {
            EnablePauseInput(true);
        }
#if UNITY_EDITOR
        Debug.Log($"<b>[PauseInputManager]</b> Pause input enabled");
#endif
    }

    void OnGameStatExit(EGameState gameState)
    {
        if (gameState.HasFlag(EGameState.Game))
        {
            EnablePauseInput(false);
        }
#if UNITY_EDITOR
        Debug.Log($"<b>[PauseInputManager]</b> Pause input disabled");
#endif
    }

    private void EnablePauseInput(bool enable)
    {
        if (enable)
        {
            playerInputsAction.PauseInput.Enable();
            playerInputsAction.PauseInput.Pause.performed += Pause;
        }
        else
        {
            playerInputsAction.PauseInput.Disable();
            playerInputsAction.PauseInput.Pause.performed -= Pause;
        }
    }

    private void Pause(InputAction.CallbackContext context)
    {
        GameManager.PauseResumeGame();
    }
}
