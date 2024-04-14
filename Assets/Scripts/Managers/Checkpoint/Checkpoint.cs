using System;
using System.Drawing;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public int ID => m_ID;
    [SerializeField] int m_ID;

    private string m_playerTag = "Player";

    public static event Action<Checkpoint> Register;
    public static event Action<Checkpoint> Activate;

    private void Start()
    {
        Register?.Invoke(this);
    }

    public void SpawnPlayer()
    {
        Debug.Log("<color=red><b>PLACEHOLDER METHOD</b> (CheckPoint.SpawnPlayer)</color>");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == m_playerTag)
        {
            Activate?.Invoke(this);
        }
    }
}
