using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

[ExecuteInEditMode]
public class SpriteScaler : MonoBehaviour
{
    [ReadOnly]
    public Vector2 designResolution;


    private Vector2 deviceReferenceResolution;

    private float scaler = 1;

    [Title("缩放")]
    public bool isLerpScaler = false;
    
    [InfoBox("这个参数是相对于设计分辨率多出来或者少的像素范围")]
    [ShowIf("isLerpScaler")]
    public Vector2 LerpSizeRange = new Vector2(-300,0);

    [InfoBox("这个参数是根据LerpSizeRange的区间来映射，确定最终缩放")]
    [ShowIf("isLerpScaler")]
    public Vector2 LerpValueRange = new Vector2(0.8f, 1f);

    [Title("位移")]
    public bool isLerpPos = false;

    [InfoBox("这个参数是相对于设计分辨率多出来或者少的像素范围")]
    [ShowIf("isLerpPos")]
    public Vector2 LerpPosSizeRange = new Vector2(-300, 300);

    [InfoBox("这个参数是根据LerpPosSizeRange的区间来映射，确定最终位移")]
    [ShowIf("isLerpPos")] 
    public Vector2 LerpPosValueRange = new Vector2(-1, 1);

    private Vector3 originLocalPos;
    private void Awake()
    {
        designResolution = new Vector2(1365, 768);
        originLocalPos = this.transform.localPosition;
        HandleScaleWithScreenSize();
        HandlePos();
    }


    protected virtual void HandleScaleWithScreenSize()
    {
        
        var screenWidth = Math.Max(Screen.width, Screen.height);
        var screenHeight = Math.Min(Screen.width, Screen.height);

        var referenceResolutionY = designResolution.y;
        var referenceWidth = (float) (screenWidth) / screenHeight * referenceResolutionY;

        deviceReferenceResolution = new Vector2(referenceWidth, referenceResolutionY);

        
        if (isLerpScaler)
        {
            float min = deviceReferenceResolution.x - designResolution.x;
            if (LerpSizeRange.y > LerpSizeRange.x)
            {
                min = Mathf.Clamp(min, LerpSizeRange.x, LerpSizeRange.y);

            }
            else
            {
                min = Mathf.Clamp(min, LerpSizeRange.y, LerpSizeRange.x);

            }

            scaler = Remap(min, LerpSizeRange.x, LerpSizeRange.y, LerpValueRange.x, LerpValueRange.y);
        }
        else
        {
            scaler = deviceReferenceResolution.x / designResolution.x;
            scaler = Mathf.Min(1, scaler);
        }

        this.transform.localScale = new Vector3(scaler, scaler, 1);
        //Debug.LogError($"=======Device:{deviceReferenceResolution} scaler:{scaler}");
    }


    protected virtual void HandlePos()
    {
        if (isLerpPos)
        {
            float min = deviceReferenceResolution.x - designResolution.x;
            if (LerpPosSizeRange.y > LerpPosSizeRange.x)
            {
                min = Mathf.Clamp(min, LerpPosSizeRange.x, LerpPosSizeRange.y);
            }
            else
            {
                min = Mathf.Clamp(min, LerpPosSizeRange.y, LerpPosSizeRange.x);
            }
            float pos = Remap(min, LerpPosSizeRange.x, LerpPosSizeRange.y, LerpPosValueRange.x, LerpPosValueRange.y);

            var originPos = originLocalPos;
            if (Screen.height > Screen.width)
            {
                originPos.y -= pos;
            }
            else
            {
                originPos.x -= pos;
            }

            this.transform.localPosition = originPos;
        }
        else
        {
            this.transform.localPosition = originLocalPos;
        }
    }



    private void LateUpdate()
    {
#if UNITY_EDITOR
        HandleScaleWithScreenSize();
        HandlePos();
#endif
    }
    
    
    float Remap(float s, float a1, float a2, float b1, float b2)
    {
        return b1 + (s-a1)*(b2-b1)/(a2-a1);
    }
}
