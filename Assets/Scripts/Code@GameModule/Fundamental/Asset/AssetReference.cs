// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date :2021-07-05 6:23 PM
// Ver : 1.0.0
// Description : AssetHandle.cs
// ChangeLog :
// **********************************************

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;

namespace GameModule
{
    /// <summary>
    /// AssetReference 做为HotRuntime层和Addressable之间的中间层，所有的的资源加载请求都要通过AssetReference来实现，
    /// 不允许在HotRuntime里面直接调用Addressables的函数来加载资源
    /// 目的是防止游戏内重启的时候，统一处理取消Addressables 的异步回调，避免Addressable的回调到已经不用的Appdomain中，造成逻辑混乱，导致不可预知的异常。
    /// 同时可以统一的管理和监控游戏内资源的引用
    /// </summary>
    public class AssetReference: AssetAsyncOperation
    {
        private Action<AssetReference> completeHandler;
        private bool requestComplete;
        private bool requestedMultiAssets;

        private AsyncOperationHandle asyncOperationHandle;

        private Dictionary<string, UnityEngine.Object> referencedAssetsDict;

        public AssetReference(string inAssetAddress)
            :base(inAssetAddress)
        {
            requestComplete = false;
            requestedMultiAssets = false;
        }

        /// <summary>
        /// 准备assetAddress所对应的单个资源
        /// </summary>
        /// <param name="inCompleteHandler"></param>
        /// <typeparam name="T"></typeparam>
        public void PrepareAsset<T>(Action<AssetReference> inCompleteHandler) where T : UnityEngine.Object
        {
            if (!requestComplete)
            {
                XDebug.Log("AssetAddress:"+assetAddress);
                completeHandler = inCompleteHandler;
                requestedMultiAssets = false;
                asyncOperationHandle = Addressables.LoadAssetAsync<T>(assetAddress);
                asyncOperationHandle.Completed += OnAssetPrepared;
            }
        }
        
        /// <summary>
        /// 准备assetAddress所对应的多个资源
        /// </summary>
        /// <param name="inCompleteHandler"></param>
        /// <typeparam name="T"></typeparam>
        public void PrepareAssets<T>(Action<AssetReference> inCompleteHandler) where T : UnityEngine.Object
        {
            if (!requestComplete)
            {
                XDebug.Log("AssetsAddress:"+assetAddress);
                completeHandler = inCompleteHandler;
                requestedMultiAssets = true;
                asyncOperationHandle = Addressables.LoadAssetsAsync<T>(assetAddress, null);
                asyncOperationHandle.Completed += OnAssetPrepared;
            }
        }
        
        /// <summary>
        /// Addressable层资源准备好，或者下载失败的的回调
        /// </summary>
        /// <param name="completedHandle"></param>
        private void OnAssetPrepared(AsyncOperationHandle completedHandle)
        {
            if (!requestComplete)
            {
                if (completedHandle.Status == AsyncOperationStatus.Succeeded)
                {
                    if (requestedMultiAssets)
                    {
                        referencedAssetsDict = new Dictionary<string, UnityEngine.Object>();
                        var assets = this.GetReferencedAssets();
                        if (assets != null)
                        {
                            for (var i = 0; i < assets.Count; i++)
                            {
                                if(!referencedAssetsDict.ContainsKey(assets[i].name))
                                    referencedAssetsDict.Add(assets[i].name, assets[i]);
                                else
                                {
                                    XDebug.LogError($"Name Conflict :{assets[i].name}:{assets[i].GetType()}/{referencedAssetsDict[assets[i].name].GetType()}");
                                }
                            }
                        }
                    }

                    completeHandler?.Invoke(this);
                    completeHandler = null;
                    requestComplete = true;
                }
                else
                {
                    ReleaseOperation();
                    
                    //TODO Report Resource Error
                    completeHandler?.Invoke(null);
                    completeHandler = null;
                }
            }
        }

        /// <summary>
        /// 释放当前AssetReference引用资源
        /// </summary>
        public override void ReleaseOperation()
        {
            if (asyncOperationHandle.IsValid())
            {
                asyncOperationHandle.Completed -= OnAssetPrepared;
                Addressables.Release(asyncOperationHandle);
            }
            
            base.ReleaseOperation();
        }

        /// <summary>
        /// 获取当前AssetReference引用到的所以资源
        /// </summary>
        /// <returns></returns>
        public IList<UnityEngine.Object> GetReferencedAssets()
        {
            if (asyncOperationHandle.IsValid() && asyncOperationHandle.Status == AsyncOperationStatus.Succeeded)
            {
                if (requestedMultiAssets)
                {
                    return (IList<UnityEngine.Object>) asyncOperationHandle.Result;
                }

                var list = new List<UnityEngine.Object>();
                list.Add((UnityEngine.Object) asyncOperationHandle.Result);
                return list;
            }

            return null;
        }
        
        /// <summary>
        /// 获取当前AssetReference对应的 Addressable的 AsyncOperationHandle<T>
        /// </summary>
        /// <returns></returns>
        public AsyncOperationHandle<T> GetOperationHandle<T>()
        {
            return asyncOperationHandle.Convert<T>();
        }

        /// <summary>
        /// 获取当前AssetReference对应的 Addressable的 AsyncOperationHandle
        /// </summary>
        /// <returns></returns>
        public override AsyncOperationHandle GetOperationHandle()
        {
            return asyncOperationHandle;
        }
        
        /// <summary>
        /// 创建一个引用的资源的实例
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T InstantiateAsset<T>() where T : UnityEngine.Object
        {
            if (!requestedMultiAssets && asyncOperationHandle.IsValid())
            {
                return UnityEngine.Object.Instantiate((UnityEngine.Object) asyncOperationHandle.Result) as T;
            }

            return null;
        }
 
        /// <summary>
        /// 获得引用的资源
        /// </summary>
        /// <param name="inAssetAddress">当前AssetReference引用了多个资源的时候，传入要获取资源的名称，如果当前AssetReference引用单个资源，无论传入什么参数都返回引用的资源</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetAsset<T>(string inAssetAddress = null) where T : UnityEngine.Object
        {
            if (!requestedMultiAssets && asyncOperationHandle.IsValid())
            {
                return asyncOperationHandle.Result as T;
            }

            if (requestedMultiAssets && inAssetAddress != null)
            {
                if (referencedAssetsDict.ContainsKey(inAssetAddress))
                {
                    return referencedAssetsDict[inAssetAddress] as T;
                }
            }

            return null;
        }
    }
}