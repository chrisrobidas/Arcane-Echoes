using System;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    private static TutorialManager m_instance;
    [SerializeField] private TutorialScript m_script;
    [SerializeField] private int m_stepIndex = -1;
    private TutorialTrigger m_currentTrigger;

    public static event Action<TutorialScript.TutorialScriptStep> StepActivation;

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

    private void OnEnable()
    {
        GameManager.GameStateMachine.OnStateEnter += OnGameStateEnter;
        GameManager.GameStateMachine.OnStateExit += OnGameStateExit;
    }

    private void OnDisable()
    {
        GameManager.GameStateMachine.OnStateEnter -= OnGameStateEnter;
        GameManager.GameStateMachine.OnStateExit -= OnGameStateExit;
    }
    private void OnGameStateEnter(EGameState state)
    {
        switch (state)
        {
            default:
                break;
            case EGameState.Tutorial:
                Debug.Log($"<b>[TutorialManager]</b> Initialization");
                //Initialize
                m_stepIndex = 0;
                //Enter waiting trigger state
                m_currentTrigger = m_script.Steps[m_stepIndex].tutorialTrigger;
                m_currentTrigger.Triggered += OnTutorialTriggered;

                break;
            case EGameState.TutorialFreeze:
                break;
        }
    }

    private void OnGameStateExit(EGameState state)
    {
        switch (state)
        {
            default:
                break;
            case EGameState.Tutorial:
                //Hide tutorial
                break;
            case EGameState.TutorialFreeze:
                //Hide tutorial
                break;
        }
    }

    private void OnTutorialTriggered()
    {
        StepActivation?.Invoke(m_script.Steps[m_stepIndex]);
    }

    public static void SkipTutorial()
    {
        Debug.Log($"<b>[TutorialManager]</b> Tutorial skipped");
        GameManager.EnableTutorial(false);
    }

    public static void SkipTutorialStep()
    {
        m_instance.SkipStep();
    }

    private void SkipStep()
    {
        m_currentTrigger.Triggered -= OnTutorialTriggered;
        m_stepIndex++;
        if (m_stepIndex < m_script.Steps.Count)
        {
            m_currentTrigger = m_script.Steps[m_stepIndex].tutorialTrigger;
            m_currentTrigger.Triggered += OnTutorialTriggered;
        }
        else
        {
            Debug.Log($"<b>[TutorialManager]</b> End of tutorial");
            GameManager.EnableTutorial(false);
        }
    }

}
