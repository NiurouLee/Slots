namespace GameModule
{
	public class SpecialBonusProxy11005 : SpecialBonusProxy
	{
		public SpecialBonusProxy11005(MachineContext context) : base(context)
		{
		}

		protected override void RegisterInterestedWaitEvent()
		{
			waitEvents.Add(WaitEvent.WAIT_WIN_NUM_ANIMTION);
		}


		protected override async void HandleCustomLogic()
		{
			//Test
			//var bonusResult = await Client.Get<FeatureMachine>().SendBonusResult(null);
			//Proceed();
			
			machineContext.view.Get<BaseLetterView11005>().SetBtnGetMoreEnable(false);
			

			
			
			StartBonusGame();
		}


		protected override void RecoverLogicState()
		{
			RestoreTriggerWheelElement();
		}


		protected async virtual void StartBonusGame()
		{
			AudioUtil.Instance.StopMusic();
			await machineContext.view.Get<BaseLetterView11005>().PlayUnlockAnim();

			var popUp = PopUpManager.Instance.ShowPopUp<UIMiniGameStart11005>("UIMiniGameStart11005");
			await popUp.RefreshUI();
			ShowMiniGameView();
		}


		protected virtual async void ShowMiniGameView()
		{
			
			var popUp = PopUpManager.Instance.ShowPopUp<UIMiniGame11005>("UIMiniGame");
			
			//AudioUtil.Instance.PlayMusic(machineContext.machineConfig.audioConfig.GetBonusBackgroundMusicName(),true);

			await popUp.RefreshUI();

			machineContext.view.Get<BaseLetterView11005>().RefreshUINoAnim();

			var extralState = machineContext.state.Get<ExtraState11005>();
			var mapInfoProcess = extralState.GetMapInfo();

			ulong winNum = extralState.GetMapInfo().LetterWin + mapInfoProcess.StepWin;
			if ( winNum > 0)
			{
				if (machineContext.state.Get<WinState>().winLevel < (int)WinLevel.BigWin)
				{
					AddWinChipsToControlPanel(winNum);
				}
			}


			if (!machineContext.state.Get<FreeSpinState>().IsTriggerFreeSpin)
			{
				machineContext.view.Get<BaseLetterView11005>().SetBtnGetMoreEnable(true);
			}
 
			Proceed();
		}
		
		protected virtual void RestoreTriggerWheelElement()
		{
			var triggerPanels = machineContext.state.Get<ExtraState11005>().GetMapInfo().TriggeringPanels;
			machineContext.state.Get<WheelState>().UpdateWheelStateInfo(triggerPanels[0]);
            
			machineContext.view.Get<Wheel>().ForceUpdateElementOnWheel();
			
			
			//把字母替换掉
			var listWheels = machineContext.state.Get<WheelsActiveState11005>().GetRunningWheel();
			var listElement = listWheels[0].GetElementMatchFilter((container) =>
			{
				if (container.sequenceElement.config.id >= 15 &&
				    container.sequenceElement.config.id <= 114)
				{
					return true;
				}
                
				return false;
                
			}, 0);
            
			var elementConfigSet = machineContext.state.machineConfig.GetElementConfigSet();
			for (int i = 0; i < listElement.Count; i++)
			{
				SequenceElement sequenceElement = null;
				string strId = listElement[i].sequenceElement.config.name;
				strId = strId.Replace("S", "");
				uint elementId = uint.Parse(strId);
				while (elementId > 10)
				{
					elementId -= 10;
				}

				string newId = elementId.ToString().PadLeft(2, '0');
				newId = $"S{newId}";

				sequenceElement = new SequenceElement(elementConfigSet.GetElementConfigByName(newId),
					machineContext);

				listElement[i].UpdateElement(sequenceElement);
			}
		}
		
	}
}