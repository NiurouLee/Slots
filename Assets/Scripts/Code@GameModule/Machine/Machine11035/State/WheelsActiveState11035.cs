using System.Collections.Generic;
using UnityEngine;
using System;
using GameModule;
using DragonU3DSDK.Network.API.ILProtocol;

namespace GameModule
{
    public class WheelsActiveState11035 : WheelsActiveState
    {

        public WheelsActiveState11035(MachineState machineState)
        : base(machineState)
        {
        }

        public override void UpdateRunningWheelState(GameResult gameResult)
        {
            var extraState = machineState.Get<ExtraState11035>();

            if (extraState.HasJackpotWheel())
            {
                UpdateRunningWheel(new List<string>() { "WheelJackpotGame" });

                machineState.machineContext.view.Get<ControlPanel>().ShowSpinButton(false);

                machineState.Get<JackpotInfoState>().LockJackpot = true;
            }
            else
            {
                UpdateRunningWheel(new List<string>() { "WheelBaseGame" });

                machineState.machineContext.view.Get<ControlPanel>().ShowSpinButton(true);

                machineState.Get<JackpotInfoState>().LockJackpot = false;
            }
        }

        public override string GetReelNameForWheel(Wheel wheel)
        {
            if (wheel.wheelName == "WheelBaseGame")
                return "Reels1";
            return "JackpotReels";
        }
    }
}