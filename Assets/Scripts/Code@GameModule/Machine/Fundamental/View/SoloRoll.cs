// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date :2021-07-05 9:01 PM
// Ver : 1.0.0
// Description : SoloRoll.cs
// ChangeLog :
// **********************************************
using UnityEngine;
namespace GameModule
{
    public class SoloRoll: Roll
    {
        protected SpriteMask soloRollMask;

        protected int wheelVisibleRowCount;

        protected readonly int soloRollSortIndexInterval = 100;
        private int _frontOrder = 0;
        private int _backOrder = 0;
        
        public SoloRoll(Transform inTransform, bool inTopRowHasHighSortOrder,  bool inLeftColHasHighSortOrder, string inElementSortLayerName)
            : base(inTransform, inTopRowHasHighSortOrder, inLeftColHasHighSortOrder,inElementSortLayerName)
        {
            
        }
 
        public override void BuildRoll(int rollId, int inRowCount, Vector2 inContentSize,
            IElementSupplier inElementSupplier, RollBuildingExtraConfig extraConfig)
        {
            wheelVisibleRowCount = inRowCount;
            extraConfig.elementMaxHeight = 1;
            extraConfig.extraTopElementCount = 0;
            base.BuildRoll(rollId, 1, inContentSize, inElementSupplier, extraConfig);
            InitializeRollMaskAndSortOrder();
        }

        public override void PreComputeContainerBaseSortOrder()
        {
            containerBaseSortOrder = new int[containerCount + 1];

            for (var i = 0; i <= containerCount; i++)
            {
                containerBaseSortOrder[i] = rollIndex * soloRollSortIndexInterval + (topRowHasHighSortOrder ? containerCount - i : i) + 1 + GetContainerBaseSortOrderOffset();
            }
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
        protected override void ComputeContainerViewCountAndElementStepSize()
        {
            elementMaxHeight = 1;
            containerCount = rowCount + elementMaxHeight;
            stepSize = contentSize.y / rowCount;
        }
        
        public override void ShiftOneRow(int currentIndex, bool useSeqElement=false)
        {
            var nextContainerView = containers[1];
            var fistContainerView = containers[0];

            SequenceElement element = elementSupplier.GetElement(currentIndex, useSeqElement);
            
            nextContainerView.UpdateElement(element);
          
            nextContainerView.transform.localPosition = new Vector3(0, containerInitPos[0], 0);
            fistContainerView.transform.localPosition = new Vector3(0, containerInitPos[1], 0);
            
            containers[0] = nextContainerView;
            containers[1] = fistContainerView;
           
            containers[0].UpdateBaseSortingOrder(containerBaseSortOrder[0]);
            containers[1].UpdateBaseSortingOrder(containerBaseSortOrder[1]);
        }
        
        public override void ShiftOneRowUp(int currentIndex, bool useSeqElement=false)
        {
            var nextContainerView = containers[0];
            var fistContainerView = containers[1];
            
            var maxIndex = elementSupplier.GetElementSequenceLength();
            var sequenceElement = elementSupplier.GetElement((currentIndex + 1) % maxIndex, useSeqElement);
            nextContainerView.UpdateElement(sequenceElement);
            
            nextContainerView.transform.localPosition = new Vector3(0, containerInitPos[2], 0);
            fistContainerView.transform.localPosition = new Vector3(0, containerInitPos[1], 0);
            
            containers[0] = fistContainerView;
            containers[1] = nextContainerView;
           
            containers[0].UpdateBaseSortingOrder(containerBaseSortOrder[1]);
            containers[1].UpdateBaseSortingOrder(containerBaseSortOrder[2]);
        }


        public void EnableSoloRollMask(bool enable)
        {
            if (soloRollMask)
                soloRollMask.gameObject.SetActive(enable);
        }

        public override void RemoveElements()
        {
            containers[0].RemoveElement();
            containers[1].RemoveElement();
        }

        public override int GetSpinningDurationMultiplier(int wheelStartIndex, int updaterIndex, int updaterStopIndex)
        {
            return updaterStopIndex;
            
            
            int lockColumnCount = 0;

            var preColumnCount = rollIndex / wheelVisibleRowCount;

            for (var i = 0; i < preColumnCount; i++)
            {
                bool columnLocked = true;

                for (var j = 0; j < wheelVisibleRowCount; j++)
                {
                    if (!elementSupplier.IsRollLocked(i * wheelVisibleRowCount + j))
                    {
                        columnLocked = false;
                    }
                }
                
                if (columnLocked)
                    lockColumnCount++;
            }
            
            return rollIndex / wheelVisibleRowCount - lockColumnCount;
        }

        public override int GetSpinningDurationMultiplier(int wheelStartIndex, int preUpdaterIndex, int preRollIndex, int preUpdaterStopIndex, int updaterIndex, int updaterStopIndex)
        {
            return updaterStopIndex - preUpdaterStopIndex;
            
            // int lockColumnCount = 0;
            //
            // var preColumnIndex = preRollIndex / wheelVisibleRowCount;
            //
            // if (preRollIndex < 0)
            // {
            //     preColumnIndex = 0;
            // }
            //
            // var preColumnCount = rollIndex / wheelVisibleRowCount;
            //
            // for (var i = preColumnIndex; i < preColumnCount; i++)
            // {
            //     bool columnLocked = true;
            //
            //     for (var j = 0; j < wheelVisibleRowCount; j++)
            //     {
            //         if (!elementSupplier.IsRollLocked(i * wheelVisibleRowCount + j))
            //         {
            //             columnLocked = false;
            //         }
            //     }
            //     
            //     if (columnLocked)
            //         lockColumnCount++;
            // }
            // return preColumnCount - preColumnIndex - lockColumnCount;
        }
        
        public void ShiftRollMaskAndSortOrder(int shiftOrder)
        {
            if (soloRollMask)
            {
                _frontOrder += shiftOrder;
                _backOrder += shiftOrder;
                soloRollMask.frontSortingOrder = _frontOrder;
                soloRollMask.backSortingOrder = _backOrder;

                   
                soloRollMask.transform.SetParent(transform.parent);

                containerBaseSortOrder[0] = containerBaseSortOrder[0] + shiftOrder;
                containerBaseSortOrder[1] = containerBaseSortOrder[1] + shiftOrder;
                containers[0].UpdateBaseSortingOrder(containerBaseSortOrder[0]);
                containers[1].UpdateBaseSortingOrder(containerBaseSortOrder[1]);
            }
        }

        public void SetRollMaskScale(float scaleX, float scaleY=1f)
        {
            if (soloRollMask)
            {
                var scale = soloRollMask.transform.localScale;
                soloRollMask.transform.localScale = new Vector3(scaleX*scale.x, scaleY*scale.y,1);
            }
        }
    }
}