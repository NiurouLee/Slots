//  **********************************************
//  Copyright(c) 2021 by com.ustar
//  All right reserved
// 
//  Author : Besure.Chen
//  Date :2021-09-02 11:03
//  Ver : 1.0.0
//  Description : WinLineAnimationController11010.cs
//  ChangeLog :
//  **********************************************

using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering;

namespace GameModule
{
    public class WinLineAnimationController11010: WinLineAnimationController
    {
        public override void PlayElementWinAnimation(uint rollIndex, uint rowIndex, bool showWinFrame=true)
        {
            base.PlayElementWinAnimation(rollIndex, rowIndex, showWinFrame);
            var elementContainer = wheel.GetWinLineElementContainer((int)rollIndex, (int)rowIndex);
            if (Constant11010.IsWildElementId(elementContainer.sequenceElement.config.id))
            {
                ChangeElementSortingOrder(elementContainer, "Element",false, (int)rollIndex*300);
                return;
            }
            int middleRollIndex = Mathf.FloorToInt(wheel.rollCount * 0.5f);
            int middleRowIndex = Mathf.FloorToInt(wheel.GetRoll((int)rollIndex).rowCount * 0.5f);
            var element = elementContainer.GetElement();
            var offsetX = (rollIndex-middleRollIndex)*GetWinElementPositionOffset().x;
            var offsetY = (middleRowIndex-rowIndex) * GetWinElementPositionOffset().y;
            for (int i = 0; i < 2; i++)
            {
                element?.transform.DOScale(GetWinElementMaxScale(), 0.75f).SetDelay(i*1.5f);
                element?.transform.DOLocalMove(new Vector3(offsetX,offsetY,0), 0.75f).SetDelay(i*1.5f);
                element?.transform.DOScale(Vector3.one, 0.75f).SetDelay(i * 1.5f + 0.75f);
                element?.transform.DOLocalMove(Vector3.zero, 0.75f).SetDelay(i * 1.5f + 0.75f);
            }
        }

        protected virtual Vector3 GetWinElementMaxScale()
        {
            return new Vector3(1.15f, 1.15f, 1f);
        }

        protected virtual Vector2 GetWinElementPositionOffset()
        {
            return new Vector2(0.2f, 0.2f);
        }

        public override void StopAllElementAnimation(bool force = false)
        {
            for (var rollIndex = 0; rollIndex < wheel.rollCount; rollIndex++)
            {
                var roll = wheel.GetRoll(rollIndex);
                var rollRowCount = roll.containerCount;

                for (var row = 0; row < rollRowCount; row++)
                {
                    var container = roll.GetContainer(row);
                    if (Constant11010.IsWildElementId(container.sequenceElement.config.id))
                    {
                        ChangeElementSortingOrder(container, "Default",true, -rollIndex*300);
                    }
                }
            }
            base.StopAllElementAnimation(force);
        }

        public override void StopElementWinAnimation(uint rollIndex, uint rowIndex)
        {
            var container = wheel.GetWinLineElementContainer((int)rollIndex, (int)rowIndex);

            var element = container.GetElement();
            if (element!=null && !element.HasAnimState("Win"))
            {
                return;
            }
            
            if (!container.doneTag)
            {
                if (Constant11010.IsWildElementId(container.sequenceElement.config.id))
                {
                    ChangeElementSortingOrder(container, "Default",true, -(int)rollIndex*300);
                }
                //  symbolContainerView.ToggleAnimationShareInstance(false);
                container.UpdateAnimationToStatic();
                container.ShiftSortOrder(false);
                //   panelView.ReelViews[col].UpdateContainerWinSortingOrder(symbolContainerView, false);
                container.doneTag = true;
            }
        }

        private void ChangeElementSortingOrder(ElementContainer elementContainer, string sortingOrder, bool enable, int shiftOrder)
        {
            elementContainer.transform.GetComponent<SortingGroup>().enabled = enable;
            var spriteRenderers = elementContainer.transform.GetComponentsInChildren<SpriteRenderer>(true);
            for (int i = 0; i < spriteRenderers.Length; i++)
            {
                var spriteRender = spriteRenderers[i];
                spriteRender.sortingLayerID = SortingLayer.NameToID(sortingOrder);
                if (spriteRender.name.Contains("SpriteBg"))
                    continue;
                spriteRender.sortingOrder = spriteRender.sortingOrder + shiftOrder;
            }
            var particleSystems = elementContainer.transform.GetComponentsInChildren<ParticleSystem>(true);
            for (int i = 0; i < particleSystems.Length; i++)
            {
                var render = particleSystems[i].GetComponent<Renderer>();
                render.sortingLayerID = SortingLayer.NameToID(sortingOrder);
                render.sortingOrder = render.sortingOrder + shiftOrder;
            }
        }
    }
}