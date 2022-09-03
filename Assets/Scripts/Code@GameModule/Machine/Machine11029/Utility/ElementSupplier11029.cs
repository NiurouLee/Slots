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
    public class ElementSupplier11029 : ElementSupplier
    {
        public override bool CheckResultAtIndex(int index, SequenceElement sequenceElement)
        {
            resultSequence = wheelState.GetSpinResultSequenceElement(roll);
            bool idEqual = resultSequence[index].config.id == sequenceElement.config.id;
            if (sequenceElement.machineContext.state.Get<ExtraState11029>().GetIsDrag())
            {
                if (sequenceElement.config.id == Constant11029.StarElementId)
                {
                    idEqual = true;
                }
            }

            if (resultSequence != null && index < resultSequence.Count && idEqual)
            {
                return true;
            }

            return false;
        }
    }
}