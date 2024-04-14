using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SummonableObject : MonoBehaviour
{
    // Outline colors scheme
    [Header("Outline Colors scheme")]
    [SerializeField]
    private Color m_defaultColor = Color.black;
    [SerializeField]
    private Color m_highlightedColor = Color.yellow;
    [SerializeField]
    private Color m_canBeSummonedColor = Color.green;
    [SerializeField]
    private Color m_canNOTBeSummonedColor = Color.red;
    [Space]
    // End outline colors scheme

    // Other
    [SerializeField, Range(30f, 300f)]
    private float m_cloneLifeTime = 60f;
    // End Others

    // Internal
    public bool IsInstanciable => m_isInstanciable;
    private bool m_isInstanciable;
    public bool IsSummoned { get { return m_isSummoned; } set { m_isSummoned = value; } }
    private bool m_isSummoned = false;
    
    private int m_collidesWithCounter = 0;
    private bool deathBed;

    // Update is called once per frame
    void Update()
    {
        if (m_isSummoned)
        {
            if (m_collidesWithCounter > 0)
            {
                m_isInstanciable = false;
            }
            else
            {
                m_isInstanciable = true;
            }
            SetOutlineColor(m_isInstanciable);
        }
        if (deathBed)
        {
            m_cloneLifeTime -= Time.deltaTime;
            if (m_cloneLifeTime <= 0f) { Destroy(gameObject); }
        }
    }

    public void OnMouseHooverEnter()
    {
        foreach (Material material in GetComponent<Renderer>().materials)
        {
            if (material.HasProperty("_OutlineColor"))
            {
                material.SetColor("_OutlineColor", m_highlightedColor);
            }
        }
    }

    public void OnMouseHooverExit()
    {
        foreach (Material material in GetComponent<Renderer>().materials)
        {
            if (material.HasProperty("_OutlineColor"))
            {
                material.SetColor("_OutlineColor", m_defaultColor);
            }
        }
    }

    public void OnDrop()
    {
        m_isSummoned = false;
        transform.SetParent(null);
        gameObject.GetComponent<Collider>().enabled = true;
        gameObject.GetComponent<Rigidbody>().isKinematic = false;
        foreach (Material material in GetComponent<Renderer>().materials)
        {
            if (material.HasProperty("_OutlineColor"))
            {
                material.SetColor("_OutlineColor", m_defaultColor);
            }
        }
        deathBed = true;
    }

    private void SetOutlineColor(bool canBeSummoned)
    {
        foreach (Material material in GetComponent<Renderer>().materials)
        {
            if (material.HasProperty("_OutlineColor"))
            {
                if (canBeSummoned)
                {
                    material.SetColor("_OutlineColor", m_canBeSummonedColor);
                    return;
                }
                material.SetColor("_OutlineColor", m_canNOTBeSummonedColor);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        m_collidesWithCounter += 1;
    }

    private void OnTriggerExit(Collider other)
    {
        m_collidesWithCounter -= 1;
    }
}
