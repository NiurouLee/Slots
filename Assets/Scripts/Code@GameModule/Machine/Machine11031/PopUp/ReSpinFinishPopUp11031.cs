// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date :2021-06-21 11:50 AM
// Ver : 1.0.0
// Description : ReSpinStartPopUp.cs
// ChangeLog :
// **********************************************

using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameModule
{
    public class ReSpinFinishPopUp11031 : ReSpinFinishPopUp
    {
        public ReSpinFinishPopUp11031(Transform transform) : base(transform)
        {
        }


        public override void Initialize(MachineContext inContext)
        {
            base.Initialize(inContext);
            // var index = context.state.Get<ExtraState11031>().GetLinkPepperCount();
            // ulong winRate;
            // if (index >= 40)
            // {
            var winRate = context.state.Get<ExtraState11031>().GetLinkPepperJackpotPay() +
                          context.state.Get<ExtraState11031>().GetLinkPepperWinRate();
            // }
            // else
            // {
            //     winRate = context.state.Get<ExtraState11031>().GetLinkPepperWinRate();
            // }

            var chips = context.state.Get<BetState>().GetPayWinChips(winRate);
            var finalChips = context.state.Get<ReSpinState>().GetRespinTotalWin() + chips;
            if (txtReSpinWinChips)
                UpdateTotalWin(finalChips);
        }
    }
}