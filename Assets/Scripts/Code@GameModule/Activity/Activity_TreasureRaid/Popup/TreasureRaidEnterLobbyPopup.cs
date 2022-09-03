using DragonU3DSDK.Network.API.ILProtocol;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameModule
{
    [AssetAddress("UITreasureRaidAd")]
    public class TreasureRaidEnterLobbyPopup : Popup<TreasureRaidEnterLobbyPopupController>
    {
        [ComponentBinder("Root/MainGroup/GetButton")]
        private Button goBtn;

        [ComponentBinder("Root/MainGroup/LeftGroup/LeftTimeText")]
        public TextMeshProUGUI leftTimeText;

        public Activity_TreasureRaid activityTreasureRaid;
        
        public TreasureRaidEnterLobbyPopup(string address)
            : base(address)
        {
            contentDesignSize = new Vector2(1465, 768);
        }
        protected override void OnViewSetUpped()
        {
            base.OnViewSetUpped();
            goBtn.onClick.AddListener(OnGoBtnClicked);
            activityTreasureRaid =
                Client.Get<ActivityController>().GetDefaultActivity(ActivityType.TreasureRaid) as Activity_TreasureRaid;

            if (activityTreasureRaid != null)
            {
                var leftTime = activityTreasureRaid.GetCountDown();
                UpdateLeftTime(leftTime);
            }
        }

        private void OnGoBtnClicked()
        {
            if (activityTreasureRaid == null)
            {
                Close();
                return;
            }
            BiManagerGameModule.Instance.SendGameEvent(BiEventFortuneX.Types.GameEventType.GameEventTreasureraidEnter, ("OperationId", "1"));

            if (activityTreasureRaid.GetCurrentRunningRoundID() == 0)
                EventBus.Dispatch(new EventShowPopup(new PopupArgs(typeof(TreasureRaidMapPopup), true,"EnterLobbyAd")));
            else
                EventBus.Dispatch(new EventShowPopup(new PopupArgs(typeof(TreasureRaidMainPopup), true,"EnterLobbyAd")));
            
            Close();
        }

        public void UpdateLeftTime(float leftTime)
        {
            if (leftTime <= 0)
            {
                leftTimeText.SetText("00:00:00");
            }
            else
            {
                leftTimeText.SetText(XUtility.GetTimeText(leftTime));
            }
        }
    }

    public class TreasureRaidEnterLobbyPopupController : ViewController<TreasureRaidEnterLobbyPopup>
    {
        public override void OnViewEnabled()
        {
            base.OnViewEnabled();
            EnableUpdate(1);
        }

        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();
            SubscribeEvent<EventTreasureRaidOnExpire>(ActivityOnExpire);
        }

        private void ActivityOnExpire(EventTreasureRaidOnExpire obj)
        {
            view.Close();
        }

        public override void Update()
        {
            base.Update();
            if (view.activityTreasureRaid == null)
            {
                DisableUpdate();
                view.UpdateLeftTime(0);
                return;
            }
            var leftTime = view.activityTreasureRaid.GetCountDown();
            if (leftTime <= 0)
                DisableUpdate();
            
            view.UpdateLeftTime(leftTime);
        }
    }
}