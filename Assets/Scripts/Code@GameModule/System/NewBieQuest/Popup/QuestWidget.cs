// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/11/05/19:06
// Ver : 1.0.0
// Description : QuestWidget.cs
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
    [AssetAddress("UIQuestWidget")]
    public class QuestWidget : SystemWidgetView<QuestWidgetViewController>
    {
        [ComponentBinder("ProgressBar")] 
        public Slider progressBar;   
        
        [ComponentBinder("ProgressText")] 
        public TextMeshProUGUI progressText;   
        
        [ComponentBinder("ActivationNoticeBubble")] 
        public Transform activationNoticeBubble;
        
        public QuestWidget(string address)
            : base(address)
        {
            
        }

        public override void OnWidgetClicked(SystemWidgetContainerViewController widgetContainerViewController)
        {
            XDebug.Log("WidgetClicked");
           SoundController.PlayButtonClick();
           viewController.OnWidgetClicked(widgetContainerViewController);
        }
        
        public override void SetSystemWidgetContainerViewController(SystemWidgetContainerViewController widgetContainerViewController)
        {
            base.SetSystemWidgetContainerViewController(widgetContainerViewController);
            viewController.OnWidgetAttached();
        }
    }

    public class QuestWidgetViewController : ViewController<QuestWidget>
    {
        protected bool inQuestMission;

        protected bool questDataUpdated;

        protected NewBieQuestController _newBieQuestController;
       
        protected override void SubscribeEvents()
        {
            SubscribeEvent<EventEnterMachineScene>(OnEnterMachineScene);
            SubscribeEvent<EventQuestDataUpdated>((evt) => questDataUpdated = true);

            SubscribeEvent<EventSpinRoundEnd>(OnSpinRoundEnd, HandlerPriorityWhenSpinEnd.Quest);
            SubscribeEvent<EventSubRoundEnd>(OnSpinSubRoundFinished);
            SubscribeEvent<EventQuestFinished>(OnQuestFinished);
            SubscribeEvent<EventCheckNeedDisableGameWelcome>(OnCheckNeedDisableGameWelcome,0);
            SubscribeEvent<EventQuestClaimSuccess>(OnQuestClaimSuccess);
        }


        public void OnWidgetAttached()
        {
            // if (_newBieQuestController.NeedShowUnlockTip)
            // {
            //     ShowActiveTip();
            //   
            //     _newBieQuestController.NeedShowUnlockTip = false;
            //
            //     WaitForSeconds(3, HideActiveBubbleTip);
            // }
        }

        private bool isShowingTip = false;
        
        public void ShowActiveTip()
        {
            view.activationNoticeBubble.gameObject.SetActive(true);

            var leftBubble = view.activationNoticeBubble.Find("ContentGroupL");
            var rightBubble = view.activationNoticeBubble.Find("ContentGroupR");
                
            if (view.transform.position.x > 0)
            {
                leftBubble.gameObject.SetActive(true);
                rightBubble.gameObject.SetActive(false);
                var animator = leftBubble.GetComponent<Animator>();
                animator.Play("ShowTip");
            }
            else
            {
                leftBubble.gameObject.SetActive(false);
                rightBubble.gameObject.SetActive(true);
                var animator = rightBubble.GetComponent<Animator>();
                animator.Play("ShowTip");
            }

            isShowingTip = true;
        }
        
        public void HideActiveBubbleTip()
        {
            var leftBubble = view.activationNoticeBubble.Find("ContentGroupL");
            var rightBubble = view.activationNoticeBubble.Find("ContentGroupR");
            
            if (view.transform.position.x > 0)
            {
                var animator = leftBubble.GetComponent<Animator>();
                animator.Play("HideTip");
                rightBubble.gameObject.SetActive(false);
            }
            else
            {
                var animator = rightBubble.GetComponent<Animator>();
                animator.Play("HideTip");
                leftBubble.gameObject.SetActive(false);
            }
            
            isShowingTip = false;
        }
        
        protected void OnQuestClaimSuccess(EventQuestClaimSuccess questClaimSuccess)
        {
            UpdateProgressText(false);
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
        
        public void OnQuestFinished(EventQuestFinished eventQuestFinished)
        {
            view.systemWidgetContainerViewController.RemoveSystemWidgetView(view);
        }
        
        public override void OnViewEnabled()
        {
            base.OnViewEnabled();
            
            _newBieQuestController = Client.Get<NewBieQuestController>();
             
            UpdateProgressText();
            
            var countDown = Client.Get<NewBieQuestController>().GetQuestCountDown();
            if (countDown > 0)
            {
                WaitForSeconds(countDown, () =>
                {
                    var machineScene = ViewManager.Instance.GetSceneView<MachineScene>();
                    if (machineScene != null && !machineScene.viewController.IsInSpinning())
                    {
                        if(PopupStack.GetPopup<QuestPopup>() == null)
                            CheckAndProcessQuestTimeOut();
                    }
                });

                if (!inQuestMission)
                {
                    EnableUpdate(2);
                }
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
                    
                EventBus.Dispatch(new EventUpdateQuestWidgetProgress());
            }
        }

        protected void UpdateProgressText(bool showAnimation = false)
        {
            if (!inQuestMission)
            {
                view.progressText.text = XUtility.GetTimeText(_newBieQuestController.GetQuestCountDown(),true);
                view.progressBar.value = 0;
                return;
            }
            
            var missions = _newBieQuestController.GetCurrentMission();
  
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
            
            if (!showAnimation || !view.transform.gameObject.activeInHierarchy)
            {
                view.progressBar.value = progress;
                view.progressText.text = Math.Floor(progress * 100) + "%";
            }
            else
            {
                view.progressBar.DOValue(progress, 0.5f).OnUpdate(() =>
                {
                    view.progressText.text = Math.Floor(view.progressBar.value * 100) + "%";
                }).OnComplete(() =>
                {
                    view.progressText.text = Math.Floor(progress * 100) + "%";
                });
            }
        }

        public bool CheckAndProcessQuestTimeOut()
        {
            XDebug.Log("CheckAndProcessQuestTimeOut");
           
            if (Client.Get<NewBieQuestController>().GetQuestCountDown() <= 0)
            {
                XDebug.Log("CheckAndProcessQuestTimeOut:TimeOut");
               
                if (inQuestMission)
                {
                    
                    if (!PopupStack.HasPopup(typeof(QuestPopup)))
                    {
                        EventBus.Dispatch(new EventEnqueuePopup(new PopupArgs(typeof(QuestOverPopup))));
                        EventBus.Dispatch(new EventEnqueueFencePopup(null));
                    }
                    
                    //PopupStack.ShowPopupNoWait<QuestOverPopup>();
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
            if (inQuestMission)
            {
                if (CheckAndProcessQuestTimeOut())
                {
                    return;
                }
                
                if (questDataUpdated)
                {
                    UpdateProgressText(true);
                    
                    EventBus.Dispatch(new EventUpdateQuestWidgetProgress());
                }
                 
                var currentQuest = Client.Get<NewBieQuestController>().GetCurrentQuest();

                if (currentQuest.Collectable)
                {
                    EventBus.Dispatch(new EventStopAutoSpin());
                    PopupStack.ShowPopupNoWait<QuestSlotGameFinishPopup>();
                }
            }
            handleEndCallback?.Invoke();
        }

        protected async void OnEnterMachineScene(EventEnterMachineScene evt)
        {
            var currentQuest = Client.Get<NewBieQuestController>().GetCurrentQuest();

            if (evt.context.assetProvider.MachineId == currentQuest.GameId)
            {
                inQuestMission = true;
                var startPopup = await PopupStack.ShowPopup<QuestStartPopup>();
               
                startPopup.SubscribeCloseAction(ShowDetailWidget);
            }
            else
            {
                inQuestMission = false;
            }
            
            UpdateProgressText(false);
        }

        public override void Update()
        {
            if(!inQuestMission)
                UpdateProgressText();
        }

        public async void ShowDetailWidget()
        {
            var detailWidget = await View.CreateView<QuestDetailWidget>();
            view.systemWidgetContainerViewController.AttachSecondLevelWidget(detailWidget);
            
        }

        public void OnWidgetClicked(SystemWidgetContainerViewController widgetContainerViewController)
        {
            var currentQuest = Client.Get<NewBieQuestController>().GetCurrentQuest();

            if (widgetContainerViewController.machineContext.assetProvider.MachineId != currentQuest.GameId)
            {
                EventBus.Dispatch(new EventShowPopup(new PopupArgs(typeof(QuestPopup), "QuestWidget")));
            }
            else
            {
                ShowDetailWidget();
            }
        }
    }
}