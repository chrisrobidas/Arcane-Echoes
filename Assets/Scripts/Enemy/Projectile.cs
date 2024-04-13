using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float _speed = 7f;
    [SerializeField] private float _lifeTime = 5f;

    private float _elapsedTime;

    private void Update()
    {
        _elapsedTime += Time.deltaTime;
        transform.position += transform.forward * _speed * Time.deltaTime;

        if (_elapsedTime > _lifeTime)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.GameOver();
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
