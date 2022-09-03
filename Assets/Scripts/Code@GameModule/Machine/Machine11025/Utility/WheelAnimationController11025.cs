using System;
using System.Collections.Generic;
using System.Linq;
using Google.ilruntime.Protobuf.Collections;
using Spine;
using UnityEngine;
using UnityEngine.Rendering;

namespace GameModule
{
    public class WheelAnimationController11025:WheelAnimationController
    {
        public Dictionary<string, int> blinkEffectStopList = new Dictionary<string, int>();
        public AudioSource J01AntiEffect;
        public Wheel11025 wheel11025;
        private ExtraState11025 extraState;
        public MachineContext machineContext;
        public bool IsInScatterAnti = false;
        public List<FlowerElementContainer11025> scatterBlinkList = new List<FlowerElementContainer11025>();
        public List<GameObject> antiStickyList = new List<GameObject>();
        public WheelAnimationController11025()
        {
        }
        
        public override void BindingWheel(Wheel inWheel)
        {
            base.BindingWheel(inWheel);
            machineContext = wheel.GetContext();
            wheel11025 = (Wheel11025) wheel;
        }
        
        public override void OnWheelStartSpinning()
        {
            listWheelAppears.Clear();
            scatterBlinkList.Clear();
            containerPlayBlinkAnimation.Clear();
            canCheckBlinkFinished = false;
            var freeState = machineContext.state.Get<FreeSpinState>();
            if (wheel11025.wheelName == Constant11025.WheelFreeGameName && Constant11025.ShopSpecialFreeId.Contains(freeState.freeSpinId))
            {
            }
            else
            {
                for (var i = 0; i < wheel11025.rollCount; i++)
                {
                    wheel11025.SetRollMaskColor(i,RollMaskOpacityLevel11025.Level2,0.2f); 
                }   
            }
        }
        public override void OnRollEnterSlowDown(int rollIndex)
        {
            if (wheel11025.wheelName == Constant11025.WheelBaseGameName)
            {
                wheel11025.SetRollMaskColor(rollIndex,RollMaskOpacityLevel11025.None,0.3f); 
            }
        }
        public override void StopAnticipationAnimation(bool playStopSound = true)
        {
            base.StopAnticipationAnimation(playStopSound);
            IsInScatterAnti = false;
            for (var i = 0; i < scatterBlinkList.Count; i++)
            {
                if (scatterBlinkList[i].IsAnti)
                {
                    scatterBlinkList[i].IsAnti = false;
                    scatterBlinkList[i].UpdateAnimationToStatic();
                }
            }
            for (var i = 0; i < antiStickyList.Count; i++)
            {
                machineContext.assetProvider.RecycleGameObject("WinFrameLan",antiStickyList[i]);
            }
            antiStickyList.Clear();
            if (J01AntiEffect)
            {
                J01AntiEffect.Stop();
                J01AntiEffect = null;
            }
        }
        public override void ShowAnticipationAnimation(int rollIndex)
        {
            //  XDebug.Log("ShowDrum:" + rollView.RollIndex);
            if (((WheelState11025) wheel.wheelState).AntiByScatter(rollIndex))
            {
                IsInScatterAnti = true;
                for (var i = 0; i < scatterBlinkList.Count; i++)
                {
                    if (!scatterBlinkList[i].IsBlink && !scatterBlinkList[i].IsAnti)
                    {
                        scatterBlinkList[i].IsAnti = true;
                        scatterBlinkList[i].PlayScatterAntiIdle();
                    }
                }
                
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
                    XUtility.PlayAnimation(anticipationAnimationGo.GetComponent<Animator>(),"AnticipationAnimation"+Constant11025.RollHeightList[rollIndex]);
                    //}

                    XDebug.Log("PlayAnticipationSound:" + wheel.wheelState.GetAnticipationSoundAssetName());
                
                    anticipationAnimationGo.transform.localPosition = wheel.GetAnticipationAnimationPosition(rollIndex);
                    AudioUtil.Instance.StopAudioFx(wheel.wheelState.GetAnticipationSoundAssetName());
                    AudioUtil.Instance.PlayAudioFx(wheel.wheelState.GetAnticipationSoundAssetName());
                }
            }
            else if(IsInScatterAnti)
            {
                base.StopAnticipationAnimation();
                IsInScatterAnti = false;
                for (var i = 0; i < scatterBlinkList.Count; i++)
                {
                    if (scatterBlinkList[i].IsAnti)
                    {
                        scatterBlinkList[i].IsAnti = false;
                        scatterBlinkList[i].UpdateAnimationToStatic();
                    }
                }
            }

            for (var i = 0; i < antiStickyList.Count; i++)
            {
                machineContext.assetProvider.RecycleGameObject("WinFrameLan",antiStickyList[i]);
            }
            antiStickyList.Clear();
            var antiPosY = ((WheelState11025) wheel.wheelState).AntiByChip(rollIndex);
            if (antiPosY >= 0)
            {
                var antiFrame = machineContext.assetProvider.InstantiateGameObject("WinFrameLan", true);
                antiFrame.transform.SetParent(wheel11025.transform,false);
                antiFrame.transform.position = wheel.GetRoll(rollIndex).GetVisibleContainerPosition(antiPosY);
                antiFrame.SetActive(true);
                antiStickyList.Add(antiFrame);
                if (J01AntiEffect)
                {
                    J01AntiEffect.Stop();
                    J01AntiEffect = null;
                }
                J01AntiEffect = AudioUtil.Instance.PlayAudioFx("J01_Anticipation");
            }
        }
        
        public override bool ShowBlinkAnimation(RepeatedField<uint> blinkInfo, int rollIndex)
        {
            for (var i = 0; i < Constant11025.RollHeightList[rollIndex]; i++)
            {
                var container = (FlowerElementContainer11025)wheel.GetRoll(rollIndex).GetVisibleContainer(i);
                var customData = container.sequenceElement.elementCustomData;
                if (customData != null &&
                    customData is int)
                {
                    ((WheelBase11025) wheel11025).AddFlowerContainer(container);
                    var appearKey = wheel.wheelName + rollIndex + "flower" + i;
                    listWheelAppears.Add(appearKey);
                    container.ShowCollectFlower((int)customData, () =>
                    {
                        listWheelAppears.Remove(appearKey);
                        CheckAllBlinkFinished();
                    });
                }
            }
            
            bool blinkAnimationPlayed = false;
            ReelStopSoundState.SoundState item = GetReelStopSoundState(rollIndex);
            if (blinkInfo != null && blinkInfo.Count > 0)
            {
                int blinkIndex = GetNeedPlayBlinkSoundRowIndex(blinkInfo, rollIndex);

                for (var i = 0; i < blinkInfo.Count; i++)
                {

                    if (((Wheel11025) wheel).StickyMap[rollIndex][blinkInfo[i]].HasContainer())
                    {
                        continue;
                    }
                    
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

                    var tempI = i;
                    if (Constant11025.ScatterList.Contains(container.sequenceElement.config.id))
                    {
                        ((FlowerElementContainer11025) container).IsBlink = true;
                        scatterBlinkList.Add((FlowerElementContainer11025)container);
                    }
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

            return blinkAnimationPlayed;
        }
        
        protected override int GetNeedPlayBlinkSoundRowIndex(RepeatedField<uint> blinkInfo,int rollIndex)
        {
            var blinkIndex = 0;
            var listSoundOrderId = new List<Tuple<int,int>>();
            for (var i = 0; i < blinkInfo.Count; i++)
            {
                var container = wheel.GetRoll(rollIndex).GetVisibleContainer((int) blinkInfo[i]);
                if (((Wheel11025) wheel).StickyMap[rollIndex][blinkInfo[i]].HasContainer())
                {
                    continue;
                }
                listSoundOrderId.Add(new Tuple<int, int>(container.sequenceElement.config.blinkSoundOrderId,(int)blinkInfo[i]));
            }
 
            if (listSoundOrderId.Count>0)
            {
                listSoundOrderId.Sort((a, b) =>  b.Item1.CompareTo(a.Item1));
                blinkIndex = listSoundOrderId[0].Item2;
            }
            return blinkIndex;
        }
        
        protected override void OnBlinkAnimationFinished(ElementContainer elementContainer, string appearKey,int rollIndex,int rowIndex)
        {
            ((FlowerElementContainer11025) elementContainer).IsBlink = false;

            listWheelAppears.Remove(appearKey);
            if (elementContainer.sequenceElement.config.blinkType == BlinkAnimationPlayStyleType.Default)
            {
                elementContainer.UpdateAnimationToStatic();
                elementContainer.ShiftSortOrder(false);
            }
            else 
            {
                containerPlayBlinkAnimation.Remove(elementContainer);
                if (Constant11025.ScatterList.Contains(elementContainer.sequenceElement.config.id) && IsInScatterAnti)
                {
                    ((FlowerElementContainer11025) elementContainer).IsAnti = true;
                    ((FlowerElementContainer11025) elementContainer).PlayScatterAntiIdle();
                }
            }

            if (containerPlayBlinkAnimation.Count == 0 && canCheckBlinkFinished)
            {
                CheckAndStopBlinkAnimation();
            }

            CheckAllBlinkFinished();
        }
        
        public override void OnAllRollSpinningStopped(Action callback)
        {
            base.OnAllRollSpinningStopped(callback);

            //如果玩家点了Quick Stop，每列停下的时候不会播放blink，在最好全部停下的时候播放一次ReelStop
            if (wheel.wheelState.playerQuickStopped)
            {
                var keyList = blinkEffectStopList.Keys.ToList();
                for (var i = 0; i < keyList.Count; i++)
                {
                    var key = keyList[i];
                    if (blinkEffectStopList[key] > 0)
                    {
                        AudioUtil.Instance.PlayAudioFx(key);
                    }
                }
            }
            blinkEffectStopList.Clear();
        }
        
        public override void PlayBlinkSound(ElementContainer container, int rollIndex, int rowIndex)
        {
            var soundName = GetBlinkSoundName(container, rollIndex, rollIndex);
            if (wheel.wheelState.playerQuickStopped)
            {
                if (!blinkEffectStopList.ContainsKey(soundName))
                {
                    blinkEffectStopList[soundName] = 0;
                }
                blinkEffectStopList[soundName]++;
            }
            else
            {
                AudioUtil.Instance.PlayAudioFx(soundName);   
            }
        }
    }
}