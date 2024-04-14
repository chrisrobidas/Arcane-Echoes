using System.Collections.Generic;
using UnityEngine;

public class EnemySummoner : MonoBehaviour
{
    [SerializeField] private List<GameObject> _enemies;
    [SerializeField] private GameObject _summonEffect;

    private bool _isSummonDone;

    private void Awake()
    {
        foreach (var enemy in _enemies) 
        {
            enemy.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_isSummonDone)
            return;

        if (other.CompareTag("Player"))
        {
            foreach (var enemy in _enemies)
            {
                enemy.SetActive(true);
                Instantiate(_summonEffect, enemy.transform.position, enemy.transform.rotation, enemy.transform);
                SoundManager.PlaySoundAt(SoundManager.SoundBank.enemySummonSound, enemy.transform.position);
            }
        }

        _isSummonDone = true;
    }
}
