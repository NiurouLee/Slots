using System.Collections.Generic;
using UnityEngine;
using System;
using GameModule;

namespace GameModule
{
	public class MachineSetUpProxy11022 : MachineSetUpProxy
	{
		public MachineSetUpProxy11022(MachineContext context)
		:base(context)
		{

	
		}
		protected override void UpdateRunningWheelElement()
		{
			var wheelsRunningStatusState = machineContext.state.Get<WheelsActiveState>();
			wheelsRunningStatusState.SetSpinningOder(WheelSpinningOrder.SAME_TIME_START_ONE_BY_ONE_STOP);

			var wheels = wheelsRunningStatusState.GetRunningWheel();

			for (var i = 0; i < wheels.Count; i++)
			{
				wheels[i].ForceUpdateElementOnWheel();
				var runningWheel = wheels[i];
				if (runningWheel.wheelName == "WheelBaseGame" || runningWheel.wheelName == "WheelFreeGame")
				{
					for (var i1 = 0; i1 < runningWheel.GetMaxSpinningUpdaterCount(); i1++)
					{
						var roll = runningWheel.GetRoll(i1);
						for (var j = 0; j < runningWheel.GetRollRowCount(0,runningWheel.wheelState.GetWheelConfig()); j++)
						{
							var container = roll.GetVisibleContainer(j);
							if (container.sequenceElement.config.id == 13)
							{
								container.UpdateElement(container.sequenceElement,true);
								container.UpdateElementMaskInteraction(true);
								container.ShiftSortOrder(true);
							}
						}
					}
				}
			}
		}
	}
}