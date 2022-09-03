//  **********************************************
//  Copyright(c) 2021 by com.ustar
//  All right reserved
// 
//  Author : Besure.Chen
//  Date :2021-12-27 14:59
//  Ver : 1.0.0
//  Description : ReelStopSoundState.cs
//  ChangeLog :
//  **********************************************

using System.Collections.Generic;

namespace GameModule
{
    public class ReelStopSoundState:SubState
    {
        public List<SoundState> ListStopSoundState;
        public ReelStopSoundState(MachineState machineState)
            :base(machineState)
        {
            ListStopSoundState = new List<SoundState>();
        }

        public override void UpdateStateOnSubRoundStart()
        {
            base.UpdateStateOnSubRoundStart();
            for (int i = 0; i < ListStopSoundState.Count; i++)
            {
                ListStopSoundState[i].Reset();
            }

            //处理有的列锁住的情况
            var wheels = machineState.Get<WheelsActiveState>().GetRunningWheel();
            for (int i = 0; i < wheels.Count; i++)
            {
                for (int j = 0; j < wheels[i].rollCount; j++)
                {
                    if (wheels[i].wheelState.IsRollLocked(j))
                    {
                        ListStopSoundState[j].RollStopCount++;
                    }
                }
            }
        }

        public void ResetRollCount(int maxRollCount)
        {
            ListStopSoundState.Clear();
            for (int i = 0; i < maxRollCount; i++)
            {
                ListStopSoundState.Add(new SoundState());
            }
        }

        public class SoundState
        {
            public string SoundName;
            public int RollStopCount;
            public int blinkSoundOrderId=-1;

            public void Reset()
            {
                RollStopCount = 0;
                blinkSoundOrderId = -1;
                SoundName = string.Empty;
            }
        }
    }
}