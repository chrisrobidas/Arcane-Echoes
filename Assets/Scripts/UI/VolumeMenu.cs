using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class VolumeMenu : GameStateResponsiveMenu
{
    public override EGameState gameStateMonitored => m_gameStateMonitored;
    [SerializeField] EGameState m_gameStateMonitored;

    [SerializeField] Slider masterSlider;
    public static Action<float> onMasterVolumeChanged;

    [SerializeField] Slider gameSlider;
    public static Action<float> onGameVolumeChanged;

    [SerializeField] Slider musicSlider;
    public static Action<float> onMusicVolumeChanged;


    protected override void OnEnable()
    {
        base.OnEnable();
        masterSlider.value = PlayerPrefs.GetFloat(SoundManager.s_masterVolumeParamName, 1);
        masterSlider.onValueChanged.AddListener((value) => onMasterVolumeChanged?.Invoke(value));

        gameSlider.value = PlayerPrefs.GetFloat(SoundManager.s_gameVolumeParamName, 1);
        gameSlider.onValueChanged.AddListener((value) => onGameVolumeChanged?.Invoke(value));

        musicSlider.value = PlayerPrefs.GetFloat(SoundManager.s_musicVolumeParamName, 1);
        musicSlider.onValueChanged.AddListener((value) => onMusicVolumeChanged?.Invoke(value));
    }

    protected override void OnDisable()
    {
        base.OnEnable();
        masterSlider.onValueChanged.RemoveAllListeners();
        gameSlider.onValueChanged.RemoveAllListeners();
        musicSlider.onValueChanged.RemoveAllListeners();
    }

    public void OnBackButton()
    {
        switch (m_gameStateMonitored)
        {
            case EGameState.SettingsMainMenu:
                GameManager.CloseSettingsMainMenu();
                break;
            case EGameState.SettingsPause:
                GameManager.CloseSettingsPause();
                break;
            default:
                break;
        }
    }
}
