using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace GameModule{
    public class Roll11301 : Roll
    {
        public Roll11301(Transform inTransform, bool inTopRowHasHighSortOrder, bool inLeftColHasHighSortOrder, string inElementSortLayerName) : base(inTransform, inTopRowHasHighSortOrder, inLeftColHasHighSortOrder, inElementSortLayerName)
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
                var seqElement = elementSupplier.GetElement(index,useSeqElement);
                
                var listDoorIsContains = Constant11301.AllListDoorElementIds.Contains(seqElement.config.id);
                var freeSpin = seqElement.machineContext.state.Get<FreeSpinState>();
                if( listDoorIsContains && freeSpin.IsTriggerFreeSpin){
                    XDebug.Log("替换前："+seqElement.config.id);
                    var random = Mathf.FloorToInt(UnityEngine.Random.Range(1,9));
                    var elementConfigSet = seqElement.machineContext.state.machineConfig.GetElementConfigSet();
                    seqElement = new SequenceElement(elementConfigSet.GetElementConfig((uint)random), seqElement.machineContext);
                    XDebug.Log("替换后："+seqElement.config.id);
                }
                
                containers[(shiftRowArrowIndex + i) % containerCount]
                    .UpdateElement(seqElement);
               
                index++;
                
                containers[(shiftRowArrowIndex+ i) % containerCount].UpdateBaseSortingOrder(containerBaseSortOrder[i]);
            }
        }
    }
}
