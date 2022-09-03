
using DragonU3DSDK.Asset;
using UnityEngine;
using UnityEngine.Networking;

public static class PackageSetting
{
   
    public static string AddressablesUrl
    {
        get
        {
 
            //wang 
           // return "http://10.10.214.218:63635";
           
           return GetResURL();
 
        }
    }
    public static string GetResURL()
    {
        string url;
        
#if PRODUCTION_PACKAGE 
        url = ConfigurationController.Instance.Res_Server_URL_Release;
#else
        url = ConfigurationController.Instance.Res_Server_URL_Beta;
#endif
        
        url += $"{BundleFolderSetting.BundleFolderName}/{BundleFolderSetting.BundleRootFolderName}/@ResVersion@";

        Debug.Log($"=========url:{url}");
        return url;
    }
}