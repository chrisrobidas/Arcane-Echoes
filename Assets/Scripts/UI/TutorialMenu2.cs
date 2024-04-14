using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static TutorialScript;

public class TutorialMenu2 : GameStateResponsiveMenu
{
    public override EGameState gameStateMonitored => EGameState.Tutorial;
    [SerializeField] TMP_Text m_titleText;
    private readonly string m_titlePrefix = "Tutorial : ";
    [SerializeField] TMP_Text m_descriptionText;

    protected override void OnEnable()
    {
        base.OnEnable();

        TutorialManager.StepActivation += DisplayStep;
    }

    protected override void OnDisable()
    {
        base.OnEnable();

        TutorialManager.StepActivation -= DisplayStep;
    }

    [ContextMenu("Skip tutorial")]
    public void OnSkipTutorialButton()
    {
        TutorialManager.SkipTutorial();
    }

    [ContextMenu("Skip step")]
    public void OnSkipTutorialStepButton()
    {
        TutorialManager.SkipTutorialStep();
    }

    private void DisplayStep(TutorialScriptStep step)
    {
        if (step == null) return;
        if (m_titleText != null) m_titleText.text = m_titlePrefix + step.Title;
        if (m_descriptionText != null) m_descriptionText.text = step.Description;
    }
}

