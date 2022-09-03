//  **********************************************
//  Copyright(c) 2021 by com.ustar
//  All right reserved
// 
//  Author : Besure.Chen
//  Date :2022-01-17 16:44
//  Ver : 1.0.0
//  Description : WinLineAnimationController11028.cs
//  ChangeLog :
//  **********************************************

using System.Threading.Tasks;
using DragonU3DSDK.Network.API.ILProtocol;

namespace GameModule
{
    public class WinLineAnimationController11028:WinLineAnimationController
    {
        public override async Task BlinkWinLineAsync(WinLine winLine)
        {
            var activeState = wheel.GetContext().state.Get<WheelsActiveState>();
            var jackpotWinLines = wheel.GetContext().state.Get<WheelState11028>().rapidHitWinLines;
            for (int i = 0; i < jackpotWinLines.Count; i++)
            {
                var jackpotWinLine = jackpotWinLines[i];
                for (int j = 0; j < jackpotWinLine.Positions.Count; j++)
                {
                    var pos = jackpotWinLine.Positions[j];
                    var container =  activeState.GetRunningWheel()[0].GetWinLineElementContainer((int) pos.X,(int) pos.Y);
                    container.PlayElementAnimation("Win");
                }
            }
            await base.BlinkWinLineAsync(winLine);
        }
        

        public override void StopElementWinAnimation(uint rollIndex, uint rowIndex)
        {
            if (wheel.GetContext().state.Get<WheelState11028>().IsRapidHitJackpot(rollIndex, rowIndex) || 
                wheel.GetContext().state.Get<WheelState11028>().IsInSpecialWinLine(rollIndex,rowIndex))
            {
                return;
            }
            base.StopElementWinAnimation(rollIndex, rowIndex);
        }

        protected override bool NeedPlayWinAnimation(WinLine winLine)
        {
            return wheel.GetContext().state.Get<WheelState11028>().IsSpecialLine(winLine);
        }

        protected override bool NeedShowWinFrame(WinLine winLine)
        {
            return !wheel.GetContext().state.Get<WheelState11028>().IsSpecialLine(winLine);
        }
    }
}