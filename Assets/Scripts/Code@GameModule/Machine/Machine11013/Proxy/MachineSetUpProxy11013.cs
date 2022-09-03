using System.Collections.Generic;
using UnityEngine;
using System;
using GameModule;

namespace GameModule
{
	public class MachineSetUpProxy11013 : MachineSetUpProxy
	{
		public MachineSetUpProxy11013(MachineContext context)
		:base(context)
		{

	
		}
		
		protected override void HandleCustomLogic()
		{
			machineContext.view.Get<BaseCollect11013>().RefreshProgressNoAnim();
			
			var freeSpinState  = machineContext.state.Get<FreeSpinState>();
			if (freeSpinState.IsInFreeSpin  && !freeSpinState.IsTriggerFreeSpin)
			{
				machineContext.view.Get<LockView11013>().ScatterToWildNoAnim();
			}

			base.HandleCustomLogic();
		}
	}
}