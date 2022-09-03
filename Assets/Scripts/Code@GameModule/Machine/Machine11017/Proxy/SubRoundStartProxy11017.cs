using System.Collections.Generic;
using UnityEngine;
using System;
using GameModule;
using System.Threading.Tasks;

namespace GameModule
{
	public class SubRoundStartProxy11017 : SubRoundStartProxy
	{
		public SubRoundStartProxy11017(MachineContext context)
		:base(context)
		{

			
		}
		
		protected override void HandleCommonLogic()
		{
			machineContext.view.Get<LinkRemaining11017>().RefreshReSpinCount(true);
			if (machineContext.state.Get<FreeSpinState>().IsInFreeSpin)
			{
				machineContext.view.Get<SuperFreeGameIcon11017>().ShowSuperFreeIcon();
			}
			base.HandleCommonLogic();
		}
		
	}
}