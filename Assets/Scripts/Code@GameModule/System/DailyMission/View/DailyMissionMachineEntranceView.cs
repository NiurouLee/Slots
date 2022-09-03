//  **********************************************
//  Copyright(c) 2021 by com.ustar
//  All right reserved
// 
//  Author : Besure.Chen
//  Date :2021-09-26 14:40
//  Ver : 1.0.0
//  Description : DailyMissionMachineView.cs
//  ChangeLog :
//  **********************************************

using System;
using System.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameModule
{
    public class DailyMissionMachineEntranceView: View<DailyMissionMachineEntranceViewViewController>
    {
        public Animator animator;
        
        [ComponentBinder("Root/Mask")] 
        public Transform transMask;
        [ComponentBinder("Root/ProgressBar")] 
        public Slider progressBar;

        [ComponentBinder("Root/BoxGroup")]
        public Transform transNormalGroup;
        
        [ComponentBinder("Root/BoxGroupHonor")]
        public Transform transHonorGroup;
        [ComponentBinder("Root/LockState")]
        public Transform transLocked;

        [ComponentBinder("Root/SpinButton")] public Button spinButton;
        [ComponentBinder("Root/UIDailyMissionLobbyUnlockBubbelH")] public Transform transBubble;
        [ComponentBinder("Root/UIDailyMissionLobbyUnlockBubbelV")] public Transform transBubbleV;
        [ComponentBinder("Root/FG/WinText")] public TextMeshProUGUI txtSpinState;

        [ComponentBinder("Root/ProgressBar/Mask/Handle Slide Area/Handle")]
        public Transform progressHandle;
        
        public CommonTextBubbleView dailyMissionBubble;


        public DailyMissionMachineEntranceView(Transform inTransform): base(null)
        {
            transform = inTransform;
            animator = transform.GetComponent<Animator>();
            SetUpController(null);
            BindingComponent();
            OnViewSetUpped();
        }

        protected override void SetUpExtraView()
        {
            base.SetUpExtraView();
            dailyMissionBubble = AddChild<CommonTextBubbleView>(ViewManager.Instance.IsPortrait ? transBubbleV : transBubble);
            dailyMissionBubble.isStandalone = true;
        }

        public override void Destroy()
        {
            base.Destroy();
            dailyMissionBubble = null;
        }
    }

    public class DailyMissionMachineEntranceViewViewController : ViewController<DailyMissionMachineEntranceView>
    {
        private bool needShowFailed = false;
        private string strEntranceAnim;
        private DailyMissionController _dailyMissionController;
        public override void OnViewDidLoad()
        {
            base.OnViewDidLoad();
            _dailyMissionController = Client.Get<DailyMissionController>();
            SubscribeEvent<EventSubRoundEnd>(OnSubRoundEnd);
            SubscribeEvent<EventSpinSuccess>(OnSpinSuccess);
            SubscribeEvent<EventDailyMissionUpdate>(OnDailyMissionUpdate);
            SubscribeEvent<EventDailyMissionUnLock>(OnDailyMissionUnLock);
            SubscribeEvent<EventSpinRoundEnd>(OnSpinRoundEnd, HandlerPriorityWhenSpinEnd.DailyMission);
            RefreshProgressOnRound(0,true);
            view.spinButton.onClick.AddListener(OnDailyMissionClicked);
        }

        protected void OnSubRoundEnd(EventSubRoundEnd eventSceneSwitchEnd)
        {
            if (_dailyMissionController.IsLocked)
            {
                return;
            }
            RefreshProgressOnRound(0.5f);
        }

        protected async void OnSpinRoundEnd(Action handleEndCallback, EventSpinRoundEnd eventSceneSwitchEnd,
            IEventHandlerScheduler scheduler)
        {
            RefreshProgressOnRound(0.5f);
            if (_dailyMissionController.HasFinishedMission && _dailyMissionController.IsNormalMissionFinished)
            {
                view.spinButton.interactable = false;
                EventBus.Dispatch(new EventPauseMachine());
                await XUtility.WaitSeconds(0.5f,this);
                view.transMask.gameObject.SetActive(true);
                SoundController.PlaySfx("Mission_Light");
                XUtility.PlayAnimation(view.animator, "daily_automation");
                await XUtility.WaitSeconds(2f,this);
                view.transMask.gameObject.SetActive(false);
                EventBus.Dispatch(new EventShowPopup(new PopupArgs(typeof(DailyMissionMainPopup),
                    (object) new[] {"DailyMission", "MachineAuto"},handleEndCallback)));
                view.spinButton.interactable = true;
            }
            else
            {
                handleEndCallback?.Invoke();
            }
        }

        protected void OnSpinSuccess(EventSpinSuccess evt)
        {
            if (_dailyMissionController.IsLocked) return;
            RefreshProgressOnRound(0.5f,false,false);
        }

        private void OnDailyMissionUnLock(EventDailyMissionUnLock evt)
        {
            RefreshProgressOnRound(0,true);
            view.dailyMissionBubble.ShowBubble();
            view.dailyMissionBubble.SetText("Daily Mission is unlocked!");
        }

        protected void OnDailyMissionUpdate(EventDailyMissionUpdate evt)
        {
            if (_dailyMissionController.IsLocked) return;
            RefreshProgressOnRound(0,true);
        }

        private void RefreshMissionFinishState(bool needUpdate)
        {
            if (needUpdate)
            {
                view.SetTransformActive(view.transLocked, false);
                view.SetTransformActive(view.transHonorGroup, false);
                view.SetTransformActive(view.transNormalGroup, false);
                if (_dailyMissionController.IsLocked)
                {
                    strEntranceAnim = "daily_lock";
                    view.SetTransformActive(view.transLocked, true);
                    XUtility.PlayAnimation(view.animator, "daily_lock");
                    return;
                }
                view.SetTransformActive(view.transNormalGroup, true);
                if (_dailyMissionController.HasFinishedMission)
                {
                    if (strEntranceAnim != "daily_complete")
                    {
                        XUtility.PlayAnimation(view.animator, "daily_complete");   
                    }
                    strEntranceAnim = "daily_complete";
                }
                else
                {
                    if (strEntranceAnim != "daily_zhang")
                    {
                        XUtility.PlayAnimation(view.animator, "daily_zhang");   
                    }
                    strEntranceAnim = "daily_zhang";
                }

                var currentMission = _dailyMissionController.CurrentMission;
                if (currentMission.MissionType == DragonU3DSDK.Network.API.ILProtocol.Mission.Types.Type.WinXCoinsInYSpins && !currentMission.IsFinish())
                {
                    if (needShowFailed)
                    {
                        needShowFailed = false;
                        view.dailyMissionBubble.ShowBubble();
                        view.dailyMissionBubble.SetText("OH NO!<br>YOU ARE FAILED FINISHING THIS...<br>TRY AGAIN!");
                    }
                    if (currentMission.CurrentTimes == _dailyMissionController.CurrentMission.Times-1)
                    {
                        needShowFailed = true;
                    }
                }

                var needShow = currentMission != null;
                if (needShow && _dailyMissionController.HasFinishedMission && !_dailyMissionController.IsShowComplete)
                {
                    if (_dailyMissionController.HasFinishedMission && 
                        !_dailyMissionController.IsShowComplete)
                    {
                        view.dailyMissionBubble.ShowBubble();
                        view.dailyMissionBubble.SetText("Mission completed!");
                        Client.Get<DailyMissionController>().IsShowComplete = true;
                    }
                }
                view.txtSpinState.text = _dailyMissionController.HasFinishedMission ? "COLLECT" : currentMission.GetSpinButtonDes();
            }
        }

        private async void RefreshProgressOnRound(float duration, bool forceUpdate=false, bool isRoundFinish=true)
        {
            bool needUpdate = false;
            if (!_dailyMissionController.IsLocked && _dailyMissionController.CurrentMission != null)
            {
                needUpdate = (_dailyMissionController.CurrentMission.IsUpdateOnRoundFinish() ^ isRoundFinish) == false;
                if (forceUpdate || needUpdate)
                {
                    var progress = _dailyMissionController.HasFinishedMission
                        ? 1f
                        : _dailyMissionController.CurrentMission.GetProgress();
                    await UpdateContent(progress, duration);
                }   
            }
            RefreshMissionFinishState(needUpdate || forceUpdate);
        }

        private void OnDailyMissionClicked()
        {
            if (_dailyMissionController.IsLocked)
            {
                view.dailyMissionBubble.ShowBubble();
                view.dailyMissionBubble.SetText($"UNLOCK AT LEVEL {Client.Get<DailyMissionController>().UnlockLevel}");
            }
            else
            {
                EventBus.Dispatch(new EventShowPopup(new PopupArgs(typeof(DailyMissionMainPopup), (object)new []{"DailyMission","Machine"})));   
            }
        }
        
        public async Task UpdateContent(float value, float duration = 0)
        {
            await UpdateProgress(value, duration);
        }

        private async Task UpdateProgress(float value, float duration)
        {
            float scaleY = 1f;
            float scaleX = Mathf.Sqrt(Mathf.Pow(0.5f, 2) - Mathf.Pow(0.5f - value, 2)) * 2;
            if (value < 0.3f)
            {
                scaleX = Mathf.Sqrt(Mathf.Pow(0.5f, 2) - Mathf.Pow(0.5f - value, 2)) * 2;
                scaleY = scaleX;
            }
            else if(value > 0.7f)
            {
                scaleX = Mathf.Sqrt(Mathf.Pow(0.5f, 2) - Mathf.Pow(0.5f - value, 2)) * 2;
                scaleY = scaleX;
                scaleX *= 0.9f;
            }
            
            view.progressBar.DOValue(value, duration);
            view.progressHandle.DOScale(new Vector3(scaleX*0.6f, scaleY*0.4f, scaleX),duration);
            await XUtility.WaitSeconds(duration,this);
        }
    }
}