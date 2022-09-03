using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class CanvasMaterialAnim : MonoBehaviour
{
    private CanvasRenderer canvasRenderer;
    public Material material;

    public Vector4 TilingAndOffset = new Vector4(1,1,0,0);
    

    private void Awake()
    {
        if (canvasRenderer == null)
        {
            canvasRenderer = GetComponent<CanvasRenderer>();
        }

        if (material == null)
        {
            material = canvasRenderer.GetMaterial();
        }
    }


    private void Update()
    {
        if (material != null)
        {
            material.SetVector("_MainTex_ST",TilingAndOffset);
        }
    }
}
