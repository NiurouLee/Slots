using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameModule
{
    public class SubRoundFinishProxy11005: SubRoundFinishProxy
    {
        public SubRoundFinishProxy11005(MachineContext context) : base(context)
        {
        }

        protected override void HandleCustomLogic()
        {
            // if (!machineContext.state.Get<FreeSpinState>().IsTriggerFreeSpin)
            // {
            //     machineContext.view.Get<BaseLetterView11005>().SetBtnGetMoreEnable(true);
            // }


            RecoverFreeSpinState();
            base.HandleCustomLogic();
        }
        
        
        
        
        protected virtual void RecoverFreeSpinState()
        {
            try
            {

                // var freeSpinState = machineContext.state.Get<FreeSpinState>();
                // if (freeSpinState.IsInFreeSpin)
                // {
                //     ExtraState11005 extraState = machineContext.state.Get<ExtraState11005>();
                //     uint luckCount = extraState.GetLuckCount();
                //     machineContext.state.Get<WheelsActiveState11005>()
                //         .UpdateRunningWheel(Constant11005.GetListFree(luckCount));
                // }


                
                var betState = machineContext.state.Get<BetState11005>();
                betState.InitGameUnlockConfig();
                var baseLetterView = machineContext.view.Get<BaseLetterView11005>();
                baseLetterView.ChangeBetLetter(false);

            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
            
        }
    }
}