using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SceneLoadingScreen : MonoBehaviour
{
    [SerializeField] GameObject m_loadingScreen;
    [SerializeField] Image m_loadingBar;
    [SerializeField] float m_fillSpeed = 0.5f;

    bool m_isLoading;
    float m_targetProgress;

    private void OnEnable()
    {
        SceneLoader.OnSceneGroupLoadStart += () => { ShowLoadingScreen(true); };
        SceneLoader.OnSceneGroupLoadEnd += () => { ShowLoadingScreen(false); };
        SceneLoader.LoadingProgress.Progressed += UpdateProgressBar;
    }

    public void OnDisable()
    {
        SceneLoader.OnSceneGroupLoadStart -= () => { ShowLoadingScreen(true); };
        SceneLoader.OnSceneGroupLoadEnd -= () => { ShowLoadingScreen(false); };
        SceneLoader.LoadingProgress.Progressed -= UpdateProgressBar;
    }

    private void Update()
    {
        if (!m_isLoading) return;

        float currentFillAmount = m_loadingBar.fillAmount;
        float progressDiffenrece = Mathf.Abs(currentFillAmount - m_targetProgress);

        float dynamicFillSpeed = progressDiffenrece * m_fillSpeed;

        m_loadingBar.fillAmount = Mathf.Lerp(currentFillAmount, m_targetProgress, Time.deltaTime * dynamicFillSpeed);
    }


    private void ShowLoadingScreen(bool show)
    {
        m_loadingScreen.SetActive(show);
        m_loadingBar.fillAmount = 0;
        m_targetProgress = 1;
        m_isLoading = show;
    }

    private void UpdateProgressBar(float progress)
    {
        m_targetProgress = Mathf.Max(progress, m_targetProgress);
    }
}
