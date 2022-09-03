// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date :2021-07-05 12:15 PM
// Ver : 1.0.0
// Description : LogicProxyFreeGame.cs
// ChangeLog :
// **********************************************

using System;
using System.Threading.Tasks;
using UnityEngine;

namespace GameModule
{
    public class FreeGameProxy : LogicStepProxy
    {
        protected FreeSpinState freeSpinState;

        protected ControlPanel controlPanel;
        
        public FreeGameProxy(MachineContext context)
            : base(context)
        {
        }

        public override bool CheckCurrentStepHasLogicToHandle()
        {
            return freeSpinState.IsInFreeSpin || freeSpinState.NextIsFreeSpin || freeSpinState.IsTriggerFreeSpin || freeSpinState.FreeNeedSettle;
        }

        protected override void RegisterInterestedWaitEvent()
        {
            waitEvents.Add(WaitEvent.WAIT_BLINK_ALL_WIN_LINE);
            waitEvents.Add(WaitEvent.WAIT_WIN_NUM_ANIMTION);
        }
         

        public override void SetUp()
        {
            base.SetUp();
            freeSpinState = machineContext.state.Get<FreeSpinState>();
            controlPanel = machineContext.view.Get<ControlPanel>();
        }

        public override bool IsConditionStep()
        {
            return true;
        }

        public override bool IsMatchCondition()
        {
            return !freeSpinState.NextIsFreeSpin;
        }

        public virtual bool IsFreeSpinTriggered()
        {
            return freeSpinState.IsTriggerFreeSpin;
        }
        
        public virtual bool NextSpinIsFreeSpin()
        {
            return freeSpinState.NextIsFreeSpin;
        }

        public virtual bool IsFreeSpinReTriggered()
        {
            return freeSpinState.NewCount > 0;
        }
        
        public virtual bool NeedSettle()
        {
            return freeSpinState.FreeNeedSettle;
        }

        public virtual bool UseAverageBet()
        {
            return false;
        }
        
        protected override void HandleCommonLogic()
        {
            if (IsFromMachineSetup())
                return;
            
            StopWinCycle();
            
            if (IsFreeSpinTriggered())
            {
                HandleFreeStartLogic();
                return;
            }
          
            if (!NextSpinIsFreeSpin())
            {
                HandleFreeFinishLogic();
                return;
            }

            if (IsFreeSpinReTriggered())
            {
                HandleFreeReTriggerLogic();
                return;
            }
            
            {
                UpdateFreeSpinUIState(true, UseAverageBet());
                Proceed();
            }
        }

        public virtual void RecoverFreeSpinStateWhenRoomSetup()
        {
            UpdateFreeSpinUIState(true, UseAverageBet());
          
            RecoverCustomFreeSpinState();
        }

        protected override void RecoverLogicState()
        {
            base.RecoverLogicState();

            if (IsFreeSpinTriggered())
            {
                HandleFreeStartLogic();
            }
            else
            {
                RecoverFreeSpinStateWhenRoomSetup();

                UpdateFreeSpinUIState(true, UseAverageBet());

                if (NeedSettle())
                {
                    HandleFreeFinishLogic();
                }
                else
                {
                    Proceed();
                }
            }
        }
        
        protected override void HandleCustomLogic()
        {
             // UpdateFreeSpinUIState(true);
             // Proceed();
        }

        protected virtual void RecoverCustomFreeSpinState()
        {
            
        }

        protected virtual async void HandleFreeStartLogic()
        {
            StopBackgroundMusic();
            if (!IsFromMachineSetup())
            {
                await ShowFreeSpinTriggerLineAnimation();
            }

            await ShowFreeGameStartPopUp();
            
            await ShowFreeSpinStartCutSceneAnimation();
            //UnPauseBackgroundMusic();
            Proceed();
        }

        protected virtual void StopBackgroundMusic()
        {
            AudioUtil.Instance.StopMusic();
        }
        
        

        protected virtual async Task ShowFreeSpinTriggerLineAnimation()
        {
            var wheels = machineContext.state.Get<WheelsActiveState>().GetRunningWheel();
            if(wheels.Count > 0)
                await wheels[0].winLineAnimationController.BlinkFreeSpinTriggerLine();
        }

        protected virtual async Task ShowFreeGameStartPopUp()
        {
            await ShowFreeGameStartPopUp<FreeSpinStartPopUp>();
        }
        
        protected virtual async Task ShowFreeGameStartPopUp<T>(string address = null) where T : FreeSpinStartPopUp
        {
            if (address == null)
            {
                address = "UIFreeGameStart" + machineContext.assetProvider.AssetsId;
            }
            
            if (machineContext.assetProvider.GetAsset<GameObject>(address) == null)
            {
                XDebug.LogError($"ShowFreeGameStartPopUp:{address} is Not Exist" );    
                return;
            }
 
            TaskCompletionSource<bool> waitTask = new TaskCompletionSource<bool>();
            machineContext.AddWaitTask(waitTask, null);

            var startPopUp = PopUpManager.Instance.ShowPopUp<T>(address);
            startPopUp.SetPopUpCloseAction(() =>
            {
                machineContext.RemoveTask(waitTask);
                waitTask.SetResult(true);
            });
            if (startPopUp.IsAutoClose())
            {
                await machineContext.WaitSeconds(GetStartPopUpDuration());
                startPopUp.Close();    
            }
            await waitTask.Task;
        }

        protected virtual ulong GetFreeSpinBet()
        {
            return freeSpinState.freeSpinBet;
        }
        
        protected virtual async Task ShowFreeSpinStartCutSceneAnimation()
        {
            //更新Bet到FreeSpin的TriggerBet
            //正常情况下FreeSpin的Bet和Base的Bet是一致的，但是有点关卡会使用平均BET
            //所以这里要将Bet在触发的时候设置成FreeSpinBet，保证jackpot等其他依赖BET的View上的数值计算正确
            //如果使用AverageBet，这里ControlPanel上面的会显示AverageBet的文字，不显示具体的数字，
            machineContext.state.Get<BetState>().SetTotalBet(GetFreeSpinBet());
            UpdateSpinUiViewTotalBet(false,false);
            UpdateFreeSpinUIState(true, UseAverageBet());
            controlPanel.ShowSpinButton(false);
            await Task.CompletedTask;
        }

        protected virtual void UpdateFreeSpinUIState(bool isFreeSpin, bool isAverage = false)
        {
            controlPanel.UpdateControlPanelState(isFreeSpin,isAverage);
            controlPanel.UpdateFreeSpinCountText(freeSpinState.CurrentCount, freeSpinState.TotalCount);
        }

        protected virtual void HandleFreeReTriggerLogic()
        {
            var assetName = "UIFreeGameExtraNotice" + machineContext.assetProvider.AssetsId;
            if (machineContext.assetProvider.GetAsset<GameObject>(assetName) != null)
            {
                var popUp = ShowFreeSpinReTriggeredPopup(assetName);
                popUp.SetPopUpCloseAction(() =>
                {
                    UpdateFreeSpinUIState(true, UseAverageBet());
                    HandleFreeReTriggerEnd();
                });
                popUp.SetExtraCount(freeSpinState.NewCount);
            }
            else
            {
                UpdateFreeSpinUIState(true, UseAverageBet());
                HandleFreeReTriggerEnd();
            }
        }
        
        protected virtual void HandleFreeReTriggerEnd()
        {
            Proceed();
        }

        protected virtual FreeSpinReTriggerPopUp ShowFreeSpinReTriggeredPopup(string assetName)
        {
            return PopUpManager.Instance.ShowPopUp<FreeSpinReTriggerPopUp>(assetName);
        }
        
        protected virtual async void HandleFreeFinishLogic()
        {
            var winState = machineContext.state.Get<WinState>();
            
            if (winState.displayCurrentWin == 0)
            {
                await machineContext.WaitSeconds(0.8f);
            }
            StopBackgroundMusic();
            await ShowFreeGameFinishPopUp();
            
            UpdateFreeSpinUIState(false);
            await ShowFreeSpinFinishCutSceneAnimation();
            await ShowFreeSpinBigWinEffect();
            //UnPauseBackgroundMusic();
            
            OnHandleFreeFinishLogicEnd();
        }
        
        protected virtual  void OnHandleFreeFinishLogicEnd()
        {
            freeSpinState.backFromFree = true;
            Proceed();
        }
        
        protected virtual async Task ShowFreeGameFinishPopUp()
        {
            await ShowFreeGameFinishPopUp<FreeSpinFinishPopUp>();
        }
        
        protected virtual async Task ShowFreeGameFinishPopUp<T>(string address = null) where T : FreeSpinFinishPopUp
        {
            if (address == null)
            {
                address = "UIFreeGameFinish" + machineContext.assetProvider.AssetsId;
            }

            if (machineContext.assetProvider.GetAsset<GameObject>(address) == null)
            {
                XDebug.LogError($"ShowFreeGameFinishPopUp:{address} is Not Exist" );    
                return;
            }

            TaskCompletionSource<bool> waitTask = new TaskCompletionSource<bool>();
            machineContext.AddWaitTask(waitTask, null);

            var startPopUp = PopUpManager.Instance.ShowPopUp<T>(address);
            startPopUp.SetPopUpCloseAction(() =>
            {
                machineContext.RemoveTask(waitTask);
                waitTask.SetResult(true);
            });

            await waitTask.Task;
        }

        protected virtual async Task ShowFreeSpinBigWinEffect()
        {
            var winState = machineContext.state.Get<WinState>();

            if (winState.winLevel >= (int)WinLevel.BigWin)
            {
                TaskCompletionSource<bool> waitTask = new TaskCompletionSource<bool>();
                machineContext.AddWaitTask(waitTask, null);

                WinEffectHelper.ShowBigWinEffect(winState.winLevel, winState.displayTotalWin, () =>
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
  
        protected virtual async Task ShowFreeSpinFinishCutSceneAnimation()
        {
            
            await Task.CompletedTask;
        }

        protected virtual float GetStartPopUpDuration()
        {
            return 3f;
        }
        
        protected virtual void RestoreTriggerWheelElement()
        {
            var triggerPanels = machineContext.state.Get<FreeSpinState>().triggerPanels;
            if (triggerPanels != null && triggerPanels.Count > 0 && triggerPanels[0] != null)
            {
                var wheels = machineContext.state.Get<WheelsActiveState>().GetRunningWheel();
                if (wheels.Count > 0 && wheels[0] != null)
                {
                    wheels[0].wheelState.UpdateWheelStateInfo(triggerPanels[0]);
                    wheels[0].ForceUpdateElementOnWheel();     
                }
            }
        }
    }
}