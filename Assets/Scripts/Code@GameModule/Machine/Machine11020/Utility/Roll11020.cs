

using UnityEngine;

namespace GameModule
{
    public class Roll11020 : Roll
    {
        protected SpriteMask rollMask;

        protected readonly int rollSortIndexInterval = 200;

        private bool isLessRoll = false;

        public Roll11020(Transform inTransform, bool inTopRowHasHighSortOrder, bool isLeftColHasHighSortOrder, string inElementSortLayerName)
            : base(inTransform, inTopRowHasHighSortOrder, isLeftColHasHighSortOrder, inElementSortLayerName)
        {
            
        }
        

        public override void BuildRoll(int inRollIndex, int inRowCount, Vector2 inContentSize,
            IElementSupplier inElementSupplier, RollBuildingExtraConfig extraConfig)
        {

            isLessRoll = (inRollIndex == 0 || inRollIndex == 2 || inRollIndex == 4);

            var rollMaskGameObject = transform.Find("RollMask").gameObject;

            if (isLessRoll)
            {
                var maskPos = rollMaskGameObject.transform.localPosition;
                
                var p = transform.localPosition;
                p.y -= maskPos.y;

                transform.localPosition = p;
            }
            else
            {
                GameObject.Destroy(rollMaskGameObject);
                rollMaskGameObject = null;
            }

            base.BuildRoll(inRollIndex, inRowCount, inContentSize, inElementSupplier, extraConfig);

            if (rollMaskGameObject != null)
            {
                rollMask = rollMaskGameObject.GetComponent<SpriteMask>();
              
                rollMask.frontSortingLayerID = SortingLayer.NameToID(elementSortLayerName);
                rollMask.frontSortingOrder = (rollIndex + 1) * rollSortIndexInterval + GetContainerBaseSortOrderOffset();

                rollMask.backSortingLayerID = SortingLayer.NameToID(elementSortLayerName);
                rollMask.backSortingOrder = (rollIndex) * rollSortIndexInterval + GetContainerBaseSortOrderOffset();

                rollMaskGameObject.transform.SetParent(transform.parent);
            }

            var blackMask = transform.parent.Find("BlackMask" + inRollIndex).gameObject;
            if (blackMask != null)
            {
                blackMask.GetComponent<SpriteRenderer>().sortingOrder  = GetContainerBaseSortOrderOffset() + (inRollIndex +1) * rollSortIndexInterval - 150;
                blackMask.SetActive(false);
            }
        }

        public override void PreComputeContainerBaseSortOrder()
        {
            containerBaseSortOrder = new int[containerCount + 1];

            for (var i = 0; i <= containerCount; i++)
            {
                containerBaseSortOrder[i] = rollIndex * rollSortIndexInterval + (topRowHasHighSortOrder ? containerCount - i : i) + 1 + GetContainerBaseSortOrderOffset();
            }
        }

        protected virtual int GetContainerBaseSortOrderOffset()
        {
            // return 200;
            // return isLessRoll ? 1000 : 200;
            return isLessRoll ? 200 : 1000;
        }

        public float GetContainerInitPos(int id)
        {
            return containerInitPos[id+1];
        }
    }
}
