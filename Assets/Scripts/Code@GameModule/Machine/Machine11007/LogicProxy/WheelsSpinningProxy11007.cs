//  **********************************************
//  Copyright(c) 2021 by com.ustar
//  All right reserved
// 
//  Author : Besure.Chen
//  Date :2021-09-08 18:08
//  Ver : 1.0.0
//  Description : WheelsSpinningProxy11007.cs
//  ChangeLog :
//  **********************************************

namespace GameModule
{
    public class WheelsSpinningProxy11007: WheelsSpinningProxy
    {
        public WheelsSpinningProxy11007(MachineContext context)
            : base(context)
        {
  
        }

        public override void OnSpinResultReceived()
        {
            base.OnSpinResultReceived();
            if (machineContext.state.Get<WheelsActiveState11007>().IsBonusWheel)
            {
                machineContext.view.Get<ControlPanel>().ShowStopButton(true);
            }
        }
    }
}