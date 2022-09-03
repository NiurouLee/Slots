using System.Collections.Generic;
using UnityEngine;
using System;
using GameModule;

namespace GameModule
{
	public class MachineSetUpProxy11024 : MachineSetUpProxy
	{
		public MachineSetUpProxy11024(MachineContext context)
		:base(context)
		{

	
		}
		protected override void UpdateViewWhenRoomSetUp()
		{
			base.UpdateViewWhenRoomSetUp();
			var extraState = machineContext.state.Get<ExtraState11024>();
			machineContext.view.Get<CollectBarView11024>().InitState();
			machineContext.view.Get<PigGroupView11024>().InitView();
			machineContext.view.Get<MapView11024>().InitState();
		}
	}
}