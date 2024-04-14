using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    private static CheckpointManager m_instance;
    public static readonly string s_chechpointPlayerPrefName = "Checkpoint";
    private static int s_chechpointSaved;

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

        s_chechpointSaved = PlayerPrefs.GetInt(s_chechpointPlayerPrefName, -1);
        s_chechpointSaved = 0;
#if UNITY_EDITOR
        if (s_chechpointSaved == -1) 
        {
            Debug.Log("<b>[CheckpointManager]</b> No checkpoint saved");
        }
#endif
    }

    private void OnEnable()
    {
        Checkpoint.register += OnRegisterCheckpoint;
        Checkpoint.activate += OnActivateCheckpoint;
    }

    private void OnDisable()
    {
        Checkpoint.register -= OnRegisterCheckpoint;
        Checkpoint.activate -= OnActivateCheckpoint;
    }

    #endregion

    private void OnRegisterCheckpoint(Checkpoint checkpoint)
    {
#if UNITY_EDITOR
        Debug.Log($"<b>[CheckpointManager]</b> Checkpoint registered (ID {checkpoint.ID})");
#endif
        if (checkpoint != null && s_chechpointSaved == checkpoint.ID)
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
        s_chechpointSaved = checkpoint.ID;
        PlayerPrefs.SetInt(s_chechpointPlayerPrefName, s_chechpointSaved);
    }

    public static void ResetProgression()
    {
        PlayerPrefs.SetInt(s_chechpointPlayerPrefName, -1);
    }
}
