//  **********************************************
//  Copyright(c) 2021 by com.ustar
//  All right reserved
// 
//  Author : Besure.Chen
//  Date :2021-10-08 15:13
//  Ver : 1.0.0
//  Description : SeasonPassModel.cs
//  ChangeLog :
//  **********************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DragonU3DSDK.Network.API.ILProtocol;
using Google.ilruntime.Protobuf.Collections;
using UnityEngine;

namespace GameModule
{
    public class SeasonPassModel : Model<MissionPass>
    {
        public bool IsNewSeason;
        public bool IsLocked => modelData == null || modelData.IsLocked;
        public uint UnlockLevel => modelData.UnlockLevel;
        public uint Season => modelData.Season;
        public ulong TimestampLeft => modelData.TimestampLeft;
        public uint Level => modelData.Level;
        public ulong Exp => modelData.Exp;
        public ulong ExpTotal => modelData.ExpTotal;
        public bool Paid => modelData.Paid;
        public Reward FinalReward => modelData.FinalReward;
        public RepeatedField<ShopItemConfig> ShopItemConfigs;
        public Dictionary<int, Google.Protobuf.Collections.RepeatedField<MissionPassReward>> FreeMissionPassRewards;
        public Dictionary<int, Google.Protobuf.Collections.RepeatedField<MissionPassReward>> GoldenMissionPassRewards;

        public SeasonPassModel() : base(ModelType.TYPE_SEASON_PASS)
        {
            FreeMissionPassRewards =
                new Dictionary<int, Google.Protobuf.Collections.RepeatedField<MissionPassReward>>();
            GoldenMissionPassRewards =
                new Dictionary<int, Google.Protobuf.Collections.RepeatedField<MissionPassReward>>();
        }

        public override async Task FetchModalDataFromServerAsync()
        {
            CGetMissionPass cGetMissionPass = new CGetMissionPass();
            var sGetMissionPass =
                await APIManagerGameModule.Instance.SendAsync<CGetMissionPass, SGetMissionPass>(cGetMissionPass);

            if (sGetMissionPass.ErrorCode == 0)
            {
                UpdateSeasonPassData(sGetMissionPass.Response.MissionPass);
            }
            else
            {
                XDebug.LogError("GetMissionPassResponseError:" + sGetMissionPass.ErrorInfo);
            }
        }
        
        public async Task RefreshModeDataFromServer()
        {
            var oldSeason = Season;
            CRefreshMissionPass cRefreshMissionPass = new CRefreshMissionPass();
            var sRefreshMissionPass =
                await APIManagerGameModule.Instance.SendAsync<CRefreshMissionPass, SRefreshMissionPass>(cRefreshMissionPass);

            if (sRefreshMissionPass.ErrorCode == 0)
            {
                UpdateSeasonPassData(sRefreshMissionPass.Response.MissionPass);
                Client.Get<SeasonPassController>().SaveNewSeason(Season != oldSeason);
            }
            else
            {
                XDebug.LogError("RefreshMissionPassResponseError:" + sRefreshMissionPass.ErrorInfo);
            }
        }

        public async void CollectMissionPass(uint level, bool isPaid, bool isTimed, Action<RepeatedField<Reward>, Action> action)
        {
            CCollectMissionPass cCollectMissionPass = new CCollectMissionPass();
            cCollectMissionPass.Level = level;
            cCollectMissionPass.IsPaid = isPaid;
            cCollectMissionPass.IsTimed = isTimed;
            var sCollectMissionPass =
                await APIManagerGameModule.Instance.SendAsync<CCollectMissionPass, SCollectMissionPass>(cCollectMissionPass);

            if (sCollectMissionPass.ErrorCode == 0)
            {
                EventBus.Dispatch(new EventUserProfileUpdate(sCollectMissionPass.Response.UserProfile));
                action?.Invoke(sCollectMissionPass.Response.Rewards, ()=>
                {
                    
                    XDebug.LogOnExceptionHandler(LitJson.JsonMapper.ToJsonField(sCollectMissionPass.Response.Rewards));
                    XDebug.LogOnExceptionHandler("MissionPassEmeraldCount:" + GetEmeraldCount(sCollectMissionPass.Response.Rewards));
                    
                    EventBus.Dispatch(new EventEmeraldBalanceUpdate(GetEmeraldCount(sCollectMissionPass.Response.Rewards),"CollectAllMissionPass",false,true));
                    UpdateSeasonPassData(sCollectMissionPass.Response.MissionPass);
                });
            }
            else
            {
                XDebug.LogError("CollectMissionPassResponseError:" + sCollectMissionPass.ErrorInfo);
            }
        }
        public async Task CollectAllMissionPass(Action<RepeatedField<Reward>, Action> callback)
        {
            CCollectAllMissionPass cCollectAllMissionPass = new CCollectAllMissionPass();
            var sCollectAllMissionPass =
                await APIManagerGameModule.Instance.SendAsync<CCollectAllMissionPass, SCollectAllMissionPass>(cCollectAllMissionPass);

            if (sCollectAllMissionPass.ErrorCode == 0)
            {
                EventBus.Dispatch(new EventUserProfileUpdate(sCollectAllMissionPass.Response.UserProfile));
                callback?.Invoke(sCollectAllMissionPass.Response.Rewards, () =>
                {
                    EventBus.Dispatch(new EventEmeraldBalanceUpdate(GetEmeraldCount(sCollectAllMissionPass.Response.Rewards),"CollectAllMissionPass",false,true));
                    UpdateSeasonPassData(sCollectAllMissionPass.Response.MissionPass);
                });
            }
            else
            {
                XDebug.LogError("CollectAllMissionPassResponseError:" + sCollectAllMissionPass.ErrorInfo);
            }
        }

        private ulong GetEmeraldCount(RepeatedField<Reward> rewards)
        {
            ulong emeraldCount = 0;
            var items = XItemUtility.GetItems(rewards, Item.Types.Type.Emerald);
            for (int i = 0; i < items.Count; i++)
            {
                emeraldCount += items[i].Emerald.Amount;
            }
            items = XItemUtility.GetItems(rewards, Item.Types.Type.ShopEmerald);
            for (int i = 0; i < items.Count; i++)
            {
                emeraldCount += items[i].Emerald.Amount;
            }
            return emeraldCount;
        }

        public async Task<SGetMissionPassPaymentItems> FetchShopItems()
        {
            CGetMissionPassPaymentItems cGetMissionPassPaymentItems = new CGetMissionPassPaymentItems();
            var sGetMissionPassPaymentItems =
                await APIManagerGameModule.Instance.SendAsync<CGetMissionPassPaymentItems, SGetMissionPassPaymentItems>(
                    cGetMissionPassPaymentItems);

            if (sGetMissionPassPaymentItems.ErrorCode == 0)
            {
                ShopItemConfigs = sGetMissionPassPaymentItems.Response.Items;
                return sGetMissionPassPaymentItems.Response;
            }
            else
            {
                XDebug.LogError("GetMissionPassPaymentItemsResponseError:" + sGetMissionPassPaymentItems.ErrorInfo);
            }
            return null;
        }
        
        public void UpdateSeasonPassData(MissionPass missionPass)
        {
            FreeMissionPassRewards.Clear();
            for (int i = 0; i < missionPass.FreeRewards.Count; i++)
            {
                var item = missionPass.FreeRewards[i];
                AddReward(in FreeMissionPassRewards, in item);
            }
            for (int i = 0; i < missionPass.FreeTimedRewards.Count; i++)
            {
                var item = missionPass.FreeTimedRewards[i];
                AddReward(in FreeMissionPassRewards, in item);
            }
            GoldenMissionPassRewards.Clear();
            for (int i = 0; i < missionPass.PaidRewards.Count; i++)
            {
                var item = missionPass.PaidRewards[i];
                AddReward(in GoldenMissionPassRewards, in item);   
            }
            for (int i = 0; i < missionPass.PaidTimedRewards.Count; i++)
            {
                var item = missionPass.PaidTimedRewards[i];
                AddReward(in GoldenMissionPassRewards, in item);   
            }
            
            UpdateModelData(missionPass);
        }

        private void AddReward(
            in Dictionary<int, Google.Protobuf.Collections.RepeatedField<MissionPassReward>> rewards,
            in MissionPassReward reward)
        {
            if (!rewards.ContainsKey((int)reward.Level))
            {
                rewards.Add((int)reward.Level, new Google.Protobuf.Collections.RepeatedField<MissionPassReward>());
            }
            rewards[(int)reward.Level].Add(reward); 
        }
        
        public long GetLimitedMissionTimeLeft(MissionPassReward reward)
        {
            return (long) reward.TimestampLeft -
                   (long) TimeSpan.FromSeconds(Time.realtimeSinceStartup - lastTimeUpdateData).TotalSeconds;
        }
        
        public long GetSeasonPassTimeLeft()
        {
            return (long) TimestampLeft -
                   (long) TimeSpan.FromSeconds(Time.realtimeSinceStartup - lastTimeUpdateData).TotalSeconds;
        }
        
        public MissionPassReward GetNewMissionReward(uint level, bool isPaid, bool isTimed)
        {
            var dictMissionRewards = isPaid ? GoldenMissionPassRewards : FreeMissionPassRewards;
            if (dictMissionRewards.ContainsKey((int)level))
            {
                return dictMissionRewards[(int)level][isTimed ? 1 : 0];
            }
            return null;
        }

        public RepeatedField<Reward> GetGoldenRewards()
        {
            RepeatedField<Reward> listRewards = new RepeatedField<Reward>();
            var listPaidKeys = GoldenMissionPassRewards.Keys.ToList();
            for (int i = 0; i < listPaidKeys.Count; i++)
            {
                var rewards = GoldenMissionPassRewards[listPaidKeys[i]];
                for (int j = 0; j < rewards.Count; j++)
                {
                    if (rewards[j].Level <= Level && !rewards[j].Collected)
                    {
                        if (rewards[j].IsTimed)
                        {
                            if (!rewards[j].TimeOver)
                            {
                                listRewards.Add(rewards[j].Reward);      
                            }
                        }
                        else
                        {
                            listRewards.Add(rewards[j].Reward);   
                        }
                    }
                }
            }
            return listRewards;
        }

        public RepeatedField<Reward> GetFreeRewards()
        {
            RepeatedField<Reward> listRewards = new RepeatedField<Reward>();
            var listPaidKeys = FreeMissionPassRewards.Keys.ToList();
            for (int i = 0; i < listPaidKeys.Count; i++)
            {
                var rewards = FreeMissionPassRewards[listPaidKeys[i]];
                for (int j = 0; j < rewards.Count; j++)
                {
                    if (rewards[j].Level <= Level && !rewards[j].Collected)
                    {
                        if (rewards[j].IsTimed)
                        {
                            if (!rewards[j].TimeOver)
                            {
                                listRewards.Add(rewards[j].Reward);      
                            }
                        }
                        else
                        {
                            listRewards.Add(rewards[j].Reward);   
                        }
                    }
                }
            }
            return listRewards;
        }

        public int GetCollectRewardCount()
        {
            int nTotalCount = 0;
            var rewards = GetFreeRewards();
            for (int i = 0; i < rewards.Count; i++)
            {
                var reward = rewards[i];
                if (reward != null)
                {
                    nTotalCount++;
                }
            }

            if (Paid)
            {
                rewards = GetGoldenRewards();
                for (int i = 0; i < rewards.Count; i++)
                {
                    var reward = rewards[i];
                    if (reward != null)
                    {
                        nTotalCount++;
                    }
                }
            }
            return nTotalCount;
        }
    }
}