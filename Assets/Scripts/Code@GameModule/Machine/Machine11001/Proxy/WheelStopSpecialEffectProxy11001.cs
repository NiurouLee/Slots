// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/08/05/17:40
// Ver : 1.0.0
// Description : WheelStopSpecialEffectProxy11001.cs
// ChangeLog :
// **********************************************

namespace GameModule
{
    public class WheelStopSpecialEffectProxy11001:WheelStopSpecialEffectProxy
    {
        private ExtraState11001 _extraState11001;
        private FreeSpinState _freeSpinState;
        public WheelStopSpecialEffectProxy11001(MachineContext context)
            : base(context)
        {
            
        }

        public override void SetUp()
        {
            base.SetUp();
            _extraState11001 = machineContext.state.Get<ExtraState11001>();
            _freeSpinState = machineContext.state.Get<FreeSpinState>();
        }

        public override bool CheckCurrentStepHasLogicToHandle()
        {
            if (base.CheckCurrentStepHasLogicToHandle())
                return true;
            
            return machineContext.state.Get<FreeSpinState>().IsInFreeSpin;
        }

        protected override void HandleCustomLogic()
        {
            if(_extraState11001.HasSpecialEffectWhenWheelStop())
                machineContext.view.Get<BingoMapView11001>().ShowPanelNewBingoItemCollectFx();

            if (_freeSpinState.IsInFreeSpin)
            {
                machineContext.view.Get<Wheel>().GetRoll(12).UpdateElementToServerResult();
            }
            
            Proceed();
        }
    }
}