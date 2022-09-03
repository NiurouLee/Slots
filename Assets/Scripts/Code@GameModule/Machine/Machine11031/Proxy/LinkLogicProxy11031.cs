using UnityEngine;
using System.Threading.Tasks;
using Random = UnityEngine.Random;
using System.Collections.Generic;
using DragonU3DSDK.Audio;
using DragonU3DSDK.Network.API.ILProtocol;
using UnityEngine.Rendering;

namespace GameModule
{
    public class LinkLogicProxy11031 : LinkLogicProxy
    {
        ElementConfigSet elementConfigSet = null;
        private WheelsActiveState11031 wheelsActiveState;
        private ExtraState11031 _extraState11031;
        private LinkRemaining11031 linkRemaining;

        public LinkLogicProxy11031(MachineContext context)
            : base(context)
        {
            wheelsActiveState = machineContext.state.Get<WheelsActiveState11031>();
            _extraState11031 = machineContext.state.Get<ExtraState11031>();
            elementConfigSet = machineContext.state.machineConfig.elementConfigSet;
            linkRemaining = machineContext.view.Get<LinkRemaining11031>();
        }

        protected void PlayBgMusic()
        {
            if (_extraState11031.IsFourLinkMode())
            {
                AudioUtil.Instance.PlayMusic("Bg_SuperRespin_11031", true);
            }
            else
            {
                AudioUtil.Instance.PlayMusic("Bg_Respin_11031", true);
            }
        }


        protected override async void HandleCustomLogic()
        {
            //处理触发Link：开始弹板或者过场动画
            if (IsLinkTriggered())
            {
                StopBackgroundMusic();
                if (_extraState11031.IsOneLinkMode())
                {
                    //先将所有的绿色辣椒转换成红色辣椒
                    HandleChilliPlayIdle();
                    await HandleGreenChilliToRedChilli();
                    // await HandleReSpinStartLogic();
                    // await HandleLinkGameTrigger();
                }

                if (!_extraState11031.IsFourLinkMode())
                {
                    _extraState11031.SetLinkOldPanelCount(0);
                    await HandleLinkBeginPopup();
                    PlayBgMusic();
                    machineContext.view.Get<BackGroundView11031>().ShowBackground(true);
                    machineContext.view.Get<BowlView11031>().ShowBowl(false);
                    linkRemaining.ShowLinkRemains(true);
                    await linkRemaining.RefreshReSpinCount(false, false);
                    machineContext.view.Get<CollectGroupView11031>().Hide();
                    ChangeWheel(false);
                    //将jackpot的圆圈变成棕色
                    HandleWinGroupView();
                    machineContext.view.Get<JackpotPanel11031>().PLayOpenJackPotPanel();
                    await HandleLinkGameTrigger();
                    await HandleReSpinStartLogic();
                    await PlayWheelAnimation(true);
                    // await HandleLinkBeginCutSceneAnimation();
                }
                else
                {
                    _extraState11031.SetLinkOldPanelCount(0);
                    await HandleReSpinStartLogic();
                    await HandleLinkGameTrigger();
                    _extraState11031.SetNeedTransView(true);
                    ChangeWheel(false);
                    machineContext.view.Get<LinkBackLineView11031>().ShowLinkLine(true);
                    linkRemaining.ShowLinkRemains(true);
                    await linkRemaining.RefreshReSpinCount(false, false);
                    machineContext.view.Get<BowlView11031>().ShowBowl(false);
                    machineContext.view.Get<CollectGroupView11031>().Hide();
                    PayWheelIde();
                    await HandleLinkBeginPopup();
                    PlayBgMusic();
                    machineContext.view.Get<BackGroundView11031>().ShowBackground(true);
                    await HandleYellowChilliToRedChilli();
                    await HandleLinkBeginCutSceneAnimation();
                }
            }

            //处理Link逻辑：锁图标或者其他
            if (IsInRespinProcess())
            {
                //LINK处理完之后，检查是否要从单link模式切换成4LINK模式
                if (_extraState11031.FromOneLinkModeToFourLinkMode() && wheelsActiveState.GetRunningWheel().Count == 1)
                {
                    HandleWinGroupView();
                    //锁住symbol
                    await LockLinkElements(IsLinkTriggered());
                    await SettleJackpotElement(IsLinkTriggered());
                    AudioUtil.Instance.StopMusic();
                    await HandleYellowChilliPlayTrigger();
                    await HandleLinkBeginPopup();
                    PlayBgMusic();
                    _extraState11031.ClearLockElementIdsList();

                    var runningWheels = machineContext.state.Get<WheelsActiveState11031>().GetRunningWheel();

                    for (var i = 0; i < runningWheels.Count; i++)
                    {
                        if (runningWheels[i] is LinkWheel11031 wheel)
                        {
                            wheel.lockLayer.ClearAllElement();
                            wheel.lockLayer.ClearLockedYellowElementList();
                            wheel.lockLayer.ClearLockedElementList();
                        }
                    }

                    machineContext.view.Get<LinkBackLineView11031>().ShowLinkLine(true);
                    linkRemaining.ShowLinkRemains(true);
                    ChangeWheel(true);
                    PayWheelIde();
                    await HandleYellowChilliToRedChilli();
                    await LockLinkElements(true, true);
                }
                else
                {
                    await HandleLinkGame();
                }
            }

            //是否Link结束：处理结算过程
            if (IsLinkSpinFinished())
            {
                StopBackgroundMusic();
                await HandleLinkReward();
            }

            //Link结算完成，恢复Normal
            if (NeedSettle())
            {
                await HandleChillisPlayWin();
                StopBackgroundMusic();
                await HandleWinGroupPopup();
                //所有symbol播放win动画
                await HandleLinkFinishPopup();
                await HandleLinkFinishCutSceneAnimation();
                await HandleLinkHighLevelEffect();
            }

            Proceed();
        }

        protected override async Task HandleLinkFinishPopup()
        {
            if (machineContext.assetProvider.GetAsset<GameObject>(GetLinkFinishAddress()) != null)
            {
                var task = GetWaitTask();
                AudioUtil.Instance.PlayAudioFx("Respin_End");
                var finishLinkPopup = PopUpManager.Instance.ShowPopUp<ReSpinFinishPopUp11031>(GetLinkFinishAddress());
                if (!ReferenceEquals(finishLinkPopup, null))
                {
                    finishLinkPopup.SetPopUpCloseAction(() =>
                    {
                        //finishLinkPopup.Close();
                        SetAndRemoveTask(task);
                    });
                    finishLinkPopup.Initialize(machineContext);
                    if (finishLinkPopup.IsAutoClose())
                    {
                        await machineContext.WaitSeconds(GetLinkFinishPopupDuration());
                        AudioUtil.Instance.PlayAudioFx("Close");
                        finishLinkPopup.Close();
                    }
                }

                await task.Task;
            }

            await Task.CompletedTask;
        }

        public async Task HandleWinGroupPopup()
        {
            await machineContext.view.Get<WinGroupFeature11031>().Open();
            var chilliCount = machineContext.state.Get<ExtraState11031>().GetLinkPepperCount();
            // if (chilliCount >= 40)
            // {
            //     winRate = machineContext.state.Get<ExtraState11031>().GetLinkPepperJackpotPay();
            // }
            // else
            // {
            //     winRate = machineContext.state.Get<ExtraState11031>().GetLinkPepperWinRate();
            // }
            var winRate = machineContext.state.Get<ExtraState11031>().GetLinkPepperJackpotPay() +
                          machineContext.state.Get<ExtraState11031>().GetLinkPepperWinRate();

            var chips = machineContext.state.Get<BetState>().GetPayWinChips(winRate);
            AddWinChipsToControlPanel(chips,1,true,false, "Symbol_SmallWin_11031");
            await machineContext.WaitSeconds(1.0f);
        }

        protected override string GetLinkFinishAddress()
        {
            return "ReSpinFinishPopUp11031";
        }

        protected override async Task HandleLinkGame()
        {
            //wheel解锁
            await PlayWheelAnimation();
            HandleWinGroupView();
            //锁住symbol
            await LockLinkElements(IsLinkTriggered());
            await SettleJackpotElement(IsLinkTriggered());
        }

        private void HandleChilliPlayIdle()
        {
            var wheels = machineContext.state.Get<WheelsActiveState11031>().GetRunningWheel();
            var reTriggerElementContainers = wheels[0].GetElementMatchFilter((container) =>
            {
                if (Constant11031.ListExceptGreenChilliIds.Contains(container.sequenceElement.config.id))
                {
                    return true;
                }

                return false;
            });

            if (reTriggerElementContainers.Count > 0)
            {
                for (var i = 0; i < reTriggerElementContainers.Count; i++)
                {
                    reTriggerElementContainers[i].PlayElementAnimation("Idle");
                    reTriggerElementContainers[i].ShiftSortOrder(true);
                    // SortingGroup sortingGroup = reTriggerElementContainers[i].transform.GetComponent<SortingGroup>();
                    // sortingGroup.sortingOrder = 300;
                }
            }
        }

        private async Task HandleChilliPlayJumpBG()
        {
            var wheels = machineContext.state.Get<WheelsActiveState11031>().GetRunningWheel();
            var reTriggerElementContainers = wheels[0].GetElementMatchFilter((container) =>
            {
                if (Constant11031.ListYellowChilli.Contains(container.sequenceElement.config.id))
                {
                    return true;
                }

                return false;
            });

            if (reTriggerElementContainers.Count > 0)
            {
                for (var i = 0; i < reTriggerElementContainers.Count; i++)
                {
                    reTriggerElementContainers[i].PlayElementAnimation("JumpBG");
                    reTriggerElementContainers[i].ShiftSortOrder(true);
                }

                await machineContext.WaitSeconds(0.6f);
            }
        }

        private async Task HandleYellowChilliJackpot()
        {
            var wheels = machineContext.state.Get<WheelsActiveState11031>().GetRunningWheel();
            var reTriggerElementContainers = wheels[0].GetElementMatchFilter((container) =>
            {
                if (Constant11031.ListYellowChilliWithJackpot.Contains(container.sequenceElement.config.id))
                {
                    return true;
                }

                return false;
            });

            if (reTriggerElementContainers.Count > 0)
            {
                for (var i = 0; i < reTriggerElementContainers.Count; i++)
                {
                    reTriggerElementContainers[i].PlayElementAnimation("BeforeFly");
                    reTriggerElementContainers[i].ShiftSortOrder(true);
                }

                AudioUtil.Instance.PlayAudioFx("Jackpot_Alarm1");
                await machineContext.WaitSeconds(1.5f);
            }
        }

        private async Task HandleYellowChilliPlayTrigger()
        {
            var wheels = machineContext.state.Get<WheelsActiveState11031>().GetRunningWheel();
            // var reTriggerElementContainers = wheels[0].GetElementMatchFilter((container) =>
            // {
            //     if (Constant11031.ListYellowChilli.Contains(container.sequenceElement.config.id))
            //     {
            //         return true;
            //     }
            //
            //     return false;
            // });
            AudioUtil.Instance.PlayAudioFx("J20-J25_Trigger"); 
            var wheel = wheels[0] as LinkWheel11031;
            wheel.lockLayer.PlayYellowChilliTrigger();
            await machineContext.WaitSeconds(2.5f);
            // if (reTriggerElementContainers.Count > 0)
            // {
            //     for (var i = 0; i < reTriggerElementContainers.Count; i++)
            //     {
            //         reTriggerElementContainers[i].PlayElementAnimation("TriggerBG");
            //         reTriggerElementContainers[i].ShiftSortOrder(true);
            //     }
            // }
        }

        private async void PayWheelIde()
        {
            await PlayWheelAnimation(false);
        }

        private async Task PlayWheelAnimation(bool playAnimation = true)
        {
            if (!_extraState11031.IsFourLinkMode())
            {
                return;
            }

            var activeWheelCount = (int) _extraState11031.GetLinkActivePanelCount() - 1;
            var wheels = machineContext.state.Get<WheelsActiveState11031>().GetRunningWheel();
            for (var i = 0; i < wheels.Count; i++)
            {
                var wheel = wheels[i];
                Animator animatorWheel = wheels[i].transform.GetComponent<Animator>();
                if (activeWheelCount == 0)
                {
                    if (i == 0)
                    {
                        XUtility.PlayAnimation(animatorWheel, "WheelSuperRespin_ActiveIdle");
                        SortingGroup sortingGroup = wheel.transform.GetComponent<SortingGroup>();
                        sortingGroup.sortingOrder = 11 + i;
                    }
                    else
                    {
                        XUtility.PlayAnimation(animatorWheel, "WheelSuperRespin_Idle");
                        SortingGroup sortingGroup = wheel.transform.GetComponent<SortingGroup>();
                        sortingGroup.sortingOrder = 1 + 4 - i;
                    }
                }
                else
                {
                    if (i < activeWheelCount)
                    {
                        XUtility.PlayAnimation(animatorWheel, "WheelSuperRespin_ActiveIdle");
                        SortingGroup sortingGroup = wheel.transform.GetComponent<SortingGroup>();
                        sortingGroup.sortingOrder = 11 + i;
                    }
                    else if (i == activeWheelCount)
                    {
                        if (playAnimation)
                        {
                            //火车symbol先播放trans动画，播放完成后变成灰色。
                            await HandleTruckPlayTrans();
                            await HandleWheelTruckTrans();
                        }
                        else
                        {
                            XUtility.PlayAnimation(animatorWheel, "WheelSuperRespin_ActiveIdle");
                            SortingGroup sortingGroup = wheel.transform.GetComponent<SortingGroup>();
                            sortingGroup.sortingOrder = 11 + i;
                        }
                    }
                    else
                    {
                        XUtility.PlayAnimation(animatorWheel, "WheelSuperRespin_Idle");
                        SortingGroup sortingGroup = wheel.transform.GetComponent<SortingGroup>();
                        sortingGroup.sortingOrder = 1 + 4 - i;
                    }
                }
            }
        }


        private async Task HandleTruckPlayTrans()
        {
            var wheels = machineContext.state.Get<WheelsActiveState11031>().GetRunningWheel();
            for (var w = 0; w < wheels.Count; w++)
            {
                var reTriggerElementContainers = wheels[w].GetElementMatchFilter((container) =>
                {
                    if (Constant11031.TruckElementId == container.sequenceElement.config.id)
                    {
                        return true;
                    }

                    return false;
                });

                if (reTriggerElementContainers.Count > 0)
                {
                    AudioUtil.Instance.PlayAudioFxOneShot("J40_Active_Rush_Break");
                    for (var i = 0; i < reTriggerElementContainers.Count; i++)
                    {
                        reTriggerElementContainers[i].PlayElementAnimation("Trans"); 
                        reTriggerElementContainers[i].ShiftSortOrder(true);
                    }
                }
            }
            await machineContext.WaitSeconds(1.0f);
        }

        private async Task HandleChillisPlayWin()
        {
            var activeCount = _extraState11031.GetLinkActivePanelCount();
            var wheels = machineContext.state.Get<WheelsActiveState11031>().GetRunningWheel();
            for (var i = 0; i < activeCount; i++)
            {
                var wheel = wheels[i] as LinkWheel11031;
                if (i <= activeCount - 1)
                {
                    wheel.lockLayer.PlayWin();
                }
            }

            AudioUtil.Instance.PlayAudioFx("All_J_Win");
            await machineContext.WaitSeconds(2.0f);
        }

        public void ScatterToStatic()
        {
            var wheels = machineContext.state.Get<WheelsActiveState11031>().GetRunningWheel();
            for (var w = 0; w < wheels.Count; w++)
            {
                var reTriggerElementContainers = wheels[w].GetElementMatchFilter((container) =>
                {
                    if (Constant11031.TruckElementId == container.sequenceElement.config.id)
                    {
                        return true;
                    }

                    return false;
                });
                if (reTriggerElementContainers.Count > 0)
                {
                    for (var i = 0; i < reTriggerElementContainers.Count; i++)
                    {
                        reTriggerElementContainers[i].ShiftSortOrder(false);
                        SortingGroup sortingGroup =
                            reTriggerElementContainers[i].transform.GetComponent<SortingGroup>();
                        sortingGroup.sortingOrder = 45;
                    }
                }
            }
        }

        private async Task HandleWheelTruckTrans()
        {
            var activeWheelCount = _extraState11031.GetLinkActivePanelCount();
            var wheels = machineContext.state.Get<WheelsActiveState11031>().GetRunningWheel();

            for (var w = 0; w < wheels.Count; w++)
            {
                var reTriggerElementContainers = wheels[w].GetElementMatchFilter((container) =>
                {
                    if (Constant11031.TruckElementId == container.sequenceElement.config.id)
                    {
                        return true;
                    }

                    return false;
                });
                if (reTriggerElementContainers.Count > 0)
                {
                    wheels[w].transform.Find("Truck").gameObject.SetActive(true);
                    Animator animatorOldTruck = wheels[w].transform.Find("Truck").gameObject.GetComponent<Animator>();
                    XUtility.PlayAnimation(animatorOldTruck, "TrackIntro");
                    await machineContext.WaitSeconds(1.0f);
                    ScatterToStatic();
                    wheels[(int) activeWheelCount - 1].transform.Find("Truck").gameObject.SetActive(true);
                    Animator animatorNewTruck = wheels[(int) activeWheelCount - 1].transform.Find("Truck").gameObject
                        .GetComponent<Animator>();
                    XUtility.PlayAnimation(animatorNewTruck, "Trans");
                    await XUtility.PlayAnimationAsync(
                        wheels[(int) activeWheelCount - 1].transform.gameObject.GetComponent<Animator>(),
                        "WheelSuperRespin_Active");
                    SortingGroup sortingGroup =
                        wheels[(int) activeWheelCount - 1].transform.GetComponent<SortingGroup>();
                    sortingGroup.sortingOrder = 10 + (int) activeWheelCount - 1;
                    wheels[w].transform.Find("Truck").gameObject.SetActive(false);
                    wheels[(int) activeWheelCount - 1].transform.Find("Truck").gameObject.SetActive(false);
                }
            }
        }


        private async Task HandleGreenChilliToRedChilli()
        {
            var wheels = machineContext.state.Get<WheelsActiveState11031>().GetRunningWheel();
            var reTriggerElementContainers = wheels[0].GetElementMatchFilter((container) =>
            {
                if (Constant11031.ListGreenChilli.Contains(container.sequenceElement.config.id))
                {
                    return true;
                }

                return false;
            });

            if (reTriggerElementContainers.Count > 0)
            {
                for (var i = 0; i < reTriggerElementContainers.Count; i++)
                {
                    reTriggerElementContainers[i].PlayElementAnimation("Change");
                    reTriggerElementContainers[i].ShiftSortOrder(true);
                    SortingGroup sortingGroup = reTriggerElementContainers[i].transform.GetComponent<SortingGroup>();
                    sortingGroup.sortingOrder = 300 + i;
                }

                AudioUtil.Instance.PlayAudioFx("J30-J35_Change");
            }

            wheels[0].GetContext().state.Get<WheelsActiveState11031>().ShowRollsMasks(wheels[0]);
            await machineContext.WaitSeconds(4.4f);
        }

        private async Task HandleYellowChilliToRedChilli()
        {
            var wheels = machineContext.state.Get<WheelsActiveState11031>().GetRunningWheel();
            var reTriggerElementContainers = wheels[0].GetElementMatchFilter((container) =>
            {
                if (Constant11031.ListGreenChilli.Contains(container.sequenceElement.config.id) ||
                    Constant11031.ListYellowChilli.Contains(container.sequenceElement.config.id))
                {
                    return true;
                }

                return false;
            });
            if (reTriggerElementContainers.Count > 0)
            {
                for (var i = 0; i < reTriggerElementContainers.Count; i++)
                {
                    var index = i;
                    reTriggerElementContainers[index].ShiftSortOrder(true);
                    reTriggerElementContainers[index].PlayElementAnimation("Change", false,
                        () => { reTriggerElementContainers[index].ShiftSortOrder(false); });
                }
                AudioUtil.Instance.PlayAudioFx("J30-J35_Change_NoRing");
                // AudioUtil.Instance.PlayAudioFx("J20_Change");
            }

            await machineContext.WaitSeconds(4.4f);

            if (reTriggerElementContainers.Count > 0)
            {
                for (var i = 0; i < reTriggerElementContainers.Count; i++)
                {
                    var oldId = reTriggerElementContainers[i].sequenceElement.config.id;
                    uint newId = 12;
                    if (Constant11031.ListGreenChilli.Contains(oldId))
                    {
                        newId = Constant11031.ChangeGreenChilliId(oldId);
                    }
                    else if (Constant11031.ListYellowChilli.Contains((oldId)))
                    {
                        newId = Constant11031.ChangeYellowChilliId(oldId);
                    }

                    var sequenceElement = new SequenceElement(
                        elementConfigSet.GetElementConfig(newId),
                        machineContext);
                    reTriggerElementContainers[i].UpdateElement(sequenceElement);
                    reTriggerElementContainers[i].ShiftSortOrder(true);
                }
            }
        }

        protected override async Task HandleLinkBeginPopup()
        {
            if (_extraState11031.IsFourLinkMode())
            {
                await machineContext.view.Get<SuperLinkView11031>().Open();
            }
            else
            {
                var wheels = machineContext.state.Get<WheelsActiveState11031>().GetRunningWheel();
                wheels[0].GetContext().state.Get<WheelsActiveState11031>().FadeOutRollMask(wheels[0]);
                await machineContext.view.Get<LinkFeatureView11031>().Open();
            }
        }

        protected void ChangeWheel(bool isOneLinkToFourLink)
        {
            wheelsActiveState.UpdateRunningWheelState(true, false);
            RestoreTriggerWheelElement();
        }

        protected override async Task HandleLinkBeginCutSceneAnimation()
        {
            //将jackpot的圆圈变成棕色
            HandleWinGroupView();
            machineContext.view.Get<JackpotPanel11031>().PLayOpenJackPotPanel();
            //将红色的辣椒bg显示出来并锁住
            await PlayWheelAnimation(true);
        }


        protected override void RecoverLogicState()
        {
            RecoverLogicStateAsync();
        }

        protected async Task RecoverLogicStateAsync()
        {
            PlayBgMusic();
            HandleWinGroupView();
            machineContext.view.Get<BowlView11031>().Hide();
            machineContext.view.Get<BackGroundView11031>().ShowBackground(true);
            if (_extraState11031.IsFourLinkMode())
            {
                machineContext.view.Get<LinkBackLineView11031>().ShowLinkLine(true);
            }

            linkRemaining.ShowLinkRemains(true);
            linkRemaining.RefreshReSpinCount(false, false);
            machineContext.view.Get<CollectGroupView11031>().Hide();
            var activeWheelCount = _extraState11031.GetLinkActivePanelCount();
            _extraState11031.SetLinkOldPanelCount(activeWheelCount);
            PayWheelIde();
            machineContext.view.Get<JackpotPanel11031>().ShowJackPotRemind();
            await LockLinkElements(true, true,false);
            if (_extraState11031.IsFourLinkMode())
            {
                _extraState11031.SetNeedTransView(true);
            }
        }

        protected override async Task HandleLinkFinishCutSceneAnimation()
        {
            if (_extraState11031.GetNeedTransView())
            {
                machineContext.view.Get<TransitionsView11031>().PlayTruckTransition();
                _extraState11031.SetNeedTransView(false);
                await machineContext.WaitSeconds(1.66f);
                HandleWinGroupViewToBase();
                _extraState11031.SetLinkOldPanelCount(0);
                linkRemaining.ShowLinkRemains(false);
                machineContext.view.Get<BowlView11031>().ShowBowl(true);
                machineContext.view.Get<CollectGroupView11031>().Show();
                machineContext.view.Get<JackpotPanel11031>().PlayJackPotRemindIdle();
                _extraState11031.ClearLockElementIdsList();
                var runningWheels = machineContext.state.Get<WheelsActiveState11031>().GetRunningWheel();

                for (var i = 0; i < runningWheels.Count; i++)
                {
                    if (runningWheels[i] is LinkWheel11031 wheel)
                    {
                        wheel.lockLayer.ClearAllElement();
                        wheel.lockLayer.ClearLockedYellowElementList();
                        wheel.lockLayer.ClearLockedElementList();
                    }
                }

                wheelsActiveState.UpdateRunningWheelState(false, false);
                machineContext.view.Get<BackGroundView11031>().ShowBackground(false);
                machineContext.view.Get<LinkBackLineView11031>().ShowLinkLine(false);
                RestoreTriggerWheelElement();
                await machineContext.WaitSeconds(3.33f - 1.66f);
            }
            else
            {
                HandleWinGroupViewToBase();
                _extraState11031.SetLinkOldPanelCount(0);
                linkRemaining.ShowLinkRemains(false);
                machineContext.view.Get<BowlView11031>().ShowBowl(true);
                machineContext.view.Get<CollectGroupView11031>().Show();
                machineContext.view.Get<JackpotPanel11031>().PlayJackPotRemindIdle();
                _extraState11031.ClearLockElementIdsList();

                var runningWheels = machineContext.state.Get<WheelsActiveState11031>().GetRunningWheel();

                for (var i = 0; i < runningWheels.Count; i++)
                {
                    if (runningWheels[i] is LinkWheel11031 wheel)
                    {
                        wheel.lockLayer.ClearAllElement();
                        wheel.lockLayer.ClearLockedYellowElementList();
                        wheel.lockLayer.ClearLockedElementList();
                    }
                }

                machineContext.view.Get<BackGroundView11031>().ShowBackground(false);
                machineContext.view.Get<LinkBackLineView11031>().ShowLinkLine(false);
                wheelsActiveState.UpdateRunningWheelState(false, false);
                RestoreTriggerWheelElement();
            }
        }

        private async Task LockLinkElements(bool lockAll, bool isStatic = false,bool playAudio = true)
        {
            bool hasNewLinkItem = false;

            var wheels = wheelsActiveState.GetRunningWheel();

            for (var i = 0; i < wheels.Count; i++)
            {
                hasNewLinkItem = LockLinkElementOnWheel((LinkWheel11031) wheels[i], lockAll, isStatic) ||
                                 hasNewLinkItem;
            }

            if (hasNewLinkItem || lockAll)
            {
                if (playAudio)
                {
                    AudioUtil.Instance.PlayAudioFx("J01-J25_Lock");
                    await machineContext.WaitSeconds(0.6f);
                }
            }
        }

        public void Hide()
        {
        }

        private bool LockLinkElementOnWheel(LinkWheel11031 wheel, bool lockAll, bool isStatic)
        {
            List<ChilliFrenzyGameResultExtraInfo.Types.LinkData.Types.LinkItem> linkItems;
            var activePanelCount = _extraState11031.GetLinkActivePanelCount();
            if (lockAll)
                linkItems = _extraState11031.GetLinkItem(wheel.wheelIndex);
            else
            {
                linkItems = _extraState11031.GetNewLinkItem(wheel.wheelIndex);
            }

            bool hasLockNewItem = false;

            for (var i = 0; i < linkItems.Count; i++)
            {
                hasLockNewItem = true;
                if ((wheel.wheelIndex <= (int) activePanelCount - 1))
                {
                    wheel.lockLayer.LockLinkElements((int) linkItems[i].PositionId, linkItems[i].SymbolId, isStatic,
                        true);
                }
                else
                {
                    wheel.lockLayer.LockLinkElements((int) linkItems[i].PositionId, linkItems[i].SymbolId, isStatic,
                        false);
                }
            }

            return hasLockNewItem;
        }


        private async Task SettleJackpotElement(bool allLinkItem)
        {
            var activeWheelCount = _extraState11031.GetLinkActivePanelCount();
            var oldWheelCount = _extraState11031.GetLinkOldActivePanelCount();
            var wheels = machineContext.state.Get<WheelsActiveState11031>().GetRunningWheel();
            _extraState11031.ResetJackpotAccNum();
            //jackpot，只有解锁的panel才可以飞jackpot
            for (var i = 0; i < activeWheelCount; i++)
            {
                var wheel = wheels[i] as LinkWheel11031;

                if (wheel != null)
                {
                    if (i == activeWheelCount - 1)
                    {
                        if (oldWheelCount == activeWheelCount - 1)
                        {
                            allLinkItem = true;
                            _extraState11031.SetLinkOldPanelCount(activeWheelCount);
                        }
                    }

                    //这次新锁的Item
                    var newJackpotItems =
                        allLinkItem ? _extraState11031.GetLinkItem(i) : _extraState11031.GetNewLinkItem(i);

                    //清楚掉非jackpotItem
                    for (var j = newJackpotItems.Count - 1; j >= 0; j--)
                    {
                        if (newJackpotItems[j].JackpotId == 0)
                        {
                            wheel.lockLayer.PLayIdleBg(newJackpotItems[j].PositionId);
                            newJackpotItems.RemoveAt(j);
                        }
                    }

                    //直接调用Sort在低包不能支持，先手写一个排序
                    for (var c = 0; c < newJackpotItems.Count; c++)
                    {
                        for (var j = c + 1; j < newJackpotItems.Count; j++)
                        {
                            if (newJackpotItems[c].JackpotId > newJackpotItems[j].JackpotId)
                            {
                                var tempItem = newJackpotItems[c];
                                newJackpotItems[c] = newJackpotItems[j];
                                newJackpotItems[j] = tempItem;
                            }
                        }
                    }

                    await wheel.lockLayer.PlayBeforeFlyToJackPot(newJackpotItems);

                    for (var j = 0; j < newJackpotItems.Count; j++)
                    {
                        var jackpotId = newJackpotItems[j].JackpotId;
                        _extraState11031.AddJackpotNum(jackpotId);
                        AudioUtil.Instance.PlayAudioFx("J01-J25_Lock_Alarm");
                        await wheel.lockLayer.FlyToJackpot(newJackpotItems[j].PositionId, newJackpotItems[j].JackpotId);
                        if (_extraState11031.GetJackpotAccNum(jackpotId) % 3 == 0)
                        {
                            await ShowJackPotPopUp(jackpotId);
                        }
                    }
                }
            }
        }

        private async Task ShowJackPotPopUp(uint jackpotId)
        {
            var jackpotPay = _extraState11031.GetJackpotPay(jackpotId);
            ulong chips = machineContext.state.Get<BetState>().GetPayWinChips(jackpotPay);
            AudioUtil.Instance.StopMusic();
            await Constant11031.ShowJackpot(machineContext, jackpotId, chips);
            PlayBgMusic();
            AddWinChipsToControlPanel(chips,1,true,false, "Symbol_SmallWin_11031");
            await machineContext.WaitSeconds(1.0f);
            ReStartShowJackPotPanel((int) jackpotId);
        }

        private void ReStartShowJackPotPanel(int jackPotId)
        {
            machineContext.view.Get<JackpotPanel11031>().ReStartShowJackPotPanel(jackPotId);
        }

        private void HandleWinGroupView()
        {
            //中奖的辣椒数量
            machineContext.view.Get<WinGroupView11031>().HideAllHighLight();
            machineContext.view.Get<WinGroupView11031>().PlayBiggerNumIdle();
            machineContext.view.Get<WinGroupView11031>().ShowLinkHighLight();
            machineContext.view.Get<WinGroupView11031>().PlayLinkBiggerNum();
        }

        private void HandleWinGroupViewToBase()
        {
            machineContext.view.Get<WinGroupView11031>().HideAllHighLight();
            machineContext.view.Get<WinGroupView11031>().PlayBiggerNumIdle();
        }

        protected override void RestoreTriggerWheelElement()
        {
            var wheels = machineContext.state.Get<WheelsActiveState11031>().GetRunningWheel();
            var triggerPanels = machineContext.state.Get<ReSpinState>().triggerPanels;
            for (var i = 0; i < wheels.Count; i++)
            {
                var wheel = wheels[i];
                if (triggerPanels != null && triggerPanels.Count > 0)
                {
                    if (wheel != null)
                    {
                        if (triggerPanels[i] != null)
                        {
                            wheel.wheelState.UpdateWheelStateInfo(triggerPanels[i]);
                            wheel.ForceUpdateElementOnWheel();
                        }
                    }
                }
            }
        }
    }
}