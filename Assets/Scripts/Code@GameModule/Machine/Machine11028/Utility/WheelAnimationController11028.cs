//  **********************************************
//  Copyright(c) 2021 by com.ustar
//  All right reserved
// 
//  Author : Besure.Chen
//  Date :2022-01-18 16:43
//  Ver : 1.0.0
//  Description : WheelAnimationController11028.cs
//  ChangeLog :
//  **********************************************

using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

namespace GameModule
{
    public class WheelAnimationController11028:WheelAnimationController
    {
        private bool isPlayingAnticipaton;
        private string anticipationName = string.Empty;
        public override void CheckAndStopAllElementAnticipation()
        {
            var extraState = wheel.GetContext().state.Get<ExtraState>();
            var freeState = wheel.GetContext().state.Get<FreeSpinState>();
            if (!(extraState.HasBonusGame() || 
                  extraState.HasSpecialBonus() || 
                  freeState.IsTriggerFreeSpin || 
                  freeState.IsInFreeSpin && freeState.NewCount > 0))
            {
                StopAllElementAnticipation();  
            }
        }

        public override void ShowAnticipationAnimation(int rollIndex)
        {
            if (!isPlayingAnticipaton)
            {
                isPlayingAnticipaton = true;
                wheel.GetContext().view.Get<BackgroundView11028>().PlayBackgroundAnimation("Open");   
            }
            
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
                        sortingGroup.sortingOrder = 400;
                    }

                    wheel.AttachGameObject(GetAnticipationName(rollIndex), anticipationAnimationGo);
                }
            }

            if (anticipationAnimationGo)
            {
                anticipationAnimationGo.SetActive(true);

                XDebug.Log("PlayAnticipationSound:" + wheel.wheelState.GetAnticipationSoundAssetName());
                
                anticipationAnimationGo.transform.localPosition = wheel.GetAnticipationAnimationPosition(rollIndex);
                AudioUtil.Instance.StopAudioFx(wheel.wheelState.GetAnticipationSoundAssetName());
                AudioUtil.Instance.PlayAudioFx(wheel.wheelState.GetAnticipationSoundAssetName());
            }
        }

        public override void OnRollSpinningStopped(int rollIndex, Action rollLogicEnd)
        {
            base.OnRollSpinningStopped(rollIndex, rollLogicEnd);
            var roll = wheel.GetRoll(rollIndex);
            for (int i = 0; i < roll.containerCount; i++)
            {
                var element = roll.GetContainer(i).GetElement();
                if (element.isStaticElement && element.sequenceElement.config.id == Constant11028.B01)
                {
                    element.PlayAnimation("Out",true);
                }
            }
        }

        public override void OnAllRollSpinningStopped(Action callback)
        {
            anticipationName = string.Empty;
            base.OnAllRollSpinningStopped(callback);
        }

        public override void OnRollEnterSlowDown(int rollIndex)
        {
            base.OnRollEnterSlowDown(rollIndex);
            if (rollIndex == wheel.rollCount - 1 && isPlayingAnticipaton)
            {
                isPlayingAnticipaton = false;
                wheel.GetContext().view.Get<BackgroundView11028>().PlayBackgroundAnimation("Close");   
            }
        }

        public override void StopAnticipationAnimation(bool playStopSound = true)
        {
            var anticipationAnimationGo = wheel.GetAttachedGameObject("AnticipationAnimation");

            if (anticipationAnimationGo != null)
            {
                anticipationAnimationGo.SetActive(false);
                AudioUtil.Instance.StopAudioFx(wheel.wheelState.GetAnticipationSoundAssetName());
            }
            
            anticipationAnimationGo = wheel.GetAttachedGameObject("AnticipationAnimation01");
            if (anticipationAnimationGo != null)
            {
                anticipationAnimationGo.SetActive(false);
                AudioUtil.Instance.StopAudioFx(wheel.wheelState.GetAnticipationSoundAssetName());
            }
        }

        private string GetAnticipationName(int rollIndex)
        {
            int rapidCount = 0;
            int scatterCount = 0;
            for (int i = 0; i < rollIndex; i++)
            {
                var animationInfo = wheel.wheelState.GetBlinkAnimationInfo(i);
                for (int j = 0; j < animationInfo.Count; j++)
                {
                    var container = wheel.GetRoll(i).GetVisibleContainer((int) animationInfo[j]);
                    if (Constant11028.B01 == container.sequenceElement.config.id)
                    {
                        scatterCount++;
                    }

                    if (Constant11028.RapidHit.Contains(container.sequenceElement.config.id))
                    {
                        rapidCount++;
                    }
                }
            }

            if (string.IsNullOrEmpty(anticipationName))
            {
                anticipationName = scatterCount >= 2 ? "AnticipationAnimation" : "AnticipationAnimation01";
            }
            return anticipationName;
        }
    }
}