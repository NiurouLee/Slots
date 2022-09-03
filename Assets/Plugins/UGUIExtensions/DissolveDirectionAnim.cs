using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class DissolveDirectionAnim : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    public Material material;

    public float dissolve = 0;
    

    private void Awake()
    {
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        if (material == null)
        {
            material = spriteRenderer.sharedMaterial;
        }

        _propertyBlock = new MaterialPropertyBlock();
    }

    private MaterialPropertyBlock _propertyBlock;

    private void Update()
    {
        if (material != null)
        {
            // _propertyBlock.SetFloat("_Dissolve",dissolve);
            // spriteRenderer.SetPropertyBlock(_propertyBlock);
            material.SetFloat("_Dissolve",dissolve);
        }
    }
}