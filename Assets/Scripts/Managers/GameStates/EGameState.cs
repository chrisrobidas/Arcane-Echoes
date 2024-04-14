using System;

[Flags]
public enum EGameState
{
    None             = 0,
    MainMenu         = 1 << 0,
    SettingsMainMenu = 1 << 1,
    SettingsPause    = 1 << 2,
    Tutorial         = 1 << 3,
    Game             = 1 << 4,
    Pause            = 1 << 5,
    Victory          = 1 << 6,
    GameOver         = 1 << 7,
}
