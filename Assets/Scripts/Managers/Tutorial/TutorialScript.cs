using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class TutorialScript : ScriptableObject
{
    [Serializable]
    public class TutorialScriptStep
    {
        public string Title;
        [TextArea(3,10)] public string Description;
    }
    public List<TutorialScriptStep> Steps;
}
