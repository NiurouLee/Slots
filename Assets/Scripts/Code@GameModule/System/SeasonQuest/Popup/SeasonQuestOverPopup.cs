// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/11/05/19:02
// Ver : 1.0.0
// Description : QuestOverPopup.cs
// ChangeLog :
// **********************************************

using UnityEngine;
using UnityEngine.UI;

namespace GameModule
{
    [AssetAddress("UISeasonQuestOneGameOver")]
    public class SeasonQuestOverPopup:Popup
    {
        [ComponentBinder("SpinButton")] private Button goSpinButton;

        public SeasonQuestOverPopup(string address)
            : base(address)
        {
        
        }
        
        public override Vector3 CalculateScaleInfo()
        {
            if (ViewManager.Instance.IsPortrait)
            {
                return Vector3.one * 0.9f;
            }

            return Vector3.one;
        }

        protected override void SetUpExtraView()
        {
            base.SetUpExtraView();
            goSpinButton.onClick.AddListener(OnGoSpinButtonClicked);
        }

        protected void OnGoSpinButtonClicked()
        {
            Close();
            SoundController.PlayButtonClick();
          
            bool needCloseView = SeasonQuestPopup.OutQuest();

            if (needCloseView)
            {
                var questPopup = PopupStack.GetPopup<SeasonQuestPopup>();
                if(questPopup != null)
                    questPopup.Close();
            }
        }
    }
}