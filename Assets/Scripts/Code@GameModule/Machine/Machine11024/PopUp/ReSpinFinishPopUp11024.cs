using System.Collections.Generic;
using UnityEngine;
using System;
using GameModule;
using UnityEngine.UI;

namespace GameModule
{
	public class ReSpinFinishPopUp11024 : ReSpinFinishPopUp
	{
		public ReSpinFinishPopUp11024(Transform transform)
		:base(transform)
		{
			txtReSpinWinChips = transform.Find("Root/MainGroup/IntegralGroup/IntegralText").GetComponent<Text>();
			collectButton = transform.Find("Root/CollectButton").GetComponent<Button>();
			if (collectButton)
			{
				collectButton.onClick.AddListener(async () =>
				{
					if (!isClosing)
					{
						isClosing = true;
						AudioUtil.Instance.PlayAudioFx("Close");
						collectButton.enabled = false;
						await context.state.Get<ReSpinState>().SettleReSpin();
						Close();
					}
				});   
			}
		}

	}
}