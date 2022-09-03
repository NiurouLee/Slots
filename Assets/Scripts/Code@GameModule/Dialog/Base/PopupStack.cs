// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/08/18/10:23
// Ver : 1.0.0
// Description : PopupStack.cs
// ChangeLog :
// **********************************************

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BestHTTP.SecureProtocol.Org.BouncyCastle.Security;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GameModule
{
    
    public static class PopupStack
    {
        private static List<Popup> _instanceList;

        /// <summary>
        /// 是否在dialog关闭的时候触发，关闭dialog事件，默认情况下是可以的，在特殊情况下，如断线重连，关闭所有弹出的时候，不要再触发别的逻辑了
        /// </summary>
        public static bool CanSpreadDialogCloseEvent { get; set; }
        public static bool PopUpInLoading => _popupInLoading;

        private static Transform containerTransform;

        private static Transform alphaLayerTransform;

        private static bool _popupInLoading = false;
        private static BlockLevel loadingPopUpLevel = BlockLevel.DefaultLevel;

        private static PerformCategory _loadingPerformCategory = PerformCategory.None;
         
        //TODO SHOW POPUP 添加队列逻辑
         
        static PopupStack()
        {
            _instanceList = new List<Popup>();
            CanSpreadDialogCloseEvent = true;

            BindingContainer();
        }
        
        private static void BindingContainer()
        {
            containerTransform = GameObject.Find("Launcher/PopupContainerCanvas").transform;
            alphaLayerTransform = containerTransform.Find("GrayMask");

            var pointerEventCustomHandler = alphaLayerTransform.gameObject.AddComponent<PointerEventCustomHandler>();
            
            pointerEventCustomHandler.BindingPointerClick(OnAlphaMaskClicked);
        }

        private static void OnAlphaMaskClicked(PointerEventData eventData)
        {
            var popup = GetTopPopup();

            if (popup != null)
            {
                popup.OnAlphaMaskClicked();
            }
        }

        public static async void ShowPopupNoWait<T>(string address = null, object argument = null, Action closeAction = null)
            where T : Popup
        {
            await ShowPopup<T>(address, argument, closeAction);
        }
        
        public static async Task<T> ShowPopup<T>(string address = null, object argument = null, Action closeAction = null)
            where T : Popup
        {
            if (address == null)
            {
                address = View.TryGetAssetAddressFromAttribute(typeof(T));
            }

            if (address == null)
            {
                throw new InvalidParameterException("Popup Address Can't Be Empty");
            }

            var popup = await ShowPopup(typeof(T), address, argument);

            if (closeAction != null)
            {
                popup.SubscribeCloseAction(closeAction);
            }
            
            return (T) popup;
        }
 
        /// <summary>
        /// 目前代码风险点，如果同时连续调用ShowPopup，上一个还没调用完成，开始下一个会出问题
        /// </summary>
        /// <param name="type"></param>
        /// <param name="address"></param>
        /// <param name="argument"></param>
        /// <returns></returns>
        public static async Task<Popup> ShowPopup(Type type, string address, object argument = null)
        {
            Popup popup = null;
            ViewManager.Instance.BlockingUserClick(true, "PopupStack.ShowPopup" + type.Name);
            try
            {
                //外面再套一层，因为分辨率缩放可能会跟动画缩放相互发生冲突
                var container = new GameObject("Container");
                container.transform.localPosition = Vector3.zero;
                container.transform.SetParent(containerTransform, false);
                var rect = container.AddComponent<RectTransform>();
                rect.anchorMin = new Vector2(0, 0);
                rect.anchorMax = new Vector2(1, 1);
                rect.pivot = new Vector2(0.5f, 0.5f);
                rect.sizeDelta = Vector2.zero;
                
                _popupInLoading = true;
               
                if (argument != null && argument is PopupArgs popupArgs)
                {
                    loadingPopUpLevel = popupArgs.blockLevel;
                    _loadingPerformCategory = popupArgs.performCategory;
                }
                else
                {
                    loadingPopUpLevel = BlockLevel.DefaultLevel;
                    _loadingPerformCategory = PerformCategory.None;
                }
                
                await ViewManager.Instance.DelayShowScreenLoadingView();

                popup = await View.CreateView(type, address, container.transform, argument) as Popup;

                if (popup != null && popup.NeedForceLandscapeScreen())
                {
                    if (ViewManager.Instance.IsPortrait)
                    {
                        // var sizeDelta = rect.sizeDelta;
                        // rect.sizeDelta = new Vector2(sizeDelta.y, sizeDelta.x);
                        //
                        // container.transform.localRotation = Quaternion.Euler(0,0,-90);
                        await ViewManager.Instance.UpdateViewToLandscape();
                        // var sizeDelta = rect.sizeDelta;
                        // rect.sizeDelta = new Vector2(sizeDelta.y, sizeDelta.x);
                        LayoutRebuilder.ForceRebuildLayoutImmediate(rect);
                    }
                }

                if (popup != null)
                {
                    await popup.PrepareAsyncAssets();
                    popup.containerNode = container;
                    alphaLayerTransform.SetAsLastSibling();
                    container.transform.SetAsLastSibling();

                    var scaleInfo = popup.CalculateScaleInfo();
                    container.transform.localScale = scaleInfo;

                    if (_instanceList.Count == 0)
                    {
                        alphaLayerTransform.gameObject.SetActive(true);
                        var maskImage = alphaLayerTransform.GetComponent<Image>();
                        maskImage.DOKill();
                        maskImage.DOFade(0, 0.4f).From();
                        maskImage.DOFade(popup.GetPopUpMaskAlpha(), 0.4f);
                        EventBus.Dispatch(new EventFirstPopupShow());
                    }
                    else
                    {
                        //还原背景遮罩透明度
                        var maskImage = alphaLayerTransform.GetComponent<Image>();
                        maskImage.DOKill();
                        var color = maskImage.color;
                        color.a = popup.GetPopUpMaskAlpha();
                        maskImage.color = color;
                    }

                    _instanceList.Add(popup);
                    _popupInLoading = false;
#if UNITY_IOS
                    if (ViewManager.Instance.IsPortrait)
                    {
                        UnityEngine.iOS.Device.hideHomeButton = true;
                    }
#endif
                    popup.OnOpen();
                    SoundController.PlaySfx(popup.GetOpenAudioName());
                }
            }
            catch (Exception e)
            {
                ViewManager.Instance.BlockingUserClick(false, "Exception:PopupStack.ShowPopup" + type.Name);
                ViewManager.Instance.HideScreenLoadingView();
                XDebug.LogError(e.Message);
                XDebug.LogError(e.StackTrace);

                //出现异常导致弹窗没有加载成功就抛一个event，让别的逻辑做下检查，避免游戏卡住
                if (_loadingPerformCategory != PerformCategory.None)
                {
                    EventBus.Dispatch(new EventPerformRemoved(_loadingPerformCategory));
                }
                
                throw;
            }
            
            ViewManager.Instance.BlockingUserClick(false, "PopupStack.ShowPopup" + type.Name);
            ViewManager.Instance.HideScreenLoadingView();
            
            return popup;
        }


        public static void ClosePopup<T>() where T : Popup
        {
            var popup = GetPopup<T>();

            if (popup != null)
            {
                ClosePopup(popup);
            }
        }
        
        public static async void ClosePopup(Popup popup)
        {
            SoundController.PlaySfx(popup.GetCloseAudioName());
            
            if (popup.transform != null && popup.transform.TryGetComponent(out Animator animator))
            {
                var closeAnimationName = popup.GetCloseAnimationName();
                if (!string.IsNullOrEmpty(closeAnimationName))
                {
                    var stateId = Animator.StringToHash(popup.GetCloseAnimationName());
                    if (animator.HasState(0, stateId))
                    {
                        PlayAlphaLayerFadeOutAnim(popup);
                        await XUtility.PlayAnimationAsync(animator, popup.GetCloseAnimationName(),forceCallback: true);
                    }
                }
            }

            ClosePopupWithoutAnim(popup);
        }

       
        public static async void ClosePopupWithoutAnim(Popup popup)
        {
            var containerNode = popup.containerNode;
            _instanceList.Remove(popup);

            bool forceUpdateToLandScape = popup.NeedForceLandscapeScreen();
            
            //函数写在这里，让屏幕先旋转，再关掉UI，避免横竖屏切换出现竖屏内容，显示成横屏的问题
            if (forceUpdateToLandScape && !ViewManager.Instance.IsInSwitching() && ViewManager.Instance.IsPortraitScene())
            {
               await ViewManager.Instance.UpdateViewToPortrait();
            }
            
            popup.OnClose();
            popup.Destroy();

            if (containerNode != null)
            {
                GameObject.Destroy(containerNode);
            }

            if (_instanceList.Count > 0)
            {
                alphaLayerTransform.SetAsLastSibling();
                _instanceList[_instanceList.Count - 1].transform.parent.SetAsLastSibling();

                var topPopUp = _instanceList[_instanceList.Count - 1];
                var maskImage = alphaLayerTransform.GetComponent<Image>();
                maskImage.DOKill();
                maskImage.color = new Color(0, 0, 0, topPopUp.GetPopUpMaskAlpha());
            }
            else
            {
                alphaLayerTransform.gameObject.SetActive(false);

                if (_popupInLoading == false)
                {
                    EventBus.Dispatch(new EventLastPopupClose());
#if UNITY_IOS
                    if (ViewManager.Instance.IsPortrait)
                    {
                        UnityEngine.iOS.Device.hideHomeButton = false;
                    }
#endif
                }
            }
            
            //所有事情处理之后，发送Popup关闭的事件
            popup.DispatchCloseAction();
            
            EventBus.Dispatch(new EventPopupClose(popup.GetType()));
            
        }

        public static bool HasPopup(Type type)
        {
            for (var i = 0; i < _instanceList.Count; i++)
            {
                if (_instanceList[i].GetType() == type)
                {
                    return true;
                }
            }

            return false;
        }

        public static bool HasPopup(Type type, string address)
        {
            for (var i = 0; i < _instanceList.Count; i++)
            {
                if (_instanceList[i].GetType() == type && _instanceList[i].GetAssetAddressName() == address)
                {
                    return true;
                }
            }

            return false;
        }

        public static T GetPopup<T>() where T : Popup
        {
            for (var i = 0; i < _instanceList.Count; i++)
            {
                if (_instanceList[i].GetType() == typeof(T))
                {
                    return _instanceList[i] as T;
                }
            }

            return null;
        }

        public static int GetPopupMaxBlockLevel()
        {
            var maxBlockLevel = 0;
            for (var i = 0; i < _instanceList.Count; i++)
            {
                var blockLevel = (int)_instanceList[i].GetBlockLevel();
                
                if(blockLevel > maxBlockLevel)
                {
                    maxBlockLevel = blockLevel;
                }
            }

            if (_popupInLoading)
            {
                if ((int)loadingPopUpLevel > maxBlockLevel)
                {
                    maxBlockLevel = (int)loadingPopUpLevel;
                }
            }
            
            return maxBlockLevel;
        }
        
        public static Popup GetPopup(Type type)
        {
            for (var i = 0; i < _instanceList.Count; i++)
            {
                if (_instanceList[i].GetType() == type)
                {
                    return _instanceList[i];
                }
            }
            
            return null;
        }

        public static void CloseAllPopup()
        {
            for (var i = 0; i < _instanceList.Count; i++)
            {
                var outerNode = _instanceList[i].containerNode;
                _instanceList[i].Destroy();
                GameObject.Destroy(outerNode);
            }

            _instanceList.Clear();

            alphaLayerTransform.gameObject.SetActive(false);
        }

        public static Popup GetTopPopup()
        {
            if (_instanceList.Count > 0)
            {
                return _instanceList[_instanceList.Count - 1];
            }

            return null;
        }

        public static int GetPopupCount()
        {
            return _instanceList.Count;
        }

        public static void PlayAlphaLayerFadeOutAnim(Popup popup)
        {
            //背景遮罩播放淡出特效
            if (alphaLayerTransform.gameObject.activeSelf && alphaLayerTransform.TryGetComponent(out Image image))
            {
                if (_instanceList.Count == 1)
                {
                    image.DOKill();
                    image.DOFade(popup.GetPopUpMaskAlpha(), 1f / 3f).From();
                    image.DOFade(0f, 1f / 3f).SetDelay(1f / 3f);
                }
            }

            //增加屏蔽点击遮罩
            GameObject touchMask = null;
            if (popup.containerNode != null)
            {
                touchMask = new GameObject("BlockMask");
                touchMask.transform.SetParent(popup.containerNode.transform);
                touchMask.transform.localPosition = new Vector3(0, 0, 0);
                touchMask.transform.localScale = new Vector3(1, 1, 1);
                var rect = touchMask.AddComponent<RectTransform>();
                rect.sizeDelta = containerTransform.GetComponent<RectTransform>().sizeDelta;
                var touchImg = touchMask.AddComponent<Image>();
                touchImg.color = Color.clear;
            }
        }
        
        public static void ShowOverLayView(View view)
        {
            view.transform.SetParent(containerTransform,false);
            view.transform.SetAsLastSibling();
        }

        //由于资源异步加载的问题，可能某些Perform还未开始
        public static bool HasPerformNotStart(PerformCategory category)
        {
            if (_popupInLoading)
            {
                if (_loadingPerformCategory == category)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
