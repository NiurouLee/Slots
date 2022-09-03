using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BestHTTP;
using DragonU3DSDK;
using UnityEngine;

namespace Code
{
    
    
    
    
    public class ServerVersion
    {

        protected static readonly string serverUrl = @"https://fortune-x-api-alpha.ustargames.com";
        
        
#if UNITY_EDITOR || !PRODUCTION_PACKAGE
        protected static readonly bool isDebug = false; 
#endif
        
        
        public static string ServerConnectVersion { get; set; }
        
        
    //    private static TaskCompletionSource<bool> taskCompletionSource;
    public static async Task CheckServerVersion(string playerId)
    {



#if UNITY_EDITOR || !PRODUCTION_PACKAGE
        if (isDebug)
        {
            await ResVersionManager.GetRemoteVersion();
            return;
        }
#endif



        // if (taskCompletionSource == null)
        // {
        var taskCompletionSource = new TaskCompletionSource<bool>();
        // }


        //先检查网络是否可达
        while (Application.internetReachability == NetworkReachability.NotReachable)
        {
            TaskCompletionSource<bool> taskInternet = new TaskCompletionSource<bool>();
            OfflineCommonNoticePopup.ShowCommonOfflineNoticePopup(() => { taskInternet.SetResult(true); },
                "Your connection has been lost! Check your Internet connection and try again!", "OK", "No Internet");

            await taskInternet.Task;
            await Task.Delay(1000);
        }


        //重置服务器url，防止之前替换过服务器版本发url反复加后缀 
#if UNITY_EDITOR || !PRODUCTION_PACKAGE

        ConfigurationController.Instance.API_Server_URL_Beta =
            GetResetServerUrl(ConfigurationController.Instance.API_Server_URL_Beta);


#else
                ConfigurationController.Instance.API_Server_URL_Release =
                GetResetServerUrl(ConfigurationController.Instance.API_Server_URL_Release);
#endif




        string serverJson = string.Empty;
        //获取服务器信息
        while (string.IsNullOrEmpty(serverJson))
        {
            serverJson = await GetServerInfo(playerId);

            if (string.IsNullOrEmpty(serverJson))
            {
                TaskCompletionSource<bool> taskInternet = new TaskCompletionSource<bool>();
                OfflineCommonNoticePopup.ShowCommonOfflineNoticePopup(() => { taskInternet.SetResult(true); },
                    "Your connection has been lost! Check your Internet connection and try again!", "OK",
                    "Version Error");

                await taskInternet.Task;
            }
        }





        var dic = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(serverJson);

        string strServerRootversion = string.Empty;

        Debug.Log($"====== ClientRootVersion:{BundleFolderSetting.BundleRootFolderName}  serverInfo:{serverJson}");


        try
        {
            //检查强更新版本
#if UNITY_ANDROID
            strServerRootversion = dic["client-android-version"];
#elif UNITY_IOS
            strServerRootversion = dic["client-ios-version"];
#endif

            int serverRootVersion = int.Parse(strServerRootversion.Replace("v", ""));
            int clientRootVersion = int.Parse(BundleFolderSetting.BundleRootFolderName.Replace("rv", ""));

            if (clientRootVersion < serverRootVersion)
            {
                ShowUpdatePopup();
            }
            else
            {


                //检查热更新版本
                string strResVersion = string.Empty;
#if UNITY_ANDROID
                strResVersion = dic["resource-android-version"];
#elif UNITY_IOS
                strResVersion = dic["resource-ios-version"];
#endif
                
                int serverResVersion = int.Parse(strResVersion.Replace("v", ""));
                string strClientResVersion = PlayerPrefs.HasKey("BundleVersionKey")
                    ? PlayerPrefs.GetString("BundleVersionKey")
                    : VersionSetting.BundleVersion;
                
                int clientResVersion = int.Parse(strClientResVersion.Replace("v", ""));
                int inPackageResVersion = int.Parse(VersionSetting.BundleVersion.Replace("v", ""));
               
                if (inPackageResVersion > clientResVersion)
                {
                    clientResVersion = inPackageResVersion;
                    strClientResVersion = VersionSetting.BundleVersion;
                }

                //只热更新大于当前版本的资源
                if (serverResVersion > clientResVersion)
                {
                    ResVersionManager.ResVersion = strResVersion;
                }
                else
                {
                    ResVersionManager.ResVersion = strClientResVersion;
                }

                //服务器版本
#if UNITY_ANDROID
                ServerConnectVersion = dic["server-android-version"];
#elif UNITY_IOS
                ServerConnectVersion = dic["server-ios-version"];
#endif






                if (!string.IsNullOrEmpty(ServerConnectVersion))
                {
#if UNITY_EDITOR || !PRODUCTION_PACKAGE

                    if (ConfigurationController.Instance.API_Server_URL_Beta.Contains(serverUrl))
                    {

                        ConfigurationController.Instance.API_Server_URL_Beta =
                            $"{ConfigurationController.Instance.API_Server_URL_Beta}/{ServerConnectVersion}";
                    }
#else
                ConfigurationController.Instance.API_Server_URL_Release =
                    $"{ConfigurationController.Instance.API_Server_URL_Release}/{ServerConnectVersion}";
#endif
                }

                PlayerPrefs.SetString("BundleVersionKey", ResVersionManager.ResVersion);

                if (!String.IsNullOrEmpty(serverJson))
                {
                    PlayerPrefs.SetString("ServerVersionInfo", serverJson);
                }

                PlayerPrefs.Save();

                taskCompletionSource.SetResult(true);
            }
        }
        catch (Exception e)
        {
            Debug.LogException(e);
            ShowCodeError();
        }

        await taskCompletionSource.Task;
    }


    static void ShowCodeError()
    {
        OfflineCommonNoticePopup.ShowCommonOfflineNoticePopup(() => { ShowCodeError(); },
            "Your connection has been lost! Check your Internet connection and try again!", "OK",
            "Error");
    }



    protected static void ShowUpdatePopup()
        {
            OfflineCommonNoticePopup.ShowCommonOfflineUpdatePopup(() =>
            {
                string url = ConfigurationController.Instance.UPDATE_URL;
                Application.OpenURL(url);
                ShowUpdatePopup();
            });
        }


        protected async static Task<string> GetServerInfo(string playerId)
        {

            string serverJson = "";



            string url = "";
#if UNITY_EDITOR || !PRODUCTION_PACKAGE
            url = $@"{serverUrl}/version?id={playerId}&code={BundleFolderSetting.BundleRootFolderName}&platform={DeviceHelper.GetPlatform()}";
#else
            url =
 $@"{ConfigurationController.Instance.API_Server_URL_Release}/version?id={playerId}&code={BundleFolderSetting.BundleRootFolderName}&platform={DeviceHelper.GetPlatform()}";
#endif
            Debug.Log($"==== ServerVersion url:{url}");
            BestHTTP.HTTPRequest request =
                new BestHTTP.HTTPRequest(new Uri(url), HTTPMethods.Get);

            request.Timeout = TimeSpan.FromSeconds(8);
            request.ConnectTimeout = TimeSpan.FromSeconds(8);
            try
            {
                serverJson = await request.GetAsStringAsync();

                switch (request.State)
                {

                    case HTTPRequestStates.ConnectionTimedOut:
                    case HTTPRequestStates.TimedOut:
                        return null;
                    case HTTPRequestStates.Error:
                        //如果因为各种原因取不到就用上一次用过的
                        serverJson = PlayerPrefs.GetString("ServerVersionInfo", "");
                        break;
                }

                return serverJson;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                if (request.Response != null && request.Response.StatusCode >= 400)
                {
                    serverJson = PlayerPrefs.GetString("ServerVersionInfo", "");
                }
                else
                {
                    serverJson = "";
                }
            }

            return serverJson;
        }


        protected static string GetResetServerUrl(string url)
        {
            int index = url.IndexOf("/v");
            if (index != -1)
            {
                url = url.Substring(0, index);
            }
            
            return url;
        }
    }
}