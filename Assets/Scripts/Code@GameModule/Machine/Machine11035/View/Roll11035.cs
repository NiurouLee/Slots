using System.Collections.Generic;
using UnityEngine;

namespace GameModule
{
    public class Roll11035 : Roll
    {
        public Roll11035(Transform inTransform, bool inTopRowHasHighSortOrder, bool inLeftColHasHighSortOrder, string inElementSortLayerName) : base(inTransform, inTopRowHasHighSortOrder, inLeftColHasHighSortOrder, inElementSortLayerName)
        {
        }

        public override void PreComputeContainerInitializePosition()
        {
            //多计算一个的目的为了能够实现由下到上回退功能
            containerInitPos = new float[containerCount + 1];

            for (var i = 0; i <= containerCount; i++)
            {
                containerInitPos[i] = contentSize.y * 0.5f - (i - 0.5f) * stepSize;
                containerInitPos[i] *= 1.2f;
            }
        }
    }
}