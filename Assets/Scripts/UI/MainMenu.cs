using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MainMenu : GameStateResponsiveMenu
{
    public override EGameState gameStateMonitored => EGameState.MainMenu;

    public void OnPlayButton()
    {
        GameManager.PlayGame();
    }

    public void OnSettingsButton()
    {
        GameManager.OpenSettingsMainMenu();
    }

    public void OnQuitButton()
    {
        GameManager.ExitGame();
    }

    public void OnResetProgression()
    {
        CheckpointManager.ResetProgression();
    }
}
