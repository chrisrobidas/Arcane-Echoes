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

    private void Start()
    {
        Register?.Invoke(this);

        for (int i = 0; i < transform.childCount; i++)
        {
            GameObject gameObject = transform.GetChild(i).gameObject;
            if (gameObject != m_spawnPoint) { }
        }
    }

    public void SpawnPlayer(GameObject player)
    {
        Vector3 position = m_spawnPoint != null ? m_spawnPoint.position : transform.position;
        Quaternion rotation = m_spawnPoint != null ? m_spawnPoint.rotation : transform.rotation;
        Instantiate(player, position, rotation);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == m_playerTag)
        {
            Activate?.Invoke(this);
        }
    }
}
