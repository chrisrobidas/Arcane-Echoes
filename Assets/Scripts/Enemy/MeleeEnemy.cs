using UnityEngine;

public class MeleeEnemy : Enemy
{
    private float _elapsedTimeInAttackState;

    public override void Attack()
    {
        LookAtPlayer();

        if (IsAtStoppingDistanceFromPlayer())
        {
            _animator.SetBool("IsAttacking", true);
            _elapsedTimeInAttackState += Time.deltaTime;

            if (_animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1)
            {
                if (!GameManager.IsGameOver)
                {
                    SoundManager.PlaySoundAt(SoundManager.SoundBank.strikeSound, transform.position);
                }
            }

            if (_elapsedTimeInAttackState >= _animator.GetCurrentAnimatorStateInfo(0).length)
            {
                GameManager.TriggerGameOver();
            }
        }
        else
        {
            if (_animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1)
            {
                _elapsedTimeInAttackState = 0;
                _animator.SetBool("IsAttacking", false);
                StartChasing();
            }
        }
    }
}
