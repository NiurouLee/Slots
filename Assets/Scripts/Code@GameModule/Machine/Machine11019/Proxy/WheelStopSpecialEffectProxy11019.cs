using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading.Tasks;
using DG.Tweening;
using GameModule;

namespace GameModule
{
	public class WheelStopSpecialEffectProxy11019 : WheelStopSpecialEffectProxy
	{
		public WheelStopSpecialEffectProxy11019(MachineContext machineContext)
		:base(machineContext)
		{

	
		}


		protected async override void HandleCustomLogic()
		{

			if (machineContext.state.Get<FreeSpinState>().IsTriggerFreeSpin)
			{
				machineContext.view.Get<TransitionsView11019>().PlayManFree();

			}
			await FreeBonusFly();
			await machineContext.view.Get<LinkLockView11019>().RefreshReSpinCount(false);
			base.HandleCustomLogic();
		}
		
		
		
		
		protected async Task FreeBonusFly()
		{
			if (machineContext.state.Get<FreeSpinState>().IsInFreeSpin)
			{
				var bonusWinView = machineContext.view.Get<BonusWinView11019>(Constant11019.FreeBonusWinViewName);
				await Constant11019.BounsFly(machineContext,this, bonusWinView);
				
			}

			
		}


		

	}
}