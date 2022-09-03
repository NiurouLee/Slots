//  **********************************************
//  Copyright(c) 2021 by com.ustar
//  All right reserved
// 
//  Author : Besure.Chen
//  Date :2021-11-03 15:27
//  Ver : 1.0.0
//  Description : WheelAnimationController11016.cs
//  ChangeLog :
//  **********************************************

using System;
using System.Collections.Generic;
using Google.ilruntime.Protobuf.Collections;
using UnityEngine;

namespace GameModule
{
    public class WheelAnimationController11016:WheelAnimationController
    {
        public override bool ShowBlinkAnimation(RepeatedField<uint> blinkInfo, int rollIndex)
        {
            bool blinkAnimationPlayed = false;
            var item = wheel.GetContext().state.Get<ReelStopSoundState>().ListStopSoundState[rollIndex];
            if (blinkInfo != null && blinkInfo.Count > 0)
            {
                int blinkIndex = GetNeedPlayBlinkSoundRowIndex(blinkInfo, rollIndex);
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

                    if (!blinkAnimationPlayed && blinkInfo[i] == blinkIndex)
                    {
                        item.SoundName = GetBlinkSoundName(container, rollIndex, (int) blinkInfo[i]);
                        item.RollStopCount++;
                    }
                    container.PlayElementAnimation("Blink", false, () =>
                    {
                        listWheelAppears.Remove(appearKey);
                        if (container.sequenceElement.config.blinkType == BlinkAnimationPlayStyleType.Default)
                        {
                            container.UpdateAnimationToStatic();
                            container.ShiftSortOrder(false);
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
                    if (Constant11016.IsFeaturedSlotElement(container.sequenceElement.config.id))
                    {
                        container.EnableSortingGroup(false);
                        (container.GetElement() as Element11016).ShiftMaskAndSortOrder(i*100);
                    }
                    blinkAnimationPlayed = true;
                }
            }
            else
            {
                item.RollStopCount++;
            }
            return blinkAnimationPlayed;
        }
    }
}