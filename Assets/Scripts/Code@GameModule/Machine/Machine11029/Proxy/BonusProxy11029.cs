using System.Threading.Tasks;
using UnityEngine;

namespace GameModule
{
    public class BonusProxy11029 : BonusProxy
    {
        private ExtraState11029 extraState;

        public BonusProxy11029(MachineContext context)
            : base(context)
        {
            extraState = machineContext.state.Get<ExtraState11029>();
        }

        protected override async void HandleCustomLogic()
        {
            StopWinCycle(true);
         
            if (extraState.GetIsWheel())
            {
                await StartWheelRollingGame();
            }
        }


        public async Task StartWheelRollingGame()
        {
            if (machineContext.state.Get<AutoSpinState>().IsAutoSpin)
            {
                machineContext.view.Get<ControlPanel>().ShowAutoStopButton(false);
            }
            if (IsFromMachineSetup())
            {
                if (machineContext.state.Get<AutoSpinState>().IsAutoSpin)
                {
                    machineContext.view.Get<ControlPanel>().ShowAutoStopButton(false);
                }
                machineContext.state.Get<JackpotInfoState>().LockJackpot = true;

                if (extraState.GetIsWheelSettle())
                {
                    AudioUtil.Instance.StopMusic();
                    PLayBonusMusic();
                    machineContext.view.Get<MeiDuSha11029>().ShowMeiDuSha(false);
                    machineContext.transform.Find("WheelFeature/Wheels/WheelBaseGame").gameObject.SetActive(false);
                    machineContext.view.Get<WheelBonus11029>().InitializeBonusWheelView();
                    await machineContext.view.Get<WheelBonus11029>().From();
                }
                else
                {
                    AudioUtil.Instance.StopMusic();
                    //B01trigger动画
                    StartPlayElephantEliminateAnimation();
                    await machineContext.WaitSeconds(3.0f);
                    ScatterToStatic();
                    await machineContext.view.Get<WheelFeature11029>().Open();
                    //过场动画
                    machineContext.view.Get<TransitionsView11029>().PlayMapTransition();
                    await machineContext.WaitSeconds(1.5f);
                    machineContext.view.Get<MeiDuSha11029>().ShowMeiDuSha(false);
                    machineContext.transform.Find("WheelFeature/Wheels/WheelBaseGame").gameObject.SetActive(false);
                    machineContext.view.Get<WheelFeature11029>().Close();
                    //进入wheel
                    machineContext.view.Get<WheelBonus11029>().InitializeBonusWheelView();
                    await machineContext.WaitSeconds(5.6f - 1.5f);
                    AudioUtil.Instance.StopMusic();
                    PLayBonusMusic();
                    machineContext.view.Get<TransitionsView11029>().HideMapTransition();
                    await machineContext.view.Get<WheelBonus11029>().PlayBonusWheelView();
                }
            }
            else
            {
                //B01trigger动画
                StartPlayElephantEliminateAnimation();
                await machineContext.WaitSeconds(3.0f);
                ScatterToStatic();
                //wheel弹窗
                await machineContext.view.Get<WheelFeature11029>().Open();
                //过场动画
                machineContext.view.Get<TransitionsView11029>().PlayMapTransition();
                await machineContext.WaitSeconds(1.5f);
                machineContext.view.Get<MeiDuSha11029>().ShowMeiDuSha(false);
                machineContext.transform.Find("WheelFeature/Wheels/WheelBaseGame").gameObject.SetActive(false);
                machineContext.view.Get<MeiDuSha11029>().ShowMeiDuSha(false);
                machineContext.view.Get<WheelFeature11029>().Close();
                machineContext.state.Get<JackpotInfoState>().LockJackpot = true;
                //进入wheel
                machineContext.view.Get<WheelBonus11029>().InitializeBonusWheelView();
                await machineContext.WaitSeconds(5.6f - 1.5f);
                AudioUtil.Instance.StopMusic();
                PLayBonusMusic();
                machineContext.view.Get<TransitionsView11029>().HideMapTransition(); 
                await machineContext.view.Get<WheelBonus11029>().PlayBonusWheelView();
            }
        }


        //bigWin
        protected virtual async Task ShowBonusBigWinEffect()
        {
            var winState = machineContext.state.Get<WinState>();
            if (winState.winLevel >= (int) WinLevel.BigWin)
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

        //播放trigger动画
        public void StartPlayElephantEliminateAnimation()
        {
            var wheels = machineContext.state.Get<WheelsActiveState>().GetRunningWheel();
            var reTriggerElementContainers = wheels[0].GetElementMatchFilter((container) =>
            {
                if (container.sequenceElement.config.id == Constant11029.ScatterElementId)
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
            var reTriggerElementContainers = wheels[0].GetElementMatchFilter((container) =>
            {
                if (container.sequenceElement.config.id == Constant11029.ScatterElementId)
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

        public async Task BonusGameFinish()
        {
            if (extraState.NeedWheelSettle())
            {
                await extraState.SettleBonusProgress();
                //加钱
                var wheel = extraState.GetWheelData();
                var wheelItems = wheel.Items;
                var index = (int) wheel.Index;
                ulong bonusWin = wheel.Bet * (wheelItems[index].WinRate + wheelItems[index].JackpotPay) / 100;
                AddWinChipsToControlPanel(bonusWin, 1, false, false);
                await machineContext.WaitSeconds(1.0f);
                machineContext.view.Get<TransitionsView11029>().PlayFreeTransition();
                await machineContext.WaitSeconds(1.5f);
                machineContext.view.Get<WheelBonus11029>().transform.gameObject.SetActive(false);
                machineContext.transform.Find("WheelFeature/Wheels/WheelBaseGame").gameObject.SetActive(true);
                machineContext.state.Get<JackpotInfoState>().LockJackpot = false;
                machineContext.view.Get<MeiDuSha11029>().ShowMeiDuSha(true);
                machineContext.view.Get<MeiDuSha11029>().PlayInBase();
                machineContext.view.Get<BackGroundView11029>().ShowFreeBackground(false, false, false);
                if (machineContext.state.Get<AutoSpinState>().IsAutoSpin)
                {
                    machineContext.view.Get<ControlPanel>().ShowAutoStopButton(true);
                }
                await machineContext.WaitSeconds(2.0f - 1.5f);
                machineContext.view.Get<TransitionsView11029>().HideFreeTransition();
                await ShowBonusBigWinEffect();
                Proceed();
            }
        }
        
        public async Task BonusBagFinish()
        {
            if (extraState.NeedWheelSettle())
            {
                machineContext.view.Get<TransitionsView11029>().PlayFreeTransition();
                await machineContext.WaitSeconds(1.5f);
                machineContext.view.Get<WheelBonus11029>().transform.gameObject.SetActive(false);
                machineContext.transform.Find("WheelFeature/Wheels/WheelBaseGame").gameObject.SetActive(true);
                machineContext.state.Get<JackpotInfoState>().LockJackpot = false;
                machineContext.view.Get<MeiDuSha11029>().ShowMeiDuSha(true);
                machineContext.view.Get<MeiDuSha11029>().PlayInBase();
                if (machineContext.state.Get<AutoSpinState>().IsAutoSpin)
                {
                    machineContext.view.Get<ControlPanel>().ShowAutoStopButton(true);
                }
                machineContext.view.Get<BackGroundView11029>().ShowFreeBackground(false, false, false);
                await machineContext.WaitSeconds(2.0f - 1.5f);
                machineContext.view.Get<TransitionsView11029>().HideFreeTransition();
                //显示bag弹窗
                var wheel = extraState.GetWheelData();
                var wheelItems = wheel.Items;
                var index = (int) wheel.Index;
                ulong bonusWin = wheel.Bet * (wheelItems[index].WinRate + wheelItems[index].JackpotPay) / 100;
                await machineContext.view.Get<BonusClollectionPop11029>().ShowCollectionPop((long)bonusWin);
            }
        }
        
        public async Task BonusFreeFinish()
        {
            // var _extraState11029 = machineContext.state.Get<ExtraState11029>();
            // if (_extraState11029.NeedWheelSettle())
            // {
                Proceed();
            // }
        }
        
        public async Task BonusBagAddWin()
        {
            if (extraState.NeedWheelSettle())
            {
                await extraState.SettleBonusProgress();
                //加钱
                var wheel = extraState.GetWheelData();
                var wheelItems = wheel.Items;
                var index = (int) wheel.Index;
                ulong bonusWin = wheel.Bet * (wheelItems[index].WinRate + wheelItems[index].JackpotPay) / 100;
                AddWinChipsToControlPanel(bonusWin, 1, false, false);
                await machineContext.WaitSeconds(1.0f);
                await ShowBonusBigWinEffect();
                Proceed();
            }
        }
        
        public void PLayBonusMusic()
        {
            AudioUtil.Instance.PlayMusic("Bg_Wheel_11029",true);
        }

        public void StopMusic()
        {
             AudioUtil.Instance.StopMusic();
        }
    }
}