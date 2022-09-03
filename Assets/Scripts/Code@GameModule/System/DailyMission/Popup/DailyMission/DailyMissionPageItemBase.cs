//  **********************************************
//  Copyright(c) 2021 by com.ustar
//  All right reserved
// 
//  Author : Besure.Chen
//  Date :2021-09-27 14:27
//  Ver : 1.0.0
//  Description : DailyMissionPageItemBase.cs
//  ChangeLog :
//  **********************************************

using System;
using DragonU3DSDK.Network.API.ILProtocol;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameModule
{
    public class DailyMissionPageItemBase : View
    {
        [ComponentBinder("Root/NumberText")]
        protected Text txtMissionNum;

        //NormalState
        [ComponentBinder("Root/NormalState")]
        protected Transform transNormalState;
        [ComponentBinder("Root/ContentText")]
        protected TextMeshProUGUI txtNormalContent;

        [ComponentBinder("Root/NormalState/RewardProgressGroup/RewardGiftButton")]
        protected Animator animatorGiftBox;
        [ComponentBinder("Root/NormalState/RewardProgressGroup/RewardGiftButton")]
        protected Canvas canvasGiftBox;
        [ComponentBinder("Root/NormalState/RewardProgressGroup/ProgressBar")]
        protected Slider progressBar;
        [ComponentBinder("Root/NormalState/RewardProgressGroup/ProgressBar/Fill Area/ProgressText")]
        protected TextMeshProUGUI txtProgress;
        [ComponentBinder("Root/NormalState/GetButton/CurrencyGroup/CountText")]
        protected TextMeshProUGUI txtCompleteCostDiamond;

        //FinishState
        [ComponentBinder("Root/FinishState")]
        protected Transform transFinishState;

        //LockState
        [ComponentBinder("Root/LockState")]
        protected Transform transLockState;

        [ComponentBinder("Root/StarPoint")]
        protected Transform transStarPoint;
        [ComponentBinder("Root/StarPoint/CountText")]
        protected TextMeshProUGUI txtnStarPoint;

        [ComponentBinder("Root/ActivityPoint")]
        protected Transform activityPointParent;
        protected bool initActivityPoint = false;


        protected int nMissionIndex;
        protected MissionController normalMission;

        public Action<int, bool, Action> actionClaim;



        public DailyMissionPageItemBase()
        {

        }

        public virtual void InitMission(int missionNum = 0)
        {
            nMissionIndex = missionNum;
            RefreshMission();
            RefreshUI();
        }

        public virtual void RefreshMission()
        {
            normalMission = Client.Get<DailyMissionController>().GetNormalMission(nMissionIndex);
        }

        public virtual void RefreshUI()
        {
            if (normalMission != null)
            {
                RefreshUIState();

                txtMissionNum.text = (nMissionIndex + 1).ToString();
                progressBar.value = normalMission.GetProgress();
                txtProgress.text = normalMission.GetProgressDescText();
                txtNormalContent.text = normalMission.GetContentDescText();
                txtnStarPoint.text = normalMission.GetMissionStar().ToString();
                if (normalMission.IsClaimed())
                {
                    EventBus.Dispatch(new EventJulyCarnivalRefreshItem(nMissionIndex));
                    txtNormalContent.text = normalMission.GetFinishContentText();
                }
                else if (Client.Get<DailyMissionController>().CurrentMission == normalMission)
                {
                    txtCompleteCostDiamond.text = normalMission.CompleteNeedCostDiamond().ToString();
                }
                else
                {
                    txtNormalContent.text = normalMission.GetLockContentText();
                }
            }


            SetActivityItem(normalMission?.GetMission());
        }

        protected void SetActivityItem(Mission mission)
        {
            if (initActivityPoint)
                return;

            if (mission == null || mission.Items == null || mission.Items.count == 0) { return; }
            if (mission.Collected) { return; }

            var activityController = Client.Get<ActivityController>();
            if (activityController == null) { return; }

            initActivityPoint = true;
            EventBus.Dispatch(new EventSetActivityIconInDailyMission(this, activityPointParent, mission, nMissionIndex));
            
            // Item valentineItem = null;
            // foreach (var item in mission.Items)
            // {
            //     if (item.Type == Item.Types.Type.ValentineActivityPoint)
            //     {
            //         valentineItem = item;
            //         break;
            //     }
            // }
            //
            // var valentineActivity = activityController.GetDefaultActivity(ActivityType.Valentine2022) as Activity_ValentinesDay;
            //
            // if (valentineActivity != null && valentineActivity.collectFinish == false
            //     && valentineActivity.IsValid() && valentineItem != null)
            // {
            //     if (transformActivityValentineRewardCell != null)
            //     {
            //         transformActivityValentineRewardCell.gameObject.SetActive(true);
            //         XItemUtility.InitItemUI(
            //             transformActivityValentineRewardCell.parent,
            //             valentineItem,
            //             null,
            //             transformActivityValentineRewardCell.name);
            //     }
            // }
        }

        private void RefreshUIState()
        {
            animatorGiftBox.gameObject.SetActive(true);
            XUtility.PlayAnimation(animatorGiftBox, "Idle");
            var currentMission = Client.Get<DailyMissionController>().CurrentMission;
            if (normalMission.IsClaimed())
            {
                transLockState.gameObject.SetActive(false);
                transNormalState.gameObject.SetActive(false);
                transFinishState.gameObject.SetActive(true);
            }
            else if (currentMission == normalMission)
            {
                transLockState.gameObject.SetActive(false);
                transNormalState.gameObject.SetActive(true);
                transFinishState.gameObject.SetActive(false);
            }
            else
            {
                transLockState.gameObject.SetActive(true);
                transNormalState.gameObject.SetActive(false);
                transFinishState.gameObject.SetActive(false);
            }
            var canShowStar = !Client.Get<SeasonPassController>().IsLocked && !normalMission.IsClaimed() && normalMission.GetMissionStar() > 0;
            transStarPoint.gameObject.SetActive(canShowStar);
        }
    }
}