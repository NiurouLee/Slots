// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date :2021-07-05 12:15 PM
// Ver : 1.0.0
// Description : LateHighLevelWinEffectProxy.cs
// ChangeLog :
// **********************************************

namespace GameModule
{
    public class LateHighLevelWinEffectProxy : LogicStepProxy
    {
        protected WinState _winState;
        protected ExtraState _extraState;

        public LateHighLevelWinEffectProxy(MachineContext context)
            : base(context)
        {
        }

        public override void SetUp()
        {
            base.SetUp();
            _winState = machineContext.state.Get<WinState>();
        }

        protected override void RegisterInterestedWaitEvent()
        {
            waitEvents.Add(WaitEvent.WAIT_BLINK_ALL_WIN_LINE);
        }

        public override bool CheckCurrentStepHasLogicToHandle()
        {
            var extraState = machineContext.state.Get<ExtraState>();
            if (extraState != null && extraState.IsSpecialBonusFinish())
            {
                if (_winState.winLevel >= (int)WinLevel.BigWin)
                    return true;
            }

            return false;
        }

        protected override async void HandleCustomLogic()
        {
            await WinEffectHelper.ShowBigWinEffectAsync(_winState.winLevel, _winState.displayCurrentWin,
                machineContext);
            ForceUpdateWinChipsToDisplayTotalWin();
            Proceed();
        }
    }
}