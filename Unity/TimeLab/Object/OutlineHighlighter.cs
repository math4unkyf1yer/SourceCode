using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutlineHighlighter : MonoBehaviour
{
    private Renderer rend;
    private Material originalMaterial;
    public Material brightYellow;
    public Material brightPurple;
    private Outline outlineScript;
  //  public Material highlightMaterial;

    void Start()
    {
        outlineScript = GetComponent<Outline>();
        rend = GetComponent<Renderer>();
        if (rend != null)
        {
            originalMaterial = rend.material;
        }
    }

    public void Highlight()
    {
        if (!outlineScript.enabled)
        {
            outlineScript.enabled = true;
        }
    }

    public void Unhighlight()
    {
        if (outlineScript.enabled)
        {
            outlineScript.enabled = false;
        }
    }
    public void selectedHighlight(string color)
    {
        if (rend != null && color == "yellow")
        {
            rend.material = brightYellow;
        }else if(rend != null && color == "purple")
        {
            rend.material = brightPurple;
        }
    }
    public void UnselectedHighlight()
    {
        if (rend != null)
        {
            rend.material = originalMaterial;
        }
    }
}
