using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameStateDebugger : MonoBehaviour
{
#if UNITY_EDITOR
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
        Debug.Log($"<b>[GameManager]</b> Enter state : {gameState}");
    }

    public virtual void OnGameStateExit(EGameState gameState)
    {
        Debug.Log($"<b>[GameManager]</b> Exit state : {gameState}");
    }
#endif
}
