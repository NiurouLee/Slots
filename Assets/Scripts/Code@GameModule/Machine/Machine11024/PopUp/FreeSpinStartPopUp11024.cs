using System.Collections.Generic;
using UnityEngine;
using System;
using GameModule;
using UnityEngine.UI;

namespace GameModule
{
	public class FreeSpinStartPopUp11024 : FreeSpinStartPopUp
	{
		public FreeSpinStartPopUp11024(Transform transform)
		:base(transform)
		{
			startButton = transform.Find("Root/CollectButton").GetComponent<Button>();
			startButton.onClick.AddListener(OnStartClicked);
			freeSpinCountText = transform.Find("Root/MainGroup/IntegralGroup/IntegralText").GetComponent<Text>();
		}

		public override void OnOpen()
		{
			AudioUtil.Instance.PlayAudioFx("MapGameStart_Open");
			if(animator != null && animator.HasState("Open"))
				animator.Play("Open");
		}
	}
}