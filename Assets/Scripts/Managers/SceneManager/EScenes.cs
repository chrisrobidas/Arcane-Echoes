using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Flags]
public enum EScenes
{
    None                = 0,

    Manager             = 1 << 0,
    Game                = 1 << 1,
    UI                  = 1 << 2,
    MainMenuBackground  = 1 << 3,

    All                 = ~0
}

public static class EScenesExtensions
{
    public static int GetSceneIndex(this EScenes scenes)
    {
        return (int)Mathf.Log(((int)scenes), 2);
    }
}
