using UnityEngine;

public class MeleeEnemy : Enemy
{
    [SerializeField] private float _timeBeforeAttack = 1.5f;

    private float _elapsedTimeInAttackState;

    public override void Attack()
    {
        if (IsAtStoppingDistanceFromPlayer())
        {
            _elapsedTimeInAttackState += Time.deltaTime;

            if (_elapsedTimeInAttackState >= _timeBeforeAttack)
            {
                Debug.Log("Attack!!!");
            }
        }
        else
        {
            _elapsedTimeInAttackState = 0;
            StartChasing();
        }
    }
}
