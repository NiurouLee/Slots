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
using System.Collections.Generic;
using com.adjust.sdk;
using Google.ilruntime.Protobuf.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace GameModule
{
    public class IndependentWheelAnimationController : WheelAnimationController
    {
        protected List<bool> listWheelBlinkFlag;

        public override void BindingWheel(Wheel inWheel)
        {
            base.BindingWheel(inWheel);
            listWheelBlinkFlag = new List<bool>();
            var rollCount = wheel.wheelState.rollCount;
            for (int i = 0; i < rollCount; i++)
            {
                listWheelBlinkFlag.Add(false);
            }
        }

        public override void OnWheelStartSpinning()
        {
            base.OnWheelStartSpinning();
            var listLength = listWheelBlinkFlag.Count;
            for (int i = 0; i < listLength; i++)
            {
                listWheelBlinkFlag[i] = false;
            }
        }

        public override void OnRollStartBounceBack(int rollIndex)
        {
            XDebug.Log("OnRollStartBounceBack "+rollIndex);
            var rowCount = wheel.wheelState.GetWheelConfig().rollRowCount;
            var columnIndex = rollIndex / rowCount;
            var rowIndex = rollIndex % rowCount;
            var startIndex = columnIndex * rowCount;
            var endIndex = (columnIndex + 1) * rowCount - 1;
            bool isLastSpinRoll = false;
            for (int i = endIndex; i >= startIndex; i--)
            {
                // if (listWheelBlinkFlag[i])
                // {
                //     hasDoneBonusFlag = true;
                //     break;
                // }

                if (!wheel.wheelState.IsRollLocked(i))
                {
                    if (rollIndex == i)
                    {
                        isLastSpinRoll = true;   
                    }
                    break;
                }
            }
            listWheelBlinkFlag[rollIndex] = true;
            // if (rollIndex % rowCount == rowCount - 1)
            if (isLastSpinRoll)
            {   
                var blinkInfoForARow = new List<int>();
                
                // for (var i = rollIndex - (rowCount - 1); i <= rollIndex; i++)
                for (var i = rollIndex/rowCount * rowCount; i < (rollIndex/rowCount + 1) * rowCount; i++)
                {
                    if (!wheel.wheelState.IsRollLocked(i))
                    {
                        var blinkInfo = wheel.wheelState.GetBlinkAnimationInfo(i);
                        if (blinkInfo != null && blinkInfo.Count > 0)
                            blinkInfoForARow.Add(i);
                    }
                }
                XDebug.Log("blinkInfoForARow "+LitJson.JsonMapper.ToJsonField(blinkInfoForARow));
                if (blinkInfoForARow.Count > 0)
                {
                    bool hasBlinkAppeared = ShowBlinkAnimation(blinkInfoForARow);

                    if (!hasBlinkAppeared)
                        PlayReelStop(rollIndex);
                }
                else
                {
                    PlayReelStop(rollIndex);
                }
            }
        }
        
        public virtual bool ShowBlinkAnimation(List<int> blinkInfo)
        {
            bool blinkAnimationPlayed = false;

            if (blinkInfo != null && blinkInfo.Count > 0)
            {
                int blinkIndex = GetNeedPlayBlinkSoundRowIndex(blinkInfo);

                foreach (var item in blinkInfo)
                {
                    var container = wheel.GetRoll(item).GetVisibleContainer(0);

                    XDebug.Log($"ShowAppearAnimation:{item}:{0}");

                    container.ShiftSortOrder(true);

                    if (container.sequenceElement.config.blinkType != BlinkAnimationPlayStyleType.Default)
                    {
                        containerPlayBlinkAnimation.Add(container);
                    }

                    var appearKey = wheel.wheelName + item + item;
                    listWheelAppears.Add(appearKey);

                    if (!blinkAnimationPlayed && item == blinkIndex)
                    {
                        PlayBlinkSound(container, item, 0);   
                    }

                    container.PlayElementAnimation("Blink", false, () =>
                    {
                        OnBlinkAnimationFinished(container, appearKey,item,(int)0);
                    });

                    blinkAnimationPlayed = true;
                }
            }

            return blinkAnimationPlayed;
        }

        protected override void OnBlinkAnimationFinished(ElementContainer container, string appearKey,int rollIndex,int rowIndex)
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
        }
        public override int GetBlinkSoundIndex(ElementConfig elementConfig, int rollIndex)
        {
            var index = 0;
            var rollRowCount = wheel.wheelState.GetWheelConfig().rollRowCount;
           
            var listVariantId = wheel.GetContext().elementExtraInfoProvider.GetElementBlinkVariantList(elementConfig.id);

            int i = 0;
            while(i < rollIndex)
            {
                bool findElement = false;
                foreach (var variantId in listVariantId)
                {
                    if (wheel.wheelState.IsSpinResultContainElementAtRollIndex(variantId, i))
                    {
                       
                        findElement = true;
                        break;
                    }
                }
                
                if (findElement)
                {
                    index++;
                   i = (i / rollRowCount + 1) * rollRowCount;
                }
                else
                {
                    i++;
                }
            }

            return index;
        }

        public override void PlayBlinkSound(ElementContainer container, int rollIndex, int rowIndex)
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
                XDebug.Log("PlayBlinkSound:" + blinkSoundName);
                AudioUtil.Instance.PlayAudioFx(blinkSoundName);
            }
            else
            {
                blinkSoundName = config.blinkSoundName + "01";
                var freeState = container.sequenceElement.machineContext.state.Get<FreeSpinState>();
                if (freeState.IsInFreeSpin && assetProvider.GetAsset<AudioClip>(blinkSoundName+ "_Free"))
                {
                    blinkSoundName = blinkSoundName + "_Free";
                }
                XDebug.Log("PlayBlinkSound:" + blinkSoundName);
                AudioUtil.Instance.PlayAudioFx(blinkSoundName);
            }
        }
        
        protected virtual int GetNeedPlayBlinkSoundRowIndex(List<int> blinkInfo)
        {
            var blinkIndex = 0;
            var listSoundOrderId = new List<Tuple<int,int>>();

            foreach (var item in blinkInfo)
            {
                var container = wheel.GetRoll(item).GetVisibleContainer(0);
                listSoundOrderId.Add(new Tuple<int, int>(container.sequenceElement.config.blinkSoundOrderId,
                    item));
            }
          
            if (listSoundOrderId.Count > 0)
            {
                listSoundOrderId.Sort((a, b) =>  b.Item1.CompareTo(a.Item1));
                blinkIndex = listSoundOrderId[0].Item2;
            }
            return blinkIndex;
        }

    }
}