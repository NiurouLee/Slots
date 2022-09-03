


using System;
using System.Collections.Generic;
using UnityEngine;
using Google.ilruntime.Protobuf.Collections;

namespace GameModule
{
    public class WheelAnimationController11020: WheelAnimationController
    {
        public override async void OnWheelStartSpinning()
        {
            base.OnWheelStartSpinning();
            
            // spin启动0.1后，慢慢变黑（1s）
            {
                await wheel.GetContext().WaitSeconds(0.1f);
                wheel.GetContext().state.Get<WheelsActiveState11020>().ShowRollsMasks(wheel);
            }
        }
        
        public override void OnRollSpinningStopped(int rollIndex, Action rollLogicEnd)
        {
            base.OnRollSpinningStopped(rollIndex, rollLogicEnd);
            
            if (rollIndex == 4)
            {
                Constant11020.hasSpined = false;
            }

            var activeState = wheel.GetContext().state.Get<WheelsActiveState11020>();
            activeState.UpdateHighElementSortingOrder(wheel, rollIndex);
        }
        
        
        public override void OnRollStartBounceBack(int rollIndex)
        {
            base.OnRollStartBounceBack((rollIndex));
            
            wheel.GetContext().state.Get<WheelsActiveState11020>().FadeOutRollMask(wheel, rollIndex);
            
            var activeState = wheel.GetContext().state.Get<WheelsActiveState11020>();
            activeState.MakeFireBallElementsAnimation("Out", rollIndex, true);
        }

        public override void ShowAnticipationAnimation(int rollIndex)
        {
            //  XDebug.Log("ShowDrum:" + rollView.RollIndex);

            var wheelState = wheel.wheelState;

            var anticipationAnimationGo = wheel.GetAttachedGameObject("AnticipationAnimation");

            if (wheel.GetAttachedGameObject("AnticipationAnimation") == null)
            {
                // wheel.AttachGameObject(); =
                anticipationAnimationGo =
                    assetProvider.InstantiateGameObject(wheelState.GetAnticipationAnimationAssetName());

                if (anticipationAnimationGo)
                {
                    anticipationAnimationGo.transform.SetParent(wheel.transform.Find("Rolls"), false);
                    wheel.AttachGameObject("AnticipationAnimation", anticipationAnimationGo);
                }
            }

            if (anticipationAnimationGo)
            {
                anticipationAnimationGo.SetActive(false);
                anticipationAnimationGo.SetActive(true);

                anticipationAnimationGo.transform.localPosition = wheel.GetAnticipationAnimationPosition(rollIndex);
                AudioUtil.Instance.StopAudioFx(wheel.wheelState.GetAnticipationSoundAssetName());
                AudioUtil.Instance.PlayAudioFx(wheel.wheelState.GetAnticipationSoundAssetName());
            }
            
            var o = wheel.GetAttachedGameObject("AnticipationAnimation");
            if (o != null)
            {
                var p = o.transform.localPosition;
                p.y   = -0.05f;
                p.x  += 0.08f;
                o.transform.localPosition = p;

                var s = o.transform.localScale;
                s.y = (rollIndex == 1 || rollIndex == 3) ? 1.24f : 1.0f;

                o.transform.localScale = s;
            }
        }

        public override bool ShowBlinkAnimation(RepeatedField<uint> blinkInfo, int rollIndex)
        {
            bool blinkAnimationPlayed = false;
            bool scatterBlinkAnimationPlayed = false;

            if (blinkInfo != null && blinkInfo.Count > 0)
            {
                for (var i = 0; i < blinkInfo.Count; i++)
                {
                    var container = wheel.GetRoll(rollIndex).GetVisibleContainer((int) blinkInfo[i]);

                    XDebug.Log($"ShowAppearAnimation:{rollIndex}:{blinkInfo[i]}");

                    container.ShiftSortOrder(true);

                    if (container.sequenceElement.config.blinkType != BlinkAnimationPlayStyleType.Default)
                    {
                        containerPlayBlinkAnimation.Add(container);
                    }

                    var appearKey = wheel.wheelName + rollIndex + blinkInfo[i];
                    listWheelAppears.Add(appearKey);

                    PlayBlinkSound(container, rollIndex, (int) blinkInfo[i]);

                    container.PlayElementAnimation("Blink", false, () =>
                    {
                        listWheelAppears.Remove(appearKey);
                        if (container.sequenceElement.config.blinkType == BlinkAnimationPlayStyleType.Default)
                        {
                            container.UpdateAnimationToStatic();
                            container.ShiftSortOrder(false);
                            container.GetElement().UpdateMaskInteraction(SpriteMaskInteraction.None, true);
                        }
                        else
                        {
                            containerPlayBlinkAnimation.Remove(container);
                        }

                        if (containerPlayBlinkAnimation.Count == 0 && canCheckBlinkFinished)
                        {
                            CheckAndStopBlinkAnimation();
                        }

                        CheckAllBlinkFinished();
                    });

                    blinkAnimationPlayed = true;
                    if (container.sequenceElement.config.id == Constant11020.bonusElement)
                    {
                        scatterBlinkAnimationPlayed = true;
                    }
                }
            }

            return blinkAnimationPlayed && scatterBlinkAnimationPlayed;
        }

        // public override void CheckAndStopBlinkAnimation()
        // {
        //     var rollCount = wheel.wheelState.rollCount;

        //     var extraState = wheel.GetContext().state.Get<ExtraState>();

        //     for (var i = 0; i < rollCount; i++)
        //     {
        //         var animationInfo = wheel.wheelState.GetBlinkAnimationInfo(i);
        //         if (animationInfo != null && animationInfo.Count > 0)
        //         {
        //             for (var index = 0; index < animationInfo.Count; index++)
        //             {
        //                 var container = wheel.GetRoll(i).GetVisibleContainer((int) animationInfo[index]);

        //                 if (container.sequenceElement.config.blinkType == BlinkAnimationPlayStyleType.IdleCondition)
        //                 {
        //                     if (!extraState.IsBlinkFeatureTriggered(container.sequenceElement.config.id))
        //                     {
        //                         container.UpdateAnimationToStatic();
        //                         container.ShiftSortOrder(false);

        //                         container.GetElement().UpdateMaskInteraction(SpriteMaskInteraction.None, true);
        //                     }
        //                 }
        //                 else
        //                 {
        //                     container.GetElement().UpdateMaskInteraction(SpriteMaskInteraction.None, true);
        //                 }
        //             }
        //         }
        //     }
        // }
    }
}
