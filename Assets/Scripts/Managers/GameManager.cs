using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class GameManager : MonoBehaviour
{
    public static GameStateMachine GameStateMachine => m_instance.m_gameStateMachine;
    private GameStateMachine m_gameStateMachine;

    private static GameManager m_instance;

    public static bool IsGamePaused => m_instance.m_gameStateMachine.GameState.HasFlag(EGameState.Pause);

    public static bool IsGameOver;

    private void Awake()
    {
        if (m_instance != null && m_instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            m_instance = this;
        }
        m_gameStateMachine = new GameStateMachine(EGameState.None);
    }

    public static void OpenMainMenu()
    {
        Time.timeScale = 1f;
        SoundManager.PlayMusic(SoundManager.SoundBank.mainMenuMusic);
        GameStateMachine.RemoveState(EGameState.Pause);

        m_instance.StartCoroutine(SceneLoader.LoadScenesCoroutine(EScenes.MainMenuBackground, EScenes.MainMenuBackground | EScenes.UI, false,
        () => { GameStateMachine.ChangeState(EGameState.MainMenu); }));

        //SceneLoader.LoadScenes(EScenes.MainMenuBackground, EScenes.MainMenuBackground | EScenes.UI, false,
        //() => { GameStateMachine.ChangeState(EGameState.MainMenu); });
    }

    public static void PlayGame()
    {
        IsGameOver = false;
        SoundManager.PlayMusic(SoundManager.SoundBank.gameMusic);
        GameStateMachine.RemoveState(EGameState.Pause);

        m_instance.StartCoroutine(SceneLoader.LoadScenesCoroutine(EScenes.Game, EScenes.Game | EScenes.UI, false,
        () => { GameStateMachine.ChangeState(EGameState.Game); }));

        //SceneLoader.LoadScenes(EScenes.Game, EScenes.Game | EScenes.UI, false,
        //() => { GameStateMachine.ChangeState(EGameState.Game); });
    }

    public static void RestartGame()
    {
        m_instance.StartCoroutine(SceneLoader.LoadScenesCoroutine(EScenes.Game, EScenes.Game, true) );
        //SceneLoader.LoadScenes(EScenes.Game, EScenes.Game, true);
    }

    public static void EnableTutorial(bool enable)
    {
        if (enable)
        {
            GameStateMachine.AddState(EGameState.Tutorial);
        }
        else
        {
            GameStateMachine.RemoveState(EGameState.Tutorial);
        }
    }

    public static void PauseResumeGame()
    {
        if (IsGamePaused)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }

    public static void PauseGame()
    {
        Time.timeScale = 0f;
        GameStateMachine.AddState(EGameState.Pause);
    }

    public static void ResumeGame()
    {
        Time.timeScale = 1f;
        GameStateMachine.RemoveState(EGameState.Pause);
    }

    public static void OpenSettingsPause()
    {
        GameStateMachine.AddState(EGameState.SettingsPause);
    }

    public static void CloseSettingsPause()
    {
        GameStateMachine.RemoveState(EGameState.SettingsPause);
    }

    public static void OpenSettingsMainMenu()
    {
        GameStateMachine.AddState(EGameState.SettingsMainMenu);
    }

    public static void CloseSettingsMainMenu()
    {
        GameStateMachine.RemoveState(EGameState.SettingsMainMenu);
    }

    public static void TriggerVictory()
    {
        Time.timeScale = 0f;
        GameStateMachine.ChangeState(EGameState.Victory);
    }

    public static void TriggerGameOver()
    {
        if (!IsGameOver)
        {
            SoundManager.PlaySound(SoundManager.SoundBank.deathSound);
        }
        Time.timeScale = 0f;
        IsGameOver = true;
        GameStateMachine.ChangeState(EGameState.GameOver);
    }

    public static void ExitGame()
    {
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#else
        Application.Quit();
#endif
    }
}
