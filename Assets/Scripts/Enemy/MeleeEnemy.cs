using UnityEngine;

public class MeleeEnemy : Enemy
{
    [SerializeField] private float _timeBeforeAttack = 1.5f;

    private float _elapsedTimeInAttackState;

    public override void Attack()
    {
        transform.LookAt(GameObject.FindWithTag("Player").transform.position, transform.up);

        if (IsAtStoppingDistanceFromPlayer())
        {
            _elapsedTimeInAttackState += Time.deltaTime;

            if (_elapsedTimeInAttackState >= _timeBeforeAttack)
            {
                GameManager.Instance.GameOver();
            }
        }
        else
        {
            _elapsedTimeInAttackState = 0;
            StartChasing();
        }
    }
}
