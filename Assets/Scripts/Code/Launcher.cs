using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Code;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Tool;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

#if UNITY_ANDROID
// using Unity.Notifications.Android;
#elif UNITY_IOS
#endif

public class Launcher : MonoBehaviour
{
    private static Launcher instance;

    public static bool IsRestarted { get; private set; } = false;
    
    //初始化canvas的referenceResolution，让logo能够正常显示
    private void SetUpGameView()
    {
        ViewResolution.SetUpViewResolution();

        var canvasScene = GameObject.Find("Launcher/SceneContainer/CanvasScene").transform;
        var canvasScaler = canvasScene.GetComponent<CanvasScaler>();

        canvasScaler.referenceResolution = ViewResolution.referenceResolutionLandscape;

        Camera.main.transform.localPosition = ViewResolution.GetCameraPositionByViewResolution(Camera.main, false);
        
#if !UNITY_EDITOR && !PRODUCTION_PACKAGE
        if(!canvasScene.gameObject.GetComponent<ApplicationExceptionHandler>())
                canvasScene.gameObject.AddComponent<ApplicationExceptionHandler>();
#endif
    }
 
    protected static GameObject picLauncher = null;

    protected static GameObject splashView;
    
    private void Start()
    {
        instance = this;

        picLauncher = GameObject.Find("Launcher/SceneContainer/CanvasScene/picLauncher");
         
        AttachExternalEventListener();

        SetUpGameView();

        DontDestroyOnLoad(gameObject);
         
        LaunchGameAsync();
    }

    private void ShowSplashView()
    {
        var splashViewTemplate = Resources.Load<GameObject>("UIOffline/Prefabs/UISplashView");
        splashView = GameObject.Instantiate(splashViewTemplate,
            GameObject.Find("Launcher/SceneContainer/CanvasScene").transform);
    }

    private void HideSplashView()
    {
        GameObject.Destroy(splashView);
    }

    public async void LaunchGameAsync()
    {
        
        Init();
        
#if !PRODUCTION_PACKAGE || UNITY_EDITOR
        var severSelectAsset = Resources.Load<GameObject>("ServerOption");
        var severSelect = GameObject.Instantiate(severSelectAsset,GameObject.Find("Launcher/SceneContainer/CanvasScene").transform);
        var serverSelector = severSelect.AddComponent<ServerSelector>();
        await serverSelector.WaitServerSelection();
#endif
        // ConfigurationController.Instance.version = VersionStatus.RELEASE;
        InPackageBIManager.SendGameEvent(BiEventFortuneX.Types.GameEventType.GameEventLaunchApp);
        
        PerformanceTracker.AddTrackPoint("ShowSplashView");
        
        ShowSplashView();
        
        PerformanceTracker.FinishTrackPoint("ShowSplashView");
        
        PerformanceTracker.AddTrackPoint("CheckServerVersion");
        
        var storageCommon = StorageManager.Instance.GetStorage<StorageCommon>();
        await ServerVersion.CheckServerVersion(storageCommon.PlayerId.ToString());
       
        PerformanceTracker.FinishTrackPoint("CheckServerVersion");
        
        PerformanceTracker.AddTrackPoint("InitializeAddressable");
      
        bool result = await ContentUpdater.InitializeAddressable();
        
        while (!result)
        {
            result = await ContentUpdater.InitializeAddressable();
        }
        
        InPackageBIManager.SendGameEvent(BiEventFortuneX.Types.GameEventType.GameEventInitAddressableSuccess);
        
        PerformanceTracker.FinishTrackPoint("InitializeAddressable");

        LaunchGame();
    }
    
    protected void Init()
    {
        if (!StorageManager.Instance.Inited)
        {
            List<StorageBase> listStorageBases = new List<StorageBase>();
            listStorageBases.Add(new StorageCommon());
            listStorageBases.Add(new StorageUserGroup());
            listStorageBases.Add(new StorageAd());
            StorageManager.Instance.Init(listStorageBases);
        }
    }
    
    public static async void Restart()
    {
        PerformanceTracker.AddTrackPoint("ShowSplashView");
        
        instance.ShowSplashView();
       
        PerformanceTracker.FinishTrackPoint("ShowSplashView");
        
        //await ResVersionManager.GetRemoteVersion();
        var storageCommon = StorageManager.Instance.GetStorage<StorageCommon>();
        await ServerVersion.CheckServerVersion(storageCommon.PlayerId.ToString());
      
        bool hasContentUpdate = await ContentUpdater.CheckHasNewContentUpdate();

        if (hasContentUpdate)
        {
            await ContentUpdater.UpdateNewContent();
        }
      
        IsRestarted = true;
        
        instance.LaunchGame();
    }

    private async void LaunchGame()
    {
        ProcessPreLaunchAction();
        var waitTask = ShowLoadingView();
#if HOT_FIX
        PerformanceTracker.AddTrackPoint("ILRuntimeInitialized");
        StartCoroutine(ILRuntimeHelp.LoadGameModule(async () =>
        {
            PerformanceTracker.FinishTrackPoint("ILRuntimeInitialized");
            await waitTask;
            OnILRuntimeInitialized();
        }));
#else
        
        await waitTask;
        //不直接调用 GameModule.GameModuleLaunch.Start 是防止编译dll的时候找不到 GameModule部分的GameModule.GameModuleLaunch类而报错
        var assembly = Assembly.Load("GameModuleAssembly");
        var type = assembly.GetType("GameModule.GameModuleLaunch");
        var method = type.GetMethod("Start", BindingFlags.Public | BindingFlags.Static);
        method?.Invoke(null, new object[] {false});
        //GameModule.GameModuleLaunch.Start(false);
#endif
    }
    
    void OnILRuntimeInitialized()
    {
        ILRuntimeHelp.appdomain.Invoke("GameModule.GameModuleLaunch", "Start", null, new object[] {true});
    }

    void OnApplicationQuit()
    {
        ILRuntimeHelp.Dispose();
        GC.Collect();
    }

    async Task ShowLoadingView()
    {
        TaskCompletionSource<bool> waitTask = new TaskCompletionSource<bool>();
        
        InPackageBIManager.SendGameEvent(BiEventFortuneX.Types.GameEventType.GameEventStartLoadLoading);
        
        PerformanceTracker.AddTrackPoint("ShowLoadingView");
        
        Addressables.LoadAssetAsync<GameObject>("AssetsLoading").Completed += async (handle) =>
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                var animator = splashView.GetComponent<Animator>();
                var normalizedTime = Math.Floor(animator.GetCurrentAnimatorStateInfo(0).normalizedTime);
                var length = animator.GetCurrentAnimatorStateInfo(0).length;

                double waitTime = length - normalizedTime;

                if (waitTime > 0)
                {
                    await Task.Delay((int) (waitTime * 1000));
                }

                var loadingView = GameObject.Instantiate(handle.Result, 
                    GameObject.Find("Launcher/SceneContainer/CanvasScene").transform);
                
                loadingView.AddComponent<InPackageLoadingView>();
                loadingView.name = "AssetsLoading";
                
                HideSplashView();
                PerformanceTracker.FinishTrackPoint("ShowLoadingView");
                waitTask.SetResult(true);
                InPackageBIManager.SendGameEvent(BiEventFortuneX.Types.GameEventType.GameEventEnterLoadingScene);
            }
            else
            {
                await ShowLoadingView();
            }
        };

        await waitTask.Task;
    }
    
    void ProcessPreLaunchAction()
    {
        LitJson.JsonMapper.RegisterExporter<float>((obj, writer) => writer.Write(Convert.ToDouble(obj)));
        LitJson.JsonMapper.RegisterImporter<double, float>(input => Convert.ToSingle(input));
    }

    void AttachExternalEventListener()
    {
        gameObject.AddComponent<UpdateEventListener>();
        gameObject.AddComponent<ApplicationEventListener>();
    }
}