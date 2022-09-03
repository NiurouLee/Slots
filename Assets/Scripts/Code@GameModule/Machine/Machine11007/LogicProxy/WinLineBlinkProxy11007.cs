//  **********************************************
//  Copyright(c) 2021 by com.ustar
//  All right reserved
// 
//  Author : Besure.Chen
//  Date :2021-08-27 12:31
//  Ver : 1.0.0
//  Description : WinLineBlinkProxy11007.cs
//  ChangeLog :
//  **********************************************

namespace GameModule
{
    public class WinLineBlinkProxy11007: WinLineBlinkProxy
    {
        public WinLineBlinkProxy11007(MachineContext context)
            : base(context)
        {
            
        }

        protected override void HandleCommonLogic()
        {
            base.HandleCommonLogic();
            
            var bonusState = machineContext.state.Get<BonusWheelState11007>();
            if (bonusState.IsBonusSpinSameWin())
            {
                machineContext.view.Get<LinkBonusTitle11007>().ShowBonusTitleAnimation();
            }
        }
    }
}