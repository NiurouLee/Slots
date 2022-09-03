using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace GameModule
{
    public class IndependentWheelAnimationController11024:IndependentWheelAnimationController
    {
        private ExtraState11024 _extraState;
        public ExtraState11024 extraState
        {
            get
            {
                if (_extraState == null)
                {
                    _extraState =  wheel.GetContext().state.Get<ExtraState11024>();
                }
                return _extraState;
            }
        }
        protected override void OnBlinkAnimationFinished(ElementContainer container, string appearKey,int rollIndex,int rowIndex)
        {
            if (Constant11024.IsGoldId(container.sequenceElement.config.id))
            {
                if (((WheelLink11024) wheel).NeedRefreshSpinTimes())
                {
                    ((WheelLink11024) wheel).RefreshLeftSpinTimes();   
                }
                
                var linkWheel = ((WheelLink11024) wheel);
                var stickyElement = linkWheel.GetStickyElement(rollIndex);
                if (!stickyElement.HasContainer())
                {
                    linkWheel.GetRoll(rollIndex).transform.gameObject.SetActive(false);
                    linkWheel.wheelState.SetRollLockState(rollIndex, true);
                    stickyElement.SetStickyData(linkWheel.GetItemData(rollIndex),false);
                }
                else
                {
                    throw new Exception("StickyElement重复Blink");
                }
                //切换到粘黏状态
            }
            else
            {
                throw new Exception("link内非金币触发blink");
            }
            base.OnBlinkAnimationFinished(container,appearKey,rollIndex,rowIndex);
        }
        
        
        public override bool ShowBlinkAnimation(List<int> blinkInfo)
        {
            bool blinkAnimationPlayed = false;

            if (blinkInfo != null && blinkInfo.Count > 0)
            {
                // int blinkIndex = GetNeedPlayBlinkSoundRowIndex(blinkInfo);

                foreach (var item in blinkInfo)
                {
                    if (wheel.GetRoll(item).transform.gameObject.activeSelf || !wheel.wheelState.IsRollLocked(item))
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
                }
            }

            return blinkAnimationPlayed;
        }
        
         public override void ShowAnticipationAnimation(int rollIndex)
        {
            //  XDebug.Log("ShowDrum:" + rollView.RollIndex);

            var wheelState = wheel.wheelState;

            var anticipationAnimationGo = wheel.GetAttachedGameObject("AnticipationAnimation");

            // var wheelConfig = wheelState.GetWheelConfig();
            // var startRollIndex = rollIndex * wheelConfig.rollRowCount;
            // var endRollIndex = startRollIndex + wheelConfig.rollRowCount;
            //
            // var positionY = 0;
            // for (int i = startRollIndex; i < endRollIndex; i++)
            // {
            //     if (wheelState.HasAnticipationAnimationInRollIndex(i))
            //     {
            //         positionY = startRollIndex+1-i;
            //     }
            // }
            
            if (wheel.GetAttachedGameObject("AnticipationAnimation") == null)
            {
                // wheel.AttachGameObject(); =
                anticipationAnimationGo =
                    assetProvider.InstantiateGameObject(wheelState.GetAnticipationAnimationAssetName());

                if (anticipationAnimationGo)
                {
                    anticipationAnimationGo.transform.SetParent(wheel.transform.Find("Rolls"), false);

                    if (!anticipationAnimationGo.GetComponent<SortingGroup>())
                    {
                        var sortingGroup = anticipationAnimationGo.AddComponent<SortingGroup>();
                        sortingGroup.sortingLayerID = SortingLayer.NameToID("Element");
                        sortingGroup.sortingOrder = 400;
                    }

                    wheel.AttachGameObject("AnticipationAnimation", anticipationAnimationGo);
                }
            }

            if (anticipationAnimationGo)
            {
                // AudioUtil.Instance.StopAudioFx(wheelState.GetAnticipationSoundAssetName());
                // AudioUtil.Instance.PlayAudioFx(wheelState.GetAnticipationSoundAssetName());

                anticipationAnimationGo.SetActive(false);
                // if (!anticipationAnimationGo.activeSelf)
                // {
                    anticipationAnimationGo.SetActive(true);
                //}

                XDebug.Log("PlayAnticipationSound:" + wheel.wheelState.GetAnticipationSoundAssetName());
                
                anticipationAnimationGo.transform.localPosition = wheel.GetAnticipationAnimationPosition(rollIndex);
                AudioUtil.Instance.StopAudioFx(wheel.wheelState.GetAnticipationSoundAssetName());
                AudioUtil.Instance.PlayAudioFx(wheel.wheelState.GetAnticipationSoundAssetName());
            }
        }
    }
}