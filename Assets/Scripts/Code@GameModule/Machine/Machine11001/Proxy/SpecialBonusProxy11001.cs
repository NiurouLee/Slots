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
    public class SpecialBonusProxy11001 : SpecialBonusProxy
    {
        private ExtraState11001 _extraState11001;
        private BingoMapView11001 _bingoMapView11001;
        public SpecialBonusProxy11001(MachineContext context) 
            : base(context)
        {
        }

        public override void SetUp()
        {
            _extraState11001 = machineContext.state.Get<ExtraState11001>();
            _bingoMapView11001 = machineContext.view.Get<BingoMapView11001>();
            base.SetUp();
        }

        protected override void RegisterInterestedWaitEvent()
        {
            base.RegisterInterestedWaitEvent();
            
            waitEvents.Add(WaitEvent.WAIT_SPECIAL_EFFECT);
        }
        
        protected override async void HandleCustomLogic()
        {
            var bingoData = _extraState11001.GetBingoData();

            if (bingoData.Lines.Count > 0)
            {
               await _bingoMapView11001.StartBingoAnimation();
               await machineContext.state.Get<ExtraState11001>().SettleBonusProgress();
               var winState = machineContext.state.Get<WinState>();
               
               if (winState.winLevel < (int)WinLevel.BigWin)
               {
                   UpdateWinChipsToDisplayTotalWin();
               }
               Proceed();
            }
            else
            {
                Proceed();
            }
        }
    }
}