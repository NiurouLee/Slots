using System.Collections.Generic;
using UnityEngine;
using System;
using GameModule;

namespace GameModule
{
	public class FreeSpinFinishPopUp11024 : FreeSpinFinishPopUp
	{
		public FreeSpinFinishPopUp11024(Transform transform)
		:base(transform)
		{

	
		}
		public override void OnOpen()
		{
			AudioUtil.Instance.PlayAudioFxOneShot("MapGameEnd_Open");
			if(animator != null && animator.HasState("Open"))
				animator.Play("Open");
		}
	}
}