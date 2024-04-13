using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSummoning : MonoBehaviour
{
    [Header("Summon parameters")]
    [SerializeField]
    private LayerMask summonableLayer;
    [SerializeField, Range(0f, 100f)]
    private float maxSummonableDistance;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
