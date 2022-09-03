// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date :2021-06-21 8:11 PM
// Ver : 1.0.0
// Description : RollElementSupplier.cs
// ChangeLog :
// **********************************************

using System.Collections.Generic;

namespace GameModule
{
    public class ElementSupplier : IElementSupplier
    {
        protected List<SequenceElement> activeSequence;
        protected List<SequenceElement> resultSequence;

        protected WheelState wheelState;
        protected Roll roll;
        protected int rollIndex;
        protected int stopIndex;
        protected int maxIndex;
        
        public void InitializeSupplier(WheelState inWheelState, Roll inRollPanel)
        {
            wheelState = inWheelState;
            roll = inRollPanel;
            rollIndex = roll.rollIndex;
        }

        
        public int GetElementMaxHeight()
        {
            return wheelState.GetElementMaxHeight();
        }

        public int GetElementSequenceLength()
        {
            return maxIndex;
        }

        public int ComputeStopIndex(int currentIndex, int slowDownStepCount)
        {
            resultSequence = wheelState.GetSpinResultSequenceElement(roll);

            if (slowDownStepCount > resultSequence.Count)
            {
                currentIndex = (currentIndex - (slowDownStepCount - resultSequence.Count) + maxIndex) % maxIndex;
            }
            
            var sequenceElement = GetElement(currentIndex);
            var placeHolder = (int) (sequenceElement.config.height - sequenceElement.config.position - 1);
            
            //如果当前currentIndex是一个大图标，需要等大图标全部走完之后再替换结果
          //  var leftPlaceHolderCount = activeSequence[currentIndex].config;
            stopIndex = (currentIndex - placeHolder - resultSequence.Count + maxIndex) % maxIndex;
            
            return stopIndex;
        }

        public int GetStartIndex()
        {
            activeSequence = wheelState.GetActiveSequenceElement(roll);
            maxIndex = activeSequence.Count;
            stopIndex = -1;
            return wheelState.GetRollStartReelIndex(rollIndex);;
        }
         
        public int OnStopAtIndex(int inStopIndex)
        {
            stopIndex = inStopIndex;
            resultSequence = wheelState.GetSpinResultSequenceElement(roll);
            return stopIndex;
        }

        public void OnForceUpdateReel()
        {
            activeSequence = wheelState.GetActiveSequenceElement(roll);
            maxIndex = activeSequence.Count;
            stopIndex = -1;
        }
        
        public int GetStopIndex()
        {
           return wheelState.GetRollStopReelIndex(rollIndex);
        }
       
        public SequenceElement GetElement(int index, bool useSeqElement=false)
        {
            if (useSeqElement)
                return activeSequence[index];
            //如果StopIndex大于0，就需要检查是否需要使用服务器下发的结果序列了
            if (stopIndex >= 0 && resultSequence != null)
            {
                int stepToStopIndex = index < stopIndex
                    ? (maxIndex - stopIndex + index)
                    : index - stopIndex;

                //剩余的步数小于结果序列，就开始用结果序列替换
                if (stepToStopIndex < resultSequence.Count)
                {
                    return resultSequence[stepToStopIndex];
                }
            }
            //Stack 卷轴替换，如果发生了替换，就用替换的结果，否则用初始卷轴的结果
            var sequenceElement = wheelState.GetSubstituteElement(rollIndex, index);

            if (sequenceElement != null)
            {
                return sequenceElement;
            }
            
            return activeSequence[index];
        }

        public SequenceElement GetResultElement(int index)
        {
            resultSequence = wheelState.GetSpinResultSequenceElement(roll);

            if (resultSequence != null)
            {
                return resultSequence[index];
            }

            return null;
        }
 
        public bool IsRollLocked(int index)
        {
            return wheelState.IsRollLocked(index);
        }

        public virtual bool CheckResultAtIndex(int index, SequenceElement sequenceElement)
        {
            resultSequence = wheelState.GetSpinResultSequenceElement(roll);
            
            // XDebug.LogError("resultSequence="+(resultSequence != null));
            // XDebug.LogError("resultSequence.Count="+resultSequence?.Count);
            // XDebug.LogError("index="+index);
            // XDebug.LogError("resultSequence[index].config.id="+resultSequence?[index].config.id);
            // XDebug.LogError("sequenceElement.config.id="+sequenceElement.config.id);
            
            if (resultSequence != null && index < resultSequence.Count && resultSequence[index].config.id == sequenceElement.config.id)
            {
                return true;
            }
            return false;
        }
    }
}