using UnityEngine;

public class EnemySummoner : MonoBehaviour
{
    [SerializeField] private GameObject _enemy;

    private void Awake()
    {
        _enemy.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _enemy.SetActive(true);
        }
    }
}
