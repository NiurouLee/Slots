using System.Collections.Generic;
using UnityEngine;
using System;
using GameModule;

namespace GameModule
{
	public class WheelsSpinningProxy11030 : WheelsSpinningProxy
	{
		public WheelsSpinningProxy11030(MachineContext context)
		:base(context)
		{

	
		}

		protected override void HandleCustomLogic()
		{
			((WheelTrain11030)runningWheel[0]).SetPurpleReel(false);
			base.HandleCustomLogic();
		}
	}
}