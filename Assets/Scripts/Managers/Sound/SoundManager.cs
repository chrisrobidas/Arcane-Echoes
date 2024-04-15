using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Pool;

public class SoundManager : MonoBehaviour
{
    public static SoundBank SoundBank => m_instance.soundBank;
    [SerializeField] SoundBank soundBank;

    private AudioSource m_musicAudioSource;

    private static SoundManager m_instance;

    [Header("SoundEmitter ObjectPool")]
    [SerializeField] private SoundEmitter m_soundEmitterPrefab;
    [SerializeField] private int m_initialPoolCapacity = 10;
    [SerializeField] private int m_maxPoolSize = 10;
    [SerializeField] private bool m_collectionChecks = true;
    private ObjectPool<SoundEmitter> m_pool;

    [Header("Audio control")]
    [SerializeField] private AudioMixer audioMixer = default;
    [SerializeField] [Range(0,1)]private float m_customSpacializationValue = 1f;

    [Range(0.0001f, 1f)]
    [SerializeField] private float m_masterVolume = 1f;
    public static readonly string masterVolumeParamName = "MasterVolume";
    [Range(0.0001f, 1f)]
    [SerializeField] private float m_gameVolume = 1f;
    public static readonly string gameVolumeParamName = "GameVolume";
    [Range(0.0001f, 1f)]
    [SerializeField] private float m_musicVolume = 1f;
    public static readonly string musicVolumeParamName = "MusicVolume";

    #region Initialization
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
        m_musicAudioSource = GetComponent<AudioSource>();

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
    #endregion

    #region ObjectPool
    private void InitializeObjectPool()
    {
        m_pool = new ObjectPool<SoundEmitter>(CreatePooledItem, OnTakeFromPool, OnReturnedToPool, OnDestroyPoolObject, m_collectionChecks, m_initialPoolCapacity, m_maxPoolSize);
    }

    // Called when an item is needed but none exists in the pool
    SoundEmitter CreatePooledItem()
    {
        SoundEmitter soundEmitter = Instantiate(m_soundEmitterPrefab, transform);
        soundEmitter.Initialize(m_pool);
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

    #region VolumeManagement
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
    #endregion

    #region SoundPlay

    public static void PlaySoundAt(SoundBank.Sound sound, Vector3 position)
    {
        SoundEmitter soundEmitter = m_instance.m_pool.Get();
        soundEmitter.PlaySoundAt(sound, position, m_instance.m_customSpacializationValue);
    }

    public static void PlaySound(SoundBank.Sound sound)
    {
        SoundEmitter soundEmitter = m_instance.m_pool.Get();
        soundEmitter.PlaySound(sound);
    }

    public static void PlayMusic(SoundBank.Sound audioClip)
    {
        m_instance.m_musicAudioSource.clip = audioClip.clip;
        m_instance.m_musicAudioSource.volume = audioClip.volume;
        m_instance.m_musicAudioSource.loop = true;
        m_instance.m_musicAudioSource.Play();
    }   

    public void StopCurrent()
    {
        m_musicAudioSource.Stop();
    }
    #endregion

#if UNITY_EDITOR
    [ContextMenu("Test sound")]
    public async void TestSound()
    {
        PlaySound(soundBank.impactSound);
        await Task.Delay(100);
        PlaySoundAt(soundBank.impactSound, transform.position + Vector3.right);
        await Task.Delay(1000);
        PlaySoundAt(soundBank.impactSound, transform.position + Vector3.left * 100);
        //PlaySound(GetComponent<AudioSource>(), soundBank.swapColorSound);
    }
#endif
}
