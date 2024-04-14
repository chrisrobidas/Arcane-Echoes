using System;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public int ID => m_ID;
    [SerializeField] int m_ID;

    public static event Action<Checkpoint> register;
    public static event Action<Checkpoint> activate;

    private void Start()
    {
        register?.Invoke(this);
    }

    public void SpawnPlayer()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            activate?.Invoke(this);
        }
    }
}
