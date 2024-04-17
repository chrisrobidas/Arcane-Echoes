using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class SceneLoadingScreen : MonoBehaviour
{
    [SerializeField] CanvasGroup m_loadingScreen;
    [SerializeField] Image m_loadingBar;
    [SerializeField] float m_fillSpeed = 0.5f;
    [SerializeField] float m_fadeInOutDuration = 0.5f;
    public static float FadeInOutDuration { get; private set; } = 0;

    bool m_isLoading;
    float m_targetProgress;

    private void Awake()
    {
        FadeInOutDuration = m_fadeInOutDuration;
    }

    private void OnEnable()
    {
        SceneLoader.FadeInOutScreen += FadeInOut;
        SceneLoader.LoadingProgress.Progressed += UpdateProgressBar;
    }

    public void OnDisable()
    {
        SceneLoader.FadeInOutScreen -= FadeInOut;
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
        m_loadingBar.fillAmount = 0;
        m_targetProgress = 1;
        m_isLoading = show;
    }

    private void UpdateProgressBar(float progress)
    {
        m_targetProgress = Mathf.Max(progress, m_targetProgress);
    }

    private void FadeInOut(bool fadeIn)
    {
        m_isLoading = fadeIn;
        if (fadeIn)
        {
            m_loadingScreen.gameObject.SetActive(true);
            LeanTween.alphaCanvas(m_loadingScreen, 1, m_fadeInOutDuration);
        }
        else
        {
            LeanTween.alphaCanvas(m_loadingScreen, 0, m_fadeInOutDuration).setOnComplete(() => { m_loadingScreen.gameObject.SetActive(false); });
        }
    }
}
