// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2022/01/17/12:12
// Ver : 1.0.0
// Description : SeasonQuestWidget.cs
// ChangeLog :
// **********************************************

using System;
using DG.Tweening;
using DragonU3DSDK.Network.API.ILProtocol;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameModule
{
    [AssetAddress("UIQuestSeasonOneWidget")]
    public class SeasonQuestWidget : SystemWidgetView<SeasonQuestWidgetViewController>
    {
        [ComponentBinder("ProgressBar")] public Slider progressBar;

        [ComponentBinder("ProgressText")] public TextMeshProUGUI progressText;

        public SeasonQuestWidget(string address)
            : base(address)
        {
        }

        public override void OnWidgetClicked(SystemWidgetContainerViewController widgetContainerViewController)
        {
            XDebug.Log("WidgetClicked");
            SoundController.PlayButtonClick();
            viewController.OnWidgetClicked(widgetContainerViewController);
        }
    }

    public class SeasonQuestWidgetViewController : ViewController<SeasonQuestWidget>
    {
        protected bool inQuestMission;

        protected bool questDataUpdated;

        protected SeasonQuestController _seaonQuestController;

        protected int spinRoundCount = 0;

        protected int baseSpinCount = 50;
        
        protected int intervalSpinCount = 30;
        protected int lastShowPassPaymentPopup = 0;

        protected bool spinRoundEndShowDeal = false;
        protected override void SubscribeEvents()
        {
            SubscribeEvent<EventEnterMachineScene>(OnEnterMachineScene);
            SubscribeEvent<EventSeasonQuestDataUpdated>((evt) => questDataUpdated = true);

            SubscribeEvent<EventSpinRoundEnd>(OnSpinRoundEnd, HandlerPriorityWhenSpinEnd.Quest);
            SubscribeEvent<EventSubRoundEnd>(OnSpinSubRoundFinished);
            SubscribeEvent<EventSeasonQuestSeasonFinish>(OnQuestFinished);
            SubscribeEvent<EventCheckNeedDisableGameWelcome>(OnCheckNeedDisableGameWelcome, 0);
            SubscribeEvent<EventSeasonQuestClaimSuccess>(OnQuestClaimSuccess);
            SubscribeEvent<EventPreNoticeLevelChanged>(OnQuestClaimSuccess);
           
            SubscribeEvent<EventSeasonQuestCheckQuestFinish>(OnEventCheckQuestFinish);
        }

        protected void OnEventCheckQuestFinish(EventSeasonQuestCheckQuestFinish evt)
        {
            var currentQuest = _seaonQuestController.GetCurrentQuest();

            if (currentQuest.Collectable)
            {
                EventBus.Dispatch(new EventStopAutoSpin());
                PopupStack.ShowPopupNoWait<SeasonQuestSlotFinishPopup>();
            }
        }
        
        protected void OnQuestClaimSuccess(EventSeasonQuestClaimSuccess questClaimSuccess)
        {
            UpdateProgressText(false);
        }

        protected void OnQuestClaimSuccess(EventPreNoticeLevelChanged evt)
        {
            spinRoundEndShowDeal = evt.levelUpInfo.ShowDeal > 0;
            
            var vipItem = XItemUtility.GetItem(evt.levelUpInfo.RewardItems, Item.Types.Type.VipPoints);
            if (vipItem != null && vipItem.VipPoints.LevelUpRewardItems != null)
            {
                spinRoundEndShowDeal = true;
            }
        }
        
        protected void OnCheckNeedDisableGameWelcome(Action handleEndCallback,
            EventCheckNeedDisableGameWelcome evt,
            IEventHandlerScheduler scheduler)
        {
            if (!inQuestMission)
            {
                handleEndCallback.Invoke();
            }
        }

        public void OnQuestFinished(EventSeasonQuestSeasonFinish eventQuestFinished)
        {
            view.systemWidgetContainerViewController.RemoveSystemWidgetView(view);
        }

        public override void OnViewEnabled()
        {
            base.OnViewEnabled();

            _seaonQuestController = Client.Get<SeasonQuestController>();

            UpdateProgressText();

            var countDown = _seaonQuestController.GetQuestCountDown();
            if (countDown > 0)
            {
                WaitForSeconds(countDown, () =>
                {
                    var machineScene = ViewManager.Instance.GetSceneView<MachineScene>();
                    if (machineScene != null && !machineScene.viewController.IsInSpinning())
                    {
                        if (PopupStack.GetPopup<SeasonQuestPopup>() == null)
                            CheckAndProcessQuestTimeOut();
                    }
                });
            }
            else
            {
                CheckAndProcessQuestTimeOut();
            }
        }

        protected void OnSpinSubRoundFinished(EventSubRoundEnd evt)
        {
            if (questDataUpdated)
            {
                UpdateProgressText(true);

                EventBus.Dispatch(new EventUpdateSeasonQuestWidgetProgress());
            }
        }

        protected void UpdateProgressText(bool showAnimation = false)
        {
            var missions = _seaonQuestController.GetCurrentMission();

            showAnimation = showAnimation && view.transform.gameObject.activeInHierarchy;

            var progress = 1.0f;

            if (missions != null && missions.Count > 0)
            {
                var rate = 1.0f / missions.Count;

                progress = 0;

                for (var i = 0; i < missions.Count; i++)
                {
                    progress += missions[i].GetProgress() * rate;
                }
            }
            else
            {
                view.progressBar.gameObject.SetActive(false);
                return;
                
            }
            view.progressBar.gameObject.SetActive(true);
            if (!showAnimation || !view.transform.gameObject.activeInHierarchy)
            {
                view.progressBar.gameObject.SetActive(true);
                view.progressBar.value = progress;
                view.progressText.text = Math.Floor(progress * 100) + "%";
            }
            else
            {
                view.progressBar.DOValue(progress, 0.5f).OnUpdate(() =>
                {
                    view.progressText.text = Math.Floor(view.progressBar.value * 100) + "%";
                }).OnComplete(() => { view.progressText.text = Math.Floor(progress * 100) + "%"; });
            }
        }

        public bool CheckAndProcessQuestTimeOut()
        {
            XDebug.Log("CheckAndProcessQuestTimeOut");

            if (_seaonQuestController.GetQuestCountDown() <= 0)
            {
                XDebug.Log("CheckAndProcessQuestTimeOut:TimeOut");

                if (inQuestMission)
                {
                    BlockLevel blockLevel = BlockLevel.DefaultLevel;
                  
                    if (! PopupStack.HasPopup(typeof(SeasonQuestPopup)))
                    {
                        EventBus.Dispatch(new EventEnqueuePopup(new PopupArgs(typeof(SeasonQuestOverPopup),blockLevel)));
                        EventBus.Dispatch(new EventEnqueueFencePopup(null, blockLevel));
                    }
                }

                var secondLevelWidget = view.systemWidgetContainerViewController.GetSecondLevelWidget();
                if (secondLevelWidget != null && secondLevelWidget is QuestDetailWidget)
                {
                    view.systemWidgetContainerViewController.HideSecondLevelWidget();
                }

                view.systemWidgetContainerViewController.RemoveSystemWidgetView(view);

                return true;
            }

            return false;
        }

        protected void OnSpinRoundEnd(Action handleEndCallback, EventSpinRoundEnd eventSceneSwitchEnd,
            IEventHandlerScheduler scheduler)
        {
            //如果这次Spin升级了，弹出了Deal弹版，这次Spin就不弹出
            bool canShowPassPaymentPopup = !spinRoundEndShowDeal;
            spinRoundEndShowDeal = false;
            
            if (inQuestMission)
            {
                spinRoundCount++;
                
                if (CheckAndProcessQuestTimeOut())
                {
                    return;
                }

                if (questDataUpdated)
                {
                    UpdateProgressText(true);

                    EventBus.Dispatch(new EventUpdateSeasonQuestWidgetProgress());
                }

                var currentQuest = _seaonQuestController.GetCurrentQuest();

                if (currentQuest.Collectable)
                {
                    EventBus.Dispatch(new EventStopAutoSpin());
                    PopupStack.ShowPopupNoWait<SeasonQuestSlotFinishPopup>();
                }
                else
                {
                    //满足Spin多少次，之后没有完成当前 Quest 才弹出 SeasonQuestPassPaymentPopup
                    if (spinRoundCount >= baseSpinCount && (spinRoundCount - lastShowPassPaymentPopup >= intervalSpinCount))
                    {
                        var machineScene = ViewManager.Instance.GetSceneView<MachineScene>();

                        if (machineScene != null && !machineScene.viewController.IsInAutoSpinning() && canShowPassPaymentPopup)
                        {
                            PopupStack.ShowPopupNoWait<SeasonQuestPassPaymentPopup>(argument:("AutoPopup",
                                $"{machineScene.viewController.GetMachineContext().assetProvider.MachineId}"));
                            
                            EventBus.Dispatch(new EventEnableSeasonQuestBuffSwitchProgress());
                            lastShowPassPaymentPopup = spinRoundCount;
                        }
                    }
                }
            }

            handleEndCallback?.Invoke();
        }

        protected async void OnEnterMachineScene(EventEnterMachineScene evt)
        {
            var currentQuest = Client.Get<SeasonQuestController>().GetCurrentQuest();

            if (evt.context.assetProvider.MachineId == currentQuest.GameId)
            {
                inQuestMission = true;
                var startPopup = await PopupStack.ShowPopup<SeasonQuestStartPopup>();

                startPopup.SubscribeCloseAction(ShowDetailWidget);
            }
        }

        public async void ShowDetailWidget()
        {
            var detailWidget = await View.CreateView<SeasonQuestDetailWidget>();
            view.systemWidgetContainerViewController.AttachSecondLevelWidget(detailWidget);

            if (spinRoundCount >= baseSpinCount && detailWidget != null)
            {
                detailWidget.questBufferView.viewController.EnableBuffSwitch(true);
            }
        }

        public void OnWidgetClicked(SystemWidgetContainerViewController widgetContainerViewController)
        {
            var currentQuest = Client.Get<SeasonQuestController>().GetCurrentQuest();

            if (widgetContainerViewController.machineContext.assetProvider.MachineId != currentQuest.GameId)
            {
                EventBus.Dispatch(new EventShowPopup(new PopupArgs(typeof(SeasonQuestPopup), "QuestWidget")));
            }
            else
            {
                ShowDetailWidget();
            }
        }
    }
}