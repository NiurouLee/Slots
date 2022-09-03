// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/08/05/17:40
// Ver : 1.0.0
// Description : WheelStopSpecialEffectProxy11001.cs
// ChangeLog :
// **********************************************

namespace GameModule
{
    public class WheelStopSpecialEffectProxy11017 : WheelStopSpecialEffectProxy
    {
        private ExtraState11017 _extraState11017;
        private FreeSpinState _freeSpinState;

        public WheelStopSpecialEffectProxy11017(MachineContext context)
            : base(context)
        {
        }

        protected override void HandleCustomLogic()
        {
            machineContext.view.Get<SuperFreeGameIcon11017>().HideSuperFreeIcon();
            if (machineContext.state.Get<FreeSpinState>().IsInFreeSpin)
            {
                uint level = machineContext.state.Get<ExtraState11017>().GetLevel();
                if (level == 5)
                {
                    var wheel = machineContext.state.Get<WheelsActiveState11017>().GetRunningWheel()[0];
                    var elementContainer = wheel.GetRoll(2).GetVisibleContainer(2);
                    var elementConfigSet = machineContext.state.machineConfig.GetElementConfigSet();
                    var fixedConfig = elementConfigSet.GetElementConfig(Constant11017.PuePleElementId);
                    elementContainer.UpdateElement(new SequenceElement(fixedConfig, machineContext));
                    elementContainer.ShiftSortOrder(false);
                }
            }

            machineContext.view.Get<LinkRemaining11017>().ShowReSpinRemaining();
            base.HandleCustomLogic();
        }
    }
}