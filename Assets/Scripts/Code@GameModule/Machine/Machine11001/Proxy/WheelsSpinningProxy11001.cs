// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/08/05/18:12
// Ver : 1.0.0
// Description : WheelsSpinningProxy11001.cs
// ChangeLog :
// **********************************************

using System;
using UnityEngine;

namespace GameModule
{
    public class WheelsSpinningProxy11001 : WheelsSpinningProxy
    {
        public WheelsSpinningProxy11001(MachineContext context)
            : base(context)
        {
            
        }
        
        public override void OnSpinResultReceived()
        {
            if (machineContext.state.Get<ExtraState11001>().HasNewBingoItemFromRandom())
            {
                ShowRandomRewardIncreaseEffect();
            }
            else
            {
                base.OnSpinResultReceived();
            }
        }

        protected async void ShowRandomRewardIncreaseEffect()
        {
            // try
            // {
                await machineContext.view.Get<BingoMapView11001>().ShowRewardIncreaseEffect();

                base.OnSpinResultReceived();
                
            // }
            // catch (Exception e)
            // {
            //     XDebug.Log(e.Message);
            // }
        }
    }
}