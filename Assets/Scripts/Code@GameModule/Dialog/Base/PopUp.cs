using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace GameModule
{
    public enum BlockLevel
    {
        LowerLevel2 = 8,
        LowerLevel1 = 9,
        DefaultLevel = 10,
        HigherLevel1 = 11,
        HigherLevel2 = 12,
    }
    
    public class PopUpLifeCycleEventHook
    {
        public virtual void OnEventBeforePop(Popup popup)
        {
            
        }

        public virtual void OnEventAfterPop(Popup popup)
        {
            
        }
    }
     
    [AttributeUsage(AttributeTargets.Class)]
    public class PopupArgsAttribute : Attribute
    {
        /// <summary>
        /// popup是否运行同时弹出多个实例
        /// </summary>
        private bool _allowMultiInstance;
        /// <summary>
        /// 在弹窗弹出生命周期中触发一些事件的处理函数
        /// </summary>
        private PopUpLifeCycleEventHook _eventHook;
        public PopupArgsAttribute()
        {
            _allowMultiInstance = false;
            _eventHook = null;
        }

        public PopupArgsAttribute(bool allowMultiInstance)
        {
            _allowMultiInstance = allowMultiInstance;
            _eventHook = null;
        }
        
        public PopupArgsAttribute(
            bool allowMultiInstance,
            BlockLevel blockLevel,
            PopUpLifeCycleEventHook eventHook)
        {
            _allowMultiInstance = allowMultiInstance;
            _eventHook = eventHook;
        }
 
        public bool IsAllowMultiInstance()
        {
            return _allowMultiInstance;
        }

        public PopUpLifeCycleEventHook GetEventHook()
        {
            return _eventHook;
        }
    }

    public class Popup : View, IPerform
    {
        public Animator animator;

        protected Action closeAction;
        protected Action closeClickAction;

        protected Vector2 contentDesignSize = Vector2.zero;
        protected Vector2 contentDesignSizeH = Vector2.zero;

        public GameObject containerNode;

        public PerformCategory performCategory = PerformCategory.None;

        [ComponentBinder("Root/TopGroup/CloseButton")]
        public Button closeButton;
        
        public BlockLevel blockLevel = BlockLevel.DefaultLevel;
        public string triggerSource = "NotAssigned";
        
        public Popup()
        {
            
        }

        public virtual void OnAlphaMaskClicked()
        {
            
        }

        public Popup(string assetAddress)
            : base(assetAddress)
        {
            
        }
        

        // 计算缩放尺寸，适配不同分辨率设备
        public virtual Vector3 CalculateScaleInfo()
        {
            if(contentDesignSize == Vector2.zero)
                return Vector3.one;

            var viewSize = ViewResolution.referenceResolutionLandscape;
          
            if (ViewManager.Instance.IsPortrait)
            {
                viewSize = ViewResolution.referenceResolutionPortrait;

                if (contentDesignSizeH !=  Vector2.zero)
                {
                    if (viewSize.y < contentDesignSizeH.y)
                    {
                        var scale = viewSize.y / contentDesignSizeH.y;
                        return new Vector3(scale, scale, scale);
                    }
                    
                    return Vector3.one;
                }
            }

            if (viewSize.x < contentDesignSize.x)
            {
                var scale = viewSize.x / contentDesignSize.x;
                return new Vector3(scale, scale, scale);
            }

            return Vector3.one;
        }

        public virtual void AdaptScaleTransform(Transform transformToScale, Vector2 preferSize)
        {
            var viewSize = ViewResolution.referenceResolutionLandscape;
          
            if (ViewManager.Instance.IsPortrait)
            {
                viewSize = ViewResolution.referenceResolutionPortrait;
            }

            if (viewSize.x < preferSize.x)
            {
                var scale = viewSize.x / preferSize.x;
                transformToScale.localScale =  new Vector3(scale, scale, scale);
            }
        }

      
      //  [ComponentBinder("Root/TopGroup/CloseButton")]
        protected virtual void OnCloseClicked()
        {
            DispatchCloseClickAction();
            Close();    
            SoundController.PlayButtonClose();
        }

        public async Task<CurrencyCoinView> AddCurrencyCoinView()
        {
            return await AddChild<CurrencyCoinView>();
        }
        
        public async Task<CurrencyEmeraldView> AddCurrencyEmeraldView()
        {
            return await AddChild<CurrencyEmeraldView>();
        }

        protected override void SetUpController(object inExtraData, object inExtraAsyncData = null)
        {
            base.SetUpController(inExtraData, inExtraAsyncData);

            if (inExtraData is PopupArgs popupArgs)
            {
                blockLevel = popupArgs.blockLevel;
                triggerSource = popupArgs.source;

                if (popupArgs.performCategory != PerformCategory.None)
                {
                    UpdatePerformCategory(popupArgs.performCategory);
                }
            }
            
            AddToPerformInAction();
        }

        public virtual bool NeedForceLandscapeScreen()
        {
            return false;
        }

        public virtual float GetPopUpMaskAlpha()
        {
            return 0.8f;
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
            PopupStack.ClosePopup(this);
        }
       
        public virtual void OnOpen()
        {
             
        }
        
        public virtual async Task PrepareAsyncAssets()
        {
            await Task.CompletedTask;
        }
        
        public virtual void OnClose()
        {
            
        }
        
        protected override void OnViewSetUpped()
        {
            base.OnViewSetUpped();
         
            animator = transform.GetComponent<Animator>();
            
            // if(animator)
            //     animator.keepAnimatorControllerStateOnDisable = true;
            if (closeButton == null)
            {
                var closeTransform = transform.Find("Root/CloseButton");
                if (closeTransform != null)
                {
                    closeButton = closeTransform.GetComponent<Button>();
                }
            }
            
            if(closeButton)
                closeButton.onClick.AddListener(OnCloseClicked);
        }
        
        public void DispatchCloseAction()
        {
            closeAction?.Invoke();
            RemoveFromPerformInAction();
        }

        public Action GetCloseAction()
        {
            return closeAction;
        }
        
        public void ResetCloseAction()
        { 
            closeAction = null;
        }
        
        public  void DispatchCloseClickAction()
        {
            closeClickAction?.Invoke();
        }
      
        public virtual string GetCloseAudioName()
        {
            return "General_CloseWindow";
        }
        
        public virtual string GetCloseAnimationName()
        {
            return "Close";
        }

        public virtual string GetOpenAudioName()
        {
            return "General_OpenWindow";
        }

        public  BlockLevel GetBlockLevel()
        {
            return blockLevel;
        }
        
        public string GetTriggerSource()
        {
            return triggerSource;
        }
        
        
        /// <summary>
        /// 是否允许多个实例
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsAllowMultiInstance(Type type)
        {
            var attributes = type.GetCustomAttributes(typeof(PopupArgsAttribute), false);
          
            if (attributes.Length > 0)
            {
                return ((PopupArgsAttribute)attributes[0]).IsAllowMultiInstance();
            }
            
            return false;
        }

        public virtual bool  IsPerformStillValid()
        {
            return transform != null;
        }

        public virtual PerformCategory GetPerformCategory()
        {
            return performCategory;
        }

        public void UpdatePerformCategory(PerformCategory inPerformCategory)
        {
            performCategory = inPerformCategory;
        }

        public void AddToPerformInAction()
        {
            if(performCategory != PerformCategory.None)
                PerformInAction.AddPerform(this);
        }
        
        public void RemoveFromPerformInAction()
        {
            if(performCategory != PerformCategory.None)
                PerformInAction.RemovePerform(this);
        }
    }

    public class Popup<T> : Popup where T : ViewController
    {
        public T viewController;

        public Popup()
        {
            
        }

        public Popup(string address)
            : base(address)
        {
            
        }
        
        protected override void SetUpController(object inExtraData, object inAsyncExtraData)
        {
            base.SetUpController(inExtraData, inAsyncExtraData);
            
            viewController = Activator.CreateInstance<T>();
            
            viewController.BindingView(this, inExtraData, inAsyncExtraData);
        }
        
        protected override void OnViewSetUpped()
        {
            base.OnViewSetUpped();
            viewController.OnViewDidLoad();
        }

        protected override void BindingComponent()
        {
            if (viewController != null && viewController.IsBindingUIComponentToController())
            {
                ComponentBinder.BindingComponent(viewController, transform);
            }
            else
            {
                ComponentBinder.BindingComponent(this, transform);
            }
        }

        public override void Destroy()
        {
            base.Destroy();
            viewController.OnViewDestroy();
        }

        public override void Hide()
        {
            base.Hide();
            viewController?.OnViewHide();
        }

        public override void Show()
        {
            base.Show();
            viewController?.OnViewShow();
        }

        protected override void EnableView()
        {
            // base.Show();
            // viewController?.OnViewEnabled();
        }
        
        public override async Task PrepareAsyncAssets()
        {
            if(viewController != null)
                await viewController.LoadExtraAsyncAssets();
        }

        public override void  OnOpen()
        {
            base.Show();
            viewController?.OnViewEnabled();
        }
    }
}