using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DragonU3DSDK.Network.API.ILProtocol;
using DragonU3DSDK.Network.API.Protocol;
using static DragonU3DSDK.Network.API.ILProtocol.SGetActivityUserData.Types;
using static DragonU3DSDK.Network.API.ILProtocol.SGetValentineMainPageInfo.Types;

using BiEventFortuneX = DragonU3DSDK.Network.API.ILProtocol.BiEventFortuneX;

namespace GameModule
{
    public class Activity_ValentinesDay : ActivityBase
    {
        public static readonly int[] steps = { 8, 15, 25, 35, 40, 50 };

        public static readonly Dictionary<uint, int> stepToIndex = new Dictionary<uint, int>{
            {8,0},
            {15,1},
            {25,2},
            {35,3},
            {40,4},
            {50,5}
        };

        public ValenTineActivityConfigPB config { get; private set; }
        public ValentineActivityDataPB userData { get; private set; }
 
        public SGetValentineMainPageInfo sGetValentineMainPageInfo { get; private set; }

        public SCollectValentineRewards sCollectValentineRewards { get; private set; }

        public ValentineStepReward[] sortedRewards { get; private set; }
        public ShopItemConfig shopItemConfig { get; private set; }
        public Item[] availableFreeItems { get; private set; }
        public Item[] availableSpecialItems { get; private set; }

        private List<uint> _availableFreeRewardSteps = new List<uint>();
        private List<uint> _availableSpecialRewardIndexes = new List<uint>();

        public bool purchased { get; private set; }

        public int lastStep { get; set; } = -1;

        public int purchaseSource { get; set; } = -1;
        public int itemSource { get; set; } = -1;

        private bool _hasShown = false;

        public bool collectFinish
        {
            get
            {
                if (sGetValentineMainPageInfo == null) { return true; }
                var stepMax = sGetValentineMainPageInfo.StepMax;
                return lastStep >= stepMax;
            }
        }

        public Activity_ValentinesDay()
            : base(ActivityType.Valentine2022)
        {
            
        }

        protected override bool IsExpired()
        {
            if (config == null) { return true; }
            return XUtility.GetLeftTime((ulong) config.EndTimestamp * 1000) <= 0;
        }

        public override bool IsUnlockState()
        {
            if (config == null || userData == null) { return false; }
            var userController = Client.Get<UserController>();
            var level = userController.GetUserLevel();
            return (long)level >= config.LevelLimited;
        }

        public bool hasRewardToCollect
        {
            get
            {
                if (availableFreeItems != null && availableFreeItems.Length > 0) { return true; }
                if (purchased == true && availableSpecialItems != null && availableSpecialItems.Length > 0) { return true; }
                return false;
            }
        }

        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();
            SubscribeEvent<Event_Activity_Valentine2022_CollectItem>(OnEvent_Activity_Valentine2022_CollectItem);
            SubscribeEvent<Event_Activity_Valentine2022_PurchaseFinish>(OnEvent_Activity_Valentine2022_PurchaseFinish);
            SubscribeEvent<EventSetActivityIconInDailyMission>(OnEventSetActivityIconInDailyMission);
        }

        private async void OnEventSetActivityIconInDailyMission(EventSetActivityIconInDailyMission evt)
        {
            var item = XItemUtility.GetItem(evt.mission.Items, Item.Types.Type.ValentineActivityPoint);
            if (item == null)
                return;
            if (sGetValentineMainPageInfo == null)
                return;
            if (sGetValentineMainPageInfo.Step >= 50)
                return;
            var view = await evt.parentView.AddChild<ValentinesDayDailyMissionWidgetView>();
            view.SetUpContent(item);
            view.transform.SetParent(evt.parentTransform, false);
        }

        public async Task<SGetValentineMainPageInfo> PrepareMainPageData()
        {
            await SendCGetValentineMainPageInfo();
            return sGetValentineMainPageInfo;
        }

        private async void OnEvent_Activity_Valentine2022_PurchaseFinish(Event_Activity_Valentine2022_PurchaseFinish obj)
        {
            await PrepareMainPageData();
            int step = 0;
            if (sGetValentineMainPageInfo != null) { step = (int)sGetValentineMainPageInfo.Step; }
            BiManagerGameModule.Instance.SendGameEvent(BiEventFortuneX.Types.GameEventType.GameEventValentinesdayPassunlock, ("source", $"{purchaseSource}"), ("step", $"{step}"));
        }

        private async void OnEvent_Activity_Valentine2022_CollectItem(Event_Activity_Valentine2022_CollectItem obj)
        {
            await PrepareMainPageData();
            if (collectFinish) { return; }
            await PopupStack.ShowPopup<UIActivity_ValentinesDay_MapPopup>();
            BiManagerGameModule.Instance.SendGameEvent(BiEventFortuneX.Types.GameEventType.GameEventValentinesdayEnter, ("source", "3"));
        }

        public override async void OnBannerJump(JumpInfo jumpInfo)
        {
            // await PrepareMainPageData();
            await PopupStack.ShowPopup<UIActivity_ValentinesDay_MapPopup>();
            BiManagerGameModule.Instance.SendGameEvent(BiEventFortuneX.Types.GameEventType.GameEventValentinesdayEnter, ("source", "2"));
        }

        public override void OnRefreshUserData(ActivityData activityData)
        {
            purchased = false;

            if (activityData != null)
            {
                config = ValenTineActivityConfigPB.Parser.ParseFrom(activityData.ActivityConfig.Data);
                userData = ValentineActivityDataPB.Parser.ParseFrom(activityData.ActivityUserData.Data);
                EnableUpdate(1);
                if (userData != null)
                {
                    purchased = userData.PaymentAlreadyPaid;
                }
            }
            else
            {
                config = null;
                userData = null;
            }

            EventBus.Dispatch<Event_Activity_Valentine2022_ReceiveUserDate>();
        }

        private Item CloneItem(Item item)
        {
            if (item == null) { return null; }
            var type = item.Type;
            var result = new Item() { Type = type };
            switch (type)
            {
                case Item.Types.Type.Coin:
                    result.Coin = new Item.Types.Coin();
                    result.Coin.Amount = item.Coin.Amount;
                    break;
                case Item.Types.Type.Emerald:
                    result.Emerald = new Item.Types.Emerald();
                    result.Emerald.Amount = item.Emerald.Amount;
                    break;
                case Item.Types.Type.VipPoints:
                    result.VipPoints = new Item.Types.VipPoints();
                    result.VipPoints.Amount = item.VipPoints.Amount;
                    break;
                case Item.Types.Type.ShopGiftBox:
                    result.ShopGiftBox = new Item.Types.ShopGiftBox();
                    result.ShopGiftBox.Amount = item.ShopGiftBox.Amount;
                    break;
                case Item.Types.Type.SuperWheel:
                    result.SuperWheel = new Item.Types.SuperWheel();
                    result.SuperWheel.Amount = item.SuperWheel.Amount;
                    break;
                case Item.Types.Type.DoubleExp:
                    result.DoubleExp = new Item.Types.DoubleExp();
                    result.DoubleExp.Amount = item.DoubleExp.Amount;
                    break;
                case Item.Types.Type.LevelUpBurst:
                    result.LevelUpBurst = new Item.Types.LevelUpBurst();
                    result.LevelUpBurst.Amount = item.LevelUpBurst.Amount;
                    break;
            }
            return result;
        }

        private void MergeItem(Item mergeTo, Item target)
        {
            if (mergeTo == null || target == null) { return; }
            if (mergeTo.Type != target.Type) { return; }
            var type = mergeTo.Type;
            switch (type)
            {
                case Item.Types.Type.Coin:
                    mergeTo.Coin.Amount += target.Coin.Amount;
                    break;
                case Item.Types.Type.Emerald:
                    mergeTo.Emerald.Amount += target.Emerald.Amount;
                    break;
                case Item.Types.Type.VipPoints:
                    mergeTo.VipPoints.Amount += target.VipPoints.Amount;
                    break;
                case Item.Types.Type.ShopGiftBox:
                    mergeTo.ShopGiftBox.Amount += target.ShopGiftBox.Amount;
                    break;
                case Item.Types.Type.SuperWheel:
                    mergeTo.SuperWheel.Amount += target.SuperWheel.Amount;
                    break;
                case Item.Types.Type.DoubleExp:
                    mergeTo.DoubleExp.Amount += target.DoubleExp.Amount;
                    break;
                case Item.Types.Type.LevelUpBurst:
                    mergeTo.LevelUpBurst.Amount += target.LevelUpBurst.Amount;
                    break;
            }
        }

        public override async void OnEnterLobby()
        {
            base.OnEnterLobby();
            if (IsValid() == false) { return; }
            if (_hasShown) { return; }
            if (userData == null) { return; }
            if (userData.PaymentAlreadyPaid && userData.StepAlreadyBiggest) { return; }
            EventBus.Dispatch(new EventEnqueuePopup(new PopupArgs(typeof(UIActivity_ValentinesDay_MainPopup)) { extraArgs = this }));
            EventBus.Dispatch(new EventEnqueueFencePopup(null));
            _hasShown = true;
            await PrepareMainPageData();
        }

        protected override async void OnExpire()
        {
            XDebug.Log($"11111111111111 activity on expire:{ServerActivityType}");
            await RequestCGetActivityUserDataAsync();
            base.OnExpire();
        }

        public async Task SendCGetValentineMainPageInfo()
        {
            sGetValentineMainPageInfo = null;
            sortedRewards = null;
            availableFreeItems = null;
            availableSpecialItems = null;
            purchased = false;
            shopItemConfig = null;
            _availableFreeRewardSteps.Clear();
            _availableSpecialRewardIndexes.Clear();

            if (config == null) { return; }
            var c = new CGetValentineMainPageInfo() { ActivityId = config.Id };
            var s = await APIManagerGameModule.Instance.SendAsync<CGetValentineMainPageInfo, SGetValentineMainPageInfo>(c);
            if (s != null && s.ErrorCode == 0 && s.Response != null)
            {
                sGetValentineMainPageInfo = s.Response;

                shopItemConfig = sGetValentineMainPageInfo.PayItem;

                purchased = sGetValentineMainPageInfo.PaymentAlreadyPaid;
                if (lastStep == -1)
                {
                    lastStep = (int)sGetValentineMainPageInfo.Step;
                }

                // sort rewards
                var rewards = sGetValentineMainPageInfo.ValentineRewards;
                if (rewards == null || rewards.count == 0) { return; }
                sortedRewards = new ValentineStepReward[rewards.count];
                rewards.CopyTo(sortedRewards, 0);

                var length = sortedRewards.Length;
                var orderly = true;
                for (int i = 0; i < length - 1; i++)
                {
                    for (int j = 0; j < length - 1 - i; j++)
                    {
                        if (sortedRewards[j].Step > sortedRewards[j + 1].Step)
                        {
                            var temp = sortedRewards[j];
                            sortedRewards[j] = sortedRewards[j + 1];
                            sortedRewards[j + 1] = temp;

                            orderly = false;
                        }
                    }
                    if (orderly) { break; }
                }

                var normalStartStep = sGetValentineMainPageInfo.NormalRewardsReceivedStep;
                var specialStartStep = sGetValentineMainPageInfo.SpecialRewardsReceivedStep;
                var step = sGetValentineMainPageInfo.Step;

                // merge items
                var freeItems = new Dictionary<Item.Types.Type, Item>();
                var specialItems = new Dictionary<Item.Types.Type, Item>();

                foreach (var reward in sortedRewards)
                {
                    if (reward == null) { continue; }

                    if (reward.Step > normalStartStep && reward.Step <= step)
                    {
                        _availableFreeRewardSteps.Add(reward.Step);

                        var freeReward = reward.NormalReward;
                        if (freeReward != null && freeReward.Items != null && freeReward.Items.count > 0)
                        {
                            foreach (var item in freeReward.Items)
                            {
                                if (item == null) { continue; }
                                var type = item.Type;
                                Item targetItem;
                                freeItems.TryGetValue(type, out targetItem);

                                if (targetItem == null)
                                {
                                    targetItem = CloneItem(item);
                                    freeItems.Add(type, targetItem);
                                }
                                else
                                {
                                    MergeItem(targetItem, item);
                                }
                            }
                        }
                    }

                    if (reward.Step > specialStartStep && reward.Step <= step)
                    {
                        _availableSpecialRewardIndexes.Add(reward.Step);

                        var specialReward = reward.SpecialReward;
                        if (specialReward != null && specialReward.Items != null && specialReward.Items.count > 0)
                        {
                            foreach (var item in specialReward.Items)
                            {
                                if (item == null) { continue; }
                                var type = item.Type;
                                Item targetItem;
                                specialItems.TryGetValue(type, out targetItem);

                                if (targetItem == null)
                                {
                                    targetItem = CloneItem(item);
                                    specialItems.Add(type, targetItem);
                                }
                                else
                                {
                                    MergeItem(targetItem, item);
                                }
                            }
                        }
                    }
                }

                if (freeItems.Count > 0)
                {
                    availableFreeItems = new Item[freeItems.Count];
                    freeItems.Values.CopyTo(availableFreeItems, 0);
                }

                if (specialItems.Count > 0)
                {
                    availableSpecialItems = new Item[specialItems.Count];
                    specialItems.Values.CopyTo(availableSpecialItems, 0);
                }

                EventBus.Dispatch<Event_Activity_Valentine2022_ReceiveMainPageInfo>();
            }
        }

        public async Task SendCCollectValentineRewards()
        {
            sCollectValentineRewards = null;
            var c = new CCollectValentineRewards() { ActivityId = config.Id };
            var s = await APIManagerGameModule.Instance.SendAsync<CCollectValentineRewards, SCollectValentineRewards>(c);
            if (s != null && s.ErrorCode == 0 && s.Response != null)
            {
                sCollectValentineRewards = s.Response;

                var sb = new StringBuilder();

                if (_availableFreeRewardSteps?.Count > 0)
                {
                    foreach (var step in _availableFreeRewardSteps)
                    {
                        int index = 0;
                        if (stepToIndex.TryGetValue(step, out index))
                        {
                            sb.Append(index);
                            sb.Append(" ");
                        }
                    }
                }

                if (_availableSpecialRewardIndexes?.Count > 0 && purchased)
                {
                    foreach (var step in _availableSpecialRewardIndexes)
                    {
                        int index = 0;
                        if (stepToIndex.TryGetValue(step, out index))
                        {
                            sb.Append(index + 6);
                            sb.Append(" ");
                        }
                    }
                }
                BiManagerGameModule.Instance.SendGameEvent(BiEventFortuneX.Types.GameEventType.GameEventValentinesdayCollect, ("reward index", $"{sb.ToString()}"));
            }
        }
    }
}