using System;
using System.Collections.Generic;
using UnityEngine.Rendering;

namespace GameModule
{
    public class IndependentWheelAnimationController11022:IndependentWheelAnimationController
    {
        private LinkLogicProxy11022 linkProxy;
        protected override void OnBlinkAnimationFinished(ElementContainer container, string appearKey,int rollIndex,int rowIndex)
        {
            if (container.sequenceElement.config.id == 13)
            {
                linkProxy.ChangeToPerformLayer(rollIndex);
            }
            else
            {
                throw new Exception("link内非箱子触发blink");
            }
            //抽出linkproxy的表演层
            base.OnBlinkAnimationFinished(container,appearKey,rollIndex,rowIndex);
        }

        public void BindLinkProxy(LinkLogicProxy11022 bindingProxy)
        {
            linkProxy = bindingProxy;
        }
        
        public override bool ShowBlinkAnimation(List<int> blinkInfo)
        {
            bool blinkAnimationPlayed = false;

            if (blinkInfo != null && blinkInfo.Count > 0)
            {
                // int blinkIndex = GetNeedPlayBlinkSoundRowIndex(blinkInfo);

                foreach (var item in blinkInfo)
                {
                    if (wheel.GetRoll(item).transform.gameObject.activeSelf)
                    {
                        // XDebug.Log($"ShowAppearAnimation:{item}:{0}");
                        var container = wheel.GetRoll(item).GetVisibleContainer(0);
                        container.ShiftSortOrder(true);

                        if (container.sequenceElement.config.blinkType != BlinkAnimationPlayStyleType.Default)
                        {
                            containerPlayBlinkAnimation.Add(container);
                        }

                        var appearKey = wheel.wheelName + item + item;
                        listWheelAppears.Add(appearKey);

                        if (!blinkAnimationPlayed/* && item == blinkIndex*/)
                        {
                            PlayBlinkSound(container, item, 0);   
                        }
                        
                        var sortingGroup = container.transform.GetComponent<SortingGroup>();
                        var tempSortingLayerOrder = sortingGroup.sortingOrder;
                        var tempSortingLayerName = sortingGroup.sortingLayerName;
                        sortingGroup.sortingLayerName = "SoloElement";
                        sortingGroup.sortingOrder = 1000;
                        container.PlayElementAnimation("Blink", false, () =>
                        {
                            sortingGroup.sortingLayerName = tempSortingLayerName;
                            sortingGroup.sortingOrder = tempSortingLayerOrder;
                            OnBlinkAnimationFinished(container, appearKey,item,(int)0);
                        });   

                        blinkAnimationPlayed = true;    
                    }
                    else
                    {
                        // XDebug.Log($"HideElementNotShowAppearAnimation:{item}:{0}");
                    }
                    
                }
            }

            return blinkAnimationPlayed;
        }
    }
}