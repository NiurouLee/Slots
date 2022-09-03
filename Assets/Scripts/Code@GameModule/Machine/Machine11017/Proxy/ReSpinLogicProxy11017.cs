using System;
using System.Collections.Generic;
using System.Runtime.Remoting.Contexts;
using System.Threading.Tasks;
using UnityEngine;

namespace GameModule
{
    public class ReSpinLogicProxy11017 : ReSpinLogicProxy
    {
        protected Animator animatorZhenping;
        public ReSpinLogicProxy11017(MachineContext context) : base(context)
        {
          animatorZhenping = context.transform.Find("ZhenpingAnim").GetComponent<Animator>();
        }
        
        protected override async void HandleCustomLogic()
        {
            //当respin开始时播放转场动画
            if (reSpinState.ReSpinTriggered)
            {
                await HandleReSpinStartLogic();
            }
            if (IsInRespinProcess())
            {
                await HandleBananaGameTrigger();
            }
            if (NeedSettle())
            {
                AudioUtil.Instance.StopMusic();
                await HandleReSpinFinishPopup();
                await HandleReSpinFinishLogic();
            }
            Proceed();
        }
        protected virtual bool IsReSpinTriggered()
        {
            return reSpinState.ReSpinTriggered;
        }
        protected virtual bool IsInRespinProcess()
        {
            return IsReSpinTriggered() || reSpinState.IsInRespin || reSpinState.ReSpinNeedSettle;
        }
        
        protected virtual bool NeedSettle()
        {
            return reSpinState.ReSpinNeedSettle;
        }
        
        protected virtual string GetLinkFinishAddress()
        {
            return "UIRespinFinish" + machineContext.assetProvider.AssetsId;;
        }
        
        protected TaskCompletionSource<bool> GetWaitTask()
        {
            var task = new TaskCompletionSource<bool>();
            machineContext.AddWaitTask(task,null);
            return task;
        }
        
        private void RemoveTask(TaskCompletionSource<bool> task)
        {
            machineContext.RemoveTask(task);
        }
        
        protected void SetAndRemoveTask(TaskCompletionSource<bool> task)
        {
            if (!ReferenceEquals(task,null))
            {
                task.SetResult(true);
                RemoveTask(task);
            }
        }
        
        protected virtual float GetReSpinFinishPopupDuration()
        {
            return 2;
        }

        //开始逻辑
        protected override async Task HandleReSpinStartLogic()
        {
            await ShowStartBananaAnim();
        }

        //结束逻辑
        protected virtual async  Task HandleReSpinFinishLogic()
        {
             await ShowFinishBananaAnim();
             await machineContext.WaitSeconds(1.6f);
             bool isFree = machineContext.state.Get<FreeSpinState>().IsInFreeSpin;
             machineContext.state.Get<WheelsActiveState11017>().UpdateRunningWheelState(false,isFree);
             RestoreTriggerWheelElement();
             var superFreeGameLock11017 = machineContext.view.Get<SuperFreeGameLock11017>();
             superFreeGameLock11017.LockSuperFreeIdle(!machineContext.state.Get<BetState>().IsFeatureUnlocked(0));
             await machineContext.WaitSeconds(3.0f - 1.6f);
             await ShowFinalReSpinBigWinEffect();
        }

        //播放香蕉转场动画以及震屏动画
        protected virtual async  Task ShowStartBananaAnim()
        {
            AudioUtil.Instance.StopMusic();
            //播放大象转场动画
            HandleElephantTrigger();
            await machineContext.WaitSeconds(3.0f);
            //显示转场动画
            AudioUtil.Instance.PlayAudioFx("BonusStart_Open");
            machineContext.view.Get<FeatureGameTips11017>().FeatureTipShow();
            await machineContext.WaitSeconds(2.05f);
            machineContext.view.Get<FeatureGameTips11017>().FeatureTipHide();
            XUtility.PlayAnimation(animatorZhenping, "Link", null, machineContext);
            machineContext.view.Get<TransitionsView11017>().PlayLinkTransition();
             //进入linkwheel
            await machineContext.WaitSeconds(1.66f);
            machineContext.state.Get<WheelsActiveState11017>().UpdateRunningWheelState(true,false);
            machineContext.view.Get<LinkRemaining11017>().ShowReSpinRemaining();
        }
        
        protected virtual async  Task ShowFinishBananaAnim()
        {
            XUtility.PlayAnimation(animatorZhenping, "Link",null, machineContext);
            machineContext.view.Get<TransitionsView11017>().PlayLinkTransition();
        }

         //respin结算弹窗
        protected virtual async Task HandleReSpinFinishPopup()
        {
            if (machineContext.assetProvider.GetAsset<GameObject>(GetLinkFinishAddress()) != null)
            {
                var task = GetWaitTask();
                var finishLinkPopup = PopUpManager.Instance.ShowPopUp<ReSpinFinishPopUp>(GetLinkFinishAddress());
                AudioUtil.Instance.PlayAudioFx("BonusEnd_Open");
                if (!ReferenceEquals(finishLinkPopup, null))
                {
                    finishLinkPopup.SetPopUpCloseAction(() =>
                    {
                        SetAndRemoveTask(task);
                    });
                    finishLinkPopup.Initialize(machineContext);
                   
                    if (finishLinkPopup.IsAutoClose())
                    {
                        await machineContext.WaitSeconds(GetReSpinFinishPopupDuration());
                        finishLinkPopup.Close();
                    }
                }
                await task.Task;
            }
            await Task.CompletedTask;
        }
        
        //respin的bigwin特效
        protected virtual async Task ShowReSpinBigWinEffect()
        {
            var winState = machineContext.state.Get<WinState>();

            if (winState.winLevel >= (int)WinLevel.BigWin)
            {
                TaskCompletionSource<bool> waitTask = new TaskCompletionSource<bool>();
                machineContext.AddWaitTask(waitTask, null);
                WinEffectHelper.ShowBigWinEffect(winState.winLevel, winState.displayCurrentWin, () =>
                {
                    machineContext.RemoveTask(waitTask);
                    waitTask.SetResult(true);
                });
                await waitTask.Task;
            }
            else
            {
                await Task.CompletedTask;
            }
        }
        
        protected virtual async Task ShowFinalReSpinBigWinEffect()
        {
            var winState = machineContext.state.Get<WinState>();

            if (winState.winLevel >= (int)WinLevel.BigWin)
            {
                TaskCompletionSource<bool> waitTask = new TaskCompletionSource<bool>();
                machineContext.AddWaitTask(waitTask, null);

                WinEffectHelper.ShowBigWinEffect(winState.winLevel, machineContext.state.Get<ReSpinState>().GetRespinTotalWin(), () =>
                {
                    machineContext.RemoveTask(waitTask);
                    waitTask.SetResult(true);
                });
                await waitTask.Task;
            }
            else
            {
                await Task.CompletedTask;
            }
        }
        
        private TaskCompletionSource<bool> bananaGameWaitTask;
        protected virtual async Task HandleBananaGameTrigger()
        {
            bananaGameWaitTask = new TaskCompletionSource<bool>();
            machineContext.AddWaitTask(bananaGameWaitTask,null);
            var _extraState = machineContext.state.Get<ExtraState11017>();
            _extraState.SetEatenAniFinish(false);
            await machineContext.view.Get<FeatureGame11017>().EatingAction();
            while (!_extraState.GetEatenAniFinish())
            {
                await XUtility.WaitSeconds(1);
            }
            if (machineContext.view.Get<FeatureGame11017>().GetIsHaveEaten())
            { 
                await HandleJackpot();
                var totalWin = machineContext.state.Get<WinState11017>().totalCrystalWin; 
                AddWinChipsToControlPanel(totalWin);
                if (totalWin != 0)
                {
                    await bananaGameWaitTask.Task; 
                    machineContext.RemoveTask(bananaGameWaitTask); 
                    await ShowReSpinBigWinEffect();
                }
            }
        }

        protected virtual async Task HandleJackpot()
        {
            var runningWheel = machineContext.state.Get<WheelsActiveState>().GetRunningWheel()[0];
            var winLines = runningWheel.wheelState.GetJackpotWinLines(); 
            for (int i = 0; i < winLines.Count; i++) 
            { 
                var winLine = winLines[i];
                var index = winLine.JackpotId;
                if (winLine.JackpotId >0)
                {
                    var task = new TaskCompletionSource<bool>();
                    AudioUtil.Instance.PlayAudioFx("JpStart_Open");
                    var grandWinChips = machineContext.state.Get<BetState>().GetPayWinChips(winLine.Pay);
                    var view = PopUpManager.Instance.ShowPopUp<UIJackpotBase>(Constant11017.GetJackpotAddress((int)winLine.JackpotId-1));
                    view.SetJackpotWinNum((ulong)grandWinChips);
                    view.SetPopUpCloseAction(() =>
                    {
                        task.SetResult(true);
                    });
                    await task.Task;
                    await machineContext.WaitSeconds(1.0f);
                } 
            }
        }
        
        //猛犸象进入feature前的动画
        protected void HandleElephantTrigger()
        {
            var wheels = machineContext.state.Get<WheelsActiveState>().GetRunningWheel();
			var reTriggerElementContainers = wheels[0].GetElementMatchFilter((container) =>
            {
                if (container.sequenceElement.config.id == 12 || container.sequenceElement.config.id == 13)
                {
                    return true;
                }

                return false;
            });

            if (reTriggerElementContainers.Count > 0)
            { 
                for (var i = 0; i < reTriggerElementContainers.Count; i++)
                {
                    reTriggerElementContainers[i].PlayElementAnimation("Trigger");
                }
                AudioUtil.Instance.PlayAudioFx("W01W02_Trigger");
            }
        }
        
        protected override void RecoverLogicState()
		{
			RecoverLogicStateAsync();
		}

		protected async Task RecoverLogicStateAsync()
		{
			machineContext.view.Get<LinkRemaining11017>().ShowReSpinRemaining();
        }
        
        public override void OnMachineInternalEvent(MachineInternalEvent internalEvent, params object[] args)
        {
            switch (internalEvent)
            {
                case MachineInternalEvent.EVENT_WAIT_EVENT_COMPLETE:
                {
                    var waitEvent = (WaitEvent) args[0];
                    if (waitEvent == WaitEvent.WAIT_WIN_NUM_ANIMTION)
                    {
                        machineContext.view.Get<ControlPanel>().ShowSpinButton(false);
                        if (bananaGameWaitTask != null && !bananaGameWaitTask.Task.IsCompleted)
                        {
                            bananaGameWaitTask.TrySetResult(true);
                        }
                    }
                    break;
                }
                case MachineInternalEvent.EVENT_CONTROL_STOP:
                    machineContext.view.Get<ControlPanel>().StopWinAnimation();
                    break;
                
                case MachineInternalEvent.EVENT_CONTROL_AUTO_SPIN_STOP:
                    if (machineContext.state.Get<AutoSpinState>().IsAutoSpin)
                    {
                        machineContext.state.Get<AutoSpinState>().OnDisableAutoSpin();
                        machineContext.view.Get<ControlPanel>().ShowStopButton(true);
                    }
                    break;
            }
            base.OnMachineInternalEvent(internalEvent, args);
        }
        
        protected override void RestoreTriggerWheelElement()
        {
            var triggerPanels = reSpinState.triggerPanels;
            if (triggerPanels != null && triggerPanels.Count > 0 && triggerPanels[triggerPanels.Count-1] != null)
            {
                if (machineContext.state.Get<FreeSpinState>().IsInFreeSpin) 
                { 
                     machineContext.state.Get<WheelState11017>(2).UpdateWheelStateInfo(triggerPanels[triggerPanels.Count-1]);
                     machineContext.view.Get<Wheel>(2).ForceUpdateElementOnWheel();
                }
                else
                {
                     machineContext.state.Get<WheelState11017>(0).UpdateWheelStateInfo(triggerPanels[triggerPanels.Count-1]);
                     machineContext.view.Get<Wheel>(0).ForceUpdateElementOnWheel();
                }
            }
        }
    }
}