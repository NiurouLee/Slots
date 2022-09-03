//  **********************************************
//  Copyright(c) 2021 by com.ustar
//  All right reserved
// 
//  Author : Besure.Chen
//  Date :2021-10-04 08:29
//  Ver : 1.0.0
//  Description : SeasonPassLevelUp.cs
//  ChangeLog :
//  **********************************************

using UnityEngine.UI;

namespace GameModule
{
    [AssetAddress("UISeasonPassLevelUpH","UISeasonPassLevelUpV")]
    public class SeasonPassLevelUp:Popup
    {
        [ComponentBinder("Root/MainGroup/IconImage/LevelText")]
        private Text txtLevelText;
        public SeasonPassLevelUp(string assetAddress)
            : base(assetAddress)
        {
            
        }

        public void InitWith(int rewardLevel)
        {
            txtLevelText.text = rewardLevel.ToString();
        }

        [ComponentBinder("Root/BottomGroup/LaterButton")]
        private void OnBtnLaterClick()
        {
            Close();
            CheckAndCloseBuyLevel();
        }
        [ComponentBinder("Root/BottomGroup/CheckButton")]
        private void OnBtnCheckClick()
        {
            Close();
            EventBus.Dispatch(new EventSeasonPassCloseBuyLevel());
        }

        private void CheckAndCloseBuyLevel()
        {
            if (Client.Get<SeasonPassController>().IsLevel100)
            {
                EventBus.Dispatch(new EventSeasonPassCloseBuyLevel());
            }  
        }

    }
}