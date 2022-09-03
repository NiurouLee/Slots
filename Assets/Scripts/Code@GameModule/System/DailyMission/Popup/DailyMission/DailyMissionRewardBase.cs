//  **********************************************
//  Copyright(c) 2021 by com.ustar
//  All right reserved
// 
//  Author : Besure.Chen
//  Date :2021-09-27 14:22
//  Ver : 1.0.0
//  Description : DailyMissionRewardBase.cs
//  ChangeLog :
//  **********************************************

using DragonPlus;
using DragonU3DSDK.Network.API.ILProtocol;
using Google.ilruntime.Protobuf.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameModule
{
    public class DailyMissionRewardBase : Popup<ViewController>
    {
        [ComponentBinder("Root/BottomGroup")]
        protected Transform transBottomGroup;
        [ComponentBinder("Root/BottomGroup/ConfirmButton")]
        protected Button confirmButton;

        [ComponentBinder("Root/BottomGroup/ADSConfirmButton")]
        protected Button adsButton;

        [ComponentBinder("Root/BottomGroup/CollectButton")]
        protected Button collectButton;

        [ComponentBinder("Root/RewardGroup")]
        public Transform rewardGroup;

        protected DailyMissionMainViewController _mainViewController;
        protected RewardController _reward;
        protected MissionController _missionController;

        protected eAdReward _adPlaceID = eAdReward.DailyMissionRV;
         
        public DailyMissionRewardBase(string address) : base(address)
        {

        }

        public DailyMissionRewardBase()
        {

        }

        public void Init(DailyMissionMainViewController mainViewController)
        {
            _mainViewController = mainViewController;
        }

        protected void SetButtonsInteractable(bool interactable)
        {
            if (confirmButton != null)
            {
                confirmButton.interactable = interactable;
            }

            if (adsButton != null)
            {
                adsButton.interactable = interactable;
            }

            if (collectButton != null)
            {
                collectButton.interactable = interactable;
            }
        }

        protected void SetButtonState()
        {
            if (adsButton != null)
            {
                confirmButton.gameObject.SetActive(false);
                if (AdController.Instance.ShouldShowRV(_adPlaceID,false) && GetCoinAmount() > 0)
                {
                    adsButton.gameObject.SetActive(true);
                    collectButton.gameObject.SetActive(false);
                    confirmButton.gameObject.SetActive(true);
                     
                    if (transBottomGroup && ViewResolution.referenceResolutionLandscape.x < ViewResolution.designSize.x)
                    {
                        var scale = (float)ViewResolution.referenceResolutionLandscape.x / ViewResolution.designSize.x;
                        transBottomGroup.localScale = new Vector3(scale, scale, scale);
                    }
                }
                else
                {
                    confirmButton.gameObject.SetActive(false);
                    collectButton.gameObject.SetActive(true);
                    adsButton.gameObject.SetActive(false);
                }
            }
        }

        protected virtual void RefreshUI()
        {
            InitRewardContent(_reward.Reward);
            SetButtonState();
            SetButtonsInteractable(true);
        }

        public void InitRewardContent(Reward reward)
        {
            int childCount = rewardGroup.childCount;
            for (int i = childCount - 1; i >= 1; i--)
            {
                GameObject.DestroyImmediate(rewardGroup.GetChild(i).gameObject);
            }

            var template = rewardGroup.GetChild(0);
            template.gameObject.SetActive(false);
            XItemUtility.InitItemsUI(rewardGroup, new RepeatedField<Reward> { reward }, template, items =>
               {
                   if (items[0].Type == Item.Types.Type.Coin)
                   {
                       return GetCoinAmount().GetCommaFormat();
                   }
                   return XItemUtility.GetItemDefaultDescText(items[0]);
               });
        }

        protected virtual long GetCoinAmount()
        {
            return _reward.GetCoinsAmount();
        }

        public void InitializeReward(RewardController reward, MissionController missionController = null)
        {
            _reward = reward;
            _missionController = missionController;
            Show();
            RefreshUI();
        }

        protected void CheckActivityItem(string source)
        {
            if (_reward == null || _reward.Reward == null) { return; }
            var reward = _reward.Reward;
            if (reward.Items == null || reward.Items.count == 0) { return; }

            foreach (var item in reward.Items)
            {
                if (item.Type == Item.Types.Type.ValentineActivityPoint)
                {
                    EventBus.Dispatch(new Event_Activity_Valentine2022_CollectItem());

                    var activityController = Client.Get<ActivityController>();
                    var valentineActivity = activityController.GetDefaultActivity(ActivityType.Valentine2022) as Activity_ValentinesDay;
                    if (valentineActivity != null)
                    {
                        switch (source)
                        {
                            case "DailyMission":
                                valentineActivity.itemSource = 1;
                                break;
                            case "HonorMission":
                                valentineActivity.itemSource = 2;
                                break;
                            default:
                                valentineActivity.itemSource = -1;
                                break;
                        }
                    }

                    break;
                }
                else if (item.Type == Item.Types.Type.IndependenceDayActivityPoint)
                {
                    var activityController = Client.Get<ActivityController>();
                    var valentineActivity = activityController.GetDefaultActivity(ActivityType.JulyCarnival) as Activity_JulyCarnival;
                    if (valentineActivity != null)
                    {
                        var lastData = valentineActivity.GetIndependenceDayMainPageInfo();
                        if (lastData != null && lastData.Step < lastData.StepMax)
                        {
                            switch (source)
                            {
                                case "DailyMission":
                                    valentineActivity.itemSource = 1;
                                    break;
                                case "HonorMission":
                                    valentineActivity.itemSource = 2;
                                    break;
                                default:
                                    valentineActivity.itemSource = -1;
                                    break;
                            }
                            EventBus.Dispatch(new EventJulyCarnivalCollectItem());
                        }
                    }
                    break;
                }
            }
        }
    }
}