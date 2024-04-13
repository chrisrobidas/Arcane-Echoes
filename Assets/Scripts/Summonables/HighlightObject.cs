using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighlightObject : MonoBehaviour
{
    public Material baseMaterial;
    public Material highlightedMaterial;

    public void Highlight()
    {
        GetComponent<Renderer>().material = highlightedMaterial;
    }

    public void NotInVision()
    {
        GetComponent<Renderer>().material = baseMaterial;
    }
}
