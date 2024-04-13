using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameHUD : GameStateResponsiveMenu
{
    public override EGameState gameStateMonitored => EGameState.Game;

    public void CheatPause()
    {
        GameManager.PauseGame();
    }

    public void CheatWin()
    {
        GameManager.TriggerVictory();
    }

    public void CheatLose()
    {
        GameManager.TriggerGameOver();
    }
}
