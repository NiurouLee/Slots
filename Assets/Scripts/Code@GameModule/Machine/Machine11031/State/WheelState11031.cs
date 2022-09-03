using System;
using System.Collections.Generic;
using System.Runtime.Remoting.Contexts;
using DragonU3DSDK.Network.API.ILProtocol;
using Google.ilruntime.Protobuf.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GameModule
{
    public class LinkWheelState11031: WheelState
    {
        public int staticResultIndex = 0;
        public LinkWheelState11031(MachineState state) : base(state)
        {
        
        }
        public void SetStaticResultIndex(int index)
        {
            staticResultIndex = index;
        }


        protected override void UpdateStartAndStopIndex(Panel panel)
        {
            base.UpdateStartAndStopIndex(panel);
            var superRespin = machineState.Get<ExtraState11031>().IsFourLinkMode();
            if (superRespin)
            {
                currentSequenceName = "BlastReSpinReels";
                UpdateReelSequences();
            }
        }

        public override void UpdateStateOnReceiveSpinResult(SSpin spinResult)
        {
            base.UpdateStateOnReceiveSpinResult(spinResult);
            

            //OneLinkToFourLink的时候需要将Panel的数据同步到WheelState里面
            if (!wheelIsActive && spinResult.GameResult.Panels.Count > 1)
            {
                var wheelSpinResult = spinResult.GameResult.Panels[staticResultIndex];
                
                UpdateWheelStateInfo(wheelSpinResult);
  
                if (staticResultIndex == 0)
                {
                    var configSet = machineState.machineConfig.GetElementConfigSet();
                    var config = configSet.GetElementConfig(Constant11031.SingleRedChill);
                    var sequenceElement = new SequenceElement(config, machineState.machineContext);
                    
                    if (spinResultElementDef != null)
                    {
                        for (var i = 0; i < spinResultElementDef.Count; i++)
                        {
                            var seq = spinResultElementDef[i];

                            for (var j = 0; j < seq.sequenceElements.Count; j++)
                            {
                                if (Constant11031.ListRedChilliWithJackpot.Contains(seq.sequenceElements[j].config.id))
                                {
                                    spinResultElementDef[i].sequenceElements[j] = sequenceElement;
                                }
                            }
                        }
                    }
                }
            }
        }

        protected override void UpdateAppearAnimationInfo(Panel panel)
        {
            base.UpdateAppearAnimationInfo(panel);

            var lastLinkData = machineState.Get<ExtraState11031>().GetLinkData();
            var linkData = lastLinkData[resultIndex];

            if (linkData != null)
            {
                var count = linkData.Items.Count;
                var lockPositionList = new List<uint>();
                //var listPositionId = linkData.Items[]
                for (var i = 0; i < count; i++)
                {
                    if (linkData.Items[i].SymbolId > 0)
                        lockPositionList.Add(linkData.Items[i].PositionId);
                }

                for (int i = 0; i < blinkRows.Count; i++)
                {
                    var blinkCount = blinkRows[i].Count;
                    if (blinkCount > 0)
                    {
                        var newBlinkRow = new RepeatedField<uint>();

                        for (var j = blinkCount - 1; j >= 0; j--)
                        {
                            var id = i * 3 + blinkRows[i][j];
                            if (!lockPositionList.Contains((uint) id))
                            {
                                newBlinkRow.Add(blinkRows[i][j]);
                            }
                        }

                        blinkRows[i] = newBlinkRow;
                    }
                }
            }
            // for (var w = 0; w < wheels.Count; w++)
            // {
            //     if (wheels[w].wheelName == wheelName)
            //     {
            //         if (machineState.Get<ExtraState11031>().LinkLockElementIdsList != null &&
            //             machineState.Get<ExtraState11031>().LinkLockElementIdsList.Count > 0)
            //         {
            //             if (machineState.Get<ExtraState11031>().LinkLockElementIdsList.ContainsKey(wheels[w]))
            //             {
            //                 noNeedBlinkList = machineState.Get<ExtraState11031>().LinkLockElementIdsList[wheels[w]];
            //             }
            //         }
            //     }
            // }

            // if (noNeedBlinkList.Count > 0)
            // {
            //     for (var i = 0; i < rollCount; i++)
            //     {
            //         for (var a = 0; a < panel.Columns[i].AppearingRows.Count; a++)
            //         {
            //             var id = i * 3 + panel.Columns[i].AppearingRows[a];
            //             if (noNeedBlinkList.Contains((int) id))
            //             {
            //                 panel.Columns[i].AppearingRows.Remove(panel.Columns[i].AppearingRows[a]);
            //             }
            //         }
            //
            //         blinkRows[i] = panel.Columns[i].AppearingRows;
            //     }
            // }
            // else
            // {
            //     for (var i = 0; i < rollCount; i++)
            //     {
            //         blinkRows[i] = panel.Columns[i].AppearingRows;
            //     }
            // }
        }
    }
}