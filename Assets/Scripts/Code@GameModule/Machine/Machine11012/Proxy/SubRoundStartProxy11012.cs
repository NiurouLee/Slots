using System.Collections.Generic;
using UnityEngine;
using System;
using GameModule;

namespace GameModule
{
	public class SubRoundStartProxy11012 : SubRoundStartProxy
	{
		public SubRoundStartProxy11012(MachineContext context)
		:base(context)
		{

		}

		protected override void HandleCustomLogic()
		{
			machineContext.view.Get<DoorView11012>().RollingStartLockFreeDoor();
			machineContext.view.Get<DoorView11012>().OpenLinkAnticipation();
			machineContext.view.Get<LinkLockView11012>().RefreshReSpinCount(true);

			base.HandleCustomLogic();
		}
	}
}