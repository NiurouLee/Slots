using System.Collections.Generic;
using UnityEngine;
using System;
using GameModule;

namespace GameModule
{
	public class SubRoundStartProxy11019 : SubRoundStartProxy
	{
		public SubRoundStartProxy11019(MachineContext context)
		:base(context)
		{

			
		}


		protected override void HandleCommonLogic()
		{
			machineContext.view.Get<LinkLockView11019>().RefreshReSpinCount(true);
			
			base.HandleCommonLogic();
		}
	}
}