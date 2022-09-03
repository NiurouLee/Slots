// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/11/16/10:31
// Ver : 1.0.0
// Description : DownloadSizeRequestAsyncOperation.cs
// ChangeLog :
// **********************************************

using System.Threading.Tasks;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace GameModule
{
    public class DownloadSizeRequestAsyncOperation:AssetAsyncOperation
    {
        private TaskCompletionSource<long> requestTask;
        
        public DownloadSizeRequestAsyncOperation(string address)
        :base(address)
        {
            
        }

        public AsyncOperationHandle<long> asyncOperationHandle;

        public override AsyncOperationHandle GetOperationHandle()
        {
            return asyncOperationHandle;
        }
        
        public async Task<long> GetDownloadSizeAsync()
        {
            requestTask = new TaskCompletionSource<long>();
            asyncOperationHandle = Addressables.GetDownloadSizeAsync(assetAddress);
            asyncOperationHandle.Completed += OnAsyncOperationComplete;

            return await requestTask.Task;
        }

        public void OnAsyncOperationComplete(AsyncOperationHandle<long> operationHandle)
        {
            requestTask.SetResult(operationHandle.Result);
            asyncOperationHandle.Completed -= OnAsyncOperationComplete;
        }

        public override void ReleaseOperation()
        {
            if (requestTask != null && !requestTask.Task.IsCompleted)
            {
                requestTask.SetCanceled();
            } 
            
            if (asyncOperationHandle.IsValid())
            {
                asyncOperationHandle.Completed -= OnAsyncOperationComplete;
                
                Addressables.Release(asyncOperationHandle);
            }
            base.ReleaseOperation();
        }
    }
}