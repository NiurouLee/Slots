using System.Collections.Generic;
using DragonU3DSDK.Audio;
using Google.ilruntime.Protobuf.Collections;
using UnityEngine;
using UnityEngine.Rendering;

namespace GameModule
{
    public class WheelAnimationController11031 : WheelAnimationController
    {
        public bool isIdleSound = false;
        public override void OnWheelStartSpinning()
        {
            var activeState = wheel.GetContext().state.Get<WheelsActiveState11031>();
            if (!activeState.IsInLink)
            {
                wheel.GetContext().state.Get<WheelsActiveState11031>().FadeOutRollMask(wheel);
            }
            isIdleSound = false;
            base.OnWheelStartSpinning();
        }

        public override void CheckAndStopBlinkAnimation()
        {
            var rollCount = wheel.wheelState.rollCount;

            var extraState = wheel.GetContext().state.Get<ExtraState>();

            for (var i = 0; i < rollCount; i++)
            {
                var animationInfo = wheel.wheelState.GetBlinkAnimationInfo(i);
                if (animationInfo != null && animationInfo.Count > 0)
                {
                    for (var index = 0; index < animationInfo.Count; index++)
                    {
                        var container = wheel.GetRoll(i).GetVisibleContainer((int) animationInfo[index]);

                        if (container.sequenceElement.config.blinkType == BlinkAnimationPlayStyleType.IdleCondition)
                        {
                            if (!extraState.IsBlinkFeatureTriggered(container.sequenceElement.config.id))
                            {
                                container.UpdateAnimationToStatic();
                                container.ShiftSortOrder(false);
                            }
                            else
                            {
                                if (container.sequenceElement.config.id == Constant11031.TruckElementId && isIdleSound == false)
                                {
                                    AudioUtil.Instance.PlayAudioFxOneShot("Idle");
                                    isIdleSound = true;
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}