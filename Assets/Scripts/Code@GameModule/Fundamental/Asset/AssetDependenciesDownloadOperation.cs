// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date :2021-07-05 7:42 PM
// Ver : 1.0.0
// Description : AssetDependenciesDownloadOperation.cs
// ChangeLog :
// **********************************************

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace GameModule
{
    /// <summary>
    /// 下载资源依赖的Bundle包的操作的异步封装
    /// </summary>
    public class AssetDependenciesDownloadOperation : AssetAsyncOperation
    {
        private AsyncOperationHandle asyncOperationHandle;
        private readonly  bool autoReleaseHandle;
        private Action<bool> completeHandler;
        private List<string> addressableKeys;

        public AssetDependenciesDownloadOperation(string inAddress, bool inAutoReleaseHandle = true) :
            base(inAddress)
        {
            addressableKeys = null;
            autoReleaseHandle = inAutoReleaseHandle;
        }
        
        public AssetDependenciesDownloadOperation(List<string> inAddresses, bool inAutoReleaseHandle = true) :
            base(null)
        {
            addressableKeys = inAddresses;
            autoReleaseHandle = inAutoReleaseHandle;
        }

        /// <summary>
        /// 开始下载依赖资源
        /// </summary>
        /// <param name="inCompleteHandler"></param>
        public void StartOperation(Action<bool> inCompleteHandler)
        {
            completeHandler = inCompleteHandler;
            
            if (addressableKeys != null && addressableKeys.Count > 0)
            {
                asyncOperationHandle = Addressables.DownloadDependenciesAsync((IEnumerable)addressableKeys, Addressables.MergeMode.Union, autoReleaseHandle);
            }
            else
            {
                asyncOperationHandle = Addressables.DownloadDependenciesAsync(assetAddress, autoReleaseHandle);
            }
            
            asyncOperationHandle.Completed += OnOperationComplete;
        }

        public override AsyncOperationHandle GetOperationHandle()
        {
            return asyncOperationHandle;
        }

        /// <summary>
        /// 底层下载完成的回调
        /// </summary>
        /// <param name="operationHandle"></param>
        public void OnOperationComplete(AsyncOperationHandle operationHandle)
        {
            if (autoReleaseHandle)
                ReleaseOperation();

            if (operationHandle.Status == AsyncOperationStatus.Succeeded)
            {
                completeHandler?.Invoke(true);
                return;
            }

            completeHandler?.Invoke(false);
        }

        public override void ReleaseOperation()
        {
            asyncOperationHandle.Completed -= OnOperationComplete;

            if (!autoReleaseHandle)
            {
                Addressables.Release(asyncOperationHandle);
            }
            
            base.ReleaseOperation();
        }
    }
}