using System.Collections.Generic;
using UnityEngine;
using System;
using GameModule;

namespace GameModule
{
	public class WheelStateFree11024 : WheelState
	{
		public WheelStateFree11024(MachineState state)
		:base(state)
		{

	
		}
		public override IRollUpdaterEasingConfig GetEasingConfig()
		{
			return machineState.machineConfig.GetEasingConfig(wheelConfig.normalEasingName);
		}
	}
}