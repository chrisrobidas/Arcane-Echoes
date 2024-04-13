using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateMachine
{
    public EGameState GameState => m_gameState;
    private EGameState m_gameState;

    public event Action<EGameState> OnStateEnter;
    public event Action<EGameState> OnStateExit;

    public GameStateMachine(EGameState gameState)
    {
        ChangeState(gameState);
    }

    public void ChangeState(EGameState state)
    {
        OnStateExit?.Invoke(m_gameState);
        m_gameState = state;
        OnStateEnter?.Invoke(m_gameState);
    }

    public void AddState(EGameState state)
    {
        if (m_gameState.HasFlag(state)) return;
        m_gameState |= state;
        OnStateEnter?.Invoke(state);
    }
    public void RemoveState(EGameState state)
    {
        if (!m_gameState.HasFlag(state)) return;
        m_gameState &= ~state;
        OnStateExit?.Invoke(state);
    }
}
