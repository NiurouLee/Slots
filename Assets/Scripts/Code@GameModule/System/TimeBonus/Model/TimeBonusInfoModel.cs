// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/10/12/11:50
// Ver : 1.0.0
// Description : TimeBonusInfoModel.cs
// ChangeLog :
// **********************************************

using System.Threading.Tasks;
using DragonU3DSDK.Network.API.ILProtocol;
using UnityEngine;

namespace GameModule
{
    public class TimeBonusInfoModel : Model<TimerBonusInfo>
    {
        public TimeBonusInfoModel()
            : base(ModelType.TYPE_TIME_BONUS_INFO)
        {
        }

        public override async Task FetchModalDataFromServerAsync()
        {
            CGetTimerBonus cGetTimerBonus = new CGetTimerBonus();
            var sGetTimeBonus =
                await APIManagerGameModule.Instance.SendAsync<CGetTimerBonus, SGetTimerBonus>(cGetTimerBonus);

            if (sGetTimeBonus.ErrorCode == 0)
            {
                UpdateModelData(sGetTimeBonus.Response.TimerBonusInfo);
            }
            else
            {
                XDebug.LogError("GetTimeBonusResponseError:" + sGetTimeBonus.ErrorInfo);
            }
        }
 
        public TimerBonusStage GetTimerBonusStage()
        {
            return modelData.BonusStage;
        }

        public ulong GetSpinBuffMultiplier(int spinBuffLevel)
        {
            if (modelData.HourlyTimerBonusInfo.TimerBonusMultiplePercentageList.Count > spinBuffLevel)
            {
                return modelData.HourlyTimerBonusInfo.TimerBonusMultiplePercentageList[spinBuffLevel];
            }

            return 100;
        }
         
        public ulong GetSpinBuffMultiplier()
        {
            return modelData.HourlyTimerBonusInfo.TimerBonusMultiplePercentage;
        }

        public float GetWheelBonusCountDown()
        {
            return modelData.WheelBonus.AwardCountdown - TimeElapseSinceLastUpdate();
        }
        
        public float GetMultiplierCountDown()
        {
            return modelData.HourlyTimerBonusInfo.RefreshCountDown  - TimeElapseSinceLastUpdate();
        }   
        
        public int GetMaxSpinBuffLevel()
        {
            return modelData.HourlyTimerBonusInfo.TimerBonusMultiplePercentageList.Count;;
        }
        
        public ulong GetCurrentCoinBonus()
        {
            if (modelData.HourlyTimerBonusInfo != null)
            {
                var incSpeed = modelData.HourlyTimerBonusInfo.IncSpeedPerSeconds;
                var coin = (ulong) (modelData.HourlyTimerBonusInfo.Coins + TimeElapseSinceLastUpdate() * incSpeed);

                if (coin > modelData.HourlyTimerBonusInfo.CoinsMax)
                    coin = modelData.HourlyTimerBonusInfo.CoinsMax;
                return coin;
            }
            
            return 0;
        }
        
        public ulong GetMaxCollectableCoins()
        {
            if (modelData.HourlyTimerBonusInfo != null)
            {
                return modelData.HourlyTimerBonusInfo.CoinsMax;
            }
            
            return 0;
        }

        public float GetCoinBonusProgress(ulong coin)
        {
            return  (coin)/(float) modelData.HourlyTimerBonusInfo.CoinsMax;
        }
        
        public uint GetWheelBonusProgress()
        {
            if (modelData.WheelBonus != null)
            {
                return modelData.WheelBonus.WheelCurrent;
            }
            
            return 0;
        }
        
        public ulong GetWheelMaxProgress()
        {
            if (modelData.WheelBonus != null)
            {
                return modelData.WheelBonus.WheelTarget;
            }
            
            return 0;
        }
    }
}