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
    [SerializeField] private AudioMixer m_audioMixer = default;
    [SerializeField] [Range(0,1)]private float m_customSpacializationValue = 1f;

    [Range(0.0001f, 1f)]
    [SerializeField] private float m_masterVolume = 1f;
    public static readonly string s_masterVolumeParamName = "MasterVolume";
    [Range(0.0001f, 1f)]
    [SerializeField] private float m_gameVolume = 1f;
    public static readonly string s_gameVolumeParamName = "GameVolume";
    [Range(0.0001f, 1f)]
    [SerializeField] private float m_musicVolume = 1f;
    public static readonly string s_musicVolumeParamName = "MusicVolume";

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

        InitializeObjectPool();
    }

    void OnValidate()
    {
        if (Application.isPlaying)
        {
            SetGroupVolume(s_masterVolumeParamName, m_masterVolume);
            SetGroupVolume(s_gameVolumeParamName, m_gameVolume);
            SetGroupVolume(s_musicVolumeParamName, m_musicVolume);
        }
    }

    private void OnEnable()
    {
        VolumeMenu.onMasterVolumeChanged += SetMasterVolume;
        VolumeMenu.onGameVolumeChanged += SetGameVolume;
        VolumeMenu.onMusicVolumeChanged += SetMusicVolume;
    }

    private void OnDisable()
    {
        VolumeMenu.onMasterVolumeChanged -= SetMasterVolume;
        VolumeMenu.onGameVolumeChanged -= SetGameVolume;
        VolumeMenu.onMusicVolumeChanged -= SetMusicVolume;
    }

    private void Start()
    {
        SetMasterVolume(PlayerPrefs.GetFloat(s_masterVolumeParamName, 1));
        SetGameVolume(PlayerPrefs.GetFloat(s_gameVolumeParamName, 1));
        SetMusicVolume(PlayerPrefs.GetFloat(s_gameVolumeParamName, 1));
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
        if (!m_audioMixer.SetFloat(parameterName, NormalizedToMixerValue(normalizedVolume)))
        {
#if UNITY_EDITOR
            Debug.LogError("The AudioMixer parameter was not found");
#endif
        }
    }

    private float NormalizedToMixerValue(float normalizedValue)
    {
        normalizedValue = normalizedValue > 0 ? normalizedValue : 0.0001f;
        return Mathf.Log10(normalizedValue) * 20;
    }

    void SetMasterVolume(float value)
    {
        m_masterVolume = value;
        PlayerPrefs.SetFloat(s_masterVolumeParamName, m_masterVolume);
        SetGroupVolume(s_masterVolumeParamName, m_masterVolume);
    }

    void SetGameVolume(float value)
    {
        m_gameVolume = value;
        PlayerPrefs.SetFloat(s_gameVolumeParamName, m_gameVolume);
        SetGroupVolume(s_gameVolumeParamName, m_gameVolume);
    }

    void SetMusicVolume(float value)
    {
        m_musicVolume = value;
        PlayerPrefs.SetFloat(s_musicVolumeParamName, m_musicVolume);
        SetGroupVolume(s_musicVolumeParamName, m_musicVolume);
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
