using System.Collections.Generic;
using UnityEngine;
using System;
using GameModule;

namespace GameModule
{
	public class NextSpinPrepareProxy11030 : NextSpinPrepareProxy
	{
		public NextSpinPrepareProxy11030(MachineContext context)
		:base(context)
		{

	
		}
		protected override void OnBetChange()
		{
			base.OnBetChange();
			var runningWheel = machineContext.state.Get<WheelsActiveState11030>().GetRunningWheel()[0];
			for (var i = 0; i < runningWheel.rollCount; i++)
			{
				var roll = runningWheel.GetRoll(i);
				for (var j = 0; j < roll.rowCount; j++)
				{
					var container = roll.GetVisibleContainer(j);
					if (Constant11030.ValueList.Contains(container.sequenceElement.config.id))
					{
						((ElementValue11030)container.GetElement()).UpdateElementContent();
					}
					else if (Constant11030.TrainList.Contains(container.sequenceElement.config.id))
					{
						((ElementTrain11030)container.GetElement()).UpdateElementContent();
					}
				}
			}
		}
	}
}