
using System;
using DragonU3DSDK.Network.API.ILProtocol;
using static DragonU3DSDK.Network.API.ILProtocol.SGetActivityUserData.Types;
using static DragonU3DSDK.Network.API.ILProtocol.RushPassPopUpInfo.Types;
using System.Collections.Generic;
using System.Threading.Tasks;
using Google.ilruntime.Protobuf.Collections;
using Google.Protobuf.Reflection;

namespace GameModule
{
    public class Activity_LevelRushRushPass : ActivityBase
    {
        public Activity_LevelRushRushPass() : base(ActivityType.RushPass)
        {
        }


        public RushPassActivityConfigPB config { get; private set; }

        public RushPassActivityDataPB userData { get; private set; }

       
        public APIFixHotAsyncHandler<SClaimRushPassRewards> collectResult { get; private set; }

        /// <summary>
        /// 是否已经付费
        /// </summary>
        public bool IsPaid { get; private set; }

        /// <summary>
        /// 收集进度  达成但不一定收集了
        /// </summary>  
        public int CollectSchedule { get; private set; }

        public bool CollectOver
        {
            get
            {
                if (config==null || userData==null)
                {
                    return true;
                }
                if (userData.RushPassPopUpInfo.Steps[userData.RushPassPopUpInfo.Steps.Count - 1].RewardPaid.Status ==
                    2)
                {
                    return true;
                }
                return false;
            }
        }

        /// <summary>
        /// 商品相关的数据
        /// </summary>
        /// <value></value>
        public ShopItemConfig ShopItemConfig
        {
            get { return userData.RushPassPopUpInfo.PayItem; }
        }

        
        protected override bool IsExpired()
        {
            if (config == null)
            {
                return true;
            }
            return GetCountDown() <= 0;
        }

        public override bool IsUnlockState()
        {
            if (config == null)
            {
                return false;
            }

            var userController = Client.Get<UserController>();
            var level = userController.GetUserLevel();
            return (long) level >= config.LevelLimited;
        }

        public override void OnRefreshUserData(ActivityData inActivityData)
        {
            if (inActivityData != null)
            {
                config = RushPassActivityConfigPB.Parser.ParseFrom(inActivityData.ActivityConfig.Data);
                userData = RushPassActivityDataPB.Parser.ParseFrom(inActivityData.ActivityUserData.Data);
                OnUpdateCountDown((ulong) config.EndCountDown);
                DealCollectSchedule();
                IsPaid = userData.RushPassPopUpInfo.PaymentHasPaid;

                base.OnRefreshUserData(inActivityData);
            }
            else
            {
                config = null;
                userData = null;
            }
        }

        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();
            SubscribeEvent<EventSceneSwitchEnd>(OnSceneSwitchEnd, HandlerPriorityWhenEnterLobby.ActivityLogic);
        }

        private void OnSceneSwitchEnd( Action handleEndCallback,
            EventSceneSwitchEnd eventSceneSwitchEnd,
            IEventHandlerScheduler scheduler)
        {

            if (eventSceneSwitchEnd.currentSceneType == SceneType.TYPE_LOBBY &&
                eventSceneSwitchEnd.lastSceneType == SceneType.TYPE_MACHINE)
            {
                OnEnterLobby();
            }
            
            handleEndCallback.Invoke();

        }

        public override void OnEnterLobby()
        {
            if (IsValid() == false) return;
            if (userData == null) return;
            var levelRushEnabled = Client.Get<LevelRushController>().IsLevelRushEnabled();
            if (!levelRushEnabled&& !IsHaveCanCollect()) return;
            if (CollectOver)
                return;
            
            EventBus.Dispatch(new EventEnqueuePopup(new PopupArgs(typeof(RushPassPopup))));
            base.OnEnterLobby();
        }


        public RepeatedField<Step> GetRewardInfos()
        {
            if (userData == null)
            {
                return null;
            }

            return userData.RushPassPopUpInfo.Steps;
        }


        private void DealCollectSchedule()
        {
            int schedule = -1;
            var step = userData.RushPassPopUpInfo.Steps.array;
            for (int i = 0; i < step.Length; i++)
            {
                var info = step[i];
                if (info != null && info.RewardFree.Status == 2)
                {
                    schedule = i;
                }
            }
            CollectSchedule = schedule;
        }

        /// <summary>
        /// 检查是否有可以领取的
        /// </summary>
        /// <returns></returns>
        public bool IsHaveCanCollect()
        {
            if (userData == null)
            {
                return false;
            }

            var steps = GetRewardInfos();
            for (int i = 0; i < steps.Count; i++)
            {
                if (steps[i] != null)
                {
                    if (steps[i].RewardFree.Status == 1)
                    {
                        return true;
                    }

                    if (IsPaid && steps[i].RewardPaid.Status == 1)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// 是否有免费的可以领取 
        /// </summary>
        /// <returns></returns>
        public bool FreeIsHaveCanCollect()
        {
            if (userData == null)
            {
                return false;
            }

            var steps = GetRewardInfos();
            for (int i = 0; i < steps.Count; i++)
            {
                if (steps[i].RewardFree.Status == 1)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 免费的可以领的话 第几个可以领取
        /// </summary>
        /// <returns></returns>
        public int GetFreeCanCollectDay()
        {
            if (userData == null)
            {
                return -1;
            }

            var steps = GetRewardInfos();
            for (int i = 0; i < steps.Count; i++)
            {
                if (steps[i] != null && steps[i].RewardFree.Status == 1)
                {
                    return   i;
                }
            }

            return -1;
        }

        /// <summary>
        /// 付费的可以领取的话 是那几天可以领取
        /// </summary>
        /// <returns></returns>
        public List<int> GetPaidCanCollectDays()
        {
            var list = new List<int>();
            if (userData == null)
            {
                return list;
            }

            var steps = GetRewardInfos();
            for (int i = 0; i < steps.Count; i++)
            {
                if (steps[i] != null && steps[i].RewardPaid.Status == 1)
                {
                    int index = i;
                    list.Add(index);
                }
            }

            return list;
        }




        /// <summary>
        /// 获取领取奖励的物品
        /// </summary>
        /// <returns></returns>
        public RepeatedField<Reward> GetCanCollectReward()
        {
            var rewards = new RepeatedField<Reward>();
            var steps = GetRewardInfos();
            
            for (int i = 0; i < steps.Count; i++)
            {
                if (steps[i] != null)
                {
                    var rewardFree = steps[i].RewardFree;
                    var rewardPaid = steps[i].RewardPaid;
                    if (rewardFree != null && rewardFree.Status == 1)
                    {
                        rewards.Add(rewardFree.Reward);
                    }

                    if (IsPaid && rewardPaid != null && rewardPaid.Status == 1)
                    {
                       rewards.Add(rewardPaid.Reward);
                    }
                }
            }

            return rewards;
        }


        /// <summary>
        /// 第n天前的付费的奖励  包含n天
        /// </summary>
        /// <param name="day"></param>
        /// <returns></returns>
        public RepeatedField<Item> GetPaidItems(int day)
        {
            var items = new RepeatedField<Item>();
            for (int i = 0; i <= day; i++)
            {
                var rewardPaid = userData.RushPassPopUpInfo.Steps.array[i].RewardPaid.Reward;
                for (int j = 0; j < rewardPaid.Items.array.Length; j++)
                {
                    if (rewardPaid.Items.array[j] != null)
                    {
                        items.Add(rewardPaid.Items.array[j]);
                    }
                }
            }

            return items;
        }

        public RepeatedField<Reward> GetPaidRewards(int day){
            var rewards = new RepeatedField<Reward>();
            for (int i = 0; i <= day; i++)
            {
                var rewardPaid = userData.RushPassPopUpInfo.Steps.array[i].RewardPaid.Reward;
                rewards.Add(rewardPaid);
            }
            return rewards;
        }


        /// <summary>
        /// 购买成功
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        public async Task PurchaseSuccess()
        { 
            BiManagerGameModule.Instance.SendGameEvent(BiEventFortuneX.Types.GameEventType.GameEventRushpassPaysuccess,("BuyIndex",PayBiInfo));
            IsPaid = true;
            await GetRushPassData();
        }

        /// <summary>
        /// 领取奖励
        /// </summary>
        /// <returns></returns>
        public async Task<bool> Collect()
        {
            collectResult = null;
            var cClaimRushPassRewards = new CClaimRushPassRewards();
            cClaimRushPassRewards.ActivityId = ActivityID;
            var info = await APIManagerGameModule.Instance.SendAsync<CClaimRushPassRewards, SClaimRushPassRewards>(cClaimRushPassRewards);
            if (info != null && info.ErrorCode == 0 && info.Response != null)
            {
                collectResult = info;
                await GetRushPassData();
                var biInfo = (CollectSchedule + 1).ToString();
                BiManagerGameModule.Instance.SendGameEvent(BiEventFortuneX.Types.GameEventType.GameEventRushpassWin,("collectIndex",biInfo));
                EventBus.Dispatch(new EventUserProfileUpdate(info.Response.UserProfile));
                return true;
            }
            return false;
        }

      

        /// <summary>
        ///  重新拉取服务器的数据
        /// </summary>
        public async Task GetRushPassData()
        {
            await RequestCGetActivityUserDataAsync();
        }
        

        /// <summary>
        /// 是否有表演
        /// </summary>
        /// <returns></returns>
        public async Task<bool> HasPerform()
        {
            await GetRushPassData();
            bool isHaveCanCollect = IsHaveCanCollect();
            return isHaveCanCollect;
        }

        /// <summary>
        /// 从level  rush 出来的表演
        /// </summary>
        public void ShowSpinRoundPerform()
        {
            EventBus.Dispatch(new EventShowPopup(new PopupArgs(typeof(RushPassPopup), PerformCategory.LevelRush)
           ));
        }


        public string PayBiInfo
        {
            get;
            private set;
        }

        public void SetPayBiInfo(string id)
        {
            PayBiInfo = id;
        }
    }
}

