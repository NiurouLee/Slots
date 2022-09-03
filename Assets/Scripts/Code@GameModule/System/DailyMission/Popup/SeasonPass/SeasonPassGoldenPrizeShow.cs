//  **********************************************
//  Copyright(c) 2021 by com.ustar
//  All right reserved
// 
//  Author : Besure.Chen
//  Date :2021-10-04 08:54
//  Ver : 1.0.0
//  Description : UISeasonPassGoldenPrize.cs
//  ChangeLog :
//  **********************************************

using DragonU3DSDK.Network.API.ILProtocol;
using TMPro;
using UnityEngine;

namespace GameModule
{
    [AssetAddress("UISeasonPassGoldenChestPrizeShowH","UISeasonPassGoldenChestPrizeShowV")]
    public class SeasonPassGoldenPrizeShow:Popup<SeasonPassGoldenPrizeViewController>
    {
        [ComponentBinder("Root/MainGroup/LockState")]
        public Transform transLockState;
        [ComponentBinder("Root/MainGroup/EnableState")]
        public Transform transEnableState;
        [ComponentBinder("Root/MainGroup/IntegralGroup/IntegralText")]
        public TextMeshProUGUI integralText;
        public SeasonPassGoldenPrizeShow(string assetAddress)
            : base(assetAddress)
        {
            
        }
    }

    public class SeasonPassGoldenPrizeViewController : ViewController<SeasonPassGoldenPrizeShow>
    {
        public override void OnViewDidLoad()
        {
            base.OnViewDidLoad();
            UpdateFinalPrize();
        }

        public void UpdateFinalPrize()
        {
            var passController = Client.Get<SeasonPassController>();
            var finalReward = passController.FinalReward;
            var item = XItemUtility.GetItem(finalReward.Items, Item.Types.Type.Coin);
            if (item != null)
            {
                view.integralText.text = item.Coin.Amount.GetCommaFormat();
            }
            view.transLockState.gameObject.SetActive(!passController.Paid);
            view.transEnableState.gameObject.SetActive(passController.Paid);
        }
    }
}