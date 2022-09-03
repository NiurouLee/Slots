using System.Threading.Tasks;

namespace GameModule
{
    public class WheelStopSpecialEffectProxy11005: WheelStopSpecialEffectProxy
    {
        private FreeSpinState freeSpinState;
        
        private string[] _jackpotsAddress =
            {"UIJackpotMinor11005", "UIJackpotMajor11005", "UIJackpotGrand11005"};
        public WheelStopSpecialEffectProxy11005(MachineContext machineContext) : base(machineContext)
        {
            freeSpinState = machineContext.state.Get<FreeSpinState>();
        }


        protected override async void HandleCustomLogic()
        {
            
            
            
            
            var jackpotState = machineContext.state.Get<JackpotInfoState>();
            if (jackpotState.HasJackpotWin())
            {
                var jackpotWinInfo = jackpotState.GetJackpotWinInfo();

                machineContext.view.Get<JackPotPanel11005>().PlayJackpotAnim(jackpotWinInfo.jackpotId - 1);
                await BlinkJackpotLine(jackpotWinInfo.jackpotId);

                //await context.WaitSeconds(1);

                var assetAddress = _jackpotsAddress[jackpotWinInfo.jackpotId - 1];

                var view = PopUpManager.Instance.ShowPopUp<UIJackpot11005>(assetAddress);
                var jackpotWin = machineContext.state.Get<BetState>().GetPayWinChips(jackpotWinInfo.jackpotPay);
            
                view.SetJackpotWinNum(jackpotWin);

                view.SetPopUpCloseAction(()=>
                {
                    AddWinChipsToControlPanel(jackpotWin);
                    HandleBaseLetter();
                });
            }
            else
            {
                if (freeSpinState.IsInFreeSpin)
                {
                    HandleFreeCollect();
                }
                else
                {
                    HandleBaseLetter();
                }
            }
        }

        private string[] jackpotAnimName = {"Minor","Major","Grand" };
        protected async Task BlinkJackpotLine(uint jackpotId)
        {
            var wheels = machineContext.state.Get<WheelsActiveState>().GetRunningWheel();
            if (wheels.Count > 0)
            {
                
                var bonusWinLines =  wheels[0].wheelState.GetJackpotWinLines();

                for (int i = 0; i < bonusWinLines.Count; i++)
                {
                    for (var index = 0; index < bonusWinLines[i].Positions.Count; index++)
                    {
                        var pos = bonusWinLines[i].Positions[index];
                        var container =  wheels[0].GetWinLineElementContainer((int) pos.X,(int) pos.Y);
                        

                        AudioUtil.Instance.PlayAudioFx(container.sequenceElement.config.name + "_Trigger");
                        container.PlayElementAnimation("Trigger");
                        container.ShiftSortOrder(true);
                        //container.ShiftSortOrder(true);
                        
                        var tranZi = container.GetElement().transform.Find("Zi");
                        for (int j = 0; j < jackpotAnimName.Length; j++)
                        {
                            if (j == jackpotId - 1)
                            {
                                tranZi.Find(jackpotAnimName[j]).gameObject.SetActive(true);
                            }
                            else
                            {
                                tranZi.Find(jackpotAnimName[j]).gameObject.SetActive(false);
                            }
                        }
                    }
                }

                await machineContext.WaitSeconds(2);
                //await wheels[0].winLineAnimationController.BlinkJackpotLine();
            }
        }

        protected virtual async void HandleBaseLetter()
        {
            if (machineContext.state.Get<ExtraState11005>().HasSpecialBonus())
            {
                

                
                var popUp = machineContext.view.Get<BaseLetterView11005>();
                await popUp.RefreshUI();
                //await popUp.PlayUnlockAnim();
            }
            else
            {
                machineContext.view.Get<BaseLetterView11005>().RefreshUI();
            }

            base.HandleCustomLogic();
        }

        protected async virtual void HandleFreeCollect()
        {

            await machineContext.view.Get<FreeCollectView11005>().RefresUI();
            base.HandleCustomLogic();
        }
    }
}