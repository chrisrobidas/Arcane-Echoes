using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ObjectSummoning : MonoBehaviour
{
    // Summon parameters
    [Header("Summon parameters")]
    [SerializeField]
    private LayerMask summonableLayer;
    [SerializeField, Range(0f, 100f)]
    private float maxSummonableDistance;
    [SerializeField]
    private Transform summonPoint;
    [SerializeField, Range(10f, 5000f)]
    private float projectionForce;

    private PlayerInputsAction playerInputsAction;
    private GameObject highLightedObject;
    private GameObject summonedObject;

    // Start is called before the first frame update
    void Start()
    {
        playerInputsAction = new PlayerInputsAction();
        playerInputsAction.PlayerSummoning.Enable();
        playerInputsAction.PlayerSummoning.Summon.performed += SummonObject;
        playerInputsAction.PlayerSummoning.Project.performed += ProjectSummonedObject;
    }

    // Update is called once per frame
    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit hit;

        if (summonedObject == null)
        {
            if (Physics.Raycast(ray, out hit, maxSummonableDistance, summonableLayer))
            {
                if (hit.transform.gameObject != highLightedObject)
                {
                    if (highLightedObject != null)
                    {
                        highLightedObject.GetComponent<HighlightObject>().NotInVision();
                    }
                    highLightedObject = hit.transform.gameObject;
                    hit.transform.gameObject.GetComponent<HighlightObject>().Highlight();
                }
            }
            else
            {
                if (highLightedObject != null)
                {
                    highLightedObject.GetComponent<HighlightObject>().NotInVision();
                    highLightedObject = null;
                }
            }
        }
        else
        {
            if (playerInputsAction.PlayerSummoning.Summon.WasReleasedThisFrame())
            {
                DropSummonedObject();
            }
        }
    }

    private void SummonObject(InputAction.CallbackContext context)
    {
        if (highLightedObject != null)
        {
            highLightedObject.GetComponent<HighlightObject>().NotInVision();
            summonedObject = Instantiate(highLightedObject, summonPoint.position, highLightedObject.transform.rotation);
            summonedObject.GetComponent<Rigidbody>().isKinematic = true;
            summonedObject.GetComponent<Collider>().enabled = false;
            summonedObject.transform.SetParent(gameObject.transform);
            highLightedObject = null;
        }        
    }

    private void ProjectSummonedObject(InputAction.CallbackContext context)
    {
        if (summonedObject != null)
        {
            summonedObject.transform.SetParent(null);
            summonedObject.GetComponent<Collider>().enabled = true;
            Rigidbody rb = summonedObject.GetComponent<Rigidbody>();
            rb.isKinematic = false;
            rb.AddForce(transform.forward * projectionForce);
            summonedObject = null;
        }        
    }

    private void DropSummonedObject()
    {
        if (summonedObject != null)
        {
            summonedObject.transform.SetParent(null);
            summonedObject.GetComponent<Collider>().enabled = true;
            Rigidbody rb = summonedObject.GetComponent<Rigidbody>();
            rb.isKinematic = false;
            summonedObject = null;
        }        
    }

    private void OnDestroy()
    {        
        playerInputsAction.PlayerSummoning.Summon.performed -= SummonObject;
        playerInputsAction.PlayerSummoning.Project.performed -= ProjectSummonedObject;
        playerInputsAction.PlayerSummoning.Disable();
    }
}
