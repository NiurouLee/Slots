using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading.Tasks;
using DragonU3DSDK.Network.API.ILProtocol;
using GameModule;

namespace GameModule
{
    public class FreeGameProxy11017 : FreeGameProxy
    {
        public bool isAvgBet = false;
        public FreeGameProxy11017(MachineContext context)
            :base(context)
        {
        }
        
        protected override void RegisterInterestedWaitEvent()
        {
            waitEvents.Add(WaitEvent.WAIT_BLINK_ALL_WIN_LINE);
        }

        protected override void RecoverCustomFreeSpinState()
        {
            machineContext.view.Get<FreeRemaining11017>().RefreshReSpinCount();
            base.RecoverCustomFreeSpinState();
        }
        
        protected override void HandleCustomLogic()
        {
            base.HandleCustomLogic();
            if (!IsFreeSpinReTriggered())
            {
                 machineContext.view.Get<FreeRemaining11017>().RefreshReSpinCount();
            }
        }
        public override bool UseAverageBet()
        {
            uint level = machineContext.state.Get<ExtraState11017>().GetLevel();
            if (level == 5)
            {
                isAvgBet = true;
                return true;
            }
            else
            {
                isAvgBet = false;
                return false;
            }
        }
        
        protected override void HandleFreeReTriggerEnd()
        {
            machineContext.view.Get<FreeRemaining11017>().RefreshReSpinCount();
            Proceed();
        }
        
        protected async Task ChangeElement()
        {
            uint level = machineContext.state.Get<ExtraState11017>().GetLevel();
            if (level == 5)
            {
                machineContext.view.Get<SuperFreeGameIcon11017>().ShowSuperFreeIcon();
                machineContext.view.Get<SuperFreeGameGlowIcon11017>().ShowSuperFreeGlowIcon();
            }
            else
            {
                 machineContext.view.Get<SuperFreeGameIcon11017>().HideSuperFreeIcon();
                 machineContext.view.Get<SuperFreeGameGlowIcon11017>().HideSuperFreeGlowIcon();
            }
        }

        protected override async Task ShowFreeGameStartPopUp()
        {
            if (IsFromMachineSetup())
            {
                machineContext.state.Get<WheelsActiveState11017>().UpdateRunningWheelState(false,false);
                var superFreeGameLock11017 = machineContext.view.Get<SuperFreeGameLock11017>();
                superFreeGameLock11017.LockSuperFreeIdle(!machineContext.state.Get<BetState>().IsFeatureUnlocked(0));
                RestoreTriggerWheelElement();
            }
            else
            {
                //当收集金币被锁时，不需要播放任何动画。
                if (machineContext.state.Get<BetState>().IsFeatureUnlocked(0))
                { 
                    await machineContext.WaitSeconds(1.0f); 
                    await machineContext.view.Get<SuperFreeGameCoins11017>().setWheelCoin();
                    await machineContext.WaitSeconds(0.8f); 
                }
                else 
                { 
                    await machineContext.WaitSeconds(1.0f); 
                }
            }
            await ShowFreeGameStartPopUp<FreeSpinStartPopUp11017>("UIFreeGameStart11017"); 
        }
        
         protected override void RecoverLogicState()
        {
            base.RecoverLogicState();

            if (IsFreeSpinTriggered())
            {
                // HandleFreeStartLogic();
            }
            else
            {
                RecoverFreeSpinStateWhenRoomSetup();
                uint level = machineContext.state.Get<ExtraState11017>().GetLevel(); 
                if (level == 5) 
                {
                    machineContext.view.Get<SuperFreeGameGlowIcon11017>().ShowSuperFreeGlowIcon(); 
                }
                else 
                {
                    machineContext.view.Get<SuperFreeGameGlowIcon11017>().HideSuperFreeGlowIcon(); 
                }
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
        
        protected override async  Task ShowFreeSpinStartCutSceneAnimation()
        {
            machineContext.view.Get<TransitionsView11017>().PlayFreeTransition();
            await machineContext.WaitSeconds(2.67f);
            machineContext.state.Get<WheelsActiveState11017>().UpdateRunningWheelState(false,true);
            machineContext.view.Get<FreeRemaining11017>().ShowRemaining();
            //将牌面换成base的
            RestoreTBaseWheelElement();
            await machineContext.WaitSeconds(3.25f - 2.67f);
            await base.ShowFreeSpinStartCutSceneAnimation();
            machineContext.state.machineContext.view.Get<FeatureGame11017>().SetFeature();
            machineContext.view.Get<FreeRemaining11017>().RefreshReSpinCount();
            await machineContext.WaitSeconds(0.58f);
            //更换猛犸象图标
            await ShowSuperFreeSpinChangeAnimaion();
            await ChangeElement();
        }

        protected async Task ShowSuperFreeSpinChangeAnimaion()
        {
            uint level = machineContext.state.Get<ExtraState11017>().GetLevel();
            if (level == 5)
            {
                var wheel = machineContext.state.Get<WheelsActiveState11017>().GetRunningWheel()[0];
                var elementContainer = wheel.GetRoll(2).GetVisibleContainer(2);
                var elementConfigSet = machineContext.state.machineConfig.GetElementConfigSet();
                var fixedConfig = elementConfigSet.GetElementConfig(Constant11017.PuePleElementId);
                elementContainer.UpdateElement(new SequenceElement(fixedConfig,machineContext));
                elementContainer.ShiftSortOrder(false); 
                AudioUtil.Instance.PlayAudioFxOneShot("B02_STACK");
                elementContainer.PlayElementAnimation("SFGBlink");
                await machineContext.WaitSeconds(2.5f);
                // elementContainer.PlayElementAnimation("SFGIdle");
                // await machineContext.WaitSeconds(0.5f);
                // elementContainer.PlayElementAnimation("SFGIdle");
                // await machineContext.WaitSeconds(0.5f);
            }
        }
        protected  override async  Task ShowFreeSpinFinishCutSceneAnimation()
        {
            UpdateFreeSpinUIState(true, isAvgBet);
            machineContext.view.Get<TransitionsView11017>().PlayFreeTransition();
            await machineContext.WaitSeconds(2.67f);
            machineContext.view.Get<SuperFreeGameGlowIcon11017>().HideSuperFreeGlowIcon();
            machineContext.state.Get<WheelsActiveState11017>().UpdateRunningWheelState(false,false);
            RestoreFinishTriggerWheelElement();
            machineContext.state.machineContext.view.Get<FeatureGame11017>().SetFeature();
            var superFreeGameLock11017 = machineContext.view.Get<SuperFreeGameLock11017>();
            superFreeGameLock11017.LockSuperFreeIdle(!machineContext.state.Get<BetState>().IsFeatureUnlocked(0));
            await machineContext.WaitSeconds(3.25f - 2.67f);
            await base.ShowFreeSpinFinishCutSceneAnimation();
        }
        
        protected override async void HandleFreeReTriggerLogic()
        {
            var wheel = machineContext.state.Get<WheelsActiveState11017>().GetRunningWheel()[0];
            await wheel.winLineAnimationController.BlinkFreeSpinTriggerLine();
            base.HandleFreeReTriggerLogic();
        }

        protected virtual void RestoreTBaseWheelElement()
        {
            var triggerPanels = machineContext.state.Get<FreeSpinState>().triggerPanels;
            var panelCount = machineContext.state.Get<WheelState11017>(0).panelCount;
            if (panelCount > 0)
            {
                var triggerPanel = machineContext.state.Get<WheelState11017>(0).serverSpinResult.GameResult.Panels[panelCount - 1];
                if (triggerPanel != null)
                {
                    machineContext.state.Get<WheelState11017>(2).UpdateWheelStateInfo(triggerPanel);
                    machineContext.view.Get<Wheel>(2).ForceUpdateElementOnWheel();
                }
            }
            else
            { 
                if (triggerPanels != null && triggerPanels.Count > 0 && triggerPanels[triggerPanels.Count-1] != null)
                {
                      machineContext.state.Get<WheelState11017>(2).UpdateWheelStateInfo(triggerPanels[triggerPanels.Count-1]);
                      machineContext.view.Get<Wheel>(2).ForceUpdateElementOnWheel();
                }
            }
        }

        protected virtual void RestoreFinishTriggerWheelElement()
        {
            var triggerPanels = machineContext.state.Get<FreeSpinState>().triggerPanels;
            if (triggerPanels != null && triggerPanels.Count > 0 && triggerPanels[triggerPanels.Count-1] != null)
            {
                if (freeSpinState.NextIsFreeSpin) 
                { 
                    machineContext.state.Get<BetState>().SetTotalBet(freeSpinState.freeSpinBet);
                    UpdateSpinUiViewTotalBet(false,false);
                }
                else 
                { 
                    RestoreBet();
                }
                controlPanel.UpdateControlPanelState(false, false);
                var baseWheel = machineContext.view.Get<Wheel>("WheelBaseGame");
                baseWheel.wheelState.UpdateWheelStateInfo(triggerPanels[triggerPanels.Count-1]);
                baseWheel.ForceUpdateElementOnWheel();   
            }
        }
        
        protected override void RestoreTriggerWheelElement()
        {
            var triggerPanels = machineContext.state.Get<FreeSpinState>().triggerPanels;
            if (triggerPanels != null && triggerPanels.Count > 0 && triggerPanels[triggerPanels.Count-1] != null)
            {
                var wheels = machineContext.state.Get<WheelsActiveState>().GetRunningWheel();
                if (wheels.Count > 0 && wheels[0] != null)
                {
                    wheels[0].wheelState.UpdateWheelStateInfo(triggerPanels[triggerPanels.Count-1]);
                    wheels[0].ForceUpdateElementOnWheel();     
                }
            }
        }
    }
}