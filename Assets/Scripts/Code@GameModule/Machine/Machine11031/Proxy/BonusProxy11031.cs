using DG.Tweening;
using UnityEngine;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace GameModule
{
    public class BonusProxy11031 : BonusProxy
    {
        private ExtraState11031 _extraState11031;
        protected ControlPanel controlPanel;

        public BonusProxy11031(MachineContext context) : base(context)
        {
            _extraState11031 = machineContext.state.Get<ExtraState11031>();
        }

        protected override void RegisterInterestedWaitEvent()
        {
            waitEvents.Add(WaitEvent.WAIT_WIN_NUM_ANIMTION);
            waitEvents.Add(WaitEvent.WAIT_BLINK_ALL_WIN_LINE);
        }

        public override void SetUp()
        {
            base.SetUp();
            controlPanel = machineContext.view.Get<ControlPanel>();
        }

        protected override async void HandleCustomLogic()
        {
            StopWinCycle(true);
            AudioUtil.Instance.StopMusic();
            machineContext.view.Get<LuckyPot11031>().PLayLuckyPotMusic();
            controlPanel.UpdateControlPanelState(false,UseAverageBet());
            machineContext.view.Get<LuckyPot11031>().SetPrizeText();
            if (IsFromMachineSetup())
            {
                int round = (int) _extraState11031.GetMapRound();
                bool isSelect = _extraState11031.GetMapIsSelect();
                bool isOffer = _extraState11031.GetMapIsOffer();
                bool isFinal = _extraState11031.GetMapIsFinal();
                machineContext.view.Get<LuckyPot11031>().SetBeforePos();
                if (round == 0 && isSelect == false)
                {
                    machineContext.view.Get<LuckyPot11031>().ShowLuckyPot(false);
                    await machineContext.view.Get<LuckyPot11031>().ShowLuckPotPickYourPot();
                }
                else if (_extraState11031.GetMapToSettle())
                {
                    machineContext.view.Get<LuckyPot11031>().ToSettle();
                }
                else if (round > 0 && isSelect == true && isOffer == false && isFinal == false)
                {
                    machineContext.view.Get<LuckyPot11031>().RecoverSelect();
                }
                else if (round > 0 && isSelect == false && isOffer == false && isFinal == false)
                {
                    machineContext.view.Get<LuckyPot11031>().RecoverBreakLetter();
                }
                else if (round > 0 && isSelect == false && isOffer == true && isFinal == false)
                {
                    machineContext.view.Get<LuckyPot11031>().RecoverShowOffer();
                }
                else if (isFinal == true)
                {
                    machineContext.view.Get<LuckyPot11031>().RecoverFinalView();
                }
            }
            else
            {
                machineContext.view.Get<LuckyPot11031>().ShowLuckyPot(false);
                int round = (int) _extraState11031.GetMapRound();
                if (round == 0)
                {
                    await machineContext.view.Get<LuckyPot11031>().ShowLuckPotPickYourPot();
                }
            }
        }
        
        public bool UseAverageBet()
        {
            return true;
        }

        private async Task ShowLuckPotStartPopup()
        {
            TaskCompletionSource<bool> taskStartPopup = new TaskCompletionSource<bool>();
            var view = PopUpManager.Instance.ShowPopUp<LuckyPotStart1Popup11031>("UIMiniGame_Start");
            view.SetPopUpCloseAction(async () => { taskStartPopup.SetResult(true); });
            await taskStartPopup.Task;
        }

        public async Task BonusFinish()
        { 
            machineContext.view.Get<CollectGroupView11031>().SetProgress(0);
            machineContext.view.Get<LuckyPot11031>().Hide();
            machineContext.view.Get<LuckyPot11031>().RecoverStartPos();
            controlPanel.UpdateControlPanelState(false, false);
            ulong bonusWin = _extraState11031.GetMapTotalWin() - _extraState11031.GetMapPreWin();
            AddWinChipsToControlPanel(bonusWin, 1, false, false);
            await machineContext.WaitSeconds(1.0f);
            await ShowBonusBigWinEffect();
            Proceed();
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
    }
}