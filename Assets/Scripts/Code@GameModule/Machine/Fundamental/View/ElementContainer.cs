// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date :2021-07-05 10:27 PM
// Ver : 1.0.0
// Description : ElementContainer.cs
// ChangeLog :
// **********************************************

using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;

namespace GameModule
{
    public class ElementContainer : TransformHolder
    {
        protected Element currentAttachElement;
        protected SortingGroup containerGroup;

        public bool doneTag = false;

        public SequenceElement sequenceElement;

        protected int baseSortOrder = 0;
        protected int extraSortOrder = 0;

        public ElementContainer(Transform transform, string sortingLayer)
            : base(transform)
        {
            containerGroup = transform.gameObject.AddComponent<SortingGroup>();
            containerGroup.sortingLayerID = SortingLayer.NameToID(sortingLayer);
        }

        public Element GetElement()
        {
            return currentAttachElement;
        }


        public void RemoveElement()
        {
            if (!ReferenceEquals(currentAttachElement, null))
            {
                currentAttachElement.DoRecycle();
            }
            currentAttachElement = null;
        }

        public virtual void UpdateElement(SequenceElement seqElement, bool active = false)
        {
            sequenceElement = seqElement;

            if (currentAttachElement != null)
            {
                currentAttachElement.DoRecycle();
                currentAttachElement = null;
            }

#if !PRODUCTION_PACKAGE
            if (sequenceElement != null && sequenceElement.config == null)
            {
                XDebug.Log("Invalid SequenceElement");
                return;
            }
#endif
            if (seqElement != null)
            {
                if (seqElement.config.position == 0 || (seqElement.config.createBigElementParts && seqElement.config.position > 0))
                {
                    currentAttachElement =
                        active ? seqElement.config.GetActiveElement() : seqElement.config.GetStaticElement();
                    currentAttachElement.UpdateOnAttachToContainer(transform, seqElement);
                }
            }
        }

        public void UpdateExtraSortingOrder(int sortingOrder)
        {
            extraSortOrder = sortingOrder;

            UpdateBaseSortingOrder(baseSortOrder);
        }

        public virtual void UpdateBaseSortingOrder(int sortingOrder)
        {
            baseSortOrder = sortingOrder;
            containerGroup.sortingOrder = extraSortOrder + baseSortOrder + (ReferenceEquals(sequenceElement, null) ? 0 : sequenceElement.config.zOffset);
        }

        public int GetBaseSortOrder()
        {
            return baseSortOrder;
        }

        public void ShiftSortOrder(bool isHighLight)
        {
            if (sequenceElement != null)
            {
                containerGroup.sortingOrder = extraSortOrder + baseSortOrder + (isHighLight ? sequenceElement.config.activeZOffset : sequenceElement.config.zOffset);
            }
        }

        public void UpdateAnimationToStatic(bool force = true)
        {
            if (currentAttachElement != null)
            {
                if (force || !currentAttachElement.isStaticElement)
                    UpdateElement(currentAttachElement.sequenceElement);
            }
        }

        public void UpdateElementMaskInteraction(bool activeState)
        {
            if (currentAttachElement != null && sequenceElement != null)
            {
                var elementConfig = sequenceElement.config;

                if (!activeState)
                {
                    ShiftSortOrder(false);
                }

                currentAttachElement.UpdateMaskInteraction(activeState ? elementConfig.activeStateInteraction : elementConfig.defaultInteraction, false);

                if (!activeState)
                {
                    ShiftSortOrder(false);
                }
            }
        }

        public void PlayElementAnimation(string animationName, bool maskByWheelMask = false, Action finishCallback = null)
        {
            if (ReferenceEquals(currentAttachElement, null)) return;
            if (!currentAttachElement.HasAnimState(animationName))
            {
                return;
            }

            if (currentAttachElement.isStaticElement)
            {
                UpdateElement(currentAttachElement.sequenceElement, true);
            }

            currentAttachElement.PlayAnimation(animationName, maskByWheelMask, finishCallback);
        }


        public async Task PlayElementAnimationAsync(string animationName, bool maskByWheelMask = false)
        {
            TaskCompletionSource<bool> taskCompletionSource = new TaskCompletionSource<bool>();
            GetElement()?.GetContext()?.AddWaitTask(taskCompletionSource, null);

            PlayElementAnimation(animationName, maskByWheelMask, () =>
            {
                GetElement()?.GetContext()?.RemoveTask(taskCompletionSource);
                taskCompletionSource.SetResult(true);
            });

            await taskCompletionSource.Task;
        }

        public void EnableSortingGroup(bool enabled)
        {
            if (containerGroup)
            {
                containerGroup.enabled = enabled;
            }
        }


        public SortingGroup GetContainzerGroup()
        {
            return containerGroup;
        }
    }
}