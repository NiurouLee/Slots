using System.Collections.Generic;
using UnityEngine;
using System;
using GameModule;

namespace GameModule
{
	public class SubRoundStartProxy11030 : SubRoundStartProxy
	{
		public SubRoundStartProxy11030(MachineContext context)
		:base(context)
		{

	
		}
		protected override void PlayBgMusic()
		{

			//Debug.LogError($"=======NextIsReSpin:{machineContext.state.Get<ReSpinState>().NextIsReSpin}");
			if (machineContext.state.Get<ReSpinState>().NextIsReSpin)
			{
				//Debug.LogError("====play");
				AudioUtil.Instance.PlayMusic(machineContext.machineConfig.audioConfig.GetFreeBackgroundMusicName());
			}
			else if (machineContext.state.Get<FreeSpinState>().NextIsFreeSpin)
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