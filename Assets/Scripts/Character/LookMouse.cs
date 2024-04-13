using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class LookMouse : MonoBehaviour
{
    [SerializeField]
    private Transform m_playerBody;
    public Transform Playerbody => m_playerBody;

    [SerializeField]
    [Range(1f, 100f)]
    private float m_mouseSensitivity;
    public float MouseSensitivity => m_mouseSensitivity;

    private PlayerInputsAction playerInputsAction;

    private float camXRotation = 0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        playerInputsAction = new PlayerInputsAction();
    }

    void Update()
    {

        float mouseX = Mouse.current.delta.ReadValue().x * m_mouseSensitivity * Time.deltaTime;
        float mouseY = Mouse.current.delta.ReadValue().y * m_mouseSensitivity * Time.deltaTime;

        camXRotation -= mouseY;
        camXRotation = Mathf.Clamp(camXRotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(camXRotation, 0f, 0f);
        m_playerBody.Rotate(Vector3.up * mouseX);
    }
}
