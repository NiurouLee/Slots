//  **********************************************
//  Copyright(c) 2021 by com.ustar
//  All right reserved
// 
//  Author : Besure.Chen
//  Date :2022-01-07 18:46
//  Ver : 1.0.0
//  Description : BonusProxy11028.cs
//  ChangeLog :
//  **********************************************

using System.Threading.Tasks;
using DragonU3DSDK.Network.API.ILProtocol;

namespace GameModule
{
    public class BonusProxy11028:BonusProxy
    {
        private WheelViewPopup11028 popupWheel;
        public BonusProxy11028(MachineContext context)
            : base(context)
        {
            
        }
        protected override async void HandleCustomLogic()
        {
            StopWinCycle(true);
            machineContext.state.Get<JackpotInfoState>().LockJackpot = true;
            var extraState = machineContext.state.Get<ExtraState11028>();
            if (extraState.NeedSettle())
            {
                CheckWheelEnd();
            }
            else if (extraState.IsChooseStep())
            {
                if (!IsFromMachineSetup())
                {
                    var wheels = machineContext.state.Get<WheelsActiveState>().GetRunningWheel();
                    var triggerElementContainers = wheels[0].GetElementMatchFilter((container) =>
                    {
                        if (container.sequenceElement.config.id == Constant11028.B01)
                        {
                            return true;
                        }

                        return false;
                    });

                    if (triggerElementContainers.Count > 0)
                    {
                        AudioUtil.Instance.PauseMusic();
                        for (var j = 0; j < triggerElementContainers.Count; j++)
                        {
                            triggerElementContainers[j].PlayElementAnimation("Trigger");
                            triggerElementContainers[j].ShiftSortOrder(true);
                        }
                        AudioUtil.Instance.PlayAudioFx("B01_Trigger");
                        await XUtility.WaitSeconds(3, machineContext);
                        for (var j = 0; j < triggerElementContainers.Count; j++)
                        {
                            triggerElementContainers[j].UpdateAnimationToStatic();
                            triggerElementContainers[j].ShiftSortOrder(false);
                        }
                    }   
                }
                AudioUtil.Instance.PlayAudioFx("ChooseFeature_Open");
                var chooseView = machineContext.view.Get<WheelBonusChooseView11028>();
                chooseView.SelectAction += OnChooseClick;
                chooseView.OpenChoose();
            }
            else
            {
                ShowBonusWheelPopup();
            }
        }

        private void ShowBonusWheelPopup()
        {
            if (popupWheel == null)
            {
                AudioUtil.Instance.PlayAudioFx("Wheel_Open");
                popupWheel = PopUpManager.Instance.ShowPopUp<WheelViewPopup11028>("UIWheelBonus11028");
                popupWheel.WheelEndAction = CheckFinishOrWheelAgain;   
            }
            else
            {
                popupWheel.ReInitialize();
            }
        }

        private void CheckFinishOrWheelAgain()
        {
            if (machineContext.state.Get<ExtraState11028>().HasWheelWin)
            {
                AudioUtil.Instance.PauseMusic();
                if (machineContext.state.Get<ExtraState11028>().NormalWheelIndex == 0 || 
                    machineContext.state.Get<ExtraState11028>().NormalWheelIndex == 4 || 
                    machineContext.state.Get<ExtraState11028>().NormalWheelIndex == 12)
                {
                    var popup = PopUpManager.Instance.ShowPopUp<BonusRapidHitPopUp11028>("UIWheelBonusFinish11028");
                    popup.SetPopUpCloseAction(CheckWheelEnd);    
                }
                else
                {
                    var extraState = machineContext.state.Get<ExtraState11028>();
                    var winRate = extraState.NormalHit.WinRate;
                    winRate += extraState.NormalHit.JackpotPay;
                    if (extraState.IsMultiplierWheel)
                    {
                        winRate *= extraState.MultiplierWheelHit;
                    }
                    var popup = PopUpManager.Instance.ShowPopUp<RapidHitPopUp11028>(machineContext.assetProvider.GetAssetNameWithPrefix("UIRapidStart"));
                    popup.InitializeJackpot(extraState.IsNight,extraState.GetJackpotCount(), winRate);
                    popup.SetPopUpCloseAction(CheckWheelEnd);
                }
            }
            else
            {
                CheckWheelEnd();
            }
        }

        public void CloseWheelPopup()
        {
            if (popupWheel != null)
            {
                popupWheel.Close();
                popupWheel = null;
            }
        }

        private async void CheckWheelEnd()
        {
            if (machineContext.state.Get<ExtraState11028>().NeedSettle())
            {
                if (machineContext.state.Get<ExtraState11028>().NormalHit.IsOver)
                {
                    CloseWheelPopup();
                }
                await machineContext.state.Get<ExtraState11028>().SettleBonusProgress();
                ForceUpdateWinChipsToDisplayTotalWin(0.1f);
                Proceed();   
            }
            else
            {
                ShowBonusWheelPopup();
            } 
        }

        private async void OnChooseClick(bool isNightClick)
        {
            machineContext.view.Get<WheelBonusChooseView11028>().PlayAnimation(isNightClick ? "Night" : "Day");
            AudioUtil.Instance.PlayAudioFx("ChooseFeature_Selected");
            await machineContext.WaitSeconds(2.5f);
            await SendBonusProcess(isNightClick);
            ShowBonusWheelPopup();
        }

        private async Task SendBonusProcess(bool isNightClick)
        {
            CBonusProcess cBonusProcess = new CBonusProcess();
            cBonusProcess.Json = "Night";
            await machineContext.state.Get<ExtraState11028>().SendBonusProcess(isNightClick ? cBonusProcess : null); 
            machineContext.view.Get<WheelBonusChooseView11028>().Hide();
        }
    }
}