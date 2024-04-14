using System;
using System.Drawing;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public int ID => m_ID;
    [SerializeField] [Min(0)] int m_ID;

    private string m_playerTag = "Player";

    public static event Action<Checkpoint> Register;
    public static event Action<Checkpoint> Activate;

    private void Start()
    {
        Register?.Invoke(this);
#if !UNITY_EDITOR
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }
#endif
    }

    public void SpawnPlayer()
    {
        Debug.Log("<color=red><b>SPAWN PLAYER</b> (placeholder)</color>");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == m_playerTag)
        {
            Activate?.Invoke(this);
        }
    }
}
