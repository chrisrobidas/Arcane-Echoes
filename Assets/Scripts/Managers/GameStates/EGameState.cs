using System;

[Flags]
public enum EGameState
{
    None             = 0,
    MainMenu         = 1 << 0,
    SettingsMainMenu = 1 << 1,
    SettingsPause    = 1 << 2,
    Tutorial         = 1 << 3,
    TutorialFreeze   = 1 << 4,
    Game             = 1 << 5,
    Pause            = 1 << 6,
    Victory          = 1 << 7,
    GameOver         = 1 << 8,
}
