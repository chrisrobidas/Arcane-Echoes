using System;
using UnityEngine;
using UnityEngine.Audio;
using static UnityEngine.Rendering.DebugUI;

[Serializable]
public class SoundBank
{
    [Serializable]
    public class Sound
    {
        public AudioClip clip => _clip;
        [SerializeField] AudioClip _clip;
        public AudioMixerGroup mixerGroup => _mixerGroup;
        [SerializeField] AudioMixerGroup _mixerGroup;
    }
    public Sound swapColorSound => _swapColorSound;
    [SerializeField] private Sound _swapColorSound;
    public Sound lockedColorSound => _lockedColorSound;
    [SerializeField] private Sound _lockedColorSound;
    public Sound translationSound => _translationSound;
    [SerializeField] private Sound _translationSound;

    public Sound endlevelSound => _endlevelSound;
    [SerializeField] private Sound _endlevelSound;
}

public class SoundManager : MonoBehaviour
{
    public SoundBank soundBank;
    private AudioSource defaultAudioSource;

    private static SoundManager m_instance;
    public static SoundManager Instance { get { return m_instance; } }

    [Header("Audio control")]
    [SerializeField] private AudioMixer audioMixer = default;

    [Range(0.0001f, 1f)]
    [SerializeField] private float m_masterVolume = 1f;

    public static readonly string masterVolumeParamName = "MasterVolume";
    [Range(0.0001f, 1f)]
    [SerializeField] private float m_gameVolume = 1f;
    public static readonly string gameVolumeParamName = "GameVolume";
    [Range(0.0001f, 1f)]
    [SerializeField] private float m_musicVolume = 1f;
    public static readonly string musicVolumeParamName = "MusicVolume";

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
        defaultAudioSource = GetComponent<AudioSource>();

        m_masterVolume = PlayerPrefs.GetFloat(masterVolumeParamName);
        m_gameVolume = PlayerPrefs.GetFloat(gameVolumeParamName);
        m_musicVolume = PlayerPrefs.GetFloat(musicVolumeParamName);
    }

    void OnValidate()
    {
        if (Application.isPlaying)
        {
            SetGroupVolume(masterVolumeParamName, m_masterVolume);
            SetGroupVolume(gameVolumeParamName, m_gameVolume);
            SetGroupVolume(musicVolumeParamName, m_musicVolume);
        }
    }

    private void OnEnable()
    {
        VolumeMenu.onMasterVolumeChanged += SetMasterVolume;
        VolumeMenu.onGameVolumeChanged += SetEffectsVolume;
        VolumeMenu.onMusicVolumeChanged += SetMusicVolume;
    }

    private void OnDisable()
    {
        VolumeMenu.onMasterVolumeChanged -= SetMasterVolume;
        VolumeMenu.onGameVolumeChanged -= SetEffectsVolume;
        VolumeMenu.onMusicVolumeChanged -= SetMusicVolume;
    }

    public void SetGroupVolume(string parameterName, float normalizedVolume)
    {
        bool volumeSet = audioMixer.SetFloat(parameterName, NormalizedToMixerValue(normalizedVolume));
        if (!volumeSet)
        {
#if UNITY_EDITOR
            Debug.LogError("The AudioMixer parameter was not found");
#endif
        }
    }

    private float NormalizedToMixerValue(float normalizedValue)
    {
        //return (normalizedValue - 1f) * 80f;
        return Mathf.Log10(normalizedValue) * 20;
    }

    void SetMasterVolume(float value)
    {
        Debug.Log(value);
        m_masterVolume = value;
        PlayerPrefs.SetFloat(masterVolumeParamName, m_masterVolume);
        SetGroupVolume(masterVolumeParamName, m_masterVolume);
    }

    void SetEffectsVolume(float value)
    {
        m_gameVolume = value;
        PlayerPrefs.SetFloat(gameVolumeParamName, m_gameVolume);
        SetGroupVolume(gameVolumeParamName, m_gameVolume);
    }

    void SetMusicVolume(float value)
    {
        m_musicVolume = value;
        PlayerPrefs.SetFloat(musicVolumeParamName, m_musicVolume);
        SetGroupVolume(musicVolumeParamName, m_musicVolume);
    }


    void PlaySound(AudioSource source, SoundBank.Sound sound)
    {
        source.clip = sound.clip;
        source.outputAudioMixerGroup = sound.mixerGroup;
        source.Play();
    }

    public void StopCurrent()
    {
        defaultAudioSource.Stop();
    }

    public void SetVolume(float volume)
    {
        defaultAudioSource.volume = volume;
    }
}
