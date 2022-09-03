//  **********************************************
//  Copyright(c) 2021 by com.ustar
//  All right reserved
// 
//  Author : Besure.Chen
//  Date :2021-09-17 20:46
//  Ver : 1.0.0
//  Description : DailyMissionHonor.cs
//  ChangeLog :
//  **********************************************

using System;
using DragonU3DSDK.Network.API.ILProtocol;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameModule
{
    public class DailyMissionPageItemHonor : DailyMissionPageItemBase
    {
        [ComponentBinder("Root/TimerGroup/Root/TimerText")]
        private TextMeshProUGUI txtHonorLeftText;

        private MissionController honorMission;

        public DailyMissionPageItemHonor()
        {

        }

        public override void RefreshUI()
        {
            if (honorMission != null)
            {
                progressBar.value = honorMission.GetProgress();
                txtProgress.text = honorMission.GetProgressDescText();
                txtNormalContent.text = honorMission.GetContentDescText();
                transNormalState.gameObject.SetActive(!honorMission.IsFinish());
                transFinishState.gameObject.SetActive(honorMission.IsFinish());
                txtnStarPoint.text = honorMission.GetMissionStar().ToString();
                txtHonorLeftText.text = Client.Get<DailyMissionController>().GetHornorMissionTimeLeft();
                transStarPoint.gameObject.SetActive(!Client.Get<SeasonPassController>().IsLocked && honorMission.GetMissionStar() > 0);

                SetActivityItem(honorMission?.GetMission());
            }
        }

        public void Update()
        {
            if (honorMission != null)
            {
                txtHonorLeftText.text = Client.Get<DailyMissionController>().GetHornorMissionTimeLeft();
            }
        }

        public override void RefreshMission()
        {
            honorMission = Client.Get<DailyMissionController>().GetHonorMission();
        }

        [ComponentBinder("CollectButton")]
        private void OnBtnCollectClicked()
        {
            actionClaim?.Invoke(0, true, () =>
             {
                 RefreshMission();
                 RefreshUI();
             });
        }
    }
}