//  **********************************************
//  Copyright(c) 2021 by com.ustar
//  All right reserved
// 
//  Author : Besure.Chen
//  Date :2021-09-17 17:55
//  Ver : 1.0.0
//  Description : DailyMissionPage.cs
//  ChangeLog :
//  **********************************************

using System;
using System.Collections.Generic;
using System.Threading;
using DG.Tweening;
using DragonU3DSDK.Network.API.ILProtocol;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameModule
{
    public class DailyMissionPage : View<DailyMissionPageViewController>
    {
        [ComponentBinder("ProgressGroup")]
        public Canvas canvasProgressGroup;

        [ComponentBinder("BoxCell1")]
        public Transform boxCell1;

        [ComponentBinder("BoxCell2")]
        public Transform boxCell2;

        [ComponentBinder("BoxCell3")]
        public Transform boxCell3;


        [ComponentBinder("ProgressGroup/ProgressBar")]
        public Slider progressBarStage;
        [ComponentBinder("ProgressGroup/ProgressBar/ProgressText")]
        public TextMeshProUGUI txtProgressBarStage;
        [ComponentBinder("ProgressGroup/ProgressBar/Fill Area/ep_UI_Fill")]
        public Transform transProgressBarTrail;
        [ComponentBinder("ProgressGroup/DailyMissionTag/DailyMissionTimerText")]
        public TextMeshProUGUI txtStageTimeLeft;

        [ComponentBinder("ProgressGroup/BoxGroup/BoxCell1/Root")]
        private Transform transLevelUp;
        [ComponentBinder("ProgressGroup/BoxGroup/BoxCell3/Root")]
        private Transform transDiamond;


        [ComponentBinder("RewardBubble")]
        public Animator animatorRewardBubble;
        [ComponentBinder("RewardBubble/Root")]
        public Transform transBubbleGroup;
        // [ComponentBinder("RewardBubble/Root/ContentText")]
        // public TextMeshProUGUI txtRewardBubbleContent;

        [ComponentBinder("UIDailyMissionHonerCell")]
        public Transform transDailyMissionHorner;

        [ComponentBinder("ScrollView")]
        public ScrollRect ScrollView;

        public DailyMissionPageItemHonor _dailyMissionHonor;
        public List<DailyMissionPageItemNormal> listDailyMissionNormal;
        public DailyMissionMainViewController _mainViewController;

        public DailyMissionPage()
        {
        }
        public DailyMissionPage(string address) : base(address)
        {
        }

        protected override void OnViewSetUpped()
        {
            base.OnViewSetUpped();
            listDailyMissionNormal = new List<DailyMissionPageItemNormal>();

            string honorPath = "ScrollView/Viewport/Content/UIDailyMissionCellHoner";
            string normalFormat = "ScrollView/Viewport/Content/UIDailyMissionCellCommon{0}";

            transDailyMissionHorner = transform.Find(honorPath);
            for (int i = 0; i < 3; i++)
            {
                var transformNormal = transform.Find(string.Format(normalFormat, i));
                var dailyMission = AddChild<DailyMissionPageItemNormal>(transformNormal);
                dailyMission.actionClaim += ClaimMission;
                dailyMission.actionCompleteNow += CompeleteNormalMission;
                dailyMission.InitMission(i);
                listDailyMissionNormal.Add(dailyMission);
            }
            _dailyMissionHonor = AddChild<DailyMissionPageItemHonor>(transDailyMissionHorner);
            _dailyMissionHonor.actionClaim += ClaimMission;
            _dailyMissionHonor.InitMission(-1);
            boxCell2.gameObject.SetActive(false);
            RefreshUI();
            viewController.Update();
        }

        public void InitWith(DailyMissionMainViewController controller)
        {
            _mainViewController = controller;
        }

        public void TurnToLastPage(bool isLast)
        {
            if (isLast)
            {
                if (ScrollView.horizontal)
                {
                    ScrollView.horizontalNormalizedPosition = 1;
                }

                if (ScrollView.vertical)
                {
                    ScrollView.verticalNormalizedPosition = 0;
                }
            }
        }

        public void RefreshUI()
        {
            for (int i = 0; i < listDailyMissionNormal.Count; i++)
            {
                listDailyMissionNormal[i].RefreshMission();
                listDailyMissionNormal[i].RefreshUI();
            }
            _dailyMissionHonor.RefreshMission();
            _dailyMissionHonor.RefreshUI();
            RefreshStagePoint(0);
            boxCell1.gameObject.SetActive(GetLevelUpItem() != null && CanShowLevelUp());
            boxCell3.gameObject.SetActive(GetDiamondItem() != null && CanShowDiamond());
        }

        public void RefreshItems()
        {
            for (int i = 0; i < listDailyMissionNormal.Count; i++)
            {
                listDailyMissionNormal[i].InitMission(i);
            }
            _dailyMissionHonor.InitMission();
        }

        public async void RefreshStagePoint(float time)
        {
            progressBarStage.DOValue(Client.Get<DailyMissionController>().GetStagePointProgress(), time);
            await XUtility.WaitSeconds(time);
            transProgressBarTrail.gameObject.SetActive(Client.Get<DailyMissionController>().GetStagePointProgress() < 1);
            txtProgressBarStage.text =
                $"{Client.Get<DailyMissionController>().StatePoint}/{Client.Get<DailyMissionController>().StagePointTotal}";

        }


        [ComponentBinder("ProgressGroup/BoxGroup/BoxCell1/Root/CloseState")]
        private void OnBtnLevelUpClicked()
        {
            var item = GetLevelUpItem();
            if (item != null)
            {
                animatorRewardBubble.gameObject.SetActive(true);
                animatorRewardBubble.transform.position = transLevelUp.position;
                XUtility.PlayAnimation(animatorRewardBubble, "Open");
                InitItems(item);
            }
        }

        private void InitItems(DailyMission.Types.Stage item)
        {
            for (int i = transBubbleGroup.childCount - 1; i > 1; i--)
            {
                GameObject.DestroyImmediate(transBubbleGroup.GetChild(i).gameObject);
            }
            XItemUtility.InitItemsUI(transBubbleGroup, item.Items, transBubbleGroup.GetChild(1));
        }

        private DailyMission.Types.Stage GetLevelUpItem()
        {
            return Client.Get<DailyMissionController>().GetStateItem(0);
        }

        private bool CanShowLevelUp()
        {
            var stage = Client.Get<DailyMissionController>().GetStateItem(0);
            return stage.Point > Client.Get<DailyMissionController>().StatePoint;
        }
        private bool CanShowDiamond()
        {
            var stage = Client.Get<DailyMissionController>().GetStateItem(1);
            return stage.Point > Client.Get<DailyMissionController>().StatePoint;
        }

        private DailyMission.Types.Stage GetDiamondItem()
        {
            return Client.Get<DailyMissionController>().GetStateItem(1);
        }

        [ComponentBinder("ProgressGroup/BoxGroup/BoxCell3/Root/CloseState")]
        private void OnBtnDiamondClicked()
        {
            var item = GetDiamondItem();
            if (item != null)
            {
                animatorRewardBubble.gameObject.SetActive(true);
                animatorRewardBubble.transform.position = transDiamond.position;
                XUtility.PlayAnimation(animatorRewardBubble, "Open");
                InitItems(item);
            }
        }

        [ComponentBinder("TopGroup/InformationButton")]
        private void OnBtnInformationClick()
        {
            PopupStack.ShowPopup<DailyMissionHelp>();
        }

        public void ModifyMissionGroupSortingOrder(bool toTop)
        {
            if (toTop)
            {
                canvasProgressGroup.overrideSorting = true;
                canvasProgressGroup.sortingOrder = 5;
                canvasProgressGroup.sortingLayerID = SortingLayer.NameToID("SystemPopup");
                return;
            }

            canvasProgressGroup.overrideSorting = false;
            canvasProgressGroup.sortingOrder = 1;
            canvasProgressGroup.sortingLayerID = SortingLayer.NameToID("UI");
        }

        public void ClaimMission(int index, bool isHonor, Action callback)
        {
            TurnToLastPage(index == 2);
            Client.Get<DailyMissionController>().ClaimMission(index, isHonor, async () =>
            {
                if (isHonor)
                {
                    BiManagerGameModule.Instance.SendGameEvent(BiEventFortuneX.Types.GameEventType.GameEventDailyMissionHonorCollect);
                    var popup = await PopupStack.ShowPopup<DailyMissionRewardHonor>();
                    popup.Init(_mainViewController);
                    popup.InitializeReward(Client.Get<DailyMissionController>().GetMissionReward());
                    popup.SubscribeCloseAction(callback);
                }
                else
                {
                    BIRecordNormalMission(index);
                    await _mainViewController.LoadNormalReward();
                    var popup = _mainViewController.GetNormalRewardPopup();
                    popup.Init(_mainViewController);
                    popup.InitializeReward(Client.Get<DailyMissionController>().GetMissionReward());
                    popup.SubscribeCloseAction(callback);
                    popup.ShowNormalReward();
                    listDailyMissionNormal[index].FlyGiftBox(popup.GetTargetGiftBoxWorldPos(), viewController);
                }
            });
        }
        public void CompeleteNormalMission(int index, Action callback)
        {
            TurnToLastPage(index == 2);
            Client.Get<DailyMissionController>().CompleteMissionNow(index, false, async () =>
            {
                BIRecordNormalMission(index);
                await _mainViewController.LoadNormalReward();
                var popup = _mainViewController.GetNormalRewardPopup();
                popup.Init(_mainViewController);
                popup.InitializeReward(Client.Get<DailyMissionController>().GetMissionReward(), Client.Get<DailyMissionController>().GetNormalMission(index));
                popup.SubscribeCloseAction(() =>
                {
                    callback?.Invoke();
                    EventBus.Dispatch(new EventRefreshUserProfile());
                });
                popup.ShowNormalReward();
                listDailyMissionNormal[index].FlyGiftBox(popup.GetTargetGiftBoxWorldPos(), viewController);
            });
        }

        private void BIRecordNormalMission(int index)
        {
            if (index == 0)
            {
                BiManagerGameModule.Instance.SendGameEvent(BiEventFortuneX.Types.GameEventType.GameEventDailyMissionNormalCollect1);
            }
            if (index == 1)
            {
                BiManagerGameModule.Instance.SendGameEvent(BiEventFortuneX.Types.GameEventType.GameEventDailyMissionNormalCollect2);
            }
            if (index == 2)
            {
                BiManagerGameModule.Instance.SendGameEvent(BiEventFortuneX.Types.GameEventType.GameEventDailyMissionNormalCollect3);
            }
        }
    }

    public class DailyMissionPageViewController : ViewController<DailyMissionPage>
    {
        public override void OnViewDidLoad()
        {
            base.OnViewDidLoad();
            EnableUpdate(1);
        }

        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();
            SubscribeEvent<EventDailyMissionUpdate>(OnDailyMissionUpdate);
            SubscribeEvent<Event_Activity_Valentine2022_ReceiveMainPageInfo>(OnEvent_Activity_Valentine2022_ReceiveMainPageInfo);
        }

        private void OnEvent_Activity_Valentine2022_ReceiveMainPageInfo(Event_Activity_Valentine2022_ReceiveMainPageInfo obj)
        {
            view.RefreshUI();
        }

        private void OnDailyMissionUpdate(EventDailyMissionUpdate evt)
        {
            view.RefreshUI();
        }
        public override void Update()
        {
            base.Update();
            view._dailyMissionHonor.Update();
            view.txtStageTimeLeft.text = Client.Get<DailyMissionController>().GetStageTimeLeft();
        }
    }
}