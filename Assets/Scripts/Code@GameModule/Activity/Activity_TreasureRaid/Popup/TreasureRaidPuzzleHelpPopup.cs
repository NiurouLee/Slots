using UnityEngine;
using UnityEngine.UI;

namespace GameModule
{
    [AssetAddress("UITreasureRaidPuzzleHelpPanel")]
    public class TreasureRaidPuzzleHelpPopup:Popup<TreasureRaidPuzzleHelpPopupController>
    {
        [ComponentBinder("Root/MainGroup/CloseButton")]
        private Button closeBtn;

        public TreasureRaidPuzzleHelpPopup(string address)
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

    public class TreasureRaidPuzzleHelpPopupController : ViewController<TreasureRaidPuzzleHelpPopup>
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