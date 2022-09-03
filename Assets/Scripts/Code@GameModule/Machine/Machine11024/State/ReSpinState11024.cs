using System.Collections.Generic;
using UnityEngine;
using System;
using GameModule;

namespace GameModule
{
	public class ReSpinState11024 : ReSpinState
	{
		public ReSpinState11024(MachineState state)
		:base(state)
		{

	
		}

		public override ulong GetRespinTotalWin()
		{
			ulong totalWinNum = 0;
			var extraState = machineState.Get<ExtraState11024>();
			var betState = machineState.Get<BetState>();
			var linkCount = extraState.GetReSpinPanelCount();
			for (var i = 0; i < linkCount; i++)
			{
				totalWinNum += betState.GetPayWinChips(extraState.GetLinkData(i).FullWinRate);
				var linkData = extraState.GetLinkData(i).Items;
				for (var i1 = 0; i1 < linkData.Count; i1++)
				{
					var itemData = linkData[i1];
					if (itemData.SymbolId > 0)
					{
						var winRate = itemData.WinRate + itemData.JackpotPay;
						var winValue = betState.GetPayWinChips(winRate);
						totalWinNum += winValue;
					}
				}
			}
			return totalWinNum;
		}
	}
}