using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class PopupOffline : MonoBehaviour
{
    
    
    public Animator animator;

    protected Action closeAction;
    protected Action closeClickAction;

    protected Vector2 contentDesignSize = Vector2.zero;

    public GameObject containerNode;

    [OffineComponentBinder("Root/TopGroup/CloseButton")]
    public Button closeButton;


    protected AudioSource audioSource;
    

    public virtual void OnAlphaMaskClicked()
    {
            
    }


    public virtual void SetInfo(object inExtraData)
    {
        OffineComponentBinder.BindingComponent(this,this.transform);
        if (audioSource == null)
        {
            this.gameObject.AddComponent<AudioSource>();
        }

        if(closeButton)
            closeButton.onClick.AddListener(OnCloseClicked);
    }
    
    protected virtual void OnCloseClicked()
    {
        DispatchCloseClickAction();
        Close();    
        //audioSource.clip ==
    }

    // 计算缩放尺寸，适配不同分辨率设备
    public virtual Vector3 CalculateScaleInfo()
    {
        if(contentDesignSize == Vector2.zero)
            return Vector3.one;

        var viewSize = ViewResolution.referenceResolutionLandscape;
     

        if (viewSize.x < contentDesignSize.x)
        {
            var scale = viewSize.x / contentDesignSize.x;
            return new Vector3(scale, scale, scale);
        }

        return Vector3.one;
    }
    
    
    
    public virtual void SubscribeCloseAction(Action action)
    {
        closeAction += action;
    }
        
    public virtual void SubscribeCloseClickAction(Action action)
    {
        closeClickAction += action;
    }

    public virtual void UnSubscribeCloseAction(Action action)
    {
        closeAction -= action;
    }
        
    public virtual void UnSubscribeCloseClickAction(Action action)
    {
        closeClickAction -= action;
    }

    public virtual void Close()
    {
        PopupOfflineStack.ClosePopupOffline(this);
    }
    
    
    public  void DispatchCloseAction()
    {
        closeAction?.Invoke();
    }
    
    public  void DispatchCloseClickAction()
    {
        closeClickAction?.Invoke();
    }
    
    public virtual float GetPopupOfflineMaskAlpha()
    {
        return 0.8f;
    }

    public virtual void OnOpen()
    {
             
    }
    
    public virtual void OnClose()
    {
            
    }
        
    public virtual async Task PrepareAsyncAssets()
    {
        await Task.CompletedTask;
    }
}
