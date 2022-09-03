using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading.Tasks;
using GameModule;

namespace GameModule
{
    public class WheelStopSpecialEffectProxy11013 : WheelStopSpecialEffectProxy
    {
        public WheelStopSpecialEffectProxy11013(MachineContext machineContext)
            : base(machineContext)
        {
        }

        protected override void HandleCustomLogic()
        {
            CollectStarElements();
            base.HandleCustomLogic();
        }

        protected async void CollectStarElements()
        {
            machineContext.AddWaitEvent(WaitEvent.WAIT_STOP_SPECIAL_EFFECT);
            await machineContext.view.Get<BaseCollect11013>().CollectStarElements();
            machineContext.RemoveWaitEvent(WaitEvent.WAIT_STOP_SPECIAL_EFFECT);
        }
    }
}