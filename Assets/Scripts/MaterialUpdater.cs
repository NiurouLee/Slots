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
public class MaterialUpdater : MonoBehaviour, IMaterialModifier
#if UNITY_EDITOR
    , ISerializationCallbackReceiver
#endif
{
    [SerializeField] private string propertyFloat1Name = "";
    [SerializeField] private float propertyFloat1 = 0;

    [SerializeField] private string propertyFloat2Name = "";
    [SerializeField] private float propertyFloat2 = 0;

    [SerializeField] private string propertyFloat3Name = "";
    [SerializeField] private float propertyFloat3 = 0;

    [SerializeField] private string propertyFloat4Name = "";
    [SerializeField] private float propertyFloat4 = 0;

    [SerializeField] private string propertyFloat5Name = "";
    [SerializeField] private float propertyFloat5 = 0;

    [SerializeField] private string propertyFloat6Name = "";
    [SerializeField] private float propertyFloat6 = 0;

    [SerializeField] private string propertyFloat7Name = "";
    [SerializeField] private float propertyFloat7 = 0;

    [SerializeField] private string propertyFloat8Name = "";
    [SerializeField] private float propertyFloat8 = 0;

    [SerializeField] private string propertyFloat9Name = "";
    [SerializeField] private float propertyFloat9 = 0;

    [SerializeField] private string propertyVector1Name = "";
    [SerializeField] private Vector4 propertyVector1 = Vector4.zero;

    [SerializeField] private string propertyVector2Name = "";
    [SerializeField] private Vector4 propertyVector2 = Vector4.zero;

    [SerializeField] private string propertyVector3Name = "";
    [SerializeField] private Vector4 propertyVector3 = Vector4.zero;

    [SerializeField] private string propertyVector4Name = "";
    [SerializeField] private Vector4 propertyVector4 = Vector4.zero;

    [SerializeField] private string propertyVector5Name = "";
    [SerializeField] private Vector4 propertyVector5 = Vector4.zero;

    [SerializeField] private string propertyVector6Name = "";
    [SerializeField] private Vector4 propertyVector6 = Vector4.zero;

    [SerializeField] private string propertyVector7Name = "";
    [SerializeField] private Vector4 propertyVector7 = Vector4.zero;

    [SerializeField] private string propertyVector8Name = "";
    [SerializeField] private Vector4 propertyVector8 = Vector4.zero;

    [SerializeField] private string propertyVector9Name = "";
    [SerializeField] private Vector4 propertyVector9 = Vector4.zero;
 
    [SerializeField] private string propertyColor1Name = "";
    [SerializeField] private Color propertyColor1 = Color.white;

    [SerializeField] private string propertyColor2Name = "";
    [SerializeField] private Color propertyColor2 = Color.white;

    [SerializeField] private string propertyColor3Name = "";
    [SerializeField] private Color propertyColor3 = Color.white;

    [SerializeField] private string propertyColor4Name = "";
    [SerializeField] private Color propertyColor4 = Color.white;

    [SerializeField] private string propertyColor5Name = "";
    [SerializeField] private Color propertyColor5 = Color.white;

    [SerializeField] private string propertyColor6Name = "";
    [SerializeField] private Color propertyColor6 = Color.white;

    [SerializeField] private string propertyColor7Name = "";
    [SerializeField] private Color propertyColor7 = Color.white;

    [SerializeField] private string propertyColor8Name = "";
    [SerializeField] private Color propertyColor8 = Color.white;

    [SerializeField] private string propertyColor9Name = "";
    [SerializeField] private Color propertyColor9 = Color.white;

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
            else if (image != null)
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
        UpdateMaterial();
    }

    public void OnAfterDeserialize()
    {
    }
#endif

    public Material GetModifiedMaterial(Material baseMaterial)
    {
        if (baseMaterial.HasProperty("_SpriteText_Rect"))
            baseMaterial.SetVector("_SpriteText_Rect", spriteRect);

        UpdateMaterialProperty(baseMaterial);

        modifyMaterial = baseMaterial;

        return baseMaterial;
    }

    public void UpdateMaterialProperty(Material material)
    {
        if (propertyFloat1Name != "" && material.HasProperty(propertyFloat1Name))
            material.SetFloat(propertyFloat1Name, propertyFloat1);
        if (propertyFloat2Name != "" && material.HasProperty(propertyFloat2Name))
            material.SetFloat(propertyFloat2Name, propertyFloat2);
        if (propertyFloat3Name != "" && material.HasProperty(propertyFloat3Name))
            material.SetFloat(propertyFloat3Name, propertyFloat3);
        if (propertyFloat4Name != "" && material.HasProperty(propertyFloat4Name))
            material.SetFloat(propertyFloat4Name, propertyFloat4);
        if (propertyFloat5Name != "" && material.HasProperty(propertyFloat5Name))
            material.SetFloat(propertyFloat5Name, propertyFloat5);
        if (propertyFloat6Name != "" && material.HasProperty(propertyFloat6Name))
            material.SetFloat(propertyFloat6Name, propertyFloat6);
        if (propertyFloat7Name != "" && material.HasProperty(propertyFloat7Name))
            material.SetFloat(propertyFloat7Name, propertyFloat7);
        if (propertyFloat8Name != "" && material.HasProperty(propertyFloat8Name))
            material.SetFloat(propertyFloat8Name, propertyFloat8);
        if (propertyFloat9Name != "" && material.HasProperty(propertyFloat9Name))
            material.SetFloat(propertyFloat9Name, propertyFloat9);

        if (propertyColor1Name != "" && material.HasProperty(propertyColor1Name))
            material.SetColor(propertyColor1Name, propertyColor1);
        if (propertyColor2Name != "" && material.HasProperty(propertyColor2Name))
            material.SetColor(propertyColor2Name, propertyColor2);
        if (propertyColor3Name != "" && material.HasProperty(propertyColor3Name))
            material.SetColor(propertyColor3Name, propertyColor3);
        if (propertyColor4Name != "" && material.HasProperty(propertyColor4Name))
            material.SetColor(propertyColor4Name, propertyColor4);
        if (propertyColor5Name != "" && material.HasProperty(propertyColor5Name))
            material.SetColor(propertyColor5Name, propertyColor5);
        if (propertyColor6Name != "" && material.HasProperty(propertyColor6Name))
            material.SetColor(propertyColor6Name, propertyColor6);
        if (propertyColor7Name != "" && material.HasProperty(propertyColor7Name))
            material.SetColor(propertyColor7Name, propertyColor7);
        if (propertyColor8Name != "" && material.HasProperty(propertyColor8Name))
            material.SetColor(propertyColor8Name, propertyColor8);
        if (propertyColor9Name != "" && material.HasProperty(propertyColor9Name))
            material.SetColor(propertyColor9Name, propertyColor9);

        if (propertyVector1Name != "" && material.HasProperty(propertyVector1Name))
            material.SetVector(propertyVector1Name, propertyVector1);
        if (propertyVector2Name != "" && material.HasProperty(propertyVector2Name))
            material.SetVector(propertyVector2Name, propertyVector2);
        if (propertyVector3Name != "" && material.HasProperty(propertyVector3Name))
            material.SetVector(propertyVector3Name, propertyVector3);
        if (propertyVector4Name != "" && material.HasProperty(propertyVector4Name))
            material.SetVector(propertyVector4Name, propertyVector4);
        if (propertyVector5Name != "" && material.HasProperty(propertyVector5Name))
            material.SetVector(propertyVector5Name, propertyVector5);
        if (propertyVector6Name != "" && material.HasProperty(propertyVector6Name))
            material.SetVector(propertyVector6Name, propertyVector6);
        if (propertyVector7Name != "" && material.HasProperty(propertyVector7Name))
            material.SetVector(propertyVector7Name, propertyVector7);
        if (propertyVector8Name != "" && material.HasProperty(propertyVector8Name))
            material.SetVector(propertyVector8Name, propertyVector8);
        if (propertyVector9Name != "" && material.HasProperty(propertyVector9Name))
            material.SetVector(propertyVector9Name, propertyVector9);
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

        if (effectMaterial.HasProperty("_SpriteText_Rect"))
            effectMaterial.SetVector("_SpriteText_Rect", spriteRect);

        UpdateMaterialProperty(effectMaterial);
    }
}