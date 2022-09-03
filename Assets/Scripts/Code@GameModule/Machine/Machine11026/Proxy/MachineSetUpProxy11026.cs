using System.Collections.Generic;
using UnityEngine;
using System;
using GameModule;

namespace GameModule
{
	public class MachineSetUpProxy11026 : MachineSetUpProxy
	{
		public MachineSetUpProxy11026(MachineContext context)
		:base(context)
		{

	
		}
		protected override void UpdateViewWhenRoomSetUp()
        {
	        base.UpdateViewWhenRoomSetUp();
	        UpdateSuperBonusProgressView();
            UpdateBackGroupView();
        }
		
		protected void UpdateBackGroupView()
        {
            machineContext.view.Get<BackGroundView11026>().ShowBackground(machineContext.state.Get<FreeSpinState>().IsInFreeSpin,machineContext.state.Get<ReSpinState>().IsInRespin);
        }
		
		protected void UpdateSuperBonusProgressView()
        {
            machineContext.view.Get<SuperFreeProgressView11026>().UpdateProgress();
            var superFreeProgressView11026 = machineContext.view.Get<SuperFreeProgressView11026>();
            superFreeProgressView11026.LockSuperFree(! machineContext.state.Get<BetState>().IsFeatureUnlocked(0),false);
	        // machineContext.view.Get<JackpotPanel11026>().UpdateJackpotLockState(false,true);
        }

	}
}