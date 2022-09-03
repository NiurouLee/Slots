
using System;
using System.Threading.Tasks;
using UnityEngine;

namespace GameModule
{
    public class WheelStopSpecialEffectProxy11020 : WheelStopSpecialEffectProxy
    {
        private LockedFramesView11020 lockedFramesView;

        public WheelStopSpecialEffectProxy11020(MachineContext context)
            : base(context)
        {
            
        }

        public override void SetUp()
        {
            base.SetUp();

            lockedFramesView = machineContext.view.Get<LockedFramesView11020>();
        }

        public override bool CheckCurrentStepHasLogicToHandle()
        {
            return true;
        }

        protected override void HandleCustomLogic()
        {
            machineContext.state.Get<SuperBonusInfoState11020>().UpdateSuperBonusUI(false);
            HandleLockedFrame(base.HandleCustomLogic);
        }

        private async void HandleLockedFrame(Action actionEnd)
        {
            await lockedFramesView.HandleSpinStopLogic();

            actionEnd?.Invoke();
        }
    }
}
