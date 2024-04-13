using UnityEngine;

[ExecuteInEditMode]
public class EnemyFieldOfView : FieldOfView
{
    private bool _initialDrawFieldOfView;

    protected override void Start()
    {
        base.Start();
        _initialDrawFieldOfView = _drawFieldOfView;
    }

    protected override void Update()
    {
        base.Update();

        //GameObject target = VisibleTargets[0];
        //_enemyCombatScript.SetTarget(target);

        if (_initialDrawFieldOfView)
        {
            //_drawFieldOfView = _enemyCombatScript.Target == null && _statsScript.Health > 0;
        }
    }
}
