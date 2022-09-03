using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading.Tasks;
using GameModule;

namespace GameModule
{
	public class FreeGameProxy11026 : FreeGameProxy
	{
		public bool isAvgBet = false;
		ElementConfigSet elementConfigSet = null;
		public FreeGameProxy11026(MachineContext context)
		:base(context)
		{
			 elementConfigSet = machineContext.state.machineConfig.elementConfigSet;
		}
		
		protected override async Task ShowFreeGameStartPopUp()
        {
	        if (IsFromMachineSetup())
	        {
		        machineContext.state.Get<WheelsActiveState11026>().UpdateRunningWheelState(false,false);
		        var triggerPanels = machineContext.state.Get<FreeSpinState>().triggerPanels;
		        
		        if (triggerPanels != null && triggerPanels.Count > 0)
		        {
			        var baseWheel = machineContext.view.Get<Wheel>("WheelBaseGame");
			        baseWheel.wheelState.UpdateWheelStateInfo(triggerPanels[0]);
			        baseWheel.ForceUpdateElementOnWheel();
		        }
	        }
	        
	        await ShowFreeGameStartPopUp<FreeSpinStartPopUp11026>("UIFreeGameStart11026"); 
        }
		
		//megafree和superfree时用avgBet
		public override bool UseAverageBet()
        {
	        if (machineContext.state.Get<ExtraState11026>().GetIsMega() ||  machineContext.state.Get<ExtraState11026>().GetIsSuper())
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
		
		public override void RecoverFreeSpinStateWhenRoomSetup()
        {
	        base.RecoverFreeSpinStateWhenRoomSetup();
	        machineContext.view.Get<BackGroundView11026>().ShowFreeBackGround();
	        machineContext.view.Get<FreeGameTips11026>().showFreeTip(machineContext.state.Get<ExtraState11026>().GetIsMega(), machineContext.state.Get<ExtraState11026>().GetIsSuper());
        }

		protected override void RegisterInterestedWaitEvent()
		{
			waitEvents.Add(WaitEvent.WAIT_BLINK_ALL_WIN_LINE);
		}
		
		protected override async void HandleFreeReTriggerLogic()
        {
	        //retrigger时先清除覆盖在scatter上的wild
	        var wheel = machineContext.state.Get<WheelsActiveState11026>().GetRunningWheel()[0];
	        var extraState = machineContext.state.Get<ExtraState11026>();
	        var listWildPos = extraState.GetFreeRandomWildIds();
	        var listStickyWildPos = extraState.GetFreeStickyWildIds();
	        if (listWildPos.Count > 0 || listStickyWildPos.Count > 0)
	        {
		        var freeSpinTriggerLines = wheel.wheelState.GetFreeSpinTriggerLine();
		        List<string> listTriggerPos = new List<string>();
		        for (int i = 0; i < freeSpinTriggerLines.Count; i++)
		        {
			        var line = freeSpinTriggerLines[i];
			        for (var index = 0; index < line.Positions.Count; index++)
			        {
				        var pos = freeSpinTriggerLines[i].Positions[index];
				        var elementConfig = elementConfigSet.GetElementConfig(Constant11026.B01ElementId);
				        var container =  wheel.GetWinLineElementContainer((int) pos.X,(int) pos.Y);
				        container.UpdateElement(new SequenceElement(elementConfig,machineContext));
			        }
		        }
	        }
	        await wheel.winLineAnimationController.BlinkFreeSpinTriggerLine();
	        base.HandleFreeReTriggerLogic();
        }
		
		protected override async Task ShowFreeSpinTriggerLineAnimation()
        {
            if(machineContext.state.Get<BetState>().IsFeatureUnlocked(0))
                machineContext.view.Get<SuperFreeProgressView11026>().UpdateProgress(true);
            await base.ShowFreeSpinTriggerLineAnimation();
        }

		//开始free转场动画
		protected override async  Task ShowFreeSpinStartCutSceneAnimation()
		{
			machineContext.view.Get<TransitionsView11026>().PlayFreeTransition();
			await machineContext.WaitSeconds(1.1f);
			machineContext.view.Get<BackGroundView11026>().ShowFreeBackGround();
			machineContext.state.Get<WheelsActiveState11026>().UpdateRunningWheelState(false,true); 
			machineContext.view.Get<FreeGameTips11026>().showFreeTip(machineContext.state.Get<ExtraState11026>().GetIsMega(), machineContext.state.Get<ExtraState11026>().GetIsSuper());
			await machineContext.WaitSeconds(2.5f - 1.1f);
			await base.ShowFreeSpinStartCutSceneAnimation();
		}
		
		//结束free转场动画
		protected override async  Task ShowFreeSpinFinishCutSceneAnimation()
		{ 
			UpdateFreeSpinUIState(true, isAvgBet);
			machineContext.view.Get<TransitionsView11026>().PlayFreeTransition();
			await machineContext.WaitSeconds(1.1f);
			machineContext.view.Get<BackGroundView11026>().ShowBaseBackFround();
			machineContext.view.Get<LockElementLayer11026>().FinishRecyleStickeyWild();
			machineContext.state.Get<WheelsActiveState11026>().UpdateRunningWheelState(false,false);
			RestoreFinishTriggerWheelElement();
			var superFreeProgress = machineContext.view.Get<SuperFreeProgressView11026>();
			superFreeProgress.UpdateProgress();
			superFreeProgress.LockSuperFreeIdle(!machineContext.state.Get<BetState>().IsFeatureUnlocked(0));
			await machineContext.WaitSeconds(2.5f - 1.1f);
			await base.ShowFreeSpinFinishCutSceneAnimation();
		}
		
		//free回到base，将牌面换成触发时的牌面。
		protected virtual void RestoreFinishTriggerWheelElement()
        {
            var triggerPanels = machineContext.state.Get<FreeSpinState>().triggerPanels;
            if (triggerPanels != null && triggerPanels.Count > 0 && triggerPanels[0] != null)
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
                baseWheel.wheelState.UpdateWheelStateInfo(triggerPanels[0]);
                baseWheel.ForceUpdateElementOnWheel();   
            }
        }
	}
}