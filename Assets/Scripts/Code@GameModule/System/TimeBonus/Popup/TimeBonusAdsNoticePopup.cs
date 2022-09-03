// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/11/24/11:19
// Ver : 1.0.0
// Description : TimeBonusAdsNoticePopup.cs
// ChangeLog :
// **********************************************

using DragonU3DSDK.Network.API.ILProtocol;
using UnityEngine;
using UnityEngine.UI;

namespace GameModule
{
    [AssetAddress("UITimerBonusADSNotice")]
    public class TimeBonusAdsNoticePopup : Popup
    {
        [ComponentBinder("SpinButton")] 
        public Button spinButton;

        public TimeBonusAdsNoticePopup(string address)
            : base(address)
        {
            contentDesignSize = new Vector2(1300, 768);
        }

        protected override void OnViewSetUpped()
        {
            base.OnViewSetUpped();
            spinButton.onClick.AddListener(OnSpinAgainButtonClicked);
            BiManagerGameModule.Instance.SendGameEvent(BiEventFortuneX.Types.GameEventType.GameEventAdsPop, ("adsType","rewardVideo"),("placeId", eAdReward.WheelBonusRV.ToString()), ("userGroup", AdConfigManager.Instance.MetaData.GroupId.ToString()));
        }

        public void OnSpinAgainButtonClicked()
        {
            EventBus.Dispatch(new EventOnLuckyWheelAdNoticeChoose(true));
            Close();
        }

        protected override void OnCloseClicked()
        {
            base.OnCloseClicked();
            EventBus.Dispatch(new EventOnLuckyWheelAdNoticeChoose(false));
        }
    }
}