using DragonU3DSDK.Network.API.ILProtocol;
using TMPro;
using UnityEngine.UI;

namespace GameModule
{
    [AssetAddress("UIIndependnceDay_MissionStar")]
    public class JulyCarnivalDailyMissionWidgetView : View<JulyCarnivalDailyMissionWidgetViewController>
    {
        [ComponentBinder("StandarType/CountText")]
        private TextMeshProUGUI countText;

        public int missionIndex;
        public JulyCarnivalDailyMissionWidgetView(string address) : base(address)
        {
            
        }

        public void SetUpContent(Item item, int inMissionIndex)
        {
            missionIndex = inMissionIndex;
            countText.SetText($"+{item.IndependenceActivityPoint.Amount}");
        }
    }
    
    public class JulyCarnivalDailyMissionWidgetViewController : ViewController<JulyCarnivalDailyMissionWidgetView>
    {
        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();
            EventBus.Subscribe<EventActivityExpire>(OnEventJulyCarnivalExpired);
            EventBus.Subscribe<EventJulyCarnivalActivityFinish>(OnEventJulyCarnivalActivityFinish);
            EventBus.Subscribe<EventJulyCarnivalRefreshItem>(OnEventJulyCarnivalRefreshItem);
        }

        private void OnEventJulyCarnivalRefreshItem(EventJulyCarnivalRefreshItem evt)
        {
            if (view.transform == null)
                return;
            if (evt.nMissionIndex != view.missionIndex)
                return;
            view.GetParentView().RemoveChild(view);
        }

        private void OnEventJulyCarnivalActivityFinish(EventJulyCarnivalActivityFinish evt)
        {
            if (view.transform == null)
                return;
            view.GetParentView().RemoveChild(view);
        }

        private void OnEventJulyCarnivalExpired(EventActivityExpire evt)
        {
            if (evt.activityType != ActivityType.JulyCarnival)
                return;
            if (view.transform == null)
                return;
            view.GetParentView().RemoveChild(view);
        }
    }
}