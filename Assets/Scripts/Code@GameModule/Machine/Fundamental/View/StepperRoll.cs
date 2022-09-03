// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Besure.Chen
// Date :2021-08-11 17:19
// Ver : 1.0.0
// Description : StepperRoll.cs
// ChangeLog :
// **********************************************

using UnityEngine;

namespace GameModule
{
    /// 含有Empty Symbol的Roll
    public class StepperRoll: Roll
    {
        public StepperRoll(Transform inTransform, bool inTopRowHasHighSortOrder, bool inLeftColHasHighSortOrder, string inElementSortLayerName)
            : base(inTransform, inTopRowHasHighSortOrder, inLeftColHasHighSortOrder,inElementSortLayerName)
        {

        }
        protected override void ComputeContainerViewCountAndElementStepSize()
        {
            containerCount = rowCount*2 + elementMaxHeight - 1;
            stepSize = contentSize.y * GetStepSizeScale() / (rowCount+1);
        }
        public override void PreComputeContainerInitializePosition()
        {
            containerInitPos = new float[containerCount];

            for (var i = 0; i < containerCount; i++)
            {
                containerInitPos[i] = contentSize.y * 0.5f * GetStepSizeScale() - (i-1) * stepSize;
            }
        }

        //设置图标之间距离倍率
        protected virtual float GetStepSizeScale()
        {
            return 1f;
        }
    }
}