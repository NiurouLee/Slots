using System.Collections.Generic;
using UnityEngine;
using System;
using GameModule;

namespace GameModule
{
	public class SubRoundStartProxy11025 : SubRoundStartProxy
	{
		public SubRoundStartProxy11025(MachineContext context)
		:base(context)
		{

	
		}
		protected override void PlayBgMusic()
		{
			if (machineContext.state.Get<FreeSpinState>().NextIsFreeSpin)
			{
				AudioUtil.Instance.PlayMusic(machineContext.machineConfig.audioConfig.GetFreeBackgroundMusicName());
			}
			else
			{
				AudioUtil.Instance.PlayMusic(machineContext.machineConfig.audioConfig.GetBaseBackgroundMusicName());
			}
		}
	}
}