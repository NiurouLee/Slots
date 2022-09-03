using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading.Tasks;
using GameModule;
using GameModule.PopUp;

namespace GameModule
{
	public class FreeGameProxy11021 : FreeGameProxy
	{

		public FreeGameProxy11021(MachineContext context)
		:base(context)
		{

		}
		
		
		protected override void RecoverCustomFreeSpinState()
		{
			
			base.RecoverCustomFreeSpinState();
		}


		protected async override Task ShowFreeSpinTriggerLineAnimation()
		{
			
		}


		protected override async Task ShowFreeGameStartPopUp()
		{

			
			RestoreTriggerWheelElement();
			




			await ShowFreeGameStartPopUp<FreeSpinStartPopUp>();
		}
		
		
		protected async override Task ShowFreeSpinStartCutSceneAnimation()
		{
			machineContext.state.Get<WheelsActiveState11021>().UpdateRunningWheelState(true);
			AudioUtil.Instance.PlayAudioFxOneShot("FreeGame_Welcome");
			ShowDefultFreeElement();
			await machineContext.WaitSeconds(2f);
			
			await base.ShowFreeSpinStartCutSceneAnimation();
		}


		protected void ShowDefultFreeElement()
		{
			var wheelsActiveState = machineContext.state.Get<WheelsActiveState11021>();
			var wheel = wheelsActiveState.GetRunningWheel()[0];

			RefreshDefultWheel(wheel, wheelsActiveState);

			var listContainer = wheel.GetElementMatchFilter((container) =>
			{
				if (container.sequenceElement.config.id == Constant11021.ElementScatter)
				{
					return true;
				}

				return false;
			});

			foreach (var container in listContainer)
			{
				container.PlayElementAnimation("Get");
				container.ShiftSortOrder(true);
			}
		}
		
		
		public void RefreshDefultWheel(Wheel wheel,WheelsActiveState wheelsActiveState)
		{
			var wheelState = wheel.wheelState;
			List<int> listIndex = new List<int>();
			for (int i = 0; i < wheelState.rollCount; i++)
			{
				int index = wheelState.GetActiveSequenceElement(i).Count-1;
				listIndex.Add(index);
			}
            
			wheelState.UpdateCurrentActiveSequence(wheelsActiveState.GetReelNameForWheel(wheel),listIndex);
			wheel.ForceUpdateElementOnWheel(true,true);
		}


		protected async override Task ShowFreeSpinFinishCutSceneAnimation()
		{ 
			
			machineContext.state.Get<WheelsActiveState11021>().UpdateRunningWheelState(false);
			RestoreTriggerWheelElement();
			
			
			await base.ShowFreeSpinFinishCutSceneAnimation();
		}
		
		
		
        
		




	}
}