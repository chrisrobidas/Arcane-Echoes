using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class VolumeMenu : MonoBehaviour
{
    [SerializeField] GameObject m_volumeMenu;

    [SerializeField] Slider masterSlider;
    public static Action<float> onMasterVolumeChanged;

    [SerializeField] Slider gameSlider;
    public static Action<float> onGameVolumeChanged;

    [SerializeField] Slider musicSlider;
    public static Action<float> onMusicVolumeChanged;

    private void OnEnable()
    {
        masterSlider.value = PlayerPrefs.GetFloat(SoundManager.masterVolumeParamName, 1);
        masterSlider.onValueChanged.AddListener((value) => onMasterVolumeChanged?.Invoke(value));

        gameSlider.value = PlayerPrefs.GetFloat(SoundManager.gameVolumeParamName, 1);
        gameSlider.onValueChanged.AddListener((value) => onGameVolumeChanged?.Invoke(value));

        musicSlider.value = PlayerPrefs.GetFloat(SoundManager.musicVolumeParamName, 1);
        musicSlider.onValueChanged.AddListener((value) => onMusicVolumeChanged?.Invoke(value));
    }

    private void OnDisable()
    {
        masterSlider.onValueChanged.RemoveAllListeners();
        gameSlider.onValueChanged.RemoveAllListeners();
        musicSlider.onValueChanged.RemoveAllListeners();
    }

    public void Open(bool open)
    {
        m_volumeMenu.SetActive(open);
        if (open) EventSystem.current.firstSelectedGameObject = masterSlider.gameObject;
    }
}
