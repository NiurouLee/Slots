using System;
using System.Threading.Tasks;
using BestHTTP;
using DragonU3DSDK.Asset;
using Newtonsoft.Json.Linq;
using UnityEngine;


public class ResVersionManager
    {
        private static ResVersionManager _resVersionManager;

        public static ResVersionManager Instance
        {
            get
            {
                if (_resVersionManager == null)
                {
                    _resVersionManager = new ResVersionManager();
                }

                return _resVersionManager;
            }
        }



        public static string ResVersion
        {
            get;
            set;
        }

        public static string VersionCode
        {
            get
            {
#if UNITY_ANDROID
               return  AssetConfigController.Instance.VersionCode; 
             
#elif UNITY_IOS
            return AssetConfigController.Instance.IOSVersionCode;
#endif
            }
        }
        
        
        
        public static string Version
        {
            get
            {
#if UNITY_ANDROID
                return  AssetConfigController.Instance.RootVersion; 
             
#elif UNITY_IOS
            return AssetConfigController.Instance.IOSRootVersion;
#endif
            }
        }
        
        

        public static async Task<string> GetRemoteVersion()
        {
            
            string url;
#if PRODUCTION_PACKAGE 
        url = ConfigurationController.Instance.Res_Server_URL_Release;
#else
            
            url = ConfigurationController.Instance.Res_Server_URL_Beta;
#endif

            try
            {
                url += $"{BundleFolderSetting.BundleFolderName}/{BundleFolderSetting.bundleRootFolderName}/ResVersion.txt?=" + Guid.NewGuid().ToString().Replace("-", "");//强制CDN回源
                HTTPRequest httpRequest = new HTTPRequest(new Uri(url), HTTPMethods.Get);
                var response = await httpRequest.GetHTTPResponseAsync();

                JObject jObject = JObject.Parse(response.DataAsText);
                ResVersion = jObject.GetValue("ResVersion").ToObject<string>();
                
                PlayerPrefs.SetString("BundleVersionKey", ResVersion);
            }
            catch (Exception e)
            {
                ResVersion = PlayerPrefs.HasKey("BundleVersionKey") ? PlayerPrefs.GetString("BundleVersionKey") : VersionSetting.BundleVersion;
                Debug.LogException(e);
            }

            return ResVersion;
        }


        public static string CreateResVersionConfig(string resVersion)
        {
            JObject jObject = new JObject();
            jObject.Add("ResVersion",resVersion);
            return jObject.ToString();
        }

    }