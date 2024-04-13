using UnityEngine;

public class RangedEnemy : Enemy
{
    [SerializeField] private GameObject _projectilePrefab;
    [SerializeField] private Transform _projectileSpawn;
    [SerializeField] private float _projectileCooldown = 4f;

    private float _elapsedTimeSinceLastProjectile;

    public override void Attack()
    {
        LookAtPlayer();

        if (IsAtStoppingDistanceFromPlayer())
        {
            _elapsedTimeSinceLastProjectile += Time.deltaTime;

            if (_elapsedTimeSinceLastProjectile >= _projectileCooldown)
            {
                ShootProjectileAtPlayer();
                _elapsedTimeSinceLastProjectile = 0;
            }
        }
        else
        {
            _elapsedTimeSinceLastProjectile = 0;
            StartChasing();
        }
    }

    private void ShootProjectileAtPlayer()
    {
        GameObject projectile = Instantiate(_projectilePrefab);
        projectile.transform.position = _projectileSpawn.position;
        projectile.transform.LookAt(GameObject.FindWithTag("Player").transform.position);
    }
}
