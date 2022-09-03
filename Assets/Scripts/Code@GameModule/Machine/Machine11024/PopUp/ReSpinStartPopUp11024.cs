using System.Collections.Generic;
using UnityEngine;
using System;
using GameModule;
using UnityEngine.UI;

namespace GameModule
{
	public class ReSpinStartPopUp11024 : ReSpinStartPopUp
	{
		bool touchAvailable = false;
		public ReSpinStartPopUp11024(Transform transform)
		:base(transform)
		{
			startButton = transform.Find("Root").GetComponent<Button>();
			if (startButton)
			{
				startButton.onClick.AddListener(() =>
				{
					if (touchAvailable)
					{
						AudioUtil.Instance.PlayAudioFx("Close");
						Close();	
					}
				});
			}
		}
		public override void Initialize(MachineContext inContext)
		{
			base.Initialize(inContext);
			XUtility.PlayAnimation(animator,"Open1", () =>
			{
				context.WaitSeconds(0.5f, () =>
				{
					touchAvailable = true;
				});
			},context);
		}
		public override void OnOpen()
		{
			AudioUtil.Instance.PlayAudioFx("LinkGameStart_Open");
			if(animator != null && animator.HasState("Open1"))
				animator.Play("Open1");
		}
	}
}