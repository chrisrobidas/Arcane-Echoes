using UnityEngine;

public class CursorManager : MonoBehaviour
{
    private void OnEnable()
    {
        GameManager.GameStateMachine.OnStateEnter += (gameState) => { OnGameStateChange(gameState, true); };
        GameManager.GameStateMachine.OnStateExit += (gameState) => { OnGameStateChange(gameState, false); };
    }

    private void OnDisable()
    {
        GameManager.GameStateMachine.OnStateEnter -= (gameState) => { OnGameStateChange(gameState, true); };
        GameManager.GameStateMachine.OnStateExit -= (gameState) => { OnGameStateChange(gameState, false); };
    }

    void OnGameStateChange(EGameState gameState, bool enter)
    {
        CursorLockMode lockMode = CursorLockMode.None;
        switch (gameState)
        {
            case EGameState.Game:
                enter = !enter;
                break;
            case EGameState.MainMenu:
            case EGameState.Pause:
            case EGameState.Victory:
            case EGameState.GameOver:
                break;
            default:
                break;
        }
        lockMode = enter ? CursorLockMode.None : CursorLockMode.Locked;
#if UNITY_EDITOR

        Debug.Log($"<b>[CursorManager]</b> Set cursor to {lockMode}");
#endif
        Cursor.lockState = lockMode;
       
    }
}
