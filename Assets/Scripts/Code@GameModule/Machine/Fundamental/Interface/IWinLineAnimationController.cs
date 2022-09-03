// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/07/08/11:45
// Ver : 1.0.0
// Description : IWinLineAnimationController.cs
// ChangeLog :
// **********************************************

using System;
using System.Threading.Tasks;

namespace GameModule
{
    public interface IWinLineAnimationController
    {
        bool IsWinCyclePlaying { get; }
       
        void BindingWheel(Wheel inWheel);

        void BlinkAllWinLine(Action action);

        void BlinkWinLineOneByOne();

        Task BlinkBonusLine();

        Task BlinkJackpotLine();

        Task BlinkFreeSpinTriggerLine();

        Task BlinkReSpinLine();

        void PlayElementWinAnimation(uint rollIndex, uint rowIndex, bool showWinFrame = true);
       
        void StopElementWinAnimation(uint rollIndex, uint rowIndex);

        void StopAllElementAnimation(bool force = false);
    }
}