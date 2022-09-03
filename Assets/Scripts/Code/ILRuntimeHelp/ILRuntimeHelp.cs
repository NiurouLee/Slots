using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using ILRuntime.CLR.TypeSystem;
using ILRuntime.CLR.Method;
using ILRuntime.CLR.Utils;
using ILRuntime.Runtime.Intepreter;
using ILRuntime.Runtime.Stack;
using UnityEngine.Networking;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Text;
using BestHTTP.SocketIO.Events;
using DragonU3DSDK;
using DragonU3DSDK.Network.API.Protocol;
using ILRuntime.Runtime.Enviorment;
using ILRuntimeAdapters;
using UnityEngine.ResourceManagement;

public class ILRuntimeHelp
{
    //AppDomain是ILRuntime的入口，最好是在一个单例类中保存，整个游戏全局就一个，这里为了示例方便，每个例子里面都单独做了一个
    //大家在正式项目中请全局只创建一个AppDomain
    //private static System.IO.MemoryStream fs;
    private static Stream fs;
    private static System.IO.MemoryStream p;

    public static ILRuntime.Runtime.Enviorment.AppDomain appdomain;

    public static IEnumerator LoadGameModule(Action loadedFinish)
    {

        PerformanceTracker.AddTrackPoint("StartLoadGameCode");
        AsyncOperationHandle<TextAsset> handle = Addressables.LoadAssetAsync<TextAsset>("GameModule");
 
        yield return handle;
        PerformanceTracker.FinishTrackPoint("StartLoadGameCode");
#if !PRODUCTION_PACKAGE
        
        PerformanceTracker.AddTrackPoint("StartLoadGameCodePDB");
        AsyncOperationHandle<TextAsset> handlePdb = Addressables.LoadAssetAsync<TextAsset>("GameModule.PDB");
        yield return handlePdb;
       
        PerformanceTracker.FinishTrackPoint("StartLoadGameCodePDB");
        
        //首先实例化ILRuntime的AppDomain，AppDomain是一个应用程序域，每个AppDomain都是一个独立的沙盒
        appdomain = new ILRuntime.Runtime.Enviorment.AppDomain();

        Init(handle.Result.bytes, handlePdb.Result.bytes);

        Addressables.Release(handle);
        Addressables.Release(handlePdb);

#else
        //首先实例化ILRuntime的AppDomain，AppDomain是一个应用程序域，每个AppDomain都是一个独立的沙盒
        appdomain = new ILRuntime.Runtime.Enviorment.AppDomain();

        Init(handle.Result.bytes,null);

        Addressables.Release(handle);
    
#endif
        InPackageBIManager.SendGameEvent(BiEventFortuneX.Types.GameEventType.GameEventDownloadCode);
        loadedFinish?.Invoke();
    }

    /*
    public static IEnumerator LoadILRuntime(Action LoadedFinish)
    {
        //首先实例化ILRuntime的AppDomain，AppDomain是一个应用程序域，每个AppDomain都是一个独立的沙盒
        appdomain = new ILRuntime.Runtime.Enviorment.AppDomain();

        //正常项目中应该是自行从其他地方下载dll，或者打包在AssetBundle中读取，平时开发以及为了演示方便直接从StreammingAssets中读取，
#if UNITY_EDITOR
        string path = "file:///" + Application.streamingAssetsPath + "/GameModule.dll";
#else
        string path = Application.streamingAssetsPath + "/GameModule.dll";
#endif
        UnityWebRequest webRequest = UnityWebRequest.Get(path);
        yield return webRequest.SendWebRequest();

        if (webRequest.isNetworkError || webRequest.isHttpError)
            Debug.Log("load dll error");
        byte[] dll = webRequest.downloadHandler.data;
        webRequest = null;

        Init(dll, null);

        LoadedFinish?.Invoke();
    }*/

    public static string dllKey = "fdsajHej^&&#jde7";

    public static bool isDLLEncrypt = true;

    public static void Init(byte[] dll, byte[] pdb)
    {
        //fs = new MemoryStream(dll);
        if (isDLLEncrypt)
        {
            fs = new DllStream(dll, dllKey);
        }
        else
        {
            fs = new MemoryStream(dll);
        }

        //p = new MemoryStream(pdb);


        try
        {
            //appdomain.LoadAssembly(fs, p, new ILRuntime.Mono.Cecil.Pdb.PdbReaderProvider());
#if UNITY_EDITOR
            var pdbAsset = UnityEditor.AssetDatabase.LoadAssetAtPath<TextAsset>("Assets/GameModuleDll/GameModule.PDB.bytes");
            p = new MemoryStream(pdbAsset.bytes);
            appdomain.LoadAssembly(fs, p, new ILRuntime.Mono.Cecil.Pdb.PdbReaderProvider());
#elif !PRODUCTION_PACKAGE
           p = new MemoryStream(pdb);
            appdomain.LoadAssembly(fs,p,new ILRuntime.Mono.Cecil.Pdb.PdbReaderProvider());
#else
            appdomain.LoadAssembly(fs);
#endif

        }
        catch (Exception e)
        {
            Debug.LogError(
                "加载热更DLL失败，请确保已经通过VS打开Assets/Samples/ILRuntime/1.6/Demo/GameModule_Project/GameModule_Project.sln编译过热更DLL");
            Debug.LogException(e);
        }

        //Launch.launch.debugTxt.text += "init over****";
        InitializeILRuntime();
    }

    private unsafe static void InitializeILRuntime()
    {
#if DEBUG && (UNITY_EDITOR || UNITY_ANDROID || UNITY_IPHONE)
        //由于Unity的Profiler接口只允许在主线程使用，为了避免出异常，需要告诉ILRuntime主线程的线程ID才能正确将函数运行耗时报告给Profiler
        appdomain.UnityMainThreadID = System.Threading.Thread.CurrentThread.ManagedThreadId;
#endif
        //这里做一些ILRuntime的注册

        appdomain.RegisterCrossBindingAdaptor(new MonoBehaviourAdapter());
        appdomain.RegisterCrossBindingAdaptor(new CoroutineAdapter());

        appdomain.RegisterCrossBindingAdaptor(new IAsyncStateMachineAdapter());
        appdomain.RegisterCrossBindingAdaptor(new Adapt_IMessage());

        appdomain.RegisterCrossBindingAdaptor(new ConfigManagerBaseAdapter());

        //appdomain.RegisterCrossBindingAdaptor(new ScrollViewHelperAdapter());
        //appdomain.RegisterValueTypeBinder(typeof(Vector3), new Vector3Binder());

        appdomain.RegisterValueTypeBinder(typeof(Vector3), new Vector3Binder());
        appdomain.RegisterValueTypeBinder(typeof(Quaternion), new QuaternionBinder());
        appdomain.RegisterValueTypeBinder(typeof(Vector2), new Vector2Binder());



        appdomain.DelegateManager.RegisterMethodDelegate<BestHTTP.SocketIO.Socket, BestHTTP.SocketIO.Packet, object[]>();
        appdomain.DelegateManager.RegisterMethodDelegate<DragonU3DSDK.Network.API.Protocol.ErrorCode, System.String, Google.ilruntime.Protobuf.IMessage>();
        appdomain.DelegateManager.RegisterMethodDelegate<Google.ilruntime.Protobuf.IMessage>();
        appdomain.DelegateManager.RegisterFunctionDelegate<global::Adapt_IMessage.Adaptor, global::Adapt_IMessage.Adaptor, System.Int32>();

        appdomain.DelegateManager.RegisterMethodDelegate<DragonU3DSDK.Network.API.Protocol.ErrorCode, System.String, Google.Protobuf.IMessage>();
        appdomain.DelegateManager.RegisterMethodDelegate<Google.Protobuf.IMessage>();

        appdomain.DelegateManager.RegisterDelegateConvertor<SocketIOCallback>((action) =>
        {
            //转换器的目的是把Action或者Func转换成正确的类型，这里则是把Action<int>转换成TestDelegateMethod
            return new SocketIOCallback((a, b, c) =>
            {
                //调用委托实例
                ((System.Action<BestHTTP.SocketIO.Socket, BestHTTP.SocketIO.Packet, object[]>)action)(a, b, c);
            });
        });
        appdomain.DelegateManager.RegisterDelegateConvertor<SocketIOAckCallback>((action) =>
        {
            //转换器的目的是把Action或者Func转换成正确的类型，这里则是把Action<int>转换成TestDelegateMethod
            return new SocketIOAckCallback((a, b, c) =>
            {
                //调用委托实例
                ((System.Action<BestHTTP.SocketIO.Socket, BestHTTP.SocketIO.Packet, object[]>)action)(a, b, c);
            });
        });


        appdomain.DelegateManager.RegisterFunctionDelegate<Adapt_IMessage.Adaptor>();
        appdomain.DelegateManager
            .RegisterMethodDelegate<
                UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<UnityEngine.GameObject>>();
        appdomain.DelegateManager.RegisterDelegateConvertor<UnityEngine.Events.UnityAction>((act) =>
        {
            return new UnityEngine.Events.UnityAction(() => { ((Action)act)(); });
        });
        appdomain.DelegateManager.RegisterDelegateConvertor<DG.Tweening.Core.DOSetter<UnityEngine.Color>>((act) =>
        {
            return new DG.Tweening.Core.DOSetter<UnityEngine.Color>((pNewValue) =>
            {
                ((Action<UnityEngine.Color>)act)(pNewValue);
            });
        });
        appdomain.DelegateManager.RegisterDelegateConvertor<DG.Tweening.Core.DOGetter<UnityEngine.Color>>((act) =>
        {
            return new DG.Tweening.Core.DOGetter<UnityEngine.Color>(() =>
            {
                return ((Func<UnityEngine.Color>)act)();
            });
        });
        appdomain.DelegateManager.RegisterDelegateConvertor<DG.Tweening.Core.DOGetter<UnityEngine.Color>>((act) =>
        {
            return new DG.Tweening.Core.DOGetter<UnityEngine.Color>(() =>
            {
                return ((Func<UnityEngine.Color>)act)();
            });
        });
        appdomain.DelegateManager.RegisterDelegateConvertor<DG.Tweening.TweenCallback>((act) =>
        {
            return new DG.Tweening.TweenCallback(() => { ((Action)act)(); });
        });
        appdomain.DelegateManager.RegisterDelegateConvertor<System.AsyncCallback>((act) =>
        {
            return new System.AsyncCallback((ar) => { ((Action<System.IAsyncResult>)act)(ar); });
        });
        appdomain.DelegateManager.RegisterDelegateConvertor<LitJson.ImporterFunc<System.Int64, System.String>>((act) =>
        {
            return new LitJson.ImporterFunc<System.Int64, System.String>((input) =>
            {
                return ((Func<System.Int64, System.String>)act)(input);
            });
        });
        appdomain.DelegateManager.RegisterDelegateConvertor<LitJson.ImporterFunc<System.String, System.Int64>>((act) =>
        {
            return new LitJson.ImporterFunc<System.String, System.Int64>((input) =>
            {
                return ((Func<System.String, System.Int64>)act)(input);
            });
        });

        appdomain.DelegateManager.RegisterDelegateConvertor<LitJson.ImporterFunc<System.Int32, System.String>>((act) =>
        {
            return new LitJson.ImporterFunc<System.Int32, System.String>((input) =>
            {
                return ((Func<System.Int32, System.String>)act)(input);
            });
        });
        appdomain.DelegateManager.RegisterDelegateConvertor<LitJson.ImporterFunc<System.String, System.Int32>>((act) =>
        {
            return new LitJson.ImporterFunc<System.String, System.Int32>((input) =>
            {
                return ((Func<System.String, System.Int32>)act)(input);
            });
        });
        appdomain.DelegateManager.RegisterDelegateConvertor<LitJson.ImporterFunc<System.Double, System.Single>>((act) =>
        {
            return new LitJson.ImporterFunc<System.Double, System.Single>((input) =>
            {
                return ((Func<System.Double, System.Single>)act)(input);
            });
        });

        appdomain.DelegateManager.RegisterDelegateConvertor<LitJson.ImporterFunc<System.Single, System.Double>>((act) =>
        {
            return new LitJson.ImporterFunc<System.Single, System.Double>((input) =>
            {
                return ((Func<System.Single, System.Double>)act)(input);
            });
        });

        appdomain.DelegateManager.RegisterDelegateConvertor<UnityEngine.Events.UnityAction<System.String>>((act) =>
        {
            return new UnityEngine.Events.UnityAction<System.String>((input) =>
            {
                ((Action<System.String>)act)(input);
            });
        });

        appdomain.DelegateManager.RegisterDelegateConvertor<UnityEngine.Events.UnityAction<System.Boolean>>((act) =>
        {
            return new UnityEngine.Events.UnityAction<System.Boolean>((input) =>
            {
                ((Action<System.Boolean>)act)(input);
            });
        });

        appdomain.DelegateManager.RegisterMethodDelegate<System.String, System.Action<UnityEngine.U2D.SpriteAtlas>>();

        appdomain.DelegateManager.RegisterMethodDelegate<ILTypeInstance>();
        appdomain.DelegateManager.RegisterMethodDelegate<System.Object>();
        appdomain.DelegateManager.RegisterFunctionDelegate<System.Object, System.Object>();

        appdomain.DelegateManager
            .RegisterMethodDelegate<UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<
                System.Collections.Generic.IList<UnityEngine.GameObject>>>();
        appdomain.DelegateManager
            .RegisterMethodDelegate<
                UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<UnityEngine.Object>>();
        appdomain.DelegateManager
            .RegisterMethodDelegate<UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<
                UnityEngine.AddressableAssets.ResourceLocators.IResourceLocator>>();
        appdomain.DelegateManager
            .RegisterMethodDelegate<
                UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<UnityEngine.TextAsset>>();
        appdomain.DelegateManager
            .RegisterMethodDelegate<UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<
                UnityEngine.U2D.SpriteAtlas>>();

        appdomain.DelegateManager.RegisterMethodDelegate<UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<System.Int64>>();

        appdomain.DelegateManager.RegisterMethodDelegate<DragonU3DSDK.SDKEvents.ThirdAlreadyBindErrorEvent>();

        appdomain.DelegateManager.RegisterMethodDelegate<BestHTTP.HTTPRequest, BestHTTP.HTTPResponse>();
        appdomain.DelegateManager.RegisterMethodDelegate<Facebook.Unity.IGraphResult>();

        appdomain.DelegateManager.RegisterMethodDelegate<System.Int32>();
        appdomain.DelegateManager.RegisterMethodDelegate<UnityEngine.Color>();
        appdomain.DelegateManager.RegisterFunctionDelegate<UnityEngine.Color>();
        appdomain.DelegateManager.RegisterMethodDelegate<System.IAsyncResult>();
        appdomain.DelegateManager.RegisterMethodDelegate<SimpleJson.JsonObject>();
        appdomain.DelegateManager.RegisterMethodDelegate<System.Int64>();
        appdomain.DelegateManager.RegisterMethodDelegate<System.Object>();
        appdomain.DelegateManager.RegisterMethodDelegate<System.Int64, System.Int64>();
        appdomain.DelegateManager.RegisterMethodDelegate<System.Int64, System.Int64, System.Double>();
        appdomain.DelegateManager.RegisterMethodDelegate<System.Single>();
        appdomain.DelegateManager.RegisterFunctionDelegate<System.Int64, System.String>();
        appdomain.DelegateManager.RegisterFunctionDelegate<System.String, System.Int64>();
        appdomain.DelegateManager.RegisterFunctionDelegate<System.Int32, System.String>();
        appdomain.DelegateManager.RegisterFunctionDelegate<System.String, System.Int32>();
        appdomain.DelegateManager.RegisterFunctionDelegate<System.Double, System.Single>();
        appdomain.DelegateManager.RegisterFunctionDelegate<System.Single, System.Double>();
        appdomain.DelegateManager.RegisterMethodDelegate<UnityEngine.GameObject>();
        appdomain.DelegateManager.RegisterMethodDelegate<ILRuntime.Runtime.Intepreter.ILTypeInstance>();
        appdomain.DelegateManager.RegisterMethodDelegate<UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle>();
        appdomain.DelegateManager.RegisterMethodDelegate<System.String>();
        appdomain.DelegateManager.RegisterMethodDelegate<System.Double>();

        appdomain.DelegateManager.RegisterDelegateConvertor<Facebook.Unity.FacebookDelegate<Facebook.Unity.IGraphResult>>((act) =>
        {
            return new Facebook.Unity.FacebookDelegate<Facebook.Unity.IGraphResult>((value) =>
            {
                ((Action<Facebook.Unity.IGraphResult>)act)(value);
            });
        });

        appdomain.DelegateManager.RegisterDelegateConvertor<DG.Tweening.TweenCallback<System.Single>>((act) =>
        {
            return new DG.Tweening.TweenCallback<System.Single>((value) =>
            {
                ((Action<System.Single>)act)(value);
            });
        });
        appdomain.DelegateManager
            .RegisterFunctionDelegate<global::MonoBehaviourAdapter.Adaptor, System.Int32,
                global::MonoBehaviourAdapter.Adaptor>();
        appdomain.DelegateManager.RegisterMethodDelegate<System.Boolean>();

        appdomain.DelegateManager.RegisterFunctionDelegate<System.Int64, System.Boolean>();
        appdomain.DelegateManager.RegisterFunctionDelegate<System.Int32, System.Boolean>();
        appdomain.DelegateManager.RegisterFunctionDelegate<System.Double, System.Boolean>();
        appdomain.DelegateManager.RegisterFunctionDelegate<System.Single, System.Boolean>();
        appdomain.DelegateManager.RegisterFunctionDelegate<System.String, System.Boolean>();
        appdomain.DelegateManager
            .RegisterFunctionDelegate<ILRuntime.Runtime.Intepreter.ILTypeInstance, System.Boolean>();
        appdomain.DelegateManager.RegisterFunctionDelegate<System.Int64, System.Int64, System.Int32>();

        appdomain.DelegateManager.RegisterFunctionDelegate<System.Int32, System.Int32, System.Int32>();
        appdomain.DelegateManager.RegisterFunctionDelegate<System.Double, System.Double, System.Int32>();
        appdomain.DelegateManager.RegisterFunctionDelegate<System.Single, System.Single, System.Int32>();
        appdomain.DelegateManager.RegisterFunctionDelegate<System.String, System.String, System.Int32>();
        appdomain.DelegateManager
            .RegisterFunctionDelegate<ILRuntime.Runtime.Intepreter.ILTypeInstance,
                ILRuntime.Runtime.Intepreter.ILTypeInstance, System.Int32>();
        appdomain.DelegateManager
            .RegisterFunctionDelegate<System.Collections.Generic.List<ILRuntime.Runtime.Intepreter.ILTypeInstance>,
                System.Collections.Generic.List<ILRuntime.Runtime.Intepreter.ILTypeInstance>, System.Int32>();

        appdomain.DelegateManager.RegisterFunctionDelegate<System.Int32>();
        appdomain.DelegateManager.RegisterFunctionDelegate<System.Int16>();
        appdomain.DelegateManager.RegisterFunctionDelegate<System.Int64>();
        appdomain.DelegateManager.RegisterFunctionDelegate<System.Single>();
        appdomain.DelegateManager.RegisterFunctionDelegate<System.Double>();
        appdomain.DelegateManager.RegisterFunctionDelegate<System.Boolean>();
        appdomain.DelegateManager.RegisterFunctionDelegate<System.String>();
        appdomain.DelegateManager.RegisterFunctionDelegate<ILRuntime.Runtime.Intepreter.ILTypeInstance>();

        appdomain.DelegateManager.RegisterFunctionDelegate<Quaternion>();
        appdomain.DelegateManager.RegisterFunctionDelegate<Vector3>();
        appdomain.DelegateManager.RegisterFunctionDelegate<Vector4>();
        appdomain.DelegateManager.RegisterFunctionDelegate<Vector2>();
        appdomain.DelegateManager.RegisterFunctionDelegate<Rect>();
        appdomain.DelegateManager.RegisterFunctionDelegate<RectOffset>();

        appdomain.DelegateManager.RegisterDelegateConvertor<DG.Tweening.Core.DOGetter<System.Int64>>((act) =>
        {
            return new DG.Tweening.Core.DOGetter<System.Int64>(() => { return ((Func<System.Int64>)act)(); });
        });
        appdomain.DelegateManager.RegisterDelegateConvertor<DG.Tweening.Core.DOGetter<System.String>>((act) =>
        {
            return new DG.Tweening.Core.DOGetter<System.String>(() => { return ((Func<System.String>)act)(); });
        });
        appdomain.DelegateManager.RegisterDelegateConvertor<DG.Tweening.Core.DOGetter<System.Single>>((act) =>
        {
            return new DG.Tweening.Core.DOGetter<System.Single>(() => { return ((Func<System.Single>)act)(); });
        });
        appdomain.DelegateManager.RegisterDelegateConvertor<DG.Tweening.Core.DOGetter<System.Double>>((act) =>
        {
            return new DG.Tweening.Core.DOGetter<System.Double>(() => { return ((Func<System.Double>)act)(); });
        });
        appdomain.DelegateManager.RegisterDelegateConvertor<DG.Tweening.Core.DOGetter<System.Int32>>((act) =>
        {
            return new DG.Tweening.Core.DOGetter<System.Int32>(() => { return ((Func<System.Int32>)act)(); });
        });
        appdomain.DelegateManager.RegisterFunctionDelegate<UnityEngine.GameObject, System.Boolean>();
        appdomain.DelegateManager.RegisterFunctionDelegate<System.Tuple<System.Int32, System.Int32>, System.Tuple<System.Int32, System.Int32>, System.Int32>();
        appdomain.DelegateManager.RegisterFunctionDelegate<System.Tuple<System.Int32, System.Int32>, System.Boolean>();
        appdomain.DelegateManager.RegisterDelegateConvertor<System.Predicate<System.Tuple<System.Int32, System.Int32>>>((act) =>
        {
            return new System.Predicate<System.Tuple<System.Int32, System.Int32>>((obj) =>
            {
                return ((Func<System.Tuple<System.Int32, System.Int32>, System.Boolean>)act)(obj);
            });
        });
        appdomain.DelegateManager
            .RegisterDelegateConvertor<DG.Tweening.Core.DOGetter<ILRuntime.Runtime.Intepreter.ILTypeInstance>>((act) =>
            {
                return new DG.Tweening.Core.DOGetter<ILRuntime.Runtime.Intepreter.ILTypeInstance>(() =>
                {
                    return ((Func<ILRuntime.Runtime.Intepreter.ILTypeInstance>)act)();
                });
            });
        appdomain.DelegateManager.RegisterDelegateConvertor<DG.Tweening.Core.DOGetter<UnityEngine.Color>>((act) =>
        {
            return new DG.Tweening.Core.DOGetter<UnityEngine.Color>(() =>
            {
                return ((Func<UnityEngine.Color>)act)();
            });
        });
        appdomain.DelegateManager.RegisterDelegateConvertor<DG.Tweening.Core.DOGetter<UnityEngine.Quaternion>>((act) =>
        {
            return new DG.Tweening.Core.DOGetter<UnityEngine.Quaternion>(() =>
            {
                return ((Func<UnityEngine.Quaternion>)act)();
            });
        });
        appdomain.DelegateManager.RegisterDelegateConvertor<DG.Tweening.Core.DOGetter<UnityEngine.Vector2>>((act) =>
        {
            return new DG.Tweening.Core.DOGetter<UnityEngine.Vector2>(() =>
            {
                return ((Func<UnityEngine.Vector2>)act)();
            });
        });
        appdomain.DelegateManager.RegisterDelegateConvertor<DG.Tweening.Core.DOGetter<UnityEngine.Vector3>>((act) =>
        {
            return new DG.Tweening.Core.DOGetter<UnityEngine.Vector3>(() =>
            {
                return ((Func<UnityEngine.Vector3>)act)();
            });
        });
        appdomain.DelegateManager.RegisterDelegateConvertor<DG.Tweening.Core.DOGetter<UnityEngine.Vector4>>((act) =>
        {
            return new DG.Tweening.Core.DOGetter<UnityEngine.Vector4>(() =>
            {
                return ((Func<UnityEngine.Vector4>)act)();
            });
        });
        appdomain.DelegateManager.RegisterDelegateConvertor<DG.Tweening.Core.DOGetter<UnityEngine.Rect>>((act) =>
        {
            return new DG.Tweening.Core.DOGetter<UnityEngine.Rect>(() =>
            {
                return ((Func<UnityEngine.Rect>)act)();
            });
        });
        appdomain.DelegateManager.RegisterDelegateConvertor<DG.Tweening.Core.DOGetter<UnityEngine.RectOffset>>((act) =>
        {
            return new DG.Tweening.Core.DOGetter<UnityEngine.RectOffset>(() =>
            {
                return ((Func<UnityEngine.RectOffset>)act)();
            });
        });

        appdomain.DelegateManager.RegisterDelegateConvertor<System.Predicate<System.Int64>>((act) =>
        {
            return new System.Predicate<System.Int64>((obj) =>
            {
                return ((Func<System.Int64, System.Boolean>)act)(obj);
            });
        });

        appdomain.DelegateManager.RegisterDelegateConvertor<System.Predicate<System.Int32>>((act) =>
        {
            return new System.Predicate<System.Int32>((obj) =>
            {
                return ((Func<System.Int32, System.Boolean>)act)(obj);
            });
        });

        appdomain.DelegateManager.RegisterDelegateConvertor<System.Predicate<System.Double>>((act) =>
        {
            return new System.Predicate<System.Double>((obj) =>
            {
                return ((Func<System.Double, System.Boolean>)act)(obj);
            });
        });

        appdomain.DelegateManager.RegisterDelegateConvertor<System.Predicate<System.Single>>((act) =>
        {
            return new System.Predicate<System.Single>((obj) =>
            {
                return ((Func<System.Single, System.Boolean>)act)(obj);
            });
        });

        appdomain.DelegateManager.RegisterDelegateConvertor<System.Predicate<System.String>>((act) =>
        {
            return new System.Predicate<System.String>((obj) =>
            {
                return ((Func<System.String, System.Boolean>)act)(obj);
            });
        });

        appdomain.DelegateManager
            .RegisterDelegateConvertor<System.Predicate<ILRuntime.Runtime.Intepreter.ILTypeInstance>>((act) =>
            {
                return new System.Predicate<ILRuntime.Runtime.Intepreter.ILTypeInstance>((obj) =>
                {
                    return ((Func<ILRuntime.Runtime.Intepreter.ILTypeInstance, System.Boolean>)act)(obj);
                });
            });

        appdomain.DelegateManager.RegisterDelegateConvertor<System.Comparison<System.Int64>>((act) =>
        {
            return new System.Comparison<System.Int64>((x, y) =>
            {
                return ((Func<System.Int64, System.Int64, System.Int32>)act)(x, y);
            });
        });

        appdomain.DelegateManager.RegisterDelegateConvertor<System.Comparison<System.Int32>>((act) =>
        {
            return new System.Comparison<System.Int32>((x, y) =>
            {
                return ((Func<System.Int32, System.Int32, System.Int32>)act)(x, y);
            });
        });

        appdomain.DelegateManager.RegisterDelegateConvertor<System.Comparison<System.Double>>((act) =>
        {
            return new System.Comparison<System.Double>((x, y) =>
            {
                return ((Func<System.Double, System.Double, System.Int32>)act)(x, y);
            });
        });

        appdomain.DelegateManager.RegisterDelegateConvertor<System.Comparison<System.Single>>((act) =>
        {
            return new System.Comparison<System.Single>((x, y) =>
            {
                return ((Func<System.Single, System.Single, System.Int32>)act)(x, y);
            });
        });

        appdomain.DelegateManager.RegisterDelegateConvertor<System.Comparison<System.String>>((act) =>
        {
            return new System.Comparison<System.String>((x, y) =>
            {
                return ((Func<System.String, System.String, System.Int32>)act)(x, y);
            });
        });

        appdomain.DelegateManager
            .RegisterDelegateConvertor<System.Comparison<ILRuntime.Runtime.Intepreter.ILTypeInstance>>((act) =>
            {
                return new System.Comparison<ILRuntime.Runtime.Intepreter.ILTypeInstance>((x, y) =>
                {
                    return ((Func<ILRuntime.Runtime.Intepreter.ILTypeInstance,
                        ILRuntime.Runtime.Intepreter.ILTypeInstance, System.Int32>)act)(x, y);
                });
            });

        appdomain.DelegateManager
            .RegisterDelegateConvertor<
                System.Comparison<System.Collections.Generic.List<ILRuntime.Runtime.Intepreter.ILTypeInstance>>>(
                (act) =>
                {
                    return new
                        System.Comparison<System.Collections.Generic.List<ILRuntime.Runtime.Intepreter.ILTypeInstance>>(
                            (x, y) =>
                            {
                                return ((Func<
                                    System.Collections.Generic.List<ILRuntime.Runtime.Intepreter.ILTypeInstance>,
                                    System.Collections.Generic.List<ILRuntime.Runtime.Intepreter.ILTypeInstance>,
                                    System.Int32>)act)(x, y);
                            });
                });

        appdomain.DelegateManager.RegisterDelegateConvertor<DG.Tweening.Core.DOSetter<System.Int64>>((act) =>
        {
            return new DG.Tweening.Core.DOSetter<System.Int64>((pNewValue) =>
            {
                ((Action<System.Int64>)act)(pNewValue);
            });
        });
        appdomain.DelegateManager.RegisterDelegateConvertor<DG.Tweening.Core.DOSetter<System.String>>((act) =>
        {
            return new DG.Tweening.Core.DOSetter<System.String>((pNewValue) =>
            {
                ((Action<System.String>)act)(pNewValue);
            });
        });
        appdomain.DelegateManager.RegisterDelegateConvertor<DG.Tweening.Core.DOSetter<System.Int32>>((act) =>
        {
            return new DG.Tweening.Core.DOSetter<System.Int32>((pNewValue) =>
            {
                ((Action<System.Int32>)act)(pNewValue);
            });
        });
        appdomain.DelegateManager.RegisterDelegateConvertor<DG.Tweening.Core.DOSetter<System.Boolean>>((act) =>
        {
            return new DG.Tweening.Core.DOSetter<System.Boolean>((pNewValue) =>
            {
                ((Action<System.Boolean>)act)(pNewValue);
            });
        });
        appdomain.DelegateManager.RegisterDelegateConvertor<DG.Tweening.Core.DOSetter<System.Single>>((act) =>
        {
            return new DG.Tweening.Core.DOSetter<System.Single>((pNewValue) =>
            {
                ((Action<System.Single>)act)(pNewValue);
            });
        });
        appdomain.DelegateManager.RegisterDelegateConvertor<DG.Tweening.Core.DOSetter<System.Double>>((act) =>
        {
            return new DG.Tweening.Core.DOSetter<System.Double>((pNewValue) =>
            {
                ((Action<System.Double>)act)(pNewValue);
            });
        });
        appdomain.DelegateManager.RegisterDelegateConvertor<DG.Tweening.Core.DOSetter<UnityEngine.Color>>((act) =>
        {
            return new DG.Tweening.Core.DOSetter<UnityEngine.Color>((pNewValue) =>
            {
                ((Action<UnityEngine.Color>)act)(pNewValue);
            });
        });
        appdomain.DelegateManager.RegisterDelegateConvertor<DG.Tweening.Core.DOSetter<UnityEngine.Vector2>>((act) =>
        {
            return new DG.Tweening.Core.DOSetter<UnityEngine.Vector2>((pNewValue) =>
            {
                ((Action<UnityEngine.Vector2>)act)(pNewValue);
            });
        });
        appdomain.DelegateManager.RegisterDelegateConvertor<DG.Tweening.Core.DOSetter<UnityEngine.Vector3>>((act) =>
        {
            return new DG.Tweening.Core.DOSetter<UnityEngine.Vector3>((pNewValue) =>
            {
                ((Action<UnityEngine.Vector3>)act)(pNewValue);
            });
        });
        appdomain.DelegateManager.RegisterDelegateConvertor<DG.Tweening.Core.DOSetter<UnityEngine.Vector4>>((act) =>
        {
            return new DG.Tweening.Core.DOSetter<UnityEngine.Vector4>((pNewValue) =>
            {
                ((Action<UnityEngine.Vector4>)act)(pNewValue);
            });
        });
        appdomain.DelegateManager.RegisterDelegateConvertor<DG.Tweening.Core.DOSetter<UnityEngine.RectOffset>>((act) =>
        {
            return new DG.Tweening.Core.DOSetter<UnityEngine.RectOffset>((pNewValue) =>
            {
                ((Action<UnityEngine.RectOffset>)act)(pNewValue);
            });
        });
        appdomain.DelegateManager.RegisterDelegateConvertor<DG.Tweening.Core.DOSetter<UnityEngine.Rect>>((act) =>
        {
            return new DG.Tweening.Core.DOSetter<UnityEngine.Rect>((pNewValue) =>
            {
                ((Action<UnityEngine.Rect>)act)(pNewValue);
            });
        });
        appdomain.DelegateManager.RegisterDelegateConvertor<DG.Tweening.Core.DOSetter<UnityEngine.Quaternion>>((act) =>
        {
            return new DG.Tweening.Core.DOSetter<UnityEngine.Quaternion>((pNewValue) =>
            {
                ((Action<UnityEngine.Quaternion>)act)(pNewValue);
            });
        });
        appdomain.DelegateManager
            .RegisterDelegateConvertor<DG.Tweening.Core.DOSetter<ILRuntime.Runtime.Intepreter.ILTypeInstance>>((act) =>
            {
                return new DG.Tweening.Core.DOSetter<ILRuntime.Runtime.Intepreter.ILTypeInstance>((pNewValue) =>
                {
                    ((Action<ILRuntime.Runtime.Intepreter.ILTypeInstance>)act)(pNewValue);
                });
            });

        appdomain.DelegateManager.RegisterDelegateConvertor<UnityEngine.Video.VideoPlayer.EventHandler>((act) =>
            {
                return new UnityEngine.Video.VideoPlayer.EventHandler((source) =>
                {
                    ((Action<UnityEngine.Video.VideoPlayer>)act)(source);
                });
            });



        appdomain.DelegateManager.RegisterMethodDelegate<System.Action, ILRuntime.Runtime.Intepreter.ILTypeInstance, ILRuntime.Runtime.Intepreter.ILTypeInstance>();
        appdomain.DelegateManager.RegisterMethodDelegate<UnityEngine.Vector2>();
        appdomain.DelegateManager.RegisterMethodDelegate<UnityEngine.Vector3>();
        appdomain.DelegateManager.RegisterMethodDelegate<UnityEngine.Vector4>();
        appdomain.DelegateManager.RegisterMethodDelegate<UnityEngine.Rect>();
        appdomain.DelegateManager.RegisterMethodDelegate<UnityEngine.Quaternion>();
        appdomain.DelegateManager.RegisterMethodDelegate<UnityEngine.Matrix4x4>();
        appdomain.DelegateManager.RegisterMethodDelegate<UnityEngine.Sprite>();

        appdomain.DelegateManager.RegisterDelegateConvertor<UnityEngine.Events.UnityAction<UnityEngine.Vector2>>((act) =>
        {
            return new UnityEngine.Events.UnityAction<UnityEngine.Vector2>((arg0) =>
            {
                ((Action<UnityEngine.Vector2>)act)(arg0);
            });
        });

        appdomain.DelegateManager.RegisterDelegateConvertor<UnityEngine.Events.UnityAction<UnityEngine.Vector3>>((act) =>
        {
            return new UnityEngine.Events.UnityAction<UnityEngine.Vector3>((arg0) =>
            {
                ((Action<UnityEngine.Vector3>)act)(arg0);
            });
        });

        appdomain.DelegateManager.RegisterDelegateConvertor<UnityEngine.Events.UnityAction<UnityEngine.Vector4>>((act) =>
        {
            return new UnityEngine.Events.UnityAction<UnityEngine.Vector4>((arg0) =>
            {
                ((Action<UnityEngine.Vector4>)act)(arg0);
            });
        });

        appdomain.DelegateManager.RegisterDelegateConvertor<UnityEngine.Events.UnityAction<UnityEngine.Quaternion>>((act) =>
        {
            return new UnityEngine.Events.UnityAction<UnityEngine.Quaternion>((arg0) =>
            {
                ((Action<UnityEngine.Quaternion>)act)(arg0);
            });
        });

        appdomain.DelegateManager.RegisterDelegateConvertor<UnityEngine.Events.UnityAction<UnityEngine.Rect>>((act) =>
        {
            return new UnityEngine.Events.UnityAction<UnityEngine.Rect>((arg0) =>
            {
                ((Action<UnityEngine.Rect>)act)(arg0);
            });
        });

        appdomain.DelegateManager.RegisterDelegateConvertor<UnityEngine.Events.UnityAction<System.Single>>((act) =>
        {
            return new UnityEngine.Events.UnityAction<System.Single>((arg0) =>
            {
                ((Action<System.Single>)act)(arg0);
            });
        });
        appdomain.DelegateManager.RegisterDelegateConvertor<System.Comparison<System.Tuple<System.Int32, System.Int32>>>((act) =>
        {
            return new System.Comparison<System.Tuple<System.Int32, System.Int32>>((x, y) =>
             {
                 return ((Func<System.Tuple<System.Int32, System.Int32>, System.Tuple<System.Int32, System.Int32>, System.Int32>)act)(x, y);
             });
        });
        appdomain.DelegateManager.RegisterDelegateConvertor<UnityEngine.Events.UnityAction<System.Boolean>>((act) =>
        {
            return new UnityEngine.Events.UnityAction<System.Boolean>((arg0) =>
            {
                ((Action<System.Boolean>)act)(arg0);
            });
        });

        appdomain.DelegateManager.RegisterMethodDelegate<UnityEngine.Purchasing.Product>();
        appdomain.DelegateManager.RegisterMethodDelegate<UnityEngine.Purchasing.Product[]>();

        appdomain.DelegateManager.RegisterMethodDelegate<UnityEngine.EventSystems.BaseEventData>();
        appdomain.DelegateManager.RegisterMethodDelegate<UnityEngine.EventSystems.PointerEventData>();
        appdomain.DelegateManager.RegisterMethodDelegate<UnityEngine.EventSystems.AxisEventData>();
        appdomain.DelegateManager.RegisterMethodDelegate<UnityEngine.Video.VideoPlayer>();
        appdomain.DelegateManager.RegisterMethodDelegate<UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<System.Collections.Generic.IList<UnityEngine.ResourceManagement.ResourceLocations.IResourceLocation>>>();

        appdomain.DelegateManager.RegisterMethodDelegate<DragonU3DSDK.SDKEvents.SocketIOError>();
        appdomain.DelegateManager.RegisterMethodDelegate<DragonU3DSDK.SDKEvents.SocketIODisconnected>();
        appdomain.DelegateManager.RegisterMethodDelegate<DragonU3DSDK.SDKEvents.ProfileConflictEvent>();
        appdomain.DelegateManager.RegisterMethodDelegate<DragonU3DSDK.SDKEvents.DeepLinkEvent>();
        appdomain.DelegateManager.RegisterMethodDelegate<DragonU3DSDK.SDKEvents.DiskFullEvent>();
        appdomain.DelegateManager.RegisterMethodDelegate<DragonU3DSDK.SDKEvents.ConfigHubUpdatedEvent>();
        appdomain.DelegateManager.RegisterMethodDelegate<DragonU3DSDK.SDKEvents.ThirdAlreadyBindErrorEvent>();
        appdomain.DelegateManager.RegisterMethodDelegate<DragonU3DSDK.SDKEvents.FireBaseOnMessageReceivedEvent>();
        
        appdomain.DelegateManager.RegisterMethodDelegate<DragonU3DSDK.PurchaseCallbackArgs>();
        appdomain.DelegateManager.RegisterMethodDelegate<System.Boolean, System.Byte[]>();
        appdomain.DelegateManager.RegisterMethodDelegate<System.UInt32>();
        appdomain.DelegateManager.RegisterFunctionDelegate<Int32, Int32>();
        appdomain.DelegateManager.RegisterFunctionDelegate<Int32, Int32>();

        appdomain.DelegateManager.RegisterMethodDelegate<System.String, Dlugin.PluginStructs.UserInfo, Dlugin.PluginStructs.SDKError>();
        appdomain.DelegateManager.RegisterDelegateConvertor<DragonPlus.AdLogicManager.FailDelegate>((act) =>
        {
            return new DragonPlus.AdLogicManager.FailDelegate((placementId) =>
            {
                return ((Func<System.String, System.Boolean>)act)(placementId);
            });
        });

        appdomain.DelegateManager.RegisterDelegateConvertor<BestHTTP.OnRequestFinishedDelegate>((act) =>
        {
            return new BestHTTP.OnRequestFinishedDelegate((originalRequest, response) =>
            {
                ((Action<BestHTTP.HTTPRequest, BestHTTP.HTTPResponse>)act)(originalRequest, response);
            });
        });
        
        appdomain.DelegateManager.RegisterDelegateConvertor<System.Comparison<global::Adapt_IMessage.Adaptor>>((act) =>
        {
            return new System.Comparison<global::Adapt_IMessage.Adaptor>((x, y) =>
            {
                return ((Func<global::Adapt_IMessage.Adaptor, global::Adapt_IMessage.Adaptor, System.Int32>)act)(x, y);
            });
        });

        appdomain.DelegateManager.RegisterMethodDelegate<System.Boolean, System.String>();

        SetupCLRRedirection();
        SetupCLRRedirection2();
        var miLog = typeof(Debug).GetMethod("Log", new System.Type[] { typeof(object) });
        appdomain.RegisterCLRMethodRedirection(miLog, Log_11);
        var miLogWarning = typeof(Debug).GetMethod("LogWarning", new System.Type[] { typeof(object) });
        appdomain.RegisterCLRMethodRedirection(miLogWarning, LogWarning);
        var miLogError = typeof(Debug).GetMethod("LogError", new System.Type[] { typeof(object) });
        appdomain.RegisterCLRMethodRedirection(miLogError, LogError);
        var miLogException = typeof(Debug).GetMethod("LogException", new System.Type[] { typeof(object) });
        appdomain.RegisterCLRMethodRedirection(miLogException, LogException);




        LitJson.JsonMapper.RegisterILRuntimeCLRRedirection(appdomain);
        ILRuntime.Runtime.Generated.CLRBindings.Initialize(appdomain);
    }

    unsafe static void SetupCLRRedirection()
    {
        //这里面的通常应该写在InitializeILRuntime，这里为了演示写这里
        var arr = typeof(GameObject).GetMethods();
        foreach (var i in arr)
        {
            if (i.Name == "AddComponent" && i.GetGenericArguments().Length == 1)
            {
                appdomain.RegisterCLRMethodRedirection(i, AddComponent);
            }
        }
    }

    unsafe static void SetupCLRRedirection2()
    {
        //这里面的通常应该写在InitializeILRuntime，这里为了演示写这里
        var arr = typeof(GameObject).GetMethods();
        foreach (var i in arr)
        {
            if (i.Name == "GetComponent" && i.GetGenericArguments().Length == 1)
            {
                //Debug.LogError("****GetComponent");
                appdomain.RegisterCLRMethodRedirection(i, GetComponent);
            }
        }
    }

    unsafe static StackObject* AddComponent(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack,
        CLRMethod __method, bool isNewObj)
    {
        //CLR重定向的说明请看相关文档和教程，这里不多做解释
        ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;

        var ptr = __esp - 1;
        //成员方法的第一个参数为this
        GameObject instance = StackObject.ToObject(ptr, __domain, __mStack) as GameObject;
        if (instance == null)
            throw new System.NullReferenceException();
        __intp.Free(ptr);

        var genericArgument = __method.GenericArguments;
        //AddComponent应该有且只有1个泛型参数
        if (genericArgument != null && genericArgument.Length == 1)
        {
            var type = genericArgument[0];
            object res;
            if (type is CLRType)
            {
                //Unity主工程的类不需要任何特殊处理，直接调用Unity接口
                res = instance.AddComponent(type.TypeForCLR);
            }
            else
            {
                //热更DLL内的类型比较麻烦。首先我们得自己手动创建实例
                var ilInstance =
                    new ILTypeInstance(type as ILType, false); //手动创建实例是因为默认方式会new MonoBehaviour，这在Unity里不允许
                //接下来创建Adapter实例
                var clrInstance = instance.AddComponent<MonoBehaviourAdapter.Adaptor>();
                //unity创建的实例并没有热更DLL里面的实例，所以需要手动赋值
                clrInstance.ILInstance = ilInstance;
                clrInstance.AppDomain = __domain;
                //这个实例默认创建的CLRInstance不是通过AddComponent出来的有效实例，所以得手动替换
                ilInstance.CLRInstance = clrInstance;

                res = clrInstance.ILInstance; //交给ILRuntime的实例应该为ILInstance

                clrInstance.Awake(); //因为Unity调用这个方法时还没准备好所以这里补调一次
            }

            return ILIntepreter.PushObject(ptr, __mStack, res);
        }

        return __esp;
    }

    unsafe static StackObject* GetComponent(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack,
        CLRMethod __method, bool isNewObj)
    {
        //Debug.LogError("&&&GetComponent");
        //CLR重定向的说明请看相关文档和教程，这里不多做解释
        ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;

        var ptr = __esp - 1;
        //成员方法的第一个参数为this
        GameObject instance = StackObject.ToObject(ptr, __domain, __mStack) as GameObject;
        if (instance == null)
            throw new System.NullReferenceException();
        __intp.Free(ptr);

        var genericArgument = __method.GenericArguments;
        //AddComponent应该有且只有1个泛型参数
        if (genericArgument != null && genericArgument.Length == 1)
        {
            var type = genericArgument[0];
            object res = null;
            if (type is CLRType)
            {
                //Unity主工程的类不需要任何特殊处理，直接调用Unity接口
                res = instance.GetComponent(type.TypeForCLR);
            }
            else
            {
                //因为所有DLL里面的MonoBehaviour实际都是这个Component，所以我们只能全取出来遍历查找
                var clrInstances = instance.GetComponents<MonoBehaviourAdapter.Adaptor>();
                for (int i = 0; i < clrInstances.Length; i++)
                {
                    var clrInstance = clrInstances[i];
                    if (clrInstance.ILInstance != null) //ILInstance为null, 表示是无效的MonoBehaviour，要略过
                    {
                        if (clrInstance.ILInstance.Type == type)
                        {
                            res = clrInstance.ILInstance; //交给ILRuntime的实例应该为ILInstance
                            break;
                        }
                    }
                }
            }

            return ILIntepreter.PushObject(ptr, __mStack, res);
        }

        return __esp;
    }

    //编写重定向方法对于刚接触ILRuntime的朋友可能比较困难，比较简单的方式是通过CLR绑定生成绑定代码，然后在这个基础上改，比如下面这个代码是从UnityEngine_Debug_Binding里面复制来改的
    //如何使用CLR绑定请看相关教程和文档
    unsafe static StackObject* Log_11(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack,
        CLRMethod __method, bool isNewObj)
    {
        //ILRuntime的调用约定为被调用者清理堆栈，因此执行这个函数后需要将参数从堆栈清理干净，并把返回值放在栈顶，具体请看ILRuntime实现原理文档
        ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
        StackObject* ptr_of_this_method;
        //这个是最后方法返回后esp栈指针的值，应该返回清理完参数并指向返回值，这里是只需要返回清理完参数的值即可
        StackObject* __ret = ILIntepreter.Minus(__esp, 1);
        //取Log方法的参数，如果有两个参数的话，第一个参数是esp - 2,第二个参数是esp -1, 因为Mono的bug，直接-2值会错误，所以要调用ILIntepreter.Minus
        ptr_of_this_method = ILIntepreter.Minus(__esp, 1);

        //这里是将栈指针上的值转换成object，如果是基础类型可直接通过ptr->Value和ptr->ValueLow访问到值，具体请看ILRuntime实现原理文档
        object message = typeof(object).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
        //所有非基础类型都得调用Free来释放托管堆栈
        __intp.Free(ptr_of_this_method);

        //在真实调用Debug.Log前，我们先获取DLL内的堆栈
        var stacktrace = __domain.DebugService.GetStackTrace(__intp);

        //我们在输出信息后面加上DLL堆栈
        UnityEngine.Debug.Log(message + "\n" + stacktrace);

        return __ret;
    }



    unsafe static StackObject* LogError(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack,
        CLRMethod __method, bool isNewObj)
    {
        //ILRuntime的调用约定为被调用者清理堆栈，因此执行这个函数后需要将参数从堆栈清理干净，并把返回值放在栈顶，具体请看ILRuntime实现原理文档
        ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
        StackObject* ptr_of_this_method;
        //这个是最后方法返回后esp栈指针的值，应该返回清理完参数并指向返回值，这里是只需要返回清理完参数的值即可
        StackObject* __ret = ILIntepreter.Minus(__esp, 1);
        //取Log方法的参数，如果有两个参数的话，第一个参数是esp - 2,第二个参数是esp -1, 因为Mono的bug，直接-2值会错误，所以要调用ILIntepreter.Minus
        ptr_of_this_method = ILIntepreter.Minus(__esp, 1);

        //这里是将栈指针上的值转换成object，如果是基础类型可直接通过ptr->Value和ptr->ValueLow访问到值，具体请看ILRuntime实现原理文档
        object message = typeof(object).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
        //所有非基础类型都得调用Free来释放托管堆栈
        __intp.Free(ptr_of_this_method);

        //在真实调用Debug.Log前，我们先获取DLL内的堆栈
        var stacktrace = __domain.DebugService.GetStackTrace(__intp);

        //我们在输出信息后面加上DLL堆栈
        UnityEngine.Debug.LogError(message + "\n" + stacktrace);

        return __ret;
    }


    unsafe static StackObject* LogWarning(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack,
        CLRMethod __method, bool isNewObj)
    {
        //ILRuntime的调用约定为被调用者清理堆栈，因此执行这个函数后需要将参数从堆栈清理干净，并把返回值放在栈顶，具体请看ILRuntime实现原理文档
        ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
        StackObject* ptr_of_this_method;
        //这个是最后方法返回后esp栈指针的值，应该返回清理完参数并指向返回值，这里是只需要返回清理完参数的值即可
        StackObject* __ret = ILIntepreter.Minus(__esp, 1);
        //取Log方法的参数，如果有两个参数的话，第一个参数是esp - 2,第二个参数是esp -1, 因为Mono的bug，直接-2值会错误，所以要调用ILIntepreter.Minus
        ptr_of_this_method = ILIntepreter.Minus(__esp, 1);

        //这里是将栈指针上的值转换成object，如果是基础类型可直接通过ptr->Value和ptr->ValueLow访问到值，具体请看ILRuntime实现原理文档
        object message = typeof(object).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
        //所有非基础类型都得调用Free来释放托管堆栈
        __intp.Free(ptr_of_this_method);

        //在真实调用Debug.Log前，我们先获取DLL内的堆栈
        var stacktrace = __domain.DebugService.GetStackTrace(__intp);

        //我们在输出信息后面加上DLL堆栈
        UnityEngine.Debug.LogWarning(message + "\n" + stacktrace);

        return __ret;
    }


    unsafe static StackObject* LogException(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack,
        CLRMethod __method, bool isNewObj)
    {
        //ILRuntime的调用约定为被调用者清理堆栈，因此执行这个函数后需要将参数从堆栈清理干净，并把返回值放在栈顶，具体请看ILRuntime实现原理文档
        ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
        StackObject* ptr_of_this_method;
        //这个是最后方法返回后esp栈指针的值，应该返回清理完参数并指向返回值，这里是只需要返回清理完参数的值即可
        StackObject* __ret = ILIntepreter.Minus(__esp, 1);
        //取Log方法的参数，如果有两个参数的话，第一个参数是esp - 2,第二个参数是esp -1, 因为Mono的bug，直接-2值会错误，所以要调用ILIntepreter.Minus
        ptr_of_this_method = ILIntepreter.Minus(__esp, 1);

        //这里是将栈指针上的值转换成object，如果是基础类型可直接通过ptr->Value和ptr->ValueLow访问到值，具体请看ILRuntime实现原理文档
        object message = typeof(object).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
        //所有非基础类型都得调用Free来释放托管堆栈
        __intp.Free(ptr_of_this_method);

        //在真实调用Debug.Log前，我们先获取DLL内的堆栈
        var stacktrace = __domain.DebugService.GetStackTrace(__intp);

        //我们在输出信息后面加上DLL堆栈
        UnityEngine.Debug.LogError(stacktrace + "\n" + message);

        return __ret;
    }

    public static void Dispose()
    {
        fs?.Dispose();
        p?.Dispose();
    }
}