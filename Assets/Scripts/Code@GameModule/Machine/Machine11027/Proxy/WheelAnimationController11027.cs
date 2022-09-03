using System.Collections.Generic;
using Google.ilruntime.Protobuf.Collections;
using UnityEngine;
using UnityEngine.Rendering;

namespace GameModule
{
    public class WheelAnimationController11027: WheelAnimationController
    {
        private string anticipationName = string.Empty;
        public override async void OnWheelStartSpinning()
        {
            wheel.GetContext().state.Get<WheelsActiveState11027>().FadeOutRollMask(wheel);
            base.OnWheelStartSpinning();
        }
        
        public virtual void ShowRollIndexAnticipationAnimation(int rollIndex)
        {
            //  XDebug.Log("ShowDrum:" + rollView.RollIndex);

            var wheelState = wheel.wheelState;

            var anticipationAnimationGo = wheel.GetAttachedGameObject(GetAnticipationName(rollIndex));

            if (wheel.GetAttachedGameObject(GetAnticipationName(rollIndex)) == null)
            {
                // wheel.AttachGameObject(); =
                anticipationAnimationGo =
                     assetProvider.InstantiateGameObject(GetAnticipationName(rollIndex));

                if (anticipationAnimationGo)
                {
                    anticipationAnimationGo.transform.SetParent(wheel.transform.Find("Rolls"), false);
                    if (!anticipationAnimationGo.GetComponent<SortingGroup>())
                    {
                        var sortingGroup = anticipationAnimationGo.AddComponent<SortingGroup>();
                        sortingGroup.sortingLayerID = SortingLayer.NameToID("Element");
                        sortingGroup.sortingOrder = -1;
                    }

                    wheel.AttachGameObject(GetAnticipationName(rollIndex), anticipationAnimationGo);
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
                // AudioUtil.Instance.StopAudioFx(wheel.wheelState.GetAnticipationSoundAssetName()); 
                // AudioUtil.Instance.PlayAudioFx(wheel.wheelState.GetAnticipationSoundAssetName());
            }
        }

        public virtual void StopRollIndexAnticipationAnimation(int rollIndex,bool playStopSound = true)
        {
            var anticipationAnimationGo = wheel.GetAttachedGameObject(GetAnticipationName(rollIndex));
            if (anticipationAnimationGo != null)
            {
                anticipationAnimationGo.SetActive(false);
                // AudioUtil.Instance.StopAudioFx(wheel.wheelState.GetAnticipationSoundAssetName());
            }
        }
        private string GetAnticipationName(int rollIndex)
        {
            switch (rollIndex)
            {
                case 2:
                    anticipationName = "Anticipation";
                    break;
                case 3:
                    anticipationName = "Anticipation1";
                    break;
                case 4:
                    anticipationName = "Anticipation2";
                    break;
            }

            return anticipationName;
        }
        
        // public override bool ShowBlinkAnimation(RepeatedField<uint> blinkInfo, int rollIndex)
        // {
        //     var blinkAnimationPlayed = base.ShowBlinkAnimation(blinkInfo, rollIndex);
        //     if (blinkInfo.Count == 0)
        //     {
        //         for (int i = 2; i < 4; i++)
        //         {
        //             StopRollIndexAnticipationAnimation(i);
        //         }
        //         for (var i = 0; i < 3; i++)
        //         {
        //             var container = wheel.GetRoll(2).GetVisibleContainer(i);
        //             if (Constant11027.ScatterElementId == container.sequenceElement.config.id)
        //             {
        //                 container.PlayElementAnimation("Default");
        //                 container.ShiftSortOrder(false);
        //             }
        //         }
        //         for (var i = 0; i < 3; i++)
        //         {
        //             var container = wheel.GetRoll(3).GetVisibleContainer(i);
        //             if (Constant11027.ScatterElementId == container.sequenceElement.config.id)
        //             {
        //                  container.PlayElementAnimation("Default");
        //                  container.ShiftSortOrder(false);
        //             }
        //         }
        //     }
        //     // else
        //     // {
        //     //     for (var i = 0; i < blinkInfo.Count; i++)
        //     //     {
        //     //         var container = wheel.GetRoll(rollIndex).GetVisibleContainer((int) blinkInfo[i]);
        //     //         container.PlayElementAnimation("Idle");
        //     //     }
        //     //
        //     // }
        //     return blinkAnimationPlayed;
        // }
        //
        public override bool ShowBlinkAnimation(RepeatedField<uint> blinkInfo, int rollIndex)
        {
            bool blinkAnimationPlayed = false;
            ReelStopSoundState.SoundState item = GetReelStopSoundState(rollIndex);
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

                    var tempRowIndex = (int)blinkInfo[i];
                    container.PlayElementAnimation("Blink", false,
                        () =>
                        {
                            OnBlinkAnimationFinished(container, appearKey,rollIndex,tempRowIndex);
                            ShowElementIdleAnimation(rollIndex, container.sequenceElement.config.id);
                        });
                    if (wheel.GetContext().elementExtraInfoProvider
                        .CanShowElementAnticipation(container.sequenceElement.config.id))
                    {
                        ShowElementAnticipationAnimation(rollIndex, container.sequenceElement.config.id);
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

                for (int i = 2; i < 4; i++)
                {
                    StopRollIndexAnticipationAnimation(i);
                }

                // if (!wheel.wheelState.playerQuickStopped)
                // {
                    for (var i = 0; i < 3; i++)
                    {
                        var container = wheel.GetRoll(2).GetVisibleContainer(i);
                        if (Constant11027.ScatterElementId == container.sequenceElement.config.id)
                        {
                            // container.PlayElementAnimation("Default");
                            // container.ShiftSortOrder(false);
                            container.UpdateAnimationToStatic(); 
                            container.ShiftSortOrder(false);
                        }
                    }

                    for (var i = 0; i < 3; i++)
                    {
                        var container = wheel.GetRoll(3).GetVisibleContainer(i);
                        if (Constant11027.ScatterElementId == container.sequenceElement.config.id)
                        {
                            // container.PlayElementAnimation("Default");
                            // container.ShiftSortOrder(false);
                            container.UpdateAnimationToStatic(); 
                            container.ShiftSortOrder(false);
                        }
                    }
                // }
            }

            return blinkAnimationPlayed;
        }
        
        public override void ShowElementAnticipationAnimation(int rollIndex, uint id)
        {
            List<ElementContainer> twoColumnWildList = new List<ElementContainer>();
            List<ElementContainer> threeColumnWildList = new List<ElementContainer>();
            List<ElementContainer> fourColumnWildList = new List<ElementContainer>();
            if (rollIndex == 3)
            {
                for (var i = 0; i < 3; i++)
                {
                    var container = wheel.GetRoll(2).GetVisibleContainer(i);
                    if (Constant11027.ScatterElementId == container.sequenceElement.config.id)
                    {
                        twoColumnWildList.Add(container);
                    }
                }

                for (var i = 0; i < 3; i++)
                {
                    var container = wheel.GetRoll(3).GetVisibleContainer(i);
                    if (Constant11027.ScatterElementId == container.sequenceElement.config.id)
                    {
                        threeColumnWildList.Add(container);
                    }
                }

                if (twoColumnWildList.Count > 0 && threeColumnWildList.Count > 0)
                {
                    for (int a = 2; a < 4; a++)
                    {
                        ShowRollIndexAnticipationAnimation(a);
                    }
                }
            }

            if (rollIndex == 4)
            {
                for (var i = 0; i < 3; i++)
                {
                    var container = wheel.GetRoll(2).GetVisibleContainer(i);
                    if (Constant11027.ScatterElementId == container.sequenceElement.config.id)
                    {
                        twoColumnWildList.Add(container);
                    }
                }

                for (var i = 0; i < 3; i++)
                {
                    var container = wheel.GetRoll(3).GetVisibleContainer(i);
                    if (Constant11027.ScatterElementId == container.sequenceElement.config.id)
                    {
                        threeColumnWildList.Add(container);
                    }
                }
                for (var i = 0; i < 3; i++)
                {
                    var container = wheel.GetRoll(4).GetVisibleContainer(i);
                    if (Constant11027.ScatterElementId == container.sequenceElement.config.id)
                    {
                        fourColumnWildList.Add(container);
                    }
                }

                if (twoColumnWildList.Count > 0 && threeColumnWildList.Count > 0)
                {
                    // AudioUtil.Instance.PlayAudioFx("B01AnticipationSound");
                    if (fourColumnWildList.Count > 0)
                    {
                        ShowRollIndexAnticipationAnimation(rollIndex);
                    }
                    else
                    {
                        for (int c = 2; c < 4; c++)
                        {
                             StopRollIndexAnticipationAnimation(c);
                        }
                    }
                }
            }
        }
        
        public virtual void ShowElementIdleAnimation(int rollIndex, uint id)
        { 
            if (wheel.wheelState.playerQuickStopped)
            {
                for (var q = 2; q < 5; q++)
                {
                    for (var j = 0; j < 3; j++)
                    {
                        var container = wheel.GetRoll(q).GetVisibleContainer(j);
                        if (Constant11027.ScatterElementId == container.sequenceElement.config.id)
                        {
                            container.UpdateAnimationToStatic();
                            container.ShiftSortOrder(false);
                        }
                    }
                }
                return;
            }
            List<ElementContainer> twoColumnWildList = new List<ElementContainer>();
            List<ElementContainer> threeColumnWildList = new List<ElementContainer>();
            List<ElementContainer> fourColumnWildList = new List<ElementContainer>();
            if (rollIndex == 3)
            {
                for (var i = 0; i < 3; i++)
                {
                    var container = wheel.GetRoll(2).GetVisibleContainer(i);
                    if (Constant11027.ScatterElementId == container.sequenceElement.config.id)
                    {
                        twoColumnWildList.Add(container);
                    }
                }

                for (var i = 0; i < 3; i++)
                {
                    var container = wheel.GetRoll(3).GetVisibleContainer(i);
                    if (Constant11027.ScatterElementId == container.sequenceElement.config.id)
                    {
                        threeColumnWildList.Add(container);
                    }
                }
                
                for (var i = 0; i < 3; i++)
                {
                    var container = wheel.GetRoll(4).GetVisibleContainer(i);
                    if (Constant11027.ScatterElementId == container.sequenceElement.config.id)
                    {
                        fourColumnWildList.Add(container);
                    }
                }

                if (twoColumnWildList.Count > 0 && threeColumnWildList.Count > 0)
                {
                    for (var w = 0; w < twoColumnWildList.Count; w++)
                    {
                        var twoContainer = twoColumnWildList[w];
                        twoContainer.PlayElementAnimation("Idle");
                    }
                    for (var h = 0; h < threeColumnWildList.Count; h++)
                    {
                        var threeContainer = threeColumnWildList[h];
                        threeContainer.PlayElementAnimation("Idle");
                    }
                    for (var h = 0; h < fourColumnWildList.Count; h++)
                    {
                        var fourContainer = fourColumnWildList[h];
                        fourContainer.PlayElementAnimation("Idle");
                    }
                }
            }

            if (rollIndex == 4)
            {
                for (var i = 0; i < 3; i++)
                {
                    var container = wheel.GetRoll(2).GetVisibleContainer(i);
                    if (Constant11027.ScatterElementId == container.sequenceElement.config.id)
                    {
                        twoColumnWildList.Add(container);
                    }
                }

                for (var i = 0; i < 3; i++)
                {
                    var container = wheel.GetRoll(3).GetVisibleContainer(i);
                    if (Constant11027.ScatterElementId == container.sequenceElement.config.id)
                    {
                        threeColumnWildList.Add(container);
                    }
                }
                for (var i = 0; i < 3; i++)
                {
                    var container = wheel.GetRoll(4).GetVisibleContainer(i);
                    if (Constant11027.ScatterElementId == container.sequenceElement.config.id)
                    {
                        fourColumnWildList.Add(container);
                    }
                }

                if (twoColumnWildList.Count > 0 && threeColumnWildList.Count > 0)
                {
                    // AudioUtil.Instance.PlayAudioFx("B01AnticipationSound");
                    if (fourColumnWildList.Count > 0)
                    {
                        for (var o = 0; o < fourColumnWildList.Count; o++)
                        {
                            var fourContainer = fourColumnWildList[o];
                            fourContainer.PlayElementAnimation("Idle");
                        }
                    }
                }
            }
        }
    }
}
