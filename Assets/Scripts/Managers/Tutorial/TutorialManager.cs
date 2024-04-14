using System;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    private static TutorialManager m_instance;
    private bool m_skipped = true;
    [SerializeField] private TutorialScript m_script;
    [SerializeField] private int m_stepIndex = -1;

    public static event Action<TutorialScript.TutorialScriptStep> m_step;

    private void Awake()
    {
        if (m_instance != null && m_instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            m_instance = this;
        }
    }

    public static void ActivateTutorial()
    {
        m_instance.m_skipped = false;
        m_instance.m_stepIndex = 0;
    }

    public static void SkipTutorial()
    {
        m_instance.m_skipped = true;
    }

    public static void SkipTutorialStep()
    {

    }
}
