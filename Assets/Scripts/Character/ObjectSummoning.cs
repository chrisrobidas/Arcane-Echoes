using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ObjectSummoning : MonoBehaviour
{
    // Summon tunables
    [Header("Summon tunables")]
    [SerializeField, Range(0f, 100f)]
    private float m_maxSummonableDistance;
    [SerializeField, Range(1f, 10f)]
    private float m_SummonCoolDown;
    [SerializeField, Range(10f, 5000f)]
    private float m_projectionForce;
    [SerializeField, Range(10f, 100f)]
    private float m_rotationSpeed;
    [SerializeField, Range(1f, 5f)]
    private float m_translationSpeed;
    [Space]
    // End Summon tunables

    // Summon Setup
    [Header("Summon Setup")]
    [SerializeField]
    private LayerMask m_summonableLayer;
    [SerializeField]
    private Transform m_objectSummonPoint;
    [SerializeField]
    private Transform m_objectHoldPoint;
    [SerializeField, Range(-2f, 2f)]
    private float m_sPointForwardDistance;
    [Space]
    // End Summon setup

    // Effects
    [Header("Effects")]
    [SerializeField]
    private GameObject m_invocationCircle;
    [SerializeField]
    private GameObject m_leftHandEffect;
    [SerializeField]
    private GameObject m_rightHandEffect;
    [SerializeField]
    private Transform m_summonCircleEmplacement;
    private Animator playerAnimator;
    // End Effects

    private PlayerInputsAction m_playerInputsAction;
    private GameObject m_highlightedObject;
    private GameObject m_summonedObject;
    private GameObject m_currentSummonEffect;
    private float m_summonTimer;

    void Start()
    {
        playerAnimator = GetComponentInChildren<Animator>();
        m_playerInputsAction = new PlayerInputsAction();
        m_playerInputsAction.PlayerSummoning.Enable();
        m_playerInputsAction.PlayerSummoning.Summon.performed += SummonObject;
        m_playerInputsAction.PlayerSummoning.Project.performed += ProjectSummonedObject;
        m_playerInputsAction.PlayerSummoning.Delete.performed += DeleteObject;

        m_objectHoldPoint.Translate(m_objectHoldPoint.forward * m_sPointForwardDistance);
    }

    void Update()
    {
        if (m_summonTimer <= 0f & m_summonedObject == null)
        {
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, m_maxSummonableDistance, m_summonableLayer))
            {
                if (hit.transform.gameObject != m_highlightedObject)
                {
                    if (m_highlightedObject != null)
                    {
                        m_highlightedObject.GetComponent<SummonableObject>().OnMouseHooverExit();
                    }
                    m_highlightedObject = hit.transform.gameObject;
                    hit.transform.gameObject.GetComponent<SummonableObject>().OnMouseHooverEnter();
                }
            }
            else
            {
                if (m_highlightedObject != null)
                {
                    m_highlightedObject.GetComponent<SummonableObject>().OnMouseHooverExit();
                    m_highlightedObject = null;
                }
            }
        }
        else if (m_summonedObject != null)
        {
            float rotationX = m_playerInputsAction.PlayerSummoning.RotateX.ReadValue<float>();
            float rotationY = m_playerInputsAction.PlayerSummoning.RotateY.ReadValue<float>();
            float rotationZ = m_playerInputsAction.PlayerSummoning.RotateZ.ReadValue<float>();

            float translation = m_playerInputsAction.PlayerSummoning.MoveZ.ReadValue<float>();

            if (rotationX != 0f)
            {
                m_summonedObject.transform.Rotate(rotationX * m_rotationSpeed * Time.deltaTime, 0f, 0f);
            }
            if (rotationY != 0f)
            {
                m_summonedObject.transform.Rotate(0f, rotationY * m_rotationSpeed * Time.deltaTime, 0f);
            }
            if (rotationZ != 0f)
            {
                m_summonedObject.transform.Rotate(0f, 0f, rotationZ * m_rotationSpeed * Time.deltaTime);
            }

            if (translation != 0f)
            {
                m_summonedObject.transform.Translate(Vector3.forward * translation * m_translationSpeed * Time.deltaTime, transform);
            }

            if (m_playerInputsAction.PlayerSummoning.Summon.WasReleasedThisFrame() & m_summonedObject.GetComponent<SummonableObject>().IsInstanciable)
            {
                DropSummonedObject();
            }
        }
        else
        {
            m_summonTimer -= Time.deltaTime;
        }
    }

    private void DeleteObject(InputAction.CallbackContext context)
    {
        if (m_summonedObject != null)
        {
            Destroy(m_summonedObject);
            m_summonedObject = null;
        }
    }

    private void SummonObject(InputAction.CallbackContext context)
    {
        if (m_highlightedObject != null & m_summonTimer <= 0f)
        {
            playerAnimator.SetTrigger("HoldTrigger");
            m_highlightedObject.GetComponent<SummonableObject>().OnMouseHooverExit();
            Vector3 l_scale = m_highlightedObject.transform.localScale;
            m_currentSummonEffect = Instantiate(m_invocationCircle, m_summonCircleEmplacement.position, Quaternion.identity, null);
            m_summonedObject = Instantiate(m_highlightedObject, m_objectSummonPoint.position, m_highlightedObject.transform.rotation);
            m_highlightedObject = null;

            m_summonedObject.GetComponent<Rigidbody>().isKinematic = true;
            m_summonedObject.GetComponent<Collider>().enabled = false;
            m_summonedObject.GetComponent<SummonableObject>().m_playerObjectSummoning = this;
            m_summonedObject.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            m_summonedObject.transform.SetParent(m_objectHoldPoint);
            LeanTween.moveLocal(m_summonedObject, m_objectHoldPoint.localPosition, 1f).setEase(LeanTweenType.easeOutElastic).setOnComplete(AnimationOver);
            LeanTween.scale(m_summonedObject, l_scale, 1f).setEase(LeanTweenType.easeInOutBounce);
            m_summonTimer += m_SummonCoolDown;
        }        
    }

    private void ProjectSummonedObject(InputAction.CallbackContext context)
    {
        if (m_summonedObject != null & m_summonedObject.GetComponent<SummonableObject>().IsInstanciable)
        {
            playerAnimator.SetTrigger("ProjectTrigger");
            m_summonedObject.GetComponent<SummonableObject>().OnDrop();
            Rigidbody rb = m_summonedObject.GetComponent<Rigidbody>();
            rb.AddForce(m_objectHoldPoint.forward * m_projectionForce);
            m_summonedObject = null;
        }        
    }

    private void DropSummonedObject()
    {
        if (m_summonedObject != null)
        {
            playerAnimator.SetTrigger("DropTrigger");
            m_summonedObject.GetComponent<SummonableObject>().OnDrop();
            m_summonedObject = null;
        }        
    }

    private void AnimationOver()
    {
        m_summonedObject.GetComponent<SummonableObject>().IsSummoned = true;
        Destroy(m_currentSummonEffect);
    }

    public void SummonedObjectDestroyed()
    {
        playerAnimator.SetTrigger("ResetTrigger");
        m_summonedObject = null;
    }

    private void OnDestroy()
    {        
        m_playerInputsAction.PlayerSummoning.Summon.performed -= SummonObject;
        m_playerInputsAction.PlayerSummoning.Project.performed -= ProjectSummonedObject;
        m_playerInputsAction.PlayerSummoning.Disable();
    }
}
