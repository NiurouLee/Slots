// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date :2021-07-05 7:21 PM
// Ver : 1.0.0
// Description : AssetHelper.cs
// ChangeLog :
// **********************************************

using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using Object = UnityEngine.Object;

namespace GameModule
{
    public static class AssetHelper
    {
        private static List<AssetAsyncOperation> assetAsyncOperations;
        
        //常驻内存的资源
        private static Dictionary<string, UnityEngine.Object> residentAssets;
        
        //常驻内存资源的名字（不要轻易资源到常驻内存）
        private static readonly string[] residentAssetsAddress = {"CoinCollectFx", "ItemIconAtlas", "CommonUIAtlas", "PlayerAvatar"};

        static AssetHelper()
        {
            assetAsyncOperations = new List<AssetAsyncOperation>();
            residentAssets = new Dictionary<string, Object>();
        }
 
        /// <summary>
        /// 准备常驻内存的资源
        /// </summary>
        /// <param name="finishCallback">准备完成的回调</param>
        public static void PrepareResidentAssets(Action finishCallback)
        {
            if (residentAssetsAddress.Length == 0)
            {
                finishCallback?.Invoke();
                return;
            }
            
            int downloadFinished = 0;
            
            for (var i = 0; i < residentAssetsAddress.Length; i++)
            {
                PrepareAssets<UnityEngine.Object>(residentAssetsAddress[i], assetReference =>
                {
                    if (assetReference != null)
                    {
                        var assets = assetReference.GetReferencedAssets();

                        if (assets != null && assets.Count > 0)
                        {
                            for (var index = 0; index < assets.Count; index++)
                            {
                                if(!residentAssets.ContainsKey(assets[index].name))
                                    residentAssets.Add(assets[index].name, assets[index]);
                            }
                        }
                    }
                    
                    downloadFinished++;
                    if (downloadFinished == residentAssetsAddress.Length)
                        finishCallback?.Invoke();
                });
            }
        }

        /// <summary>
        // 获取常驻内存的资源
        /// </summary>
        /// <param name="address"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T GetResidentAsset<T>(string address) where T: UnityEngine.Object
        {
            if (residentAssets.ContainsKey(address))
            {
                return (T) residentAssets[address];
            }
            return null;
        }
        /// <summary>
        // 实力化常驻内存的资源,非常驻内存的会返回空
        /// </summary>
        /// <param name="address">资源的名字</param>
        /// <typeparam name="T">资源类型</typeparam>
        /// <returns></returns>
        
        public static T InstantiateResidentAsset<T>(string address) where T : UnityEngine.Object
        {
            if (residentAssets.ContainsKey(address))
            {
                return UnityEngine.Object.Instantiate(residentAssets[address]) as T;
            }

            return null;
        }
        
        
        /// <summary>
        /// 准备单个资源
        /// </summary>
        /// <param name="assetAddress">资源的Address</param>
        /// <param name="completeHandler">准备完成的回调</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static AssetReference PrepareAsset<T>(string assetAddress, Action<AssetReference> completeHandler)
            where T : UnityEngine.Object
        {
            var assetRef = new AssetReference(assetAddress);
            assetRef.PrepareAsset<T>(completeHandler);
            assetAsyncOperations.Add(assetRef);
            return assetRef;
        }
        
        
        /// <summary>
        /// 异步准备单个资源
        /// </summary>
        /// <param name="assetAddress">资源的Address</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static async Task<AssetReference> PrepareAssetAsync<T>(string assetAddress)
            where T : UnityEngine.Object
        {
            TaskCompletionSource<AssetReference> taskCompletionSource = new TaskCompletionSource<AssetReference>();
             
            PrepareAsset<T>(assetAddress, (assetReference) =>
            {
                taskCompletionSource.SetResult(assetReference);
            });

            return await taskCompletionSource.Task;
        }
        
        /// <summary>
        /// 准备多个资源
        /// </summary>
        /// <param name="assetAddress">资源的Address</param>
        /// <param name="completeHandler">准备完成的回调</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static AssetReference PrepareAssets<T>(string assetAddress, Action<AssetReference> completeHandler)
            where T : UnityEngine.Object
        {
            var assetRef = new AssetReference(assetAddress);
            assetRef.PrepareAssets<T>(completeHandler);
            assetAsyncOperations.Add(assetRef);
            return assetRef;
        }


        /// <summary>
        /// 释放对asyncOperation的引用
        /// </summary>
        /// <param name="asyncOperation"></param>
        public static void RemoveAsyncOperation(AssetAsyncOperation asyncOperation)
        {
            assetAsyncOperations.Remove(asyncOperation);
        }

        /// <summary>
        /// 释放所有的资源引用
        /// </summary>
        public static void RemoveAllAsyncOperation()
        {
            if (assetAsyncOperations.Count > 0)
            {
                for (int i = 0; i < assetAsyncOperations.Count; i++)
                {
                    assetAsyncOperations[i].ReleaseOperation();
                }
                
                assetAsyncOperations.Clear();
            }
        }

        /// <summary>
        /// 检查资源是否存在
        /// </summary>
        /// <param name="assetAddress"></param>
        /// <param name="completeHandler"></param>
        /// 
        public static void CheckAssetExist(string assetAddress, Action<bool> completeHandler)
        {
            AssetExistCheckOperation operation = new AssetExistCheckOperation(assetAddress);
            assetAsyncOperations.Add(operation);
            operation.StartOperation(completeHandler);
        }

        /// <summary>
        /// 下载特定Label的资源依Bundle包
        /// </summary>
        /// <param name="assetAddress"></param>
        /// <param name="completeHandler"></param>
        /// <param name="autoReleaseHandle"></param>
        /// <returns></returns>
        public static AssetDependenciesDownloadOperation DownloadDependencies(string assetAddress,
            Action<bool> completeHandler,
            bool autoReleaseHandle)
        {
            AssetDependenciesDownloadOperation operation =
                new AssetDependenciesDownloadOperation(assetAddress, autoReleaseHandle);

            assetAsyncOperations.Add(operation);
            operation.StartOperation(completeHandler);

            return operation;
        }
        
        /// <summary>
        /// 下载特定Label的资源依Bundle包
        /// </summary>
        /// <param name="assetAddressList"></param>
        /// <param name="completeHandler"></param>
        /// <param name="autoReleaseHandle"></param>
        /// <returns></returns>
        public static AssetDependenciesDownloadOperation DownloadDependencies(List<string> assetAddressList,
            Action<bool> completeHandler,
            bool autoReleaseHandle)
        {
            AssetDependenciesDownloadOperation operation =
                new AssetDependenciesDownloadOperation(assetAddressList, autoReleaseHandle);

            assetAsyncOperations.Add(operation);
            operation.StartOperation(completeHandler);
            
            return operation;
        }

        public static async Task<long> GetNeedDownloadSize(string assetAddress)
        {
            DownloadSizeRequestAsyncOperation asyncOperation = new DownloadSizeRequestAsyncOperation(assetAddress);
            assetAsyncOperations.Add(asyncOperation);
            
            var downloadSize = await asyncOperation.GetDownloadSizeAsync();

            asyncOperation.ReleaseOperation();
            
            return downloadSize;
        }
        
        public static async void GetNeedDownloadSize(string assetAddress, Action<long> checkCallback)
        {
            DownloadSizeRequestAsyncOperation asyncOperation = new DownloadSizeRequestAsyncOperation(assetAddress);
            assetAsyncOperations.Add(asyncOperation);
            
            var downloadSize = await asyncOperation.GetDownloadSizeAsync();

            asyncOperation.ReleaseOperation();
            
            checkCallback.Invoke(downloadSize);
        }
        
        public static void CleanUp()
        {
            RemoveAllAsyncOperation();
            residentAssets.Clear();
        }

    }
}