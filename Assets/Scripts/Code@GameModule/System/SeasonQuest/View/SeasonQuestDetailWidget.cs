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
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.U2D;
using UnityEngine.UI;
namespace GameModule
{

    public class SeasonQuestBufferView : View<SeasonQuestBufferViewController>
    {
        [ComponentBinder("SpeedUpCell")] 
        public Transform speedUpCell;
        
        [ComponentBinder("PassCell")] 
        public Transform passCell;
        
        [ComponentBinder("ContentGroupR")] 
        protected Transform bubbleTipTransformR;
        
        [ComponentBinder("ContentGroupL")] 
        public Transform bubbleTipTransformL;  
         
        [ComponentBinder("SpeedUpCell/TimerGroup")] 
        public Transform timerGroup;
        
        [ComponentBinder("SpeedUpCell/TimerGroup/TimerText")] 
        public TMP_Text timerText;
        
        private bool isShowingTip = false;
 
        protected override void OnViewSetUpped()
        {
            speedUpCell.gameObject.SetActive(true);
            passCell.gameObject.SetActive(false);
            
            timerGroup.gameObject.SetActive(false);
            
            base.OnViewSetUpped();
            
            bubbleTipTransformL.gameObject.SetActive(false);
            bubbleTipTransformR.gameObject.SetActive(false);
            
            var animator = bubbleTipTransformL.GetComponent<Animator>();
            animator.keepAnimatorControllerStateOnDisable = true;
            
            animator = bubbleTipTransformR.GetComponent<Animator>();
            animator.keepAnimatorControllerStateOnDisable = true;
        }
        
        public void ShowSpeedUpBubbleTip()
        {
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
        
        public void HideSpeedUpBubbleTip()
        {
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

    public class SeasonQuestBufferViewController:ViewController<SeasonQuestBufferView>
    {
        private bool buffSwitchEnabled = false;

        protected CancelableCallback _doSwitchCallback;
        
        protected override void SubscribeEvents()
        {
            view.speedUpCell.GetComponent<Button>().onClick.AddListener(OnSpeedUpCellClicked);
            view.passCell.GetComponent<Button>().onClick.AddListener(OnQuestPassCellClicked);
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
                PopupStack.ShowPopupNoWait<SeasonQuestSpeedUpPopup>(argument:$"{machineScene.viewController.GetMachineContext().assetProvider.MachineId}");
            }
        }
        public void OnQuestPassCellClicked()
        {
            var machineScene = ViewManager.Instance.GetSceneView<MachineScene>();
            if (machineScene != null && !machineScene.viewController.IsInSpinning())
            {
                PopupStack.ShowPopupNoWait<SeasonQuestPassPaymentPopup>(argument:("PlayerClick",$"{machineScene.viewController.GetMachineContext().assetProvider.MachineId}"));
            }
        }

        public void EnableBuffSwitch(bool enabled)
        {
            if (buffSwitchEnabled != enabled)
            {
                buffSwitchEnabled = enabled;
                if (enabled)
                    DoSwitch();
            }
        }

        protected void DoSwitch()
        {
            if (_doSwitchCallback != null)
            {
                _doSwitchCallback.CancelCallback();
            }
            
            if (buffSwitchEnabled)
            {
                if (view.passCell.gameObject.activeSelf)
                {
                    view.passCell.gameObject.SetActive(false);
                    view.speedUpCell.gameObject.SetActive(true);
                }
                else
                {
                    view.passCell.gameObject.SetActive(true);
                    view.speedUpCell.gameObject.SetActive(false);
                }
                
                _doSwitchCallback = WaitForSeconds(5, DoSwitch);
            }
        }
        
        public void CheckBoostBuffUpdate()
        {
            var boosBuff = Client.Get<BuffController>().GetBuff<SeasonQuestStarBoostBuff>();

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
            var boosBuff = Client.Get<BuffController>().GetBuff<SeasonQuestStarBoostBuff>();
            if (boosBuff != null && boosBuff.GetBuffLeftTimeInSecond() > 0)
            {
                view.timerGroup.gameObject.SetActive(true);
                view.timerText.text = XUtility.GetTimeText(boosBuff.GetBuffLeftTimeInSecond());
                
                EnableUpdate(2);
            }
        }
        public override void Update()
        {
            var boosBuff = Client.Get<BuffController>().GetBuff<SeasonQuestStarBoostBuff>();

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
    
    [AssetAddress("UIQuestSeasonOneListInSlot")]
    public class SeasonQuestDetailWidget : SystemSecondLevelWidgetView<SeasonQuestDetailWidgetViewController>
    {
        [ComponentBinder("Content")] public Transform content;

        [ComponentBinder("MissionCell")] public Transform missionCellTemplate;
        [ComponentBinder("BuffCell")] public Transform buffCell;
        [ComponentBinder("Root/QuestWdiget")] public Transform activityCollectTarget;

       // [ComponentBinder("TimerText")] public TextMeshProUGUI timerText;

        [ComponentBinder("HideButton")] public Button hideWidgetButton;

        public List<QuestMissionWidgetView> questWidgetViews;
        
        public SeasonQuestBufferView questBufferView;
        
        public bool tipIsShowing = false;

        public SeasonQuestDetailWidget(string address)
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
            questBufferView = AddChild<SeasonQuestBufferView>(buffCell);
        }
        
        public override void AttachDragDropEvent(SystemWidgetContainerViewController inWidgetContainerViewController)
        {
            var dropEventCustomHandler = hideWidgetButton.gameObject.AddComponent<DragDropEventCustomHandler>();
            dropEventCustomHandler.BindingDragAction(inWidgetContainerViewController.OnDrag);
            dropEventCustomHandler.BindingEndDragAction(inWidgetContainerViewController.OnEndDrag);
            dropEventCustomHandler.BindingBeginDragAction(inWidgetContainerViewController.OnBeginDrag);
             
            dropEventCustomHandler = questBufferView.speedUpCell.gameObject.AddComponent<DragDropEventCustomHandler>();
            dropEventCustomHandler.BindingDragAction(inWidgetContainerViewController.OnDrag);
            dropEventCustomHandler.BindingEndDragAction(inWidgetContainerViewController.OnEndDrag);
            dropEventCustomHandler.BindingBeginDragAction(inWidgetContainerViewController.OnBeginDrag);
            
            widgetContainerViewController = inWidgetContainerViewController;
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

    public class SeasonQuestDetailWidgetViewController : ViewController<SeasonQuestDetailWidget>
    {
        private AssetReference _assetReference;

        private SeasonQuestController _seasonQuestController;
        public override void OnViewDidLoad()
        {
            base.OnViewDidLoad();
            
            view.hideWidgetButton.onClick.AddListener(HideWidgetButtonClicked);

            SetUpMissionUI();

           var selectEventCustomHandler =  view.transform.gameObject.AddComponent<SelectEventCustomHandler>();
           
           selectEventCustomHandler.BindingDeselectedAction(OnTipDeselect);
        }
 
        public override void OnViewEnabled()
        {
            _seasonQuestController = Client.Get<SeasonQuestController>();
            base.OnViewEnabled();
        }
        
        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();
            SubscribeEvent<EventUpdateSeasonQuestWidgetProgress>(UpdateUpdateQuestWidgetProgress);
            SubscribeEvent<EventEnableSeasonQuestBuffSwitchProgress>(OnEventEnableSeasonQuestWidgetSwitchProgress);
            SubscribeEvent<EventSeasonQuestSeasonFinish>(OnQuestFinished);
        }
        
        protected void OnQuestFinished(EventSeasonQuestSeasonFinish evt)
        {
            view.HideMissionDescTextTip();
            view.widgetContainerViewController.DetachSecondLevelWidget();
        }
        protected void UpdateUpdateQuestWidgetProgress(EventUpdateSeasonQuestWidgetProgress evt)
        {
            var currentMission = _seasonQuestController.GetCurrentMission();
            
            view.UpdateMissionProgress(currentMission, true);
        }
        
        protected void OnEventEnableSeasonQuestWidgetSwitchProgress(EventEnableSeasonQuestBuffSwitchProgress evt)
        {
            view.questBufferView.viewController.EnableBuffSwitch(true);
        }
        
        public async void OnTipDeselect(BaseEventData baseEventData)
        {
           await WaitNFrame(1);
           
           if (EventSystem.current.currentSelectedGameObject == null
               || !EventSystem.current.currentSelectedGameObject.transform.IsChildOf(view.transform))
           {
               view.HideMissionDescTextTip();
               view.questBufferView.HideSpeedUpBubbleTip();
           }
        }
        public async void OnShowWidget()
        {
            await WaitForSeconds(0.5f);
            view.ShowMissionDescTextTip();
            
            view.questBufferView.ShowSpeedUpBubbleTip();
            
            WaitForSeconds(3, () =>
            {
                view.HideMissionDescTextTip();
                view.questBufferView.HideSpeedUpBubbleTip();
            });
        }
        protected void SetUpMissionUI()
        {
            var missions = Client.Get<SeasonQuestController>().GetCurrentMission();

            var missionCount = missions.Count;

          //  var spriteAtlas = _assetReference.GetAsset<SpriteAtlas>();

            for (var i = 0; i < missionCount; i++)
            {
                view.AddMissionCell(missions[i], i == missionCount-1, null, OnMissionButtonClicked);
            }
            
            view.questBufferView.transform.SetAsLastSibling();
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
                view.questBufferView.HideSpeedUpBubbleTip();
                view.widgetContainerViewController.DetachSecondLevelWidget();
            }
        }
    }
}