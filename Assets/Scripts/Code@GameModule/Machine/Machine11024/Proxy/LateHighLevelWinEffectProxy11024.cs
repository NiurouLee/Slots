using System.Collections.Generic;
using UnityEngine;
using System;
using GameModule;

namespace GameModule
{
	public class LateHighLevelWinEffectProxy11024 : LateHighLevelWinEffectProxy
	{
		public LateHighLevelWinEffectProxy11024(MachineContext context)
		:base(context)
		{

	
		}
		protected override async void HandleCustomLogic()
		{
			await WinEffectHelper.ShowBigWinEffectAsync(_winState.winLevel, _winState.displayTotalWin,
				machineContext);
			ForceUpdateWinChipsToDisplayTotalWin();
			Proceed();
		}
	}
}