using System.Collections.Generic;
using UnityEngine;
using System;
using GameModule;

namespace GameModule
{
	public class ReSpinState11012 : ReSpinState
	{
		public ReSpinState11012(MachineState state)
		:base(state)
		{

	
		}
		
		public override ulong GetRespinTotalWin()
		{
			return (ulong)machineState.machineContext.state.Get<ExtraState11012>().GetRespinTotalWin();
			//return this.ReSpinTototalWin;
		}

	}
}