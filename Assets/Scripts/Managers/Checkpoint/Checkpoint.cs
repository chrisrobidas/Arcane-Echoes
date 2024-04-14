using System;
using System.Drawing;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public int ID => m_ID;
    [SerializeField] int m_ID;
    [SerializeField] Transform m_spawnPoint;

    private string m_playerTag = "Player";

    public static event Action<Checkpoint> Register;
    public static event Action<Checkpoint> Activate;

    private void Awake()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            GameObject gameObject = transform.GetChild(i).gameObject;
            if (gameObject != m_spawnPoint) { }
        }
    }

    private void OnEnable()
    {
        GameManager.GameStateMachine.OnStateEnter += OnGameStateEnter;
    }

    private void OnDisable()
    {
        GameManager.GameStateMachine.OnStateEnter += OnGameStateEnter;
    }

    void OnGameStateEnter(EGameState state)
    {
        if (state.HasFlag(EGameState.Game))
        {
            Register?.Invoke(this);
        }
    }

    public void SpawnPlayer()
    {
        Transform player = FindObjectOfType<PlayerMovement>().transform;
        player.position = m_spawnPoint != null ? m_spawnPoint.position : transform.position;
        player.rotation = m_spawnPoint != null ? m_spawnPoint.rotation : transform.rotation;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == m_playerTag)
        {
            Activate?.Invoke(this);
        }
    }
}
