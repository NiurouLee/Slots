// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date :2021-07-05 7:50 PM
// Ver : 1.0.0
// Description : AssetAsyncOperation.cs
// ChangeLog :
// **********************************************

using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace GameModule
{
    /// <summary>
    /// 对资源操作相关的异步操作的封装，方便AssetHelper中对异步操作进行统一管理
    /// </summary>
    public abstract class AssetAsyncOperation
    {
        protected readonly string assetAddress;

        public AssetAsyncOperation(string inAssetAddress)
        {
            assetAddress = inAssetAddress;
        }
        
        public virtual void ReleaseOperation()
        {
             AssetHelper.RemoveAsyncOperation(this);
        }

        public abstract AsyncOperationHandle GetOperationHandle();
        
    }
}