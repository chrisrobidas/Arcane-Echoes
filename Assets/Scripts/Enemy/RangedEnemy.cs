using UnityEngine;

public class RangedEnemy : Enemy
{
    public override void Attack()
    {
        if (IsAtStoppingDistanceFromPlayer())
        {
            Debug.Log("Attack!!!");
        }
        else
        {
            StartChasing();
        }
    }
}
