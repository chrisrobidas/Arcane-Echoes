using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverMenu : GameStateResponsiveMenu
{
    public override EGameState gameStateMonitored => EGameState.GameOver;

    public void OnRetryButton()
    {
        GameManager.RestartGame();
    }

    public void OnMainMenuButton()
    {
        GameManager.OpenMainMenu();
    }
}
