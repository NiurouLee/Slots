//  **********************************************
//  Copyright(c) 2021 by com.ustar
//  All right reserved
// 
//  Author : Besure.Chen
//  Date :2021-10-20 17:48
//  Ver : 1.0.0
//  Description : FreeSpinState11011.cs
//  ChangeLog :
//  **********************************************

namespace GameModule
{
    public class FreeSpinState11011:FreeSpinState
    {
        private ulong _totalWinRate;

        public ulong LastTotalWinRate
        {
            get
            {
                return _totalWinRate;
            }
            set
            {
                _totalWinRate = value;
            }
        }
        public FreeSpinState11011(MachineState state) : base(state)
        {
        }
    }
}