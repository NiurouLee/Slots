using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading.Tasks;
using DG.Tweening;
using GameModule;

namespace GameModule
{
    public class WheelStopSpecialEffectProxy11022 : WheelStopSpecialEffectProxy
    {
        public WheelStopSpecialEffectProxy11022(MachineContext machineContext)
            : base(machineContext)
        {
        }

        protected override async void HandleCustomLogic()
        {
            var runningWheel = machineContext.state.Get<WheelsActiveState11022>().GetRunningWheel()[0];
            var runningWheelName = runningWheel.wheelName;
            if (runningWheelName == "WheelFreeGame")
            {
                var wheel = machineContext.view.Get<Wheel>(1);
                var mapPositions = machineContext.state.Get<ExtraState11022>().GetDragReelPositionMap();
                var listKeys = new List<uint>(mapPositions.Keys);
                int moveRollCount = 0;
                int completeRollCount = 0;
                float maxDelayTime = 0;
                for (int i = 0; i < listKeys.Count; i++)
                {
                    uint rollIndex = listKeys[i];
                    int shiftStep = mapPositions[rollIndex];
                    if (shiftStep != 0)
                    {
                        moveRollCount++;
                        // var task = new TaskCompletionSource<bool>();
                        var tempRoll = wheel.GetRoll((int) rollIndex);
                        var elementContainer = tempRoll.GetContainer(tempRoll.rowCount + shiftStep);
                        elementContainer.UpdateExtraSortingOrder(100);
                        var element = elementContainer.GetElement();
                        float duration = Math.Abs(shiftStep) * 0.2f;
                        if (duration > maxDelayTime)
                            maxDelayTime = duration;
                        element.transform.DOLocalMoveY(shiftStep * tempRoll.stepSize, duration).OnComplete(
                            () =>
                            {
                                try
                                {
                                    completeRollCount++;
                                    element.transform.localPosition = Vector3.zero;
                                    elementContainer.UpdateExtraSortingOrder(0);
                                    int foward = shiftStep / Math.Abs(shiftStep);
                                    int baseIndex = foward > 0 ? tempRoll.rowCount + 1 : 0;
                                    var tempElementConfigSet = machineContext.machineConfig.GetElementConfigSet();
                                    for (int j = 0; j != shiftStep; j += foward)
                                    {
                                        var tempSequenceElement = new SequenceElement(
                                            tempElementConfigSet.GetElementConfig(
                                                Constant11022.GetRandomNormalElementId()), machineContext);
                                        tempRoll.GetContainer(baseIndex + j).UpdateElement(tempSequenceElement);
                                        tempRoll.GetContainer(baseIndex + j).ShiftSortOrder(false);
                                    }
                                    for (int j = 0; j < Constant11022.LongWildList.Count; j++)
                                    {
                                        var tempSequenceElement = new SequenceElement(
                                            tempElementConfigSet.GetElementConfig(Constant11022.LongWildList[j]),
                                            machineContext);
                                        tempRoll.GetContainer(j + 1).UpdateElement(tempSequenceElement, false);
                                        tempRoll.GetContainer(j + 1).ShiftSortOrder(false);
                                    }
                                    // task.SetResult(true);
                                }
                                catch (System.Exception e)
                                {
                                    XDebug.LogError("ERROR=" + e);
                                }

                                if (moveRollCount == completeRollCount)
                                {
                                    base.HandleCustomLogic();
                                }
                            });
                        // listTask.Add(task.Task);
                    }
                }

                if (moveRollCount == 0)
                {
                    base.HandleCustomLogic();
                }
                else
                {
                    AudioUtil.Instance.PlayAudioFx("W02_Nudge");
                    // await XUtility.WaitSeconds(maxDelayTime, machineContext);
                    // base.HandleCustomLogic();
                }
                // await Task.WhenAll(listTask);
                // base.HandleCustomLogic();	
            }
            else
            {
                XDebug.LogError("wrong runningWheelName " + runningWheelName);
            }
        }
    }
}