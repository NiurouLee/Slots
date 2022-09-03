// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/11/05/21:19
// Ver : 1.0.0
// Description : QuestComingSoonPopup.cs
// ChangeLog :
// **********************************************

using UnityEngine.UI;

namespace GameModule
{
    [AssetAddress("UIQuestComingSoon")]
    public class QuestComingSoonPopup:Popup
    {
        [ComponentBinder("ConfirmButton")]
        public Button confirmButton;

        public QuestComingSoonPopup(string address)
            : base(address)
        {
            
        }
        
        protected override void SetUpExtraView()
        {
            confirmButton.onClick.AddListener(OnConfirmClicked);
        }
        
        protected void OnConfirmClicked()
        {
            SoundController.PlayButtonClick();
            EventBus.Dispatch(new EventQuestFinished());
            Close();
        }
    }
}