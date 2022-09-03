using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.Rendering;
using System;
using System.Collections.Generic;
using System.Runtime.Remoting.Contexts;
using System.Threading.Tasks;
using DG.Tweening;
using DragonU3DSDK.Network.API.ILProtocol;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace GameModule
{
    public class WheelStopSpecialEffectProxy11031 : WheelStopSpecialEffectProxy
    {
        private ExtraState11031 _extraState11031;
        private ReSpinState _reSpinState;

        public WheelStopSpecialEffectProxy11031(MachineContext context)
            : base(context)
        {
            _reSpinState = machineContext.state.Get<ReSpinState>();
            _extraState11031 = machineContext.state.Get<ExtraState11031>();
        }

        protected override async void HandleCustomLogic()
        {
            var bonusLine = false;
            var jpLine = false;
            var wheel = machineContext.state.Get<WheelsActiveState11031>().GetRunningWheel()[0];
            var bonusWinLines = wheel.wheelState.GetBonusWinLine();
            if (bonusWinLines.Count > 0)
            {
                bonusLine = true;
            }
            
            var normalWinLines = wheel.wheelState.GetNormalWinLine();
            if (normalWinLines.Count > 0)
            {
                for (var i = 0; i < normalWinLines.Count; i++)
                {
                    if (normalWinLines[i].JackpotId == 6)
                    {
                        jpLine = true;
                    }
                }
            }

            if (_extraState11031.GetMapIsStarted() || _reSpinState.ReSpinTriggered || bonusLine ||
                _extraState11031.IsMapLevelUp() || jpLine)
            {
                //收集辣椒
                await StartCollectChillis();
                await BlinkBonusLine();
                await BlinkJackpotLine();
            }
            else
            {
                if (!_reSpinState.IsInRespin)
                {
                    CollectChillis();
                }

                await machineContext.view.Get<LinkRemaining11031>().RefreshReSpinCount(false);
            }

            base.HandleCustomLogic();
        }

        public async Task BlinkBonusLine()
        {
            var wheel = machineContext.state.Get<WheelsActiveState11031>().GetRunningWheel()[0];
            var bonusWinLines = wheel.wheelState.GetBonusWinLine();
            if (bonusWinLines.Count > 0)
            {
                AudioUtil.Instance.PlayAudioFxOneShot("Jackpot_Collect");
                machineContext.view.Get<LightView11031>().ShowLight(true);
                await machineContext.view.Get<LightView11031>().ShowLightView();
                //winGoup扫光 数字变大
                wheel.GetContext().view.Get<WinGroupView11031>().ShowHighLight();
                wheel.GetContext().view.Get<WinGroupView11031>().PlayBaseBiggerNum();
                if (!_reSpinState.IsInRespin && !_reSpinState.ReSpinTriggered && bonusWinLines[0].Pay > 0)
                {
                    AudioUtil.Instance.PlayAudioFx("All_J_Win");
                    for (int i = 0; i < bonusWinLines.Count; i++)
                    {
                        for (var index = 0; index < bonusWinLines[i].Positions.Count; index++)
                        {
                            var pos = bonusWinLines[i].Positions[index];
                            var container = wheel.GetWinLineElementContainer((int) pos.X, (int) pos.Y);
                            container.PlayElementAnimation("BaseWin");
                            container.ShiftSortOrder(true);
                        }
                    }
                }
                await machineContext.WaitSeconds(0.2f);
                machineContext.view.Get<LightView11031>().ShowLight(false);
                await machineContext.WaitSeconds(1.83f - 0.2f);
                if (!_reSpinState.IsInRespin && !_reSpinState.ReSpinTriggered && bonusWinLines[0].Pay > 0)
                {
                    await machineContext.view.Get<WinGroupFeature11031>().InBaseOpen((int) bonusWinLines[0].Pay);
                    for (int i = 0; i < bonusWinLines.Count; i++)
                    {
                        for (var index = 0; index < bonusWinLines[i].Positions.Count; index++)
                        {
                            var pos = bonusWinLines[i].Positions[index];
                            var container = wheel.GetWinLineElementContainer((int) pos.X, (int) pos.Y);
                            container.UpdateAnimationToStatic();
                            container.ShiftSortOrder(false);
                        }
                    }
                }
            }
        }

        public async Task BlinkJackpotLine()
        {
            var wheel = machineContext.state.Get<WheelsActiveState11031>().GetRunningWheel()[0];
            var normalWinLines = wheel.wheelState.GetNormalWinLine();
            if (normalWinLines.Count > 0)
            {
                for (var i = 0; i < normalWinLines.Count; i++)
                {
                    if (normalWinLines[i].JackpotId == 6)
                    {
                        wheel.GetContext().view.Get<WinGroupView11031>().PlayBaseBiggestNum();
                        for (var index = 0; index < normalWinLines[i].Positions.Count; index++)
                        {
                            var pos = normalWinLines[i].Positions[index];
                            var container = wheel.GetWinLineElementContainer((int) pos.X, (int) pos.Y);
                            container.PlayElementAnimation("Trigger");
                            container.ShiftSortOrder(true);
                        }
                        AudioUtil.Instance.PlayAudioFxOneShot("B01_Trigger");
                        await machineContext.WaitSeconds(3.0f); 
                        await machineContext.view.Get<WinGroupFeature11031>().InBaseHighOpen((int) normalWinLines[i].Pay);
                    }
                }
                
            }
        }

        private async void CollectChillis()
        {
            await StartCollectChillis();
        }

        private async Task StartCollectChillis()
        {
            bool audioPlayed = false;
            var wheels = machineContext.state.Get<WheelsActiveState11031>().GetRunningWheel();
            var reTriggerElementContainers = wheels[0].GetElementMatchFilter((container) =>
            {
                if (Constant11031.ListChilliIds.Contains(container.sequenceElement.config.id))
                {
                    return true;
                }

                return false;
            });
            if (reTriggerElementContainers.Count > 0)
            {
                AudioUtil.Instance.PlayAudioFxOneShot("J_All_Fly");
                for (var i = 0; i < reTriggerElementContainers.Count; i++)
                {
                    var chilliIcon = machineContext.assetProvider.InstantiateGameObject("FlyCollect");
                    chilliIcon.transform.parent = machineContext.transform;
                    if (!chilliIcon.GetComponent<SortingGroup>())
                    {
                        var sortingGroup = chilliIcon.gameObject.AddComponent<SortingGroup>();
                        sortingGroup.sortingLayerID = SortingLayer.NameToID("LocalFx");
                        sortingGroup.sortingOrder = 99;
                    }
                    var targetPosition = machineContext.view.Get<CollectGroupView11031>().GetIntegralPos();
                    chilliIcon.transform.position = reTriggerElementContainers[i].transform.position;
                    machineContext.WaitSeconds(0f, () =>
                    {
                        // if (!audioPlayed)
                        //     AudioUtil.Instance.PlayAudioFxOneShot("J_All_Fly");
                        // audioPlayed = true;
                        XUtility.Fly(chilliIcon.transform, chilliIcon.transform.position, targetPosition, 0, 0.5f,
                            null);
                        _extraState11031.SetFlyChilliList(chilliIcon);
                    });
                }
                await machineContext.WaitSeconds(0.5f);
                AudioUtil.Instance.PlayAudioFxOneShot("J_Response");
                await machineContext.view.Get<CollectGroupView11031>().ChangeFill(true, true);
            }
        }
    }
}