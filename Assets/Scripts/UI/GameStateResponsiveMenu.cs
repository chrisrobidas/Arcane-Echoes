using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class GameStateResponsiveMenu : MonoBehaviour
{
    [SerializeField] GameObject m_menuBody;
    [SerializeField] GameObject m_firstSelectedGameObject;

    public abstract EGameState gameStateMonitored { get; }

    protected virtual void OnEnable()
    {
        GameManager.GameStateMachine.OnStateEnter += OnGameStateEnter;
        GameManager.GameStateMachine.OnStateExit += OnGameStateExit;
    }

    protected virtual void OnDisable()
    {
        GameManager.GameStateMachine.OnStateEnter -= OnGameStateEnter;
        GameManager.GameStateMachine.OnStateExit -= OnGameStateExit;
    }

    public virtual void OnGameStateEnter(EGameState gameState)
    {
        if (gameStateMonitored.HasFlag(gameState))
        {
            m_menuBody.SetActive(true);
            EventSystem.current.firstSelectedGameObject = m_firstSelectedGameObject;
        }
    }

    public virtual void OnGameStateExit(EGameState gameState)
    {
        if (gameStateMonitored.HasFlag(gameState))
        {
            m_menuBody.SetActive(false);
        }
    }
}
