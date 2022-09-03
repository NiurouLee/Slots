using UnityEngine.UI;

namespace GameModule {
    [AssetAddress("UIIndependenceDayRewardShow", "UIIndependenceDayRewardShowV")]
    public class JulyCarnivalStarShow: View 
    {
        [ComponentBinder("Root/RewardGroup/UIValentinesDay2022RewardCell/StandarType/CountText")] private Text rewardText;

        public JulyCarnivalStarShow(string address):base(address) 
        {

        }

        public void SetViewContent(int starCount)
        {
            rewardText.SetText($"{starCount}");
        }
    }
}