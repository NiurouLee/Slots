// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date :2021-07-06 4:57 PM
// Ver : 1.0.0
// Description : ReelSequenceMap.cs
// ChangeLog :
// **********************************************

using System.Collections.Generic;
using DragonU3DSDK.Network.API.ILProtocol;
using Google.ilruntime.Protobuf.Collections;

namespace GameModule
{
    public class SequenceElementConstructor:ISequenceElementConstructor
    {
        protected ElementConfigSet elementConfigSet;
        protected Dictionary<string, List<ReelSequence>> cachedSequence;

        protected MachineContext machineContext;
        public SequenceElementConstructor(MachineContext inMachineContext)
        {
            cachedSequence = new Dictionary<string, List<ReelSequence>>();
            machineContext = inMachineContext;
        }

        public virtual void ConstructReelSequenceFromServerData(ElementConfigSet inElementConfigSet, MapField<string, Reels> reelsMap)
        {
            elementConfigSet = inElementConfigSet;
            foreach (var reels in reelsMap)
            {
                var sequences = ConstructReelSequence(reels.Key, reels.Value, elementConfigSet);
                cachedSequence.Add(reels.Key, sequences);
            }
        }

        public virtual List<ReelSequence> ConstructReelSequence(string sequenceKey, Reels reels, ElementConfigSet inElementConfigSet)
        {
            var reelsCount = reels.Reels_.Count;
            var sequences = new List<ReelSequence>(reelsCount);

            for (var i = 0; i < reelsCount; i++)
            {
                sequences.Add(new ReelSequence(inElementConfigSet, reels.Reels_[i], machineContext));
            }

            return sequences;
        }

        public virtual List<ReelSequence> ConstructSpinResultReelSequence(WheelState wheelState, DragonU3DSDK.Network.API.ILProtocol.Panel panel)
        {
            var columnsCount = panel.Columns.Count;
            var sequences = new List<ReelSequence>(columnsCount);
            var reelSequences = GetReelSequences(panel.ReelsId);
            var maxElementHeight = wheelState.GetWheelConfig().elementMaxHeight;
            var maxExtraTopElementCount = wheelState.GetWheelConfig().extraTopElementCount;
          
            for (var i = 0; i < columnsCount; i++)
            {
                var resultSequence = new ReelSequence(elementConfigSet, panel.Columns[i], machineContext);
                
                resultSequence = AppendExtraSequenceElement(GetFixedReelSequence(reelSequences, i, wheelState), resultSequence, (int )panel.Columns[i].StopPosition,maxElementHeight-1,i,maxExtraTopElementCount);
                
                sequences.Add(resultSequence);
            }

            PostProcessSpinResultSequence(sequences, wheelState, panel);
            return sequences;
        }

        public virtual void PostProcessSpinResultSequence(List<ReelSequence> sequences, WheelState wheelState,
            Panel panel)
        {
            
        }
        
        protected virtual ReelSequence GetFixedReelSequence(List<ReelSequence> sequences, int column, WheelState wheelState)
        {
            //Link玩法取第0个
            if (column >= sequences.Count)
            {
                return sequences[0];
            }

            return sequences[column];
        }

        protected virtual ReelSequence AppendExtraSequenceElement(ReelSequence reelSequence, ReelSequence resultSequence, int stopPosition, int extraElementAppend,int column,int extraTopElementAppend)
        {
            var elementCountInReelSequence = reelSequence.sequenceElements.Count;

            int visibleSymbolCount = resultSequence.sequenceElements.Count;
            var nextElementIndex = (stopPosition - 1 + elementCountInReelSequence) %
                                   elementCountInReelSequence;
            
            var nextElement = reelSequence.sequenceElements[nextElementIndex];
            
            //AppendHeadElement
            resultSequence.sequenceElements.Insert(0, nextElement);
            if (extraTopElementAppend > 0)
            {
                for (var a = 0; a < extraTopElementAppend; a++)
                {
                    var tempNextElementIndex = (stopPosition - 1 - (1+a) + elementCountInReelSequence * (a+2)) %
                                           elementCountInReelSequence;
            
                    var tempNextElement = reelSequence.sequenceElements[tempNextElementIndex];
            
                    //AppendHeadElement
                    resultSequence.sequenceElements.Insert(0, tempNextElement);
                }
            }
            
            //AppendExtraTailElement
            if (extraElementAppend > 0)
            {
                var firstPos = stopPosition + visibleSymbolCount;
                for (var a = 0; a < extraElementAppend; a++)
                {
                    var appendElement = reelSequence.sequenceElements[
                        (int)(firstPos + a) % elementCountInReelSequence];
                        
                    resultSequence.sequenceElements.Add(appendElement);
                }
            }
            
            // XDebug.Log("rollIndex:" + column + " stopPosition:" + stopPosition);
            // for (int i = 0; i < resultSequence.sequenceElements.Count; i++)
            // {
            //     XDebug.Log(resultSequence.sequenceElements[i].config.id);
            // }
            return resultSequence;
        }

        public void ConstructReelSequence(WheelState wheelState, int maxLength = 20)
        {
            cachedSequence = new Dictionary<string, List<ReelSequence>>();

            var sequences = new List<ReelSequence>(wheelState.rollCount);

            for (var i = 0; i < wheelState.rollCount; i++)
            {
                sequences.Add(new ReelSequence(wheelState.elementConfigSet, maxLength, machineContext));
            }
            
            cachedSequence.Add("Normal", sequences);
            
        }

        public List<ReelSequence> GetReelSequences(string reelName)
        {
            if (cachedSequence.ContainsKey(reelName))
            {
                return cachedSequence[reelName];
            }
            
            return null;
        }

        public Dictionary<int, Dictionary<int, SequenceElement>> ConstructReelSubstituteInfo(
            RepeatedField<Panel.Types.ReelSubstitute> substitutes)
        {
            if (substitutes == null || substitutes.Count <= 0)
                return null;

            var substituteInfo = new Dictionary<int, Dictionary<int, SequenceElement>>();

            for (var i = 0; i < substitutes.Count; i++)
            {
                var col = (int) substitutes[i].Col;

                if (!substituteInfo.ContainsKey(col))
                {
                    substituteInfo[col] = new Dictionary<int, SequenceElement>();
                }

                var pos = substitutes[i].Pos;

                for (var p = 0; p < pos.Count; p++)
                {
                    substituteInfo[col][(int) substitutes[i].Pos[p]] =
                        new SequenceElement(elementConfigSet.GetElementConfig(substitutes[i].To), machineContext);
                }
            }

            return substituteInfo;
        }
    }
}