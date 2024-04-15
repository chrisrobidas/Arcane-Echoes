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
        switch (gameState)
        {
             //case EGameState.Game:
             //   Cursor.lockState = !enter ? CursorLockMode.None : CursorLockMode.Locke;
             //   return;
            case EGameState.MainMenu:
                break;
            case EGameState.Pause:
                break;
            case EGameState.Victory:
                break;
            case EGameState.GameOver:
                break;
            default:
                return;
        }
        CursorLockMode lockMode = !enter ? CursorLockMode.Locked : CursorLockMode.None;
#if UNITY_EDITOR

        Debug.Log($"<b>[CursorManager]</b> Set cursor to {lockMode}");
#endif
        Cursor.lockState = lockMode;
    }
}
