// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date :2021-07-06 8:27 PM
// Ver : 1.0.0
// Description : LogicStepProxy.cs
// ChangeLog :
// **********************************************

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DragonU3DSDK.Network.API.ILProtocol;
using UnityEngine;

namespace GameModule
{
    public class LogicStepProxy : ILogicStepProxy
    {
        private Action handleEndCallback;

        /// <summary>
        /// 当前LogicStep关注的WaitEvent，只有关注的所以WaitEvent结束之后，才能开始当前的Logic处理
        /// </summary>
        protected List<WaitEvent> waitEvents;

        protected MachineContext machineContext;

        protected bool isInWaitMode;

        protected LogicStepType preLogicStep;

        public LogicStepProxy(MachineContext inMachineContext)
        {
            machineContext = inMachineContext;
            waitEvents = new List<WaitEvent>();
        }

        public virtual void SetUp()
        {
            isInWaitMode = false;
            RegisterInterestedWaitEvent();

            XDebug.Log("SetUp");
        }

        //注册当前逻辑Step关注WaitEvent
        protected virtual void RegisterInterestedWaitEvent()
        {
        }

        protected virtual bool CheckIsAllWaitEventComplete()
        {
            if (waitEvents.Count > 0)
                return !machineContext.HasWaitEvent(waitEvents);
            return true;
        }

        public void HandleStepLogic(LogicStepType preLogicStepType, Action inHandleEndCallback)
        {
            isInWaitMode = true;

            preLogicStep = preLogicStepType;
            handleEndCallback = inHandleEndCallback;

            if (IsFromMachineSetup())
            {
                RecoverLogicState();
            }

            SafeHandleLogic();
        }

        protected virtual void RecoverLogicState()
        {
        }

        private void SafeHandleLogic()
        {
            if (!machineContext.IsPaused
                && isInWaitMode
                && CheckIsAllWaitEventComplete())
            {
                isInWaitMode = false;
                HandleCommonLogic();
                HandleCustomLogic();
            }
        }

        protected virtual void Proceed()
        {
            HandleToNextStep();
        }

        protected void HandleToNextStep()
        {
            if (handleEndCallback != null)
            {
                var tempHandle = handleEndCallback;
                handleEndCallback = null;
                tempHandle.Invoke();
            }
        }

        public virtual bool CheckCurrentStepHasLogicToHandle()
        {
            return true;
        }

        protected virtual void HandleCommonLogic()
        {
        }


        protected virtual void HandleCustomLogic()
        {
            Proceed();
        }

        public virtual bool IsConditionStep()
        {
            return false;
        }

        public virtual bool IsMatchCondition()
        {
            return true;
        }

        public virtual void LogicUpdate()
        {
        }

        public virtual void OnMachineInternalEvent(MachineInternalEvent internalEvent, params object[] args)
        {
            switch (internalEvent)
            {
                case MachineInternalEvent.EVENT_WAIT_EVENT_COMPLETE:
                case MachineInternalEvent.EVENT_MACHINE_RESUMED:
                    SafeHandleLogic();
                    break;
                case MachineInternalEvent.EVENT_UI_PAY_TABLE:
                    // if (!machineContext.view.Get<ControlPanel>().IsSpinButtonEnabled())
                    // {
                    //     return;
                    // }

                    PopUpManager.Instance.ShowPopUp<UIPayTablePopUp>("PayTable" +
                                                                     machineContext.assetProvider.AssetsId);

                    bool isAutoSpin = machineContext.state.Get<AutoSpinState>().IsAutoSpin;
                    ulong bet = machineContext.state.Get<BetState>().totalBet;
                    BiManagerGameModule.Instance.SendSpinAction(
                        machineContext.machineConfig.machineId.ToString(),
                        BiEventFortuneX.Types.SpinActionType.OpenPaytable, isAutoSpin, bet, "");

                    break;
                case MachineInternalEvent.EVENT_MUSIC_PREFERENCE_STATUS_CHANGE:
                    bool musicEnabled = (bool) args[0];
                    AudioUtil.Instance.OnMusicEnabled(musicEnabled);
                    break;
                case MachineInternalEvent.EVENT_SOUND_PREFERENCE_STATUS_CHANGE:
                    bool soundEnabled = (bool) args[0];
                    AudioUtil.Instance.OnSoundEnabled(soundEnabled);
                    break;
            }
        }

        protected void OnAutoSpinStopClicked(params object[] args)
        {
            if (machineContext.state.Get<AutoSpinState>().IsAutoSpin)
            {
                machineContext.state.Get<AutoSpinState>().OnDisableAutoSpin();
                machineContext.view.Get<ControlPanel>().ShowStopButton(false);
            }
        }


        public virtual void OnDestroy()
        {
        }

        public void UpdateSpinUiViewTotalBet(bool lockBet, bool updateControlPanelState = true)
        {
            var betState = machineContext.state.Get<BetState>();
            var controlPanel = machineContext.view.Get<ControlPanel>();
            controlPanel.SetTotalBet(betState.totalBet, betState.IsMaxBet(), betState.IsMinBet(), betState.IsExtraBet(), lockBet);
            if(updateControlPanelState)
                controlPanel.UpdateControlPanelState(false,false);
        }
        
        public virtual void ForceUpdateWinChipsToDisplayTotalWin(float effectDuration = 0.5f,
            bool needPlayWinOutAnimation = false)
        {
            var winState = machineContext.state.Get<WinState>();

            winState.SetCurrentWin(winState.displayTotalWin);

            machineContext.view.Get<ControlPanel>().UpdateWinLabelChipsWithAnimation((long) winState.currentWin,
                effectDuration, needPlayWinOutAnimation, "", "");
        }

        public virtual WinLevel UpdateWinChipsToDisplayTotalWin(bool withAudio = true, bool withAutoAni = false)
        {
            var winState = machineContext.state.Get<WinState>();

            var deltaWin = winState.displayTotalWin - winState.currentWin;

            return AddWinChipsToControlPanel(deltaWin, 0, withAudio, withAutoAni);
        }

        public virtual WinLevel AddWinChipsToControlPanel(ulong winChips, float configDuration = 0f,
            bool withAudio = true, bool withAutoAni = true, string specifiedAudioName = null,bool audioLoop = false,string specifiedWinAudioName = null)
        {
            if (winChips > 0)
            {
                //Debug.LogError($"=======aaaa currentWin:{machineContext.state.Get<WinState>().currentWin}");

                machineContext.state.Get<WinState>().AddCurrentWin(winChips);
                //Debug.LogError($"=======bbbb currentWin:{machineContext.state.Get<WinState>().currentWin} add:{winChips} origin:{machineContext.state.Get<WinState>().currentWin - winChips}");
                
                var winLevel = machineContext.state.Get<BetState>().GetSmallWinLevel((long) winChips);

                //     var machineConfig = MachineState.GetMachineConfig();

                string audioName = null;
                string stopAudioName = null;
                float effectDuration = 0.5f;

                bool needPlayWinOutAnimation = winLevel == WinLevel.NiceWin && withAutoAni;

                effectDuration = 0.5f;

                if (winLevel != WinLevel.NoWin)
                {
                    if (winLevel == WinLevel.SmallWin)
                    {
                        audioName = "Symbol_SmallWin_" + machineContext.assetProvider.AssetsId;
                        stopAudioName = "Symbol_SmallWinEnding_" + machineContext.assetProvider.AssetsId;
                        effectDuration = 1.0f;
                        if (!string.IsNullOrEmpty(specifiedWinAudioName))
                        {
                            audioName = specifiedWinAudioName;
                            stopAudioName = "";
                        }
                    }
                    else if (winLevel == WinLevel.Win || !withAutoAni)
                    {
                        audioName = "Symbol_Win_" + machineContext.assetProvider.AssetsId;
                        stopAudioName = "Symbol_WinEnding_" + machineContext.assetProvider.AssetsId;
                        effectDuration = 2.0f;
                        if (!string.IsNullOrEmpty(specifiedWinAudioName))
                        {
                            audioName = specifiedWinAudioName; 
                            stopAudioName = "";
                        }
                    }
                    else if (winLevel == WinLevel.NiceWin)
                    {
                        if (!string.IsNullOrEmpty(specifiedWinAudioName))
                        {
                            audioName = specifiedWinAudioName;
                            stopAudioName = "";
                        }
                        //Nice Win走通用音效
                        effectDuration = 6;
                    }
                }
                
                if (configDuration > 0)
                {
                    effectDuration = configDuration;
                }

                if (!string.IsNullOrEmpty(specifiedAudioName))
                {
                    audioName = specifiedAudioName;
                    stopAudioName = "Symbol_SmallWinEnding_" + machineContext.assetProvider.AssetsId;
                }

                if (!withAudio)
                {
                    audioName = String.Empty;
                    stopAudioName = String.Empty;
                }

                machineContext.view.Get<ControlPanel>().UpdateWinLabelChipsWithAnimation(
                    (long) machineContext.state.Get<WinState>().currentWin,
                    effectDuration, needPlayWinOutAnimation, audioName, stopAudioName,audioLoop);
       
                return winLevel;
            }

            return WinLevel.NoWin;
        }

        //是否重进机器
        protected virtual bool IsFromMachineSetup()
        {
            return preLogicStep == LogicStepType.STEP_MACHINE_SETUP;
        }

        protected void RestoreBet()
        {
            var betState = machineContext.state.Get<BetState>();

            if (betState.RestoreTotalBet())
            {
                var controlPanel = machineContext.view.Get<ControlPanel>();
                controlPanel.SetTotalBet(betState.totalBet, betState.IsMaxBet(), betState.IsMinBet(), betState.IsExtraBet(),false);

                controlPanel.UpdateControlPanelState(false, false);
            }
        }
        
        protected virtual void StopWinCycle(bool force = false)
        {
            var activeState = machineContext.state.Get<WheelsActiveState>();
            var runningWheel = activeState.GetRunningWheel();
            
            for (var i = 0; i < runningWheel.Count; i++)
                if(runningWheel[i].winLineAnimationController.IsWinCyclePlaying || force)
                    runningWheel[i].winLineAnimationController.StopAllElementAnimation();
        }
        protected TaskCompletionSource<bool> GetWaitTask()
        {
            var task = new TaskCompletionSource<bool>();
            machineContext.AddWaitTask(task,null);
            return task;
        }
    }
}