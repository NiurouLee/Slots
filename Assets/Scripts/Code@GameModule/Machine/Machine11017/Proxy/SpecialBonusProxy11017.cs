using DG.Tweening;
using UnityEngine;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace GameModule
{
    public class SpecialBonusProxy11017 : SpecialBonusProxy
    {
        public SpecialBonusProxy11017(MachineContext context) : base(context)
        {
        }

        protected override void RegisterInterestedWaitEvent()
        {
            waitEvents.Add(WaitEvent.WAIT_WIN_NUM_ANIMTION);
            waitEvents.Add(WaitEvent.WAIT_BLINK_ALL_WIN_LINE);
        }

        protected override async void HandleCustomLogic()
        {
            if (machineContext.state.Get<AutoSpinState>().IsAutoSpin)
            {
                machineContext.view.Get<ControlPanel>().ShowAutoStopButton(false);
            }

            machineContext.view.Get<SuperFreeGameIcon11017>().HideSuperFreeIcon();
            if (machineContext.state.Get<FreeSpinState>().IsInFreeSpin)
            {
                uint level = machineContext.state.Get<ExtraState11017>().GetLevel();
                if (level == 5)
                {
                    var wheel = machineContext.state.Get<WheelsActiveState11017>().GetRunningWheel()[0];
                    var elementContainer = wheel.GetRoll(2).GetVisibleContainer(2);
                    var elementConfigSet = machineContext.state.machineConfig.GetElementConfigSet();
                    var fixedConfig = elementConfigSet.GetElementConfig(Constant11017.PuePleElementId);
                    elementContainer.UpdateElement(new SequenceElement(fixedConfig, machineContext));
                }
            }

            await StartEliminate();
            var totalWin = machineContext.state.Get<WinState11017>().totalElimateWin;
            AddWinChipsToControlPanel(totalWin, 1, true, false, "Symbol_SmallWin_11017");
        
            var winState = machineContext.state.Get<WinState>();
          
            if (winState.winLevel >= (int) WinLevel.BigWin)
            {
                await machineContext.WaitSeconds(1f);
            }

            await ShowSpecialBonusBigWinEffect();
            
            ShowBlinkWinLine();
            
            if (machineContext.state.Get<AutoSpinState>().IsAutoSpin)
            {
                machineContext.view.Get<ControlPanel>().ShowAutoStopButton(true);
            }

            Proceed();
        }

        //消除完成后的bigwin特效
        protected virtual async Task ShowSpecialBonusBigWinEffect()
        {
            var winState = machineContext.state.Get<WinState>();

            if (winState.winLevel >= (int) WinLevel.BigWin)
            {
                TaskCompletionSource<bool> waitTask = new TaskCompletionSource<bool>();
                machineContext.AddWaitTask(waitTask, null);
                ulong win = winState.displayTotalWin;
                if (machineContext.state.Get<FreeSpinState>().IsInFreeSpin)
                {
                    win = winState.displayCurrentWin;
                }

                WinEffectHelper.ShowBigWinEffect(winState.winLevel, win, () =>
                {
                    machineContext.RemoveTask(waitTask);
                    waitTask.SetResult(true);
                });
                await waitTask.Task;
            }
            else
            {
                await Task.CompletedTask;
            }
        }

        protected void ShowBlinkWinLine()
        {
            var wheel = machineContext.state.Get<WheelsActiveState11017>().GetRunningWheel()[0];
            if (wheel != null)
            {
                machineContext.AddWaitEvent(WaitEvent.WAIT_BLINK_ALL_WIN_LINE);
                wheel.winLineAnimationController.BlinkAllWinLine(() =>
                {
                    machineContext.RemoveWaitEvent(WaitEvent.WAIT_BLINK_ALL_WIN_LINE);
                });
            }
        }

        public async Task StartEliminate()
        {
            var remainingCouns = 0;
            if (machineContext.state.Get<FreeSpinState>().IsInFreeSpin)
            {
                remainingCouns = machineContext.state.Get<WheelState11017>(2).panelCount;
            }
            else
            {
                remainingCouns = machineContext.state.Get<WheelState11017>().panelCount;
            }

            var wheels = machineContext.state.Get<WheelsActiveState>().GetRunningWheel();
            for (int k = 0; k < remainingCouns; k++)
            {
                if (k == remainingCouns - 1)
                {
                    if (machineContext.state.Get<FreeSpinState>().IsInFreeSpin)
                    {
                        machineContext.state.Get<WheelState11017>(2).UpdateStateOnReceiveSpinResultByResult();
                    }
                    else
                    {
                        machineContext.state.Get<WheelState11017>().UpdateStateOnReceiveSpinResultByResult();
                    }

                    wheels[0].ForceUpdateElementOnWheel();
                    if (machineContext.state.Get<FreeSpinState>().IsInFreeSpin)
                    {
                        uint level = machineContext.state.Get<ExtraState11017>().GetLevel();
                        if (level == 5)
                        {
                            var wheel = machineContext.state.Get<WheelsActiveState11017>().GetRunningWheel()[0];
                            var elementContainer = wheel.GetRoll(2).GetVisibleContainer(2);
                            var elementConfigSet = machineContext.state.machineConfig.GetElementConfigSet();
                            var fixedConfig = elementConfigSet.GetElementConfig(Constant11017.PuePleElementId);
                            elementContainer.UpdateElement(new SequenceElement(fixedConfig, machineContext));
                        }
                    }
                }
                else
                {
                    if (k == 0)
                    {
                        //播放消除动画
                        StartPlayElephantEliminateAnimation();
                        await machineContext.WaitSeconds(0.5f);
                        StartPlayEliminateAnimation1();
                        AudioUtil.Instance.PlayAudioFx("Remove");
                        await machineContext.WaitSeconds(0.5f + 0.66f);
                    }
                    else
                    {
                        //播放消除动画
                        StartPlayEliminateAnimation2();
                        AudioUtil.Instance.PlayAudioFx("Remove");
                        await machineContext.WaitSeconds(0.5f);
                    }

                    var n = new List<uint>();
                    if (machineContext.state.Get<FreeSpinState>().IsInFreeSpin)
                    {
                        machineContext.state.Get<WheelState11017>(2).UpdateStateOnReceiveSpinResultByResult();
                        n = machineContext.state.Get<WheelState11017>(2).changeList;
                    }
                    else
                    {
                        machineContext.state.Get<WheelState11017>().UpdateStateOnReceiveSpinResultByResult();
                        n = machineContext.state.Get<WheelState11017>().changeList;
                    }

                    if (machineContext.state.Get<FreeSpinState>().IsInFreeSpin)
                    {
                        uint level = machineContext.state.Get<ExtraState11017>().GetLevel();
                        if (level == 5)
                        {
                            var wheel = machineContext.state.Get<WheelsActiveState11017>().GetRunningWheel()[0];
                            var elementContainer = wheel.GetRoll(2).GetVisibleContainer(2);
                            var elementConfigSet = machineContext.state.machineConfig.GetElementConfigSet();
                            var fixedConfig = elementConfigSet.GetElementConfig(Constant11017.PuePleElementId);
                            elementContainer.UpdateElement(new SequenceElement(fixedConfig, machineContext));
                        }
                    }

                    wheels[0].ForceUpdateElementOnWheel();
                   
                    for (int i = 0; i < 5; i++)
                    {
                        for (int j = 0; j < 3; j++)
                        {
                            var elementContainer = wheels[0].GetRoll(i).GetVisibleContainer(j);
                            var element = elementContainer.GetElement();
                            var roll = wheels[0].GetRoll(0);
                            if (n[i * 3 + j] != 0)
                            {
                                element.transform.localPosition = new Vector3(0, roll.stepSize * n[i * 3 + j], 0);
                            }
                        }
                    }

                    uint levelFix = machineContext.state.Get<ExtraState11017>().GetLevel();
                    //SFG时第二列的第二个金色猛犸不要移动
                    if (levelFix == 5)
                    {
                        var elementContainer = wheels[0].GetRoll(2).GetVisibleContainer(2);
                        var element = elementContainer.GetElement();
                        element.transform.localPosition = Vector3.zero;
                    }

                    for (int i = 0; i < 5; i++)
                    {
                        for (int j = 0; j < 3; j++)
                        {
                            var elementContainer = wheels[0].GetRoll(i).GetVisibleContainer(j);
                            var element = elementContainer.GetElement();
                            var roll = wheels[0].GetRoll(0);
                            if (n[i * 3 + j] != 0)
                            {
                                element.transform.DOLocalMoveY(0, 0.25f);
                            }
                        }
                    }
                }

                if (k == 0)
                {
                    await machineContext.WaitSeconds(1.4f - 0.66f - 0.5f);
                }
                else
                {
                    await machineContext.WaitSeconds(0.25f);
                }
            }
        }

        public void StartPlayElephantEliminateAnimation()
        {
            var wheels = machineContext.state.Get<WheelsActiveState>().GetRunningWheel();
            var reTriggerElementContainers = wheels[0].GetElementMatchFilter((container) =>
            {
                if (container.sequenceElement.config.id == 13)
                {
                    return true;
                }

                return false;
            });

            if (reTriggerElementContainers.Count > 0)
            {
                for (var i = 0; i < reTriggerElementContainers.Count; i++)
                {
                    reTriggerElementContainers[i].PlayElementAnimation("Remove");
                }

                AudioUtil.Instance.PlayAudioFx("W02_RemoveStart");
            }
        }

        public void StartPlayEliminateAnimation1()
        {
            var wheels = machineContext.state.Get<WheelsActiveState>().GetRunningWheel();
            if (wheels.Count > 0)
            {
                BlinkEliminateLine1();
            }
        }

        public void StartPlayEliminateAnimation2()
        {
            var wheels = machineContext.state.Get<WheelsActiveState>().GetRunningWheel();
            if (wheels.Count > 0)
            {
                BlinkEliminateLine2();
            }
        }

        public void BlinkEliminateLine1()
        {
            var wheels = machineContext.state.Get<WheelsActiveState>().GetRunningWheel();
            var bonusWinLines = wheels[0].wheelState.GetBonusWinLine();
            for (int i = 0; i < bonusWinLines.Count; i++)
            {
                for (var index = 0; index < bonusWinLines[i].Positions.Count; index++)
                {
                    var pos = bonusWinLines[i].Positions[index];
                    var container = wheels[0].GetWinLineElementContainer((int) pos.X, (int) pos.Y);
                    container.PlayElementAnimation("Remove");
                    container.ShiftSortOrder(true);
                }
            }
        }

        public void BlinkEliminateLine2()
        {
            var wheels = machineContext.state.Get<WheelsActiveState>().GetRunningWheel();
            var bonusWinLines = wheels[0].wheelState.GetBonusWinLine();
            for (int i = 0; i < bonusWinLines.Count; i++)
            {
                for (var index = 0; index < bonusWinLines[i].Positions.Count; index++)
                {
                    var pos = bonusWinLines[i].Positions[index];
                    var container = wheels[0].GetWinLineElementContainer((int) pos.X, (int) pos.Y);
                    container.PlayElementAnimation("Remove2");
                    container.ShiftSortOrder(true);
                }
            }
        }
    }
}