//  **********************************************
//  Copyright(c) 2021 by com.ustar
//  All right reserved
// 
//  Author : Besure.Chen
//  Date :2021-09-01 17:49
//  Ver : 1.0.0
//  Description : WheelStopSpecialEffectProxy11010.cs
//  ChangeLog :
//  **********************************************

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;

namespace GameModule
{
    public class WheelStopSpecialEffectProxy11010: WheelStopSpecialEffectProxy
    {
        private List<RollShiftHelper> _rollShiftHelper = new List<RollShiftHelper>();
        public WheelStopSpecialEffectProxy11010(MachineContext machineContext) : base(machineContext)
        {
            
        }

        public override void SetUp()
        {
            base.SetUp();
            for (int i = 0; i < 5; i++)
            {
                _rollShiftHelper.Add(new RollShiftHelper(machineContext));
            }
        }

        protected override async void HandleCustomLogic()
        {
            if (!machineContext.state.Get<WheelsActiveState11010>().isLinkWheel)
            {
                var mapPositions = machineContext.state.Get<ExtraState11010>().GetDragStackWildPos();
                if (mapPositions.Count>0)
                {
                    List<Task> listTask = new List<Task>();       
                    var wheel = machineContext.state.Get<WheelsActiveState>().GetRunningWheel()[0];
                    var listKeys = new List<uint> (mapPositions.Keys);
                    for (int i = 0; i < listKeys.Count; i++)
                    {
                        uint rollIndex = listKeys[i];
                        int shiftStep = mapPositions[rollIndex];
                        listTask.Add(_rollShiftHelper[(int)rollIndex].ShiftRoll(wheel.GetRoll((int)rollIndex), Math.Abs(shiftStep), shiftStep > 0, 0.5f));
                    }
                    AudioUtil.Instance.PlayAudioFx("W01_Nudge");
                    await Task.WhenAll(listTask);
                }   
            }
            base.HandleCustomLogic();
        }
    }
}