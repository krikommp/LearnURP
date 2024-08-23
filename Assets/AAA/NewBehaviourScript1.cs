using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

[ExecuteAlways]
public class NewBehaviourScript1 : MonoBehaviour
{
    public MeshRenderer maskRenderer;

    public MeshRenderer lineRenderer;

    private RenderTexture maskTexture;
    
    // Start is called before the first frame update
    void Start()
    {
        maskTexture = new RenderTexture(1024, 1024, 0, GraphicsFormat.R8_UNorm);
        maskTexture.name = "MaskTexture";
    }

    // Update is called once per frame
    void Update()
    {
        Graphics.Blit(null, maskTexture, maskRenderer.sharedMaterial);
        
        lineRenderer.sharedMaterial.SetTexture("_VornoiTex", maskTexture);
    }
}
