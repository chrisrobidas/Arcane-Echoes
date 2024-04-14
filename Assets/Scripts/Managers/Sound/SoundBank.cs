using System;
using UnityEngine;
using UnityEngine.Audio;


[Serializable]
public class SoundBank
{
    [Serializable]
    public class Sound
    {
        public AudioClip clip => _clip;
        [SerializeField] AudioClip _clip;
        public float volume => m_volume;
        [SerializeField][Range(0, 1)] float m_volume = 1;
    }
    public Sound impactSound => m_impactSound;
    [SerializeField] private Sound m_impactSound;
}
