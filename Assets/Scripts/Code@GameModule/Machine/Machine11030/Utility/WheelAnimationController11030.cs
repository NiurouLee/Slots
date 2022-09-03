using System.Collections.Generic;
using Google.ilruntime.Protobuf.Collections;
using Spine;
using UnityEngine;
using UnityEngine.Rendering;

namespace GameModule
{
    public class WheelAnimationController11030:WheelAnimationController
    {
        private ExtraState11030 extraState;
        protected List<ElementContainer> containerPlayScatterLoopAnimation;
        public bool keepScatterLoopFlag = false;
        public MachineContext machineContext;
        public WheelAnimationController11030()
        {
            containerPlayScatterLoopAnimation = new List<ElementContainer>();
        }
        
        public override void BindingWheel(Wheel inWheel)
        {
            base.BindingWheel(inWheel);
            machineContext = wheel.GetContext();
        }
        public override bool ShowBlinkAnimation(RepeatedField<uint> blinkInfo, int rollIndex)
        {
            var changeReelBackColorFlag = false;
            bool blinkAnimationPlayed = false;
            ReelStopSoundState.SoundState item = GetReelStopSoundState(rollIndex);
            bool hasScatterInRoll = false;
            if (blinkInfo != null && blinkInfo.Count > 0)
            {
                int blinkIndex = GetNeedPlayBlinkSoundRowIndex(blinkInfo, rollIndex);

                for (var i = 0; i < blinkInfo.Count; i++)
                {
                    var container = wheel.GetRoll(rollIndex).GetVisibleContainer((int) blinkInfo[i]);
                    if (Constant11030.GoldTrainList.Contains(container.sequenceElement.config.id))
                    {
                        changeReelBackColorFlag = true;
                        var blinkObject = assetProvider.InstantiateGameObject("B03_Blink");
                        blinkObject.transform.SetParent(container.transform.parent,false);
                        blinkObject.transform.position = container.transform.position;
                        var tempSortingGroup = blinkObject.AddComponent<SortingGroup>();
                        tempSortingGroup.sortingLayerName = "Element";
                        tempSortingGroup.sortingOrder = 0;
                        var blinkAnimation = blinkObject.transform.GetComponent<Animator>();
                        XUtility.PlayAnimation(blinkAnimation, "B03_Blink", () =>
                        {
                            GameObject.Destroy(blinkObject);
                        });
                    }
                    if (Constant11030.StarList.Contains(container.sequenceElement.config.id))
                    {
                        changeReelBackColorFlag = true;
                        var blinkObject = assetProvider.InstantiateGameObject("B02_Blink");
                        blinkObject.transform.SetParent(container.transform.parent,false);
                        blinkObject.transform.position = container.transform.position;
                        var tempSortingGroup = blinkObject.AddComponent<SortingGroup>();
                        tempSortingGroup.sortingLayerName = "Element";
                        tempSortingGroup.sortingOrder = 0;
                        var blinkAnimation = blinkObject.transform.GetComponent<Animator>();
                        XUtility.PlayAnimation(blinkAnimation, "B02_Blink", () =>
                        {
                            GameObject.Destroy(blinkObject);
                        });
                    }

                    XDebug.Log($"ShowAppearAnimation:{rollIndex}:{blinkInfo[i]}");

                    container.ShiftSortOrder(true);

                    if (container.sequenceElement.config.blinkType != BlinkAnimationPlayStyleType.Default)
                    {
                        containerPlayBlinkAnimation.Add(container);
                    }

                    if (Constant11030.ScatterAndWildList.Contains(container.sequenceElement.config.id))
                        hasScatterInRoll = true;

                    var appearKey = wheel.wheelName + rollIndex + blinkInfo[i];
                    listWheelAppears.Add(appearKey);
                    if (extraState == null)
                    {
                        extraState = wheel.GetContext().state.Get<ExtraState11030>();
                    }
                    if (!extraState.IsInTrain() && (Constant11030.GoldTrainList.Contains(container.sequenceElement.config.id)|| Constant11030.StarList.Contains(container.sequenceElement.config.id)))
                    {
                        
                    }
                    else
                    {
                        if (!blinkAnimationPlayed && blinkInfo[i] == blinkIndex)
                        {
                            if (item != null)
                            {
                                if (item.blinkSoundOrderId < container.sequenceElement.config.blinkSoundOrderId)
                                {
                                    item.blinkSoundOrderId = container.sequenceElement.config.blinkSoundOrderId;
                                    item.SoundName = GetBlinkSoundName(container, rollIndex, (int) blinkInfo[i]);
                                }
                                item.RollStopCount++;   
                            }
                            else
                            {
                                PlayBlinkSound(container, rollIndex, (int) blinkInfo[i]);   
                            }
                        }   
                    }

                    var tempI = i;
                    container.PlayElementAnimation("Blink", false,
                        () => { OnBlinkAnimationFinished(container, appearKey,rollIndex,(int)blinkInfo[tempI]); });
                    if (wheel.GetContext().elementExtraInfoProvider.CanShowElementAnticipation(container.sequenceElement.config.id))
                    {
                        ShowElementAnticipationAnimation(rollIndex,container.sequenceElement.config.id);
                    }

                    blinkAnimationPlayed = true;
                }
            }
            else
            {
                if (item != null)
                {
                    item.RollStopCount++;
                }

            }

            if (rollIndex == 0)
            {
                if (hasScatterInRoll)
                {
                    keepScatterLoopFlag = true;   
                }
                else
                {
                    keepScatterLoopFlag = false;
                }
            }
            else
            {
                if (keepScatterLoopFlag && !hasScatterInRoll)
                {
                    if (!machineContext.state.Get<ExtraState11030>().IsInChoose() &&
                        !(machineContext.state.Get<FreeSpinState11030>().NewCount > 0))
                    {
                        for (var i = 0; i < containerPlayScatterLoopAnimation.Count; i++)
                        {
                            var elementContainer = containerPlayScatterLoopAnimation[i];
                            elementContainer.UpdateAnimationToStatic();
                            elementContainer.ShiftSortOrder(false);
                        }   
                        keepScatterLoopFlag = false;
                        containerPlayScatterLoopAnimation.Clear();
                    }
                }
            }

            if (changeReelBackColorFlag)
            {
                ((WheelTrain11030)wheel).SetPurpleReel(true);
            }

            return blinkAnimationPlayed;
        }
        protected override void OnBlinkAnimationFinished(ElementContainer elementContainer, string appearKey,int rollIndex,int rowIndex)
        {
            listWheelAppears.Remove(appearKey);
            if (elementContainer.sequenceElement.config.blinkType == BlinkAnimationPlayStyleType.Default)
            {
                elementContainer.UpdateAnimationToStatic();
                elementContainer.ShiftSortOrder(false);
            }
            else if (elementContainer.sequenceElement.config.blinkType == BlinkAnimationPlayStyleType.IdleCondition && !keepScatterLoopFlag)
            {
                elementContainer.UpdateAnimationToStatic();
                elementContainer.ShiftSortOrder(false);
            }
            else
            {
                containerPlayBlinkAnimation.Remove(elementContainer);
                containerPlayScatterLoopAnimation.Add(elementContainer);
            }

            if (containerPlayBlinkAnimation.Count == 0 && canCheckBlinkFinished)
            {
                containerPlayScatterLoopAnimation.Clear();
                CheckAndStopBlinkAnimation();
            }

            CheckAllBlinkFinished();
        }
    }
}