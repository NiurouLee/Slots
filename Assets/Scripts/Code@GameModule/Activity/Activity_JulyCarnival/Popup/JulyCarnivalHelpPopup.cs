using DragonU3DSDK.Network.API.ILProtocol;
using UnityEngine.UI;

namespace GameModule
{
    [AssetAddress("UIIndependenceDayHelp", "UIIndependenceDayHelpV")]
    public class JulyCarnivalHelpPopup : Popup<JulyCarnivalHelpPopupController>
    {
        [ComponentBinder("Root/MainGroup/L/Text")]
        private Text dailyMissionText;
        
        [ComponentBinder("Root/MainGroup/R/Text")]
        private Text honorMissionText;
        public JulyCarnivalHelpPopup(string address) : base(address)
        {
            // contentDesignSize = new Vector2(1024, 768);
        }

        protected override void EnableView()
        {
            base.EnableView();
            var dailyMissionController = Client.Get<DailyMissionController>();
            var missionController = dailyMissionController.GetNormalMission(1);
            var mission = missionController.GetMission();
            var item = XItemUtility.GetItem(mission.Items, Item.Types.Type.IndependenceDayActivityPoint);
            if (item != null)
            {
                dailyMissionText.SetText($"X{item.IndependenceActivityPoint.Amount}");
            }

            var honorMissionController = dailyMissionController.GetHonorMission();
            var honorMission = honorMissionController.GetMission();
            var honorItem = XItemUtility.GetItem(honorMission.Items, Item.Types.Type.IndependenceDayActivityPoint);
            if (honorItem != null)
            {
                honorMissionText.SetText($"X{honorItem.IndependenceActivityPoint.Amount}");
            }
        }
    }

    public class JulyCarnivalHelpPopupController : ViewController<JulyCarnivalHelpPopup>
    {
        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();
            SubscribeEvent<EventActivityExpire>(OnEventActivityExpire);
        }

        private void OnEventActivityExpire(EventActivityExpire evt)
        {
            if (evt.activityType != ActivityType.JulyCarnival)
                return;
            view.Close();
        }
    }
}