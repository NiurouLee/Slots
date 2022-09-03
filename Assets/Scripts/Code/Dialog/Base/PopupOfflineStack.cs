// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/08/18/10:23
// Ver : 1.0.0
// Description : PopupOfflineStack.cs
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

    
    public static class PopupOfflineStack
    {
        private static List<PopupOffline> _instanceList;

//         /// <summary>
//         /// 是否在dialog关闭的时候触发，关闭dialog事件，默认情况下是可以的，在特殊情况下，如断线重连，关闭所有弹出的时候，不要再触发别的逻辑了
//         /// </summary>
        public static bool CanSpreadDialogCloseEvent { get; set; }

        private static Transform containerTransform;

        private static Transform alphaLayerTransform;

        private static bool _PopupOfflineInLoading = false;
        
        static PopupOfflineStack()
        {
            _instanceList = new List<PopupOffline>();
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
            var PopupOffline = GetTopPopupOffline();

            if (PopupOffline != null)
            {
                PopupOffline.OnAlphaMaskClicked();
            }
        }

        public static async void ShowPopupOfflineNoWait<T>(string address = null, object argument = null, Action closeAction = null)
            where T : PopupOffline
        {
            await ShowPopupOffline<T>(address, argument, closeAction);
        }
        
        public static async Task<T> ShowPopupOffline<T>(string address = null, object argument = null, Action closeAction = null)
            where T : PopupOffline
        {
           

            if (address == null)
            {
                throw new InvalidParameterException("PopupOffline Address Can't Be Empty");
            }

            var PopupOffline = await ShowPopupOffline<T>( address, argument);

            if (closeAction != null)
            {
                PopupOffline.SubscribeCloseAction(closeAction);
            }
            
            return (T) PopupOffline;
        }
 
        public static async Task<PopupOffline> ShowPopupOffline<T>( string address, object argument = null) where T : PopupOffline
        {
            PopupOffline PopupOffline = null;
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
                _PopupOfflineInLoading = true;



                var popupObj = Resources.Load<GameObject>($"UIOffline/Prefabs/{address}");
                popupObj = GameObject.Instantiate(popupObj);
                popupObj.transform.SetParent(container.transform, false);
                popupObj.transform.localPosition = Vector3.zero;
                popupObj.transform.localScale = Vector3.one;
                
                
                PopupOffline = popupObj.AddComponent<T>();
                PopupOffline.SetInfo(argument);

             

                if (PopupOffline != null)
                {
                    await PopupOffline.PrepareAsyncAssets();
                    PopupOffline.containerNode = container;
                    alphaLayerTransform.SetAsLastSibling();
                    container.transform.SetAsLastSibling();

                    var scaleInfo = PopupOffline.CalculateScaleInfo();
                    container.transform.localScale = scaleInfo;

                    if (_instanceList.Count == 0)
                    {
                        alphaLayerTransform.gameObject.SetActive(true);
                        var maskImage = alphaLayerTransform.GetComponent<Image>();
                        maskImage.DOKill();
                        maskImage.DOFade(0, 0.4f).From();
                        maskImage.DOFade(PopupOffline.GetPopupOfflineMaskAlpha(), 0.4f);
                    }
                    else
                    {
                        //还原背景遮罩透明度
                        var maskImage = alphaLayerTransform.GetComponent<Image>();
                        maskImage.DOKill();
                        var color = maskImage.color;
                        color.a = PopupOffline.GetPopupOfflineMaskAlpha();
                        maskImage.color = color;
                    }

                    _instanceList.Add(PopupOffline);
                    _PopupOfflineInLoading = false;
                    
                    PopupOffline.OnOpen();
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
                throw;
            }
            
            return PopupOffline;
        }


        public static void ClosePopupOffline<T>() where T : PopupOffline
        {
            var PopupOffline = GetPopupOffline<T>();

            if (PopupOffline != null)
            {
                ClosePopupOffline(PopupOffline);
            }
        }
        
        public static async void ClosePopupOffline(PopupOffline PopupOffline)
        {
            
            
            if (PopupOffline.transform != null && PopupOffline.transform.TryGetComponent(out Animator animator))
            {
                var stateId = Animator.StringToHash("Close");
                if (animator.HasState(0, stateId))
                {
                    PlayAlphaLayerFadeOutAnim(PopupOffline);
                    animator.Play("Close");
                }
            }

            ClosePopupOfflineWithoutAnim(PopupOffline);
        }

       
        public static void ClosePopupOfflineWithoutAnim(PopupOffline PopupOffline)
        {
            var containerNode = PopupOffline.containerNode;
            _instanceList.Remove(PopupOffline);

            
            PopupOffline.OnClose();
            
            GameObject.Destroy(PopupOffline.gameObject);
            

            if (containerNode != null)
            {
                GameObject.Destroy(containerNode);
            }

            if (_instanceList.Count > 0)
            {
                alphaLayerTransform.SetAsLastSibling();
                _instanceList[_instanceList.Count - 1].transform.parent.SetAsLastSibling();

                var topPopupOffline = _instanceList[_instanceList.Count - 1];
                var maskImage = alphaLayerTransform.GetComponent<Image>();
                maskImage.DOKill();
                maskImage.color = new Color(0, 0, 0, topPopupOffline.GetPopupOfflineMaskAlpha());
            }
            else
            {
                alphaLayerTransform.gameObject.SetActive(false);
            }
 

            
            //所有事情处理之后，发送PopupOffline关闭的事件
            PopupOffline.DispatchCloseAction();

            
        }

        public static bool HasPopupOffline(Type type)
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

    

        public static T GetPopupOffline<T>() where T : PopupOffline
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
        
        public static PopupOffline GetPopupOffline(Type type)
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

        public static void CloseAllPopupOffline()
        {
            for (var i = 0; i < _instanceList.Count; i++)
            {
                var outerNode = _instanceList[i].containerNode;
                //_instanceList[i].Destroy();
                GameObject.Destroy(outerNode);
            }

            _instanceList.Clear();

            alphaLayerTransform.gameObject.SetActive(false);
        }

        public static PopupOffline GetTopPopupOffline()
        {
            if (_instanceList.Count > 0)
            {
                return _instanceList[_instanceList.Count - 1];
            }

            return null;
        }

        public static int GetPopupOfflineCount()
        {
            return _instanceList.Count;
        }

        public static void PlayAlphaLayerFadeOutAnim(PopupOffline PopupOffline)
        {
            //背景遮罩播放淡出特效
            if (alphaLayerTransform.gameObject.activeSelf && alphaLayerTransform.TryGetComponent(out Image image))
            {
                if (_instanceList.Count == 1)
                {
                    image.DOKill();
                    image.DOFade(PopupOffline.GetPopupOfflineMaskAlpha(), 1f / 3f).From();
                    image.DOFade(0f, 1f / 3f).SetDelay(1f / 3f);
                }
            }

            //增加屏蔽点击遮罩
            GameObject touchMask = null;
            if (PopupOffline.containerNode != null)
            {
                touchMask = new GameObject("BlockMask");
                touchMask.transform.SetParent(PopupOffline.containerNode.transform);
                touchMask.transform.localPosition = new Vector3(0, 0, 0);
                touchMask.transform.localScale = new Vector3(1, 1, 1);
                var rect = touchMask.AddComponent<RectTransform>();
                rect.sizeDelta = containerTransform.GetComponent<RectTransform>().sizeDelta;
                var touchImg = touchMask.AddComponent<Image>();
                touchImg.color = Color.clear;
            }
        }
    }

