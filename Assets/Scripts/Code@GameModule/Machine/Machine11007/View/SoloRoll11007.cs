//  **********************************************
//  Copyright(c) 2021 by com.ustar
//  All right reserved
// 
//  Author : Besure.Chen
//  Date :2021-08-17 19:15
//  Ver : 1.0.0
//  Description : SoloRoll11007.cs
//  ChangeLog :
//  **********************************************

using UnityEngine;

namespace GameModule
{
    public class SoloRoll11007: SoloRoll
    {
        public SoloRoll11007(Transform inTransform, bool inTopRowHasHighSortOrder, bool inLeftColHasHighSortOrder, string inElementSortLayerName)
            : base(inTransform, inTopRowHasHighSortOrder, inLeftColHasHighSortOrder, inElementSortLayerName)
        {
            
        }
        public override void PreComputeContainerInitializePosition()
        {
            base.PreComputeContainerInitializePosition();
            containerInitPos[0] += 0.2f;
        }
        
        protected override int GetContainerBaseSortOrderOffset()
        {
            return 200;
        }

        public override void BuildRoll(int inRollIndex, int inRowCount, Vector2 inContentSize,
            IElementSupplier inElementSupplier, RollBuildingExtraConfig extraConfig)
        {
            base.BuildRoll(inRollIndex, inRowCount, inContentSize, inElementSupplier, extraConfig);
            for (int i = 0; i < containers.Length; i++)
            {
                var container = containers[i];
                container.transform.localScale = Constant11007.LinkElementScale;
            }
        }

        public override int GetSpinningDurationMultiplier(int wheelStartIndex, int updaterIndex, int updaterStopIndex)
        {
            int lockColumnCount = 0;

            for (var i = 0; i < rollIndex; i++)
            {
                if (elementSupplier.IsRollLocked(i))
                {
                    lockColumnCount++;
                }
            }

            return rollIndex - lockColumnCount;
        }
    }
}