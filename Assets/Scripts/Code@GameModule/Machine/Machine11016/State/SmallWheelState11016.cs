//  **********************************************
//  Copyright(c) 2021 by com.ustar
//  All right reserved
// 
//  Author : Besure.Chen
//  Date :2021-11-06 16:50
//  Ver : 1.0.0
//  Description : SmallWheelState11016.cs
//  ChangeLog :
//  **********************************************

using System.Collections.Generic;
using DragonU3DSDK.Network.API.ILProtocol;

namespace GameModule
{
    public class SmallWheelState11016:WheelState
    {
        public ulong TotalSmallWin;
        public int GameId;
        public List<List<int>> ListStartSpin;
        public List<List<int>> ListResultSpin;
        public SmallWheelState11016(MachineState state, int index, int gameId) : base(state)
        {
            GameId = gameId;
            PayLineOffsetZOrder = 1080 + (gameId-1+index)* 100 - 1;
        }

        public override void UpdateWheelStateInfo(Panel panel)
        {
            ListResultSpin = new List<List<int>>();
            base.UpdateWheelStateInfo(panel);

            TotalSmallWin = machineState.Get<BetState>().GetPayWinChips(panel.WinRate);
            for (int i = 0; i < 3; i++)
            {
                ListResultSpin.Add(new List<int>());
                for (int j = 2; j >= 0; j--)
                {
                    ListResultSpin[i].Add((int)panel.Columns[i].Symbols[j+1]);   
                }
            }
        }

        public override void UpdateCurrentActiveSequence(string sequenceName, List<int> startIndex = null)
        {
            base.UpdateCurrentActiveSequence(sequenceName, startIndex);
            ListStartSpin = new List<List<int>>();
            ListStartSpin.Add(new List<int>());
            ListStartSpin[0].Add(23);   
            ListStartSpin[0].Add(17+GameId);   
            ListStartSpin[0].Add(22);   
            ListStartSpin.Add(new List<int>());
            ListStartSpin[1].Add(22);   
            ListStartSpin[1].Add(17+GameId);   
            ListStartSpin[1].Add(21);  
            ListStartSpin.Add(new List<int>());
            ListStartSpin[2].Add(25);   
            ListStartSpin[2].Add(17+GameId);   
            ListStartSpin[2].Add(25);  
        }
    }
}