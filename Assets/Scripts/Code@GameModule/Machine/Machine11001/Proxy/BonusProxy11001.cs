// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/08/09/11:12
// Ver : 1.0.0
// Description : SpecialBonusProxy11001.cs
// ChangeLog :
// **********************************************

namespace GameModule
{
    public class BonusProxy11001 : SpecialBonusProxy
    {
        private ExtraState11001 _extraState11001;
        private BingoMapView11001 _bingoMapView11001;

        public BonusProxy11001(MachineContext context)
            : base(context)
        {
        }

        public override void SetUp()
        {
            _extraState11001 = machineContext.state.Get<ExtraState11001>();
            _bingoMapView11001 = machineContext.view.Get<BingoMapView11001>();
        }

        public override bool CheckCurrentStepHasLogicToHandle()
        {
            return _bingoMapView11001.IsNeedRestBingoMap();
        }

        protected override async void HandleCustomLogic()
        {
            await _bingoMapView11001.ResetBingoMap();
            machineContext.state.Get<JackpotInfoState>().LockJackpot = false;
            machineContext.view.Get<JackPotPanel>().UnlockJackpotPanel();
            
            Proceed();
        }
    }
}