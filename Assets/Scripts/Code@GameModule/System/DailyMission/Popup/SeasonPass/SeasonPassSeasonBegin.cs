//  **********************************************
//  Copyright(c) 2021 by com.ustar
//  All right reserved
// 
//  Author : Besure.Chen
//  Date :2021-10-04 09:10
//  Ver : 1.0.0
//  Description : SeasonPassSeasonBegin.cs
//  ChangeLog :
//  **********************************************

namespace GameModule
{
    [AssetAddress("UISeasonPassSeasonBeginH","UISeasonPassSeasonBeginV")]
    public class SeasonPassSeasonBegin:Popup
    {
        public SeasonPassSeasonBegin(string assetAddress)
            : base(assetAddress)
        {
            Client.Get<SeasonPassController>().SaveNewSeason(false);   
        }

        [ComponentBinder("Root/BottomGroup/AwesomeButton")]
        public void OnBtnAwesomeClick()
        {
            Close();
        }
    }
}