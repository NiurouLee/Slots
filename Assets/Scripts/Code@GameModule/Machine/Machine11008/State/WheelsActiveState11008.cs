using System.Collections;
using System.Collections.Generic;
using DragonU3DSDK.Network.API.ILProtocol;
using UnityEngine;
namespace GameModule{
    public class WheelsActiveState11008 : WheelsActiveState
    {
        public WheelsActiveState11008(MachineState machineState) : base(machineState)
        {
        }
        public override void UpdateRunningWheelState(GameResult gameResult)
        {
            var freeSpinState = machineState.Get<FreeSpinState>();
            if (freeSpinState.NextIsFreeSpin)
            {
                UpdateRunningWheel(new List<string>() { Constant11008.WheelName[1] }, true);
            }
            else
            {
                UpdateRunningWheel(new List<string>() { Constant11008.WheelName[0] }, true);
            }
        }
    }
}

