using System;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Pool;
using UnityEngine.UIElements;
using static SoundBank;

public class SoundManager : MonoBehaviour
{
    private static SoundManager m_instance;
    public static SoundBank SoundBank => m_instance.soundBank;
    [SerializeField] SoundBank soundBank;

    private AudioListener m_audioListener;
    private AudioListenerAnchor m_audioListenerAnchor = null;
    private bool m_audioListenerAttached = false;

    [Header("SoundEmitter ObjectPool")]
    [SerializeField] private SoundEmitter m_soundEmitterPrefab;
    [SerializeField] private int m_initialPoolCapacity = 10;
    [SerializeField] private int m_maxPoolSize = 10;
    [SerializeField] private bool m_collectionChecks = true;
    private ObjectPool<SoundEmitter> m_pool;
    private Transform m_emitterPoolParent;

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
    [SerializeField] private float m_musicFadeSpeed = 1;

    private AudioListener m_defaultAudioListener;
    private AudioSource[] m_musicAudioSources;
    private bool m_firstActive = false;
    private bool m_isFading = false;
    private float m_fadeRatio = 0.5f;
    private float m_fadeActiveTargetVolume = 0;
    private float m_fadeInactiveBaseVolume = 0;
    private int m_activeMusicSourceIndex => m_firstActive ? 0 : 1;
    private int m_inactiveMusicSourceIndex => !m_firstActive ? 0 : 1;
    

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

        m_defaultAudioListener = GetComponentInChildren<AudioListener>();

        InitializeAudioListener();
        InitializeSoundEmitterObjectPool();
        InitializeMusicSources();
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
        SettingsMenu.onMasterVolumeChanged += SetMasterVolume;
        SettingsMenu.onGameVolumeChanged += SetGameVolume;
        SettingsMenu.onMusicVolumeChanged += SetMusicVolume;
    }

    private void OnDisable()
    {
        SettingsMenu.onMasterVolumeChanged -= SetMasterVolume;
        SettingsMenu.onGameVolumeChanged -= SetGameVolume;
        SettingsMenu.onMusicVolumeChanged -= SetMusicVolume;
    }

    private void Start()
    {
        SetMasterVolume(PlayerPrefs.GetFloat(s_masterVolumeParamName, 1));
        SetGameVolume(PlayerPrefs.GetFloat(s_gameVolumeParamName, 1));
        SetMusicVolume(PlayerPrefs.GetFloat(s_gameVolumeParamName, 1));
    }

    private void InitializeAudioListener()
    {
        GameObject audioListener = new GameObject("AudioListener");
        m_audioListener = audioListener.AddComponent<AudioListener>();
        audioListener.transform.parent = transform;
    }

    private void InitializeMusicSources()
    {
        string musicAmgName = "Music";
        m_musicAudioSources = new AudioSource[2];
        for (int i = 0; i < m_musicAudioSources.Length; i++)
        {
            AudioMixerGroup musicGroup = m_audioMixer.FindMatchingGroups(musicAmgName)[0];

            if (musicGroup == null)
            {
#if UNITY_EDITOR
                Debug.LogWarningFormat("Could not find audio mixer group named {0}", musicAmgName);
#endif
                return;
            }
            m_musicAudioSources[i] = gameObject.AddComponent<AudioSource>();
            m_musicAudioSources[i].enabled = false;
            m_musicAudioSources[i].volume = 0;
            m_musicAudioSources[i].outputAudioMixerGroup = musicGroup;
            m_musicAudioSources[i].playOnAwake = false;
            m_musicAudioSources[i].loop = true;
        }
    }
    #endregion

    private void Update()
    {
        HandleAudioListener();
        HandleMusicFading();
    }

    #region AudioListenerAnchor
    public static void AttachAudioListener(AudioListenerAnchor anchor)
    {
        m_instance?.AttachAudioListenerInternal(anchor);
    }

    private void AttachAudioListenerInternal(AudioListenerAnchor anchor)
    {
        m_audioListenerAttached = true;
        m_audioListenerAnchor = anchor; 
        m_audioListener.transform.position = m_audioListenerAnchor.Position;
        m_audioListener.transform.rotation = m_audioListenerAnchor.Rotation;
    }

    public static void DetachAudioListener()
    {
        m_instance?.DetachAudioListenerInternal();
    }

    private void DetachAudioListenerInternal()
    {
        m_audioListener.transform.position = Vector3.zero;
        m_audioListener.transform.rotation = Quaternion.identity;
        m_audioListenerAttached = false;
    }

    private void HandleAudioListener()
    {
        if (!m_audioListenerAttached)
        {
            return;
        }

        if (m_audioListenerAnchor == null)
        {
            DetachAudioListenerInternal();
            return;
        }

        if (!m_audioListenerAnchor.IsStatic)
        {
            m_audioListener.transform.position = m_audioListenerAnchor.Position;
            m_audioListener.transform.rotation = m_audioListenerAnchor.Rotation;
        }

    }
    #endregion

    #region ObjectPool
    private void InitializeSoundEmitterObjectPool()
    {
        GameObject soundEmitterPool = new GameObject("SoundEmitterPool");
        m_emitterPoolParent = soundEmitterPool.transform;
        m_emitterPoolParent.parent = transform;
        m_pool = new ObjectPool<SoundEmitter>(CreatePooledItem, OnTakeFromPool, OnReturnedToPool, OnDestroyPoolObject, m_collectionChecks, m_initialPoolCapacity, m_maxPoolSize);
    }

    // Called when an item is needed but none exists in the pool
    SoundEmitter CreatePooledItem()
    {
        SoundEmitter soundEmitter = Instantiate(m_soundEmitterPrefab, m_emitterPoolParent != null ? m_emitterPoolParent : transform);
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
    private void SetGroupVolume(string parameterName, float normalizedVolume)
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

    private void SetMasterVolume(float value)
    {
        m_masterVolume = value;
        PlayerPrefs.SetFloat(s_masterVolumeParamName, m_masterVolume);
        SetGroupVolume(s_masterVolumeParamName, m_masterVolume);
    }

    private void SetGameVolume(float value)
    {
        m_gameVolume = value;
        PlayerPrefs.SetFloat(s_gameVolumeParamName, m_gameVolume);
        SetGroupVolume(s_gameVolumeParamName, m_gameVolume);
    }

    private void SetMusicVolume(float value)
    {
        m_musicVolume = value;
        PlayerPrefs.SetFloat(s_musicVolumeParamName, m_musicVolume);
        SetGroupVolume(s_musicVolumeParamName, m_musicVolume);
    }
    #endregion

    #region SoundPlay
    public static void PlaySoundAt(Sound sound, Vector3 position)
    {
        SoundEmitter soundEmitter = m_instance.m_pool.Get();
        soundEmitter.PlaySoundAt(sound, position, m_instance.m_customSpacializationValue);
    }

    public static void PlaySound(Sound sound)
    {
        SoundEmitter soundEmitter = m_instance.m_pool.Get();
        soundEmitter.PlaySound(sound);
    }

    [ContextMenu("Test0")]
    public void Test0()
    {
        PlaySoundAt(SoundBank.deathSound, Vector3.zero);
    }

    [ContextMenu("Test1")]
    public void Test()
    {
        PlaySoundAt(SoundBank.deathSound, Vector3.right * 50);
    }

    public static void PlayMusic(Sound audioClip)
    {
        m_instance?.PlayMusicInternal(audioClip);
    }

    private void PlayMusicInternal(Sound sound)
    {
        m_isFading = true;
        m_firstActive = !m_firstActive;
        m_fadeActiveTargetVolume = sound.volume;
        m_fadeInactiveBaseVolume = m_musicAudioSources[m_inactiveMusicSourceIndex].volume;
        EnableMusicSource(m_activeMusicSourceIndex, sound.clip);
    }

    #region MusicFading
    private void EnableMusicSource(int index, AudioClip clip)
    {
        if (m_musicAudioSources[index] != null)
        {
            m_musicAudioSources[index].enabled = true;
            m_musicAudioSources[index].clip = clip;
            m_musicAudioSources[index].volume = 0;
            m_musicAudioSources[index].Play();
        }
    }

    private void HandleMusicFading()
    {
        if (m_isFading)
        {
            if (m_firstActive ? m_fadeRatio > 0 : m_fadeRatio < 1)
            {
                m_fadeRatio += (m_firstActive ? -1 : 1) * Time.deltaTime * m_musicFadeSpeed;
                m_musicAudioSources[m_activeMusicSourceIndex].volume = Mathf.Lerp(0, m_fadeActiveTargetVolume, (m_firstActive ? 1 - m_fadeRatio : m_fadeRatio));
                m_musicAudioSources[m_inactiveMusicSourceIndex].volume = Mathf.Lerp(m_fadeInactiveBaseVolume, 0, m_fadeRatio);
            }
            else
            {
                m_isFading = false;
                m_fadeRatio = m_activeMusicSourceIndex;
                DisableMusicSource(m_inactiveMusicSourceIndex);
            }
        }
    }

    private void DisableMusicSource(int index)
    {
        if (m_musicAudioSources[index] != null)
        {
            m_musicAudioSources[index].Stop();
            m_musicAudioSources[index].volume = 0;
            m_musicAudioSources[index].clip = null;
            m_musicAudioSources[index].enabled = false;
        }
    }
    #endregion

    #endregion
}
