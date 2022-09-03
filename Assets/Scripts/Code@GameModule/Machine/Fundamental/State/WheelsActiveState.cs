// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date :2021-07-05 12:07 PM
// Ver : 1.0.0
// Description : WheelsActiveState.cs
// ChangeLog :
// **********************************************

using System;
using System.Collections.Generic;
using DragonU3DSDK.Network.API.ILProtocol;
using Unity.Collections;

namespace GameModule
{
    public enum WheelSpinningOrder
    {
        ONE_BY_ONE,   //一个一个的转
        SAME_TIME,    //同时一起转
        SAME_TIME_START_ONE_BY_ONE_STOP, //同时一起转，但是一个一个的停下
    }
    
    public class WheelsActiveState:SubState
    {
        public WheelSpinningOrder SpinningOrder => spinningOrder;

        protected WheelSpinningOrder spinningOrder;
        
        protected readonly List<Wheel> runningWheel;
        
        public WheelsActiveState(MachineState machineState)
            : base(machineState)
        {
            runningWheel = new List<Wheel>(5);
            spinningOrder = WheelSpinningOrder.ONE_BY_ONE;
        }


        public virtual void UpdateRunningWheelState(GameResult gameResult)
        {
            var wheels = machineState.machineContext.view.GetAll<Wheel>();
         
            if (wheels.Count == 1)
            {
                AddRunningWheel(wheels[0],-1, false);
            }
        }
        
        public void AddRunningWheel(Wheel wheel, int index = -1, bool updateReelSequence = true)
        {
            if(runningWheel.Contains(wheel))
                return;

            wheel.SetActive(true);
            wheel.wheelState.SetResultIndex(runningWheel.Count);

            if (updateReelSequence)
            {
                wheel.wheelState.UpdateCurrentActiveSequence(GetReelNameForWheel(wheel));
                wheel.ForceUpdateElementOnWheel();
            }

            if (index >= 0)
            {
                runningWheel.Insert(index, wheel);
                wheel.wheelState.SetResultIndex(index);
            }
            else
            {
                runningWheel.Add(wheel);
            }
        }
        
        public virtual void RemoveRunningWheel(Wheel wheel)
        {
            wheel.SetActive(false);
            runningWheel.Remove(wheel);
        }
        public List<Wheel> GetRunningWheel()
        {
            return runningWheel;
        }

        public void SetSpinningOder(WheelSpinningOrder inSpinningOrder)
        {
            spinningOrder = inSpinningOrder;
        }
        
        public virtual void UpdateRunningWheel(List<string> runningWheelsName, bool updateReelSequence = true)
        {
            var machineContext = machineState.machineContext;
            
            for (var i = runningWheel.Count - 1; i >= 0; i--)
            {
                RemoveRunningWheel(runningWheel[i]);
            }

            for (var i = 0; i < runningWheelsName.Count; i++)
            {
                var wheel = machineContext.view.Get<Wheel>(runningWheelsName[i]);
                if (wheel != null)
                {
                    AddRunningWheel(wheel, -1, updateReelSequence);
                }
            }
            if (machineContext.state.Get<ReelStopSoundState>() != null)
            {
                var maxRollCount = 0;
                for (int i = 0; i < runningWheel.Count; i++)
                {
                    var wheel = runningWheel[i];
                    if (wheel!=null)
                    {
                        maxRollCount = Math.Max(maxRollCount, wheel.rollCount);
                    }
                }
                machineContext.state.Get<ReelStopSoundState>().ResetRollCount(maxRollCount);   
            }
        }

        public virtual string GetReelNameForWheel(Wheel wheel)
        {
            if (machineState.Get<FreeSpinState>().NextIsFreeSpin)
            {
                return "FreeReels";
            }

            return "Reels";
        }
    }
}