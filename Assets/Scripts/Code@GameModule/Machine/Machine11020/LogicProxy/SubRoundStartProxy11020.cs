using System.Collections.Generic;

namespace GameModule
{
    public class SubRoundStartProxy11020: SubRoundStartProxy
    {
        private LockedFramesView11020 lockedFramesView;

        public SubRoundStartProxy11020(MachineContext context) 
            : base(context)
        {
            
        }

        public override void SetUp()
        {
            base.SetUp();

            lockedFramesView = machineContext.view.Get<LockedFramesView11020>();
        }

        protected override void HandleCustomLogic()
        {
            Constant11020.firstInitWheel = false;
            Constant11020.hasSpined = true;
            
            machineContext.state.Get<WheelsActiveState11020>().ListNewIds.Clear();
            machineContext.state.Get<WheelsActiveState11020>().CurTotalBet =
                machineContext.state.Get<BetState>().totalBet;
            
            lockedFramesView.HandleSubRoundStart();

            base.HandleCustomLogic();

            var view = machineContext.state.Get<WheelsActiveState11020>();
            view.MakeFireBallsDisappear();
            view.ResetElementsExtraSortOder();
            
            machineContext.view.Get<WheelView11020>().RunWheel();
        }    
    }
}
