using TMPro;

namespace GameModule
{
    [AssetAddress("UIValentinesDay2022RewardShow")]
    public class UIActivity_ValentinesDay_RewardShowPopup : Popup
    {
        [ComponentBinder("Root/RewardGroup/UIValentinesDay2022RewardCell/CountText")]
        public TMP_Text textRewardShow;

        public UIActivity_ValentinesDay_RewardShowPopup(string address) : base(address)
        {
            // contentDesignSize = new Vector2(1024, 768);
        }

        public void Set(int count)
        {
            if (textRewardShow != null)
            {
                textRewardShow.text = $"+{count}";
            }
        }
    }
}
