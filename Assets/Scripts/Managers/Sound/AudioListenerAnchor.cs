using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioListenerAnchor : MonoBehaviour
{
    public bool IsStatic => m_isStatic;
    [SerializeField] bool m_isStatic = false;
    public Vector3 Position => transform.position;
    public Quaternion Rotation => transform.rotation;

    void Start()
    {
        SoundManager.AttachAudioListener(this);
    }
}
