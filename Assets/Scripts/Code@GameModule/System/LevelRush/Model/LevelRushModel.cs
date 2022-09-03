// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2022/06/09/20:52
// Ver : 1.0.0
// Description : LevelRushPopupInfo.cs
// ChangeLog :
// **********************************************

using System.Threading.Tasks;
using DragonU3DSDK.Network.API.ILProtocol;

namespace GameModule
{
    public class LevelRushModel:Model<LevelRushPopupInfo>
    {
        private Reward rewardToGet;
        private bool hasRewardToClaim = false;
        public int   rewardIndex = 0;
        private bool failTrigged = false;
        public bool isFinish = true;

        public bool levelRushTriggered = false;
        public LevelRushModel()
            : base(ModelType.TYPE_LEVEL_RUSH)
        {
            
        }

        public override async Task FetchModalDataFromServerAsync()
        {
            CGetLevelRushPopUpInfo c = new CGetLevelRushPopUpInfo();

            var response =
                await APIManagerGameModule.Instance.SendAsync<CGetLevelRushPopUpInfo, SGetLevelRushPopUpInfo>(c);

            if (response.ErrorCode == 0)
            {
                UpdateModelData(response.Response.LevelRushPopUpInfo);
                levelRushTriggered = false;
            }
        }

        public override void UpdateModelData(LevelRushPopupInfo inModelData)
        {
            base.UpdateModelData(inModelData);

            if (modelData != null)
            {
                rewardToGet = inModelData.RewardGot;

                if (rewardToGet != null)
                {
                    var count = modelData.LevelRewardList.Count;
                    for (var i = count - 1; i > 0; i--)
                    {
                        if (modelData.LevelRewardList[i].Received)
                        {
                            rewardIndex = i;
                            break;
                        }
                    }
                }
                
                if (GetLeftTime() > 0 && modelData.LevelCurrent == Client.Get<UserController>().GetUserLevel())
                {
                    levelRushTriggered = true;
                }
            }
        }

        public void OnRewardClaimFinished()
        {
            rewardToGet = null;
            if (modelData != null)
                rewardIndex = modelData.LevelRewardList.Count;
            levelRushTriggered = false;
        }

        public bool IsAllRewardReceived()
        {
            if (modelData != null)
            {
                var count = modelData.LevelRewardList.Count;
                for (var i = count - 1; i > 0; i--)
                {
                    if (!modelData.LevelRewardList[i].Received)
                    {
                        return false;
                    }
                }
                return true;
            }
            return false;
        }

        public float GetLeftTime()
        {
            if (modelData != null)
            {
                return XUtility.GetLeftTime((ulong) modelData.EndAt * 1000);
            }

            return 0;
        }

        public uint GetBeginLevel()
        {
            return modelData.LevelCurrent;
        }
        
        public uint GetTargetLevel()
        {
            return modelData.LevelTarget;
        }

        public Reward GetReward()
        {
            return rewardToGet;
        }

        public LevelRushPopupInfo.Types.LevelRewardInfo GetRewardInfo(int index)
        {
            if (modelData != null)
            {
                if (modelData.LevelRewardList.Count > index)
                {
                    return modelData.LevelRewardList[index];
                }
            }
            return null;
        }
        
        //倒计时没结束，还有奖励没领取，才算开启。
        public bool IsEnabled()
        {
            if (modelData != null)
            {
                var leftTime = XUtility.GetLeftTime((ulong) modelData.EndAt * 1000);
                if (leftTime > 0)
                {
                    var rewardList = modelData.LevelRewardList;
                    for (var i = 1; i < rewardList.Count; i++)
                    {
                        if (!rewardList[i].Received)
                        {
                            return true;
                        }
                    }
                    
                    if (rewardToGet != null)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}