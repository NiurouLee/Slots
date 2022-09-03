using DragonU3DSDK.Network.API.ILProtocol;
using TMPro;

namespace GameModule
{
    [AssetAddress("UIIndependnceDay_MissionStar")]
    public class ValentinesDayDailyMissionWidgetView : View<ValentinesDayDailyMissionWidgetViewController>
    {
        [ComponentBinder("CountText")]
        private TextMeshProUGUI countText;

        public ValentinesDayDailyMissionWidgetView(string address) : base(address)
        {
            
        }
        
        public void SetUpContent(Item item)
        {
            countText.SetText($"+{item.ValentineActivityPoint.Amount}");
        }
    }
    
    public class ValentinesDayDailyMissionWidgetViewController : ViewController<ValentinesDayDailyMissionWidgetView>
    {
        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();
            EventBus.Subscribe<EventActivityExpire>(OnEventJulyCarnivalExpired);
        }

        private void OnEventJulyCarnivalExpired(EventActivityExpire evt)
        {
            if (evt.activityType != ActivityType.Valentine2022)
                return;
            view.GetParentView().RemoveChild(view);
        }
    }
}