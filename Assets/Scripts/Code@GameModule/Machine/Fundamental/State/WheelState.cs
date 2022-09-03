// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date :2021-07-05 12:02 PM
// Ver : 1.0.0
// Description : WheelState.cs
// ChangeLog :
// **********************************************

using System;
using System.Collections.Generic;
using DragonU3DSDK.Network.API.ILProtocol;
using Google.ilruntime.Protobuf.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GameModule
{
    public class WheelState : SubState
    {
        protected List<ReelSequence> spinResultElementDef;
        protected List<ReelSequence> currentActiveSequence;

        protected Dictionary<int, Dictionary<int, SequenceElement>> substituteInfo;

        protected WheelConfig wheelConfig;

        protected string currentSequenceName;

        protected List<bool> rollLockStatus;

        public int rollCount;

        protected bool wheelIsActive;

        public List<bool> anticipationInColumn;

        public ElementConfigSet elementConfigSet;

        public bool playerQuickStopped = false;

        public string wheelName;

        public bool IsShowPayLine = true;
        public bool IsPayLineUpElement = false;
        public int PayLineOffsetZOrder = 0;

        protected int resultIndex;

        protected ISequenceElementConstructor sequenceElementConstructor;

        protected List<int> rollStartReelIndex;
        protected List<int> rollStopReelIndex;

        protected List<WinLine> normalWinLines;
        protected List<WinLine> bonusWinLine;
        protected List<WinLine> freeSpinTriggerWinLines;
        protected List<WinLine> jackpotWinLines;
        protected List<WinLine> reSpinWinLines;

        protected List<RepeatedField<uint>> blinkRows;

        protected bool haveFiveOfKinWinLine = false;

        public WheelState(MachineState state) : base(state)
        {
            wheelIsActive = false;
            currentSequenceName = "Reels";
        }
#if UNITY_EDITOR
        public void RefreshWheelConfig()
        {
            wheelConfig = machineState.machineConfig.GetWheelConfig(wheelName);
        }
#endif
        public virtual void InitializeWheelState(string inWheelName)
        {
            wheelName = inWheelName;

            wheelConfig = machineState.machineConfig.GetWheelConfig(wheelName);

            rollCount = wheelConfig.isIndependentWheel
                ? wheelConfig.rollCount * wheelConfig.rollRowCount
                : wheelConfig.rollCount;
           
            IsPayLineUpElement = wheelConfig.isPayLineUpElement;

            resultIndex = 0;

            rollLockStatus = new List<bool>(rollCount);
            rollStartReelIndex = new List<int>(rollCount);
            rollStopReelIndex = new List<int>(rollCount);

            normalWinLines = new List<WinLine>(100);
            bonusWinLine = new List<WinLine>(100);
            freeSpinTriggerWinLines = new List<WinLine>(100);
            jackpotWinLines = new List<WinLine>(100);
            reSpinWinLines = new List<WinLine>(100);


            blinkRows = new List<RepeatedField<uint>>(rollCount);

            anticipationInColumn = new List<bool>(rollCount);

            for (var i = 0; i < rollCount; i++)
            {
                blinkRows.Add(null);
                rollLockStatus.Add(false);
                anticipationInColumn.Add(false);
                rollStartReelIndex.Add(0);
                rollStopReelIndex.Add(0);
            }
        }

        public void UpdateWheelActiveState(bool isActive)
        {
            wheelIsActive = isActive;
            UpdateReelSequences();
            // currentActiveSequence = sequenceElementConstructor?.GetReelSequences(currentSequenceName);
        }

        public virtual void UpdateReelSequences()
        {
            currentActiveSequence = sequenceElementConstructor?.GetReelSequences(currentSequenceName);
        }

        public override void UpdateStateOnRoomSetUp(SEnterGame gameEnterInfo)
        {
            sequenceElementConstructor = machineState.machineContext.sequenceElementConstructor;

            if (!wheelIsActive)
                return;

            var gameResult = gameEnterInfo.GameResult;

            if (gameResult.Panels.Count > resultIndex)
            {
                currentSequenceName = gameEnterInfo.GameResult.Panels[resultIndex].ReelsId;
                UpdateReelSequences();
                // currentActiveSequence = sequenceElementConstructor.GetReelSequences(currentSequenceName);

                var panel = gameEnterInfo.GameResult.Panels[resultIndex];

                UpdateWheelStateInfo(panel);
            }
            else
            {
                UpdateReelSequences();
                // currentActiveSequence = sequenceElementConstructor.GetReelSequences(currentSequenceName);
            }
        }

        // public override void Update
        public override void UpdateStateOnSubRoundStart()
        {
            if (!wheelIsActive)
                return;

            playerQuickStopped = false;
            UpdateReelSequences();
            // currentActiveSequence = sequenceElementConstructor.GetReelSequences(currentSequenceName);
        }

        public void UpdateSubstituteInfo(Panel panel)
        {
            substituteInfo = null;
            if (panel.Substitutes != null)
                substituteInfo = sequenceElementConstructor.ConstructReelSubstituteInfo(panel.Substitutes);
        }

        public WheelConfig GetWheelConfig()
        {
            return wheelConfig;
        }

        /// <summary>
        /// 可能服务器下发panel index和 客户端的wheel index不是对应的，这里提供一个接口可以来修改resultIndex
        /// 保证转动结果的数据正确
        /// </summary>
        /// <param name="inResultIndex"></param>
        public void SetResultIndex(int inResultIndex)
        {
            resultIndex = inResultIndex;
        }

        public override void UpdateStateOnReceiveSpinResult(SSpin spinResult)
        {
            haveFiveOfKinWinLine = false;
            
            if (!wheelIsActive)
                return;

            var wheelSpinResult = spinResult.GameResult.Panels[resultIndex];

            UpdateWheelStateInfo(wheelSpinResult);
        }

        public virtual void UpdateWheelStateInfo(Panel panel)
        {
            spinResultElementDef =
                sequenceElementConstructor.ConstructSpinResultReelSequence(this, panel);

            UpdateStartAndStopIndex(panel);

            UpdateSubstituteInfo(panel);

            var winLines = panel.WinLines;

            UpdateAppearAnimationInfo(panel);

            UpdateWinLines(winLines);
            
#if !PRODUCTION_PACKAGE
            DumpServerPanelResult(panel);
#endif
        }

        public SequenceElement GetSubstituteElement(int rollIndex, int posIndex)
        {
            if (substituteInfo != null)
            {
                if (substituteInfo.ContainsKey(rollIndex) && substituteInfo[rollIndex].ContainsKey(posIndex))
                {
                    return substituteInfo[rollIndex][posIndex];
                }
            }

            return null;
        }

        public void UpdateWinLines(RepeatedField<WinLine> winLines)
        {
            normalWinLines.Clear();
            freeSpinTriggerWinLines.Clear();
            bonusWinLine.Clear();
            jackpotWinLines.Clear();
            reSpinWinLines.Clear();

            PreUpdateWinLines(winLines);
            for (var i = 0; i < winLines.Count; i++)
            {
                if (winLines[i].Pay > 0 && winLines[i].PayLineId > 0 && winLines[i].ShowSymbolAnimation)
                {
                    normalWinLines.Add(winLines[i]);
                }

                if (winLines[i].BonusGameId != 0)
                {
                    bonusWinLine.Add(winLines[i]);
                }
                else if (winLines[i].FreeSpinCount > 0 || winLines[i].FreeSpinChoices > 0)
                {
                    freeSpinTriggerWinLines.Add(winLines[i]);
                }

                if (winLines[i].JackpotId != 0)
                {
                    jackpotWinLines.Add(winLines[i]);
                }

                if (winLines[i].ReSpinCount > 0)
                {
                    reSpinWinLines.Add(winLines[i]);
                }
            }

            if (winLines.Count > 0)
            {
                CheckHasFiveOfKindWinLine(winLines);
                DumpAllWinLine(winLines);
            }
        }

        protected  virtual void PreUpdateWinLines(RepeatedField<WinLine> winLines)
        {
            
        }
        

        protected void CheckHasFiveOfKindWinLine(RepeatedField<WinLine> winLines)
        {
            haveFiveOfKinWinLine = false;

            for (var i = 0; i < winLines.Count; i++)
            {
                if (winLines[i].SymbolCount >= 5 && winLines[i].BonusGameId == 0 && winLines[i].Mask > 0)
                {
                    //multi way
                    if (winLines[i].IsMultiway)
                    {
                        var count = winLines[i].Positions.Count;
                        for (var c = 0; c < count; c++)
                        {
                            if (winLines[i].Positions[c].X >= 4)
                            {
                                haveFiveOfKinWinLine = true;
                            }
                        }
                    }
                    //Normal WinLine
                    else
                    {
                        haveFiveOfKinWinLine = true;
                    }
                }
            }
        }

        public bool HaveFiveKindWinLine()
        {
            return haveFiveOfKinWinLine;
        }

        /// <summary>
        /// 更新卷轴
        /// </summary>
        /// <param name="sequenceName">新的卷轴</param>
        /// <param name="startIndex">新的初始化</param>
        public virtual void UpdateCurrentActiveSequence(string sequenceName, List<int> startIndex = null)
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
                for (var i = 0; i < rollCount; i++)
                {

                    ReelSequence reelSequence = i >= currentActiveSequence.Count
                        ? currentActiveSequence[0]
                        : currentActiveSequence[i]; 
                    rollStartReelIndex[i] = Random.Range(0, reelSequence.sequenceElements.Count);
                }
            }
        }

        protected virtual void UpdateAppearAnimationInfo(Panel panel)
        {
            for (var i = 0; i < rollCount; i++)
            {
                blinkRows[i] = panel.Columns[i].AppearingRows;
            }
        }

        public RepeatedField<uint> GetBlinkAnimationInfo(int rollIndex)
        {
            return blinkRows[rollIndex];
        }

        protected virtual void UpdateStartAndStopIndex(Panel panel)
        {
            currentSequenceName = panel.ReelsId;
            UpdateReelSequences();
            // currentActiveSequence = sequenceElementConstructor.GetReelSequences(currentSequenceName);

            for (var i = 0; i < rollCount; i++)
            {
                
                ReelSequence reelSequence = i >= currentActiveSequence.Count
                    ? currentActiveSequence[0]
                    : currentActiveSequence[i]; 
                
                var maxIndex = reelSequence.sequenceElements.Count;
                rollStopReelIndex[i] = (int) panel.Columns[i].StopPosition;
                rollStartReelIndex[i] = ((int) panel.Columns[i].StopPosition - 1 + maxIndex) % maxIndex;

                anticipationInColumn[i] = panel.Columns[i].DrumMode;
            }

            DumpAnticipationInfo();
        }


        protected void DumpAnticipationInfo()
        {
            string anticipationInfo = "AnticipationInfo:";
            for (var i = 0; i < rollCount; i++)
            {
                anticipationInfo += anticipationInColumn[i] + ";";
            }

            XDebug.Log(anticipationInfo);
        }

        public bool HasBonusLine()
        {
            return bonusWinLine.Count > 0;
        }

        public List<WinLine> GetBonusWinLine()
        {
            return bonusWinLine;
        }

        public List<WinLine> GetReSpinWinLine()
        {
            return reSpinWinLines;
        }

        public List<WinLine> GetJackpotWinLines()
        {
            return jackpotWinLines;
        }

        public List<WinLine> GetFreeSpinTriggerLine()
        {
            return freeSpinTriggerWinLines;
        }

        public List<WinLine> GetNormalWinLine()
        {
            return normalWinLines;
        }

        public bool HasNormalWinLine()
        {
            return normalWinLines.Count > 0;
        }

        protected void DumpAllWinLine(RepeatedField<WinLine> winLines)
        {
            var winLineString = "";

            for (var i = 0; i < winLines.Count; i++)
            {
                string oneLine = "[";
                for (var p = 0; p < winLines[i].Positions.count; p++)
                {
                    oneLine += $"({winLines[i].Positions[p].X},{winLines[i].Positions[p].Y}),";
                }

                oneLine += "]\n";
                winLineString += oneLine;
            }

            XDebug.Log("MACHINE[WIN_LINE]:\n" + winLineString);
        }

        protected void DumpServerPanelResult(Panel panel)
        {
            string result = "";
            
            var configSet = machineState.machineConfig.GetElementConfigSet();
 
            for (var i = 0; i < panel.Columns.Count; i++)
            {
                var column = panel.Columns[i];
                var symbolCount = column.Symbols.Count;
                for (var j = 0; j < symbolCount; j++)
                {
                    if (j == 0)
                    {
                        result += "[";
                    }
                    result += configSet.GetElementConfig(column.Symbols[j]).name;
                    
                    result += (j == symbolCount - 1) ? "]\n" : ",";
                }
            }

            XDebug.Log(string.Format("<color=yellow>{0}</color>", "MACHINE[WHEEL_RESULT]:\n") + result);
        }

        //result sequence需要包含滚轮上，从结果到后续不可见的SequenceElement结果
        public virtual List<SequenceElement> GetSpinResultSequenceElement(Roll roll)
        {
            if (spinResultElementDef != null)
                return spinResultElementDef[roll.rollIndex].sequenceElements;
            return null;
        }


        /// <summary>
        /// Spin结果在指定的列上是否包含指定的element
        /// </summary>
        /// <param name="elementId"></param>
        /// <param name="rollIndex"></param>
        /// <returns></returns>
        public bool IsSpinResultContainElementAtRollIndex(uint elementId, int rollIndex)
        {
            var sequenceElements = spinResultElementDef[rollIndex].sequenceElements;

            for (var i = 1 + wheelConfig.extraTopElementCount; i < sequenceElements.Count - (wheelConfig.elementMaxHeight - 1); i++)
            {
                if (sequenceElements[i].config.id == elementId)
                    return true;
            }
            
            
            return false;
        }

        //根据当前玩法状态获取当前轮盘的卷轴
        public virtual List<SequenceElement> GetActiveSequenceElement(Roll roll)
        {
            ReelSequence reelSequence = roll.rollIndex >= currentActiveSequence.Count
                ? currentActiveSequence[0]
                : currentActiveSequence[roll.rollIndex]; 
            return reelSequence.sequenceElements;
        }


        public virtual List<SequenceElement> GetActiveSequenceElement(int rollIndex)
        {
            ReelSequence reelSequence = rollIndex >= currentActiveSequence.Count
                ? currentActiveSequence[0]
                : currentActiveSequence[rollIndex]; 
            return reelSequence.sequenceElements;
        }

        public int GetElementMaxHeight()
        {
            return wheelConfig.elementMaxHeight;
        }
        public virtual int GetExtraTopElementCount()
        {
            return wheelConfig.extraTopElementCount;
        }

        public virtual int GetAnticipationAnimationStartRollIndex()
        {
            for (var i = 0; i < anticipationInColumn.Count; i++)
            {
                if (anticipationInColumn[i] == true)
                    return i;
            }

            return rollCount;
        }

        public virtual bool HasAnticipationAnimationInRollIndex(int rollIndex)
        {
            return anticipationInColumn[rollIndex];
        }

        public string GetAnticipationAnimationAssetName()
        {
            return wheelConfig.anticipationAnimationAssetName;
        }

        public string GetAnticipationSoundAssetName()
        {
            return wheelConfig.anticipationSoundAssetName;
        }

        public virtual IRollUpdaterEasingConfig GetEasingConfig()
        {
            if (machineState.Get<FreeSpinState>().NextIsFreeSpin)
                return machineState.machineConfig.GetEasingConfig(wheelConfig.freeEasingName);

            return machineState.machineConfig.GetEasingConfig(wheelConfig.normalEasingName);
        }

        public bool IsRollLocked(int rollIndex)
        {
            return rollLockStatus[rollIndex];
        }

        public bool SetRollLockState(int rollIndex, bool lockState)
        {
            return rollLockStatus[rollIndex] = lockState;
        }

        public virtual int GetRollStartReelIndex(int rollIndex)
        {
            return rollStartReelIndex[rollIndex];
        }

        public virtual int GetRollStopReelIndex(int rollIndex)
        {
            return rollStopReelIndex[rollIndex];
        }

        public override void UpdateStateBeforeCallRoundFinish()
        {
            var freeState = machineState.Get<FreeSpinState>();
            if (freeState.IsInFreeSpin && !freeState.NextIsFreeSpin)
            {
                ForceUpdateWinLineState();
            }
        }

        public void ForceUpdateWinLineState()
        {
            normalWinLines.Clear();
            bonusWinLine.Clear();
            jackpotWinLines.Clear();
            reSpinWinLines.Clear();
        }

        public override bool MatchFilter(string filter)
        {
            return filter == wheelName;
        }

        public virtual void ResetSpinResultElementDef()
        {
            spinResultElementDef = null;
        }
    }
}