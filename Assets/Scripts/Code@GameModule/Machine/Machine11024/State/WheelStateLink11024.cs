using System.Collections.Generic;
using UnityEngine;
using System;
using GameModule;

namespace GameModule
{
	public class WheelStateLink11024 : WheelState
	{
		public WheelStateLink11024(MachineState state)
		:base(state)
		{

	
		}
		public override IRollUpdaterEasingConfig GetEasingConfig()
		{
			return machineState.machineConfig.GetEasingConfig(wheelConfig.normalEasingName);
		}
	}
}