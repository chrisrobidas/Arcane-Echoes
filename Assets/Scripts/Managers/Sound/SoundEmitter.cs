using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using static SoundBank;

public class SoundEmitter : MonoBehaviour
{
    private AudioSource m_source;
    private IObjectPool<SoundEmitter> m_pool;

    public void Initialize(IObjectPool<SoundEmitter> pool)
    {
        m_source = GetComponent<AudioSource>();
        m_pool = pool;
    }

    public void PlaySoundAt(Sound sound, Vector3 position, float spatializedValue = 1)
    {
        if (m_source == null) ReleaseToPool();

        transform.position = position;
        Mathf.Clamp01(spatializedValue);
        PlaySound(sound);
        m_source.spatialBlend = spatializedValue;
    }

    public void PlaySound(Sound sound)
    {
        if (m_source == null) ReleaseToPool();

        m_source.clip = sound.clip;
        m_source.volume = sound.volume;
        m_source.spatialBlend = 0;
        m_source.Play();
        Invoke(nameof(ReleaseToPool), sound.clip.length);
    }

    private void ReleaseToPool()
    {
        m_pool.Release(this);
    }
}
