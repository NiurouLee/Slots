//  **********************************************
//  Copyright(c) 2021 by com.ustar
//  All right reserved
// 
//  Author : Besure.Chen
//  Date :2022-01-17 16:23
//  Ver : 1.0.0
//  Description : WheelState11028.cs
//  ChangeLog :
//  **********************************************

using System.Collections.Generic;
using DragonU3DSDK.Network.API.ILProtocol;
using Google.ilruntime.Protobuf.Collections;

namespace GameModule
{
    public class WheelState11028:WheelState
    {
        protected List<uint> specialPayRuleId = new List<uint>() {1, 2, 3, 4, 19, 20};
        public List<WinLine> rapidHitWinLines;
        protected List<WinLine> specialWinLines;
        public bool HasSpecialWin;
        public WheelState11028(MachineState state) : base(state)
        {
        }
        
        protected  override void PreUpdateWinLines(RepeatedField<WinLine> winLines)
        {
            HasSpecialWin = false;
            specialWinLines = new List<WinLine>();
            rapidHitWinLines = new List<WinLine>();
            for (var i = 0; i < winLines.Count; i++)
            {
                if (specialPayRuleId.Contains(winLines[i].PayRuleId))
                {
                    HasSpecialWin = true;
                    specialWinLines.Add(winLines[i]);
                    normalWinLines.Add(winLines[i]);
                }
                if (winLines[i].Pay > 0 && winLines[i].PayLineId == 0 && winLines[i].PayRuleId > 0 && winLines[i].ShowSymbolAnimation)
                {
                    rapidHitWinLines.Add(winLines[i]);
                }
            }
        }

        public bool IsRapidHitJackpot(uint rollIndex, uint rowIndex)
        {
            for (int i = 0; i < rapidHitWinLines.Count; i++)
            {
                var winLine = rapidHitWinLines[i];
                for (int j = 0; j < winLine.Positions.Count; j++)
                {
                    var position = winLine.Positions[j];
                    if (position.X == rollIndex && position.Y == rowIndex)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public bool IsSpecialLine(WinLine winLine)
        {
            return  specialPayRuleId.Contains(winLine.PayRuleId);
        }
        
        public bool IsInSpecialWinLine(uint rollIndex, uint rowIndex)
        {
            for (int i = 0; i < specialWinLines.Count; i++)
            {
                var winLine = specialWinLines[i];
                for (int j = 0; j < winLine.Positions.Count; j++)
                {
                    var position = winLine.Positions[j];
                    if (position.X == rollIndex && position.Y == rowIndex)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}