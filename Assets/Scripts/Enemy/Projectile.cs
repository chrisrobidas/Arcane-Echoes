using UnityEngine;

public class Projectile : MonoBehaviour
{
    [HideInInspector] public GameObject Caster;

    [SerializeField] private float _speed = 7f;
    [SerializeField] private float _lifeTime = 5f;
    [SerializeField] private GameObject _destroyEffect;
    [SerializeField] private GameObject _deathEffect;

    private float _elapsedTime;

    private void Update()
    {
        _elapsedTime += Time.deltaTime;
        transform.position += transform.forward * _speed * Time.deltaTime;

        if (_elapsedTime > _lifeTime)
        {
            Instantiate(_destroyEffect, gameObject.transform.position, gameObject.transform.rotation, null);
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Instantiate(_destroyEffect, gameObject.transform.position, gameObject.transform.rotation, null);
        Destroy(gameObject);

        if (other.CompareTag("Enemy") && Caster != other.gameObject)
        {
            Instantiate(_deathEffect, other.transform.position, other.transform.rotation, null);
            Destroy(other.gameObject);
        }

        if (other.CompareTag("Player"))
        {
            GameManager.TriggerGameOver();
        }
    }
}
