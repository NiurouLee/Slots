//  **********************************************
//  Copyright(c) 2021 by com.ustar
//  All right reserved
// 
//  Author : Besure.Chen
//  Date :2021-10-19 19:36
//  Ver : 1.0.0
//  Description : SpecialBonusProxy11011.cs
//  ChangeLog :
//  **********************************************

using System.Threading.Tasks;
using UnityEngine;

namespace GameModule
{
    public class SpecialBonusProxy11011:SpecialBonusProxy
    {
        public SpecialBonusProxy11011(MachineContext context)
            : base(context)
        {
            
        }
        
        protected override void RegisterInterestedWaitEvent()
        {
            waitEvents.Add(WaitEvent.WAIT_WIN_NUM_ANIMTION);
            waitEvents.Add(WaitEvent.WAIT_BLINK_ALL_WIN_LINE);
        }

        protected override async void HandleCustomLogic()
        {
            if (!machineContext.state.Get<ExtraState11011>().IsPicked())
            {
                if (!IsFromMachineSetup())
                {
                    await machineContext.WaitSeconds(2f);
                }
                var isFull = machineContext.view.Get<CollectCoinView11011>().IsFull;
                if (!isFull)
                {
                    machineContext.view.Get<CollectCoinView11011>().PlayToFull();
                    await machineContext.WaitSeconds(1f);
                }

                machineContext.view.Get<CollectCoinView11011>().PlayTrigger();
                await machineContext.WaitSeconds(0.8f);
                var mainPickView = machineContext.view.Get<MainPickView11011>();
                mainPickView.ToggleVisible(true);
                mainPickView.InitJackpotView();
                mainPickView.BindFinishJackpotAction(OnFinishJackpotAction);
                AudioUtil.Instance.PlayMusic(machineContext.machineConfig.audioConfig.GetBonusBackgroundMusicName());
            }
            else
            {
                Proceed();
            }
        }

        private async void OnFinishJackpotAction()
        {
            var extraState = machineContext.state.Get<ExtraState11011>();
            var jackpotId = extraState.PickJackpotId;
            await machineContext.state.Get<ExtraState>().SendBonusProcess();
            var jackpotState = machineContext.state.Get<JackpotInfoState>();
            if (jackpotState.HasJackpotWin())
            {
                var jackpotWinInfo = jackpotState.GetJackpotWinInfo();
                AudioUtil.Instance.PlayAudioFx("LinkGameEnd_Open");
                var jackpotWin = machineContext.state.Get<BetState>().GetPayWinChips(jackpotWinInfo.jackpotPay);
                var view = PopUpManager.Instance.ShowPopUp<UIJackpotBase>(Constant11011.GetJackpotAddress(jackpotId-1));
                view.SetJackpotWinNum(jackpotWin);
                view.SetPopUpCloseAction(async () =>
                {
                    machineContext.view.Get<CollectCoinView11011>().UpdateState(false);
                    machineContext.view.Get<MainPickView11011>().ToggleVisible(false);
                    
                    //if (!extraState.IsSpecialBonusFinish())
                    //{
                        ForceUpdateWinChipsToDisplayTotalWin();
                    //}
                    Proceed();
                });   
            }
        }
    }
}