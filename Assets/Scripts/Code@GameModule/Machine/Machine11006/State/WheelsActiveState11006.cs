using System.Collections.Generic;
using DragonU3DSDK.Network.API.ILProtocol;

namespace GameModule
{
    public class WheelsActiveState11006 : WheelsActiveState
    {
        public WheelsActiveState11006(MachineState machineState) : base(machineState)
        {
            spinningOrder = WheelSpinningOrder.SAME_TIME_START_ONE_BY_ONE_STOP;
        }
        
        
        public override void UpdateRunningWheelState(GameResult gameResult)
        {
                UpdateRunningWheel(new List<string>() {"Wheel"},false);
            
        }

      
    }
}