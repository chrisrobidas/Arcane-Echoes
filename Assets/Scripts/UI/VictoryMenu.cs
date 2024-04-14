using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VictoryMenu : GameStateResponsiveMenu
{
    public override EGameState gameStateMonitored => EGameState.Victory;

    public void OnMainMenuButton()
    {
        GameManager.OpenMainMenu();
    }
}
