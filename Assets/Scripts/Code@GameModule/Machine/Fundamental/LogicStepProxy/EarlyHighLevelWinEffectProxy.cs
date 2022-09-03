// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date :2021-07-05 12:15 PM
// Ver : 1.0.0
// Description : EarlyHighLevelWinEffectProxy.cs
// ChangeLog :
// **********************************************

namespace GameModule
{
    public class EarlyHighLevelWinEffectProxy : LogicStepProxy
    {
        protected WinState _winState;
        protected ExtraState _extraState;
        public EarlyHighLevelWinEffectProxy(MachineContext context)
            : base(context)
        {
            
        }

        public override void SetUp()
        {
            base.SetUp();
            _winState = machineContext.state.Get<WinState>();
        }

        public override bool CheckCurrentStepHasLogicToHandle()
        {
            _extraState = machineContext.state.Get<ExtraState>();
           
            if (_extraState == null || !_extraState.HasSpecialBonus())
            {
                if(_winState.winLevel >= (int)WinLevel.BigWin)
                    return true;
            }

            return _winState.winLevel >= (int)WinLevel.BigWin;
        }

        protected override void HandleCustomLogic()
        {
            machineContext.WaitSeconds(0.5f,
                () =>
                {
                    WinEffectHelper.ShowBigWinEffect(_winState.winLevel, _winState.displayCurrentWin, Proceed);
                    
                });
        }
    }
}