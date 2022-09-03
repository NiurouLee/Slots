using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading.Tasks;
using GameModule;

namespace GameModule
{
	public class WheelStopSpecialEffectProxy11012 : WheelStopSpecialEffectProxy
	{
		public WheelStopSpecialEffectProxy11012(MachineContext machineContext)
		:base(machineContext)
		{

	
		}


		protected override async void HandleCustomLogic()
		{
			machineContext.view.Get<DoorView11012>().CloseLinkAnticipation();
			await machineContext.view.Get<DoorView11012>().OpenDoor();
			await machineContext.view.Get<LinkLockView11012>().RefreshReSpinCount(false);

			base.HandleCustomLogic();
		}


		



	}
}