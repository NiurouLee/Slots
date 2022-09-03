using System.Threading.Tasks;
using UnityEngine;

namespace GameModule
{
    public class BonusProxy11027:BonusProxy
    {
        private ExtraState11027 extraState;
        public BonusProxy11027(MachineContext context)
            : base(context)
        {
            extraState = machineContext.state.Get<ExtraState11027>();
        }
        protected override async void HandleCustomLogic()
        {
            StopWinCycle(true);
            var _extraState11027 = machineContext.state.Get<ExtraState11027>();
            if (_extraState11027.GetIsPicking())
            {
                await StartPickGame();
            }
            else if (_extraState11027.GetIsRolling())
            {
                await StartWheelRollingGame();
            }
            // else if (_extraState11027.GetIsPicking())
            // {
            //     await StartPickGame();
            // }
            // else if (_extraState11027.GetIsRolling())
            // {
            //     await StartWheelRollingGame();
            // }
            // if (_extraState11027.NeedRollingSettle())
            // {
            //     WheelRollingFinish();
            // }
            // else if (_extraState11027.NeedPickSettle())
            // {
            //     await PickGameFinish();
            // }
            // else if (_extraState11027.GetIsPicking())
            // {
            //     await StartPickGame();
            // }
            // else if (_extraState11027.GetIsRolling())
            // {
            //     await StartWheelRollingGame();
            // }
        }

        private async Task StartPickGame()
        {
            var _extraState11027 = machineContext.state.Get<ExtraState11027>();
            if (IsFromMachineSetup()) 
            {
                AudioUtil.Instance.StopMusic();
                machineContext.view.Get<PickGame11027>().PLayPickMusic();
                machineContext.state.Get<JackpotInfoState>().LockJackpot = true;
                machineContext.transform.Find("WheelFeature/WheelBonusGame02").gameObject.SetActive(false);
                machineContext.transform.Find("WheelFeature/Wheels/WheelBaseGame").gameObject.SetActive(false);
                machineContext.transform.Find("WheelFeature/Wheels/CollectionGroup").gameObject.SetActive(false);
                machineContext.transform.Find("WheelFeature/Wheels/WheelPickGame").gameObject.SetActive(true);
                machineContext.transform.Find("WheelFeature/JackpotPanel").gameObject.SetActive(true);
                machineContext.view.Get<WheelFeature11027>().PlayPickIdle();
                machineContext.view.Get<PickGame11027>().CreateColorfulEggs();
                if (_extraState11027.NeedPickSettle())
                {
                     await machineContext.view.Get<PickGame11027>().RecoverSettle();
                }
                else
                {
                     machineContext.view.Get<PickGame11027>().PlayState();
                }
            }
            else
            {
                if (machineContext.state.Get<AutoSpinState>().IsAutoSpin)
                {
                    machineContext.view.Get<ControlPanel>().ShowAutoStopButton(false);
                }
                AudioUtil.Instance.StopMusic();
                machineContext.view.Get<CollectionGroup11027>().ShowFeatrue();
                machineContext.view.Get<CollectionGroup11027>().PlayPickTranstion();
                machineContext.view.Get<TransitionPickView11027>().PlayPickTransitionAnimation();
                await machineContext.WaitSeconds(2.5f);
                machineContext.view.Get<CollectionGroup11027>().HideParticle();
                machineContext.state.Get<JackpotInfoState>().LockJackpot = true;
                machineContext.transform.Find("WheelFeature/WheelBonusGame02").gameObject.SetActive(false);
                machineContext.transform.Find("WheelFeature/Wheels/WheelBaseGame").gameObject.SetActive(false);
                machineContext.transform.Find("WheelFeature/Wheels/CollectionGroup").gameObject.SetActive(false);
                machineContext.transform.Find("WheelFeature/Wheels/WheelPickGame").gameObject.SetActive(true);
                machineContext.transform.Find("WheelFeature/JackpotPanel").gameObject.SetActive(true);
                machineContext.view.Get<WheelFeature11027>().PlayPickIdle();
                machineContext.view.Get<PickGame11027>().CreateColorfulEggs();
                await machineContext.WaitSeconds(4.13f - 2.5f);
                machineContext.view.Get<PickGame11027>().PLayPickMusic();
                await machineContext.view.Get<PickGame11027>().RecoverSettle();
            }
        }
        
        public async Task PickGameFinish()
        {
            var _extraState11027 = machineContext.state.Get<ExtraState11027>();
            if (_extraState11027.NeedPickSettle())
            {
                await _extraState11027.SettleBonusProgress();
                ulong bonusWin = _extraState11027.GetPickWin();
                // ulong bonusWin = _extraState11027.GetBonusTotalWin() - _extraState11027.GetPanelWin();
                AddWinChipsToControlPanel(bonusWin, 1, false, false);
                await machineContext.WaitSeconds(1.0f);
                machineContext.view.Get<TransitionWheelView11027>().PlayPickTransitionAnimation();
                await machineContext.WaitSeconds(3.0f);
                machineContext.state.Get<JackpotInfoState>().LockJackpot = false;
                machineContext.view.Get<JackpotPanel11027>().RecoverJackpotNormalState();
                machineContext.view.Get<PickGame11027>().Reset();
                machineContext.view.Get<WheelFeature11027>().PlayBaseIdle();
                machineContext.transform.Find("WheelFeature/Wheels/WheelBaseGame").gameObject.SetActive(true);
                machineContext.transform.Find("WheelFeature/Wheels/CollectionGroup").gameObject.SetActive(true);
                machineContext.transform.Find("WheelFeature/Wheels/WheelPickGame").gameObject.SetActive(false);
                machineContext.transform.Find("WheelFeature/WheelBonusGame02").gameObject.SetActive(true);
                machineContext.transform.Find("WheelFeature/JackpotPanel").gameObject.SetActive(true);
                machineContext.transform.Find("WheelFeature/WheelBonusGame02").gameObject.SetActive(true);
                machineContext.view.Get<CollectionGroup11027>().ShowCollectionGroup(false);
                await machineContext.WaitSeconds(3.67f - 3.0f);
                await ShowBonusBigWinEffect();
                Proceed();
            }
        }

        private async Task StartWheelRollingGame()
        {
             var _extraState11027 = machineContext.state.Get<ExtraState11027>();
            if (IsFromMachineSetup())
            {
                AudioUtil.Instance.StopMusic();
                machineContext.view.Get<WheelRollingView11027>().PLayWheelMusic();
                machineContext.view.Get<WheelFeature11027>().PlayWheelIdle();
                machineContext.state.Get<JackpotInfoState>().LockJackpot = true;
                machineContext.transform.Find("WheelFeature/Wheels/WheelBaseGame").gameObject.SetActive(false);
                machineContext.transform.Find("WheelFeature/Wheels/CollectionGroup").gameObject.SetActive(false);
                machineContext.transform.Find("WheelFeature/Wheels/WheelPickGame").gameObject.SetActive(false);
                machineContext.transform.Find("WheelFeature/WheelBonusGame02").gameObject.SetActive(true);
                machineContext.view.Get<WheelRollingView11027>().Show();
                if (_extraState11027.NeedRollingSettle())
                {
                    machineContext.view.Get<WheelRollingView11027>().RecoverInitializeBonusWheelView();
                    await machineContext.view.Get<WheelRollingView11027>().RecoverSettle();
                }
                else
                {
                    machineContext.view.Get<WheelRollingView11027>().InitializeWheelView(false, true);
                    machineContext.view.Get<WheelRollingView11027>().InitializeBonusWheelView();
                }
            }
            else
            {
                if (machineContext.state.Get<AutoSpinState>().IsAutoSpin)
                {
                    machineContext.view.Get<ControlPanel>().ShowAutoStopButton(false);
                }
                AudioUtil.Instance.StopMusic();
                StartPlayElephantEliminateAnimation();
                await machineContext.WaitSeconds(3.0f);
                ScatterToStatic();
                StopRollIndexAnticipationAnimation();
                machineContext.view.Get<TransitionWheelView11027>().PlayWheelTransitionAnimation();
                await machineContext.WaitSeconds(3.67f - 0.5f);
                await machineContext.view.Get<TouchToSpin11027>().ShowTouchSpin();
            }
        }

        private void StopRollIndexAnticipationAnimation()
        {
            var wheel = machineContext.state.Get<WheelsActiveState11027>().GetRunningWheel()[0];
            var anim = (WheelSpinningController<WheelAnimationController11027>) wheel.spinningController;
            var controller = (WheelAnimationController11027) anim.animationController;
            for (int i = 2; i < 5; i++)
            {
                 controller.StopRollIndexAnticipationAnimation(i, true);
            }
        }

        public async void WheelRollingFinish()
        {
            var _extraState11027 = machineContext.state.Get<ExtraState11027>();
            if (_extraState11027.NeedRollingSettle())
            {
                await _extraState11027.SettleBonusProgress();
                ulong bonusWin = _extraState11027.GetWheelWin();
                AddWinChipsToControlPanel(bonusWin, 1, false, false);
                await machineContext.WaitSeconds(1.0f);
                machineContext.view.Get<TransitionWheelView11027>().PlayWheelTransitionAnimation();
                await machineContext.WaitSeconds(2.77f);
                await machineContext.view.Get<WheelFeature11027>().PlayToBase();
                machineContext.view.Get<WheelRollingView11027>().InitializeWheelView(true, false);
                machineContext.state.Get<JackpotInfoState>().LockJackpot = false;
                machineContext.transform.Find("WheelFeature/Wheels/WheelBaseGame").gameObject.SetActive(true);
                machineContext.transform.Find("WheelFeature/Wheels/CollectionGroup").gameObject.SetActive(true);
                machineContext.transform.Find("WheelFeature/Wheels/WheelPickGame").gameObject.SetActive(false);
                machineContext.transform.Find("WheelFeature/WheelBonusGame02").gameObject.SetActive(true);
                await machineContext.WaitSeconds(2.0f);
                machineContext.view.Get<WheelRollingView11027>().StopMusic();
                await ShowBonusBigWinEffect();
                Proceed();
               
            }
        } 
        
        //bigWin
        protected virtual async Task ShowBonusBigWinEffect()
        {
            var winState = machineContext.state.Get<WinState>();
            if (winState.winLevel >= (int)WinLevel.BigWin)
            {
                TaskCompletionSource<bool> waitTask = new TaskCompletionSource<bool>();
                machineContext.AddWaitTask(waitTask, null);
                WinEffectHelper.ShowBigWinEffect(winState.winLevel, winState.displayTotalWin, () =>
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
        
        public void StartPlayElephantEliminateAnimation()
        {
            var wheels = machineContext.state.Get<WheelsActiveState>().GetRunningWheel();
            var wheel = machineContext.state.Get<WheelsActiveState>().GetRunningWheel()[0];
            wheel.GetContext().state.Get<WheelsActiveState11027>().FadeOutRollMask(wheel);
            var reTriggerElementContainers = wheels[0].GetElementMatchFilter((container) =>
            {
                if (container.sequenceElement.config.id == Constant11027.ScatterElementId)
                {
                    return true;
                }

                return false;
            });

            if (reTriggerElementContainers.Count > 0)
            {
                for (var i = 0; i < reTriggerElementContainers.Count; i++)
                {
                    reTriggerElementContainers[i].PlayElementAnimation("Trigger");
                }
                AudioUtil.Instance.StopMusic();
                AudioUtil.Instance.PlayAudioFx("B01_Trigger");
            }
        }
        
        public void ScatterToStatic()
        {
            var wheels = machineContext.state.Get<WheelsActiveState>().GetRunningWheel();
            var wheel = machineContext.state.Get<WheelsActiveState>().GetRunningWheel()[0];
            wheel.GetContext().state.Get<WheelsActiveState11027>().FadeOutRollMask(wheel);
            var reTriggerElementContainers = wheels[0].GetElementMatchFilter((container) =>
            {
                if (container.sequenceElement.config.id == Constant11027.ScatterElementId)
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