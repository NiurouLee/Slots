// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/10/14/14:15
// Ver : 1.0.0
// Description : WheelUIInitializeHelper.cs
// ChangeLog :
// **********************************************

using System;
using DragonU3DSDK.Network.API.ILProtocol;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameModule
{
    public static class WheelUIInitializeHelper
    {
        public static void InitializeWheelUI(Transform rewardGroup, CommonWheel wheelInfo, int numberOfDigitsToSimply, double coinScaleFactor = 1.0)
        {
            var childCount = rewardGroup.childCount;
            var wedgeCount = wheelInfo.Wedge.Count;

            if (childCount != wedgeCount)
            {
                XDebug.Log("Error Wedge Info Doesn't Match UI");
                return;
            }

            for (var i = 0; i < childCount; i++)
            {
                var wedge = rewardGroup.GetChild(i);
                var wedgeInfo = wheelInfo.Wedge[i];

                switch (wedgeInfo.RewardType)
                {
                    case "coin":
                        var amount = (wedgeInfo.Item.Coin.Amount * coinScaleFactor);
                        var numText = wedge.Find("IntegralText").GetComponent<TextMeshProUGUI>();
                        if (amount > Math.Pow(10, numberOfDigitsToSimply))
                        {
                            numText.text = amount.GetAbbreviationFormat(2); 
                        }
                        else
                        {
                            numText.text = amount.GetCommaFormat();
                        }

                        break;
                    case "emerald":
                        var emeraldNumText = wedge.Find("IntegralText").GetComponent<TextMeshProUGUI>();
                        emeraldNumText.text = "X" + wedgeInfo.Item.Emerald.Amount.GetCommaFormat();
                        break;
                }
            }
        }
    }
}