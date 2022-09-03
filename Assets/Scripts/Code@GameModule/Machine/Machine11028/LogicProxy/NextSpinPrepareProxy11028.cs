//  **********************************************
//  Copyright(c) 2021 by com.ustar
//  All right reserved
// 
//  Author : Besure.Chen
//  Date :2022-01-19 21:54
//  Ver : 1.0.0
//  Description : NextSpinPrepareProxy11028.cs
//  ChangeLog :
//  **********************************************

namespace GameModule
{
    public class NextSpinPrepareProxy11028:NextSpinPrepareProxy
    {
        public NextSpinPrepareProxy11028(MachineContext context) : base(context)
        {
        }
        
        
        protected override void OnBetChange()
        {
            base.OnBetChange();
            machineContext.view.Get<JackpotPotPanel11028>().UpdateJackpotLockState(true,false);

        }


        protected override void OnUnlockBetFeatureConfigChanged()
        {
            machineContext.view.Get<JackpotPotPanel11028>().UpdateJackpotLockState(false,false);

        }
    }
}