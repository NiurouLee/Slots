//  **********************************************
//  Copyright(c) 2021 by com.ustar
//  All right reserved
// 
//  Author : Besure.Chen
//  Date :2022-01-14 14:58
//  Ver : 1.0.0
//  Description : SubRoundStartProxy11028.cs
//  ChangeLog :
//  **********************************************

namespace GameModule
{
    public class SubRoundStartProxy11028:SubRoundStartProxy
    {
        public SubRoundStartProxy11028(MachineContext context)
            : base(context)
        {
        }

        protected override void HandleCustomLogic()
        {
            StopWinCycle(true);
            machineContext.view.Get<JackpotPotPanel11028>().PlayAnimation();
            base.HandleCustomLogic();
        }
    }
}