//  **********************************************
//  Copyright(c) 2021 by com.ustar
//  All right reserved
// 
//  Author : Besure.Chen
//  Date :2021-10-20 17:09
//  Ver : 1.0.0
//  Description : ReSpinState11010.cs
//  ChangeLog :
//  **********************************************

namespace GameModule
{
    public class ReSpinState11011:ReSpinState
    {
        public ReSpinState11011(MachineState state) : base(state)
        {
            
        }

        public override ulong GetRespinTotalWin()
        {
            return machineState.Get<BetState>().GetPayWinChips(machineState.Get<ExtraState11011>().GetNextWinRate() + machineState.Get<ExtraState11011>().GetGrandJackpotWinRate());
        }
    }
}