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

    public Sound mainMenuMusic => m_mainMenuMusic;
    [SerializeField] private Sound m_mainMenuMusic;

    public Sound gameMusic => m_gameMusic;
    [SerializeField] private Sound m_gameMusic;

    public Sound impactSound => m_impactSound;
    [SerializeField] private Sound m_impactSound;

    public Sound fireballImpactSound => m_fireballImpactSound;
    [SerializeField] private Sound m_fireballImpactSound;

    public Sound fireballSound => m_fireballSound;
    [SerializeField] private Sound m_fireballSound;

    public Sound enemySummonSound => m_enemySummonSound;
    [SerializeField] private Sound m_enemySummonSound;

    public Sound strikeSound => m_strikeSound;
    [SerializeField] private Sound m_strikeSound;

    public Sound deathSound => m_deathSound;
    [SerializeField] private Sound m_deathSound;

    public Sound pushSound => m_pushSound;
    [SerializeField] private Sound m_pushSound;

    public Sound summonSound => m_summonSound;
    [SerializeField] private Sound m_summonSound;

    public Sound portalSound => m_portalSound;
    [SerializeField] private Sound m_portalSound;
}
