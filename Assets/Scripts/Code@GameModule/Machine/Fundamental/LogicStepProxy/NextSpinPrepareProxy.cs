// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date :2021-07-05 12:15 PM
// Ver : 1.0.0
// Description : LogicProxyNextSpinPrepare.cs
// ChangeLog :
// **********************************************

using System;
using System.Collections.Generic;
using DragonU3DSDK.Network.API.ILProtocol;

using UnityEngine;

namespace GameModule
{
    public class NextSpinPrepareProxy : LogicStepProxy
    {
        protected bool betChangedInCurrentStep = false;

        protected float timeToCheckPauseMusic = 0;

        protected AutoSpinState autoSpinState;

        protected Coroutine checkSpinGuideCoroutine;
        protected Coroutine pauseBackgroundMusicCoroutine;
        public NextSpinPrepareProxy(MachineContext context)
            : base(context)
        {
            
        }

        public override void SetUp()
        {
            base.SetUp();
            autoSpinState = machineContext.state.Get<AutoSpinState>();
        }

        public override bool CheckCurrentStepHasLogicToHandle()
        {
            return true;
        }
 
        protected override void HandleCommonLogic()
        {
            betChangedInCurrentStep = false;
            
            timeToCheckPauseMusic = Time.realtimeSinceStartup + 3;  
            
            base.HandleCommonLogic();

            CheckAndApplyNewUnlockBetList();

            var jackpotInfoState = machineContext.state.Get<JackpotInfoState>();
          
            if (jackpotInfoState != null)
            {
                jackpotInfoState.LockJackpot = false;
            }
            
            if (machineContext.state.Get<AutoSpinState>().IsAutoSpin)
            {
                if (autoSpinState.StopAutoOnNextSpin)
                {
                    OnAutoSpinStopClicked();
                    autoSpinState.StopAutoOnNextSpin = false;
                }
                else
                {
                    var nextSpinTime = Mathf.Max(0.5f,
                        machineContext.view.Get<ControlPanel>().GetNumAnimationEndTime()); 
                 
                    machineContext.WaitSeconds(nextSpinTime, CheckAndStartAutoSpin);
                }
            }
            else
            {
                RestoreBet();
                
                machineContext.view.Get<ControlPanel>().ShowSpinButton(true);
                
                CheckAndPlayWinCycle();
                
                pauseBackgroundMusicCoroutine = machineContext.WaitSeconds(3, PauseBackgroundMusic);
                checkSpinGuideCoroutine = machineContext.WaitSeconds(10, CheckAndShowSpinGuide);
            }
        }

        protected void CheckAndShowSpinGuide()
        {
            if (machineContext.GetRunningStep().StepType == LogicStepType.STEP_NEXT_SPIN_PREPARE)
            {
                if (machineContext.state.Get<BetState>().IsBalanceSufficient() && Client.Get<UserController>().GetUserLevel() < 15)
                {
                    machineContext.view.Get<ControlPanel>().CheckAndInitializeTapToSpinGuide(true);
                }
            }
        }
        protected void CheckAndApplyNewUnlockBetList()
        {
            if (machineContext.serviceProvider.IsBetListNeedUpdate())
            {
                machineContext.state.Get<BetState>().CheckAndApplyNewUnlockBetList();
               
                UpdateSpinUiViewTotalBet(false);
                OnUnlockBetFeatureConfigChanged();
                
               // machineContext.view.Get<ControlPanel>().ShowMaxBetFx();
            }
        }

        protected virtual void OnUnlockBetFeatureConfigChanged()
        {
            
        }

        protected void PauseBackgroundMusic()
        {
            if (Time.realtimeSinceStartup > timeToCheckPauseMusic - 0.3f)
            {
                if (machineContext.GetRunningStep().StepType == LogicStepType.STEP_NEXT_SPIN_PREPARE)
                {
                    AudioUtil.Instance.FadeOutMusic(3f);
                    autoSpinState.DisableNeverSleep();
                }
                
            }
        }
 
        //不做具体逻辑，逻辑分化到更小的函数中
        protected override void HandleCustomLogic()
        {
           //XDebug.Log("HandleCustomLogic"); 
        }

        public override void OnMachineInternalEvent(MachineInternalEvent internalEvent, params object[] args)
        {
            base.OnMachineInternalEvent(internalEvent, args);

            switch (internalEvent)
            {
                case MachineInternalEvent.EVENT_CONTROL_SPIN:
                    OnSpinButtonClicked(args);
                    break;
                case MachineInternalEvent.EVENT_CONTROL_STOP:
                    OnStopButtonClicked(args);
                    break;
                case MachineInternalEvent.EVENT_CONTROL_ADD_BET:
                    OnAddBetClicked(args);
                    break;
                case MachineInternalEvent.EVENT_CONTROL_MINUS_BET:
                    OnMinusBetClicked(args);
                    break;
                case MachineInternalEvent.EVENT_CONTROL_MAX_BET:
                    OnMaxBetClicked(args);
                    break;
                case MachineInternalEvent.EVENT_CONTROL_AUTO_SPIN:
                    OnAutoSpinClicked(args);
                    break;
                case MachineInternalEvent.EVENT_BET_CHANGED:
                    OnBetChange();
                    break;
                case MachineInternalEvent.EVENT_CONTROL_AUTO_SPIN_STOP:
                    OnAutoSpinStopClicked();
                    break;
                case MachineInternalEvent.EVENT_WAIT_EVENT_COMPLETE:
                    var waitEvent = (WaitEvent) args[0];
                    if (waitEvent == WaitEvent.WAIT_BLINK_ALL_WIN_LINE)
                        CheckAndPlayWinCycle();
                    break;
                
               case MachineInternalEvent.EVENT_CLICK_UI_UNLOCK_GAME_FEATURE:
                   int featureIndex = 0;
                   
                   if(args.Length > 0)
                       featureIndex = (int) args[0];
                   
                   machineContext.state.Get<BetState>().SetBetBigEnoughToUnlockFeature(featureIndex);
                   UpdateSpinUiViewTotalBet(false);
                   break;
               case MachineInternalEvent.EVENT_MAX_BET_UNLOCKED:
                   machineContext.view.Get<ControlPanel>().ShowMaxBetFx();
                   break;
            }
        }
        
        protected  void CheckAndPlayWinCycle()
        {
            if (NeedPlayWinCycle())
            {
                if (!machineContext.HasWaitEvent(new List<WaitEvent>() {WaitEvent.WAIT_BLINK_ALL_WIN_LINE}))
                {
                    var activeState = machineContext.state.Get<WheelsActiveState>();
                    var runningWheel = activeState.GetRunningWheel();
                    for (var i = 0; i < runningWheel.Count; i++)
                    {
                        if(!runningWheel[i].winLineAnimationController.IsWinCyclePlaying)
                            runningWheel[i].winLineAnimationController.BlinkWinLineOneByOne();
                    }
                }
            }
        }

        protected virtual bool NeedPlayWinCycle()
        {
            return !betChangedInCurrentStep && !machineContext.state.Get<FreeSpinState>().IsInFreeSpin && !machineContext.state.Get<ReSpinState>().IsInRespin;
        }

        protected virtual void OnBetChange()
        {
            betChangedInCurrentStep = true;

            StopWinCycle(true);

            machineContext.view.Get<ControlPanel>().StopWinAnimation(false, 0);
            
            SendBiBetChangeLog();
        }

        protected void SendBiBetChangeLog()
        {
            bool isAutoSpin = machineContext.state.Get<AutoSpinState>().IsAutoSpin;
            ulong bet = machineContext.state.Get<BetState>().totalBet;
            BiManagerGameModule.Instance.SendSpinAction(
                machineContext.machineConfig.machineId.ToString(),
                BiEventFortuneX.Types.SpinActionType.ChangeBet,isAutoSpin,bet,"");
        }
        
        protected void OnStopButtonClicked(params object[] args)
        {
            
        }
        
        public virtual void OnAutoSpinStopClicked()
        {
            if (autoSpinState.IsAutoSpin)
            {
                autoSpinState.OnDisableAutoSpin();
                machineContext.view.Get<ControlPanel>().ShowSpinButton(true);
            }
        }

        protected void OnSpinButtonClicked(params object[] args)
        {
            if (!machineContext.state.Get<BetState>().IsBalanceSufficient())
            {
                OnBalanceIsInsufficient();
                return;
            }
            
            StartNextSpin();
        }
        
        protected virtual void OnAddBetClicked(params object[] args)
        {
            var betState = machineContext.state.Get<BetState>();    
            if (betState.AlterBetLevel(1))
            {
                UpdateSpinUiViewTotalBet(false);
               
                EventBus.Dispatch(new EventBetChanged(1,betState.betLevel, betState.totalBet,betState.GetMexBetLevel()));
            }
        }
        
        protected virtual void OnMinusBetClicked(params object[] args)
        {
            var betState = machineContext.state.Get<BetState>();    
            if (machineContext.state.Get<BetState>().AlterBetLevel(-1))
            {
                UpdateSpinUiViewTotalBet(false);
                EventBus.Dispatch(new EventBetChanged(-1,betState.betLevel, betState.totalBet,betState.GetMexBetLevel()));
            }
        }
        
        protected virtual void OnMaxBetClicked(params object[] args)
        {
            var betState = machineContext.state.Get<BetState>();    
            if (machineContext.state.Get<BetState>().UseMaxBetLevel())
            {
                UpdateSpinUiViewTotalBet(false);
                
                EventBus.Dispatch(new EventBetChanged(100,betState.betLevel, betState.totalBet,betState.GetMexBetLevel()));
            }
        }
        
        protected void OnAutoSpinClicked(params object[] args)
        {
            int selectCount = (int) args[0];

            if (machineContext.state.Get<BetState>().IsBalanceSufficient())
            {
                machineContext.state.Get<AutoSpinState>().OnSelectAutoSpin(selectCount);
                machineContext.view.Get<ControlPanel>().ShowAutoStopButton(true);
                machineContext.view.Get<ControlPanel>().UpdateAutoSpinLeftCount(machineContext.state.Get<AutoSpinState>().AutoLeftCount);

                CheckAndStartAutoSpin();
            }
            else
            {
                OnBalanceIsInsufficient();
            }
        }
        
        public void CheckAndStartAutoSpin()
        {
            if (!machineContext.state.Get<AutoSpinState>().IsAutoSpin)
            {
                machineContext.WaitSeconds(3, PauseBackgroundMusic);
                return;
            }

            if (!machineContext.state.Get<BetState>().IsBalanceSufficient())
            {
                machineContext.state.Get<AutoSpinState>().OnDisableAutoSpin();
                machineContext.view.Get<ControlPanel>().ShowSpinButton(true);
               
                UpdateSpinUiViewTotalBet(false);
              
                OnBalanceIsInsufficient();
                return;
            }
            StartNextSpin();
        }
        protected void UpdateControlPanelStateAndWinNumAnimation()
        {
            if (machineContext.state.Get<FreeSpinState>().NextIsFreeSpin)
            { 
                if (!machineContext.state.Get<AutoSpinState>().IsAutoSpin)
                    machineContext.view.Get<ControlPanel>().ShowStopButton(false);
            }
            else
            {
                if (machineContext.state.Get<AutoSpinState>().IsAutoSpin)
                {
                    machineContext.view.Get<ControlPanel>().ShowAutoStopButton(true);
                    machineContext.view.Get<ControlPanel>().UpdateAutoSpinLeftCount(machineContext.state.Get<AutoSpinState>().AutoLeftCount);
                }
                else
                {
                    machineContext.view.Get<ControlPanel>().ShowSpinButton(false);
                }
            }
            
            machineContext.ClearAllWaitEvent();
        }
        
        public virtual void StartNextSpin()
        {
            
            UpdateControlPanelStateAndWinNumAnimation();
          
            autoSpinState.EnableNeverSleep();
            
            StopWinCycle(true);

           //     XDebug.Log("StartNextSpin");
           //     context.ActivePanelView.AnimationPlayController?.StopAllBlinkAnimation();

          //      waitForSpinHandleEndCallback.Invoke(null);
          //      waitForSpinHandleEndCallback = null;
           // }
           
           if (checkSpinGuideCoroutine != null)
           {
               machineContext.StopCoroutine(checkSpinGuideCoroutine);
           }

           if (pauseBackgroundMusicCoroutine != null)
           {
               machineContext.StopCoroutine(pauseBackgroundMusicCoroutine);
           }
           
           Proceed();
        }
        
        protected void OnBalanceIsInsufficient()
        {
            
            // Log.LogStateEvent(State.StateOutOfChips,null,StepId.ID_INVALID, "");
            //CommonNoticePopup.ShowCommonNoticePopUp("Balance Is Not Enough !!!!!");
            EventBus.Dispatch(new EventOnBalanceIsInsufficient());
            
            bool isAutoSpin = machineContext.state.Get<AutoSpinState>().IsAutoSpin;
            ulong bet = machineContext.state.Get<BetState>().totalBet;
            BiManagerGameModule.Instance.SendSpinAction(
                machineContext.machineConfig.machineId.ToString(),
                BiEventFortuneX.Types.SpinActionType.NoChips,isAutoSpin,bet,"");
        }
    }
}