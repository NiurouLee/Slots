//  **********************************************
//  Copyright(c) 2021 by com.ustar
//  All right reserved
// 
//  Author : Besure.Chen
//  Date :2021-10-16 14:26
//  Ver : 1.0.0
//  Description : WheelAnimationController11010.cs
//  ChangeLog :
//  **********************************************

using System;
using System.Collections.Generic;
using Google.ilruntime.Protobuf.Collections;
using UnityEngine;

namespace GameModule
{
    public class WheelAnimationController11010: WheelAnimationController
    {
        private List<List<int>> _listLinkElementCoord;
        public override void ShowAnticipationAnimation(int rollIndex)
        {
            base.ShowAnticipationAnimation(rollIndex);
            CheckAndPlayElementAnticipation();
        }

        private async void CheckAndPlayElementAnticipation()
        {
            if (wheel.wheelState.playerQuickStopped) return;
            await XUtility.WaitSeconds(0.5f);
            _listLinkElementCoord = CanPlayElementAnticipation();
            if (_listLinkElementCoord.Count > 0)
            {
                for (int i = 0; i < _listLinkElementCoord.Count; i++)
                {
                    var col = _listLinkElementCoord[i][0];
                    var row = _listLinkElementCoord[i][1];
                    var container = wheel.GetRoll(col).GetVisibleContainer(row);
                    container.PlayElementAnimation("Blink2");
                }
            }
        }

        public override void OnAllRollSpinningStopped(Action callback)
        {
            if (_listLinkElementCoord != null && _listLinkElementCoord.Count > 0)
            {
                for (int i = 0; i < _listLinkElementCoord.Count; i++)
                {
                    var col = _listLinkElementCoord[i][0];
                    var row = _listLinkElementCoord[i][1];
                    var container = wheel.GetRoll(col).GetVisibleContainer(row);
                    container.UpdateAnimationToStatic();
                }
                _listLinkElementCoord.Clear();
                _listLinkElementCoord = null;
            }
            base.OnAllRollSpinningStopped(callback);
        }

        private List<List<int>> CanPlayElementAnticipation()
        {
            List<List<int>> listIds = new List<List<int>>();
            bool needPlay = false;
            for (int row = 0; row < 3; row++)
            {
                var count = 0;
                List<List<int>> tmpIds = new List<List<int>>();
                for (int col = 0; col < 5; col++)
                {
                    var blinkInfo = wheel.wheelState.GetBlinkAnimationInfo(col);
                    if (blinkInfo !=null && blinkInfo.Count>0)
                    {
                        for (int j = 0; j < blinkInfo.count; j++)
                        {
                            var container = wheel.GetRoll(col).GetVisibleContainer((int) blinkInfo[j]);
                            if (blinkInfo[j] == row && Constant11010.IsLinkElement(container.sequenceElement.config.id))
                            {
                                count++;
                                tmpIds.Add(new List<int>{col,row});
                            }
                        }
                    }   
                }

                if (count >= 2)
                {
                    listIds.AddRange(tmpIds);
                }
            }
            return listIds;
        }
        
        public override bool ShowBlinkAnimation(RepeatedField<uint> blinkInfo, int rollIndex)
        {
            bool blinkAnimationPlayed = false;

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
                            if (!wheel.wheelName.Contains("Link"))
                            {
                                container.UpdateAnimationToStatic();
                                container.ShiftSortOrder(false);   
                            }
                        }
                        else
                        {
                            containerPlayBlinkAnimation.Remove(container);
                        }

                        if (containerPlayBlinkAnimation.Count == 0 && canCheckBlinkFinished)
                        {
                            if (!wheel.wheelName.Contains("Link"))
                            {
                                CheckAndStopBlinkAnimation();
                            }
                        }

                        CheckAllBlinkFinished();
                    });

                    blinkAnimationPlayed = true;
                }
            }

            return blinkAnimationPlayed;
        }
        public override void PlayBlinkSound(ElementContainer container, int rollIndex, int rowIndex)
        {
            var config = container.sequenceElement.config;

            var blinkSoundName = config.blinkSoundName + "0" + (GetBlinkSoundIndex(config, rollIndex) + 1);

            if (assetProvider.GetAsset<AudioClip>(blinkSoundName))
            {
                AudioUtil.Instance.PlayAudioFx(blinkSoundName);
            }
            else
            {
                AudioUtil.Instance.PlayAudioFx(config.blinkSoundName + "01");
                var blinkName = "J01_Blink_Base";
                if (Constant11010.IsLinkElement(config.id))
                {
                    if (wheel.GetContext().state.Get<WheelsActiveState11010>().isLinkWheel)
                    {
                        blinkName = "J01_Blink_Link";  
                    }
                    AudioUtil.Instance.PlayAudioFx(blinkName);         
                }
                else
                {
                    AudioUtil.Instance.PlayAudioFx(config.blinkSoundName + "01");   
                }
            }
        }
    }
}