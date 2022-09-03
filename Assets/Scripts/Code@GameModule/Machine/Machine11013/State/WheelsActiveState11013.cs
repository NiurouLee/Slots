using System.Collections.Generic;
using UnityEngine;
using System;
using DragonU3DSDK.Network.API.ILProtocol;
using GameModule;

namespace GameModule
{
    public class WheelsActiveState11013 : WheelsActiveState
    {
        public WheelsActiveState11013(MachineState machineState)
            : base(machineState)
        {
        }
        
        public override void UpdateRunningWheelState(GameResult gameResult)
        {
            UpdateRunningWheelState(gameResult.IsFreeSpin, false);
        }


        public void UpdateRunningWheelState(bool isFree, bool updateReelSequence = true)
        {
            UpdateRunningWheel(new List<string>() {"WheelBaseGame"}, updateReelSequence);
            var baseCollect = machineState.machineContext.view.Get<BaseCollect11013>();
            var wheel = GetRunningWheel()[0].transform;
            if (isFree)
            {
                baseCollect.Close();
                wheel.transform.localPosition = new Vector3(0, 0.28f, 0);
            }
            else
            {
                baseCollect.Open();
                machineState.machineContext.view.Get<LockView11013>().ClearWild();
                wheel.transform.localPosition = Vector3.zero;
            }
        }
        
        public override string GetReelNameForWheel(Wheel wheel)
        {
            if (!machineState.machineContext.state.Get<FreeSpinState>().IsOver)
            {
                return "FreeReels";
            }


            return "Reels";
        }
    }
}