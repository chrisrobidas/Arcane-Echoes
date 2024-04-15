using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Enemy))]
public class EnemyFieldOfView : FieldOfView
{
    private Enemy _enemy;

    protected override void Start()
    {
        base.Start();
        _enemy = GetComponent<Enemy>();
    }

    protected override void Update()
    {
        base.Update();

        if (VisibleTargets.Count > 0)
        {
            if (_enemy.CurrentState == Enemy.EnemyState.Wandering)
            {
                _enemy.StartChasingOrAttack();
            }
        }
    }
}
