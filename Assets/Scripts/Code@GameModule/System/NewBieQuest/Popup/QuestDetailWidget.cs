// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/11/09/11:46
// Ver : 1.0.0
// Description : QuestDetailWidget.cs
// ChangeLog :
// **********************************************

using System;
using System.Collections.Generic;
using DG.Tweening;
using DragonU3DSDK.Network.API.ILProtocol;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.U2D;
using UnityEngine.UI;
using Event = Spine.Event;

namespace GameModule
{
    public class QuestBufferView : View<QuestBufferViewController>
    {
        [ComponentBinder("SpeedUpCell")] 
        public Transform speedUpCell;
         
      
        [ComponentBinder("SpeedUpCell/TimerGroup")] 
        public Transform timerGroup;
        
        [ComponentBinder("SpeedUpCell/TimerGroup/TimerText")] 
        public TMP_Text timerText;
        
        private bool isShowingTip = false;
 
        protected override void OnViewSetUpped()
        {
            speedUpCell.gameObject.SetActive(true);
            
            timerGroup.gameObject.SetActive(false);
            
            base.OnViewSetUpped();
        }
    }
    
      public class QuestBufferViewController:ViewController<QuestBufferView>
    {
        private bool buffSwitchEnabled = false;

        protected CancelableCallback _doSwitchCallback;
        
        protected override void SubscribeEvents()
        {
            view.speedUpCell.GetComponent<Button>().onClick.AddListener(OnSpeedUpCellClicked);
            base.SubscribeEvents();
            
            SubscribeEvent<EventBuffDataUpdated>(OnBuffDataUpdated);
        }

        public override void OnViewEnabled()
        {
            base.OnViewEnabled();
            
            CheckBoostBuffUpdate();
        }
        
        protected void OnBuffDataUpdated(EventBuffDataUpdated evt)
        {
            CheckBoostBuffUpdate();
        }

        public void OnSpeedUpCellClicked()
        {
            var machineScene = ViewManager.Instance.GetSceneView<MachineScene>();
            if (machineScene != null && !machineScene.viewController.IsInSpinning())
            {
                PopupStack.ShowPopupNoWait<QuestPayPopup>(argument:$"{machineScene.viewController.GetMachineContext().assetProvider.MachineId}");
            }
        }
         
        public void CheckBoostBuffUpdate()
        {
            var boosBuff = Client.Get<BuffController>().GetBuff<NewbieQuestBoostBuff>();

            if (boosBuff != null && boosBuff.GetBuffLeftTimeInSecond() > 0)
            {
                if (!updateEnabled)
                {
                    EnableBoostBuff();
                }
            }
        }
        
        public void EnableBoostBuff()
        {
            var boosBuff = Client.Get<BuffController>().GetBuff<NewbieQuestBoostBuff>();
            if (boosBuff != null && boosBuff.GetBuffLeftTimeInSecond() > 0)
            {
                view.timerGroup.gameObject.SetActive(true);
                view.timerText.text = XUtility.GetTimeText(boosBuff.GetBuffLeftTimeInSecond());
                
                EnableUpdate(2);
            }
        }
        public override void Update()
        {
            var boosBuff = Client.Get<BuffController>().GetBuff<NewbieQuestBoostBuff>();

            if (boosBuff != null && boosBuff.GetBuffLeftTimeInSecond() > 0)
            {
                view.timerText.text = XUtility.GetTimeText(boosBuff.GetBuffLeftTimeInSecond());
            }
            else
            {
                view.timerGroup.gameObject.SetActive(false);
                DisableUpdate();
            }
        }
    }

    
    public class QuestMissionWidgetView : View
    {
        [ComponentBinder("TargetIcon")] 
        protected Image targetIcon;
        
        [ComponentBinder("ProgressBar")] 
        protected Slider progressBar;  
        
        [ComponentBinder("ProgressText")] 
        protected TextMeshProUGUI progressText;  
        
        [ComponentBinder("FinishState")] 
        protected Transform finishState; 
        
        [ComponentBinder("ProgressState")] 
        protected Transform progressState;

        [ComponentBinder("ContentGroupR")] 
        protected Transform bubbleTipTransformR;
        
        [ComponentBinder("ContentGroupL")] 
        protected Transform bubbleTipTransformL;
        
        [ComponentBinder("DescriptionText")] 
        protected TextMeshProUGUI descriptionText;  
        
        [ComponentBinder("DescriptionTextL")] 
        protected TextMeshProUGUI descriptionTextL;

        protected Button missionButton;

        private MissionController _missionController;
        private Action _missionButtonAction;

        private bool isShowingTip = false;
        
        public QuestMissionWidgetView()
        {
            
        }

        protected override void SetUpExtraView()
        {
            base.SetUpExtraView();
            missionButton = transform.GetComponent<Button>();
            missionButton.onClick.AddListener(OnMissionButtonClicked);
            
            var animator = bubbleTipTransformL.GetComponent<Animator>();
            animator.keepAnimatorControllerStateOnDisable = true;
            
            animator = bubbleTipTransformR.GetComponent<Animator>();
            animator.keepAnimatorControllerStateOnDisable = true;
        }

        protected void OnMissionButtonClicked()
        {
            if(!_missionController.IsFinish())
                _missionButtonAction?.Invoke();
        }

        public void UpdateMissionState(MissionController missionController, bool showAnimation = false)
        {
            _missionController = missionController;

            if (!showAnimation || !transform.gameObject.activeInHierarchy)
            {
                if (missionController.IsFinish())
                {
                    finishState.gameObject.SetActive(true);
                    progressState.gameObject.SetActive(false);
                }
                else
                {
                    progressState.gameObject.SetActive(true);
                    finishState.gameObject.SetActive(false);
                    progressBar.value = missionController.GetProgress();
                    progressText.text = missionController.GetProgressDescText();
                }
            } 
            else
            {
                if (missionController.IsFinish())
                {
                    if(!missionController.ProgressIsPercentFormat())
                        progressText.text = missionController.GetProgressDescText();
                    
                    progressBar.DOValue(1, 0.5f).OnComplete(() =>
                    {
                        finishState.gameObject.SetActive(true);
                        progressState.gameObject.SetActive(false);
                        if (missionController.ProgressIsPercentFormat())
                        {
                            progressText.text = Math.Floor(progressBar.value * 100) + "%";
                        }
                    });
                }
                else
                {
                    progressState.gameObject.SetActive(true);
                    finishState.gameObject.SetActive(false);
                    
                    if(!missionController.ProgressIsPercentFormat())
                        progressText.text = missionController.GetProgressDescText();
                    
                    progressBar.DOValue(missionController.GetProgress(), 0.5f).OnComplete(() =>
                    {
                        if(missionController.ProgressIsPercentFormat())
                            progressText.text = Math.Floor(progressBar.value * 100) + "%";
                    });
                }
            }
        }

        public void UpdateMissionState(MissionController missionController, SpriteAtlas missionIconAtlas, Action missionButtonAction)
        {
            UpdateMissionState(missionController);
          
            _missionButtonAction = missionButtonAction;

            if(targetIcon != null && missionIconAtlas != null)
                targetIcon.sprite =  missionIconAtlas.GetSprite(missionController.GetMissionUI());
            
            descriptionText.text = missionController.GetContentDescText();
            descriptionTextL.text = descriptionText.text;
            
            bubbleTipTransformL.gameObject.SetActive(false);
            bubbleTipTransformR.gameObject.SetActive(false);
        }

        public void ShowMissionBubbleTip()
        {
            if(_missionController.IsFinish())
                return;
                
            if (transform.position.x > 0)
            {
                bubbleTipTransformL.gameObject.SetActive(true);
                var animator = bubbleTipTransformL.GetComponent<Animator>();
                animator.Play("ShowTip");
                bubbleTipTransformR.gameObject.SetActive(false);
            }
            else
            {
                bubbleTipTransformR.gameObject.SetActive(true);
                var animator = bubbleTipTransformR.GetComponent<Animator>();
                animator.Play("ShowTip");
                bubbleTipTransformL.gameObject.SetActive(false);
            }

            isShowingTip = true;
        }
        
        public void HideMissionBubbleTip()
        {
            if(_missionController.IsFinish() && !isShowingTip)
                return;
            
            if (transform.position.x > 0)
            {
                var animator = bubbleTipTransformL.GetComponent<Animator>();
                animator.Play("HideTip");
                bubbleTipTransformR.gameObject.SetActive(false);
            }
            else
            {
                var animator = bubbleTipTransformR.GetComponent<Animator>();
                animator.Play("HideTip");
                bubbleTipTransformL.gameObject.SetActive(false);
            }

            isShowingTip = false;
        }
    }
    
    [AssetAddress("UIQuestListInSlot")]
    public class QuestDetailWidget : SystemSecondLevelWidgetView<QuestDetailWidgetViewController>
    {
        [ComponentBinder("Content")] public Transform content;
        
        [ComponentBinder("BuffCell")] public Transform buffCell;

        [ComponentBinder("MissionCell")] public Transform missionCellTemplate;

        [ComponentBinder("Root/QuestWdiget")] private Transform activityCollectTarget;

        [ComponentBinder("Root/QuestWdiget/Timer/TimerText")] public TextMeshProUGUI timerText;

        [ComponentBinder("HideButton")] public Button hideWidgetButton;

        public List<QuestMissionWidgetView> questWidgetViews;
        public QuestBufferView questBufferView;

        public bool tipIsShowing = false;

        public QuestDetailWidget(string address)
        :base(address)
        {
            
        }

        public override Transform GetActivityCollectTargetTransform()
        {
            return activityCollectTarget;
        }

        protected override void SetUpExtraView()
        {
            base.SetUpExtraView();
            questWidgetViews = new List<QuestMissionWidgetView>();
            questBufferView = AddChild<QuestBufferView>(buffCell);
        }

        public override void AttachDragDropEvent(SystemWidgetContainerViewController inWidgetContainerViewController)
        {
            var dropEventCustomHandler = hideWidgetButton.gameObject.AddComponent<DragDropEventCustomHandler>();
            dropEventCustomHandler.BindingDragAction(inWidgetContainerViewController.OnDrag);
            dropEventCustomHandler.BindingEndDragAction(inWidgetContainerViewController.OnEndDrag);
            dropEventCustomHandler.BindingBeginDragAction(inWidgetContainerViewController.OnBeginDrag);
            widgetContainerViewController = inWidgetContainerViewController;
            
            dropEventCustomHandler = questBufferView.speedUpCell.gameObject.AddComponent<DragDropEventCustomHandler>();
            dropEventCustomHandler.BindingDragAction(inWidgetContainerViewController.OnDrag);
            dropEventCustomHandler.BindingEndDragAction(inWidgetContainerViewController.OnEndDrag);
            dropEventCustomHandler.BindingBeginDragAction(inWidgetContainerViewController.OnBeginDrag);
        }

        public void AddMissionCell(MissionController mission, bool lastQuest, SpriteAtlas questIconAtlas, Action questClickAction)
        {
            var questCell = missionCellTemplate.gameObject;
          
            if (!lastQuest)
            {
                questCell = GameObject.Instantiate(missionCellTemplate.gameObject, content);
            }

            var widgetView = AddChild<QuestMissionWidgetView>(questCell.transform);
            widgetView.transform.SetAsLastSibling();
           
            questWidgetViews.Add(widgetView);
            
            questBufferView.transform.SetAsLastSibling();
            
            widgetView.UpdateMissionState(mission, questIconAtlas, questClickAction);
        }

        public void ShowMissionDescTextTip()
        {
            for (var i = 0; i < questWidgetViews.Count; i++)
            {
                questWidgetViews[i].ShowMissionBubbleTip();
            }

            tipIsShowing = true;
        }
        
        public void HideMissionDescTextTip()
        {
            if (!tipIsShowing)
                return;
            
            for (var i = 0; i < questWidgetViews.Count; i++)
            {
                questWidgetViews[i].HideMissionBubbleTip();
            }

            tipIsShowing = false;
        }
        
        public override void ShowWidget()
        {
            Show();
            viewController.OnShowWidget();
        }

        public void UpdateMissionProgress(List<MissionController> missionControllers, bool showAnimation = false)
        {
            for(var i = 0; i < questWidgetViews.Count; i++)
                questWidgetViews[i].UpdateMissionState(missionControllers[i], showAnimation);
        }
    }

    public class QuestDetailWidgetViewController : ViewController<QuestDetailWidget>
    {
        private AssetReference _assetReference;

        private NewBieQuestController _newBieQuestController;
        public override void OnViewDidLoad()
        {
            base.OnViewDidLoad();
            
            view.hideWidgetButton.onClick.AddListener(HideWidgetButtonClicked);

            SetUpMissionUI();

           var selectEventCustomHandler =  view.transform.gameObject.AddComponent<SelectEventCustomHandler>();
           
           selectEventCustomHandler.BindingDeselectedAction(OnTipDeselect);
        }

        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();
            SubscribeEvent<EventUpdateQuestWidgetProgress>(UpdateUpdateQuestWidgetProgress);
            SubscribeEvent<EventQuestFinished>(OnQuestFinished);
        }

        protected void OnQuestFinished(EventQuestFinished evt)
        {
            view.HideMissionDescTextTip();
            view.widgetContainerViewController.DetachSecondLevelWidget();
        }
        protected void UpdateUpdateQuestWidgetProgress(EventUpdateQuestWidgetProgress evt)
        {
            var currentMission = _newBieQuestController.GetCurrentMission();
            
            view.UpdateMissionProgress(currentMission, true);
        }

        public async void OnTipDeselect(BaseEventData baseEventData)
        {
           await WaitNFrame(1);
           
           if (EventSystem.current.currentSelectedGameObject == null
               || !EventSystem.current.currentSelectedGameObject.transform.IsChildOf(view.transform))
           {
               view.HideMissionDescTextTip();
           }
        }


        public async void OnShowWidget()
        {
            await WaitForSeconds(0.5f);
            view.ShowMissionDescTextTip();
            WaitForSeconds(3, () =>
            {
                view.HideMissionDescTextTip();
            });
           
        }
        protected void SetUpMissionUI()
        {
            var missions = Client.Get<NewBieQuestController>().GetCurrentMission();

            var missionCount = missions.Count;

            var spriteAtlas = _assetReference.GetAsset<SpriteAtlas>();

            for (var i = 0; i < missionCount; i++)
            {
                view.AddMissionCell(missions[i], i == missionCount-1, spriteAtlas, OnMissionButtonClicked);
            }
        }

        private CancelableCallback _hideCallback;
        public void OnMissionButtonClicked()
        {
            if (view.tipIsShowing)
            {
                view.HideMissionDescTextTip();
            }
            else
            {
                view.ShowMissionDescTextTip();

                if (_hideCallback != null)
                {
                    _hideCallback.CancelCallback();
                }
                
                _hideCallback = WaitForSeconds(3, () =>
                {
                    view.HideMissionDescTextTip();
                });
                
                EventSystem.current.SetSelectedGameObject(view.transform.gameObject);
            }
        }

        public void HideWidgetButtonClicked()
        {
            if (!view.widgetContainerViewController.IsDrag)
            {
                view.hideWidgetButton.interactable = false;
                view.HideMissionDescTextTip();
                view.widgetContainerViewController.DetachSecondLevelWidget();
            }
        }
        
        public override void OnViewEnabled()
        {
            _newBieQuestController = Client.Get<NewBieQuestController>();
          
            base.OnViewEnabled();
            EnableUpdate(2);
        }
         
        public override void Update()
        {
            view.timerText.text = XUtility.GetTimeText(_newBieQuestController.GetQuestCountDown(), true);
        }
        public override void BindingView(View inView, object inExtraData, object inExtraAsyncData = null)
        {
            base.BindingView(inView, inExtraData, inExtraAsyncData);
            _assetReference = inExtraAsyncData as AssetReference;
        }

        public override void OnViewDestroy()
        {
            base.OnViewDestroy();
            _assetReference.ReleaseOperation();
        }
    }
}