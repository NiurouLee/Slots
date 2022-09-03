//  **********************************************
//  Copyright(c) 2021 by com.ustar
//  All right reserved
// 
//  Author : Besure.Chen
//  Date :2021-09-17 22:16
//  Ver : 1.0.0
//  Description : DailyMissionRewardWeek.cs
//  ChangeLog :
//  **********************************************

using DragonU3DSDK.Network.API.ILProtocol;
using UnityEngine;

namespace GameModule
{
    [AssetAddress("UIDailyMissionRewardProgressH", "UIDailyMissionRewardProgressV")]
    public class DailyMissionRewardWeek : DailyMissionRewardNormal
    {
        public DailyMissionRewardWeek(string address) : base(address)
        {

        }

        protected override void OnViewSetUpped()
        {
            base.OnViewSetUpped();

            var stage = Client.Get<DailyMissionController>().GetStagePointProgress() < 0.9f ? 1 : 2;
            BiManagerGameModule.Instance.SendGameEvent(BiEventFortuneX.Types.GameEventType.GameEventDailyMissionStageCollect, ("stage", stage.ToString()), ("countDown", Client.Get<DailyMissionController>().GetStageTimeLeft()));
        }

        protected override void BindingComponent()
        {
            ComponentBinder.BindingComponent(this, transform);
        }

        protected override async void OnBtnConfirmClick()
        {
            confirmButton.interactable = false;
            if (_reward.GetCoinAmount() > 0)
            {
                await XUtility.FlyCoins(confirmButton.transform, new EventBalanceUpdate(_reward.GetCoinAmount(), "DaiMissionNormalReward"));
            }
            Close();
            await _mainViewController.ShowCollectStateMissionPoint(_reward.GetMissionPointAmount(), _reward.GetMissionStarAmount());
        }
    }
}