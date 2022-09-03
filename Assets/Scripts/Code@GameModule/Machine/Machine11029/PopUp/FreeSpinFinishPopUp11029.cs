using System.Collections.Generic;
using UnityEngine;
using System;
using GameModule;

namespace GameModule
{
	public class FreeSpinFinishPopUp11029 : FreeSpinFinishPopUp
	{
		public FreeSpinFinishPopUp11029(Transform transform)
		:base(transform)
		{

	
		}
		
		public override void OnOpen()
        {
            var freeSpinState = context.state.Get<FreeSpinState>();
	        if (freeSpinState.freeSpinId == 0)
	        {
		        AudioUtil.Instance.StopMusic();
		        AudioUtil.Instance.PlayAudioFx("FreeGameEnd_Open");
	        }
	        else if (freeSpinState.freeSpinId == 1)
	        {
		        AudioUtil.Instance.StopMusic();
		        AudioUtil.Instance.PlayAudioFx("FreeGameEnd_Open");
	        }
	        else if (freeSpinState.freeSpinId == 2)
	        {
		        AudioUtil.Instance.StopMusic();
		        AudioUtil.Instance.PlayAudioFx("Map_ClassicEnd_Open");
	        }
	        else if (freeSpinState.freeSpinId == 3 || freeSpinState.freeSpinId == 4 || freeSpinState.freeSpinId == 5)
	        {
		        AudioUtil.Instance.StopMusic();
		        AudioUtil.Instance.PlayAudioFx("Map_FreeGameEnd_Open");
	        }
	        else
	        {
		        AudioUtil.Instance.StopMusic();
		        AudioUtil.Instance.PlayAudioFx("Map_FreeGameEnd_Open");
	        }
	        if(animator != null && animator.HasState("Open"))
                animator.Play("Open");
        }
	}
}