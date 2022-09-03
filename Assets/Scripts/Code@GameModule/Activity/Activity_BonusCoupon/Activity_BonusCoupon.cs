using DragonU3DSDK.Network.API.ILProtocol;
using DragonU3DSDK.Network.API.Protocol;
using static DragonU3DSDK.Network.API.ILProtocol.SGetActivityUserData.Types;

namespace GameModule
{
    public class Activity_BonusCoupon : ActivityBase
    {
        public BonusCouponActivityConfig config { get; private set; }
        public BonusCouponActivityData userData { get; private set; }

        private bool _hasShown = false;

        public Activity_BonusCoupon() : base(ActivityType.GoldenCoupon)
        {
            
        }

        protected override bool IsExpired()
        {
            if (config == null)
            {
                return true;
            }
            return XUtility.GetLeftTime((ulong) config.EndTimestamp * 1000) <= 0;
        }

        public override bool IsUnlockState()
        {
            if (config == null) { return false; }
            var userController = Client.Get<UserController>();
            var level = userController.GetUserLevel();
            return (long)level >= config.LevelLimited;
        }

        public override void OnRefreshUserData(ActivityData activityData)
        {
            config = null;

            if (activityData != null)
            {
                config = BonusCouponActivityConfig.Parser.ParseFrom(activityData.ActivityConfig.Data);
                userData = BonusCouponActivityData.Parser.ParseFrom(activityData.ActivityUserData.Data);
                EnableUpdate(1);
            }

            EventBus.Dispatch<EventGoldenCouponActivityUserDateChanged>();
        }

        public override void OnEnterLobby()
        {
            base.OnEnterLobby();
            // test: remove valid condition
            if (IsValid() == false) { return; }
            if (userData == null || userData.CouponBinded) { return; }
            if (_hasShown) { return; }

            EventBus.Dispatch(new EventEnqueuePopup(new PopupArgs(typeof(UIActivity_BonusCoupon_MainPopup)) { extraArgs = this }));
            EventBus.Dispatch(new EventEnqueueFencePopup(null));
            _hasShown = true;
        }

        protected override async void OnExpire()
        {
            XDebug.Log($"11111111111111 activity on expire:{ServerActivityType}");
            await RequestCGetActivityUserDataAsync();
            base.OnExpire();
        }

        public SGetUserCoupons.Types.UserCoupon GetLinkedCoupon()
        {
            return Client.Get<InboxController>().GetLinkedCoupon(config.Id);
        }
    }
}