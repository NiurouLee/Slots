// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2022/05/11/20:53
// Ver : 1.0.0
// Description : InPackageBIManager.cs
// ChangeLog :
// **********************************************

using System.Collections.Generic;
using DragonU3DSDK.Network.BI;
using Google.Protobuf;
using DragonU3DSDK.Network.API.Protocol;


public class InPackageBIManager
{
    public InPackageBIManager()
    {
    }

    protected static void SendBiEvent(IMessage specificMsg)
    {
        var common = new BiEventFortuneX.Types.Common();

        var biEventFortuneX = new BiEventFortuneX();
        biEventFortuneX.Common = common;
        string messageName = specificMsg.GetType().Name;

        var eventType = biEventFortuneX.GetType();
        var property = eventType.GetProperty(messageName);
        property?.SetValue(biEventFortuneX, specificMsg);

        //biEventFortuneX.GetType().InvokeMember(messageName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetProperty, Type.DefaultBinder, biEventFortuneX, new object[] { specificMsg });


        byte[] bytes = biEventFortuneX.ToByteArray();
        BIManager.Instance.SendEvent(biEventFortuneX.GetType().Name, bytes);


        onSendEvent(biEventFortuneX);
    }

    static void onSendEvent(IMessage message)
    {
        var prop = message.GetType().GetProperty("GameEvent");
        if (prop != null)
        {
            var val = prop.GetValue(message, null);
            if (val != null)
            {
                var prop2 = val.GetType().GetProperty("GameEventType");
                if (prop2 != null)
                {
                    var val2 = prop2.GetValue(val, null);
                    if (val2 != null)
                    {
                        var gameEventType = (BiEventFortuneX.Types.GameEventType) val2;
                        BIManager.Instance.onThirdPartyTracking(gameEventType.ToString());
                    }
                }
            }
        }
    }


    static protected Dictionary<string, string> TupleToDic(params (string, string)[] extrasInfo)
    {
        Dictionary<string, string> extras = null;
        if (extrasInfo != null && extrasInfo.Length > 0)
        {
            extras = new Dictionary<string, string>();
            for (int i = 0; i < extrasInfo.Length; i++)
            {
                extras[extrasInfo[i].Item1] = extrasInfo[i].Item2;
            }
        }

        return extras;
    }


    static public void SendGameEvent(BiEventFortuneX.Types.GameEventType gameEventType,
        Dictionary<string, string> dicExtras = null)
    {
        BiEventFortuneX.Types.GameEvent gameEvent = new BiEventFortuneX.Types.GameEvent
        {
            GameEventType = gameEventType,
        };

        if (dicExtras != null)
        {
            gameEvent.Extras.Add(dicExtras);
        }

        SendBiEvent(gameEvent);
    }

    static public void SendGameEvent(BiEventFortuneX.Types.GameEventType gameEventType,
        params (string, string)[] extrasInfo)
    {
        SendGameEvent(gameEventType, TupleToDic(extrasInfo));
    }
}