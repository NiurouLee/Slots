using UnityEngine;

namespace GameModule
{
    public class SoloRoll11022:Roll
    {
        public SoloRoll11022(Transform inTransform, bool inTopRowHasHighSortOrder,  bool inLeftColHasHighSortOrder, string inElementSortLayerName)
            : base(inTransform, inTopRowHasHighSortOrder, inLeftColHasHighSortOrder,inElementSortLayerName)
        {
            
        }
        protected int wheelVisibleRowCount;
        protected SpriteMask soloRollMask;
        protected readonly int soloRollSortIndexInterval = 100;
        private int _frontOrder = 0;
        private int _backOrder = 0;
        public override void BuildRoll(int rollId, int inRowCount, Vector2 inContentSize,
            IElementSupplier inElementSupplier, RollBuildingExtraConfig extraConfig)
        {
            wheelVisibleRowCount = inRowCount;
            base.BuildRoll(rollId, 1, inContentSize, inElementSupplier, extraConfig);
            InitializeRollMaskAndSortOrder();
        }
        
        protected void InitializeRollMaskAndSortOrder()
        {
            var soloRollMaskGameObject = transform.Find("RollMask");
        
            if (soloRollMaskGameObject != null)
            {
                soloRollMask = soloRollMaskGameObject.GetComponent<SpriteMask>();
        
                //var spriteSize = soloRollMask.sprite.bounds.size;
               
                // soloRollMask.transform.localScale = new Vector3(contentSize.x / spriteSize.x,
                //     contentSize.y / spriteSize.y, spriteSize.z);
              
                if (soloRollMask)
                {
                    soloRollMask.frontSortingLayerID = SortingLayer.NameToID(elementSortLayerName);
                    soloRollMask.frontSortingOrder = (rollIndex + 1) * soloRollSortIndexInterval + GetContainerBaseSortOrderOffset();
        
                    soloRollMask.backSortingLayerID = SortingLayer.NameToID(elementSortLayerName);
                    soloRollMask.backSortingOrder = (rollIndex) * soloRollSortIndexInterval + GetContainerBaseSortOrderOffset();
                   
                    soloRollMask.transform.SetParent(transform.parent);
                   
                    _frontOrder = soloRollMask.frontSortingOrder;
                    _backOrder = soloRollMask.backSortingOrder;
                }
            }
        }
        protected virtual int GetContainerBaseSortOrderOffset()
        {
            return 0;
        }
        public override void PreComputeContainerBaseSortOrder()
        {
            containerBaseSortOrder = new int[containerCount + 1];
        
            for (var i = 0; i <= containerCount; i++)
            {
                containerBaseSortOrder[i] = rollIndex * soloRollSortIndexInterval + (topRowHasHighSortOrder ? containerCount - i : i) + 1 + GetContainerBaseSortOrderOffset();
            }
        }
        
        public override int GetSpinningDurationMultiplier(int wheelStartIndex, int updaterIndex, int updaterStopIndex)
        {
            return updaterStopIndex;
        }
        
        public override int GetSpinningDurationMultiplier(int wheelStartIndex, int preUpdaterIndex, int preRollIndex, int preUpdaterStopIndex, int updaterIndex, int updaterStopIndex)
        {
            return updaterStopIndex - preUpdaterStopIndex;
        }
    }
}