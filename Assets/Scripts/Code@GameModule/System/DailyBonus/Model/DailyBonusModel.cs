// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/08/23/11:44
// Ver : 1.0.0
// Description : DailyBonusModal.cs
// ChangeLog :
// **********************************************

using System;
using System.Threading.Tasks;
using DragonU3DSDK.Network.API.ILProtocol;

using UnityEngine;
namespace GameModule
{
    public class DailyBonusModel : Model<DailyBonus>
    {
        public DailyBonusModel() : base(ModelType.TYPE_DAILY_BONUS)
        {
        }

        public bool CheckHasDailyRewardToClaim()
        {
            return GetLeftTime() <= 0;
        }

        public bool CheckHasMonthRewardToClaim()
        {
            if(modelData.MonthStage < modelData.MonthRewards.Count)
                return modelData.MonthRewards[(int) modelData.MonthStage].Step <= modelData.MonthStep;
            return false;
        }
        
        public int GetWeekStep()
        {
            return (int)modelData.WeekStep;
        }
        
        public int GetWeekRewardIndex()
        {
            return (int)modelData.WeekRewardIndex;
        }

        public bool IsWeekRewardIndexValid()
        {
            return modelData.WeekRewardIndex < modelData.WeekRewards.Count;
        }
        
        public int GetMonthStep()
        {
            return (int)modelData.MonthStep;
        }
        
        public int GetMonthStage()
        {
            return (int)modelData.MonthStage;
        }
        
        public uint GetWatchRvExtraCoinAddition()
        {
            return modelData.ForAd;
        }
        
        public string GetWatchRvExtraCoinDesc()
        {
            return modelData.Desc;
        }


        public long GetLeftTime()
        {
            return (long) modelData.TimestampLeft -
                   (long) TimeSpan.FromSeconds((Time.realtimeSinceStartup - lastTimeUpdateData)).TotalSeconds;
        }
        
        public Reward GetDailyRewardInfo(int dayIndex)
        {
            return modelData.WeekRewards[dayIndex];
        }

        public MonthReward GetMonthRewardInfo(int stageIndex)
        {
            if (stageIndex < modelData.MonthRewards.Count)
            {
                return modelData.MonthRewards[stageIndex];
            }

            return null;
        }

        public override async Task FetchModalDataFromServerAsync()
        {
            CGetDailyBonus cGetDailyBonus = new CGetDailyBonus();
            var sGetDailyBonus =
                await APIManagerGameModule.Instance.SendAsync<CGetDailyBonus, SGetDailyBonus>(cGetDailyBonus);

            if (sGetDailyBonus.ErrorCode == 0)
            {
                UpdateModelData(sGetDailyBonus.Response.DailyBonus);
            }
            else
            {
                XDebug.LogError("GetDailyBonusResponseError:" + sGetDailyBonus.ErrorInfo);
            }
        }
    }
}