using System.Collections.Generic;
using UnityEngine;
using System;
using GameModule;
using UnityEngine.UI;

namespace GameModule
{
	public class FreeSpinReTriggerPopUp11030 : FreeSpinReTriggerPopUp
	{
		public FreeSpinReTriggerPopUp11030(Transform transform)
		:base(transform)
		{

			_extraCountText = transform.Find("Root/MainGroup/Text").GetComponent<Text>();
		}
		public override async void SetExtraCount(uint extraCount)
		{
			if(_extraCountText)
				_extraCountText.text = extraCount.ToString();
            
			await XUtility.PlayAnimationAsync(animator,"Open");
            
			Close();
		}
	}
}