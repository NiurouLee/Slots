//  **********************************************
//  Copyright(c) 2021 by com.ustar
//  All right reserved
// 
//  Author : Besure.Chen
//  Date :2021-08-16 21:00
//  Ver : 1.0.0
//  Description : LinkLogicProxy.cs
//  ChangeLog :
//  **********************************************

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace GameModule
{
    public class LinkLogicProxy: ReSpinLogicProxy
    {
        protected ControlPanel controlPanel;
        protected string strLinkTriggerSound="";
        public LinkLogicProxy(MachineContext context)
            : base(context)
        {
        }
        
        protected override void RegisterInterestedWaitEvent()
        {
            waitEvents.Add(WaitEvent.WAIT_WIN_NUM_ANIMTION);
            waitEvents.Add(WaitEvent.WAIT_BLINK_ALL_WIN_LINE);
        }
        
        protected override bool CheckIsAllWaitEventComplete()
        {
            if (IsInRespinProcess())
            {
                return base.CheckIsAllWaitEventComplete();
            }
            return true;
        }
        
        public override void SetUp()
        {
            base.SetUp();
            reSpinState = machineContext.state.Get<ReSpinState>();
            controlPanel = machineContext.view.Get<ControlPanel>();
        }

        protected override async void HandleCustomLogic()
        {
            //处理触发Link：开始弹板或者过场动画
            if (IsLinkTriggered())
            {
                StopBackgroundMusic();
                await HandleReSpinStartLogic();
                await HandleLinkGameTrigger();
                await HandleLinkBeginPopup();
                await HandleLinkBeginCutSceneAnimation();
            }

            //处理Link逻辑：锁图标或者其他
            if (IsInRespinProcess())
            {
                await HandleLinkGame();
            }

            //是否Link结束：处理结算过程
            if (IsLinkSpinFinished())
            {
                StopBackgroundMusic();
                await HandleLinkReward();
            }
            //Link结算完成，恢复Normal
            if (NeedSettle())
            {
                StopBackgroundMusic();
                await HandleLinkFinishPopup();
                await HandleLinkFinishCutSceneAnimation();
                await HandleLinkHighLevelEffect();
            }
            Proceed();
        }
        
        
        protected virtual void StopBackgroundMusic()
        {
            AudioUtil.Instance.StopMusic();
        }
        
        

        protected virtual async Task HandleLinkGameTrigger()
        {
            if (machineContext.assetProvider.GetAsset<AudioClip>(strLinkTriggerSound))
            {
                AudioUtil.Instance.PlayAudioFx(strLinkTriggerSound);
            }
            var wheels = GetLinkGameTriggerWheels();
            for (int i = 0; i < wheels.Count; i++)
            {
                var triggerElementContainers = wheels[i].GetElementMatchFilter((container) =>
                {
                    if (CheckIsTriggerElement(container))
                    {
                        return true;
                    }

                    return false;
                },GetElementTriggerOffsetRow());

                if (triggerElementContainers.Count > 0)
                {
                    for (var j = 0; j < triggerElementContainers.Count; j++)
                    {
                        triggerElementContainers[j].PlayElementAnimation(GetElementTriggerAnimation());
                        triggerElementContainers[j].ShiftSortOrder(true);
                    }

                    await XUtility.WaitSeconds(GetElementTriggerDuration(), machineContext);
                    for (var j = 0; j < triggerElementContainers.Count; j++)
                    {
                        triggerElementContainers[j].UpdateAnimationToStatic();
                    }
                }   
            }
            machineContext.view.Get<ControlPanel>().ShowStopButton(true);
        }

        protected virtual bool CheckIsTriggerElement(ElementContainer container)
        {
            return false;
        }
        protected virtual List<Wheel> GetLinkGameTriggerWheels()
        {
            return machineContext.state.Get<WheelsActiveState>().GetRunningWheel();
        }

        protected virtual float GetElementTriggerDuration()
        {
            return 2f;
        }
        
        protected virtual int GetElementTriggerOffsetRow()
        {
            return 0;
        }
        
        protected virtual string GetElementTriggerAnimation()
        {
            return "Trigger";
        }

        protected virtual async Task HandleLinkBeginPopup()
        {
            if (machineContext.assetProvider.GetAsset<GameObject>(GetLinkBeginAddress()) != null)
            {
                var task = GetWaitTask();
                var startLinkPopup = PopUpManager.Instance.ShowPopUp<ReSpinStartPopUp>(GetLinkBeginAddress());
                if (startLinkPopup != null)
                {
                    startLinkPopup.SetPopUpCloseAction(() =>
                    {
                        SetAndRemoveTask(task);
                    });
                    if (startLinkPopup.IsAutoClose())
                    {
                        await machineContext.WaitSeconds(GetLinkBeginPopupDuration());
                        startLinkPopup.Close();     
                    }
                }
                else
                {
                    SetAndRemoveTask(task);
                }

                await task.Task;
            }
            await Task.CompletedTask;
        }
        
        
        protected virtual async Task HandleLinkFinishPopup()
        {
            if (machineContext.assetProvider.GetAsset<GameObject>(GetLinkFinishAddress()) != null)
            {
                var task = GetWaitTask();
                var finishLinkPopup = PopUpManager.Instance.ShowPopUp<ReSpinFinishPopUp>(GetLinkFinishAddress());
                if (!ReferenceEquals(finishLinkPopup, null))
                {
                    finishLinkPopup.SetPopUpCloseAction(() =>
                    {
                        //finishLinkPopup.Close();
                        SetAndRemoveTask(task);
                    });
                    finishLinkPopup.Initialize(machineContext);
                    if (finishLinkPopup.IsAutoClose())
                    {
                        await machineContext.WaitSeconds(GetLinkFinishPopupDuration());
                        finishLinkPopup.Close();
                    }
                }
                await task.Task;
            }
            await Task.CompletedTask;
        }
        protected virtual async Task HandleLinkGame()
        {
            await Task.CompletedTask;
        }

        protected virtual async  Task HandleLinkReward()
        {
            await Task.CompletedTask;
        }
        
        protected virtual async  Task HandleLinkBeginCutSceneAnimation()
        {
            await Task.CompletedTask;
        }
        
        protected virtual async  Task HandleLinkFinishCutSceneAnimation()
        {
            RestoreTriggerWheelElement();
            await Task.CompletedTask;
        }
        
        protected virtual string GetLinkBeginAddress()
        {
            return "UIRespinNotice" + machineContext.assetProvider.AssetsId;;
        }
        protected virtual string GetLinkFinishAddress()
        {
            return "UIRespinFinish" + machineContext.assetProvider.AssetsId;;
        }

        //Link Spin是否已经结束
        protected virtual bool IsLinkSpinFinished()
        {
            return !NextIsLinkSpin() && reSpinState.ReSpinNeedSettle;
        }

        //Link结算过程结束，有可能结算和LinkSpin结束条件不一致
        public virtual bool NeedSettle()
        {
            return reSpinState.ReSpinNeedSettle;
        }

        //当次是否触发了Link
        protected virtual bool IsLinkTriggered()
        {
            return reSpinState.ReSpinTriggered;
        }

        protected virtual bool NextIsLinkSpin()
        {
            return reSpinState.NextIsReSpin;
        }

        // protected TaskCompletionSource<bool> GetWaitTask()
        // {
        //     var task = new TaskCompletionSource<bool>();
        //     machineContext.AddWaitTask(task,null);
        //     return task;
        // }

        protected virtual float GetLinkBeginPopupDuration()
        {
            return 2;
        }
        protected virtual float GetLinkFinishPopupDuration()
        {
            return 2;
        }

        protected void SetAndRemoveTask(TaskCompletionSource<bool> task)
        {
            if (!ReferenceEquals(task,null))
            {
                task.SetResult(true);
                RemoveTask(task);
            }
        }
        private void RemoveTask(TaskCompletionSource<bool> task)
        {
            machineContext.RemoveTask(task);
        }

        protected virtual bool IsInRespinProcess()
        {
            return IsLinkTriggered() || reSpinState.IsInRespin || reSpinState.ReSpinNeedSettle;
        }

        protected virtual ElementContainer UpdateRunningElement(uint elementId, int rollIndex, int rowIndex = 0, bool active=false)
        {
            var wheel = machineContext.state.Get<WheelsActiveState>().GetRunningWheel()[0];
            var lockRoll = wheel.GetRoll(rollIndex);
            var elementConfigSet = machineContext.state.machineConfig.GetElementConfigSet();
            var seqElement = new SequenceElement(elementConfigSet.GetElementConfig(elementId), machineContext);
            ElementContainer elementContainer= lockRoll.GetVisibleContainer(rowIndex);
            if (elementContainer != null)
            {
                elementContainer.UpdateElement(seqElement, active);
                if (active)
                {
                    elementContainer.ShiftSortOrder(true);
                    elementContainer.UpdateElementMaskInteraction(true);
                }
            }
            return elementContainer;
        }

        protected uint GetRunningElementId(int rollIndex, int rowIndex = 0)
        {
            var wheel = machineContext.state.Get<WheelsActiveState>().GetRunningWheel()[0];
            var lockRoll = wheel.GetRoll(rollIndex);
            var elementId = lockRoll.GetVisibleContainer(rowIndex).GetElement().sequenceElement.config.id;
            return elementId;
        }
        
        protected ElementContainer GetRunningElementContainer(int rollIndex, int rowIndex = 0)
        {
            var wheel = machineContext.state.Get<WheelsActiveState>().GetRunningWheel()[0];
            var lockRoll = wheel.GetRoll(rollIndex);
            return lockRoll.GetVisibleContainer(rowIndex);
        }

        protected SequenceElement GetSequenceElement(uint elementId)
        {
            var elementConfigSet = machineContext.state.machineConfig.GetElementConfigSet();
            return new SequenceElement(elementConfigSet.GetElementConfig(elementId), machineContext);
        }

        protected virtual bool IsTriggerGrand()
        {
            return false;
        }

        protected virtual async Task HandleLinkHighLevelEffect()
        {
            var winState = machineContext.state.Get<WinState>();
            var freeSpinState = machineContext.state.Get<FreeSpinState>();
            if (!freeSpinState.IsTriggerFreeSpin)
            {
                if (winState.winLevel>= (int)WinLevel.BigWin)
                {
                    await WinEffectHelper.ShowBigWinEffectAsync(winState.winLevel, winState.displayCurrentWin,
                        machineContext);   
                }
            }

            
        }
    }
}