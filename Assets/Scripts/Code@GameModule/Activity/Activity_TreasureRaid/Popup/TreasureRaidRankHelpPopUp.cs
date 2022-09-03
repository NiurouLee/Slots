using UnityEngine;
using UnityEngine.UI;

namespace GameModule
{
    [AssetAddress("UITreasureRaidHelpPanel")]
    public class TreasureRaidRankHelpPopUp:Popup<TreasureRaidRankHelpPopUpController>
    {
        [ComponentBinder("Root/MainGroup/CloseButton")]
        private Button closeBtn;

        public TreasureRaidRankHelpPopUp(string address)
            : base(address)
        {
            contentDesignSize = new Vector2(1400,768);
        }

        protected override void OnViewSetUpped()
        {
            base.OnViewSetUpped();
            closeBtn.onClick.AddListener(OnCloseClicked);
        }
    }

    public class TreasureRaidRankHelpPopUpController : ViewController<TreasureRaidRankHelpPopUp>
    {
        protected override void SubscribeEvents()
        {
            SubscribeEvent<EventTreasureRaidOnExpire>(ActivityOnExpire);
        }

        private void ActivityOnExpire(EventTreasureRaidOnExpire obj)
        {
            view.Close();
        }
    }
}