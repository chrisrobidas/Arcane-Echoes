using System;
using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    private static CheckpointManager m_instance;
    public static readonly string s_checkpointPlayerPrefName = "Checkpoint";
    private static int s_checkpointSaved;
    private static bool s_checkpointInitialized => !(s_checkpointSaved <= -1);

    #region Initialization
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

        s_checkpointSaved = PlayerPrefs.GetInt(s_checkpointPlayerPrefName, -1);

#if UNITY_EDITOR
        string checkpointLog = s_checkpointInitialized ? "<b>[CheckpointManager]</b> No checkpoint saved, enabling tutorial" : $"<b>[CheckpointManager]</b> Found checkpoint saved (ID {s_checkpointSaved})";
        Debug.Log(checkpointLog);
#endif
    }

    private void OnEnable()
    {
        Checkpoint.Register += OnRegisterCheckpoint;
        Checkpoint.Activate += OnActivateCheckpoint;
        GameManager.GameStateMachine.OnStateEnter += OnGameStateEnter;
    }

    private void OnDisable()
    {
        Checkpoint.Register -= OnRegisterCheckpoint;
        Checkpoint.Activate -= OnActivateCheckpoint;
        GameManager.GameStateMachine.OnStateEnter -= OnGameStateEnter;
    }

    #endregion

    private void OnGameStateEnter(EGameState state)
    {
        if (state == EGameState.Game)
        {
            GameManager.EnableTutorial(!s_checkpointInitialized);
        }
    }

    private void OnRegisterCheckpoint(Checkpoint checkpoint)
    {
#if UNITY_EDITOR
        Debug.Log($"<b>[CheckpointManager]</b> Checkpoint registered (ID {checkpoint.ID})");
#endif
        if (checkpoint != null && s_checkpointSaved == checkpoint.ID)
        {
            Debug.Log($"<b>[CheckpointManager]</b> Spawning player at checkpoint {checkpoint.ID}");
            checkpoint.SpawnPlayer();
        }
    }

    private void OnActivateCheckpoint(Checkpoint checkpoint)
    {
#if UNITY_EDITOR
        Debug.Log($"<b>[CheckpointManager]</b> Checkpoint activated (ID {checkpoint.ID})");
#endif
        s_checkpointSaved = checkpoint.ID;
        PlayerPrefs.SetInt(s_checkpointPlayerPrefName, s_checkpointSaved);
    }

    public static void ResetProgression()
    {
        PlayerPrefs.SetInt(s_checkpointPlayerPrefName, -1);
    }
}
