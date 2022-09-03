using System.Collections.Generic;
using DragonU3DSDK.Network.API.ILProtocol;
using Google.ilruntime.Protobuf.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GameModule
{
    public class WheelStateRight11026 : WheelState
    {
        public WheelStateRight11026(MachineState state) : base(state)
        {
        }

        //根据当前玩法状态获取当前轮盘的卷轴
        public override List<SequenceElement> GetActiveSequenceElement(Roll roll)
        {
            return GetActiveSequenceElement(roll.rollIndex);
        }
        
        public override List<SequenceElement> GetActiveSequenceElement(int rollIndex)
        { 
            var extraState = machineState.machineContext.state.Get<ExtraState11026>();
            int realIndex = 0;
            var realMapping = extraState.GetRightLinkData().ReelMapping;
            if (realMapping.ContainsKey((uint) rollIndex))
            {
                realIndex = (int) realMapping[(uint) rollIndex];
                ReelSequence reelSequence = currentActiveSequence[realIndex]; 
                return reelSequence.sequenceElements;
            }
            else
            {
                 return currentActiveSequence[0].sequenceElements;
            }
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
                var reelMapping = extraState.GetRightLinkData().ReelMapping;
                for (var i = 0; i < rollCount; i++)
                { 
                     int realIndex = i;
                     if (reelMapping.ContainsKey((uint)i)) 
                     { 
                          ReelSequence reelSequence =  currentActiveSequence[realIndex]; 
                          rollStartReelIndex[i] = Random.Range(0, reelSequence.sequenceElements.Count);
                     }
                     else 
                     { 
                         rollStartReelIndex[i] = Random.Range(0, currentActiveSequence[0].sequenceElements.Count); 
                     } 
                }
            }
        }

        protected override void UpdateStartAndStopIndex(Panel panel)
        {
            currentSequenceName = panel.ReelsId;
            currentActiveSequence = sequenceElementConstructor.GetReelSequences(currentSequenceName);

            var extraState = machineState.machineContext.state.Get<ExtraState11026>();
            var reelMapping = extraState.GetRightLinkData().ReelMapping;
            for (var i = 0; i < rollCount; i++)
            {
                int realIndex = i;
                if (reelMapping.ContainsKey((uint)i))
                {
                    ReelSequence reelSequence = currentActiveSequence[realIndex];
                    var maxIndex = reelSequence.sequenceElements.Count;
                    rollStopReelIndex[i] = (int) panel.Columns[realIndex].StopPosition;
                    rollStartReelIndex[i] = ((int) panel.Columns[realIndex].StopPosition - 1 + maxIndex) % maxIndex;
                    anticipationInColumn[i] = panel.Columns[realIndex].DrumMode;
                }
            }
            DumpAnticipationInfo();
        }

        public void ResetLinkWheelState()
        {
            for (int i = 0; i < rollCount; i++)
            {
                SetRollLockState(i, false);
            }
        }
        
        public List<SequenceElement> GetLinkSequenceElementsOnWheel()
        {
            var extraState = machineState.machineContext.state.Get<ExtraState11026>();
            var linkData = extraState.GetRightLinkData();

            List<SequenceElement> sequenceElements = new List<SequenceElement>();

            for (var i = 0; i < linkData.Items.Count; i++)
            {
                if (linkData.Items[i].SymbolId > 0)
                {
                    var symbolId = linkData.Items[i].SymbolId;
                    var elementConfig = machineState.machineConfig.GetElementConfigSet().GetElementConfig(symbolId);
                    sequenceElements.Add(new SequenceElement(elementConfig, machineState.machineContext));
                }
                else
                {
                    var index = UnityEngine.Random.Range(0, Constant11026.ListLinkLoLevelElementIds.Count);

                    var symbolId = Constant11026.ListLinkLoLevelElementIds[index];
                    var elementConfig = machineState.machineConfig.GetElementConfigSet().GetElementConfig(symbolId);
                    sequenceElements.Add(new SequenceElement(elementConfig, machineState.machineContext));
                }
            }
            
            return sequenceElements;
        }
        
        public List<int> GetNewAppearLinkItemIndex()
        {
            var extraState = machineState.machineContext.state.Get<ExtraState11026>();
            var linkData = extraState.GetRightLinkData();

            List<int> newLinkItem = new List<int>();

            for (var i = 0; i < linkData.Items.Count; i++)
            {
                if (linkData.Items[i].SymbolId > 0 && !IsRollLocked(i))
                {
                    newLinkItem.Add(i);
                }
            }
            
            return newLinkItem;
        }



        // public override void UpdateStateOnReceiveSpinResult(SSpin spinResult)
        // {
        //     haveFiveOfKinWinLine = false;
        //     
        //     if (!wheelIsActive)
        //         return;
        //
        //     var wheelSpinResult = spinResult.GameResult.Panels[2];
        //
        //     UpdateWheelStateInfo(wheelSpinResult);
        // }
    }
}