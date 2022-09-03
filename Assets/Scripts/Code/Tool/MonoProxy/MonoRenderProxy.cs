/**********************************************
Copyright(c) 2021 by com.ustar
All right reserved

Author : Jian.Wang 
Date : 2020-11-24 21:59:03
Ver : 1.0.0
Description : 
ChangeLog :  添加基础功能的MonoBehaviour方便以后做功能扩展
**********************************************/


using UnityEngine;
using System;

public class MonoRenderProxy : MonoBehaviour
{
    private Action postRenderAction;
    private Action preRenderAction;
    private Action preCullAction;
    private Action<RenderTexture,RenderTexture> renderImageAction;

    public void BindingPreRenderAction(Action inPreRenderAction)
    {
        preRenderAction = inPreRenderAction;
    }

    public void BindingPostRenderAction(Action inPostRenderAction)
    {
        postRenderAction = inPostRenderAction;
    }

    public void BindingPreCullAction(Action inPreCullAction)
    {
        preCullAction = inPreCullAction;
    }
    
    public void BindingRenderImageAction(Action<RenderTexture,RenderTexture> inRenderImageAction)
    {
        renderImageAction = inRenderImageAction;
    }

    private void OnPostRender()
    {
        postRenderAction?.Invoke();
    }

    private void OnPreRender()
    {
        preRenderAction?.Invoke();
    }

    void OnPreCull()
    {
        preCullAction?.Invoke();
    }
    
    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        renderImageAction?.Invoke(src, dest);
    }
}