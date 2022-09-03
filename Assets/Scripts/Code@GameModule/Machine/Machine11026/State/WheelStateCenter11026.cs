using System.Collections.Generic;
using DragonU3DSDK.Network.API.ILProtocol;
using Google.ilruntime.Protobuf.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GameModule
{
    public class WheelStateCenter11026 : WheelState
    {
    
        private List<SequenceElement> initPanelElements;
        
        public WheelStateCenter11026(MachineState state) : base(state)
        {
        }

        //根据当前玩法状态获取当前轮盘的卷轴
        public override List<SequenceElement> GetActiveSequenceElement(Roll roll)
        {
            return GetActiveSequenceElement(roll.rollIndex);
        }

        public bool IsRealMappingDataIsForCurrentWheel()
        {
            var extraState = machineState.machineContext.state.Get<ExtraState11026>();
            return extraState.GetRowsMore() == (wheelConfig.rollRowCount - 4);
        }

        public override List<SequenceElement> GetActiveSequenceElement(int rollIndex)
        {
            if (IsRealMappingDataIsForCurrentWheel())
            {
                var extraState = machineState.machineContext.state.Get<ExtraState11026>();
                var realMapping = extraState.GetCenterLinkData().ReelMapping;
                int realIndex = 0;
                var col = rollIndex / this.wheelConfig.rollRowCount;
                var row = rollIndex % this.wheelConfig.rollRowCount;
                var transformIndex = (uint) (col * 7 + row + (7 - wheelConfig.rollRowCount));
                if (realMapping.ContainsKey((uint) transformIndex))
                {
                    realIndex = (int) realMapping[transformIndex];
                    ReelSequence reelSequence = currentActiveSequence[realIndex];
                    return reelSequence.sequenceElements;
                }
            }
            
            return currentActiveSequence[0].sequenceElements;
            // realIndex = (int) extraState.GetCenterLinkData().ReelMapping[transformIndex];
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
                bool isValidMappingData = IsRealMappingDataIsForCurrentWheel();
                
                for (var i = 0; i < rollCount; i++)
                { 
                    var transformedIndex = GetTransformedIndex(i);
                    var reelMapping = extraState.GetCenterLinkData().ReelMapping;
                     
                    if (isValidMappingData && reelMapping.ContainsKey(transformedIndex)) 
                    { 
                        var realIndex = (int)reelMapping[transformedIndex];
                        ReelSequence reelSequence = currentActiveSequence[realIndex];
                        rollStartReelIndex[i] = Random.Range(0, reelSequence.sequenceElements.Count);
                    }
                    else
                    {
                        rollStartReelIndex[i] = Random.Range(0, currentActiveSequence[0].sequenceElements.Count);
                    }
                    
                }
            }
        }
        protected uint GetTransformedIndex(int rollIndex)
        {
            var col = rollIndex / this.wheelConfig.rollRowCount;
            var row = rollIndex % this.wheelConfig.rollRowCount;
            var transformIndex = (uint) (col * 7 + row + (7 - wheelConfig.rollRowCount));
            return transformIndex;
        }
        
        protected override void UpdateStartAndStopIndex(Panel panel)
        {
            currentSequenceName = panel.ReelsId;
            currentActiveSequence = sequenceElementConstructor.GetReelSequences(currentSequenceName);
             
            var extraState = machineState.machineContext.state.Get<ExtraState11026>();
            var reelMapping = extraState.GetCenterLinkData().ReelMapping;
          
            for (var i = 0; i < rollCount; i++)
            {
                var transformedIndex = GetTransformedIndex(i);
                if (reelMapping.ContainsKey(transformedIndex))
                {
                    var realIndex = (int)reelMapping[transformedIndex];
                    if (currentActiveSequence.Count <= realIndex)
                    {
                        realIndex = 0;
                    }
                    ReelSequence reelSequence = currentActiveSequence[realIndex];
                    var maxIndex = reelSequence.sequenceElements.Count;
                    rollStopReelIndex[i] = (int) panel.Columns[realIndex].StopPosition;
                    rollStartReelIndex[i] = ((int) panel.Columns[realIndex].StopPosition - 1 + maxIndex) % maxIndex;
                    anticipationInColumn[i] = panel.Columns[realIndex].DrumMode;
                }
            }
            DumpAnticipationInfo();
        }

        public override void UpdateWheelStateInfo(Panel panel)
        {
            
            UpdateInitPanelElements(panel);
            
            var rowNumber = wheelConfig.rollRowCount - 4;
            
            if (panel.Columns.count > 3)
            {
                for (int q = panel.Columns.count - 1; q >= 0; q--)
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

        public void UpdateInitPanelElements(Panel panel)
        {
            if (initPanelElements == null)
            {
                initPanelElements = new List<SequenceElement>();
            }
            
            initPanelElements.Clear();

            var extraState = machineState.Get<ExtraState11026>();

            var linkData = extraState.GetCenterLinkData();
            
            for (var i = 0; i < panel.Columns.Count; i++)
            {
                for (var j = 0; j < panel.Columns[i].Symbols.Count; j++)
                {
                    var symbolId = panel.Columns[i].Symbols[j];

                    if (linkData.Items[initPanelElements.Count].SymbolId > 0)
                    {
                        symbolId = linkData.Items[initPanelElements.Count].SymbolId;
                    }
                    
                    var elementConfig = machineState.machineConfig.GetElementConfigSet().GetElementConfig(symbolId);
                    
                    initPanelElements.Add(new SequenceElement(elementConfig, machineState.machineContext));
                }
            }
        }

        public void ResetLinkWheelState()
        {
            for (int i = 0; i < rollCount; i++)
            {
                SetRollLockState(i, false);
            }

            initPanelElements = null;
        }

        protected List<SequenceElement> GetInitPanelElements()
        {
            if (initPanelElements == null)
            {
                initPanelElements = new List<SequenceElement>();

                var extraState = machineState.machineContext.state.Get<ExtraState11026>();
                var linkData = extraState.GetCenterLinkData();

                for (var i = 0; i < linkData.Items.Count; i++)
                {
                    if (linkData.Items[i].SymbolId > 0)
                    {
                        var symbolId = linkData.Items[i].SymbolId;
                        var elementConfig = machineState.machineConfig.GetElementConfigSet()
                            .GetElementConfig(symbolId);
                        initPanelElements.Add(new SequenceElement(elementConfig, machineState.machineContext));
                    }
                    else
                    {
                        var index = UnityEngine.Random.Range(0, Constant11026.ListLinkLoLevelElementIds.Count);

                        var symbolId = Constant11026.ListLinkLoLevelElementIds[index];
                        var elementConfig = machineState.machineConfig.GetElementConfigSet()
                            .GetElementConfig(symbolId);
                        initPanelElements.Add(new SequenceElement(elementConfig, machineState.machineContext));
                    }
                }
            }

            return initPanelElements;
        }
        
        public List<SequenceElement> GetLinkSequenceElementsOnWheel()
        {
            if (initPanelElements == null)
            {
                if (wheelConfig.rollRowCount > 4)
                {
                    initPanelElements =  machineState.Get<WheelStateCenter11026>(wheelConfig.rollRowCount - 4 - 1).GetInitPanelElements();
                }
                else
                {
                    initPanelElements = GetInitPanelElements();
                }
            }

            List<SequenceElement> sequenceElements = new List<SequenceElement>();
           
            for (var i = 0; i < initPanelElements.Count; i++)
            {
                var rollIndex = i;
                var row = rollIndex % 7;
                    
                if (7 - wheelConfig.rollRowCount > row)
                {
                    continue;
                }
                
                sequenceElements.Add(initPanelElements[i]);
            }

            return sequenceElements;
        }
        
        public List<int> GetNewAppearLinkItemIndex()
        {
            var extraState = machineState.machineContext.state.Get<ExtraState11026>();
            var linkData = extraState.GetCenterLinkData();

            List<int> newLinkItem = new List<int>();

            for (var i = 0; i < linkData.Items.Count; i++)
            {
                var rollIndex = i;
                var row = rollIndex % 7;

                if (7 - wheelConfig.rollRowCount > row)
                {
                    continue;
                }

                var transformedRollIndex = i / 7 * wheelConfig.rollRowCount + row - (7 - wheelConfig.rollRowCount);

                if (linkData.Items[i].SymbolId > 0 && !IsRollLocked(transformedRollIndex))
                {
                    newLinkItem.Add(transformedRollIndex);
                }
            }

            return newLinkItem;
        }


        
      // public override void UpdateStateOnReceiveSpinResult(SSpin spinResult)
      //   {
      //       haveFiveOfKinWinLine = false;
      //       
      //       if (!wheelIsActive)
      //           return;
      //
      //       var wheelSpinResult = spinResult.GameResult.Panels[1];
      //
      //       UpdateWheelStateInfo(wheelSpinResult);
      //   }
    }
}