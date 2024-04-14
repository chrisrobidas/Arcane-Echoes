using UnityEngine;

public class RangedEnemy : Enemy
{
    [SerializeField] private GameObject _projectilePrefab;
    [SerializeField] private Transform _projectileSpawn;

    private float _elapsedTimeSinceLastProjectile;

    public override void Attack()
    {
        LookAtPlayer();

        if (IsAtStoppingDistanceFromPlayer())
        {
            _animator.SetBool("IsAttacking", true);
            _elapsedTimeSinceLastProjectile += Time.deltaTime;

            if (_elapsedTimeSinceLastProjectile >= _animator.GetCurrentAnimatorStateInfo(0).length)
            {
                ShootProjectileAtPlayer();
                _elapsedTimeSinceLastProjectile = 0;
            }
        }
        else
        {
            _elapsedTimeSinceLastProjectile = 0;

            if (_animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1)
            {
                _animator.SetBool("IsAttacking", false);
                StartChasing();
            }
        }
    }

    private void ShootProjectileAtPlayer()
    {
        GameObject projectile = Instantiate(_projectilePrefab);
        projectile.transform.position = _projectileSpawn.position;
        projectile.transform.LookAt(GameObject.FindWithTag("Player").transform.position);
        projectile.GetComponent<Projectile>().Caster = gameObject;
    }
}
