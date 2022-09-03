using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading.Tasks;
using GameModule;

namespace GameModule
{
	public class LinkLogicProxy11019 : LinkLogicProxy
	{
		private WheelState linkWheelState;
		private WheelsActiveState11019 wheelsActiveState;
		private LinkLockView11019 linkLockView;
		public LinkLogicProxy11019(MachineContext context)
		:base(context)
		{
			linkWheelState = context.state.Get<WheelStateLink11019>();
			wheelsActiveState = machineContext.state.Get<WheelsActiveState11019>();
			linkLockView = machineContext.view.Get<LinkLockView11019>();
		}
		
		
		// protected override string GetLinkBeginAddress()
		// {
		// 	return "UIEpicRespinStart" + machineContext.assetProvider.AssetsId;;
		// }

		protected override string GetLinkFinishAddress()
		{
			return "UIEpicRespinFinish" + machineContext.assetProvider.AssetsId;;
		}
		
		
		
		protected override async Task HandleLinkBeginPopup()
		{
			var wheel = wheelsActiveState.GetRunningWheel()[0];
			var animatorChillLink = wheel.transform.Find("ChilliLink").GetComponent<Animator>();
			animatorChillLink.gameObject.SetActive(true);
			AudioUtil.Instance.PlayAudioFx("LinkGameStart_Open");
			await XUtility.PlayAnimationAsync(animatorChillLink, "ChilliLink", machineContext);
			animatorChillLink.gameObject.SetActive(false);
		}
		
		
		protected override async Task HandleLinkBeginCutSceneAnimation()
		{
			await machineContext.view.Get<TransitionsView11019>().PlayManLink();
			machineContext.view.Get<TransitionsView11019>().PlayLinkTransition();
			await machineContext.WaitSeconds(1.6f);
			
			machineContext.view.Get<BonusWinView11019>(Constant11019.LinkBonusWinViewName).Close();

			var wheelsActiveState = machineContext.state.Get<WheelsActiveState11019>();
			wheelsActiveState.UpdateRunningWheelState(true,false);

			var tranRolls = this.wheelsActiveState.GetRunningWheel()[0].transform.Find("Rolls");
			tranRolls.gameObject.SetActive(false);
			
			Animator animatorWheel = machineContext.view.Get<Wheel>("WheelLinkGame").transform.GetComponent<Animator>();

			AudioUtil.Instance.PlayAudioFx("Link_Expanding");
			await XUtility.PlayAnimationAsync(animatorWheel, "Open");
			await XUtility.PlayAnimationAsync(animatorWheel, "Idle");
			
			
			await linkLockView.Open();
			
			//await linkLockView.RefreshUI();
			//this.wheelsActiveState.GetRunningWheel()[0].ForceUpdateElementOnWheel(true,true);
			await RecoverLogicStateAsync();
			
			tranRolls.gameObject.SetActive(true);
            
		}
		
		


		protected override void RecoverLogicState()
		{
			RecoverLogicStateAsync();
		}

		protected async Task RecoverLogicStateAsync()
		{
			linkLockView.OpenReSpin();
			Animator animatorWheel = wheelsActiveState.GetRunningWheel()[0].transform.GetComponent<Animator>();
			await XUtility.PlayAnimationAsync(animatorWheel, "Idle");
			
			var bonusWinView = machineContext.view.Get<BonusWinView11019>(Constant11019.LinkBonusWinViewName);
			bonusWinView.Close();
			bonusWinView.RefreshWin(0);
			
			ResetLinkWheels();
			await UpdateLinkWheelLockElements(true);
		}
		
		
		protected override async Task HandleLinkFinishCutSceneAnimation()
		{
			var transitionsView = machineContext.view.Get<TransitionsView11019>();
			transitionsView.PlayLinkTransition();

			await machineContext.WaitSeconds(1.6f);
			
			this.linkLockView.Close();
			
			bool isFree = machineContext.state.Get<FreeSpinState>().IsInFreeSpin;
			wheelsActiveState.UpdateRunningWheelState(false,isFree);

			RestoreTriggerWheelElement();
			
			List<Task> listTask = new List<Task>();
			
			listTask.Add(transitionsView.PlayManBase());
			listTask.Add( machineContext.WaitSeconds(3-1.6f));

			await Task.WhenAll(listTask);
		}


		protected async override Task HandleLinkGameTrigger()
		{
            
		}
		
		
		private void ResetLinkWheels()
		{
			var items = machineContext.state.Get<ExtraState11019>().GetLinkData().Items;
			for (int i = 0; i < items.Count; i++)
			{
				var item = items[i];
				int id = (int) item.PositionId;
				UpdateRunningElement(Constant11019.NormalLinkElementId, id);
				linkWheelState.SetRollLockState(id, false);
                
			}
		}
		
		
		private async Task UpdateLinkWheelLockElements(bool isSetupRoom)
		{
			var items = machineContext.state.Get<ExtraState11019>().GetLinkData().Items;

			int lockNum = 0;
			foreach (var item in items)
			{
				int id = (int) item.PositionId;
				if (!linkWheelState.IsRollLocked(id) && item.SymbolId>0)
				{
					//elementContainer.transform.position = GetElementPosition(id);
					var container = UpdateRunningElement(item.SymbolId, id,0,false);
					linkWheelState.SetRollLockState(id, true);
					container.PlayElementAnimation("Idle");
					lockNum++;
				}
			}

			if (lockNum > 0)
			{
				await linkLockView.RefreshUI(isSetupRoom);
			}
		}
		
		
		protected async override Task HandleLinkGame()
		{
			await UpdateLinkWheelLockElements(false);
		}
		
		
		 protected async override Task HandleLinkReward()
        {

            if (!IsFromMachineSetup())
            {
	            
	            this.linkLockView.CloseReSpin();
	            //var extraState = machineContext.state.Get<ExtraState11019>();
                var bonusWinView = machineContext.view.Get<BonusWinView11019>(Constant11019.LinkBonusWinViewName);
                var jackpotView = machineContext.view.Get<JackPotPanel>();
                jackpotView.transform.gameObject.SetActive(false);
                bonusWinView.Open();
                
                await Constant11019.BounsFly(machineContext,this, bonusWinView);

            }

        }
		 
		 
		 protected async override Task HandleLinkFinishPopup()
		 {
			 await base.HandleLinkFinishPopup();
			 this.linkLockView.CloseReSpin();
			 var bonusWinView = machineContext.view.Get<BonusWinView11019>(Constant11019.LinkBonusWinViewName);

			 long winNum = bonusWinView.GetWinNum();
			 if (winNum == 0)
			 {
				 var jackpotView = machineContext.view.Get<JackPotPanel>();
				 jackpotView.transform.gameObject.SetActive(false);
				 bonusWinView.Open();
				 
				 //重连需要恢复
				 // var extraState = machineContext.state.Get<ExtraState11019>();
				 // winNum = extraState.GetRespinTotalWin();
				 var respinInfo = machineContext.state.Get<ReSpinState>();
				 winNum = (long)respinInfo.GetRespinTotalWin();
                 
                
				 await bonusWinView.StartWin();
				 bonusWinView.RefreshWin(winNum);
			 }
            
			 await SendToBelence(winNum);
		 }


		 protected async Task SendToBelence(long jackpotWin)
		 {
			 AddWinChipsToControlPanel((ulong)jackpotWin,0,true,false);
			 float time = machineContext.view.Get<ControlPanel>().GetNumAnimationEndTime();
			 await machineContext.WaitSeconds(time);
		 }

	}
}