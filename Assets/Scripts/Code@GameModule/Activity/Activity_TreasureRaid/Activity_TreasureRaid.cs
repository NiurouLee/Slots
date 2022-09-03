using System.Threading.Tasks;
using DragonU3DSDK.Network.API.ILProtocol;
using DragonU3DSDK.Network.API.Protocol;
using UnityEngine;
using Google.ilruntime.Protobuf.Collections;
using System;
using System.Collections.Generic;
using DragonU3DSDK.Network.API;
using BiEventFortuneX = DragonU3DSDK.Network.API.ILProtocol.BiEventFortuneX;

namespace GameModule
{
    public class Activity_TreasureRaid : ActivityBase
    {
        public bool InActivity { set; get; }
        public bool ShowTip { set; get; }

        private MonopolyActivityConfigPB _configPb;

        private MonopolyActivityDataPB _dataPb;
        
        private MonopolyRoundInfo _roundInfo;
        private MonopolyDailyTask _dailyTask;

        private RepeatedField<MonopolyRoundSimpleInfo> _roundSimpleInfo;
        
        private float _lastBoxUpdateCountDown;

        private bool initSystemWidget = false;

        private bool showEnterLobbyAd = false;

        public bool HasSpinRewardData { get; set; }

        private SGetMonopolyPuzzleListInfo lastPuzzleListInfo;

        public Activity_TreasureRaid() 
            : base(ActivityType.TreasureRaid)
        {
            
        }

        //红点从这里获取数据
        public uint TicketCount
        {
            get
            {
                var roundId = GetCurrentRoundID();
                return BeginnersGuideStep <= 4 && roundId == 1 ? (_dataPb.TicketCount - 3) : _dataPb.TicketCount;
            }
        }

        public uint PaymentId => _dataPb.TicketPaymentId;

        public uint BeginnersGuideStep { get; private set; }

        public override void OnCreate(SGetActivitiesOpenTime.Types.ActivityTimeConfig activity, ActivityController controller)
        {
            base.OnCreate(activity, controller);
            ShowTip = false;
            initSystemWidget = false;
        }

        public override bool IsUnlockState()
        {
            if (_configPb == null)
                return false;

            var level = Client.Get<UserController>().GetUserLevel();
            return level >= (ulong)_configPb.LevelLimited;
        }

        public long GetUnlockLevel()
        {
            if (_configPb == null)
                return 30;
            return _configPb.LevelLimited;
        }

        protected override bool IsExpired()
        {
            if (_configPb == null)
                return true;
            return GetCountDown() <= 0;
        }

        public override void OnEnterLobby()
        {
            if (!IsValid())
                return;

            if (showEnterLobbyAd)
            {
                EventBus.Dispatch(new EventEnqueuePopup(new PopupArgs(typeof(TreasureRaidBoosterAdPopup))));
                return;
            }

            var dateTimeOffset =
                DateTimeOffset.FromUnixTimeMilliseconds((long) APIManager.Instance.GetServerTime());
            var weekDay = (int) dateTimeOffset.Date.DayOfWeek;
            Client.Storage.SetItem("TreasureRaidLobbyAd", weekDay);

            showEnterLobbyAd = true;
            EventBus.Dispatch(new EventEnqueuePopup(new PopupArgs(typeof(TreasureRaidEnterLobbyPopup))));
            EventBus.Dispatch(new EventEnqueueFencePopup(null));
        }

        public override void OnRefreshUserData(SGetActivityUserData.Types.ActivityData inActivityData)
        {
            _configPb = MonopolyActivityConfigPB.Parser.ParseFrom(inActivityData.ActivityConfig.Data);
            _dataPb = MonopolyActivityDataPB.Parser.ParseFrom(inActivityData.ActivityUserData.Data);

            BeginnersGuideStep = _dataPb.BeginnerGuideStep;

            OnUpdateCountDown((ulong) _configPb.EndCountDown);
            
            //EventBus.Dispatch(new EventTreasureRaidUpdateUserData());
            
            base.OnRefreshUserData(inActivityData);
        }

        protected override async void OnExpire()
        {
            if (inRequest)
            {
                expiredAction = async () =>
                {
                    await GetMonopolyLeaderboardMail();
                    base.OnExpire();
                    // 弹出一个弹窗提示
                    EventBus.Dispatch(new EventShowPopup(new PopupArgs(typeof(TreasureRaidActivityEndPopup))));
                };
                return;
            }

            await GetMonopolyLeaderboardMail();
            base.OnExpire();
            if (InActivity)
            {
                // 弹出一个弹窗提示
                EventBus.Dispatch(new EventShowPopup(new PopupArgs(typeof(TreasureRaidActivityEndPopup))));
            }
            else
            {
                EventBus.Dispatch(new EventTreasureRaidOnExpire());
            }
        }

        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();
            SubscribeEvent<EventOnCollectSystemWidget>(OnEventCollectSystemWidget, HandlerPrioritySystemCollectWidget.TreasureRaidWidget);
            SubscribeEvent<EventLevelChanged>(OnEventLevelChanged);
            SubscribeEvent<EventSceneSwitchBegin>(OnEventSceneSwitchBegin);
            SubscribeEvent<EventChangeLobbyEntranceInActivity>(OnEventChangeLobbyEntranceInActivity);
        }

        private void OnEventSceneSwitchBegin(EventSceneSwitchBegin obj)
        {
            initSystemWidget = false;
        }

        public override async Task OnEventLobbyCreated(EventOnLobbyCreated obj)
        {
            if (_dataPb == null)
                return;

            if (!IsUnlockState())
                return;

            var passView = obj.lobbyScene.GetChildView<SeasonPassLobbyEntranceView>();
            if (passView == null)
                return;

            var treasureRaidView = obj.lobbyScene.GetChildView<TreasureRaidEntranceView>();
            if (treasureRaidView != null)
                return;

            treasureRaidView = await obj.lobbyScene.AddChild<TreasureRaidEntranceView>();
            treasureRaidView.transform.SetParent(passView.transform.parent,false);
            treasureRaidView.transform.position = passView.transform.position;
            treasureRaidView.Show();
            passView.Hide();
        }

        private async void OnEventChangeLobbyEntranceInActivity(EventChangeLobbyEntranceInActivity obj)
        {
            if (_dataPb == null)
                return;

            if (!IsUnlockState())
                return;

            var passView = obj.lobbyScene.GetChildView<SeasonPassLobbyEntranceView>();
            if (passView == null)
                return;

            var treasureRaidView = obj.lobbyScene.GetChildView<TreasureRaidEntranceView>();
            if (treasureRaidView != null)
                return;

            treasureRaidView = await obj.lobbyScene.AddChild<TreasureRaidEntranceView>();
            treasureRaidView.transform.SetParent(passView.transform.parent,false);
            treasureRaidView.transform.position = passView.transform.position;
            treasureRaidView.Show();
            passView.Hide();
        }
        
        private async void OnEventLevelChanged(EventLevelChanged obj)
        {
            if (ViewManager.Instance.IsInSwitching() || !ViewManager.Instance.InMachineScene()) return;
            if (!IsUnlockState()) return;
            if (initSystemWidget) return;
            initSystemWidget = true;
            var treasureWidget = await View.CreateView<TreasureRaidWidget>();
            EventBus.Dispatch(new EventSystemWidgetNeedAttach(treasureWidget,0));
        }

        public bool GetInRequestState()
        {
            return inRequest;
        }

        public override async void OnActivityOpen()
        {
            var date = Client.Storage.GetItem("TreasureRaidLobbyAd", 0);
            var dateTimeOffset =
                DateTimeOffset.FromUnixTimeMilliseconds((long) APIManager.Instance.GetServerTime());
            var weekDay = (int) dateTimeOffset.Date.DayOfWeek;
            showEnterLobbyAd = date == weekDay;
            InitSystemWidget();
            base.OnActivityOpen();
        }

        
        private async void InitSystemWidget()
        {
            if (ViewManager.Instance.IsInSwitching() || !ViewManager.Instance.InMachineScene()) return;
            if (!IsUnlockState()) return;
            if (initSystemWidget)
                return;
            initSystemWidget = true;
            var treasureWidget = await View.CreateView<TreasureRaidWidget>();
            EventBus.Dispatch(new EventSystemWidgetNeedAttach(treasureWidget,0));
        }
        
        private async void OnEventCollectSystemWidget(Action handleEndAction, EventOnCollectSystemWidget evt, IEventHandlerScheduler eventHandlerScheduler)
        {
            if (!IsValid())
            {
                handleEndAction.Invoke();
                return;
            }

            if (!IsUnlockState())
            {
                handleEndAction.Invoke();
                return;
            }

            if (initSystemWidget)
            {
                handleEndAction.Invoke();
                return;
            }

            initSystemWidget = true;
            var treasureWidget = await View.CreateView<TreasureRaidWidget>();
            
            evt.viewController.AddSystemWidget(treasureWidget);
            handleEndAction.Invoke();
        }
        
        public override void OnBannerJump(JumpInfo jumpInfo)
        {
            BiManagerGameModule.Instance.SendGameEvent(BiEventFortuneX.Types.GameEventType.GameEventTreasureraidEnter, ("OperationId", "2"));
            if (GetCurrentRunningRoundID() == 0)
                EventBus.Dispatch(new EventShowPopup(new PopupArgs(typeof(TreasureRaidMapPopup), true, "BannerIcon")));
            else
                EventBus.Dispatch(new EventShowPopup(new PopupArgs(typeof(TreasureRaidMainPopup), true,"BannerIcon")));
        }

        public float GetBoxUpdateTime()
        {
            return _lastBoxUpdateCountDown;
        }

        /// <summary>
        /// 获取礼盒位置信息（4个，包含是否有礼盒）
        /// </summary>
        /// <returns></returns>
        public RepeatedField<MonopolyRoundInfo.Types.GiftBoxPosition> GetGiftBoxInfo()
        {
            return _roundInfo.GiftBoxPositions;
        }

        public MonopolyRoundInfo GetRoundInfo()
        {
            return _roundInfo;
        }

        public MonopolyDailyTask GetDailyTask()
        {
            return _dailyTask;
        }

        public MonopolyActivityDataPB GetDataPb()
        {
            return _dataPb;
        }

        /// <summary>
        /// 入口获取当前关卡Id，正常情况下_dataPb不会为空，这里暂时不做判断
        /// </summary>
        /// <returns></returns>
        public uint GetCurrentRunningRoundID()
        {
            return _dataPb.RunningRoundId;
        }

        public override void OnSpinSystemContentUpdate(EventSpinSystemContentUpdate evt)
        {
            var monopolyData = GetSystemData<MonopolyEnergyInfoWhenSpin>(evt.systemContent, "MonopolyEnergyInfoWhenSpin");
            if (monopolyData != null)
            {
                BiManagerGameModule.Instance.SendGameEvent(BiEventFortuneX.Types.GameEventType.GameEventTreasureraidExpup);
                _dataPb.EnergyInfoWhenSpin = monopolyData;
                if (monopolyData.Reward != null)
                {
                    _dataPb.TicketCount = monopolyData.TicketCount;
                    HasSpinRewardData = true;
                    BiManagerGameModule.Instance.SendGameEvent(BiEventFortuneX.Types.GameEventType.GameEventTreasureraidGettickets);
                }
                // 发送刷新Widget事件
                EventBus.Dispatch(new EventActivityServerDataUpdated(ActivityType.TreasureRaid, ActivityID,true, monopolyData));
            }
        }

        /// <summary>
        /// 给获得reward提供的接口
        /// </summary>
        /// <param name="addCount"></param>
        public void AddTicketCount(uint addCount)
        {
            _dataPb.TicketCount += addCount;
            EventBus.Dispatch(new EventActivityServerDataUpdated(ActivityType.TreasureRaid, ActivityID));
        }
        public void AddPortalCount(uint addCount)
        {
            if (_roundInfo == null) return;
            _roundInfo.MonopolyPortal.Amount += addCount;
        }

        public void UpdateMonopolyBuff(BuffMonopolyBooster buffMonopolyBooster)
        {
            if (_roundInfo == null) return;
            _roundInfo.MonopolyBuffMoreDamage = buffMonopolyBooster.MonopolyBuffMoreDamage;
            _roundInfo.MonopolyBuffMoreTicket = buffMonopolyBooster.MonopolyBuffMoreTicket;
        }

        public uint GetCurrentRoundID()
        {
            if (_dataPb.RunningRoundId != 0)
            {
                return _dataPb.RunningRoundId > 6 ? _dataPb.RunningRoundId % 6 : _dataPb.RunningRoundId;
            }
            if (_roundInfo != null)
            {
                return _roundInfo.SimpleInfo.RoundId > 6
                    ? _roundInfo.SimpleInfo.RoundId % 6
                    : _roundInfo.SimpleInfo.RoundId;
            }
            if (_roundSimpleInfo != null)
            {
                foreach (var info in _roundSimpleInfo)
                {
                    if (info.IsCurrentRound)
                    {
                        return info.RoundId > 6 ? info.RoundId % 6 : info.RoundId;
                    }
                }
            }
            //默认1
            return 1;
        }

        public uint GetServerNeedCurrentRoundID()
        {
            if (_dataPb.RunningRoundId != 0)
            {
                return _dataPb.RunningRoundId;
            }
            if (_roundInfo != null && _roundInfo.SimpleInfo.RoundStatus != MonopolyRoundSimpleInfo.Types.RoundStatus.Finished)
            {
                return _roundInfo.SimpleInfo.RoundId;
            }
            if (_roundSimpleInfo != null)
            {
                foreach (var info in _roundSimpleInfo)
                {
                    if (info.IsCurrentRound)
                    {
                        return info.RoundId;
                    }
                }
            }
            //默认1
            return 1;
        }

        /// <summary>
        /// 进入地图页面需要的数据
        /// </summary>
        /// <returns></returns>
        public async Task<SGetMonopolyRoundListInfo> GetRoundListInfo()
        {
            CGetMonopolyRoundListInfo cGetMonopolyRoundListInfo = new CGetMonopolyRoundListInfo();
            cGetMonopolyRoundListInfo.ActivityId = ActivityID;

            var sGetMonopolyRoundListInfo =
                await APIManagerGameModule.Instance.SendAsync<CGetMonopolyRoundListInfo, SGetMonopolyRoundListInfo>(
                    cGetMonopolyRoundListInfo);

            if (sGetMonopolyRoundListInfo.ErrorCode == 0)
            {
                _dataPb.TicketCount = sGetMonopolyRoundListInfo.Response.TicketCount;
                OnUpdateCountDown((ulong) sGetMonopolyRoundListInfo.Response.EndCountDown);
                _roundSimpleInfo = sGetMonopolyRoundListInfo.Response.RoundList;
                _dailyTask = sGetMonopolyRoundListInfo.Response.DailyTask;
                return sGetMonopolyRoundListInfo.Response;
            }
            else
                EventBus.Dispatch(new EventOnUnExceptedServerError(sGetMonopolyRoundListInfo.ErrorInfo));

            return null;
        }
        
        /// <summary>
        /// 进入关卡页面需要的数据
        /// </summary>
        /// <returns></returns>
        public async Task<SGetMonopolyRoundInfo> GetMonopolyRoundInfo()
        {
            CGetMonopolyRoundInfo cGetMonopolyRoundInfo = new CGetMonopolyRoundInfo();
            cGetMonopolyRoundInfo.ActivityId = ActivityID;
            cGetMonopolyRoundInfo.RoundId = GetServerNeedCurrentRoundID();
            cGetMonopolyRoundInfo.IsStart = _dataPb.RunningRoundId == 0;
            XDebug.Log("---------cGetMonopolyRoundInfo.IsStart:" + cGetMonopolyRoundInfo.IsStart + "-----------RoundId:" + cGetMonopolyRoundInfo.RoundId);
            if (cGetMonopolyRoundInfo.IsStart)
            {
                var level = cGetMonopolyRoundInfo.RoundId > 6
                    ? cGetMonopolyRoundInfo.RoundId % 6
                    : cGetMonopolyRoundInfo.RoundId;
                level = level == 0 ? 6 : level;
                BiManagerGameModule.Instance.SendGameEvent(BiEventFortuneX.Types.GameEventType.GameEventTreasureraidLevelprize, ("OperationId", level.ToString()));
            }
            var sGetMonopolyRoundInfo =
                await APIManagerGameModule.Instance.SendAsync<CGetMonopolyRoundInfo, SGetMonopolyRoundInfo>(
                    cGetMonopolyRoundInfo);

            if (sGetMonopolyRoundInfo.ErrorCode == 0)
            {
                _roundInfo = sGetMonopolyRoundInfo.Response.MonopolyRoundInfo;
                _lastBoxUpdateCountDown = Time.realtimeSinceStartup;
                _dataPb.TicketCount = _roundInfo.TicketCount;
                _dataPb.RunningRoundId = _roundInfo.SimpleInfo.RoundId;
                _dailyTask = sGetMonopolyRoundInfo.Response.DailyTask;
                return sGetMonopolyRoundInfo.Response;
            }
            else
                EventBus.Dispatch(new EventOnUnExceptedServerError(sGetMonopolyRoundInfo.ErrorInfo));

            return null;
        }

        public async Task<SMonopolySpin> MonopolySpin()
        {
            SetActivityState(true);
            CMonopolySpin cMonopolySpin = new CMonopolySpin();
            cMonopolySpin.ActivityId = ActivityID;
            cMonopolySpin.RoundId = GetServerNeedCurrentRoundID();

            XDebug.LogWarning($"当前spinActivityId:{ActivityID}, RoundId:{GetServerNeedCurrentRoundID()}");
            var sMonopolySpin =
                await APIManagerGameModule.Instance.SendAsync<CMonopolySpin, SMonopolySpin>(cMonopolySpin);
            if (sMonopolySpin.ErrorCode == 0)
            {
                _roundInfo = sMonopolySpin.Response.MonopolyRoundInfo;
                _dailyTask = sMonopolySpin.Response.DailyTask;

                _lastBoxUpdateCountDown = Time.realtimeSinceStartup;
                _dataPb.TicketCount = _roundInfo.TicketCount;
                if (sMonopolySpin.Response.RoundCompleteRewards != null && sMonopolySpin.Response.RoundCompleteRewards.Count > 0)
                {
                    _dataPb.RunningRoundId = 0;
                }

                if (sMonopolySpin.Response.RoundListCompleteRewards != null && sMonopolySpin.Response.RoundListCompleteRewards.Count > 0)
                {
                    BiManagerGameModule.Instance.SendGameEvent(
                        BiEventFortuneX.Types.GameEventType.GameEventTreasureraidRoundprize,
                        ("OperationId", $"{sMonopolySpin.Response.MonopolyRoundInfo.SimpleInfo.RoundNum/6}"));
                }
                // 此次spin可能获取的GiftBox
                // sMonopolySpin.Response.GiftBoxReward
                EventBus.Dispatch(new EventUserProfileUpdate(sMonopolySpin.Response.UserProfile));
                return sMonopolySpin.Response;
            }
            else
                EventBus.Dispatch(new EventOnUnExceptedServerError(sMonopolySpin.ErrorInfo));

            return null;
        }

        public async Task<SMonopolyShootAgain> MonopolyShootAgain(Action<bool, SMonopolyShootAgain> callback, string key = "")
        {
            CMonopolyShootAgain cMonopolyShootAgain = new CMonopolyShootAgain();
            cMonopolyShootAgain.ActivityId = ActivityID;
            cMonopolyShootAgain.RoundId = GetServerNeedCurrentRoundID();
            cMonopolyShootAgain.FreeKey = key;

            var sMonopolyShootAgain =
                await APIManagerGameModule.Instance.SendAsync<CMonopolyShootAgain, SMonopolyShootAgain>(cMonopolyShootAgain);
            if (sMonopolyShootAgain.ErrorCode == 0)
            {
                _roundInfo = sMonopolyShootAgain.Response.MonopolyRoundInfo;
                _dailyTask = sMonopolyShootAgain.Response.DailyTask;

                if (sMonopolyShootAgain.Response.RoundCompleteRewards != null && sMonopolyShootAgain.Response.RoundCompleteRewards.Count > 0)
                {
                    _dataPb.RunningRoundId = 0;
                }
                EventBus.Dispatch(new EventUserProfileUpdate(sMonopolyShootAgain.Response.UserProfile));
                callback.Invoke(true, sMonopolyShootAgain.Response);
                return sMonopolyShootAgain.Response;
            }
            else
                EventBus.Dispatch(new EventOnUnExceptedServerError(sMonopolyShootAgain.ErrorInfo));

            callback.Invoke(false, null);
            return null;
        }

        public async Task<SMonopolyTicketPaymentInfo> MonopolyTicketPaymentInfo()
        {
            CMonopolyTicketPaymentInfo cMonopolyTicketPaymentInfo = new CMonopolyTicketPaymentInfo();
            cMonopolyTicketPaymentInfo.ActivityId = ActivityID;
            var sMonopolyTicketPaymentInfo =
                await APIManagerGameModule.Instance.SendAsync<CMonopolyTicketPaymentInfo, SMonopolyTicketPaymentInfo>(
                    cMonopolyTicketPaymentInfo);
            if (sMonopolyTicketPaymentInfo.ErrorCode == 0)
            {
                return sMonopolyTicketPaymentInfo.Response;
            }
            else
                XDebug.LogError(sMonopolyTicketPaymentInfo.ErrorInfo);
                // EventBus.Dispatch(new EventOnUnExceptedServerError(sMonopolyTicketPaymentInfo.ErrorInfo));

            return null;
        }
        
        public async Task<SMonopolyOpenGiftBox> MonopolyOpenGiftBox(MonopolyGiftBox giftBox)
        {
            SetActivityState(true);
            CMonopolyOpenGiftBox cMonopolyOpenGiftBox = new CMonopolyOpenGiftBox();
            cMonopolyOpenGiftBox.ActivityId = ActivityID;
            cMonopolyOpenGiftBox.UinonSign = giftBox.UinonSign;

            var sMonopolyOpenGiftBox =
                await APIManagerGameModule.Instance.SendAsync<CMonopolyOpenGiftBox, SMonopolyOpenGiftBox>(
                    cMonopolyOpenGiftBox);
            if (sMonopolyOpenGiftBox.ErrorCode == 0)
            {
                _roundInfo = sMonopolyOpenGiftBox.Response.MonopolyRoundInfo;
                _dailyTask = sMonopolyOpenGiftBox.Response.DailyTask;

                _lastBoxUpdateCountDown = Time.realtimeSinceStartup;
                EventBus.Dispatch(new EventUserProfileUpdate(sMonopolyOpenGiftBox.Response.UserProfile));
                return sMonopolyOpenGiftBox.Response;
            }
            else
                EventBus.Dispatch(new EventOnUnExceptedServerError(sMonopolyOpenGiftBox.ErrorInfo));

            return null;
        }

        public async void IncreaseGuideStep(Action<SMonopolyChangeBeginnerGuideStep> finishCallback)
        {
            SetActivityState(true);
            BeginnersGuideStep++;
            var send = new CMonopolyChangeBeginnerGuideStep();
            send.ActivityId = ActivityID;
            send.BeginnerGuideStep = BeginnersGuideStep;
            
            var receive =
                await APIManagerGameModule.Instance.SendAsync<CMonopolyChangeBeginnerGuideStep, SMonopolyChangeBeginnerGuideStep>(send);

            if (receive.ErrorCode == ErrorCode.Success)
            {
                finishCallback?.Invoke(receive.Response);
                BeginnersGuideStep = receive.Response.BeginnerGuideStep;
            }
            else
            {
                finishCallback?.Invoke(null);
            }
            SetActivityState(false);
        }
        
        public async Task<SMonopolyTeleport> MonopolyTeleport(uint forwardStep)
        {
            SetActivityState(true);
            CMonopolyTeleport cMonopolyOpenGiftBox = new CMonopolyTeleport();
            cMonopolyOpenGiftBox.ActivityId = ActivityID;
            cMonopolyOpenGiftBox.ForwardStep = forwardStep;

            var sMonopolyTeleport =
                await APIManagerGameModule.Instance.SendAsync<CMonopolyTeleport, SMonopolyTeleport>(
                    cMonopolyOpenGiftBox);
            if (sMonopolyTeleport.ErrorCode == 0)
            {
                _roundInfo = sMonopolyTeleport.Response.MonopolyRoundInfo;
                _dailyTask = sMonopolyTeleport.Response.DailyTask;

                _lastBoxUpdateCountDown = Time.realtimeSinceStartup;
                
                _dataPb.TicketCount = _roundInfo.TicketCount;
                if (sMonopolyTeleport.Response.RoundCompleteRewards != null && sMonopolyTeleport.Response.RoundCompleteRewards.Count > 0)
                {
                    _dataPb.RunningRoundId = 0;
                }

                if (sMonopolyTeleport.Response.RoundListCompleteRewards != null && sMonopolyTeleport.Response.RoundListCompleteRewards.Count > 0)
                {
                    BiManagerGameModule.Instance.SendGameEvent(
                        BiEventFortuneX.Types.GameEventType.GameEventTreasureraidRoundprize,
                        ("OperationId", $"{sMonopolyTeleport.Response.MonopolyRoundInfo.SimpleInfo.RoundNum/6}"));
                }
                EventBus.Dispatch(new EventUserProfileUpdate(sMonopolyTeleport.Response.UserProfile));
                return sMonopolyTeleport.Response;
            }
            else
                EventBus.Dispatch(new EventOnUnExceptedServerError(sMonopolyTeleport.ErrorInfo));

            return null;
        }

        public async Task<SGetMonopolyDailyTaskInfo> GetMonopolyDailyTask()
        {
            CGetMonopolyDailyTaskInfo cMonopolyDailyTask = new CGetMonopolyDailyTaskInfo();
            cMonopolyDailyTask.ActivityId = ActivityID;

            var sMonopolyDailyTask =
                await APIManagerGameModule.Instance.SendAsync<CGetMonopolyDailyTaskInfo, SGetMonopolyDailyTaskInfo>(
                    cMonopolyDailyTask);
            if (sMonopolyDailyTask.ErrorCode == 0)
            {
                _dailyTask = sMonopolyDailyTask.Response.DailyTask;
                EventBus.Dispatch(new EventTreasureRaidDailyTaskRefresh());
                return sMonopolyDailyTask.Response;
            }
            else
            {
                XDebug.LogWarning(sMonopolyDailyTask.ErrorInfo);
            }

            return null;
        }
        
        public async Task<SGetMonopolyLeaderboard> GetMonopolyLeaderboard()
        {
            CGetMonopolyLeaderboard cGetMonopolyLeaderboard = new CGetMonopolyLeaderboard();
            cGetMonopolyLeaderboard.ActivityId = ActivityID;
            var sGetMonopolyLeaderboard =
                await APIManagerGameModule.Instance.SendAsync<CGetMonopolyLeaderboard, SGetMonopolyLeaderboard>(
                    cGetMonopolyLeaderboard);
            if (sGetMonopolyLeaderboard.ErrorCode == 0)
            {
                return sGetMonopolyLeaderboard.Response;
            }
            else
                EventBus.Dispatch(new EventOnUnExceptedServerError(sGetMonopolyLeaderboard.ErrorInfo));

            return null;
        }

        public bool HasLastPuzzleListInfo()
        {
            return lastPuzzleListInfo != null;
        }

        public SGetMonopolyPuzzleListInfo GetLastPuzzleListInfo()
        {
            return lastPuzzleListInfo;
        }

        public void SetLastPuzzleListInfo(SGetMonopolyPuzzleListInfo info)
        {
            lastPuzzleListInfo = info;
        }

        public async Task<SGetMonopolyPuzzleListInfo> GetMonopolyPuzzleListInfo()
        {
            CGetMonopolyPuzzleListInfo cGetMonopolyPuzzleListInfo = new CGetMonopolyPuzzleListInfo();
            cGetMonopolyPuzzleListInfo.ActivityId = ActivityID;
            var sGetMonopolyPuzzleListInfo =
                await APIManagerGameModule.Instance.SendAsync<CGetMonopolyPuzzleListInfo, SGetMonopolyPuzzleListInfo>(
                    cGetMonopolyPuzzleListInfo);
            if (sGetMonopolyPuzzleListInfo.ErrorCode == 0)
            {
                return sGetMonopolyPuzzleListInfo.Response;
            }
            else
                EventBus.Dispatch(new EventOnUnExceptedServerError(sGetMonopolyPuzzleListInfo.ErrorInfo));

            return null;
        }
        
        public async Task<SCollectMonopolyPuzzleReward> SendCollectMonopolyPuzzleReward()
        {
            CCollectMonopolyPuzzleReward cCollectMonopolyPuzzleReward = new CCollectMonopolyPuzzleReward();
            cCollectMonopolyPuzzleReward.ActivityId = ActivityID;
            var sCollectMonopolyPuzzleReward =
                await APIManagerGameModule.Instance.SendAsync<CCollectMonopolyPuzzleReward, SCollectMonopolyPuzzleReward>(
                    cCollectMonopolyPuzzleReward);
            if (sCollectMonopolyPuzzleReward.ErrorCode == 0)
            {
                EventBus.Dispatch(new EventUserProfileUpdate(sCollectMonopolyPuzzleReward.Response.UserProfile));
                return sCollectMonopolyPuzzleReward.Response;
            }
            else
                EventBus.Dispatch(new EventOnUnExceptedServerError(sCollectMonopolyPuzzleReward.ErrorInfo));

            return null;
        }
        
        
        private async Task<SGetMonopolyLeaderboardReward> GetMonopolyLeaderboardMail()
        {
            CGetMonopolyLeaderboardReward cGetMonopolyLeaderboardReward = new CGetMonopolyLeaderboardReward();
            cGetMonopolyLeaderboardReward.ActivityId = ActivityID;
            var sGetMonopolyLeaderboardReward =
                await APIManagerGameModule.Instance.SendAsync<CGetMonopolyLeaderboardReward, SGetMonopolyLeaderboardReward>(
                    cGetMonopolyLeaderboardReward);
            if (sGetMonopolyLeaderboardReward.ErrorCode == 0)
            {
                // EventBus.Dispatch(new EventUserProfileUpdate(sGetMonopolyLeaderboardReward.Response.UserProfile));
                return sGetMonopolyLeaderboardReward.Response;
            }
            else
                EventBus.Dispatch(new EventOnUnExceptedServerError(sGetMonopolyLeaderboardReward.ErrorInfo));

            return null;
        }
    }
}