using System;
using System.Collections;
using System.Collections.Generic;
using DragonU3DSDK.Network.API.ILProtocol;
using Google.ilruntime.Protobuf;
using GameModule;
using UnityEngine;

public class ProtocolUtils
{
    public static T GetAnyStruct<T>(DragonU3DSDK.Network.API.ILProtocol.AnyStruct anyStruct) where T : IMessage
    {
        T t = Activator.CreateInstance<T>();
        t.MergeFrom(anyStruct.Data);
        return t;
    }

    public static T Get<T>(byte[] bytes) where T : IMessage
    {
        Debug.Assert(bytes != null, "bytes should not be null");
        T t = Activator.CreateInstance<T>();
        t.MergeFrom(new ByteString(bytes));
        return t;
    }

    public static IMessage GetAnyStruct(DragonU3DSDK.Network.API.ILProtocol.AnyStruct anyStruct)
    {
        Type t = Type.GetType($"DragonU3DSDK.Network.API.ILProtocol.{anyStruct.Type}");
        IMessage instance = Activator.CreateInstance(t) as IMessage;
        instance.MergeFrom(anyStruct.Data);
        return instance;
    }

    public static AnyStruct ToAnyStruct(IMessage message)
    {
        AnyStruct anyStruct = new AnyStruct();
        anyStruct.Type = message.GetType().Name;
        anyStruct.Data = message.ToByteString();

        return anyStruct;
    }

    public static T CloneFrom<T>(T message) where T : IMessage
    {
        var byteArray = message.ToByteArray();
        var cloneInstance = Activator.CreateInstance<T>();
        cloneInstance.MergeFrom(byteArray);
        return cloneInstance;
    }


    public static string GetAnyStructJson(byte[] bpBytes, string type)
    {
        Type t = Type.GetType($"DragonU3DSDK.Network.API.ILProtocol.{type}");
        if (t != null)
        {
            IMessage instance = Activator.CreateInstance(t) as IMessage;
            instance.MergeFrom(bpBytes);
            string json = LitJson.JsonMapper.ToJsonField(instance);
            return json;
        }
        else
        {
            XDebug.Log($"DragonU3DSDK.Network.API.ILProtocol.{type} No Exist");
            return "";
        }
    }
}
