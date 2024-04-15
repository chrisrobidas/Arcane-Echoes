using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.Timeline.AnimationPlayableAsset;

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
        GameManager.GameStateMachine.OnStateEnter += (gameState) => { OnGameStateChange( gameState, true); };
        GameManager.GameStateMachine.OnStateExit += (gameState) => { OnGameStateChange(gameState, false); };
    }

    private void OnDisable()
    {
        GameManager.GameStateMachine.OnStateEnter -= (gameState) => { OnGameStateChange(gameState, true); };
        GameManager.GameStateMachine.OnStateExit -= (gameState) => { OnGameStateChange(gameState, false); };
    }

    void OnGameStateChange(EGameState gameState, bool enter)
    {
        if (gameState == EGameState.Game)
        {
            bool enable = enter ? true : false;
            EnablePauseInput(enable);
#if UNITY_EDITOR
            string enableLog = enable ? "enabled" : "disabled";
            Debug.Log($"<b>[PauseInputManager]</b> Pause input {enableLog}");
#endif
        }
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
