using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using GameModule.Utility;
using UnityEngine;

namespace GameModule
{
    public class WheelStopSpecialEffectProxy11009 : WheelStopSpecialEffectProxy
    {
        private List<RollShiftHelper> _rollShiftHelper = new List<RollShiftHelper>();

        public WheelStopSpecialEffectProxy11009(MachineContext machineContext) : base(machineContext)
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
            
            
            
            var wheelActiveState = machineContext.state.Get<WheelsActiveState11009>();
            var listWheel = wheelActiveState.GetRunningWheel();
            
            var listElement = listWheel[0].GetElementMatchFilter((element) =>
            {
            
                if (Constant11009.ListCollectElementId.Contains(element.sequenceElement.config.id) ||
                    Constant11009.ListElementIdPurpleVaraint.Contains(element.sequenceElement.config.id))
                {
                    
                    return true;
                }
            
                return false;
            });

            // if (listElement.Count > 0)
            // {
            //     //前面有飞金币，等飞完
            //     await machineContext.WaitSeconds(1f);
            // }



            //先进行W01 nudge
            bool hasWildNudge = await WildNudge(listElement.Count > 0);

            // if (hasWildNudge)
            // {
            //     await machineContext.WaitSeconds(1f);
            // }

            //再翻面之类
            await machineContext.view.Get<BoxesView11009>().RefreshElement(this);
            base.HandleCustomLogic();
        }


        protected virtual async Task<bool> WildNudge(bool hasFly)
        {
            if (machineContext.state.Get<ExtraState11009>().GetFreeGreenState() &&
                !machineContext.state.Get<FreeSpinState11009>().IsTriggerFreeSpin)
            {
                var mapPositions = machineContext.state.Get<ExtraState11009>().GetWildMapInfo();
                if (mapPositions.Count > 0)
                {
                    if (hasFly)
                    {
                        //前面有飞金币，等飞完
                        //await machineContext.WaitSeconds(1f);
                    }
                    
                    await machineContext.WaitSeconds(0.5f);
                    var wheel = machineContext.state.Get<WheelsActiveState>().GetRunningWheel()[0];
                    var listKeys = new List<uint>(mapPositions.Keys);

                    List<Task> listTask = new List<Task>();                    
                    for (int i = 0; i < listKeys.Count; i++)
                    {
                        int shiftStep = 0;
                        bool needShiftUp = false;

                         int rollIndex = (int) listKeys[i];
                        // var roll = wheel.GetRoll(rollIndex);
                        // var sequenceElement = roll.GetVisibleContainer(0).sequenceElement;
                        // if (sequenceElement.config.id == Constant11009.ElementIdWild)
                        // {
                        //     shiftStep = (int)(sequenceElement.config.height - sequenceElement.config.position - 1);
                        // }
                        // else
                        // {
                        //     needShiftUp = true;
                        //     sequenceElement = roll.GetVisibleContainer(2).sequenceElement;
                        //     shiftStep = (int)sequenceElement.config.position; 
                        // }

                        // int wildPos = mapPositions[(uint) rollIndex];
                        // int stopPos = wheel.wheelState.GetRollStopReelIndex(rollIndex);
                        // shiftStep = (int) wildPos - stopPos;
                        shiftStep = mapPositions[(uint)rollIndex];
                        
                        //int rollCount =  wheel.wheelState.GetWheelConfig().rollCount;
                        
                        if (shiftStep < 0)
                        {
                            needShiftUp = false;
                        }
                        else
                        {
                            needShiftUp = true;
                        }
                        
                        shiftStep = Math.Abs(shiftStep);
                        
                        //Debug.LogError($"=========shiftStep:{shiftStep} wildPos:{wildPos} stopPos:{stopPos}");
                        
                        if (shiftStep > 0)
                        {
                            listTask.Add(_rollShiftHelper[rollIndex].ShiftRoll(wheel.GetRoll(rollIndex), shiftStep, needShiftUp,
                                0.5f, Ease.OutBack));
                        
                        }

                        // await _rollShiftHelper[rollIndex].ShiftRoll(wheel.GetRoll(rollIndex), shiftStep, needShiftUp,
                        //     0.1f, Ease.Linear);
                    }

                    if (listTask.Count > 0)
                    {
                        AudioUtil.Instance.PlayAudioFx("WildNudge");
                        await Task.WhenAll(listTask);

                    }


                    return true;

                }

                
            }

            return false;
        }
    }
}