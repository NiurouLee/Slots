using System;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

namespace GameModule
{
    public class SubRoundStartProxy11029 : SubRoundStartProxy
    {
        private ExtraState11029 _extraState11029;
        private FreeSpinState _freeSpinState;

        public SubRoundStartProxy11029(MachineContext context)
            : base(context)
        {
        }

        public override void SetUp()
        {
            base.SetUp();
            _extraState11029 = machineContext.state.Get<ExtraState11029>();
            _freeSpinState = machineContext.state.Get<FreeSpinState>();
        }

        protected override void PlayBgMusic()
        {
            if (machineContext.state.Get<ReSpinState>().NextIsReSpin)
            {
                AudioUtil.Instance.PlayMusic(machineContext.machineConfig.audioConfig.GetLinkBackgroundMusicName());
            }
            else if (machineContext.state.Get<FreeSpinState>().NextIsFreeSpin)
            {
                if (machineContext.state.Get<FreeSpinState>().freeSpinId == 0 ||
                    machineContext.state.Get<FreeSpinState>().freeSpinId == 1)
                {
                    AudioUtil.Instance.PlayMusic("Bg_FreeGame_11029", true);
                }
                else if (machineContext.state.Get<FreeSpinState>().freeSpinId == 2)
                {
                    AudioUtil.Instance.StopMusic();
                }
                else
                {
                    AudioUtil.Instance.PlayMusic("Bg_FreeGame_Map_11029", true);
                }
            }
            else
            {
                AudioUtil.Instance.PlayMusic(machineContext.machineConfig.audioConfig.GetBaseBackgroundMusicName());
            }
        }

        protected override async void HandleCustomLogic()
        {
            if (_extraState11029.GetIsDrag())
            {
                await DragToBase();
            }

            if (_freeSpinState.IsInFreeSpin && machineContext.state.Get<FreeSpinState>().freeSpinId == 5 &&
                machineContext.state.Get<FreeSpinState>().NextIsFreeSpin)
            {
                var wheelsSpinningProxy11029 =
                    machineContext.GetLogicStepProxy(LogicStepType.STEP_WHEEL_SPINNING) as WheelsSpinningProxy11029;
                wheelsSpinningProxy11029._layerMapGameRandomWild1.ShowMapGame1StickyWildElement();
                wheelsSpinningProxy11029._layerMapGameRandomWild2.ShowMapGame2StickyWildElement();
                wheelsSpinningProxy11029._layerMapGameRandomWild3.ShowMapGame3StickyWildElement();
            }
            else if (machineContext.state.Get<FreeSpinState>().IsInFreeSpin &&
                     machineContext.state.Get<FreeSpinState>().freeSpinId == 4 &&
                     machineContext.state.Get<FreeSpinState>().NextIsFreeSpin)
            {
                var wheelsSpinningProxy11029 =
                    machineContext.GetLogicStepProxy(LogicStepType.STEP_WHEEL_SPINNING) as WheelsSpinningProxy11029;
                wheelsSpinningProxy11029._layerMapGameRandomWild1.ShowMapGame1WildElement();
                wheelsSpinningProxy11029._layerMapGameRandomWild2.ShowMapGame2WildElement();
                wheelsSpinningProxy11029._layerMapGameRandomWild3.ShowMapGame3WildElement();
            }
            else if (machineContext.state.Get<FreeSpinState>().IsInFreeSpin &&
                     machineContext.state.Get<FreeSpinState>().freeSpinId == 2 &&
                     machineContext.state.Get<FreeSpinState>().NextIsFreeSpin)
            {
                AudioUtil.Instance.StopMusic();
            }
            else if ((machineContext.state.Get<FreeSpinState>().freeSpinId == 6 ||
                      machineContext.state.Get<FreeSpinState>().freeSpinId == 7 ||
                      machineContext.state.Get<FreeSpinState>().freeSpinId == 8) &&
                     machineContext.state.Get<FreeSpinState>().IsInFreeSpin &&
                     machineContext.state.Get<FreeSpinState>().NextIsFreeSpin)
            {
                var wheels = machineContext.state.Get<WheelsActiveState11029>().GetRunningWheel();
                if (machineContext.state.Get<FreeSpinState>().LeftCount !=
                    machineContext.state.Get<FreeSpinState>().TotalCount)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        var wheel = machineContext.state.Get<WheelsActiveState11029>().GetRunningWheel()[i];
                        var roll = wheel.GetRoll((int) 2);
                        var container = roll.GetVisibleContainer(1);
                        for (int k = 0; k < roll.rowCount; k++)
                        {
                            if (k != 1)
                            {
                                var elementContainer = roll.GetVisibleContainer(k);
                                elementContainer.transform.gameObject.SetActive(false);
                            }
                        }

                        roll.transform.gameObject.SetActive(true);
                        container.transform.gameObject.SetActive(true);
                        container.PlayElementAnimation("Close");
                    }

                    AudioUtil.Instance.PlayAudioFx("Map_Wild_Open");
                    await machineContext.WaitSeconds(0.5f);
                    for (int i = 0; i < wheels.Count; i++)
                    {
                        wheels[i].transform.Find("Wild").gameObject.SetActive(true);
                    }

                    for (int i = 0; i < wheels.Count; i++)
                    {
                        machineContext.state.Get<WheelsActiveState11029>().HideTwoRoll(wheels[i]);
                    }
                }
                else
                {
                    for (int i = 0; i < wheels.Count; i++)
                    {
                        wheels[i].transform.Find("Wild").gameObject.SetActive(true);
                    }
                }

                machineContext.state.Get<WheelsActiveState11029>().UpdateElementContainerSize(true);
            }
            else if (machineContext.state.Get<FreeSpinState>().freeSpinId == 1 &&
                     machineContext.state.Get<FreeSpinState>().IsInFreeSpin &&
                     machineContext.state.Get<FreeSpinState>().NextIsFreeSpin)
            {
                ScatterToStatic();
            }

            base.HandleCustomLogic();
        }

        private async Task DragToBase()
        {
            if (_extraState11029.GetIsDrag())
            {
                if (_freeSpinState.NextIsFreeSpin)
                {
                    var wheelsSpinningProxy11029 =
                        machineContext.GetLogicStepProxy(LogicStepType.STEP_WHEEL_SPINNING) as
                            WheelsSpinningProxy11029;
                    var wheel = machineContext.view.Get<Wheel>("WheelMagicBonusGame");

                    //断线重连回来，处于小轮盘状态，不需要在进行变化操作
                    if (wheel.GetRoll(1).rowCount == 3)
                    {
                        return;
                    }

                    wheel.transform.Find("spiningMask").gameObject.SetActive(false);
                    //美杜莎更换长图和轮盘拉升同时进行
                    wheelsSpinningProxy11029._layerBonusFree.PlayInClose();
                    var wheelState = wheel.wheelState;
                    // wheelState.UpdateCurrentActiveSequence("RealGemReels");
                    // wheel.ForceUpdateElementOnWheel(true, true);
                    //轮盘上升
                    AudioUtil.Instance.PlayAudioFx("BonusGame_DownRaise");
                    Animator animatorWheel = wheel.transform
                        .GetComponent<Animator>();
                    XUtility.PlayAnimation(animatorWheel, "Close");

                    wheel.UpdateAnimationToStatic();

                    await machineContext.WaitSeconds(1.0f);

                    wheelsSpinningProxy11029._layerBonusFree.RecyleBigScatterElement();
                    machineContext.view.Get<LightView11029>().ShowLight(true);
                    wheelState.GetWheelConfig().extraTopElementCount = 3;
                    wheelState.GetWheelConfig().rollRowCount = 3;

                    if (wheel.rollCount > 0)
                    {
                        for (var i = 1; i < wheel.rollCount; i++)
                        {
                            wheel.GetRoll(i).ChangeRowCount(3);
                            wheel.GetRoll(i).ChangeExtraTopElementCount(3);
                        }
                    }
                    await machineContext.WaitSeconds(0.33f);
                    machineContext.view.Get<MeiDuSha11029>().ShowMeiDuSha(true);
                    machineContext.view.Get<MeiDuSha11029>().PlayOpenMeiDuSha();
                    AudioUtil.Instance.PlayAudioFx("BonusGame_Character_Back");
                }
                else if (!_freeSpinState.IsInFreeSpin && ChangeDragBefore())
                {
                    var wheel = machineContext.state.Get<WheelsActiveState11029>().GetRunningWheel()[0];
                    machineContext.state.Get<WheelsActiveState11029>().FadeOutRollMask(wheel);
                    var wheelsSpinningProxy11029 =
                        machineContext.GetLogicStepProxy(LogicStepType.STEP_WHEEL_SPINNING) as
                            WheelsSpinningProxy11029;
                    wheelsSpinningProxy11029._layerBase.PlayInClose();
                    await machineContext.WaitSeconds(1.0f);
                    machineContext.view.Get<MeiDuSha11029>().ShowMeiDuSha(true);
                    machineContext.view.Get<MeiDuSha11029>().PlayOpenMeiDuSha();
                    AudioUtil.Instance.PlayAudioFx("BonusGame_Character_Back");
                    wheelsSpinningProxy11029._layerBase.RecyleBigScatterElement();
                }
            }
        }

        public bool ChangeDragBefore()
        {
            int index = 0;
            var wheel = machineContext.state.Get<WheelsActiveState11029>().GetRunningWheel()[0];
            var roll = wheel.GetRoll(0);
            for (int k = 0; k < roll.containerCount; k++)
            {
                var elementContainer = roll.GetVisibleContainer(k);
                if (elementContainer.sequenceElement.config.id == Constant11029.StarElementId)
                {
                    index++;
                }
            }

            if (index >= 3)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void ScatterToStatic()
        {
            var wheels = machineContext.state.Get<WheelsActiveState>().GetRunningWheel();
            var reTriggerElementContainers = wheels[0].GetElementMatchFilter((container) =>
            {
                if (Constant11029.ListJSymbolElementIds.Contains(container.sequenceElement.config.id))
                {
                    return true;
                }

                return false;
            });

            if (reTriggerElementContainers.Count > 0)
            {
                for (var i = 0; i < reTriggerElementContainers.Count; i++)
                {
                    reTriggerElementContainers[i].UpdateAnimationToStatic();
                    reTriggerElementContainers[i].ShiftSortOrder(false);
                }
            }
        }
    }
}