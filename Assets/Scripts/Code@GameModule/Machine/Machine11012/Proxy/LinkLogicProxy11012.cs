using System.Threading.Tasks;
using UnityEngine;

namespace GameModule
{
    public class LinkLogicProxy11012: LinkLogicProxy
    {
        private WheelState linkWheelState;
        private WheelsActiveState11012 wheelsActiveState;
        private LinkLockView11012 linkLockView;
        private BonusWinView11012 bonusWinView;
        public LinkLogicProxy11012(MachineContext context) : base(context)
        {
            linkWheelState = context.state.Get<WheelState>(1);
            wheelsActiveState = machineContext.state.Get<WheelsActiveState11012>();
            linkLockView = machineContext.view.Get<LinkLockView11012>();
            bonusWinView = machineContext.view.Get<BonusWinView11012>();
        }
        
        
        protected override string GetLinkBeginAddress()
        {
        	return "UILinkGameStart" + machineContext.assetProvider.AssetsId;;
        }

        protected override string GetLinkFinishAddress()
        {
            return "UILinkGameFinish" + machineContext.assetProvider.AssetsId;;
        }
        
        
        
        protected override async Task HandleLinkBeginCutSceneAnimation()
        {
			
            
            var wheelsActiveState = machineContext.state.Get<WheelsActiveState11012>();
            wheelsActiveState.UpdateRunningWheelState(true,false);

			
			
            
			
          
            await RecoverLogicStateAsync();
			
            
        }
       
        
        
        protected override void RecoverLogicState()
        {
            RecoverLogicStateAsync();
        }
        
        protected async Task RecoverLogicStateAsync()
        {
            linkLockView.Open();
			
            var bonusWinView = machineContext.view.Get<BonusWinView11012>();
            bonusWinView.Close();
            bonusWinView.RefreshWin(0);
			
            ResetLinkWheels();
            await UpdateLinkWheelLockElements();
        }
        
        
        protected override async Task HandleLinkFinishCutSceneAnimation()
        {
            this.linkLockView.Close();
            bool isFree = machineContext.state.Get<FreeSpinState>().IsInFreeSpin;
            wheelsActiveState.UpdateRunningWheelState(false,isFree);
            
            var jackpotView = machineContext.view.Get<JackPotPanel>();
            jackpotView.transform.gameObject.SetActive(true);

            RestoreTriggerWheelElement();
            Constant11012.ClearDoor(machineContext);
        }
        
        
        protected async override Task HandleLinkGameTrigger()
        {
            
        }
		
		
        private void ResetLinkWheels()
        {
            var items = machineContext.state.Get<ExtraState11012>().GetLinkData().Items;
            for (int i = 0; i < items.Count; i++)
            {
                var item = items[i];
                int id = (int) item.PositionId;
                UpdateRunningElement(Constant11012.NormalLinkElementId, id);
                linkWheelState.SetRollLockState(id, false);
                
            }
        }
        
        
        private async Task UpdateLinkWheelLockElements()
        {
            var items = machineContext.state.Get<ExtraState11012>().GetLinkData().Items;

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
                    container.ShiftSortOrder(true);
                    lockNum++;
                }
            }

            
        }
        
        
        protected async override Task HandleLinkGame()
        {
            await UpdateLinkWheelLockElements();
        }
        
        
        protected async override Task HandleLinkReward()
        {

            if (!IsFromMachineSetup())
            {
	            
                this.linkLockView.Close();
                var bonusWinView = machineContext.view.Get<BonusWinView11012>();
                var jackpotView = machineContext.view.Get<JackPotPanel>();
                jackpotView.transform.gameObject.SetActive(false);
                bonusWinView.Open();
                
                var extraState = machineContext.state.Get<ExtraState11012>();
                var linkData = extraState.GetLinkData();
                if (linkData.FullWinRate > 0)
                {
                    await Constant11012.ShowJackpot(machineContext, this, 5, linkData.FullWinRate, false);
                    var jackpotWin = machineContext.state.Get<BetState>().GetPayWinChips(linkData.FullWinRate);
                    await bonusWinView.AddWin((long) jackpotWin);
                }
                
                
                await Constant11012.BounsFly(machineContext,this, bonusWinView);
                await machineContext.WaitSeconds(0.5f);
            }

        }
        
        
        
        protected async override Task HandleLinkFinishPopup()
        {
            await base.HandleLinkFinishPopup();
            this.linkLockView.Close();
            var bonusWinView = machineContext.view.Get<BonusWinView11012>();

            long winNum = bonusWinView.GetWinNum();
            if (winNum == 0)
            {
                var jackpotView = machineContext.view.Get<JackPotPanel>();
                jackpotView.transform.gameObject.SetActive(false);
                bonusWinView.Open();
				 
                //重连需要恢复
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