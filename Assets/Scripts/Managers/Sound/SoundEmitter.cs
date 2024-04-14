using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEmitter : MonoBehaviour
{
    AudioSource m_source;

    public void Prepare()
    {
        m_source = GetComponent<AudioSource>();
    }

    public void Initialize(SoundBank.Sound sound)
    {
        if (m_source == null) return;
        m_source.clip = sound.clip;
        m_source.volume = sound.volume;
        m_source.outputAudioMixerGroup = sound.mixerGroup;
        float returnTime = sound.clip.length;
    }
}
