using UnityEngine;

public class EnemyKiller : MonoBehaviour
{
    [SerializeField] private GameObject _deathEffect;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Instantiate(_deathEffect, other.transform.position, other.transform.rotation, null);
            Destroy(other.gameObject);
        }
    }
}
