// using System;
// using System.Collections;
// using System.Collections.Generic;
// using System.IO;
// using System.Threading.Tasks;
// using BestHTTP;
// using BestHTTP.Decompression.Zlib;
// using BestHTTP.SocketIO;
// using DragonU3DSDK;
// using DragonU3DSDK.Network.API;
// using DragonU3DSDK.Network.API.Protocol;
// using DragonU3DSDK.Network.BI;
// using Google.ilruntime.Protobuf;
// using Tool;
// using UnityEngine;
// using UnityEngine.UIElements;
//
// namespace GameModule
// {
//     public class APIFixHotAsyncHandler<T> where T : IMessage
//     {
//         public T Response;
//
//         public ErrorCode ErrorCode;
//
//         public string ErrorInfo;
//     }
//
//     public abstract class ApiRequest
//     {
//         public abstract string GetSendProtocolName();
//         public abstract string GetResponseProtocolName();
//     }
//     
//     public class APIRequest<T1, T2> where T1 : IMessage where T2:IMessage
//     {
//         private APIFixHotAsyncHandler<T2> handler;
//         public APIRequest(T1 send)
//         {
//             handler = new APIFixHotAsyncHandler<T2>();
//         }
//     }
//  
//     public class APIManagerGameModule: IUpdateable
//     {
//         public NetworkReachability GetNetworkStatus
//         {
//             get
//             {
//                 return m_NetworkStatus;
//             }
//         }
//         public bool HasNetwork
//         {
//             get
//             {
//                 return m_hasNetwork;
//             }
//         }
//
//         private bool m_hasNetwork = false;
//
//         private NetworkReachability m_NetworkStatus;
//         
//         public bool Inited
//         {
//             get;
//             private set;
//         }
//
//         string host = null;
//         byte[] secret = null;
//         float heartBeatTimer = 0;
//         long serverTimeOffset = 0;
//         ulong lastSyncServerTime = 0;
//         ulong lastSyncLocalTime = 0;
//         bool websocektInitCalled = false;
//         float websocketAutoReconnectTimer = 0;
//         private bool _closeByClient = false;
//
//         private SocketManager.States lastSocketState = SocketManager.States.Closed;
//         private float lastUpdateTime = 0;
//
//         // socket.io
//         private SocketManager socketManager;
//         Dictionary<string, Action<IMessage>> pushCallbacks = new Dictionary<string, Action<IMessage>>();
//         Queue<Tuple<Action<IMessage>, IMessage, string>> respCallbakQueue = new Queue<Tuple<Action<IMessage>, IMessage, string>>();
//         Queue<Tuple<Action<ErrorCode, string, IMessage>, ErrorCode, string, IMessage, string>> errorCallbackQueue = new Queue<Tuple<Action<ErrorCode, string, IMessage>, ErrorCode, string, IMessage, string>>();
//         Queue<Tuple<Action<IMessage>, IMessage, string>> pushCallbackQueue = new Queue<Tuple<Action<IMessage>, IMessage, string>>();
//
//         void Initialize()
//         {
//             secret = System.Text.Encoding.UTF8.GetBytes(ConfigurationController.Instance.APIServerSecret);
//             host = ConfigurationController.Instance.APIServerURL;
//             DebugUtil.Log("api server host = {0}", host);
//             Inited = true;
//         }
//
//         private static APIManagerGameModule _instance;
//         public static APIManagerGameModule Instance
//         {
//             get
//             {
//                 if (_instance == null)
//                 {
//                     _instance = new APIManagerGameModule();
//                 }
//
//                 return _instance;
//             }
//         }
//
//
//         protected APIManagerGameModule()
//         {
//             Awake();
//            
//             if (!Inited)
//             {
//                 Initialize();
//             }
//             
//             StartUpdate();
//         }
//
//
//         ~APIManagerGameModule()
//         {
//             StopUpdate();
//         }
//
//
//         private void Awake()
//         {
//             m_hasNetwork = Application.internetReachability != NetworkReachability.NotReachable;
//             m_NetworkStatus = Application.internetReachability;
//         }
//         
//         public void StopUpdate()
//         {
//             UpdateScheduler.UnhookUpdate(this);
//         }
//
//         public void Clear()
//         {
//             StopUpdate();
//             CloseWebsocket();
//         }
//  
//         void StartUpdate()
//         {
//             UpdateScheduler.HookUpdate(this);
//         }
//
//         public  void Update()
//         {
//             // heartBeatTimer += 33;
//             // if (heartBeatTimer > 5.0f * 1000)
//             // {
//             //
//             //     m_hasNetwork = Application.internetReachability != NetworkReachability.NotReachable;
//             //     m_NetworkStatus = Application.internetReachability;
//             //     heartBeatTimer = 0.0f;
//             //   
//             //     HeartBeat();
//             //    
//             //    //DebugUtil.LogWarning("heart beat finished====1111=====");
//             // }
//
//             // if (websocektInitCalled && !WebsocketConnected) {
//             //     websocketAutoReconnectTimer += 33;
//             //     if (websocketAutoReconnectTimer > 3.0f * 1000) {
//             //         websocketAutoReconnectTimer = 0;
//             //         InitWebsocket();
//             //     }
//             // }
//              
//             if (pushCallbackQueue.Count > 0) {
//                 var tuple = pushCallbackQueue.Dequeue();
//                 try
//                 {
//                     tuple.Item1.Invoke(tuple.Item2);
//                 }
//                 catch (Exception e)
//                 {
//                     BIManager.Instance.SendException(e, 0, tuple.Item3);
//                     
//                 }
//             }
//
//             if (respCallbakQueue.Count > 0) {
//                 var tuple = respCallbakQueue.Dequeue();
//                 try
//                 {
//                     tuple.Item1.Invoke(tuple.Item2);
//                 }
//                 catch (Exception e)
//                 {
//                     Debug.LogException(e);
//                     BIManager.Instance.SendException(e, 0, tuple.Item3);
//                 }
//             }
//
//             if (errorCallbackQueue.Count > 0) {
//                 var tuple = errorCallbackQueue.Dequeue();
//                 try
//                 {
//                     tuple.Item1.Invoke(tuple.Item2, tuple.Item3, tuple.Item4);
//                 }
//                 catch (Exception e)
//                 {
//                     BIManager.Instance.SendException(e, 0, tuple.Item5);
//                 }
//             }
//
//             if (socketManager != null && websocektInitCalled == true)
//             {
//                 var deltaTime = Time.realtimeSinceStartup - lastUpdateTime;
//                 lastUpdateTime = Time.realtimeSinceStartup;
//              
//                 //如果长时间没走Update,直接断掉连接
//                 if (deltaTime > 3600)
//                 {
//                     Clear();
//                     EventBus.Dispatch(new EventNetworkClosed());
//                     return;
//                 }
//                 
//                 if (socketManager.State == SocketManager.States.Closed && socketManager.State != lastSocketState)
//                 {
//                     XDebug.LogError($"EventNetworkClosed:socketManager.State: {socketManager.State}");
//                     EventBus.Dispatch(new EventNetworkClosed());
//                 }
//                 
//                 lastSocketState = socketManager.State;
//             }
//         }
//
//         public async Task HeartBeat()
//         {
//             if (!m_hasNetwork || !WebsocketConnected)
//                 return;
//             
//             var cHeartBeat = new DragonU3DSDK.Network.API.ILProtocol.CHeartBeat { };
//
//             var response = await SendAsync<DragonU3DSDK.Network.API.ILProtocol.CHeartBeat, DragonU3DSDK.Network.API.ILProtocol.SHeartBeat>(cHeartBeat);
//             if (!string.IsNullOrEmpty(response.ErrorInfo))
//             {
//                 DebugUtil.LogWarning("heart beat errno = {0} errmsg = {1}", response.ErrorCode, response.ErrorInfo);
//             }
//             else
//             {
//                 lastSyncLocalTime = DeviceHelper.CurrentTimeMillis();
//                 lastSyncServerTime = response.Response.Timestamp;
//                 serverTimeOffset = (long)lastSyncServerTime - (long)lastSyncLocalTime;
//             }
//
//             //DebugUtil.LogWarning("heart beat finished=========");
//             // Send(cHeartBeat, (SHeartBeat sHeartBeat) =>
//             // {
//             //     lastSyncLocalTime = DeviceHelper.CurrentTimeMillis();
//             //     lastSyncServerTime = sHeartBeat.Timestamp;
//             //     serverTimeOffset = (long)lastSyncServerTime - (long)lastSyncLocalTime;
//             // }, (errno, errmsg, resp) =>
//             // {
//             //     DebugUtil.LogWarning("heart beat errno = {0} errmsg = {1}", errno, errmsg);
//             // });
//         }
//         byte[] gzip(byte[] fi)
//         {
//             using (MemoryStream outFile = new MemoryStream())
//             {
//                 using (MemoryStream inFile = new MemoryStream(fi))
//                 using (GZipStream compress = new GZipStream(outFile, CompressionMode.Compress))
//                 {
//                     inFile.CopyTo(compress);
//                 }
//                 return outFile.ToArray();
//             }
//         }
//
//         byte[] gunzip(byte[] fi)
//         {
//             using (MemoryStream outFile = new MemoryStream())
//             {
//                 using (MemoryStream inFile = new MemoryStream(fi))
//                 using (GZipStream compress = new GZipStream(inFile, CompressionMode.Decompress))
//                 {
//                     compress.CopyTo(outFile);
//                 }
//                 return outFile.ToArray();
//             }
//         }
//
//         protected void ProcessNextApiRequest()
//         {
//             
//         }
//         
//         public async Task<APIFixHotAsyncHandler<T2>> SendAsync<T1, T2>(T1 imessage) where T1 : IMessage where T2 : IMessage,new()
//         {
//             if (!Inited) Initialize();
//              
//             string name1 = typeof(T1).Name;
//             string name2 = typeof(T2).Name;
//             
//             if (name1 == "IMessage")
//             {
//                 DebugUtil.LogWarning("DragonU3DSDK.Network.API.APIManager.Send is deprecated, please use DragonU3DSDK.Network.API.APIManager.Send<T1,T2> instead.");
//             }
//             else if (!(name1[0] == 'C' && name2[0] == 'S' && name1.Substring(1) == name2.Substring(1)))
//             {
//                 var apiAsyncHandler = new APIFixHotAsyncHandler<T2>();
//                 apiAsyncHandler.Response = default(T2);
//                 apiAsyncHandler.ErrorCode = ErrorCode.ParameterError;
//                 apiAsyncHandler.ErrorInfo = "request and response type not match";
//                 DebugUtil.LogError("request type {0} response type {1} not match", name1, name2);
//                 //onError?.Invoke(ErrorCode.ParameterError, "request and response type not match", default(T2));
//                 return apiAsyncHandler;
//             }
//
//             var taskCompletionSource = new TaskCompletionSource<APIFixHotAsyncHandler<T2>>();
//  
//             var handler = new APIFixHotAsyncHandler<T2>();
//             
//             
//             string name = imessage.GetType().Name;
//             if (WebsocketConnected && name != "CLogin")
//             {
//                 if (name != "CHeartBeat")
//                 {
//                     XDebug.Log(string.Format("<color=yellow>{0}</color>", "=======sendWebsocketMessage"));
//                 }
//                 //StartCoroutine(sendWebsocketMessage(imessage, onResponse, onError));
//                 //handler = await sendWebsocketMessageAsync<T1, T2>(imessage);
//
//                 IEnumeratorTool.instance.StartCoroutine(SendWebsocketMessage<T1,T2>(imessage,
//                     (response) =>
//                     {
//                         handler.ErrorCode = ErrorCode.Success;
//                         handler.Response = response;
//                         XDebug.Log($"<color=yellow>=====Websocket taskCompletionSource  onResponse</color>");
//                         taskCompletionSource.SetResult(handler);
//                     }, (errorCode,errorInfo,response) =>
//                     {
//                         handler.ErrorCode = errorCode;
//                         handler.ErrorInfo = errorInfo;
//                         handler.Response = response;
//                         XDebug.Log($"<color=yellow>=====Websocket taskCompletionSource  onError errorCode:{errorCode} errorInfo:{errorInfo}</color>");
//                         taskCompletionSource.SetResult(handler);
//                     }));
//             }
//             else
//             {
//                 XDebug.Log(string.Format("<color=yellow>{0}</color>", "=======sendHttpMessage"));
//                 //handler = await sendAsync<T1,T2>(imessage);
//                 IEnumeratorTool.instance.StartCoroutine(send<T1,T2>(imessage,
//                     (response) =>
//                     {
//                         handler.ErrorCode = ErrorCode.Success;
//                         handler.Response = response;
//                         
//                         XDebug.Log($"<color=yellow>=====http taskCompletionSource  onResponse</color>");
//                         taskCompletionSource.SetResult(handler);
//                     }, (errorCode,errorInfo,response) =>
//                     {
//                         handler.ErrorCode = errorCode;
//                         handler.ErrorInfo = errorInfo;
//                         handler.Response = response;
//                         
//                         XDebug.Log($"<color=yellow>=====http taskCompletionSource  onError</color>");
//                         taskCompletionSource.SetResult(handler);
//                     }));
//             }
//             
//
//             await taskCompletionSource.Task;
//             
// #if (DEBUG || DEVELOPMENT_BUILD) && SHOW_RESPONSE_JSON
//             if (isShowResponseJSON)
//             {
//                 Debug.Log(
//                     $"=======MessageInfo: ErrorCode:{handler.ErrorCode} ErrorInfo:{handler.ErrorInfo} Response:{LitJson.JsonMapper.ToJsonField(handler.Response)}");
//             }
// #endif
//             return handler;
//         }
//
//         public bool isShowResponseJSON = true;
//
//         public ulong GetLastSyncServerTime()
//         {
//             return lastSyncServerTime;
//         }
//
//         public ulong GetServerTime()
//         {
//             var localTime = DeviceHelper.CurrentTimeMillis();
//             if (localTime < lastSyncLocalTime)
//             {
//                 DebugUtil.LogError("时间前调");
//                 return lastSyncLocalTime + (ulong)serverTimeOffset;
//             }
//
//             if (lastSyncLocalTime != 0 && localTime - lastSyncLocalTime > 7200000)
//             {
//                 DebugUtil.LogError("时间前调");
//                 return lastSyncLocalTime + (ulong)serverTimeOffset;
//             }
//
//             return localTime + (ulong)serverTimeOffset;
//         }
//
//         public bool WebsocketConnected
//         {
//             get
//             {
//                 return socketManager != null && socketManager.Socket != null && socketManager.Socket.IsOpen;
//             }
//         }
//         public void CloseWebsocket()
//         {
//             if(socketManager != null)
//                 socketManager.Close();
//         }
//         public void InitWebsocket()
//         {
//             websocektInitCalled = true;
//         
//             if (!(Inited && DragonU3DSDK.Account.AccountManager.Instance.HasLogin && ConfigurationController.Instance.SocketIOEnabled))
//             {
//                 return;
//             }
//
//             if (WebsocketConnected)
//             {
//                 return;
//             }
//             
//             lastUpdateTime = Time.realtimeSinceStartup;
//
//             SocketOptions options = new SocketOptions();
//             options.AutoConnect = false;
//             options.Reconnection = true;
//             options.ReconnectionAttempts = 3;
//             
//             options.ConnectWith = BestHTTP.SocketIO.Transports.TransportTypes.WebSocket;
//             options.AdditionalQueryParams = new PlatformSupport.Collections.ObjectModel.ObservableDictionary<string, string>();
//             options.AdditionalQueryParams.Add("x-token", DragonU3DSDK.Account.AccountManager.Instance.Token);
//             options.AdditionalQueryParams.Add("x-type", "protobuf");
//             options.AdditionalQueryParams.Add("x-encrypted", "true");
//             options.AdditionalQueryParams.Add("x-accept-gzip", "1");
//          
//             options.AdditionalQueryParams.Add("x-platform", DeviceHelper.GetPlatform().ToString());
//             options.AdditionalQueryParams.Add("x-client-version-name", DeviceHelper.GetAppVersion());
//             options.AdditionalQueryParams.Add("x-client-version-code", DragonNativeBridge.GetVersionCode().ToString());
//
//             var storageCommon = DragonU3DSDK.Storage.StorageManager.Instance.GetStorage<DragonU3DSDK.Storage.StorageCommon>();
//             if (storageCommon != null)
//             {
//                 options.AdditionalQueryParams.Add("x-user-group", storageCommon.AdsPredictUserGroup.ToString());
//             }
//
//             var userAgent = Application.productName + "/" + DeviceHelper.GetPlatform().ToString() + "/" + DeviceHelper.GetAppVersion();
//             options.AdditionalQueryParams.Add("User-Agent", userAgent);
//
//             if (socketManager != null) {
//                 socketManager.Close();
//             }
//
//             socketManager = new SocketManager(new Uri(host + "/socket.io/"), options);
//             socketManager.Open();
//
//           
//             
//             var Socket = socketManager.Socket;
//             Socket.On(SocketIOEventTypes.Connect, (socket, packet, args) => {
//                 DebugUtil.Log("SocketIO Connect");
//                 EventManager.Instance.Trigger<DragonU3DSDK.SDKEvents.SocketIOConnected>().Trigger();
//                 BIManager.Instance.SendCommonGameEvent(DragonU3DSDK.Network.API.Protocol.BiEventCommon.Types.CommonGameEventType.SocketIoConnected);
//             });
//             Socket.On(SocketIOEventTypes.Disconnect, (socket, packet, args) => {
//                 DebugUtil.Log("SocketIO Disconnect");
//                 
//                 EventManager.Instance.Trigger<DragonU3DSDK.SDKEvents.SocketIODisconnected>().Trigger();
//                 BIManager.Instance.SendCommonGameEvent(DragonU3DSDK.Network.API.Protocol.BiEventCommon.Types
//                     .CommonGameEventType.SocketIoDisconnected);
//                 
//                 //  InitWebsocket();
//             });
//
//             Socket.On(SocketIOEventTypes.Error, (socket, packet, args) => {
//                 Debug.LogError($"SocketIO Error:{packet}");
//               
//                 EventManager.Instance.Trigger<DragonU3DSDK.SDKEvents.SocketIOError>().Trigger();
//                 BIManager.Instance.SendCommonGameEvent(DragonU3DSDK.Network.API.Protocol.BiEventCommon.Types
//                     .CommonGameEventType.SocketIoError);
//             });
//
//             Socket.On("/debug", (socket, package, args) =>
//             {
//                 DebugUtil.Log(string.Format("websocket debug: {0}", package.Payload));
//             });
//
//             Socket.On("/error", (socket, package, args) =>
//             {
//                 DebugUtil.LogError(string.Format("websocket error: {0}", package.Payload));
//             });
//
//             Socket.On("/push", (socket, packet, args) =>
//             {
//                 if (socket == socketManager.Socket && args != null && args.Length > 0)
//                 {
//                     string ProtocolName = args[0].ToString();
//                     #if UNITY_EDITOR
//                     DebugUtil.Log("websocket push {0}", ProtocolName);
//                     #endif
//
//                     if (!pushCallbacks.ContainsKey(ProtocolName))
//                     {
//                         DebugUtil.LogError("websocket push {0} no callbacks", ProtocolName);
//                     }
//
//                     byte[] data;
//                     if (packet != null && packet.AttachmentCount > 0)
//                     {
//                         data = packet.Attachments[0];
//                     }
//                     else
//                     {
//                         data = new byte[0];
//                     }
//
//                     byte[] decrypted;
//                     if (data.Length > 0)
//                     {
//                         decrypted = RC4.Decrypt(secret, data);
//                     }
//                     else
//                     {
//                         decrypted = new byte[0];
//                     }
//
//                     // Type responseType = Type.GetType("DragonU3DSDK.Network.API.Protocol." + ProtocolName);
//                     // PropertyInfo prop = responseType.GetProperty("Parser");
//                     // object obj = prop.GetValue(null, null);
//                     // MethodInfo m = prop.PropertyType.GetMethod("ParseFrom", new Type[] { typeof(byte[]) });
//                     // IMessage resp = (IMessage)m.Invoke(obj, new object[] { decrypted });
//                     
//                     Type responseType = Type.GetType("DragonU3DSDK.Network.API.ILProtocol." + ProtocolName);
//                     IMessage resp = (IMessage)System.Activator.CreateInstance(responseType);
//                     resp.MergeFrom(decrypted);
//
//                     //pushCallbacks[ProtocolName].Invoke(resp);
//                      pushCallbackQueue.Enqueue(new Tuple<Action<IMessage>, IMessage, string>(pushCallbacks[ProtocolName], resp, ProtocolName));
//                 }
//             });
//         }
//
//         public void OnPush<T>(Action<T> callback) where T : IMessage
//         {
//             string protoName = typeof(T).Name;
//             if (pushCallbacks.ContainsKey(protoName))
//             {
//                 DebugUtil.Log("push callback duplicated, replacing...");
//             }
//             else
//             {
//                 pushCallbacks.Add(protoName, (IMessage msg) =>  
//                 {
//                     callback.Invoke((T)msg);
//                 });
//             }
//         }
//  
//          IEnumerator SendWebsocketMessage<T1, T2>(T1 req, System.Action<T2> onResponse, System.Action<ErrorCode, string, T2> onError) where T1 : IMessage where T2 : IMessage,new()
//         {
//             var reqName = typeof(T1).Name;
//             bool callbackInvoked = false;
//             Action<ErrorCode, string, T2> onErrorWrapper = (ErrorCode errno, string errmsg, T2 resp) =>
//             {
//                 if (!callbackInvoked)
//                 {
//                     callbackInvoked = true;
//                     errorCallbackQueue.Enqueue(new Tuple<Action<ErrorCode, string, IMessage>, ErrorCode, string, IMessage, string>((ErrorCode _errno, string _errmsg, IMessage _resp) => onError?.Invoke(_errno, _errmsg, (T2)_resp), errno, errmsg, resp, reqName));
//                 }
//             };
//
//             Action<T2> onResponseWrapper = (T2 resp) =>
//             {
//                 if (!callbackInvoked)
//                 {
//                     callbackInvoked = true;
//                     respCallbakQueue.Enqueue(new Tuple<Action<IMessage>, IMessage, string>((IMessage _resp) => onResponse?.Invoke((T2)_resp), resp, reqName));
//                 }
//             };
//
//             // if (!APIConfig.APIEntries.ContainsKey(ReqName))
//             // {
//             //     onErrorWrapper(ErrorCode.ApiNotExistsError, "api mapping dosen't contain the " + ReqName, default(T2));
//             //     yield break;
//             // }
//             // var apiEntry = APIConfig.APIEntries[ReqName];
//
//             //int timeout = ConfigurationController.Instance.APIServerTimeout;
//             //WebSockt临时时间
//             int timeout = 30;
//             // if (apiEntry.timeout > timeout)
//             // {
//             //     timeout = apiEntry.timeout;
//             // }
//
//             bool useGzip = true;
//             byte[] reqData = req.ToByteArray();
//             byte[] zipped = reqData;
//             bool reqCompressed = false;
//             if (useGzip && reqData.Length > 1024)
//             {
//                 zipped = gzip(reqData);
//                 reqCompressed = true;
//             }
//             byte[] encrypted = RC4.Encrypt(secret, zipped);
//
//             socketManager.Socket.Emit(reqName, (socket, packet, args) =>
//             {
//                 try
//                 {
//                     bool resCompressed = false;
//                     if (args.Length > 3)
//                     {
//                         resCompressed = (bool)args[3];
//                     }
//
//                     byte[] resData;
//                     if (packet != null && packet.AttachmentCount > 0)
//                     {
//                         resData = packet.Attachments[0];
//                     }
//                     else  
//                     {
//                         resData = new byte[0];
//                     }
//
//                     byte[] decrypted;
//                     if (resData.Length > 0)
//                     {
//                         decrypted = RC4.Decrypt(secret, resData);
//                     }
//                     else
//                     {
//                         decrypted = new byte[0];
//                     }
//
//                     byte[] unzipped = decrypted;
//                     if (decrypted.Length > 0 && resCompressed)
//                     {
//                         unzipped = gunzip(decrypted);
//                     }
//
//                     T2 resp = new T2();
//                     resp.MergeFrom(unzipped);
//                     
//
//                     // string RespName = 'S' + ReqName.Substring(1);
//                     // Type respType = Type.GetType("DragonU3DSDK.Network.API.Protocol." + RespName);
//                     // PropertyInfo prop = respType.GetProperty("Parser");
//                     // object obj = prop.GetValue(null, null);
//                     // MethodInfo m = prop.PropertyType.GetMethod("ParseFrom", new Type[] { typeof(byte[]) });
//                     // T2 resp = (T2)m.Invoke(obj, new object[] { unzipped });
//
//                     ErrorCode errno = (ErrorCode)int.Parse(args[0].ToString());
//                     string errmsg = args[1].ToString();
//                     if (errno != ErrorCode.Success)
//                     {
//                         
//                         Debug.LogError(packet.ToString());
//
//                         if (errno == ErrorCode.TokenExpireError)
//                         {
//                             DragonU3DSDK.Account.AccountManager.Instance.OnTokenExpire();
//                         }
//
//                         if (errno == ErrorCode.RefreshTokenExpireError)
//                         {
//                             DragonU3DSDK.Account.AccountManager.Instance.OnRefreshTokenExpire();
//                         }
//
//                         if (errno == ErrorCode.FacebookAuthExpireError) {
//                             DragonU3DSDK.Account.FacebookManager.Instance.OnFacebookAuthExpire();
//                         }
//                         onErrorWrapper.Invoke(errno, errmsg, resp);
//                         return;
//                     }
//
//                     callbackInvoked = true;
//                     onResponse?.Invoke(resp);
//                 }
//                 catch (Exception e)
//                 {
//                     onErrorWrapper.Invoke(ErrorCode.UnknownError, e.ToString(), new T2());
//                     Debug.LogException(e);
//                     BIManager.Instance.SendException(e, 0, reqName);
//                 }
//             }, encrypted, reqCompressed);
//
//             yield return new WaitForSeconds(timeout);
//             onErrorWrapper.Invoke(ErrorCode.HttpTimeoutError, "websocket timeout", new T2());
//            
//             BIManager.Instance.SendCommonGameEvent(DragonU3DSDK.Network.API.Protocol.BiEventCommon.Types
//                 .CommonGameEventType.SocketIoError,"ClientTimeOut");
//         }
//         
//          IEnumerator send<T1, T2>(T1 req, System.Action<T2> onResponse, System.Action<ErrorCode, string, T2> onError)  where T1: IMessage where T2: IMessage,new()
//         {
//             var reqName = req.GetType().Name;
//             //var reqName = typeof(T1).Name;
//             bool callbakInvoked = false;
//             Action<ErrorCode, string, T2> onErrorWrapper = (ErrorCode errno, string errmsg, T2 resp) =>
//             {
//                 if (!callbakInvoked)
//                 {
//                     callbakInvoked = true;
//                     errorCallbackQueue.Enqueue(new Tuple<Action<ErrorCode, string, IMessage>, ErrorCode, string, IMessage, string>((ErrorCode _errno, string _errmsg, IMessage _resp) => onError?.Invoke(_errno, _errmsg, (T2)_resp), errno, errmsg, resp, reqName));
//                 }
//             };
//
//             Action<T2> onResponseWrapper = (T2 resp) =>
//             {
//                 if (!callbakInvoked)
//                 {
//                     callbakInvoked = true;
//                     respCallbakQueue.Enqueue(new Tuple<Action<IMessage>, IMessage, string>((IMessage _resp) => onResponse?.Invoke((T2)_resp), resp, reqName));
//                 }
//             };
//
//             if (!HasNetwork)
//             {
//                 onErrorWrapper(ErrorCode.NetworkError, "no network", new T2());
//                 yield break;
//             }
//
//             if (!APIConfig.APIEntries.ContainsKey(reqName))
//             {
//                 onErrorWrapper(ErrorCode.ApiNotExistsError, "api mapping doesn't contain the " + reqName, new T2());
//                 yield break;
//             }
//             var apiEntry = APIConfig.APIEntries[reqName];
//             string uri = apiEntry.uri;
//             string method = apiEntry.method;
//             string scheme = apiEntry.scheme;
//             bool useGzip = apiEntry.gzip;
//
//             int timeout = ConfigurationController.Instance.APIServerTimeout;
//             if (apiEntry.timeout > timeout)
//             {
//                 timeout = apiEntry.timeout;
//             }
//
//             string url = host + uri;
//             HTTPRequest httpRequest = new HTTPRequest(new Uri(url));
//
//             // check auth
//             if (!apiEntry.ignoreAuth)
//             {
//                 switch (DragonU3DSDK.Account.AccountManager.Instance.loginStatus)
//                 {
//                     case DragonU3DSDK.Account.LoginStatus.LOGOUT:
//                         onErrorWrapper(ErrorCode.TokenExpireError, "not login", new T2());
//                         yield break;
//                     case DragonU3DSDK.Account.LoginStatus.LOGIN_LOCKING:
//                     case DragonU3DSDK.Account.LoginStatus.TOKEN_EXPIRED:
//                         var frames = 0;
//                         var stopFrames = Application.targetFrameRate * timeout;
//                         yield return new WaitUntil(() => (frames++ > stopFrames) || DragonU3DSDK.Account.AccountManager.Instance.loginStatus == DragonU3DSDK.Account.LoginStatus.LOGIN);
//                         if (frames >= stopFrames)
//                         {
//                             onErrorWrapper(ErrorCode.HttpTimeoutError, "timeout waiting for login lock", new T2());
//                             yield break;
//                         }
//                         break;
//                 }
//             }
//
//             // set headers
//             httpRequest.SetHeader("x-type", "protobuf");
//             httpRequest.SetHeader("content-type", "application/octet-stream");
//             httpRequest.SetHeader("x-method", method);
//             httpRequest.SetHeader("x-accept-gzip", "1");
//             httpRequest.SetHeader("x-platform", DeviceHelper.GetPlatform().ToString());
//             httpRequest.SetHeader("x-client-version-name", DeviceHelper.GetAppVersion());
//             httpRequest.SetHeader("x-client-version-code", DragonNativeBridge.GetVersionCode().ToString());
//             httpRequest.SetHeader("User-Agent", DeviceHelper.GetUserAgent());
//
//             var storageCommon = DragonU3DSDK.Storage.StorageManager.Instance.GetStorage<DragonU3DSDK.Storage.StorageCommon>();
//             if (storageCommon != null)
//             {
//                 httpRequest.SetHeader("x-user-group", storageCommon.AdsPredictUserGroup.ToString());
//             }
//
//             if (!string.IsNullOrEmpty(DragonU3DSDK.Account.AccountManager.Instance.Token))
//             {
//                 httpRequest.SetHeader("x-token", DragonU3DSDK.Account.AccountManager.Instance.Token);
//             }
//
//             // data: encode => compress => encrypt 
//             byte[] data = req.ToByteArray();
//             byte[] zipped = data;
//             if (useGzip && data.Length > 1024)
//             {
//                 httpRequest.SetHeader("x-gzip", "1");
//                 zipped = gzip(data);
//             }
//             byte[] encrypted = RC4.Encrypt(secret, zipped);
//             httpRequest.MethodType = HTTPMethods.Post;
//             httpRequest.Timeout = TimeSpan.FromSeconds(timeout);
//             if (encrypted.Length > 0)
//             {
//                 httpRequest.RawData = encrypted;
//             }
//
//             httpRequest.Send();
//             
//             yield return IEnumeratorTool.instance.StartCoroutine(httpRequest);
//
//             while (httpRequest.State != HTTPRequestStates.Finished)
//             {
//                 switch (httpRequest.State)
//                 {
//                     case HTTPRequestStates.Initial:
//                     case HTTPRequestStates.Queued:
//                     case HTTPRequestStates.Processing:
//                         BIManager.Instance.SendException(new Exception($"http state = {httpRequest.State.ToString()}"));
//                         yield return httpRequest;
//                         break;
//                     case HTTPRequestStates.Error:
//                         if (httpRequest.Exception != null)
//                         {
//                             onErrorWrapper(ErrorCode.UnknownError, httpRequest.Exception.Message, new T2());
//                         } else {
//                             onErrorWrapper(ErrorCode.UnknownError, "http request error without exception", new T2());
//                         }
//                         yield break;
//                     case HTTPRequestStates.ConnectionTimedOut:
//                     case HTTPRequestStates.TimedOut:
//                         onErrorWrapper(ErrorCode.HttpTimeoutError, "http request timeout", new T2());
//                         yield break;
//                     case HTTPRequestStates.Aborted:
//                         onErrorWrapper(ErrorCode.HttpError, "http request aborted", new T2());
//                         yield break;
//                 }
//             }
//
//             if (httpRequest.Response == null)
//             {
//                 onErrorWrapper(ErrorCode.HttpError, "empty response", new T2());
//                 yield break;
//             }
//
//             if (!httpRequest.Response.IsSuccess)
//             {
//                 onErrorWrapper(ErrorCode.HttpError, httpRequest.Response.StatusCode.ToString(), new T2());
//                 yield break;
//             }
//
//             try
//             {
//                 byte[] decrypted;
//                 if (httpRequest.Response.Data != null && httpRequest.Response.Data.Length > 0)
//                 {
//                     decrypted = RC4.Decrypt(secret, httpRequest.Response.Data);
//                 }
//                 else
//                 {
//                     decrypted = new byte[0];
//                 }
//
//                 byte[] unzipped = decrypted;
//                 if (decrypted.Length > 0 && httpRequest.Response.HasHeader("x-gzip"))
//                 {
//                     unzipped = gunzip(decrypted);
//                 }
//
//                 T2 resp = new T2();
//                 resp.MergeFrom(unzipped);
//                 
//
//                 // string RespName = 'S' + reqName.Substring(1);
//                 // Type respType = Type.GetType("DragonU3DSDK.Network.API.Protocol." + RespName);
//                 // PropertyInfo prop = respType.GetProperty("Parser");
//                 // object obj = prop.GetValue(null, null);
//                 // MethodInfo m = prop.PropertyType.GetMethod("ParseFrom", new Type[] { typeof(byte[]) });
//                 // T2 resp = (T2)m.Invoke(obj, new object[] { unzipped });
//
//                 ErrorCode errorCode = httpRequest.Response.Headers.TryGetValue("x-errno", out var es) && es.Count > 0 && int.TryParse(es[0], out var errID) ? (ErrorCode)errID : ErrorCode.UnknownError;
//                 string errorMessage = httpRequest.Response.Headers.TryGetValue("x-errmsg", out var ms) && ms.Count > 0 ? ms[0] : "Unknow";
//                 if (errorCode > 0)
//                 {
// #if UNITY_EDITOR
//                    DebugUtil.LogError("http {0} errno: {1} errmsg: {2}", reqName, errorCode.ToString(), errorMessage);
// #endif
//                     if (errorCode == ErrorCode.TokenExpireError)
//                     {
//                         DragonU3DSDK.Account.AccountManager.Instance.OnTokenExpire();
//                     }
//
//                     if (errorCode == ErrorCode.RefreshTokenExpireError)
//                     {
//                         DragonU3DSDK.Account.AccountManager.Instance.OnRefreshTokenExpire();
//                     }
//
//                     onErrorWrapper(errorCode, errorMessage, resp);
//                     yield break;
//                 }
//
//                 onResponseWrapper(resp);
//             }
//             catch (Exception e)
//             {
//                 var ee = e;
//                 while (ee.InnerException != null) ee = ee.InnerException;   // 仅记录真正产生异常的信息
//                 onErrorWrapper(ErrorCode.UnknownError, ee.ToString(), new T2());
//
//                 if (reqName != "CSendEvents") BIManager.Instance.SendException(ee, 0, reqName);
//             }
//         }
//     }
// }