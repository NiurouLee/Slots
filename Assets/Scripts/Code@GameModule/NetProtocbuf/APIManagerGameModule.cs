using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using BestHTTP;
using BestHTTP.Decompression.Zlib;
using BestHTTP.SocketIO;
using DragonU3DSDK;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Network.BI;
using Google.ilruntime.Protobuf;
using Tool;
using UnityEngine;

namespace GameModule
{
    public class APIFixHotAsyncHandler<T> where T : IMessage
    {
        public T Response;
    
        public ErrorCode ErrorCode;
    
        public string ErrorInfo;
    }
    
    public class APIRequest
    {
        public Type requestType;
        public Type responseType;
        public IMessage requestData;
        public IMessage response;
        public ErrorCode errorCode;
        public string errorInfo;
        public Action<APIRequest> responseCallback;
    }
 
    public class APIManagerGameModule: IUpdateable
    {
        public bool Inited
        {
            get;
            private set;
        }

        string host = null;
        byte[] secret = null;
        float heartBeatTimer = 0;
        long serverTimeOffset = 0;
        ulong lastSyncServerTime = 0;
        ulong lastSyncLocalTime = 0;
        bool websocektInitCalled = false;
        float websocketAutoReconnectTimer = 0;
        private bool _closeByClient = false;

        private SocketManager.States lastSocketState = SocketManager.States.Closed;
        
        private float lastUpdateTime = 0;

        // socket.io
        private SocketManager socketManager;
        Dictionary<string, Action<IMessage>> pushCallbacks = new Dictionary<string, Action<IMessage>>();
        Queue<Tuple<Action<IMessage>, IMessage, string>> pushCallbackQueue = new Queue<Tuple<Action<IMessage>, IMessage, string>>();

        private Queue<APIRequest> _requestQueues;
        void Initialize()
        {
            secret = System.Text.Encoding.UTF8.GetBytes(ConfigurationController.Instance.APIServerSecret);
            host = ConfigurationController.Instance.APIServerURL;
            DebugUtil.Log("api server host = {0}", host);
           
            Inited = true;

            _requestQueues = new Queue<APIRequest>();
        }

        private static APIManagerGameModule _instance;
        public static APIManagerGameModule Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new APIManagerGameModule();
                }

                return _instance;
            }
        }


        protected APIManagerGameModule()
        {
            Awake();
           
            if (!Inited)
            {
                Initialize();
            }
            
            StartUpdate();
        }


        ~APIManagerGameModule()
        {
            StopUpdate();
        }


        private void Awake()
        {
            
        }
        
        public void StopUpdate()
        {
            UpdateScheduler.UnhookUpdate(this);
        }

        public void Clear()
        {
            StopUpdate();
            CloseWebsocket();
        }
 
        void StartUpdate()
        {
            UpdateScheduler.HookUpdate(this);
        }

        public  void Update()
        {
            if (pushCallbackQueue.Count > 0) {
                var tuple = pushCallbackQueue.Dequeue();
                try
                {
                    tuple.Item1.Invoke(tuple.Item2);
                }
                catch (Exception e)
                {
                    BIManager.Instance.SendException(e, 0, tuple.Item3);
                    
                }
            }
 
            if (socketManager != null && websocektInitCalled == true)
            {
                var deltaTime = Time.realtimeSinceStartup - lastUpdateTime;
                lastUpdateTime = Time.realtimeSinceStartup;
             
                //如果长时间没走Update,直接断掉连接
                if (deltaTime > 3600)
                {
                    Clear();
                    EventBus.Dispatch(new EventNetworkClosed());
                    return;
                }
                
                if (socketManager.State == SocketManager.States.Closed && socketManager.State != lastSocketState)
                {
                    XDebug.LogError($"EventNetworkClosed:socketManager.State: {socketManager.State}");
                    EventBus.Dispatch(new EventNetworkClosed());
                }
                
                lastSocketState = socketManager.State;
            }
        }
   
        byte[] gzip(byte[] fi)
        {
            using (MemoryStream outFile = new MemoryStream())
            {
                using (MemoryStream inFile = new MemoryStream(fi))
                using (GZipStream compress = new GZipStream(outFile, CompressionMode.Compress))
                {
                    inFile.CopyTo(compress);
                }
                return outFile.ToArray();
            }
        }

        byte[] gunzip(byte[] fi)
        {
            using (MemoryStream outFile = new MemoryStream())
            {
                using (MemoryStream inFile = new MemoryStream(fi))
                using (GZipStream compress = new GZipStream(inFile, CompressionMode.Decompress))
                {
                    compress.CopyTo(outFile);
                }
                return outFile.ToArray();
            }
        }

        protected void EnqueueApiRequest(APIRequest apiRequest)
        {
            _requestQueues.Enqueue(apiRequest);

            if (_requestQueues.Count == 1)
            {
                Send(apiRequest);
            }
        }

        protected void Send(APIRequest apiRequest)
        {
            if (WebsocketConnected && apiRequest.requestType.Name != "CLogin")
            {
                SendWebsocketMessage(apiRequest, (response) =>
                {
                    response.responseCallback.Invoke(response);
                    ProcessNextApiRequest();
                });
            }
            else
            {
                IEnumeratorTool.instance.StartCoroutine(SendHttpMessage(apiRequest, (response) =>
                {
                    response.responseCallback.Invoke(response);
                    ProcessNextApiRequest();
                }));
            }
        }

        protected void ProcessNextApiRequest()
        {
            if (_requestQueues.Count > 0)
            {
                _requestQueues.Dequeue();
               
                if (_requestQueues.Count > 0)
                {
                    APIRequest apiRequest = _requestQueues.Peek();
                    Send(apiRequest);
                }
            }
        }
        
        public async Task<APIFixHotAsyncHandler<T2>> SendAsync<T1, T2>(T1 imessage) where T1 : IMessage where T2 : class, IMessage,new()
        {
            if (!Inited) Initialize();
             
            string name1 = typeof(T1).Name;
            string name2 = typeof(T2).Name;
            
            if (name1 == "IMessage")
            {
                DebugUtil.LogWarning("DragonU3DSDK.Network.API.APIManager.Send is deprecated, please use DragonU3DSDK.Network.API.APIManager.Send<T1,T2> instead.");
            }
            else if (!(name1[0] == 'C' && name2[0] == 'S' && name1.Substring(1) == name2.Substring(1)))
            {
                var apiAsyncHandler = new APIFixHotAsyncHandler<T2>();
                apiAsyncHandler.Response = default(T2);
                apiAsyncHandler.ErrorCode = ErrorCode.ParameterError;
                apiAsyncHandler.ErrorInfo = "request and response type not match";
                DebugUtil.LogError("request type {0} response type {1} not match", name1, name2);
                //onError?.Invoke(ErrorCode.ParameterError, "request and response type not match", default(T2));
                return apiAsyncHandler;
            }

            XDebug.Log($"<color=yellow>=====Websocket [{typeof(T2).Name}] Send </color>");
            
            var taskCompletionSource = new TaskCompletionSource<APIFixHotAsyncHandler<T2>>();
 
            var handler = new APIFixHotAsyncHandler<T2>();

            APIRequest apiRequest = new APIRequest();

            apiRequest.requestData = imessage;
            apiRequest.requestType = typeof(T1);
            apiRequest.responseType = typeof(T2);

            apiRequest.responseCallback = (response) =>
            {
                if (response.errorCode == ErrorCode.Success)
                {
                    handler.ErrorCode = ErrorCode.Success;
                    handler.Response = (T2) response.response;
                    XDebug.Log($"<color=yellow>=====Websocket taskCompletionSource  onResponse</color>");
                    taskCompletionSource.SetResult(handler);
                }
                else
                {
                    handler.ErrorCode = response.errorCode;
                    handler.ErrorInfo = response.errorInfo;
                    handler.Response = new T2();
                    XDebug.Log(
                        $"<color=yellow>=====Websocket taskCompletionSource  onError errorCode:{response.errorCode} errorInfo:{response.errorInfo}</color>");
                    taskCompletionSource.SetResult(handler);
                }
            };
                
            EnqueueApiRequest(apiRequest);
            
            await taskCompletionSource.Task;
            
#if (DEBUG || DEVELOPMENT_BUILD) && SHOW_RESPONSE_JSON
            
            if (isShowResponseJSON)
            {
                if (handler.Response != null)
                {
                    Debug.Log($"<color=yellow>=======MessageInfo[{apiRequest.requestType.Name}]: Response: {LitJson.JsonMapper.ToJsonField(handler.Response)}</color>");
                }
                else
                {
                    Debug.LogError($"=======MessageInfo[{apiRequest.requestType.Name}]: ErrorCode:{handler.ErrorCode} ErrorInfo:{handler.ErrorInfo}");
                }
            }
#endif
            return handler;
        }

        public bool isShowResponseJSON = true;
        
        public bool WebsocketConnected
        {
            get
            {
                return socketManager != null && socketManager.Socket != null && socketManager.Socket.IsOpen;
            }
        }
        public void CloseWebsocket()
        {
            if(socketManager != null)
                socketManager.Close();
        }
        
        
        public void InitWebsocket()
        {
            websocektInitCalled = true;
        
            if (!(Inited && DragonU3DSDK.Account.AccountManager.Instance.HasLogin && ConfigurationController.Instance.SocketIOEnabled))
            {
                return;
            }

            if (WebsocketConnected)
            {
                return;
            }
            
            lastUpdateTime = Time.realtimeSinceStartup;

            SocketOptions options = new SocketOptions();
            options.AutoConnect = false;
            options.Reconnection = true;
            options.ReconnectionAttempts = 3;
            
            options.ConnectWith = BestHTTP.SocketIO.Transports.TransportTypes.WebSocket;
            options.AdditionalQueryParams = new PlatformSupport.Collections.ObjectModel.ObservableDictionary<string, string>();
            options.AdditionalQueryParams.Add("x-token", DragonU3DSDK.Account.AccountManager.Instance.Token);
            options.AdditionalQueryParams.Add("x-type", "protobuf");
            options.AdditionalQueryParams.Add("x-encrypted", "true");
            options.AdditionalQueryParams.Add("x-accept-gzip", "1");
         
            options.AdditionalQueryParams.Add("x-platform", DeviceHelper.GetPlatform().ToString());
            options.AdditionalQueryParams.Add("x-client-version-name", DeviceHelper.GetAppVersion());
            options.AdditionalQueryParams.Add("x-client-version-code", DragonNativeBridge.GetVersionCode().ToString());

            var storageCommon = DragonU3DSDK.Storage.StorageManager.Instance.GetStorage<DragonU3DSDK.Storage.StorageCommon>();
            if (storageCommon != null)
            {
                options.AdditionalQueryParams.Add("x-user-group", storageCommon.AdsPredictUserGroup.ToString());
            }

            var userAgent = Application.productName + "/" + DeviceHelper.GetPlatform().ToString() + "/" + DeviceHelper.GetAppVersion();
            options.AdditionalQueryParams.Add("User-Agent", userAgent);

            if (socketManager != null) {
                socketManager.Close();
            }

            XDebug.Log($"======InitWebsocket url:{host}/socket.io/");
            socketManager = new SocketManager(new Uri(host + "/socket.io/"), options);
            socketManager.Open();

          
            
            var Socket = socketManager.Socket;
            Socket.On(SocketIOEventTypes.Connect, (socket, packet, args) => {
                DebugUtil.Log("SocketIO Connect");
                EventManager.Instance.Trigger<DragonU3DSDK.SDKEvents.SocketIOConnected>().Trigger();
                BIManager.Instance.SendCommonGameEvent(DragonU3DSDK.Network.API.Protocol.BiEventCommon.Types.CommonGameEventType.SocketIoConnected);
            });
            Socket.On(SocketIOEventTypes.Disconnect, (socket, packet, args) => {
                DebugUtil.Log("SocketIO Disconnect");
                
                EventManager.Instance.Trigger<DragonU3DSDK.SDKEvents.SocketIODisconnected>().Trigger();
                BIManager.Instance.SendCommonGameEvent(DragonU3DSDK.Network.API.Protocol.BiEventCommon.Types
                    .CommonGameEventType.SocketIoDisconnected);
                
                //  InitWebsocket();
            });

            Socket.On(SocketIOEventTypes.Error, (socket, packet, args) => {
                Debug.LogError($"SocketIO Error:{packet}");
              
                if (args != null && args.Length > 0)
                {
                    for (var i = 0; i < args.Length; i++)
                    {
                        var error = args[i] as BestHTTP.SocketIO.Error;
                        if (error != null)
                        {
                            XDebug.LogOnExceptionHandler($"SocketIO Error: {error.Code}/{error.Message}");
                        }
                    }
                }
              
                EventManager.Instance.Trigger<DragonU3DSDK.SDKEvents.SocketIOError>().Trigger();
                BIManager.Instance.SendCommonGameEvent(DragonU3DSDK.Network.API.Protocol.BiEventCommon.Types
                    .CommonGameEventType.SocketIoError);
            });

            Socket.On("/debug", (socket, package, args) =>
            {
                DebugUtil.Log(string.Format("websocket debug: {0}", package.Payload));
            });

            Socket.On("/error", (socket, package, args) =>
            {
                DebugUtil.LogError(string.Format("websocket error: {0}", package.Payload));
            });

            Socket.On("/push", (socket, packet, args) =>
            {
                if (socket == socketManager.Socket && args != null && args.Length > 0)
                {
                    string ProtocolName = args[0].ToString();
                    #if UNITY_EDITOR
                    DebugUtil.Log("websocket push {0}", ProtocolName);
                    #endif

                    if (!pushCallbacks.ContainsKey(ProtocolName))
                    {
                        DebugUtil.LogError("websocket push {0} no callbacks", ProtocolName);
                    }

                    byte[] data;
                    if (packet != null && packet.AttachmentCount > 0)
                    {
                        data = packet.Attachments[0];
                    }
                    else
                    {
                        data = new byte[0];
                    }

                    byte[] decrypted;
                    if (data.Length > 0)
                    {
                        decrypted = RC4.Decrypt(secret, data);
                    }
                    else
                    {
                        decrypted = new byte[0];
                    }

                    // Type responseType = Type.GetType("DragonU3DSDK.Network.API.Protocol." + ProtocolName);
                    // PropertyInfo prop = responseType.GetProperty("Parser");
                    // object obj = prop.GetValue(null, null);
                    // MethodInfo m = prop.PropertyType.GetMethod("ParseFrom", new Type[] { typeof(byte[]) });
                    // IMessage resp = (IMessage)m.Invoke(obj, new object[] { decrypted });
                    
                    Type responseType = Type.GetType("DragonU3DSDK.Network.API.ILProtocol." + ProtocolName);
                    IMessage resp = (IMessage)System.Activator.CreateInstance(responseType);
                    resp.MergeFrom(decrypted);

                    //pushCallbacks[ProtocolName].Invoke(resp);
                     pushCallbackQueue.Enqueue(new Tuple<Action<IMessage>, IMessage, string>(pushCallbacks[ProtocolName], resp, ProtocolName));
                }
            });
        }

        public void OnPush<T>(Action<T> callback) where T : IMessage
        {
            string protoName = typeof(T).Name;
           
            if (pushCallbacks.ContainsKey(protoName))
            {
                DebugUtil.Log("push callback duplicated, replacing...");
            }
            else
            {
                pushCallbacks.Add(protoName, (IMessage msg) =>  
                {
                    callback.Invoke((T)msg);
                });
            }
        }
        
        private void SendWebsocketMessage(APIRequest apiRequest, Action<APIRequest> responseCallback)
        {
            var reqName = apiRequest.requestType.Name;
           
            int timeout = 30;
            
            bool useGzip = true;
            byte[] reqData = apiRequest.requestData.ToByteArray();
            byte[] zipped = reqData;
            bool reqCompressed = false;
          
            if (useGzip && reqData.Length > 1024)
            {
                zipped = gzip(reqData);
                reqCompressed = true;
            }
            byte[] encrypted = RC4.Encrypt(secret, zipped);
 
            var timeOutCallback = new CancelableCallback(() =>
            {
                if (responseCallback != null)
                {
                    apiRequest.errorInfo = "websocket timeout";
                    apiRequest.errorCode = ErrorCode.HttpTimeoutError;
                    responseCallback?.Invoke(apiRequest);
                    responseCallback = null;

                    BIManager.Instance.SendCommonGameEvent(DragonU3DSDK.Network.API.Protocol.BiEventCommon.Types
                        .CommonGameEventType.SocketIoError, "ClientTimeOut");
                }
            });

            socketManager.Socket.Emit(reqName, (socket, packet, args) =>
            {
                try
                {
                    bool resCompressed = false;
                    if (args.Length > 3)
                    {
                        resCompressed = (bool)args[3];
                    }

                    byte[] resData;
                    if (packet != null && packet.AttachmentCount > 0)
                    {
                        resData = packet.Attachments[0];
                    }
                    else  
                    {
                        resData = new byte[0];
                    }

                    byte[] decrypted;
                    if (resData.Length > 0)
                    {
                        decrypted = RC4.Decrypt(secret, resData);
                    }
                    else
                    {
                        decrypted = new byte[0];
                    }

                    byte[] unzipped = decrypted;
                    if (decrypted.Length > 0 && resCompressed)
                    {
                        unzipped = gunzip(decrypted);
                    }
     
                    //先检查错误是否是错误
                    ErrorCode errno = (ErrorCode)int.Parse(args[0].ToString());
                    string errmsg = args[1].ToString();
                    
                    if (errno != ErrorCode.Success)
                    {
                        Debug.LogError(packet.ToString());

                        switch (errno)
                        {
                            case ErrorCode.TokenExpireError:
                                DragonU3DSDK.Account.AccountManager.Instance.OnTokenExpire();
                                break;
                            case ErrorCode.RefreshTokenExpireError:
                                DragonU3DSDK.Account.AccountManager.Instance.OnRefreshTokenExpire();
                                break;
                            case ErrorCode.FacebookAuthExpireError:
                                DragonU3DSDK.Account.FacebookManager.Instance.OnFacebookAuthExpire();
                                break;
                        }

                        apiRequest.errorInfo = errmsg;
                        apiRequest.errorCode = errno;;
                        responseCallback?.Invoke(apiRequest);
                        responseCallback = null;
                        timeOutCallback.CancelCallback();
                        return;
                    }
                    
                    apiRequest.response = (IMessage) Activator.CreateInstance(apiRequest.responseType);
                    apiRequest.response.MergeFrom(unzipped);
                    apiRequest.errorCode = ErrorCode.Success;
                    responseCallback?.Invoke(apiRequest);
                    responseCallback = null;
                    timeOutCallback.CancelCallback();
                    
                }
                catch (Exception e)
                {
                    apiRequest.errorInfo = e.ToString();
                    apiRequest.errorCode = ErrorCode.UnknownError;
                    responseCallback?.Invoke(apiRequest);
                    responseCallback = null;
                    timeOutCallback.CancelCallback();
                    Debug.LogException(e);
                    BIManager.Instance.SendException(e, 0, reqName);
                }
            }, encrypted, reqCompressed);
            
            XUtility.WaitSeconds(timeout, timeOutCallback);
        }
        
        protected IEnumerator SendHttpMessage(APIRequest apiRequest, Action<APIRequest> responseCallback)
        {
            var reqName = apiRequest.requestType.Name;
           
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                apiRequest.errorInfo = "no network";
                apiRequest.errorCode = ErrorCode.NetworkError;
                responseCallback?.Invoke(apiRequest);
                yield break;
            }

            if (!APIConfig.APIEntries.ContainsKey(reqName))
            {
                apiRequest.errorInfo =  "api mapping doesn't contain the " + reqName;
                apiRequest.errorCode = ErrorCode.ApiNotExistsError;
                responseCallback?.Invoke(apiRequest);
                yield break;
            }
            var apiEntry = APIConfig.APIEntries[reqName];
            string uri = apiEntry.uri;
            string method = apiEntry.method;
            string scheme = apiEntry.scheme;
            bool useGzip = apiEntry.gzip;

            int timeout = ConfigurationController.Instance.APIServerTimeout;
            if (apiEntry.timeout > timeout)
            {
                timeout = apiEntry.timeout;
            }

            string url = host + uri;
            HTTPRequest httpRequest = new HTTPRequest(new Uri(url));

            // check auth
            if (!apiEntry.ignoreAuth)
            {
                switch (DragonU3DSDK.Account.AccountManager.Instance.loginStatus)
                {
                    case DragonU3DSDK.Account.LoginStatus.LOGOUT:
                        apiRequest.errorInfo = "not login";
                        apiRequest.errorCode = ErrorCode.TokenExpireError;
                        responseCallback?.Invoke(apiRequest);
                        responseCallback = null;
                        yield break;;
                    case DragonU3DSDK.Account.LoginStatus.LOGIN_LOCKING:
                    case DragonU3DSDK.Account.LoginStatus.TOKEN_EXPIRED:
                        var frames = 0;
                        var stopFrames = Application.targetFrameRate * timeout;
                        yield return new WaitUntil(() => (frames++ > stopFrames) || DragonU3DSDK.Account.AccountManager.Instance.loginStatus == DragonU3DSDK.Account.LoginStatus.LOGIN);
                        if (frames >= stopFrames)
                        {
                            apiRequest.errorInfo = "timeout waiting for login lock";
                            apiRequest.errorCode = ErrorCode.HttpTimeoutError;
                            responseCallback?.Invoke(apiRequest);
                            responseCallback = null;
                            yield break;
                        }
                        break;
                }
            }

            // set headers
            httpRequest.SetHeader("x-type", "protobuf");
            httpRequest.SetHeader("content-type", "application/octet-stream");
            httpRequest.SetHeader("x-method", method);
            httpRequest.SetHeader("x-accept-gzip", "1");
            httpRequest.SetHeader("x-platform", DeviceHelper.GetPlatform().ToString());
            httpRequest.SetHeader("x-client-version-name", DeviceHelper.GetAppVersion());
            httpRequest.SetHeader("x-client-version-code", DragonNativeBridge.GetVersionCode().ToString());
            httpRequest.SetHeader("User-Agent", DeviceHelper.GetUserAgent());

            var storageCommon = DragonU3DSDK.Storage.StorageManager.Instance.GetStorage<DragonU3DSDK.Storage.StorageCommon>();
            if (storageCommon != null)
            {
                httpRequest.SetHeader("x-user-group", storageCommon.AdsPredictUserGroup.ToString());
            }

            if (!string.IsNullOrEmpty(DragonU3DSDK.Account.AccountManager.Instance.Token))
            {
                httpRequest.SetHeader("x-token", DragonU3DSDK.Account.AccountManager.Instance.Token);
            }

            // data: encode => compress => encrypt 
            byte[] data = apiRequest.requestData.ToByteArray();
            byte[] zipped = data;
            if (useGzip && data.Length > 1024)
            {
                httpRequest.SetHeader("x-gzip", "1");
                zipped = gzip(data);
            }
            byte[] encrypted = RC4.Encrypt(secret, zipped);
            httpRequest.MethodType = HTTPMethods.Post;
            httpRequest.Timeout = TimeSpan.FromSeconds(timeout);
            if (encrypted.Length > 0)
            {
                httpRequest.RawData = encrypted;
            }

            httpRequest.Send();
            
            yield return IEnumeratorTool.instance.StartCoroutine(httpRequest);

            while (httpRequest.State != HTTPRequestStates.Finished)
            {
                switch (httpRequest.State)
                {
                    case HTTPRequestStates.Initial:
                    case HTTPRequestStates.Queued:
                    case HTTPRequestStates.Processing:
                        BIManager.Instance.SendException(new Exception($"http state = {httpRequest.State.ToString()}"));
                        yield return httpRequest;
                        break;
                    case HTTPRequestStates.Error:
                        if (httpRequest.Exception != null)
                        {
                            apiRequest.errorInfo = httpRequest.Exception.Message;
                            apiRequest.errorCode = ErrorCode.UnknownError;
                            responseCallback?.Invoke(apiRequest);
                        } else {
                            apiRequest.errorInfo = "http request error without exception";
                            apiRequest.errorCode = ErrorCode.UnknownError;
                            responseCallback?.Invoke(apiRequest);
                        }
                        yield break;
                    case HTTPRequestStates.ConnectionTimedOut:
                    case HTTPRequestStates.TimedOut:
                        apiRequest.errorInfo = "http request timeout";
                        apiRequest.errorCode = ErrorCode.HttpTimeoutError;
                        responseCallback?.Invoke(apiRequest);
                        yield break;
                    case HTTPRequestStates.Aborted:
                        apiRequest.errorInfo = "http request aborted";
                        apiRequest.errorCode = ErrorCode.HttpError;
                        responseCallback?.Invoke(apiRequest);
                        yield break;
                }
            }

            if (httpRequest.Response == null)
            {
                apiRequest.errorInfo = "empty response";
                apiRequest.errorCode = ErrorCode.HttpError;
                responseCallback?.Invoke(apiRequest);
                yield break;
            }

            if (!httpRequest.Response.IsSuccess)
            {
                apiRequest.errorInfo = httpRequest.Response.StatusCode.ToString();
                apiRequest.errorCode = ErrorCode.HttpError;
                responseCallback?.Invoke(apiRequest);
                yield break;
            }

            try
            {
                byte[] decrypted;
                if (httpRequest.Response.Data != null && httpRequest.Response.Data.Length > 0)
                {
                    decrypted = RC4.Decrypt(secret, httpRequest.Response.Data);
                }
                else
                {
                    decrypted = new byte[0];
                }

                byte[] unzipped = decrypted;
                if (decrypted.Length > 0 && httpRequest.Response.HasHeader("x-gzip"))
                {
                    unzipped = gunzip(decrypted);
                }
                
                ErrorCode errorCode = httpRequest.Response.Headers.TryGetValue("x-errno", out var es) && es.Count > 0 && int.TryParse(es[0], out var errID) ? (ErrorCode)errID : ErrorCode.UnknownError;
                string errorMessage = httpRequest.Response.Headers.TryGetValue("x-errmsg", out var ms) && ms.Count > 0 ? ms[0] : "Unknow";
            
                if (errorCode > 0)
                {
#if UNITY_EDITOR
                   DebugUtil.LogError("http {0} errno: {1} errmsg: {2}", reqName, errorCode.ToString(), errorMessage);
#endif
                    if (errorCode == ErrorCode.TokenExpireError)
                    {
                        DragonU3DSDK.Account.AccountManager.Instance.OnTokenExpire();
                    }

                    if (errorCode == ErrorCode.RefreshTokenExpireError)
                    {
                        DragonU3DSDK.Account.AccountManager.Instance.OnRefreshTokenExpire();
                    }

                    apiRequest.errorInfo = errorMessage;
                    apiRequest.errorCode = errorCode;
                    responseCallback?.Invoke(apiRequest);
                    responseCallback = null;
                }
                else
                {
                    apiRequest.response = (IMessage) Activator.CreateInstance(apiRequest.responseType);
                    apiRequest.response.MergeFrom(unzipped);
                    responseCallback?.Invoke(apiRequest);
                    responseCallback = null;
                }
            }
            catch (Exception e)
            {
                var ee = e;
                while (ee.InnerException != null) ee = ee.InnerException; // 仅记录真正产生异常的信息
                apiRequest.errorInfo = ee.ToString();
                apiRequest.errorCode = ErrorCode.UnknownError;
                responseCallback?.Invoke(apiRequest);
                if (reqName != "CSendEvents")
                    BIManager.Instance.SendException(ee, 0, reqName);
            }
        }
  
      
    }
}