//  **********************************************
//  Copyright(c) 2021 by com.ustar
//  All right reserved
// 
//  Author : Besure.Chen
//  Date :2021-08-17 14:36
//  Ver : 1.0.0
//  Description : LinkWheelState11007.cs
//  ChangeLog :
//  **********************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using DragonU3DSDK.Network.API.ILProtocol;
using DragonU3DSDK.Network.API.Protocol;
using Google.ilruntime.Protobuf.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GameModule
{
    public class WheelStateLink11026: WheelState
    {
        public WheelStateLink11026(MachineState state) : base(state)
        {
            
        }

         //根据当前玩法状态获取当前轮盘的卷轴
         public override List<SequenceElement> GetActiveSequenceElement(Roll roll)
         {
             var extraState = machineState.machineContext.state.Get<ExtraState11026>();
             uint rowNumber = extraState.GetRowsMore();
             // int realIndex = roll.rollIndex;
             int realIndex = -1;
             if (extraState != null)
             {
                 if (wheelName == "WheelLinkGame1")
                 { 
                     realIndex = (int)extraState.GetLeftLinkData().ReelMapping[(uint)roll.rollIndex];
                 }else if (wheelName == "WheelLinkGame6")
                 {
                      realIndex = (int)extraState.GetRightLinkData().ReelMapping[(uint)roll.rollIndex];
                 }else if (wheelName == "WheelLinkGame2" || wheelName == "WheelLinkGame3" ||
                           wheelName == "WheelLinkGame4" || wheelName == "WheelLinkGame5")
                 {
                     var items = extraState.GetCenterLinkData().Items; 
                     for (int q =  items.count - 1; q >=0; q--) 
                     { 
                         var item = items[q]; 
                         if ((item.PositionId % 7) < (3 - rowNumber)) 
                         { 
                             items.Remove(item); 
                         } 
                     }
                     var  reelMappingTemp = new MapField<uint, uint>();
                     foreach (var child in extraState.GetCenterLinkData().ReelMapping)
                    {
                        uint lastKey = child.Key;
                        uint childValue = child.Value;
                        uint newKey = 0;
                        
                        if (items.count == 12) 
                        {
                             if (lastKey <= 6)
                             {
                                 newKey = lastKey - 3;
                             }else if (lastKey > 6 && lastKey <= 13)
                             {
                                 newKey = lastKey - 6;
                             }
                             else
                             {
                                 newKey = lastKey - 9;
                             }
                             
                        }else if (items.count == 15) {
                             if (lastKey <= 6)
                             {
                                 newKey = lastKey - 2;
                             }else if (lastKey > 6 && lastKey <= 13)
                             {
                                 newKey = lastKey - 4;
                             }
                             else
                             {
                                 newKey = lastKey - 6;
                             }
                        }else if (items.count == 18){
                             if (lastKey <= 6)
                             {
                                 newKey = lastKey - 1;
                             }else if (lastKey > 6 && lastKey <= 13)
                             {
                                 newKey = lastKey - 2;
                             }
                             else
                             {
                                 newKey = lastKey - 3;
                             }
                        }
                        else if (items.count == 21)
                        {
                            newKey = lastKey;
                        }
                        reelMappingTemp.Add(newKey, childValue); 
                    }
                     
                     
                    extraState.SetReelMappingTemp(reelMappingTemp);
                    if (reelMappingTemp.ContainsKey((uint) roll.rollIndex))
                    {
                        realIndex = (int) reelMappingTemp[(uint) roll.rollIndex];
                    } 
                 }
             }
             if(realIndex >=0){
                Debug.Log("realIndex:"+realIndex+"currentActiveSequence:"+currentActiveSequence.Count);
                ReelSequence reelSequence = currentActiveSequence[realIndex]; 
                return reelSequence.sequenceElements;
            }else
            {
                return currentActiveSequence[0].sequenceElements;
            }
         }
         
         public override List<SequenceElement> GetActiveSequenceElement(int rollIndex)
         {
             var extraState = machineState.machineContext.state.Get<ExtraState11026>();
             uint rowNumber = extraState.GetRowsMore();
             int realIndex = -1;
             // int realIndex = rollIndex;
             if (extraState != null)
             {
                 if (wheelName == "WheelLinkGam1")
                 { 
                     realIndex = (int)extraState.GetLeftLinkData().ReelMapping[(uint)rollIndex];
                 }else if (wheelName == "WheelLinkGam6")
                 {
                      realIndex = (int)extraState.GetRightLinkData().ReelMapping[(uint)rollIndex];
                 }else if (wheelName == "WheelLinkGam2" || wheelName == "WheelLinkGam3" ||
                           wheelName == "WheelLinkGam4" || wheelName == "WheelLinkGam5")
                 {

                     var col = rollIndex / this.wheelConfig.rollRowCount;
                     var row = rollIndex % this.wheelConfig.rollRowCount;

                     var transformIndex =(uint) (col * 7 + row + (7 - wheelConfig.rollRowCount));

                     realIndex =  (int)extraState.GetCenterLinkData().ReelMapping[transformIndex];
                     
                     var items = extraState.GetCenterLinkData().Items; 
                     for (int q =  items.count - 1; q >=0; q--) 
                     { 
                         var item = items[q]; 
                         if ((item.PositionId % 7) < (3 - rowNumber)) 
                         { 
                             items.Remove(item); 
                         } 
                     }
                     var  reelMappingTemp = new MapField<uint, uint>();
                     foreach (var child in extraState.GetCenterLinkData().ReelMapping)
                    {
                        uint lastKey = child.Key;
                        uint childValue = child.Value;
                        uint newKey = 0;
                        
                        if (items.count == 12) 
                        {
                             if (lastKey <= 6)
                             {
                                 newKey = lastKey - 3;
                             }else if (lastKey > 6 && lastKey <= 13)
                             {
                                 newKey = lastKey - 6;
                             }
                             else
                             {
                                 newKey = lastKey - 9;
                             }
                             
                        }else if (items.count == 15) {
                             if (lastKey <= 6)
                             {
                                 newKey = lastKey - 2;
                             }else if (lastKey > 6 && lastKey <= 13)
                             {
                                 newKey = lastKey - 4;
                             }
                             else
                             {
                                 newKey = lastKey - 6;
                             }
                        }else if (items.count == 18){
                             if (lastKey <= 6)
                             {
                                 newKey = lastKey - 1;
                             }else if (lastKey > 6 && lastKey <= 13)
                             {
                                 newKey = lastKey - 2;
                             }
                             else
                             {
                                 newKey = lastKey - 3;
                             }
                        }

                        reelMappingTemp.Add(newKey, childValue);
                    }
                     
                    extraState.SetReelMappingTemp(reelMappingTemp);
                    if (reelMappingTemp.ContainsKey((uint)rollIndex))
                    { 
                        realIndex =(int)reelMappingTemp[(uint)rollIndex];
                    }
                 }
             }
             if(realIndex >=0){
                ReelSequence reelSequence = currentActiveSequence[realIndex]; 
                return reelSequence.sequenceElements;
            }else{
                return currentActiveSequence[0].sequenceElements;
            }
             // ReelSequence reelSequence =  currentActiveSequence[realIndex]; 
             // return reelSequence.sequenceElements;
         }
         
          
         /// <summary>
         /// 更新卷轴
         /// </summary>
         /// <param name="sequenceName">新的卷轴</param>
         /// <param name="startIndex">新的初始化</param>
         public override void UpdateCurrentActiveSequence(string sequenceName, List<int> startIndex = null)
         {
             currentSequenceName = sequenceName;
             if (sequenceElementConstructor == null)
                 sequenceElementConstructor = machineState.machineContext.sequenceElementConstructor;

             currentActiveSequence = sequenceElementConstructor.GetReelSequences(currentSequenceName);

             if (startIndex != null && startIndex.Count == rollCount)
             {
                 rollStartReelIndex = startIndex;
             }
             else
             {
                 var extraState = machineState.machineContext.state.Get<ExtraState11026>();
                 uint rowNumber = extraState.GetRowsMore();
                 for (var i = 0; i < rollCount; i++)
                 {
                
                     // int realIndex = i;
                     int realIndex = -1;
                     if (extraState != null)
                     {
                         if (wheelName == "WheelLinkGam1")
                         {
                             realIndex = (int) extraState.GetLeftLinkData().ReelMapping[(uint) i];
                         }
                         else if (wheelName == "WheelLinkGam6")
                         {
                             realIndex = (int) extraState.GetRightLinkData().ReelMapping[(uint) i];
                         }
                         else if (wheelName == "WheelLinkGam2" || wheelName == "WheelLinkGam3" ||
                                  wheelName == "WheelLinkGam4" || wheelName == "WheelLinkGam5")
                         {
                                     var items = extraState.GetCenterLinkData().Items; 
                     for (int q =  items.count - 1; q >=0; q--) 
                     { 
                         var item = items[q]; 
                         if ((item.PositionId % 7) < (3 - rowNumber)) 
                         { 
                             items.Remove(item); 
                         } 
                     }
                     var  reelMappingTemp = new MapField<uint, uint>();
                     foreach (var child in extraState.GetCenterLinkData().ReelMapping)
                    {
                        uint lastKey = child.Key;
                        uint childValue = child.Value;
                        uint newKey = 0;
                        
                        if (items.count == 12) 
                        {
                             if (lastKey <= 6)
                             {
                                 newKey = lastKey - 3;
                             }else if (lastKey > 6 && lastKey <= 13)
                             {
                                 newKey = lastKey - 6;
                             }
                             else
                             {
                                 newKey = lastKey - 9;
                             }
                             
                        }else if (items.count == 15) {
                             if (lastKey <= 6)
                             {
                                 newKey = lastKey - 2;
                             }else if (lastKey > 6 && lastKey <= 13)
                             {
                                 newKey = lastKey - 4;
                             }
                             else
                             {
                                 newKey = lastKey - 6;
                             }
                        }else if (items.count == 18){
                             if (lastKey <= 6)
                             {
                                 newKey = lastKey - 1;
                             }else if (lastKey > 6 && lastKey <= 13)
                             {
                                 newKey = lastKey - 2;
                             }
                             else
                             {
                                 newKey = lastKey - 3;
                             }
                        }

                        reelMappingTemp.Add(newKey, childValue);
                    }
                     
                    extraState.SetReelMappingTemp(reelMappingTemp);
                    if (reelMappingTemp.ContainsKey((uint)i))
                    {
                         realIndex =(int)reelMappingTemp[(uint) i];
                    } 
                         }
                     }
                     if(realIndex >=0){ 
                          ReelSequence reelSequence = currentActiveSequence[realIndex]; 
                          rollStartReelIndex[i] = Random.Range(0, reelSequence.sequenceElements.Count); 
                     }
                     else
                     {
                         rollStartReelIndex[i] = Random.Range(0,currentActiveSequence[0].sequenceElements.Count); 
                     }

                     // ReelSequence reelSequence = currentActiveSequence[realIndex];
                     //     rollStartReelIndex[i] = Random.Range(0, reelSequence.sequenceElements.Count); 
                 }
             }
         }
         
         
         
         
         protected override void UpdateStartAndStopIndex(Panel panel)
         {
             currentSequenceName = panel.ReelsId;
             currentActiveSequence = sequenceElementConstructor.GetReelSequences(currentSequenceName);

             var extraState = machineState.machineContext.state.Get<ExtraState11026>();
             
             for (var i = 0; i < rollCount; i++)
             {
                 // int realIndex = i;
                 int realIndex = -1;
                 if (extraState != null)
                 {
                     if (wheelName == "WheelLinkGam1") 
                     { 
                          realIndex = (int)extraState.GetLeftLinkData().ReelMapping[(uint)i]; 
                     }else if (wheelName == "WheelLinkGam6") 
                     { 
                          realIndex = (int)extraState.GetRightLinkData().ReelMapping[(uint)i];
                     }else if (wheelName == "WheelLinkGam2" || wheelName == "WheelLinkGam3" || wheelName == "WheelLinkGam4" || wheelName == "WheelLinkGam5")
                     {
                         
                         
                         uint rowNumber = extraState.GetRowsMore();
                         var items = extraState.GetCenterLinkData().Items; 
                         for (int q =  items.count - 1; q >=0; q--) 
                         { 
                             var item = items[q]; 
                             if ((item.PositionId % 7) < (3 - rowNumber)) 
                             { 
                                 items.Remove(item); 
                             } 
                         } 
                         var  reelMappingTemp = new MapField<uint, uint>(); 
                         foreach (var child in extraState.GetCenterLinkData().ReelMapping) 
                         {
                        uint lastKey = child.Key;
                        uint childValue = child.Value;
                        uint newKey = 0;
                        
                        if (items.count == 12) 
                        {
                             if (lastKey <= 6)
                             {
                                 newKey = lastKey - 3;
                             }else if (lastKey > 6 && lastKey <= 13)
                             {
                                 newKey = lastKey - 6;
                             }
                             else
                             {
                                 newKey = lastKey - 9;
                             }
                             
                        }else if (items.count == 15) {
                             if (lastKey <= 6)
                             {
                                 newKey = lastKey - 2;
                             }else if (lastKey > 6 && lastKey <= 13)
                             {
                                 newKey = lastKey - 4;
                             }
                             else
                             {
                                 newKey = lastKey - 6;
                             }
                        }else if (items.count == 18){
                             if (lastKey <= 6)
                             {
                                 newKey = lastKey - 1;
                             }else if (lastKey > 6 && lastKey <= 13)
                             {
                                 newKey = lastKey - 2;
                             }
                             else
                             {
                                 newKey = lastKey - 3;
                             }
                        }

                        reelMappingTemp.Add(newKey, childValue); 
                         } 
                         extraState.SetReelMappingTemp(reelMappingTemp); 
                         if (reelMappingTemp.ContainsKey((uint)i)) 
                         { 
                             realIndex =(int)reelMappingTemp[(uint)i]; 
                         } }
                 }
                 
                 if (realIndex >= 0)
                 {
                       ReelSequence reelSequence =  currentActiveSequence[realIndex];
                       var maxIndex = reelSequence.sequenceElements.Count; 
                       rollStopReelIndex[i] = (int) panel.Columns[realIndex].StopPosition; 
                       rollStartReelIndex[i] = ((int) panel.Columns[realIndex].StopPosition - 1 + maxIndex) % maxIndex; 
                       anticipationInColumn[i] = panel.Columns[realIndex].DrumMode;
                 }
                 // else
                 // {
                 //       var maxIndex = currentActiveSequence[0].sequenceElements.Count; 
                 //       rollStopReelIndex[i] = (int) panel.Columns[i].StopPosition; 
                 //       rollStartReelIndex[i] = ((int) panel.Columns[i].StopPosition - 1 + maxIndex) % maxIndex; 
                 //       anticipationInColumn[i] = panel.Columns[i].DrumMode; 
                 // }
             }

             DumpAnticipationInfo();
         }
         
         public override void UpdateWheelStateInfo(Panel panel)
        {
            uint rowNumber = machineState.Get<ExtraState11026>().GetRowsMore();
            if (panel.Columns.count > 3)
            { 
                for (int q =  panel.Columns.count - 1; q >=0; q--) 
                { 
                    var item = panel.Columns[q]; 
                    if ((q % 7) < (3 - rowNumber)) 
                    { 
                        panel.Columns.Remove(item); 
                    } 
                }
            }
            spinResultElementDef =
                sequenceElementConstructor.ConstructSpinResultReelSequence(this, panel);

            UpdateStartAndStopIndex(panel);

            UpdateSubstituteInfo(panel);

            var winLines = panel.WinLines;

            UpdateAppearAnimationInfo(panel);

            UpdateWinLines(winLines);

            DumpServerPanelResult(panel);
        }
         
        public void ResetLinkWheelState()
        {
            for (int i = 0; i < rollCount; i++)
            {
                SetRollLockState(i, false);
            }
        }
    }
}