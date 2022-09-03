using System;
using System.Collections.Generic;
using DragonU3DSDK.Network.API.ILProtocol;
// using UnityEditor.PackageManager;
using Random = UnityEngine.Random;

namespace GameModule
{
    public class LinkWheelState11022:WheelState
    {
        public LinkWheelState11022(MachineState state) : base(state)
        {
            wheelIsActive = false;
            currentSequenceName = Constant11022.NormalLinkReels;
        }
        
        public override List<SequenceElement> GetActiveSequenceElement(Roll roll)
         {
             var extraState = machineState.machineContext.state.Get<ExtraState11022>();
             var reelMapping = extraState.GetLinkData().ReelMapping;
             int realIndex = roll.rollIndex;
        
             if (extraState != null)
             {
                 if (extraState.IsLinkNeedInitialized())
                 {
                     realIndex = roll.rollIndex / 3;
                 }
                 else
                 {
                     if (reelMapping.ContainsKey((uint) roll.rollIndex))
                     {
                         realIndex = (int) reelMapping[(uint) roll.rollIndex];
                     }
                     else
                     {
                         realIndex = 0;
                     }
                 }
             }

             try
             {
                 ReelSequence reelSequence = currentActiveSequence[realIndex]; 
                 return reelSequence.sequenceElements;
             }
             catch (Exception e)
             {
                 Console.WriteLine(e);
                 throw;
             }
         }
        
        
         public override List<SequenceElement> GetActiveSequenceElement(int rollIndex)
         {
             var extraState = machineState.machineContext.state.Get<ExtraState11022>();
             var reelMapping = extraState.GetLinkData().ReelMapping;
             int realIndex = rollIndex;
             if (extraState != null)
             {
                 if (extraState.IsLinkNeedInitialized())
                 {
                     realIndex = rollIndex / 3;
                 }
                 else
                 {
                     if (reelMapping.ContainsKey((uint) rollIndex))
                     {
                         realIndex = (int) reelMapping[(uint) rollIndex];
                     }
                     else
                     {
                         realIndex = 0;
                     }
                 }
             }
             ReelSequence reelSequence =  currentActiveSequence[realIndex]; 
             return reelSequence.sequenceElements;
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
             UpdateReelSequences();

             if (startIndex != null && startIndex.Count == rollCount)
             {
                 rollStartReelIndex = startIndex;
             }
             else
             {
                 var extraState = machineState.machineContext.state.Get<ExtraState11022>();
                 if (extraState != null && extraState.IsLinkNeedInitialized())
                 {
                     var tempListRandomStartIndex = new List<int>();
                     for (var i = 0; i < currentActiveSequence.Count; i++)
                     {
                         tempListRandomStartIndex.Add(Random.Range(0, currentActiveSequence[i].sequenceElements.Count));
                     }

                     for (var i = 0; i < rollCount; i++)
                     {
                         int realIndex = i / 3;
                         int rowIndex = i % 3;
                         ReelSequence reelSequence =  currentActiveSequence[realIndex]; 
                         var maxIndex = reelSequence.sequenceElements.Count;
                         rollStartReelIndex[i] = (tempListRandomStartIndex[realIndex] + rowIndex)%maxIndex;
                     }
                 }
                 else
                 {
                     var reelMapping = extraState.GetLinkData()?.ReelMapping;
                     for (var i = 0; i < rollCount; i++)
                     {
                     
                         int realIndex = 0;
                         if (extraState != null && reelMapping.ContainsKey((uint) i))
                         {
                             realIndex = (int)reelMapping[(uint)i];   
                         }

                         ReelSequence reelSequence =  currentActiveSequence[realIndex];
                         var maxIndex = reelSequence.sequenceElements.Count;
                         var randomIndex = Random.Range(0, maxIndex);
                         int continuousCount = 0;
                         for (int j = maxIndex; j > 0; j--)
                         {
                             var realStartIndex = (randomIndex + j) % maxIndex;
                             if (reelSequence.sequenceElements[realStartIndex].config.id == 13)
                             {
                                 continuousCount = 0;
                             }
                             else
                             {
                                 continuousCount++;
                             }

                             if (continuousCount == 3)
                             {
                                 randomIndex = (realStartIndex - 1 + maxIndex)%maxIndex;
                                 break;
                             }
                         }

                         rollStartReelIndex[i] = randomIndex;
                     }   
                 }
             }
         }


         protected override void UpdateStartAndStopIndex(Panel panel)
         {
             currentSequenceName = panel.ReelsId;
             UpdateReelSequences();
             var extraState = machineState.machineContext.state.Get<ExtraState11022>();
             if (extraState != null && extraState.IsLinkNeedInitialized())
             {
                 for (var i = 0; i < rollCount; i++)
                 {
                     int realIndex = i/3;
                     int rowIndex = i % 3;

                     try {
                         ReelSequence reelSequence =  currentActiveSequence[realIndex]; 
                         var maxIndex = reelSequence.sequenceElements.Count;
                         rollStopReelIndex[i] = ((int) panel.Columns[realIndex].StopPosition + rowIndex)% maxIndex;
                         rollStartReelIndex[i] = ((int) panel.Columns[realIndex].StopPosition + rowIndex - 1 + maxIndex) % maxIndex;
                         anticipationInColumn[i] = false; //panel.Columns[realIndex].DrumMode;
                     } catch (System.Exception e) 
                     {
                         XDebug.Log("ERROR");
                     }
                 }   
             }
             else
             {
                 var reelMapping = extraState.GetLinkData()?.ReelMapping;
                 for (var i = 0; i < rollCount; i++)
                 {
                     int realIndex = 0;
                     if (extraState != null && reelMapping.ContainsKey((uint) i))
                     {
                         realIndex = (int) reelMapping[(uint) i];
                     }

                     try {
                         ReelSequence reelSequence =  currentActiveSequence[realIndex]; 
                         var maxIndex = reelSequence.sequenceElements.Count;
                         rollStopReelIndex[i] = (int) panel.Columns[realIndex].StopPosition;
                         rollStartReelIndex[i] = ((int) panel.Columns[realIndex].StopPosition - 1 + maxIndex) % maxIndex;
                         anticipationInColumn[i] = false; //panel.Columns[realIndex].DrumMode;
                     } catch (System.Exception e) 
                     {
                         XDebug.Log("ERROR");
                     }
                 }   
             }
             DumpAnticipationInfo();
         }
         public override IRollUpdaterEasingConfig GetEasingConfig()
         {
             if (machineState.Get<ExtraState11022>().IsLinkNeedInitialized())
             {
                 return machineState.machineConfig.GetEasingConfig("BaseLongWait");
             }
             return machineState.machineConfig.GetEasingConfig(wheelConfig.normalEasingName);
         }
         
         
    }
}