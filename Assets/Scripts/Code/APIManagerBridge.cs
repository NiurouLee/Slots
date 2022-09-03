using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Network.API.Protocol;
using Google.Protobuf;

public class APIManagerBridge
{
    private class RequestInfo
    {
        public IMessage iMessage;
        public Action<IMessage> onResponse;
        public Action<ErrorCode, string, IMessage> onError;
    }

    private readonly static Dictionary<IMessage, RequestInfo> _requestInfoMap = new Dictionary<IMessage, RequestInfo>();


    private static RequestInfo GetRequestInfo(IMessage iMessage)
    {
        RequestInfo result = null;
        _requestInfoMap.TryGetValue(iMessage, out result);
        return result;
    }

    public static void Send(IMessage iMessage, Action<IMessage> onResponse, Action<ErrorCode, string, IMessage> onError)
    {
        var requestInfo = GetRequestInfo(iMessage);
        if (requestInfo != null)
        {
            if (requestInfo.onResponse != null) { requestInfo.onResponse += onResponse; }
            else { requestInfo.onResponse = onResponse; }
            if (requestInfo.onError != null) { requestInfo.onError += onError; }
            else { requestInfo.onError = onError; }
        }
        else
        {
            requestInfo = new RequestInfo()
            {
                onResponse = onResponse,
                onError = onError
            };
            _requestInfoMap.Add(iMessage, requestInfo);
        }

        APIManager.Instance.Send<IMessage, IMessage>(iMessage,
            (response) =>
            {
                var info = GetRequestInfo(iMessage);
                if (info != null)
                {
                    if (info.onResponse != null)
                    {
                        info.onResponse.Invoke(response);
                    }
                    _requestInfoMap.Remove(iMessage);
                }

            },

            (errorCode, message, response) =>
            {
                var info = GetRequestInfo(iMessage);
                if (info != null)
                {
                    if (info.onError != null)
                    {
                        info.onError.Invoke(errorCode, message, response);
                    }
                    _requestInfoMap.Remove(iMessage);
                }
            });
    }

    public static void ClearBridgeRequest()
    {
        _requestInfoMap.Clear();
    }
}
