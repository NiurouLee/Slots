// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date :2021-02-26 11:26 AM
// Ver : 1.0.0
// Description : BuildSetting.cs
// ChangeLog :
// **********************************************

using UnityEngine;

public static class BundleFolderSetting
{
#if UNITY_IOS
    public const string bundleFolderName = "fortunex_ios";
    public const string bundleRootFolderName = "rv3";
#elif UNITY_ANDROID
    public const string bundleFolderName = "fortunex_android";
    public const string bundleRootFolderName = "rv3";
#endif

    public static string BundleRootFolderName
    {
        get
        {
            Debug.Log("GetBundleFolderName:" + bundleFolderName);
            return bundleRootFolderName;
        }
    }
  
    
    public static string BundleFolderName
    {
        get
        {
            Debug.Log("GetBundleFolderName:" + bundleFolderName);
            return bundleFolderName;
        }
    }
}
