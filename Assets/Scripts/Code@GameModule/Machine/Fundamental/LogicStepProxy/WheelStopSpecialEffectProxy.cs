// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/07/08/19:58
// Ver : 1.0.0
// Description : SpecialEffectProxy.cs
// ChangeLog :
// **********************************************

namespace GameModule
{
    public class WheelStopSpecialEffectProxy:LogicStepProxy
    {
        public WheelStopSpecialEffectProxy(MachineContext machineContext) : base(machineContext)
        {
            
        }

        public override bool CheckCurrentStepHasLogicToHandle()
        {
            var extraState = machineContext.state.Get<ExtraState>();
            if (extraState != null && extraState.HasSpecialEffectWhenWheelStop())
            {
                return true;
            }
            
            return false;
        }
    }
}