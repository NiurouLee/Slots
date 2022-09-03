// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/07/19/21:29
// Ver : 1.0.0
// Description : ViewController.cs
// ChangeLog :
// **********************************************

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace GameModule
{
    public class ViewController: Controller
    {
        protected Action onViewDestroyAction;
        protected object extraData;

        protected List<string> extraAssetNeedToLoad;
        protected Dictionary<string, AssetReference> extraAssetReferenceDict;
     
        public virtual bool IsBindingUIComponentToController()
        {
            return false;
        }
        /// <summary>
        /// 
        /// </summary>
        protected virtual void BindingViewProperty()
        {
            
        }
        
        /// <summary>
        /// View 创建成功，并且和Controller绑定之后的回调
        /// </summary>
        /// <param name="inView"></param>
        /// <param name="inExtraData"></param>
        /// <param name="inExtraAsyncData">从服务区拉取的数据</param>
        public virtual void BindingView(View inView, object inExtraData, object inExtraAsyncData = null)
        {
            extraData = inExtraData;
        }


        //获取弹窗是由什么触发的
        public string GetTriggerSource()
        {
            if (extraData != null && extraData is PopupArgs)
            {
                var popupArgs = extraData as PopupArgs;
                if (popupArgs != null && !string.IsNullOrEmpty(popupArgs.source))
                    return popupArgs.source;
            }
            return "NotSet";
        }
      

        public virtual void OnViewDidLoad()
        {
            BindingViewProperty();
            SubscribeEvents();
        }
        
        public virtual void OnViewDestroy()
        {
            base.CleanUp();

            if (extraAssetReferenceDict != null && extraAssetReferenceDict.Count > 0)
            {
                foreach (var item in extraAssetReferenceDict)
                {
                    item.Value.ReleaseOperation();
                }

                extraAssetReferenceDict.Clear();
            }
            
            onViewDestroyAction?.Invoke();
        }
 
        /// <summary>
        /// View销毁时候的回调
        /// </summary>
        /// <param name="inOnViewDestroyAction"></param>
        public void SetOnViewDestroyAction(Action inOnViewDestroyAction)
        {
            onViewDestroyAction = inOnViewDestroyAction;
        }
        
        public virtual void OnViewShow()
        {
            
        }
        
        public virtual void OnViewEnabled()
        {
            
        }
        
        public virtual async Task LoadExtraAsyncAssets()
        {
            if (extraAssetNeedToLoad != null && extraAssetNeedToLoad.Count > 0)
            {
                var finishCount = 0;

                var waitTask = new TaskCompletionSource<bool>();
                for (var i = 0; i < extraAssetNeedToLoad.Count; i++)
                {
                    var assetReference = AssetHelper.PrepareAsset<UnityEngine.Object>(extraAssetNeedToLoad[i],
                        (handler) =>
                        {
                            finishCount++;
                            if (finishCount == extraAssetNeedToLoad.Count)
                            {
                                waitTask.SetResult(true);
                            }
                        });

                    if (extraAssetReferenceDict == null)
                    {
                        extraAssetReferenceDict = new Dictionary<string, AssetReference>();
                    }

                    extraAssetReferenceDict.Add(extraAssetNeedToLoad[i], assetReference);
                }
                
                await waitTask.Task;
            }
        }
        
        public virtual async Task LoadExtraAsyncAssets(List<string> extraAssets)
        {
            if (extraAssets != null && extraAssets.Count > 0)
            {
                var finishCount = 0;

                var waitTask = new TaskCompletionSource<bool>();
                for (var i = 0; i < extraAssets.Count; i++)
                {
                    var assetReference = AssetHelper.PrepareAsset<UnityEngine.Object>(extraAssets[i],
                        (handler) =>
                        {
                            finishCount++;
                            if (finishCount == extraAssets.Count)
                            {
                                waitTask.SetResult(true);
                            }
                        });

                    if (extraAssetReferenceDict == null)
                    {
                        extraAssetReferenceDict = new Dictionary<string, AssetReference>();
                    }

                    extraAssetReferenceDict.Add(extraAssets[i], assetReference);
                }
                
                await waitTask.Task;
            }
        }

        protected AssetReference GetAssetReference(string address)
        {
            if (extraAssetReferenceDict.ContainsKey(address))
            {
                return extraAssetReferenceDict[address];
            }

            return null;
        }
        
        public void OnViewHide()
        {
            
        }
    }
    
    public class ViewController<T> : ViewController where T : View
    {
        protected T view;
        
        public override void BindingView(View inView, object inExtraData, object inExtraAsyncData = null)
        {
            view = inView as T;
            base.BindingView(inView, inExtraData, inExtraAsyncData);
        }
        
        
    }
}