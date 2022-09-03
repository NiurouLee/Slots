// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date :2021-07-05 11:30 AM
// Ver : 1.0.0
// Description : ISequenceConstructor.cs
// ChangeLog :
// **********************************************

namespace GameModule
{
    public interface IElementSupplier
    {
        void InitializeSupplier(WheelState wheelState, Roll reel);
        
        int GetElementMaxHeight();
        int GetElementSequenceLength();
      
        int GetStartIndex();

        int GetStopIndex();
         
        int OnStopAtIndex(int stopIndex);

        int ComputeStopIndex(int currentIndex, int slotDownStepCount);

        SequenceElement GetElement(int index, bool useSeqElement=false);

        SequenceElement GetResultElement(int index);

        bool CheckResultAtIndex(int index, SequenceElement sequenceElement);

        bool IsRollLocked(int index);

        void OnForceUpdateReel();
    }
}