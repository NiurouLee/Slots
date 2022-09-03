using System.Collections;
using System.Collections.Generic;
using DragonU3DSDK.Network.API.ILProtocol;
using UnityEngine;
namespace GameModule{
    public class LinkWheelState11312 : WheelState
    {
        public LinkWheelState11312(MachineState state) : base(state)
        {
        }
        public override void UpdateStateOnReceiveSpinResult(SSpin spinResult)
        {
            haveFiveOfKinWinLine = false;
            // 坑点-- 当切换轮盘时， wheelIsActive 为false 此时不会刷新spinResultElementDef。所以再次触发respin会一定几率报错
            var gameResult = spinResult.GameResult;
            if(gameResult.ReSpinInfo.ReSpinLimit!=0 && gameResult.ReSpinInfo.ReSpinCount==0){
                XDebug.Log("respin触发==");
                spinResultElementDef = null;
            }
            
            if (!wheelIsActive)
                return;

            var wheelSpinResult = spinResult.GameResult.Panels[resultIndex];

            UpdateWheelStateInfo(wheelSpinResult);
        }
        public override void UpdateWheelStateInfo(Panel panel)
        {
            spinResultElementDef =
                sequenceElementConstructor.ConstructSpinResultReelSequence(this, panel);

            UpdateStartAndStopIndex(panel);

            UpdateSubstituteInfo(panel);

            var winLines = panel.WinLines;

            UpdateAppearAnimationInfo(panel);

            UpdateWinLines(winLines);

            DumpServerPanelResult(panel);
        }
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
                for (var i = 0; i < rollCount; i++)
                {

                    ReelSequence reelSequence = i >= currentActiveSequence.Count
                        ? currentActiveSequence[0]
                        : currentActiveSequence[i]; 
                    // rollStartReelIndex[i] = Random.Range(0, reelSequence.sequenceElements.Count);
                    rollStartReelIndex[i] = 0;
                }
            }
        }
        public override IRollUpdaterEasingConfig GetEasingConfig()
        {
            var respinState = machineState.Get<ReSpinState11312>();
            if(respinState.IsInRespin){
                if(respinState.ReSpinCount == respinState.ReSpinLimit)
                    return machineState.machineConfig.GetEasingConfig(Constant11312.LastLinkEasingConfig);
                return machineState.machineConfig.GetEasingConfig(wheelConfig.reSpinEasingName);
            }
            else if (machineState.Get<FreeSpinState>().NextIsFreeSpin){
                return machineState.machineConfig.GetEasingConfig(wheelConfig.freeEasingName);
            }
            
            return machineState.machineConfig.GetEasingConfig(wheelConfig.normalEasingName);
        }

        protected override void UpdateStartAndStopIndex(Panel panel)
        {
            currentSequenceName = panel.ReelsId;
            currentActiveSequence = sequenceElementConstructor.GetReelSequences(currentSequenceName);
            XDebug.Log("currentSequenceName===:"+currentSequenceName);
            if(Constant11312.SmallSeqWheelName.Contains(currentSequenceName)){
                rollCount = 1;
            }
                
            for (var i = 0; i < rollCount; i++)
            {
                
                ReelSequence reelSequence = i >= currentActiveSequence.Count
                    ? currentActiveSequence[0]
                    : currentActiveSequence[i]; 
                
                var maxIndex = reelSequence.sequenceElements.Count;
                rollStopReelIndex[i] = (int) panel.Columns[i].StopPosition;
                // rollStartReelIndex[i] = ((int) panel.Columns[i].StopPosition - 1 + maxIndex) % maxIndex;
                rollStartReelIndex[i] = 0;

                anticipationInColumn[i] = panel.Columns[i].DrumMode;
            }

            DumpAnticipationInfo();
        }
        
    }
}

