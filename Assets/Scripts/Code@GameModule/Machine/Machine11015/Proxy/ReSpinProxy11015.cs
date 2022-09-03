using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;

namespace GameModule
{
    public class ReSpinProxy11015: ReSpinLogicProxy
    {
        public ReSpinProxy11015(MachineContext context) : base(context)
        {
        }
        
        
        protected override void RegisterInterestedWaitEvent()
        {
            waitEvents.Add(WaitEvent.WAIT_WIN_NUM_ANIMTION);
            waitEvents.Add(WaitEvent.WAIT_BLINK_ALL_WIN_LINE);
        }
        
        
        protected override bool CheckIsAllWaitEventComplete()
        {
            if (IsInRespinProcess())
            {
                return base.CheckIsAllWaitEventComplete();
            }
            return true;
        }
        
        protected ControlPanel controlPanel;
        public override void SetUp()
        {
            base.SetUp();
            reSpinState = machineContext.state.Get<ReSpinState>();
            controlPanel = machineContext.view.Get<ControlPanel>();
        }

        protected override async void HandleCustomLogic()
        {
            //处理触发ReSpin：开始弹板或者过场动画
            if (IsReSpinTriggered() ||
                (this.IsFromMachineSetup() && !IsReSpinSpinFinished()))
            {
                StopBackgroundMusic();
                await HandleReSpinStartLogic();
                await HandleReSpinBeginCutSceneAnimation();
            }

            //处理ReSpin逻辑：锁图标或者其他
            if (IsInRespinProcess())
            {
                await HandleReSpinGame();
            }

            //是否ReSpin结束：处理结算过程
            if (IsReSpinSpinFinished())
            {
                StopBackgroundMusic();
                await HandleReSpinFinishCutSceneAnimation();
            }
            //ReSpin结算完成，恢复Normal
            if (NeedSettle())
            {
                if (IsFromMachineSetup())
                {
                    RefreshEndWild();
                }
                await SettleReSpin();
                

                StopBackgroundMusic();

                await HandleHighLevelEffect();
                
                await HandleFreeReTriggerLogic();
            }
            Proceed();
        }
        
        protected virtual async Task HandleHighLevelEffect()
        {
            var winState = machineContext.state.Get<WinState>();
            var freeSpinState = machineContext.state.Get<FreeSpinState>();
            if (!freeSpinState.IsTriggerFreeSpin)
            {
                if (winState.winLevel>= (int)WinLevel.BigWin)
                {
                    await WinEffectHelper.ShowBigWinEffectAsync(winState.winLevel, winState.displayCurrentWin,
                        machineContext);   
                }
            }

            
        }


        protected virtual void RefreshEndWild()
        {
            //先替换wild
            var elementConfigSet = machineContext.state.machineConfig.elementConfigSet;
            var extraState = machineContext.state.Get<ExtraState11015>();
            var listWildPos = extraState.GetWildsPos();
            if (listWildPos.Count > 0)
            {
                var wheel = machineContext.state.Get<WheelsActiveState>().GetRunningWheel()[0];
                foreach (var wildPos in listWildPos)
                {
                    var roll = wheel.GetRoll((int) wildPos.X);
                    var container = roll.GetVisibleContainer((int)wildPos.Y);
                    var elementConfig =
                        elementConfigSet.GetElementConfig(Constant11015.WildElementId);
                    container.UpdateElement(new SequenceElement(elementConfig,machineContext));
                    container.ShiftSortOrder(false);
                }
            }
            
            //替换宙斯锁
            var listZeusPos = extraState.GetZeusPos();
            if (listZeusPos.Count > 0)
            {
                var wheel = machineContext.state.Get<WheelsActiveState>().GetRunningWheel()[0];
                foreach (var wildPos in listZeusPos)
                {
                    var roll = wheel.GetRoll((int) wildPos.X);
                    var container = roll.GetVisibleContainer((int)wildPos.Y);
                    var elementConfig =
                        elementConfigSet.GetElementConfig(Constant11015.ZeusElementId);
                    container.UpdateElement(new SequenceElement(elementConfig,machineContext));
                    container.PlayElementAnimation("LockIdle");
                    container.ShiftSortOrder(true);
                    var element = container.GetElement() as ElementZeus11015;
                    element.IsLock = true;
                }
            }
        }


        protected virtual async Task HandleFreeReTriggerLogic()
        {

            var extraState = machineContext.state.Get<ExtraState11015>();
            var freeSpinState = machineContext.state.Get<FreeSpinState>();
            uint reTriggerCount = extraState.GetReTriggerCount();
            if (!freeSpinState.IsTriggerFreeSpin && reTriggerCount > 0)
            {
                
            
                var assetName = "UIFreeGameExtraNotice" + machineContext.assetProvider.AssetsId;
                if (machineContext.assetProvider.GetAsset<GameObject>(assetName) != null)
                {
                    
                    TaskCompletionSource<bool> taskCompletionSource = new TaskCompletionSource<bool>();
                    machineContext.AddWaitTask(taskCompletionSource,null);
                    
                    var popUp = ShowFreeSpinReTriggeredPopup(assetName);
                    popUp.SetPopUpCloseAction(() =>
                    {
                        UpdateFreeSpinUIState(true, false);
                        machineContext.RemoveTask(taskCompletionSource);
                        taskCompletionSource.SetResult(true);
                    });
                    popUp.SetExtraCount(reTriggerCount);
                    await taskCompletionSource.Task;
                }
                else
                {
                    UpdateFreeSpinUIState(true, false);
                }
                
                
            }

           
        }
        
        protected virtual FreeSpinReTriggerPopUp ShowFreeSpinReTriggeredPopup(string assetName)
        {
            return PopUpManager.Instance.ShowPopUp<FreeSpinReTriggerPopUp>(assetName);
        }
        
        protected virtual void UpdateFreeSpinUIState(bool isFreeSpin, bool isAverage = false)
        {
            var freeSpinState = machineContext.state.Get<FreeSpinState>();
            controlPanel.UpdateControlPanelState(isFreeSpin,isAverage);
            controlPanel.UpdateFreeSpinCountText(freeSpinState.CurrentCount, freeSpinState.TotalCount);
        }
        
        
        

        protected override void HandleCommonLogic()
        {
            if (!IsReSpinSpinFinished())
            {
                StopWinCycle();
            }
        }
        
        
        protected virtual async Task ShowFreeSpinTriggerLineAnimation()
        {
            var wheels = machineContext.state.Get<WheelsActiveState>().GetRunningWheel();
            if(wheels.Count > 0)
                await wheels[0].winLineAnimationController.BlinkFreeSpinTriggerLine();
        }
        


        protected virtual async Task HandleReSpinBeginCutSceneAnimation()
        {

            if (this.IsFromMachineSetup())
            {
                RestoreTriggerWheelElement();
            }
            var freeSpinState = machineContext.state.Get<FreeSpinState>();

            if (freeSpinState.NewCount > 0)
            {
                await ShowFreeSpinTriggerLineAnimation();
            }




            machineContext.view.Get<TransitionView11015>().CloseIcon();
            
            

            machineContext.view.Get<LockView11015>().RefreshLock();
            if (!freeSpinState.IsInFreeSpin )
            {
                
                await machineContext.view.Get<RollMasks11015>().PlayOpenAnim();
            }
            else
            {
                //await machineContext.view.Get<LockView11015>().RefreshLock();
                await machineContext.view.Get<RollMasks11015>().PlayOpenFreeAnim();
            }

            RefreshWheel();
        }

        protected void RefreshWheel()
        {
            var wheelsActiveState = machineContext.state.Get<WheelsActiveState11015>();
            var wheel = wheelsActiveState.GetRunningWheel()[0];
            wheel.wheelState.UpdateCurrentActiveSequence(wheelsActiveState.GetReelNameForWheel(wheel));
            //wheel.ForceUpdateElementOnWheel();
            
        }

        protected virtual async Task HandleReSpinFinishCutSceneAnimation()
        {

            RefreshWheel();
            
        }
        
        protected virtual async Task HandleReSpinGame()
        {
            
        }
        
        protected virtual async Task HandleReSpinReward()
        {
            
        }


        protected virtual async Task SettleReSpin()
        {
           await reSpinState.SettleReSpin();
        }

        protected virtual bool IsInRespinProcess()
        {
            return IsReSpinTriggered() || reSpinState.IsInRespin || reSpinState.ReSpinNeedSettle;
        }
        
        
        //ReSpin Spin是否已经结束
        protected virtual bool IsReSpinSpinFinished()
        {
            return !NextIsReSpinSpin() && reSpinState.ReSpinNeedSettle;
        }

        //ReSpin结算过程结束，有可能结算和ReSpinSpin结束条件不一致
        public virtual bool NeedSettle()
        {
            return reSpinState.ReSpinNeedSettle;
        }

        //当次是否触发了ReSpin
        protected virtual bool IsReSpinTriggered()
        {
            return reSpinState.ReSpinTriggered;
        }

        protected virtual bool NextIsReSpinSpin()
        {
            return reSpinState.NextIsReSpin;
        }

        protected virtual void StopBackgroundMusic()
        {
            AudioUtil.Instance.StopMusic();
        }
        
    }
}