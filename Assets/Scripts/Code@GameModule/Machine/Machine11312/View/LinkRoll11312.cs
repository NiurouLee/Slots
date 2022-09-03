using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace GameModule{
    public class LinkRoll11312 : SoloRoll
    {
        public LinkRoll11312(Transform inTransform, bool inTopRowHasHighSortOrder, bool inLeftColHasHighSortOrder, string inElementSortLayerName) : base(inTransform, inTopRowHasHighSortOrder, inLeftColHasHighSortOrder, inElementSortLayerName)
        {
        }
        public override void ForceUpdateAllElement(int currentSequenceIndex, bool useSeqElement=false)
        {
            var maxIndex = elementSupplier.GetElementSequenceLength();
            var index = currentSequenceIndex;

            for (var i = 0; i < containerCount; i++)
            {
                if (index >= maxIndex)
                {
                    index = 0;
                }

                containers[(shiftRowArrowIndex + i) % containerCount]
                    .UpdateElement(elementSupplier.GetElement(index,useSeqElement));
               
                // index++;
                
                containers[(shiftRowArrowIndex+ i) % containerCount].UpdateBaseSortingOrder(containerBaseSortOrder[i]);
            }
        }
    }
}

