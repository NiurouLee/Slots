// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date :2021-07-05 7:41 PM
// Ver : 1.0.0
// Description : AssetExistCheckOperation.cs
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
    /// 检查资源是否存在的异步操作
    /// </summary>
    public class AssetExistCheckOperation : AssetAsyncOperation
    {
        protected Action<bool> checkCompleteHandler;
        private AsyncOperationHandle<IList<IResourceLocation>> checkOperationHandle;

        public AssetExistCheckOperation(string inAssetAddress) :
            base(inAssetAddress)
        {
        }
        
        /// <summary>
        /// 开始异步检查
        /// </summary>
        /// <param name="inCheckCompleteHandler">检查结果事件的处理器</param>
        public void StartOperation(Action<bool> inCheckCompleteHandler)
        {
            checkCompleteHandler = inCheckCompleteHandler;

            checkOperationHandle = Addressables.LoadResourceLocationsAsync(assetAddress);
            checkOperationHandle.Completed += OnCheckComplete;
        }

        /// <summary>
        /// Addressable 底层检查完成的回调
        /// </summary>
        /// <param name="operation"></param>
        public void OnCheckComplete(AsyncOperationHandle<IList<IResourceLocation>> operation)
        {
            ReleaseOperation();

            IList<IResourceLocation> locations = (IList<IResourceLocation>) operation.Result;
            if (locations != null && locations.Count > 0)
            {
                checkCompleteHandler?.Invoke(true);
            }
            else
            {
                checkCompleteHandler?.Invoke(false);
            }
        }

        public override AsyncOperationHandle GetOperationHandle()
        {
            return checkOperationHandle;
        }

        /// <summary>
        /// 释放异步操作相关的回调
        /// </summary>
        public override void ReleaseOperation()
        {
            checkOperationHandle.Completed -= OnCheckComplete;

            if (checkOperationHandle.IsValid())
            {
                Addressables.Release(checkOperationHandle);
            }
            
            base.ReleaseOperation();
        }
    }
}