// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date :2021-07-05 10:19 PM
// Ver : 1.0.0
// Description : Roll.cs
// ChangeLog :
// **********************************************

using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameModule
{
    public class Roll : TransformHolder
    {
        #region RollInfo Need By Other Class

        public int rowCount;
        public int rollIndex;
        public int containerCount;
        public float stepSize;
        public int stopIndex;
        public Vector3 initialPosition;

        public IElementSupplier elementSupplier;

        public int lastVisibleRowIndex;
        public int firstVisibleRowIndex;

        #endregion

        protected ElementContainer[] containers;
        protected int shiftRowArrowIndex = 0;

        protected Vector2 contentSize;
        protected int elementMaxHeight;
        protected int extraTopElementCount;
        protected float[] containerInitPos;
        protected int[] containerBaseSortOrder;
 
        protected bool topRowHasHighSortOrder = false;
        protected bool leftColHasHighSortOrder = false;

        protected string elementSortLayerName;
        
        public Roll(Transform inTransform, bool inTopRowHasHighSortOrder, bool inLeftColHasHighSortOrder, string inElementSortLayerName)
            : base(inTransform)
        {
            elementSortLayerName = inElementSortLayerName;
            topRowHasHighSortOrder = inTopRowHasHighSortOrder;
            leftColHasHighSortOrder = inLeftColHasHighSortOrder;
        }

        public virtual void BuildRoll(int inRollIndex,int inRowCount,Vector2 inContentSize,IElementSupplier inElementSupplier,
            RollBuildingExtraConfig buildingExtraConfig)
        {
            rollIndex = inRollIndex;
            
            stopIndex = rollIndex;
            
            rowCount = inRowCount;
            elementSupplier = inElementSupplier;
            contentSize = inContentSize;
            firstVisibleRowIndex = 0;
            lastVisibleRowIndex = rowCount;
            elementMaxHeight = buildingExtraConfig.elementMaxHeight;
            extraTopElementCount = buildingExtraConfig.extraTopElementCount;
            initialPosition = transform.localPosition;

            ComputeContainerViewCountAndElementStepSize();
            PreComputeContainerBaseSortOrder();
            PreComputeContainerInitializePosition();
            BuildElementContainer();
        }
        
        public Vector2 GetContentSize()
        {
            return contentSize;
        }

        public virtual void ChangeExtraTopElementCount(int inExtraTopElementCount)
        {
            extraTopElementCount = inExtraTopElementCount;
        }

        public int GetExtraTopElementCount()
        {
            return extraTopElementCount;
        }
        
        public virtual void ChangeRowCount(int inRowCount)
        {
            rowCount = inRowCount;
        }

        public virtual ElementContainer GetVisibleContainer(int index)
        {
            int offsetIndex = shiftRowArrowIndex + 1 + extraTopElementCount + index;
            return containers[offsetIndex % containerCount];
        }

        public virtual Vector3 GetVisibleContainerPosition(int index)
        {
            var container = containers[(shiftRowArrowIndex + 1 + extraTopElementCount + index) % containerCount];
            var  shiftPosition = initialPosition - transform.localPosition;
            var localPosition = container.transform.localPosition + shiftPosition;

            return transform.TransformPoint(localPosition);
        }

        public virtual ElementContainer GetContainer(int index)
        {
            return containers[(shiftRowArrowIndex + index + extraTopElementCount + containerCount) % containerCount];
        }

        protected virtual void ComputeContainerViewCountAndElementStepSize()
        {
            containerCount = rowCount + elementMaxHeight + extraTopElementCount;
            stepSize = contentSize.y / rowCount;
        }

        //存储图标初始位置，避免滚动过程中重复计算
        public virtual void PreComputeContainerInitializePosition()
        {
            //多计算一个的目的为了能够实现由下到上回退功能
            containerInitPos = new float[containerCount + 1];

            for (var i = 0; i <= containerCount; i++)
            {
                containerInitPos[i] = contentSize.y * 0.5f - (i - 0.5f - extraTopElementCount) * stepSize;
            }
        }

        public Vector3 GetVisibleContainerLocalPosition(int index)
        {
            return new Vector3(0, containerInitPos[index+1+extraTopElementCount], 0);
        }

        public virtual void PreComputeContainerBaseSortOrder()
        {
            containerBaseSortOrder = new int[containerCount + 1];
            
            for (var i = 0; i <= containerCount; i++)
            {
                containerBaseSortOrder[i] = GetBaseSortingOrderIndex() * containerCount + (topRowHasHighSortOrder ?  containerCount - i: i);
            }
        }

        protected virtual int GetBaseSortingOrderIndex()
        {
            return leftColHasHighSortOrder ? 4 - rollIndex : rollIndex;
        }

        public virtual void BuildElementContainer()
        {
            containers = new ElementContainer[containerCount];

            for (var i = 0; i < containerCount; i++)
            {
                GameObject cv = new GameObject("ContainerView");

                cv.SetActive(true);
                var containerView = new ElementContainer(cv.transform, elementSortLayerName);
                containerView.Initialize(context);
                cv.transform.SetParent(transform, false);
                cv.transform.localPosition = new Vector3(0, containerInitPos[i], 0);
                containers[i] = containerView;
            }

            shiftRowArrowIndex = 0;
        }

        public virtual void DoShift(float shiftAmount)
        {
            transform.localPosition =
                new Vector3(initialPosition.x, initialPosition.y - shiftAmount, initialPosition.z);
        }

        public virtual void ShiftOneRow(int currentIndex, bool useSeqElement=false)
        {
            shiftRowArrowIndex--;
            
            if (shiftRowArrowIndex < 0)
                shiftRowArrowIndex = containerCount - 1;

            var nextContainerView = containers[shiftRowArrowIndex];
            
            var sequenceElement = elementSupplier.GetElement(currentIndex, useSeqElement);
            nextContainerView.UpdateElement(sequenceElement);

            for (var i = 0; i < containerCount; i++)
            {
                containers[(shiftRowArrowIndex + i) % containerCount].transform.localPosition =
                    new Vector3(0, containerInitPos[i], 0);
                
                containers[(shiftRowArrowIndex + i) % containerCount].UpdateBaseSortingOrder(containerBaseSortOrder[i]);
            }
        }
  
        public virtual void ShiftOneRowUp(int currentIndex, bool useSeqElement = false)
        {
            var nextContainerView = containers[shiftRowArrowIndex];
            var stopReelIndex = currentIndex;
            var maxIndex = elementSupplier.GetElementSequenceLength();
            var sequenceElement = elementSupplier.GetElement((stopReelIndex + containerCount - extraTopElementCount - 1)% maxIndex, useSeqElement);
            nextContainerView.UpdateElement(sequenceElement);
            for (var i = 1; i <= containerCount; i++)
            {
                containers[(shiftRowArrowIndex + i) % containerCount].transform.localPosition =
                    new Vector3(0, containerInitPos[i], 0);
                
                containers[(shiftRowArrowIndex + i) % containerCount].UpdateBaseSortingOrder(containerBaseSortOrder[i]);
            }
            
            shiftRowArrowIndex++;
            if (shiftRowArrowIndex >= containerCount)
                shiftRowArrowIndex = 0;
        }


        public virtual void RemoveElements()
        {
            for (var i = 0; i < containerCount; i++)
            {
                containers[i].RemoveElement();
            }
        }

        public virtual void UpdateVisibleElement(List<SequenceElement> sequenceElements)
        {
            for (var i = 0; i < sequenceElements.Count && i < rowCount; i++)
            {
                GetVisibleContainer(i).UpdateElement(sequenceElements[i]);
            }
        }

        public virtual List<SequenceElement> GetVisibleSequenceElement()
        {
            var sequenceList = new List<SequenceElement>();
            for (int i = 0; i < rowCount; i++)
            {
                sequenceList.Add(GetVisibleContainer(i).sequenceElement);
            }
            
            return sequenceList;
        }

        public virtual void ForceUpdateAllElement(int currentSequenceIndex, bool useSeqElement=false)
        {
            var maxIndex = elementSupplier.GetElementSequenceLength();
         
            var index = (currentSequenceIndex  + maxIndex) % maxIndex;

            for (var i = 0; i < containerCount; i++)
            {
                if (index >= maxIndex)
                {
                    index = 0;
                }

                containers[(shiftRowArrowIndex + i) % containerCount]
                    .UpdateElement(elementSupplier.GetElement(index,useSeqElement));
               
                index++;
                
                containers[(shiftRowArrowIndex+ i) % containerCount].UpdateBaseSortingOrder(containerBaseSortOrder[i]);
            }
        }

        public virtual void ForceUpdateAllElementPosition()
        {
            for (var i = 0; i < containerCount; i++)
            {
                containers[(shiftRowArrowIndex + i) % containerCount].transform.localPosition =
                    new Vector3(0, containerInitPos[i], 0);
            }
        }
        public virtual void ForceUpdateAllElementSortingOrder(int currentSequenceIndex)
        {
            for (var i = 0; i < containerCount; i++)
            {
                containers[(shiftRowArrowIndex+ i) % containerCount].UpdateBaseSortingOrder(containerBaseSortOrder[i]);
            }
        }

        public void ResetDoneTag()
        {
            for (int i = 0; i < containerCount; i++)
            {
                containers[i].doneTag = false;
            }
        }

        public int GetReelStartIndex()
        {
            return elementSupplier.GetStartIndex();
        }

        public int ComputeReelStopIndex(int currentIndex, int slowDownStepCount)
        {
            return elementSupplier.ComputeStopIndex(currentIndex, slowDownStepCount);
        }
        
        public int OnReelStopAtIndex(int currentIndex)
        {
            return elementSupplier.OnStopAtIndex(currentIndex);
        }

        public virtual int  GetSpinningDurationMultiplier(int wheelStartIndex, int updaterIndex, int updaterRollStopIndex)
        {
            return updaterIndex;
        }
        
        public virtual int  GetSpinningDurationMultiplier(int wheelStartIndex, int preUpdaterIndex, int preRollIndex, int preUpdaterStopIndex, int updaterIndex, int updaterRollStopIndex)
        {
            return updaterIndex - preUpdaterIndex - 1;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stopByColumn"></param>
        public virtual void SetSpinningStopByEachVisibleColumn(bool stopByColumn)
        {
            
        }
        
        /// <summary>
        /// 更新Roll上的图标到服务器发送的结果
        /// </summary>
        public virtual void UpdateElementToServerResult()
        {
            for (var i = 0; i < containerCount; i++)
            {
                containers[(shiftRowArrowIndex + i) % containerCount]
                    .UpdateElement(elementSupplier.GetResultElement(i));
            }
        }
        public virtual void ValidateSpinResult()
        {
            for (var i = 0; i < rowCount; i++)
            {
                if (!elementSupplier.CheckResultAtIndex(i + 1 + extraTopElementCount, GetVisibleContainer(i).sequenceElement))
                {
                    Debug.LogError($"SpinResultError:{rollIndex},{i}");
                }
            }
        }
        
        public virtual ElementContainer GetWinLineContainer(int index)
        {
            int offsetIndex = shiftRowArrowIndex + 1 + extraTopElementCount + index;
            var container = containers[offsetIndex % containerCount];
            
            if (elementSupplier.GetElementMaxHeight() > 1 && !container.sequenceElement.config.createBigElementParts)
            {
                offsetIndex  += (int)container.sequenceElement.config.position;
            }
            
            return containers[offsetIndex % containerCount];
        }
        
        public virtual ElementContainer GetWinFrameContainer(int index)
        {
            int offsetIndex = shiftRowArrowIndex + 1 + extraTopElementCount + index;
            var container = containers[offsetIndex % containerCount];
            
            // if (elementSupplier.GetElementMaxHeight() > 1 && !container.sequenceElement.config.createBigElementParts)
            // {
            //     offsetIndex  += (int)container.sequenceElement.config.position;
            // }
            
            return containers[offsetIndex % containerCount];
        }
    }
}