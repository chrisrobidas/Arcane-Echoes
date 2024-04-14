using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : GameStateResponsiveMenu
{
    public override EGameState gameStateMonitored => EGameState.Pause;

    public void OnResumeButton()
    {
        GameManager.ResumeGame();
    }

    public void OnRestartButton()
    {
        GameManager.PlayGame();
    }

    public void OnSettingsButton()
    {
        GameManager.OpenSettingsPause();
    }

    public void OnMainMenuButton()
    {
        GameManager.OpenMainMenu();
    }

    public void OnQuitButton()
    {
        GameManager.ExitGame();
    }
}
