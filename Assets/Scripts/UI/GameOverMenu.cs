using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverMenu : GameStateResponsiveMenu
{
    public override EGameState gameStateMonitored => EGameState.GameOver;

    public void OnRetryButton()
    {
        GameManager.PlayGame();
    }

    public void OnMainMenuButton()
    {
        GameManager.OpenMainMenu();
    }
}
