using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GameManager
{
    public static event Action GameStarted;
    public static event Action GamePaused;
    public static event Action GameResumed;

    public static void InitializeGame()
    {
        SceneLoader.LoadScenes(EScenes.MainMenuBackground, EScenes.MainMenuBackground | EScenes.UI | EScenes.Test, false);
    }

    public static void StartGame()
    {
#if UNITY_EDITOR
        Debug.Log("Starting game");
#endif
        GameStarted?.Invoke();
        SceneLoader.LoadScenes(EScenes.Game, EScenes.Game | EScenes.UI, false);
    }

    public static void PauseGame()
    {
        GamePaused?.Invoke();
    }

    public static void ResumeGame()
    {
        GameResumed?.Invoke();
    }

    public static void ExitGame()
    {
#if UNITY_EDITOR
        Debug.Log("Quitting game");
        EditorApplication.ExitPlaymode();
#else
        Application.Quit();
#endif
    }
}
