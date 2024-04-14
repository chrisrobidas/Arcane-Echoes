using System;
using UnityEngine;

[CreateAssetMenu]
public class TutorialTrigger : ScriptableObject
{
    public event Action Triggered;

    public void Trigger()
    {
        Triggered?.Invoke();
    }
}
