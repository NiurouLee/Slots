using System.Threading.Tasks;
using UnityEngine;

namespace GameModule
{
    public class LinkLogicProxy11301: LinkLogicProxy
    {
        private WheelState linkWheelState;
        private WheelsActiveState11301 wheelsActiveState;
        private LinkLockView11301 linkLockView;
        private BonusWinView11301 bonusWinView;
        public LinkLogicProxy11301(MachineContext context) : base(context)
        {
            linkWheelState = context.state.Get<WheelState>(1);
            wheelsActiveState = machineContext.state.Get<WheelsActiveState11301>();
            linkLockView = machineContext.view.Get<LinkLockView11301>();
            bonusWinView = machineContext.view.Get<BonusWinView11301>();
        }
        
        public bool UseAverageBet()
        {
            return (reSpinState as ReSpinState11301).IsAvgBet;
        }
        protected override string GetLinkBeginAddress()
        {
        	return "UILinkGameStart" + machineContext.assetProvider.AssetsId;;
        }

        protected override string GetLinkFinishAddress()
        {
            return "UILinkGameFinish" + machineContext.assetProvider.AssetsId;;
        }
        
        protected override async Task HandleLinkBeginPopup()
        {
            if (machineContext.assetProvider.GetAsset<GameObject>(GetLinkBeginAddress()) != null)
            {
                var task = GetWaitTask();
                var startLinkPopup = PopUpManager.Instance.ShowPopUp<ReSpinStartPopUp>(GetLinkBeginAddress());
                if (startLinkPopup != null)
                {
                    startLinkPopup.SetPopUpCloseAction(() =>
                    {
                        SetAndRemoveTask(task);
                    });
                    if (startLinkPopup.IsAutoClose())
                    {
                        await machineContext.WaitSeconds(GetLinkBeginPopupDuration());
                        startLinkPopup.Close();     
                    }
                }
                else
                {
                    SetAndRemoveTask(task);
                }

                await task.Task;
            }
            await Task.CompletedTask;
        }
        
        protected override async Task HandleLinkFinishPopup()
        {
            if (machineContext.assetProvider.GetAsset<GameObject>(GetLinkFinishAddress()) != null)
            {
                var task = GetWaitTask();
                var finishLinkPopup = PopUpManager.Instance.ShowPopUp<ReSpinFinishPopUp>(GetLinkFinishAddress());
                if (!ReferenceEquals(finishLinkPopup, null))
                {
                    finishLinkPopup.SetPopUpCloseAction(() =>
                    {
                        finishLinkPopup.Close();
                        SetAndRemoveTask(task);
                    });
                    finishLinkPopup.Initialize(machineContext);
                    if (finishLinkPopup.IsAutoClose())
                    {
                        await machineContext.WaitSeconds(GetLinkFinishPopupDuration());
                        finishLinkPopup.Close();
                    }
                }
                await task.Task;
            }
            await Task.CompletedTask;

            this.linkLockView.Close();
            var bonusWinView = machineContext.view.Get<BonusWinView11301>();

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
            
            await SendToBalance(winNum);
        }
        protected override async Task HandleLinkBeginCutSceneAnimation()
        {
            var wheelsActiveState = machineContext.state.Get<WheelsActiveState11301>();
            wheelsActiveState.UpdateRunningWheelState(true,false);
            await RecoverLogicStateAsync();
			//AudioUtil.Instance.PlayAudioFx("Link_SpinRefresh");
            
        }
       
        
        
        protected override void RecoverLogicState()
        {
            machineContext.state.Get<BetState11301>().UpdateTotalBetForState();
            RecoverLogicStateAsync();
        }
        
        protected async Task RecoverLogicStateAsync()
        {
            linkLockView.Open();

			machineContext.view.Get<JackPotPanel>().transform.localPosition = new Vector3(0.88f,machineContext.view.Get<JackPotPanel>().transform.localPosition.y,0);
            var bonusWinView = machineContext.view.Get<BonusWinView11301>();
            bonusWinView.Close();
            bonusWinView.RefreshWin(0);
			machineContext.view.Get<TransitionView11301>().UpdateBgAnim(true);
            controlPanel.UpdateControlPanelState(false,UseAverageBet());
            ResetLinkWheels();
            await UpdateLinkWheelLockElements();
        }
        
        
        protected override async Task HandleLinkFinishCutSceneAnimation()
        {
            machineContext.view.Get<JackPotPanel>().transform.localPosition = new Vector3(0.55f,machineContext.view.Get<JackPotPanel>().transform.localPosition.y,0);
            this.linkLockView.Close();
            // 坑---当用到avgbet时，totalBet会变。所以需要恢复。
            
            if(!machineContext.state.Get<FreeSpinState>().IsTriggerFreeSpin)
                RestoreBet();
                
            Constant11301.IsShowMapFeature = machineContext.state.Get<ExtraState11301>().IsMapFeature();
            bool isFree = machineContext.state.Get<FreeSpinState>().IsInFreeSpin;
            wheelsActiveState.UpdateRunningWheelState(false,isFree);
            if(!isFree)
			    machineContext.view.Get<TransitionView11301>().UpdateBgAnim(false);
            controlPanel.UpdateControlPanelState(false,false);
            var jackpotView = machineContext.view.Get<JackPotPanel>();
            jackpotView.transform.gameObject.SetActive(true);
            
            RestoreTriggerWheelElement();
            Constant11301.ClearDoor(machineContext);
        }
        
        
        protected async override Task HandleLinkGameTrigger()
        {
            
        }
		
		
        private void ResetLinkWheels()
        {
            var items = machineContext.state.Get<ExtraState11301>().GetLinkData().Items;
            for (int i = 0; i < items.Count; i++)
            {
                var item = items[i];
                int id = (int) item.PositionId;
                UpdateRunningElement(Constant11301.NormalLinkElementId, id);
                linkWheelState.SetRollLockState(id, false);
                
            }
        }
        
        
        private async Task UpdateLinkWheelLockElements()
        {
            var items = machineContext.state.Get<ExtraState11301>().GetLinkData().Items;

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
                var bonusWinView = machineContext.view.Get<BonusWinView11301>();
                var jackpotView = machineContext.view.Get<JackPotPanel>();
                jackpotView.transform.gameObject.SetActive(false);
                bonusWinView.Open();
                
                var extraState = machineContext.state.Get<ExtraState11301>();
                var linkData = extraState.GetLinkData();
                if (linkData.FullWinRate > 0)
                {
                    // 先播一遍所有图标trigger动画
                    AudioUtil.Instance.PlayAudioFx("J01_Trigger");
                    AllBonusElmentShowTrigger("Trigger");
                    await XUtility.WaitSeconds(2);
                    AllBonusElmentShowTrigger("Idle");
                    await Constant11301.ShowJackpot(machineContext, this, 5, linkData.FullWinRate, false);
                    var jackpotWin = machineContext.state.Get<BetState>().GetPayWinChips(linkData.FullWinRate);
                    await bonusWinView.AddWin((long) jackpotWin);
                }
                
                
                await Constant11301.BounsFly(machineContext,this,bonusWinView);
                await machineContext.WaitSeconds(0.5f);
            }

        }


        private TaskCompletionSource<bool> waitNumTask;
        protected async Task SendToBalance(long jackpotWin)
        {
             waitNumTask = new TaskCompletionSource<bool>();
             AddWinChipsToControlPanel((ulong)jackpotWin);
             float time = machineContext.view.Get<ControlPanel>().GetNumAnimationEndTime();
             
             if (time > 0.5f)
             {
                 if(!machineContext.state.Get<AutoSpinState>().IsAutoSpin)
                    machineContext.view.Get<ControlPanel>().ShowStopButton(true);
             }
             
             await waitNumTask.Task;
             // float time = machineContext.view.Get<ControlPanel>().GetNumAnimationEndTime();
             // await machineContext.WaitSeconds(time);
        }
        
        public override void OnMachineInternalEvent(MachineInternalEvent internalEvent, params object[] args)
        {
            if (waitNumTask != null && !waitNumTask.Task.IsCompleted)
            {
                switch (internalEvent)
                {
                    case MachineInternalEvent.EVENT_WAIT_EVENT_COMPLETE:
                    {
                        var waitEvent = (WaitEvent) args[0];
                        if (waitEvent == WaitEvent.WAIT_WIN_NUM_ANIMTION)
                        {
                            machineContext.view.Get<ControlPanel>().ShowSpinButton(false);

                            waitNumTask.SetResult(true);
                        }

                        break;
                    }

                    case MachineInternalEvent.EVENT_CONTROL_STOP:
                        machineContext.view.Get<ControlPanel>().StopWinAnimation();
                        break;

                    case MachineInternalEvent.EVENT_CONTROL_AUTO_SPIN_STOP:
                        if (machineContext.state.Get<AutoSpinState>().IsAutoSpin)
                        {
                            machineContext.state.Get<AutoSpinState>().OnDisableAutoSpin();
                            machineContext.view.Get<ControlPanel>().ShowStopButton(true);
                        }

                        break;
                }
            }

            base.OnMachineInternalEvent(internalEvent, args);
        }
        
        private void AllBonusElmentShowTrigger(string name){
            var wheel = machineContext.state.Get<WheelsActiveState11301>().GetRunningWheel()[0];
            int XCount = wheel.rollCount;
	        int YCount = wheel.GetRoll(0).lastVisibleRowIndex - wheel.GetRoll(0).firstVisibleRowIndex;

	        
	        for (int x = 0; x < XCount; x++)
	        {
		        for (int y = 0; y < YCount; y++){
                    var container = wheel.GetRoll(x).GetVisibleContainer(y);
			        uint elementId = container.sequenceElement.config.id;
			        if (Constant11301.ListNormalCoins.Contains(elementId) || Constant11301.ListNormalJackpotCoins.Contains(elementId) 
                        || Constant11301.ListDoorCoins.Contains(elementId) || Constant11301.ListDoorJackpotCoins.Contains(elementId))
			        {
                        container.PlayElementAnimation(name);
                    }
                }
            }
        }
    }
}