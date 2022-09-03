// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date :2021-03-11 3:15 PM
// Ver : 1.0.0
// Description : ShinyMaterialPropertyUpdater.cs
// ChangeLog :
// **********************************************

using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
public class ShinyEffect : MonoBehaviour, IMaterialModifier
#if UNITY_EDITOR
    , ISerializationCallbackReceiver
#endif
{
    [SerializeField] [Range(0, 1)] float m_Location = 0;
    [SerializeField] [Range(0, 1)] float m_Width = 0.25f;
    [SerializeField] [Range(0.01f, 1)] float m_Softness = 1f;
    [SerializeField] [Range(0, 1)] float m_Brightness = 1f;
    [SerializeField] [Range(0, 360)] float m_Rotation;
    [SerializeField] [Range(0, 1)] float m_Highlight = 1;
    [SerializeField] [Range(0, 3)] float m_HighlightThreshold = 0;
    [SerializeField] [Range(0, 1)] float m_HighlightSoftFactor = 0;
    [SerializeField] Color m_ShinyColor = Color.white;

    Material effectMaterial;

    private Material modifyMaterial;

    private Image image;
    private SpriteRenderer spriteRenderer;

    private Vector4 spriteRect;

    private void OnDidApplyAnimationProperties()
    {
        UpdateMaterial();
    }
    
    private void UpdateMaterial()
    {
        if (!Application.isPlaying)
        {
            image = GetComponent<Image>();
            spriteRenderer = GetComponent<SpriteRenderer>();
           
            if (spriteRenderer != null)
            {
                if (Application.isPlaying)
                {
                    effectMaterial = spriteRenderer.material;
                }
                else
                {
                    effectMaterial = spriteRenderer.sharedMaterial;
                }
            }
            else if(image != null)
            {
                effectMaterial = image.material;
            }
        }
        
        if (modifyMaterial != null && modifyMaterial.shader.name.Contains("Shiny"))
        {
            UpdateMaterialProperty(modifyMaterial);
        }

        if (!Application.isPlaying || modifyMaterial == null)
        {
            if (effectMaterial != null && effectMaterial.shader.name.Contains("Shiny"))
            {
                UpdateMaterialProperty(effectMaterial);
            }
        }
    }

#if UNITY_EDITOR
    public void OnBeforeSerialize()
    {
      //  UpdateMaterial();
    }
    
    public void OnAfterDeserialize()
    {
    }
#endif

    public Material GetModifiedMaterial(Material baseMaterial)
    {
        baseMaterial.SetVector("_SpriteText_Rect", spriteRect);
        
        UpdateMaterialProperty(baseMaterial);
    
        modifyMaterial = baseMaterial;
        
        return baseMaterial;
    }

    public void UpdateMaterialProperty(Material material)
    {
        material.SetFloat("_ShinyLocation", m_Location);
        material.SetFloat("_ShinyWidth", m_Width);
        material.SetFloat("_ShinyRotation", m_Rotation);
        material.SetFloat("_ShinyHighlight", m_Highlight);
        material.SetFloat("_ShinySoftness", m_Softness);
        material.SetFloat("_ShinyBrightness", m_Brightness);
        material.SetColor("_ShinyColor", m_ShinyColor);
        material.SetFloat("_HighlightThreshold", m_HighlightThreshold);
        material.SetFloat("_HighlightSoftFactor", m_HighlightSoftFactor);
    }
    
    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        Sprite sp = null;

        if (spriteRenderer != null)
        {
            sp = spriteRenderer.sprite;
            if (Application.isPlaying)
            {
                effectMaterial = new Material(spriteRenderer.sharedMaterial);
                spriteRenderer.material = effectMaterial;
            }
            else
            {
                effectMaterial = spriteRenderer.sharedMaterial;
            }
        }

        image = GetComponent<Image>();

        if (image != null)
        {
            sp = image.sprite;
            if (Application.isPlaying)
            {
                effectMaterial = new Material(image.material);
                image.material = effectMaterial;
            }
            else
            {
                effectMaterial = image.material;
            }
        }

        if (sp == null || effectMaterial == null)
            return;

        if (!effectMaterial.shader.name.Contains("Shiny"))
        {
            effectMaterial = null;
            return;
        }

        var uv = sp.uv;
        Vector2 uvMin = Vector2.one;
        Vector2 uvMax = Vector2.zero;
       
        for (var i = 0; i < uv.Length; i++)
        {
            if (uv[i].x < uvMin.x)
            {
                uvMin.x = uv[i].x;
            }

            if (uv[i].x > uvMax.x)
            {
                uvMax.x = uv[i].x;
            }

            if (uv[i].y < uvMin.y)
            {
                uvMin.y = uv[i].y;
            }

            if (uv[i].y > uvMax.y)
            {
                uvMax.y = uv[i].y;
            }
        }
        
        spriteRect = new Vector4(uvMin.x, uvMin.y, uvMax.x - uvMin.x, uvMax.y - uvMin.y);
      
        effectMaterial.SetVector("_SpriteText_Rect", spriteRect);
        
        UpdateMaterialProperty(effectMaterial);
    }
}