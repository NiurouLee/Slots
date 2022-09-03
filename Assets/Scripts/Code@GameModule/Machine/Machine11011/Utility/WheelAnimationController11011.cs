//  **********************************************
//  Copyright(c) 2021 by com.ustar
//  All right reserved
// 
//  Author : Besure.Chen
//  Date :2021-10-26 19:22
//  Ver : 1.0.0
//  Description : WheelAnimationController11011.cs
//  ChangeLog :
//  **********************************************

using System;
using Google.ilruntime.Protobuf.Collections;
using UnityEngine;

namespace GameModule
{
    public class WheelAnimationController11011: WheelAnimationController
    {
        public override void OnRollSpinningStopped(int rollIndex, Action rollLogicEnd)
        {
            PlayWildCollect(rollIndex);
            base.OnRollSpinningStopped(rollIndex, rollLogicEnd);
        }

        private async void PlayWildCollect(int rollIndex)
        {
            var machineContext = wheel.GetContext();
            var winLines = wheel.wheelState.GetBonusWinLine();
            if (winLines.Count>0 && winLines[winLines.Count-1].BonusGameId == 3)
            {
                var found = false;
                bool curIsFull = machineContext.view.Get<CollectCoinView11011>().IsFull;
                var winLine = winLines[winLines.Count - 1];
                var lastRollIndex = winLine.Positions[winLine.Positions.Count - 1].X;
                for (int i = 0; i < winLine.Positions.count; i++)
                {
                    var position = winLine.Positions[i];
                    if (position.X != rollIndex)
                        continue;
                    var elementContainer = wheel.GetRoll((int) position.X).GetVisibleContainer((int)position.Y);
                    var endPos = machineContext.view.Get<CollectCoinView11011>().GetCollectEndPosition();

                    var flyName = "CoinFly";
                    AudioUtil.Instance.PlayAudioFx("WildCollect");
                    var element = elementContainer.GetElement() as Element11011;
                    var flyContainer = GetFlyContainer(flyName, element);
                    XUtility.PlayAnimation(flyContainer.GetComponent<Animator>(), "Fly");
                    elementContainer.PlayElementAnimation("Blink");
                    element = elementContainer.GetElement() as Element11011;
                    var startPos = element.GetStartWorldPos();
                    await machineContext.WaitSeconds(0.5f);
                    await XUtility.FlyAsync(flyContainer.transform, startPos, endPos, 0, 0.5f);
                    machineContext.assetProvider.RecycleGameObject(flyName,flyContainer.gameObject);
                    machineContext.view.Get<CollectCoinView11011>().PlayCollect(curIsFull);   
                    AudioUtil.Instance.PlayAudioFx("JuBaoPen_Light");
                }

                if (lastRollIndex == rollIndex)
                {
                    await machineContext.WaitSeconds(0.5f);
                    curIsFull = machineContext.view.Get<CollectCoinView11011>().IsFull;
                    var isFull = machineContext.state.Get<ExtraState11011>().Exaggerated;
                    if (!curIsFull && isFull)
                    {
                        machineContext.view.Get<CollectCoinView11011>().PlayToFull();
                    }
                }
            }
        }

        private Transform GetFlyContainer(string flyName, Element11011 element)
        {
            var machineContext = wheel.GetContext();
            var startPos = element.GetStartWorldPos();
            var flyContainer = wheel.GetContext().assetProvider.InstantiateGameObject(flyName,true);
            flyContainer.transform.position = new Vector3(startPos.x, startPos.y, startPos.z);
            flyContainer.transform.SetParent(machineContext.transform,false);
            if (flyContainer.GetComponent<Animator>())
            {
                XUtility.PlayAnimation(flyContainer.GetComponent<Animator>(), "Fly");
            }
            return flyContainer.transform;
        }

        public override bool ShowBlinkAnimation(RepeatedField<uint> blinkInfo, int rollIndex)
        {
            var blinkAnimationPlayed = base.ShowBlinkAnimation(blinkInfo, rollIndex);
            if (wheel.GetContext().state.Get<WheelsActiveState11011>().IsLinkWheel &&  blinkInfo != null && blinkInfo.Count > 0)
            {
                for (var i = 0; i < blinkInfo.Count; i++)
                {
                    var lockRoll = wheel.GetRoll(rollIndex) as SoloRoll;
                    lockRoll.ShiftRollMaskAndSortOrder(2100);
                }
            }
            return blinkAnimationPlayed;
        }
    }
}