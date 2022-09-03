using System.Collections.Generic;
using UnityEngine;
using System;
using DragonU3DSDK.Network.API.ILProtocol;
using GameModule;

namespace GameModule
{
	public class BonusProxy11022 : BonusProxy
	{
		public BonusProxy11022(MachineContext context)
		:base(context)
		{

	
		}
		
		protected override void HandleCustomLogic()
		{
			machineContext.view.Get<ControlPanel>().ShowSpinButton(false);
			StartBonusGame();
		}
		protected override void RegisterInterestedWaitEvent()
		{
			base.RegisterInterestedWaitEvent();
			waitEvents.Add(WaitEvent.WAIT_WIN_NUM_ANIMTION);
		}
		private async void StartBonusGame()
		{
			await machineContext.WaitSeconds(0.3f);
			AudioUtil.Instance.StopMusic();
			await BlinkBonusLine();
			AudioUtil.Instance.PlayAudioFx("SelectionFeature_Start");
			var popup = PopUpManager.Instance.ShowPopUp<UISelectionFeature11022>();
			popup.Initialize(machineContext);
			popup.SetChooseCallback(ChooseCallback);
			if (Constant11022.debugType)
			{
				machineContext.WaitSeconds(0.5f, () =>
				{
					popup.OnFreeBtnClicked();
				});
			}
		}

		private async void ChooseCallback(int choice)
		{
			CBonusProcess cBonusProcess = new CBonusProcess();
			cBonusProcess.Json = "Link";
			await machineContext.state.Get<ExtraState11022>().SendBonusProcess(choice == 0 ? cBonusProcess : null);
			Proceed();
		}
	}
}