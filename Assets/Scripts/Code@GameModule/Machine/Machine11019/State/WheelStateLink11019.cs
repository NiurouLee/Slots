
using System.Collections.Generic;
using DragonU3DSDK.Network.API.ILProtocol;
using Random = UnityEngine.Random;

namespace GameModule
{
    public class WheelStateLink11019: WheelState
    {
        public WheelStateLink11019(MachineState state) : base(state)
        {
            
        }


        public override void UpdateStatePreRoomSetUp(SEnterGame gameEnterInfo)
        {
            base.UpdateStatePreRoomSetUp(gameEnterInfo);
        }


        //根据当前玩法状态获取当前轮盘的卷轴
         public override List<SequenceElement> GetActiveSequenceElement(Roll roll)
         {
             var extraState = machineState.machineContext.state.Get<ExtraState11019>();
             int realIndex = roll.rollIndex;
        
             if (extraState != null)
             {
                 realIndex = (int)extraState.GetLinkData().ReelMapping[(uint)roll.rollIndex];
             }
        
             ReelSequence reelSequence = currentActiveSequence[realIndex]; 
             return reelSequence.sequenceElements;
         }
        
        
         public override List<SequenceElement> GetActiveSequenceElement(int rollIndex)
         {
             var extraState = machineState.machineContext.state.Get<ExtraState11019>();
             int realIndex = rollIndex;
             if (extraState != null)
             {
                 realIndex = (int)extraState.GetLinkData().ReelMapping[(uint)rollIndex];
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

             currentActiveSequence = sequenceElementConstructor.GetReelSequences(currentSequenceName);

             if (startIndex != null && startIndex.Count == rollCount)
             {
                 rollStartReelIndex = startIndex;
             }
             else
             {
                 var extraState = machineState.machineContext.state.Get<ExtraState11019>();
                 for (var i = 0; i < rollCount; i++)
                 {
                     
                     int realIndex = i;
                     if (extraState != null)
                     {
                         realIndex = (int)extraState.GetLinkData().ReelMapping[(uint)i];
                     }

                     ReelSequence reelSequence =  currentActiveSequence[realIndex]; 
                     rollStartReelIndex[i] = Random.Range(0, reelSequence.sequenceElements.Count);
                 }
             }
         }
         
         
         
         
         protected override void UpdateStartAndStopIndex(Panel panel)
         {
             currentSequenceName = panel.ReelsId;
             currentActiveSequence = sequenceElementConstructor.GetReelSequences(currentSequenceName);

             var extraState = machineState.machineContext.state.Get<ExtraState11019>();
             for (var i = 0; i < rollCount; i++)
             {
                 
                 
                 
                 int realIndex = i;
                 if (extraState != null)
                 {
                     realIndex = (int)extraState.GetLinkData().ReelMapping[(uint)i];
                 }
                 
                
                 ReelSequence reelSequence =  currentActiveSequence[realIndex]; 
                
                 var maxIndex = reelSequence.sequenceElements.Count;
                 rollStopReelIndex[i] = (int) panel.Columns[realIndex].StopPosition;
                 rollStartReelIndex[i] = ((int) panel.Columns[realIndex].StopPosition - 1 + maxIndex) % maxIndex;

                 anticipationInColumn[i] = panel.Columns[realIndex].DrumMode;
             }

             DumpAnticipationInfo();
         }
    }
}