using System;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Pool;

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
        public float volume => m_volume;
        [SerializeField] [Range(0,1)]float m_volume = 1;
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

    [Header("SoundEmitter ObjectPool")]
    [SerializeField] private SoundEmitter m_soundEmitterPrefab;
    [SerializeField] private int m_initialPoolCapacity = 10;
    [SerializeField] private int m_maxPoolSize = 10;
    [SerializeField] private bool m_collectionChecks = true;
    private ObjectPool<SoundEmitter> m_pool;

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

        InitializeObjectPool();
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


    [ContextMenu("Test sound")]
    public void TestSound()
    {
        PlaySound(GetComponent<AudioSource>(), soundBank.swapColorSound);
    }

    #region ObjectPool
    private void InitializeObjectPool()
    {
        m_pool = new ObjectPool<SoundEmitter>(CreatePooledItem, OnTakeFromPool, OnReturnedToPool, OnDestroyPoolObject, m_collectionChecks, m_initialPoolCapacity, m_maxPoolSize);
    }

    SoundEmitter CreatePooledItem()
    {
        SoundEmitter soundEmitter = Instantiate(m_soundEmitterPrefab, transform);
        soundEmitter.Prepare();
        return soundEmitter;
    }

    // Called when an item is returned to the pool using Release
    void OnReturnedToPool(SoundEmitter soundEmitter)
    {
        soundEmitter.gameObject.SetActive(false);
    }

    // Called when an item is taken from the pool using Get
    void OnTakeFromPool(SoundEmitter soundEmitter)
    {
        soundEmitter.gameObject.SetActive(true);
    }

    // If the pool capacity is reached then any items returned will be destroyed.
    // We can control what the destroy behavior does, here we destroy the GameObject.
    void OnDestroyPoolObject(SoundEmitter soundEmitter)
    {
        Destroy(soundEmitter.gameObject);
    }
    #endregion


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
        source.volume = sound.volume;
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
