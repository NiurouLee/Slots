using System.Collections.Generic;
using UnityEngine;
using System;
using GameModule;

namespace GameModule
{
	public class RoundStartProxy11025 : RoundStartProxy
	{
		public RoundStartProxy11025(MachineContext context)
		:base(context)
		{

	
		}

		protected override void HandleCustomLogic()
		{
			machineContext.view.Get<WheelBase11025>().ReducePointOnSpinning();
			base.HandleCustomLogic();
		}
	}
}