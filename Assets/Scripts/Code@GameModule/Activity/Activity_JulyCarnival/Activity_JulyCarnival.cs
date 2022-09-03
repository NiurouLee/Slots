using System;
using System.Threading.Tasks;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Network.API.ILProtocol;

namespace GameModule
{
    public class Activity_JulyCarnival : ActivityBase
    {
        private IndependenceDayActivityConfigPB _configPb;

        private IndependenceDayActivityDataPB _dataPb;

        private SGetIndependenceDayMainPageInfo _pageInfo;

        private bool showEnterLobbyAd = false;

        public int itemSource { get; set; } = -1;

        public Activity_JulyCarnival() : base(ActivityType.JulyCarnival)
        {
        }

        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();
            SubscribeEvent<EventJulyCarnivalCollectItem>(OnEventIndependenceDayCollectItem);
            SubscribeEvent<EventSetActivityIconInDailyMission>(OnEventSetActivityIconInDailyMission);
            SubscribeEvent<EventLevelChanged>(OnEventLevelChanged);
        }

        private async void OnEventLevelChanged(EventLevelChanged evt)
        {
            if (evt.levelUpInfo.Level != _configPb.LevelLimited)
                return;

            var pageInfo = await GetIndependenceDayMainPageInfoData();
            if (pageInfo != null)
            {
                _pageInfo = pageInfo;
            }
        }

        private async void OnEventSetActivityIconInDailyMission(EventSetActivityIconInDailyMission evt)
        {
            if (!IsValid())
                return;
            var item = XItemUtility.GetItem(evt.mission.Items, Item.Types.Type.IndependenceDayActivityPoint);
            if (item == null)
                return;
            if (_pageInfo == null)
                return;
            if (_pageInfo.Step >= _pageInfo.StepMax)
                return;

            var view = await evt.parentView.AddChild<JulyCarnivalDailyMissionWidgetView>();
            view.SetUpContent(item, evt.missionIndex);
            view.transform.SetParent(evt.parentTransform, false);
        }

        private void OnEventIndependenceDayCollectItem(EventJulyCarnivalCollectItem obj)
        {
            if (!IsValid())
                return;
            // 这个参数用来是否做动画
            EventBus.Dispatch(new EventShowPopup(new PopupArgs(typeof(JulyCarnivalMainPopup), true, "3")));
        }

        protected override void OnExpire()
        {
            if (inRequest)
            {
                expiredAction = () =>
                {
                    base.OnExpire();
                };
                return;
            }

            base.OnExpire();
        }

        protected override bool IsExpired()
        {
            if (_configPb == null)
                return true;
            return GetCountDown() <= 0;
        }

        public override float GetCountDown()
        {
            return XUtility.GetLeftTime((ulong)_configPb.EndTimestamp * 1000) - 60;
        }

        public override bool IsUnlockState()
        {
            if (_configPb == null)
                return false;

            var level = Client.Get<UserController>().GetUserLevel();
            return level >= (ulong)_configPb.LevelLimited;
        }

        public override void OnRefreshUserData(SGetActivityUserData.Types.ActivityData inActivityData)
        {
            _configPb = IndependenceDayActivityConfigPB.Parser.ParseFrom(inActivityData.ActivityConfig.Data);
            _dataPb = IndependenceDayActivityDataPB.Parser.ParseFrom(inActivityData.ActivityUserData.Data);

            base.OnRefreshUserData(inActivityData);
            OnUpdateCountDown((ulong)XUtility.GetLeftTime((ulong)_configPb.EndTimestamp * 1000));
        }

        public override async void OnActivityOpen()
        {
            var pageInfo = await GetIndependenceDayMainPageInfoData();
            _pageInfo = pageInfo;
            var date = Client.Storage.GetItem("IndependenceDayLobbyAd", 0);
            var dateTimeOffset =
                DateTimeOffset.FromUnixTimeMilliseconds((long) APIManager.Instance.GetServerTime());
            var weekDay = (int) dateTimeOffset.Date.DayOfWeek;
            showEnterLobbyAd = date == weekDay;
            base.OnActivityOpen();
        }

        public override void OnEnterLobby()
        {
            if (!IsValid())
                return;

            if (showEnterLobbyAd)
            {
                return;
            }

            var dateTimeOffset =
                DateTimeOffset.FromUnixTimeMilliseconds((long) APIManager.Instance.GetServerTime());
            var weekDay = (int) dateTimeOffset.Date.DayOfWeek;
            Client.Storage.SetItem("IndependenceDayLobbyAd", weekDay);

            showEnterLobbyAd = true;
            // 打开广告页面
            EventBus.Dispatch(new EventShowPopup(new PopupArgs(typeof(JulyCarnivalEnterLobbyPopup))));
        }

        public override void OnBannerJump(JumpInfo jumpInfo)
        {
            EventBus.Dispatch(new EventShowPopup(new PopupArgs(typeof(JulyCarnivalMainPopup), false, "2")));
        }

        public IndependenceDayActivityDataPB GetDataPb()
        {
            return _dataPb;
        }

        public async Task<SGetIndependenceDayMainPageInfo> GetIndependenceDayMainPageInfoData()
        {
            CGetIndependenceDayMainPageInfo cGetIndependenceDay = new CGetIndependenceDayMainPageInfo();
            cGetIndependenceDay.ActivityId = ActivityID;

            var sGetIndependenceDayMainPageInfo =
                await APIManagerGameModule.Instance.SendAsync<CGetIndependenceDayMainPageInfo, SGetIndependenceDayMainPageInfo>(
                    cGetIndependenceDay);

            if (sGetIndependenceDayMainPageInfo.ErrorCode == 0)
            {
                if (sGetIndependenceDayMainPageInfo.Response.Step >= sGetIndependenceDayMainPageInfo.Response.StepMax)
                {
                    EventBus.Dispatch(new EventJulyCarnivalActivityFinish());
                }
                return sGetIndependenceDayMainPageInfo.Response;
            }
            else
                EventBus.Dispatch(new EventOnUnExceptedServerError(sGetIndependenceDayMainPageInfo.ErrorInfo));

            return null;
        }
        
        public async Task<SCollectIndependenceDayRewards> SendCollect()
        {
            SetActivityState(true);
            CCollectIndependenceDayRewards cCollect = new CCollectIndependenceDayRewards();
            cCollect.ActivityId = ActivityID;

            var sCollect =
                await APIManagerGameModule.Instance.SendAsync<CCollectIndependenceDayRewards, SCollectIndependenceDayRewards>(
                    cCollect);

            if (sCollect.ErrorCode == 0)
            {
                BiManagerGameModule.Instance.SendGameEvent(
                    BiEventFortuneX.Types.GameEventType.GameEventFourthofjulyCollect);
                EventBus.Dispatch(new EventUserProfileUpdate(sCollect.Response.UserProfile));
                return sCollect.Response;
            }
            else
                EventBus.Dispatch(new EventOnUnExceptedServerError(sCollect.ErrorInfo));

            return null;
        }
        
        public SGetIndependenceDayMainPageInfo GetIndependenceDayMainPageInfo()
        {
            return _pageInfo;
        }

        public void SetIndependenceDayMainPageInfo(SGetIndependenceDayMainPageInfo pageInfo)
        {
            _pageInfo = pageInfo;
        }

    }
}