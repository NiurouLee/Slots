using System.Collections.Generic;
using UnityEngine;
using System;
using GameModule;

namespace GameModule
{
	public class FreeSpinStartPopUp11025 : FreeSpinStartPopUp
	{
		public FreeSpinStartPopUp11025(Transform transform)
		:base(transform)
		{

	
		}
		public override float GetPopUpMaskAlpha()
		{
			return 0.85f;
		}
	}
}