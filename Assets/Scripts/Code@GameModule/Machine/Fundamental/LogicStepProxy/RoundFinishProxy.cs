// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date :2021-07-05 12:15 PM
// Ver : 1.0.0
// Description : LogicProxyRoundFinish.cs
// ChangeLog :
// **********************************************

using DragonU3DSDK.Network.API.ILProtocol;

namespace GameModule
{
    public class RoundFinishProxy : LogicStepProxy
    {
        public RoundFinishProxy(MachineContext context)
            : base(context)
        {
            
        }
        
        protected override void HandleCommonLogic()
        {
            machineContext.state.UpdateStateBeforeCallRoundFinish();
            
            machineContext.state.UpdateStateOnRoundFinish();

            var autoSpinState = machineContext.state.Get<AutoSpinState>();
             
            
            if (autoSpinState.IsAutoSpin)
            {
                machineContext.view.Get<ControlPanel>()
                    .UpdateAutoSpinLeftCount(machineContext.state.Get<AutoSpinState>().AutoLeftCount);
            }
            
            //AudioUtil.Instance.PlayMusic(machineContext.machineConfig.audioConfig.GetBaseBackgroundMusicName());

            UpdateBalance();

            var winState = machineContext.state.Get<WinState>();
            var betState = machineContext.state.Get<BetState>();
            
#if UNITY_EDITOR || !PRODUCTION_PACKAGE
            if (UIDebugger.isSilent)
            {
                return;
            }
#endif
            if (winState.totalWin > 0)
            {
                EventBus.Dispatch(
                    new EventSpinRoundEnd((float) winState.totalWin / betState.totalBet, winState.totalWin,
                        winState.winLevel, machineContext.state.Get<AdStrategyState>().GetAdStrategy()),
                    () => { XDebug.Log("SystemLogicProcessFinished"); }
                );
            }
            else
            {
                EventBus.Dispatch(new EventSpinRoundEnd(0, 0, 0),
                    () => { XDebug.Log("SystemLogicProcessFinished"); }
                );
            }
        }

        protected void UpdateBalance()
        {
            var winState = machineContext.state.Get<WinState>();
            if(winState.totalWin > 0)
                EventBus.Dispatch(new EventBalanceUpdate((long) winState.totalWin, "SpinWin"));
        }
    }
}