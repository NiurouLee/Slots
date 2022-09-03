// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date :2021-06-27 11:01 AM
// Ver : 1.0.0
// Description : WheelAnimationController.cs 处理Roll滚动过程中或者滚动停止时候的各种动画效果播放
// 如Appear动画， RollStop声音等等，
// ChangeLog : 
// **********************************************

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Google.ilruntime.Protobuf.Collections;
using UnityEngine;
using UnityEngine.Rendering;

namespace GameModule
{
    public class WheelAnimationController : IWheelAnimationController
    {
        protected Wheel wheel;

        protected MachineAssetProvider assetProvider;

        protected List<string> listWheelAppears;

        protected bool canCheckBlinkFinished;

        protected List<ElementContainer> containerPlayBlinkAnimation;
        private Action appearEndCallback;
 
        public virtual void BindingWheel(Wheel inWheel)
        {
            wheel = inWheel;
            assetProvider = wheel.GetAssetProvider();

            listWheelAppears = new List<string>();
            containerPlayBlinkAnimation = new List<ElementContainer>();
        }

        public virtual void StopAnticipationAnimation(bool playStopSound = true)
        {
            var anticipationAnimationGo = wheel.GetAttachedGameObject("AnticipationAnimation");

            if (anticipationAnimationGo != null)
            {
                anticipationAnimationGo.SetActive(false);
                AudioUtil.Instance.StopAudioFx(wheel.wheelState.GetAnticipationSoundAssetName());
            }
        }

        public virtual void OnWheelStartSpinning()
        {
            listWheelAppears.Clear();
            containerPlayBlinkAnimation.Clear();
            canCheckBlinkFinished = false;
        }

        public virtual void OnRollSpinningStopped(int rollIndex, Action rollLogicEnd)
        {
            rollLogicEnd.Invoke();
        }

        public virtual void OnRollStartBounceBack(int rollIndex)
        {
            var animationInfo = wheel.wheelState.GetBlinkAnimationInfo(rollIndex);
            bool hasBlinkAppeared = ShowBlinkAnimation(animationInfo, rollIndex);

            ReelStopSoundState.SoundState item = GetReelStopSoundState(rollIndex);
            if (item != null)
            {
                if (item.RollStopCount == 
                    wheel.GetContext().state.Get<WheelsActiveState>().GetRunningWheel().Count)
                {
                    if (string.IsNullOrEmpty(item.SoundName))
                    {
                        PlayReelStop(rollIndex);
                    }
                    else
                    {
                        AudioUtil.Instance.PlayAudioFx(item.SoundName);
                    }   
                }
            }
            else
            {
                if (!hasBlinkAppeared)
                {
                    PlayReelStop(rollIndex);
                }
            }
        }
 
        public virtual void PlayReelStop(int rollIndex = -1)
        {
            // Debug.Log("needPlayReelStop" + needPlayReelStop + "||" + EarlyStopTriggered);
            if (!wheel.wheelState.playerQuickStopped || rollIndex < 0)
            {
                var reelStopSoundName = wheel.wheelState.GetEasingConfig().GetReelStopSoundName();
                AudioUtil.Instance.PlayAudioFxOneShot(reelStopSoundName);
            }
        }

        public virtual void OnRollEnterSlowDown(int rollIndex)
        {
        }

        public virtual void OnAllRollSpinningStopped(Action callback)
        {
            appearEndCallback = callback;
            canCheckBlinkFinished = true;
            if (containerPlayBlinkAnimation.Count == 0)
                CheckAndStopBlinkAnimation();

            CheckAllBlinkFinished();
            
            CheckAndStopAllElementAnticipation();

            //如果玩家点了Quick Stop，每列停下的时候不会播放ReelStop，在最好全部停下的时候播放一次ReelStop
            if (wheel.wheelState.playerQuickStopped)
            {
                PlayReelStop();
            }
        }

        public virtual void CheckAndStopAllElementAnticipation()
        {
            StopAllElementAnticipation();
        }

        public void StopAllElementAnticipation()
        {
            wheel.ToggleAttachedGameObject("ElementAnticipation",false);
        }

        public virtual void CheckAndStopBlinkAnimation()
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
                        }
                    }
                }
            }
        }

        public virtual int GetBlinkSoundIndex(ElementConfig elementConfig, int rollIndex)
        {
            var index = 0;

            var listVariantId = wheel.GetContext().elementExtraInfoProvider.GetElementBlinkVariantList(elementConfig.id);
            for (var i = 0; i < rollIndex; i++)
            {
                foreach (var variantId in listVariantId)
                {
                    if (wheel.wheelState.IsSpinResultContainElementAtRollIndex(variantId, i))
                    {
                        index++;
                        break;
                    }
                }
                
            }

            return index;
        }

        public virtual void PlayBlinkSound(ElementContainer container, int rollIndex, int rowIndex)
        {
            AudioUtil.Instance.PlayAudioFx(GetBlinkSoundName(container, rollIndex, rollIndex));
        }

        public virtual bool ShowBlinkAnimation(RepeatedField<uint> blinkInfo, int rollIndex)
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

            return blinkAnimationPlayed;
        }

        protected virtual void OnBlinkAnimationFinished(ElementContainer elementContainer, string appearKey,int rollIndex,int rowIndex)
        {
            listWheelAppears.Remove(appearKey);
            if (elementContainer.sequenceElement.config.blinkType == BlinkAnimationPlayStyleType.Default)
            {
                elementContainer.UpdateAnimationToStatic();
                elementContainer.ShiftSortOrder(false);
            }
            else
            {
                containerPlayBlinkAnimation.Remove(elementContainer);
            }

            if (containerPlayBlinkAnimation.Count == 0 && canCheckBlinkFinished)
            {
                CheckAndStopBlinkAnimation();
            }

            CheckAllBlinkFinished();
        }

        /// <summary>
        /// 检查是否所有的Blink都结束了
        /// </summary>
        protected void CheckAllBlinkFinished()
        {
            if (listWheelAppears.Count == 0 && canCheckBlinkFinished)
            {
                appearEndCallback?.Invoke();
            }
        }

        public virtual void ShowAnticipationAnimation(int rollIndex)
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
        public virtual void ShowElementAnticipationAnimation(int rollIndex, uint id)
        {
            var configSet = wheel.GetContext().machineConfig.GetElementConfigSet();
            var elementConfig = configSet.GetElementConfig(id);
            var elementAnticipationName = $"{elementConfig.name}_ElementAnticipation_{rollIndex}";
            var elementAnticipationGo = wheel.GetAttachedGameObject(elementAnticipationName);
            
            if (wheel.GetAttachedGameObject(elementAnticipationName) == null)
            {
                // wheel.AttachGameObject(); =
                elementAnticipationGo =
                    assetProvider.InstantiateGameObject($"{elementConfig.name}_Anticipation");
                elementAnticipationGo.name = elementAnticipationName;
                if (elementAnticipationGo)
                {
                    elementAnticipationGo.transform.SetParent(wheel.transform.Find("Rolls"), false);

                    if (!elementAnticipationGo.GetComponent<SortingGroup>())
                    {
                        var sortingGroup = elementAnticipationGo.AddComponent<SortingGroup>();
                        sortingGroup.sortingLayerID = SortingLayer.NameToID("Element");
                        sortingGroup.sortingOrder = 1;
                    }

                    wheel.AttachGameObject(elementAnticipationName, elementAnticipationGo);
                }
            }

            if (elementAnticipationGo)
            {
                elementAnticipationGo.SetActive(false);
                elementAnticipationGo.SetActive(true);
                                
                elementAnticipationGo.transform.localPosition = wheel.GetAnticipationAnimationPosition(rollIndex);
            }
        }
        
        public virtual string GetBlinkSoundName(ElementContainer container, int rollIndex, int rowIndex)
        {
            var config = container.sequenceElement.config;

            var blinkSoundName = config.blinkSoundName + "0" + (GetBlinkSoundIndex(config, rollIndex) + 1);

            if (assetProvider.GetAsset<AudioClip>(blinkSoundName))
            {
                var freeState = container.sequenceElement.machineContext.state.Get<FreeSpinState>();
                if (freeState.IsInFreeSpin && assetProvider.GetAsset<AudioClip>(blinkSoundName + "_Free"))
                {
                    blinkSoundName = blinkSoundName + "_Free";
                }
            }
            else
            {
                blinkSoundName = config.blinkSoundName + "01";
                var freeState = container.sequenceElement.machineContext.state.Get<FreeSpinState>();
                if (freeState.IsInFreeSpin && assetProvider.GetAsset<AudioClip>(blinkSoundName+ "_Free"))
                {
                    blinkSoundName = blinkSoundName + "_Free";
                }
            }
            return blinkSoundName;
        }
        
        public virtual string GetReelStopSoundName(int rollIndex)
        {
            return wheel.wheelState.GetEasingConfig().GetReelStopSoundName();
        }

        protected virtual int GetNeedPlayBlinkSoundRowIndex(RepeatedField<uint> blinkInfo,int rollIndex)
        {
            var blinkIndex = 0;
            var listSoundOrderId = new List<Tuple<int,int>>();
            for (var i = 0; i < blinkInfo.Count; i++)
            {
                var container = wheel.GetRoll(rollIndex).GetVisibleContainer((int) blinkInfo[i]);
                listSoundOrderId.Add(new Tuple<int, int>(container.sequenceElement.config.blinkSoundOrderId,(int)blinkInfo[i]));
            }
 
            if (listSoundOrderId.Count>0)
            {
                listSoundOrderId.Sort((a, b) =>  b.Item1.CompareTo(a.Item1));
                blinkIndex = listSoundOrderId[0].Item2;
            }
            return blinkIndex;
        }

        public ReelStopSoundState.SoundState GetReelStopSoundState(int rollIndex)
        {
            if ( wheel.GetContext().state.Get<ReelStopSoundState>() != null)
            {
                return wheel.GetContext().state.Get<ReelStopSoundState>().ListStopSoundState[rollIndex];   
            }
            return null;
        }
    }
}