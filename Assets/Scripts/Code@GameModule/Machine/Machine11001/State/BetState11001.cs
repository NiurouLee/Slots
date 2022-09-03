// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/10/21/11:52
// Ver : 1.0.0
// Description : BetState11001.cs
// ChangeLog :
// **********************************************

using System.Collections.Generic;

namespace GameModule
{
    public class BetState11001 : BetState
    {
        public BetState11001(MachineState machineState)
            : base(machineState)
        {
            
        }

        protected override List<ulong> GetValidNewUnlockedBets(List<ulong> list)
        {
            var extraState11001 = machineState.Get<ExtraState11001>();

            for (var i = list.Count - 1; i >= 0; i--)
            {
                if (!extraState11001.HasBingoDataForBet(list[i]))
                {
                    list.RemoveAt(i);
                }
            }

            return list;
        }
    }
}