using System;
using System.Collections.Generic;
using System.Reflection;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Network.API.ILProtocol;
using DragonU3DSDK.Network.BI;
using Google.ilruntime.Protobuf;
using UnityEngine;

namespace GameModule
{
    public class BiManagerGameModule
    {
        private static BiManagerGameModule _instance;

        public static BiManagerGameModule Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new BiManagerGameModule();
                }

                return _instance;
            }
        }
  
        public BiManagerGameModule()
        {
            var config = DragonU3DSDK.Network.BI.BIManager.Instance.GetThirdPartyTrackingConfig("GAME_EVENT_PASS_LV5");
 
            if (config != null)
                return;

            DragonU3DSDK.Network.BI.BIManager.Instance.AddThirdPartyTrackingConfig("GAME_EVENT_PASS_LV5",
                new DragonU3DSDK.Network.BI.ThirdPartyTrackingConfig
                {
                    eventName = "GAME_EVENT_PASS_LV5",
                    enableAdjust = true,
                    enableFirebase = true,
                    adjustEventToken = "2j9oef",
                });
            DragonU3DSDK.Network.BI.BIManager.Instance.AddThirdPartyTrackingConfig("GAME_EVENT_PASS_LV9",
                new DragonU3DSDK.Network.BI.ThirdPartyTrackingConfig
                {
                    eventName = "GAME_EVENT_PASS_LV9",
                    enableAdjust = true,
                    enableFirebase = true,
                    adjustEventToken = "6grmvr",
                });
            DragonU3DSDK.Network.BI.BIManager.Instance.AddThirdPartyTrackingConfig("GAME_EVENT_PASS_LV10",
                new DragonU3DSDK.Network.BI.ThirdPartyTrackingConfig
                {
                    eventName = "GAME_EVENT_PASS_LV10",
                    enableAdjust = true,
                    enableFirebase = true,
                    adjustEventToken = "8hx45b",
                });
            DragonU3DSDK.Network.BI.BIManager.Instance.AddThirdPartyTrackingConfig("GAME_EVENT_PASS_LV15",
                new DragonU3DSDK.Network.BI.ThirdPartyTrackingConfig
                {
                    eventName = "GAME_EVENT_PASS_LV15",
                    enableAdjust = true,
                    enableFirebase = true,
                    adjustEventToken = "hnalvm",
                });
            DragonU3DSDK.Network.BI.BIManager.Instance.AddThirdPartyTrackingConfig("GAME_EVENT_PASS_LV20",
                new DragonU3DSDK.Network.BI.ThirdPartyTrackingConfig
                {
                    eventName = "GAME_EVENT_PASS_LV20",
                    enableAdjust = true,
                    enableFirebase = true,
                    adjustEventToken = "xapxht",
                });
            DragonU3DSDK.Network.BI.BIManager.Instance.AddThirdPartyTrackingConfig("GAME_EVENT_PASS_LV25",
                new DragonU3DSDK.Network.BI.ThirdPartyTrackingConfig
                {
                    eventName = "GAME_EVENT_PASS_LV25",
                    enableAdjust = true,
                    enableFirebase = true,
                    adjustEventToken = "3qhc07",
                });
            DragonU3DSDK.Network.BI.BIManager.Instance.AddThirdPartyTrackingConfig("GAME_EVENT_PASS_LV30",
                new DragonU3DSDK.Network.BI.ThirdPartyTrackingConfig
                {
                    eventName = "GAME_EVENT_PASS_LV30",
                    enableAdjust = true,
                    enableFirebase = true,
                    adjustEventToken = "9cu661",
                });
            DragonU3DSDK.Network.BI.BIManager.Instance.AddThirdPartyTrackingConfig("GAME_EVENT_PASS_LV40",
                new DragonU3DSDK.Network.BI.ThirdPartyTrackingConfig
                {
                    eventName = "GAME_EVENT_PASS_LV40",
                    enableAdjust = true,
                    enableFirebase = true,
                    adjustEventToken = "4vqap5",
                });
            DragonU3DSDK.Network.BI.BIManager.Instance.AddThirdPartyTrackingConfig("GAME_EVENT_PASS_LV50",
                new DragonU3DSDK.Network.BI.ThirdPartyTrackingConfig
                {
                    eventName = "GAME_EVENT_PASS_LV50",
                    enableAdjust = true,
                    enableFirebase = true,
                    adjustEventToken = "ho57mw",
                });
            DragonU3DSDK.Network.BI.BIManager.Instance.AddThirdPartyTrackingConfig("GAME_EVENT_PASS_LV60",
                new DragonU3DSDK.Network.BI.ThirdPartyTrackingConfig
                {
                    eventName = "GAME_EVENT_PASS_LV60",
                    enableAdjust = true,
                    enableFirebase = true,
                    adjustEventToken = "6ayeru",
                });
            DragonU3DSDK.Network.BI.BIManager.Instance.AddThirdPartyTrackingConfig("GAME_EVENT_PASS_LV70",
                new DragonU3DSDK.Network.BI.ThirdPartyTrackingConfig
                {
                    eventName = "GAME_EVENT_PASS_LV70",
                    enableAdjust = true,
                    enableFirebase = true,
                    adjustEventToken = "hnadzm",
                });
            DragonU3DSDK.Network.BI.BIManager.Instance.AddThirdPartyTrackingConfig("GAME_EVENT_PASS_LV23",
                new DragonU3DSDK.Network.BI.ThirdPartyTrackingConfig
                {
                    eventName = "GAME_EVENT_PASS_LV23",
                    enableAdjust = true,
                    enableFirebase = true,
                    adjustEventToken = "wln9af",
                });
            DragonU3DSDK.Network.BI.BIManager.Instance.AddThirdPartyTrackingConfig("GAME_EVENT_PASS_LV65",
                new DragonU3DSDK.Network.BI.ThirdPartyTrackingConfig
                {
                    eventName = "GAME_EVENT_PASS_LV65",
                    enableAdjust = true,
                    enableFirebase = true,
                    adjustEventToken = "xn7pgc",
                });
            DragonU3DSDK.Network.BI.BIManager.Instance.AddThirdPartyTrackingConfig("GAME_EVENT_PASS_LV75",
                new DragonU3DSDK.Network.BI.ThirdPartyTrackingConfig
                {
                    eventName = "GAME_EVENT_PASS_LV75",
                    enableAdjust = true,
                    enableFirebase = true,
                    adjustEventToken = "gyeekd",
                });
            DragonU3DSDK.Network.BI.BIManager.Instance.AddThirdPartyTrackingConfig("GAME_EVENT_PASS_LV80",
                new DragonU3DSDK.Network.BI.ThirdPartyTrackingConfig
                {
                    eventName = "GAME_EVENT_PASS_LV80",
                    enableAdjust = true,
                    enableFirebase = true,
                    adjustEventToken = "3a5oi1",
                });
            DragonU3DSDK.Network.BI.BIManager.Instance.AddThirdPartyTrackingConfig("GAME_EVENT_PASS_LV90",
                new DragonU3DSDK.Network.BI.ThirdPartyTrackingConfig
                {
                    eventName = "GAME_EVENT_PASS_LV90",
                    enableAdjust = true,
                    enableFirebase = true,
                    adjustEventToken = "5ujcow",
                });
            DragonU3DSDK.Network.BI.BIManager.Instance.AddThirdPartyTrackingConfig("GAME_EVENT_PASS_LV95",
                new DragonU3DSDK.Network.BI.ThirdPartyTrackingConfig
                {
                    eventName = "GAME_EVENT_PASS_LV95",
                    enableAdjust = true,
                    enableFirebase = true,
                    adjustEventToken = "c56cmq",
                });
            DragonU3DSDK.Network.BI.BIManager.Instance.AddThirdPartyTrackingConfig("GAME_EVENT_PASS_LV100",
                new DragonU3DSDK.Network.BI.ThirdPartyTrackingConfig
                {
                    eventName = "GAME_EVENT_PASS_LV100",
                    enableAdjust = true,
                    enableFirebase = true,
                    adjustEventToken = "g6xoyn",
                });
            DragonU3DSDK.Network.BI.BIManager.Instance.AddThirdPartyTrackingConfig("GAME_EVENT_PASS_LV105",
                new DragonU3DSDK.Network.BI.ThirdPartyTrackingConfig
                {
                    eventName = "GAME_EVENT_PASS_LV105",
                    enableAdjust = true,
                    enableFirebase = true,
                    adjustEventToken = "20dn2t",
                });
            DragonU3DSDK.Network.BI.BIManager.Instance.AddThirdPartyTrackingConfig("GAME_EVENT_PASS_LV110",
                new DragonU3DSDK.Network.BI.ThirdPartyTrackingConfig
                {
                    eventName = "GAME_EVENT_PASS_LV110",
                    enableAdjust = true,
                    enableFirebase = true,
                    adjustEventToken = "3pfisb",
                });
            DragonU3DSDK.Network.BI.BIManager.Instance.AddThirdPartyTrackingConfig("GAME_EVENT_PASS_LV120",
                new DragonU3DSDK.Network.BI.ThirdPartyTrackingConfig
                {
                    eventName = "GAME_EVENT_PASS_LV120",
                    enableAdjust = true,
                    enableFirebase = true,
                    adjustEventToken = "scnku9",
                });
            DragonU3DSDK.Network.BI.BIManager.Instance.AddThirdPartyTrackingConfig("GAME_EVENT_PASS_LV130",
                new DragonU3DSDK.Network.BI.ThirdPartyTrackingConfig
                {
                    eventName = "GAME_EVENT_PASS_LV130",
                    enableAdjust = true,
                    enableFirebase = true,
                    adjustEventToken = "auq2hq",
                });
            DragonU3DSDK.Network.BI.BIManager.Instance.AddThirdPartyTrackingConfig("GAME_EVENT_PASS_LV150",
                new DragonU3DSDK.Network.BI.ThirdPartyTrackingConfig
                {
                    eventName = "GAME_EVENT_PASS_LV150",
                    enableAdjust = true,
                    enableFirebase = true,
                    adjustEventToken = "enqfzn",
                });
            DragonU3DSDK.Network.BI.BIManager.Instance.AddThirdPartyTrackingConfig("GAME_EVENT_PURCHASE_ONETIME_199",
                new DragonU3DSDK.Network.BI.ThirdPartyTrackingConfig
                {
                    eventName = "GAME_EVENT_PURCHASE_ONETIME_199",
                    enableAdjust = true,
                    enableFirebase = true,
                    adjustEventToken = "uttzot",
                });
            DragonU3DSDK.Network.BI.BIManager.Instance.AddThirdPartyTrackingConfig("GAME_EVENT_PURCHASE_ONETIME_499",
                new DragonU3DSDK.Network.BI.ThirdPartyTrackingConfig
                {
                    eventName = "GAME_EVENT_PURCHASE_ONETIME_499",
                    enableAdjust = true,
                    enableFirebase = true,
                    adjustEventToken = "go54r2",
                });
            DragonU3DSDK.Network.BI.BIManager.Instance.AddThirdPartyTrackingConfig("GAME_EVENT_PURCHASE_ONETIME_999",
                new DragonU3DSDK.Network.BI.ThirdPartyTrackingConfig
                {
                    eventName = "GAME_EVENT_PURCHASE_ONETIME_999",
                    enableAdjust = true,
                    enableFirebase = true,
                    adjustEventToken = "u211w9",
                });
            DragonU3DSDK.Network.BI.BIManager.Instance.AddThirdPartyTrackingConfig("GAME_EVENT_PURCHASE_ONETIME_1999",
                new DragonU3DSDK.Network.BI.ThirdPartyTrackingConfig
                {
                    eventName = "GAME_EVENT_PURCHASE_ONETIME_1999",
                    enableAdjust = true,
                    enableFirebase = true,
                    adjustEventToken = "we42xq",
                });
            DragonU3DSDK.Network.BI.BIManager.Instance.AddThirdPartyTrackingConfig("GAME_EVENT_PURCHASE_ONETIME_4999",
                new DragonU3DSDK.Network.BI.ThirdPartyTrackingConfig
                {
                    eventName = "GAME_EVENT_PURCHASE_ONETIME_4999",
                    enableAdjust = true,
                    enableFirebase = true,
                    adjustEventToken = "1k7xxs",
                });
            DragonU3DSDK.Network.BI.BIManager.Instance.AddThirdPartyTrackingConfig("GAME_EVENT_COMPLETE_ADRUSH_10ADS",
                new DragonU3DSDK.Network.BI.ThirdPartyTrackingConfig
                {
                    eventName = "GAME_EVENT_COMPLETE_ADRUSH_10ADS",
                    enableAdjust = true,
                    enableFirebase = true,
                    adjustEventToken = "v7ms57",
                });
            DragonU3DSDK.Network.BI.BIManager.Instance.AddThirdPartyTrackingConfig("GAME_EVENT_PASS_LV32",
                new DragonU3DSDK.Network.BI.ThirdPartyTrackingConfig
                {
                    eventName = "GAME_EVENT_PASS_LV32",
                    enableAdjust = true,
                    enableFirebase = true,
                    adjustEventToken = "em2gy3",
                });
            DragonU3DSDK.Network.BI.BIManager.Instance.AddThirdPartyTrackingConfig("GAME_EVENT_PASS_LV35",
                new DragonU3DSDK.Network.BI.ThirdPartyTrackingConfig
                {
                    eventName = "GAME_EVENT_PASS_LV35",
                    enableAdjust = true,
                    enableFirebase = true,
                    adjustEventToken = "8o5s8d",
                });
            DragonU3DSDK.Network.BI.BIManager.Instance.AddThirdPartyTrackingConfig("GAME_EVENT_PASS_LV160",
                new DragonU3DSDK.Network.BI.ThirdPartyTrackingConfig
                {
                    eventName = "GAME_EVENT_PASS_LV160",
                    enableAdjust = true,
                    enableFirebase = true,
                    adjustEventToken = "pyx67h",
                });
            DragonU3DSDK.Network.BI.BIManager.Instance.AddThirdPartyTrackingConfig("GAME_EVENT_PASS_LV180",
                new DragonU3DSDK.Network.BI.ThirdPartyTrackingConfig
                {
                    eventName = "GAME_EVENT_PASS_LV180",
                    enableAdjust = true,
                    enableFirebase = true,
                    adjustEventToken = "e5ry76",
                });
            DragonU3DSDK.Network.BI.BIManager.Instance.AddThirdPartyTrackingConfig("GAME_EVENT_PASS_LV200",
                new DragonU3DSDK.Network.BI.ThirdPartyTrackingConfig
                {
                    eventName = "GAME_EVENT_PASS_LV200",
                    enableAdjust = true,
                    enableFirebase = true,
                    adjustEventToken = "ljmrms",
                });
            DragonU3DSDK.Network.BI.BIManager.Instance.AddThirdPartyTrackingConfig("GAME_EVENT_PASS_LV220",
                new DragonU3DSDK.Network.BI.ThirdPartyTrackingConfig
                {
                    eventName = "GAME_EVENT_PASS_LV220",
                    enableAdjust = true,
                    enableFirebase = true,
                    adjustEventToken = "fxbcp7",
                });
            DragonU3DSDK.Network.BI.BIManager.Instance.AddThirdPartyTrackingConfig("GAME_EVENT_PASS_LV240",
                new DragonU3DSDK.Network.BI.ThirdPartyTrackingConfig
                {
                    eventName = "GAME_EVENT_PASS_LV240",
                    enableAdjust = true,
                    enableFirebase = true,
                    adjustEventToken = "swn37q",
                });
            DragonU3DSDK.Network.BI.BIManager.Instance.AddThirdPartyTrackingConfig("GAME_EVENT_PASS_LV260",
                new DragonU3DSDK.Network.BI.ThirdPartyTrackingConfig
                {
                    eventName = "GAME_EVENT_PASS_LV260",
                    enableAdjust = true,
                    enableFirebase = true,
                    adjustEventToken = "rahmqs",
                });
            DragonU3DSDK.Network.BI.BIManager.Instance.AddThirdPartyTrackingConfig("GAME_EVENT_PASS_LV280",
                new DragonU3DSDK.Network.BI.ThirdPartyTrackingConfig
                {
                    eventName = "GAME_EVENT_PASS_LV280",
                    enableAdjust = true,
                    enableFirebase = true,
                    adjustEventToken = "quuhsb",
                });
            DragonU3DSDK.Network.BI.BIManager.Instance.AddThirdPartyTrackingConfig("GAME_EVENT_PASS_LV320",
                new DragonU3DSDK.Network.BI.ThirdPartyTrackingConfig
                {
                    eventName = "GAME_EVENT_PASS_LV320",
                    enableAdjust = true,
                    enableFirebase = true,
                    adjustEventToken = "2sim1d",
                });
            DragonU3DSDK.Network.BI.BIManager.Instance.AddThirdPartyTrackingConfig("GAME_EVENT_PASS_LV350",
                new DragonU3DSDK.Network.BI.ThirdPartyTrackingConfig
                {
                    eventName = "GAME_EVENT_PASS_LV350",
                    enableAdjust = true,
                    enableFirebase = true,
                    adjustEventToken = "rxu8xh",
                });
            DragonU3DSDK.Network.BI.BIManager.Instance.AddThirdPartyTrackingConfig("GAME_EVENT_PASS_LV380",
                new DragonU3DSDK.Network.BI.ThirdPartyTrackingConfig
                {
                    eventName = "GAME_EVENT_PASS_LV380",
                    enableAdjust = true,
                    enableFirebase = true,
                    adjustEventToken = "syhpt9",
                });
            DragonU3DSDK.Network.BI.BIManager.Instance.AddThirdPartyTrackingConfig("GAME_EVENT_PASS_LV400",
                new DragonU3DSDK.Network.BI.ThirdPartyTrackingConfig
                {
                    eventName = "GAME_EVENT_PASS_LV400",
                    enableAdjust = true,
                    enableFirebase = true,
                    adjustEventToken = "iql5mi",
                });
            DragonU3DSDK.Network.BI.BIManager.Instance.AddThirdPartyTrackingConfig("GAME_EVENT_PASS_LV430",
                new DragonU3DSDK.Network.BI.ThirdPartyTrackingConfig
                {
                    eventName = "GAME_EVENT_PASS_LV430",
                    enableAdjust = true,
                    enableFirebase = true,
                    adjustEventToken = "bfa0n1",
                });
            DragonU3DSDK.Network.BI.BIManager.Instance.AddThirdPartyTrackingConfig("GAME_EVENT_PASS_LV450",
                new DragonU3DSDK.Network.BI.ThirdPartyTrackingConfig
                {
                    eventName = "GAME_EVENT_PASS_LV450",
                    enableAdjust = true,
                    enableFirebase = true,
                    adjustEventToken = "1ztmhm",
                });
            DragonU3DSDK.Network.BI.BIManager.Instance.AddThirdPartyTrackingConfig("GAME_EVENT_PASS_LV470",
                new DragonU3DSDK.Network.BI.ThirdPartyTrackingConfig
                {
                    eventName = "GAME_EVENT_PASS_LV470",
                    enableAdjust = true,
                    enableFirebase = true,
                    adjustEventToken = "uc1ozg",
                });
            DragonU3DSDK.Network.BI.BIManager.Instance.AddThirdPartyTrackingConfig("GAME_EVENT_PASS_LV500",
                new DragonU3DSDK.Network.BI.ThirdPartyTrackingConfig
                {
                    eventName = "GAME_EVENT_PASS_LV500",
                    enableAdjust = true,
                    enableFirebase = true,
                    adjustEventToken = "5ej4kl",
                });
            DragonU3DSDK.Network.BI.BIManager.Instance.AddThirdPartyTrackingConfig("GAME_EVENT_PASS_LV520",
                new DragonU3DSDK.Network.BI.ThirdPartyTrackingConfig
                {
                    eventName = "GAME_EVENT_PASS_LV520",
                    enableAdjust = true,
                    enableFirebase = true,
                    adjustEventToken = "504axu",
                });
            DragonU3DSDK.Network.BI.BIManager.Instance.AddThirdPartyTrackingConfig("GAME_EVENT_PASS_LV550",
                new DragonU3DSDK.Network.BI.ThirdPartyTrackingConfig
                {
                    eventName = "GAME_EVENT_PASS_LV550",
                    enableAdjust = true,
                    enableFirebase = true,
                    adjustEventToken = "d63wq3",
                });
            DragonU3DSDK.Network.BI.BIManager.Instance.AddThirdPartyTrackingConfig("GAME_EVENT_PASS_LV570",
                new DragonU3DSDK.Network.BI.ThirdPartyTrackingConfig
                {
                    eventName = "GAME_EVENT_PASS_LV570",
                    enableAdjust = true,
                    enableFirebase = true,
                    adjustEventToken = "sbsj4y",
                });
            DragonU3DSDK.Network.BI.BIManager.Instance.AddThirdPartyTrackingConfig("GAME_EVENT_PASS_LV600",
                new DragonU3DSDK.Network.BI.ThirdPartyTrackingConfig
                {
                    eventName = "GAME_EVENT_PASS_LV600",
                    enableAdjust = true,
                    enableFirebase = true,
                    adjustEventToken = "876vm5",
                });

            DragonU3DSDK.Network.BI.BIManager.Instance.AddThirdPartyTrackingConfig("GAME_EVENT_PURCHASE_SHOP_COIN_199",
                new DragonU3DSDK.Network.BI.ThirdPartyTrackingConfig
                {
                    eventName = "GAME_EVENT_PURCHASE_SHOP_COIN_199",
                    enableAdjust = true,
                    enableFirebase = true,
                    adjustEventToken = "xtpeo5",
                });
            DragonU3DSDK.Network.BI.BIManager.Instance.AddThirdPartyTrackingConfig("GAME_EVENT_PURCHASE_SHOP_COIN_499",
                new DragonU3DSDK.Network.BI.ThirdPartyTrackingConfig
                {
                    eventName = "GAME_EVENT_PURCHASE_SHOP_COIN_499",
                    enableAdjust = true,
                    enableFirebase = true,
                    adjustEventToken = "s8mvwc",
                });
            DragonU3DSDK.Network.BI.BIManager.Instance.AddThirdPartyTrackingConfig("GAME_EVENT_PURCHASE_SHOP_COIN_999",
                new DragonU3DSDK.Network.BI.ThirdPartyTrackingConfig
                {
                    eventName = "GAME_EVENT_PURCHASE_SHOP_COIN_999",
                    enableAdjust = true,
                    enableFirebase = true,
                    adjustEventToken = "w25fo4",
                });
            DragonU3DSDK.Network.BI.BIManager.Instance.AddThirdPartyTrackingConfig("GAME_EVENT_PURCHASE_SHOP_COIN_1999",
                new DragonU3DSDK.Network.BI.ThirdPartyTrackingConfig
                {
                    eventName = "GAME_EVENT_PURCHASE_SHOP_COIN_1999",
                    enableAdjust = true,
                    enableFirebase = true,
                    adjustEventToken = "7bzr9k",
                });
            DragonU3DSDK.Network.BI.BIManager.Instance.AddThirdPartyTrackingConfig("heart_beat",
                new DragonU3DSDK.Network.BI.ThirdPartyTrackingConfig
                {
                    eventName = "heart_beat",
                    enableAdjust = true,
                    enableFirebase = true,
                    adjustEventToken = "fi2uz2",
                });
            DragonU3DSDK.Network.BI.BIManager.Instance.AddThirdPartyTrackingConfig("social_login",
                new DragonU3DSDK.Network.BI.ThirdPartyTrackingConfig
                {
                    eventName = "social_login",
                    enableAdjust = true,
                    enableFirebase = true,
                    adjustEventToken = "190ljz",
                });
            DragonU3DSDK.Network.BI.BIManager.Instance.AddThirdPartyTrackingConfig("purchase",
                new DragonU3DSDK.Network.BI.ThirdPartyTrackingConfig
                {
                    eventName = "purchase",
                    enableAdjust = true,
                    enableFirebase = true,
                    adjustEventToken = "gnl0t4",
                });
            DragonU3DSDK.Network.BI.BIManager.Instance.AddThirdPartyTrackingConfig(
                "marketing_purchase_probability_most_likely", new DragonU3DSDK.Network.BI.ThirdPartyTrackingConfig
                {
                    eventName = "marketing_purchase_probability_most_likely",
                    enableAdjust = true,
                    enableFirebase = true,
                    adjustEventToken = "9mfts4",
                });
        }

        protected void SendBiEvent(IMessage specificMsg)
        {
            var common = new BiEventFortuneX.Types.Common();
            UserController userController = Client.Get<UserController>();
            if (userController != null && userController.IsValid())
            {
                common.Coin = userController.GetCoinsCount();
                common.Emerald = userController.GetDiamondCount();
                common.Exp = userController.GetUserLevelInfo().ExpCurrent;
                common.Level = userController.GetUserLevel();
                common.VipLevel = userController.GetVipInfo().Level;
                common.VipPoints = userController.GetVipInfo().ExpCurrent;
            }


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


        void onSendEvent(IMessage message)
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
                            XDebug.Log("onThirdPartyTracking:" + gameEventType);
                            BIManager.Instance.onThirdPartyTracking(gameEventType.ToString());
                        }
                    }
                }
            }
        }


        protected Dictionary<string, string> TupleToDic(params (string, string)[] extrasInfo)
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


        public void SendGameEvent(BiEventFortuneX.Types.GameEventType gameEventType,
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

        public void SendGameEvent(BiEventFortuneX.Types.GameEventType gameEventType,
            params (string, string)[] extrasInfo)
        {
            SendGameEvent(gameEventType, TupleToDic(extrasInfo));
        }


        public void SendSpinAction(string machineId, BiEventFortuneX.Types.SpinActionType spinActionType, bool isAuto,
            ulong bet, string guid, Dictionary<string, string> dicExtras = null)
        {
            BiEventFortuneX.Types.SpinAction spinAction = new BiEventFortuneX.Types.SpinAction();
            spinAction.GameId = machineId;
            spinAction.SpinActionType = spinActionType;
            spinAction.IsAuto = isAuto;
            spinAction.Bet = bet;
            spinAction.RequestId = guid;

            if (dicExtras != null)
            {
                spinAction.Extras.Add(dicExtras);
            }

            SendBiEvent(spinAction);
        }

        public void SendSpinAction(string machineId, BiEventFortuneX.Types.SpinActionType spinActionType, bool isAuto,
            ulong bet, string guid, params (string, string)[] extrasInfo)
        {
            SendSpinAction(machineId, spinActionType, isAuto, bet, guid, TupleToDic(extrasInfo));
        }


        public void SendItemChange(BiEventFortuneX.Types.Item itemType,
            BiEventFortuneX.Types.ItemChangeReason itemChangeReason,
            long amount, ulong current, Dictionary<string, string> dicExtras = null)
        {
            BiEventFortuneX.Types.ItemChange itemChange = new BiEventFortuneX.Types.ItemChange();
            itemChange.Item = itemType;
            itemChange.Reason = itemChangeReason;
            itemChange.Amount = amount;
            itemChange.Current = current;
            if (dicExtras != null)
            {
                itemChange.Extras.Add(dicExtras);
            }

            SendBiEvent(itemChange);
        }


        public void SendItemChange(BiEventFortuneX.Types.Item itemType,
            BiEventFortuneX.Types.ItemChangeReason itemChangeReason,
            long amount, ulong current, params (string, string)[] extrasInfo)
        {
            SendItemChange(itemType, itemChangeReason, amount, current, TupleToDic(extrasInfo));
        }

        public void SendUserAction(string actionName)
        {
            // var dic = TupleToDic(extrasInfo);
            //
            // if(dic != null && !dic.ContainsKey("ActionName"))
            //     dic["ActionName"] = actionName;
            // else
            // {
            //     dic = new Dictionary<string, string>();
            //     dic["ActionName"] = actionName;
            // }
          
            SendGameEvent(BiEventFortuneX.Types.GameEventType.GaemEventUserAction, ("ActionName", actionName));
        }
        
        public void SendUserAction(string actionName, params (string, string)[] extrasInfo)
        {
            var dic = TupleToDic(extrasInfo);
            
            if(dic != null && !dic.ContainsKey("ActionName"))
                dic["ActionName"] = actionName;
            else
            {
                dic = new Dictionary<string, string>();
                dic["ActionName"] = actionName;
            }
          
            SendGameEvent(BiEventFortuneX.Types.GameEventType.GaemEventUserAction, dic);
        }
    }
}