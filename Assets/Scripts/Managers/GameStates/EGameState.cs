using System;

[Flags]
public enum EGameState
{
    None             = 0,
    MainMenu         = 1 << 0,
    SettingsMainMenu = 1 << 1,
    SettingsPause    = 1 << 2,
    Game             = 1 << 3,
    Pause            = 1 << 4,
    Victory          = 1 << 5,
    GameOver         = 1 << 6,
}
