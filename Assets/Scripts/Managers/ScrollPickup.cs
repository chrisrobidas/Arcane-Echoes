using UnityEngine;

public class ScrollPickup : MonoBehaviour
{
    [SerializeField] private GameObject _endPortal;

    private void Awake()
    {
        _endPortal.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SoundManager.PlaySound(SoundManager.SoundBank.portalSound);
            _endPortal.SetActive(true);
            gameObject.SetActive(false);
        }
    }
}
