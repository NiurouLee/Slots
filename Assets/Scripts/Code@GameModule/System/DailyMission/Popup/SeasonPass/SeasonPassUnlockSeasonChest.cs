//  **********************************************
//  Copyright(c) 2021 by com.ustar
//  All right reserved
// 
//  Author : Besure.Chen
//  Date :2021-10-04 09:12
//  Ver : 1.0.0
//  Description : SeasonPassUnlockSeason.cs
//  ChangeLog :
//  **********************************************

namespace GameModule
{
    [AssetAddress("UISeasonPassUnlockSeasonChestH","UISeasonPassUnlockSeasonChestV")]
    public class SeasonPassUnlockSeasonChest:Popup<SeasonPassUnlockSeasonChestViewController>
    {
        public SeasonPassUnlockSeasonChest(string assetAddress)
            : base(assetAddress)
        {
            
        }
    }

    public class SeasonPassUnlockSeasonChestViewController : ViewController<SeasonPassUnlockSeasonChest>
    {
        
    }
}