using System.Collections.Generic;
using UnityEngine;
using System;
using GameModule;

namespace GameModule
{
	public class MachineSetUpProxy11025 : MachineSetUpProxy
	{
		public MachineSetUpProxy11025(MachineContext context)
		:base(context)
		{

	
		}
		protected override void UpdateViewWhenRoomSetUp()
		{
			base.UpdateViewWhenRoomSetUp();
			var extraState = machineContext.state.Get<ExtraState11025>();
			var shopView = machineContext.view.Get<ShopView11025>();
			shopView.SetShopData(extraState.GetShopData());
			shopView.SetPageToPlayingPage();
			shopView.RefreshAll();
			var baseWheel = machineContext.view.Get<WheelBase11025>();
			baseWheel.InitShopLockState();
			baseWheel.SetStoreCoin(extraState.GetNowShopCoins());
		}
	}
}