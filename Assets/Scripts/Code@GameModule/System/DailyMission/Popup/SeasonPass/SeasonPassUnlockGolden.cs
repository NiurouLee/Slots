//  **********************************************
//  Copyright(c) 2021 by com.ustar
//  All right reserved
// 
//  Author : Besure.Chen
//  Date :2021-10-04 09:11
//  Ver : 1.0.0
//  Description : SeasonPassUnlockGolden.cs
//  ChangeLog :
//  **********************************************

namespace GameModule
{
    [AssetAddress("UISeasonPassUnlockGoldenPassH","UISeasonPassUnlockGoldenPassV")]
    public class SeasonPassUnlockGolden:Popup
    {
        public SeasonPassUnlockGolden(string assetAddress)
            : base(assetAddress)
        {
            
        }

        [ComponentBinder("Root/BottomGroup/CheckButton")]
        public void OnBtnYeahClick()
        {
            Close();
        }
    }
} 