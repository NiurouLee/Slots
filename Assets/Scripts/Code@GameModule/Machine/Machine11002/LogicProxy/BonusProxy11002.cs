// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/07/08/14:12
// Ver : 1.0.0
// Description : BonusLogicProxy11002.cs
// ChangeLog :
// **********************************************

using System;
using DragonU3DSDK.Network.API.ILProtocol;

namespace GameModule
{
    public class BonusProxy11002 : BonusProxy
    {

        public BonusProxy11002(MachineContext context) :
            base(context)
        {
        }
        
        protected override void RegisterInterestedWaitEvent()
        {
            base.RegisterInterestedWaitEvent();
            waitEvents.Add(WaitEvent.WAIT_WIN_NUM_ANIMTION);
        }

        protected override void HandleCustomLogic()
        {
            StartBonusGame();
        }

        protected async void StartBonusGame()
        {
            if (preLogicStep != LogicStepType.STEP_MACHINE_SETUP)
            {
                await BlinkBonusLine();
                await XUtility.WaitSeconds(2.5f, machineContext);
            }

            machineContext.state.Get<JackpotInfoState>().LockJackpot = true;
            machineContext.view.Get<JackPotPanel>().transform.gameObject.SetActive(false);
            var view = PopUpManager.Instance.ShowPopUp<UIWheelBonus11002>();
            view.SetShowCallback(BonusCallback);
        }

        private void BonusCallback(GameResult gameResult, WinsOPlentyGameResultExtraInfo extraInfo)
        {
            if (extraInfo.WheelBonusInfo.JackpotId > 0)
            {
                long numJackpot = (long)gameResult.JackpotPay * (long)extraInfo.WheelBonusInfo.Bet / 100;
                string path = string.Empty;
                switch (extraInfo.WheelBonusInfo.JackpotId)
                {
                    case 1:
                        path = "UIJackpotMini11002";
                        break;
                    case 2:
                        path = "UIJackpotMinor11002";
                        break;
                    case 3:
                        path = "UIJackpotMajor11002";
                        break;
                    case 4:
                        path = "UIJackpotGrand11002";
                        break;
                }
                var view = PopUpManager.Instance.ShowPopUp<UIJackpotBase>(path);
                AudioUtil.Instance.PlayAudioFx("JACKPOT_Open");

                view.SetJackpotWinNum(Convert.ToUInt64(numJackpot));

                view.SetPopUpCloseAction(
                    async () =>
                    {
                        var popup = PopUpManager.Instance.GetPopup<UIWheelBonus11002>();
                        if (popup != null)
                        {
                            PopUpManager.Instance.ClosePopUp(popup);
                            popup.SetPopUpCloseAction(() =>
                            {
                                machineContext.view.Get<JackPotPanel>().transform.gameObject.SetActive(true);
                            });
                        }
                        await WinEffectHelper.ShowBigWinEffectAsync(gameResult.WinLevel, gameResult.DisplayCurrentWin, machineContext);
                        UpdateWinChipsToDisplayTotalWin();
                        Proceed();
                    });
            }
            else
            {
                Proceed();
            }

            machineContext.state.Get<JackpotInfoState>().LockJackpot = false;
        }
    }
}