// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/07/19/14:55
// Ver : 1.0.0
// Description : HotUpdater.cs
// ChangeLog :
// **********************************************

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Network.BI;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.ResourceManagement.Util;

public class ContentUpdater
{
    
    /// <summary>
    /// 打在包内的Bundle的名字
    /// </summary>
    private static List<string> _packageAssetBundlesInfo;

    public static void LoadInPackageAssetsBundleInfo()
    {
        try
        {
            var textAsset = Resources.Load<TextAsset>("PackageAssetBundlesInfo");
            
            if (textAsset != null)
            {
                var text = textAsset.text;
                var bundles = text.Split(';');
                if (bundles.Length > 0)
                {
                    _packageAssetBundlesInfo = new List<string>(bundles.Length);
                  
                    for (var i = 0; i < bundles.Length; i++)
                        _packageAssetBundlesInfo.Add(bundles[i]);
                    
                    Debug.Log("LoadBundleAssetsInfo:" + bundles.Length);
                }
            }
            else
            {
                _packageAssetBundlesInfo = new List<string>();
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
        }
    
    }
    
    public static async Task<bool> InitializeAddressable()
    {
        LoadInPackageAssetsBundleInfo();
        
        Addressables.InternalIdTransformFunc = InternalIdTransformFunc;
        
        var initializeTask = new TaskCompletionSource<bool>();
        
        Addressables.InitializeAsync().Completed += (data) =>
        {
            if (data.Status == AsyncOperationStatus.Succeeded)
            {
                initializeTask.SetResult(true);
            }
            else
            {
                initializeTask.SetResult(false);
            }
        };
        
        EnableExceptionHandler(true);

        return await initializeTask.Task;
    }

    public static void EnableExceptionHandler(bool enable)
    {
        if (enable)
            ResourceManager.ExceptionHandler += ExceptionHandler;
        else
        {
            ResourceManager.ExceptionHandler -= ExceptionHandler;
        }
    }

    private static void ExceptionHandler(AsyncOperationHandle handele, Exception exp)
    {
        if (exp.Message.Contains("UnityEngine.AddressableAssets.InvalidKeyException"))
        {
            OfflineCommonNoticePopup.ShowCommonOfflineNoticePopup("Update Resource Failed!");
            
            var errorEvent = new BiEventCommon.Types.ErrorEvent();
            errorEvent.Errmsg = exp.Message;
            errorEvent.Errno = "AddressableInvalidKeyException";
            
            BIManager.Instance.SendErrorEvent(errorEvent);
        }
    }

    public static string InPackageAssetBundleFolderName 
    {
        get
        {
            return "InPackageBundle";
        }
    }

    private static string InternalIdTransformFunc(IResourceLocation location)
    {
        if (location.Data is AssetBundleRequestOptions)
        {
            //PrimaryKey是AB包的名字
            //path就是StreamingAssets/Bundles/AB包名.bundle,其中Bundles是自定义文件夹名字,发布应用程序时,复制的目录
            var index = location.InternalId.LastIndexOf('/');
            
            if (index >= 0)
            {
                var fileName = location.InternalId.Substring(location.InternalId.LastIndexOf('/') + 1);
                var path = Path.Combine(Application.streamingAssetsPath, InPackageAssetBundleFolderName, fileName);
                
                if (_packageAssetBundlesInfo.Contains(fileName))
                {
                 //   Debug.Log("UseLocalBundle:" + path);
#if !PRODUCTION_PACKAGE
                     Debug.Log("[[ShowOnExceptionHandler]] LocalBundle:" + path);
#endif 
                    return path;
                }
            }
        }
#if !PRODUCTION_PACKAGE    
        if(location.InternalId.Contains(".bundle"))
            Debug.Log("[[ShowOnExceptionHandler]] RemoteBundle:" + location.InternalId.Replace("@ResVersion@", ResVersionManager.ResVersion));
#endif     
        return location.InternalId.Replace("@ResVersion@", ResVersionManager.ResVersion);
    }

    public static async Task<bool> CheckHasNewContentUpdate()
    {
        var checkTask = new TaskCompletionSource<bool>();

        Addressables.ResourceLocators.GetEnumerator();
        Addressables.CheckForCatalogUpdates().Completed += handle =>
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                if (handle.Result.Count > 0)
                {
                    checkTask.SetResult(true);
                }
                else
                {
                    checkTask.SetResult(false);
                }
            }
            else
            {
                checkTask.SetResult(false);
            }
        };
        return await checkTask.Task;
    }

    public static async Task<bool> UpdateNewContent()
    {
        var updateTask = new TaskCompletionSource<bool>();
        
        Addressables.UpdateCatalogs().Completed += updates =>
        {
            if (updates.Status == AsyncOperationStatus.Succeeded)
            {
                updateTask.SetResult(true);
            }
            else
            {
                updateTask.SetResult(false);
            }
        };
        return await updateTask.Task;
    }
}