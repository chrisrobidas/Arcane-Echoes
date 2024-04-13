using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameStateDebugger : MonoBehaviour
{
    private void OnEnable()
    {
        GameManager.GameStateMachine.OnStateEnter += OnGameStateEnter;
        GameManager.GameStateMachine.OnStateExit += OnGameStateExit;
    }

    private void OnDisable()
    {
        GameManager.GameStateMachine.OnStateEnter -= OnGameStateEnter;
        GameManager.GameStateMachine.OnStateExit -= OnGameStateExit;
    }

    public virtual void OnGameStateEnter(EGameState gameState)
    {
        Debug.Log($"GameState entered {gameState}");
    }

    public virtual void OnGameStateExit(EGameState gameState)
    {
        Debug.Log($"GameState exited {gameState}");
    }
}
