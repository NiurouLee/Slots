// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/07/28/10:25
// Ver : 1.0.0
// Description : WheelStopSpecialEffectProxy11003.cs
// ChangeLog :
// **********************************************

namespace GameModule
{
    public class WheelStopSpecialEffectProxy11003 : WheelStopSpecialEffectProxy
    {
        private string[] _jackpotsAddress =
            {"UIJackpotMini11003", "UIJackpotMinor11003", "UIJackpotMajor11003", "UIJackpotGrand11003"};
        
        private string[] _jackpotsBonusAddress =
            {"UIJackpotBonusMini11003", "UIJackpotBonusMinor11003", "UIJackpotBonusMajor11003", "UIJackpotBonusGrand11003"};

        private ExtraState11003 _extraState11003;

        public WheelStopSpecialEffectProxy11003(MachineContext context)
            : base(context)
        {
        }
        
        public override void SetUp()
        {
            base.SetUp();
            _extraState11003 = machineContext.state.Get<ExtraState11003>();
        }


        public override bool CheckCurrentStepHasLogicToHandle()
        {
            return true;
        }

        protected override async void HandleCustomLogic()
        {
            var jackpotState = machineContext.state.Get<JackpotInfoState>();
            if (jackpotState.HasJackpotWin())
            {
                var jackpotWinInfo = jackpotState.GetJackpotWinInfo();

                var assetAddress = _jackpotsAddress[jackpotWinInfo.jackpotId - 1];

                var featureId = (int)jackpotWinInfo.jackpotId - 1;
              
                if (featureId > 1)
                {
                    featureId += 1;
                }

                bool featureLocked = false;
                if (!machineContext.state.Get<BetState>().IsFeatureUnlocked(featureId))
                {
                    featureLocked = true;
                    assetAddress = _jackpotsBonusAddress[jackpotWinInfo.jackpotId - 1];
                }

                var wheelsActiveState  = machineContext.state.Get<WheelsActiveState>();

                var runningWheel = wheelsActiveState.GetRunningWheel();

                for (var i = 0; i < runningWheel.Count; i++)
                {
                    if (runningWheel[i].wheelName == "WheelBaseJackpot"
                    || runningWheel[i].wheelName == "WheelFreeJackpot")
                    {
                        var elementContainer = runningWheel[i].GetRoll(0).GetVisibleContainer(1);

                        AudioUtil.Instance.PlayAudioFx("Jackpot");
                        elementContainer.PlayElementAnimation(featureLocked ? "Win02":"Win");

                        await machineContext.WaitSeconds(2);
                        
                        elementContainer.UpdateAnimationToStatic();
                    }
                }
                 
                var view = PopUpManager.Instance.ShowPopUp<UIJackpotPopUp11003>(assetAddress);
                var jackpotWin = machineContext.state.Get<BetState>().GetPayWinChips(jackpotWinInfo.jackpotPay);
            
                view.SetJackpotWinNum(jackpotWin);

                view.SetPopUpCloseAction(()=>
                {
                    AddWinChipsToControlPanel(jackpotWin,1, false,false);
                    HandlePiggyWinProgress();
                });
            }
            else
            {
                HandlePiggyWinProgress();
            }
        }

        protected async void HandlePiggyWinProgress()
        {
            if (machineContext.state.Get<FreeSpinState>().IsInFreeSpin ||
                machineContext.state.Get<ExtraState11003>().IsSuperFreeGame())
            {
                var runningWheel = machineContext.state.Get<WheelsActiveState>().GetRunningWheel();

                if (runningWheel.Count > 0)
                {
                    var piggyElements = runningWheel[0].GetElementMatchFilter((ElementContainer elementContainer) =>
                    {
                        if (Constant11003.piggyElement.Contains(elementContainer.sequenceElement.config.id))
                        {
                            return true;
                        }

                        return false;
                    });

                    if (piggyElements.Count > 0)
                    {
                        for (var i = 0; i < piggyElements.Count; i++)
                        {

                            AudioUtil.Instance.PlayAudioFx("Piggy_Win");
                            
                            piggyElements[i].ShiftSortOrder(true);
                            piggyElements[i].PlayElementAnimation("Win02", false);
 
                            await XUtility.WaitSeconds(2.0f, machineContext);
                            
                            piggyElements[i].ShiftSortOrder(false);
                       //     piggyElements[i].UpdateElementMaskInteraction(false);

                            bool isSuperFree = _extraState11003.IsSuperFreeGame();

                            ulong piggyWinChips = 0;
                           
                            if (isSuperFree)
                            {
                                piggyWinChips = (ulong)_extraState11003
                                    .GetEachPigWinChipsInSuperFree();
                            }
                            else
                            {
                                piggyWinChips = (ulong) _extraState11003.GetEachPigWinChipInFree();
                            }

                            AddWinChipsToControlPanel(piggyWinChips,1,true,false, "Symbol_SmallWin_11003");
                            
                            await XUtility.WaitSeconds(0.5f, machineContext);
                        }
                    }
                    
                    //小猪的图标赢钱放在BonusGameID = 1 的WinLine中
                    // var bonusLines = runningWheel[0].wheelState.GetBonusWinLine();
                    //
                    // if (bonusLines.Count == 1)
                    // {
                    //     AddWinChipsToControlPanel(
                    //         machineContext.state.Get<BetState>().GetPayWinChips((ulong)bonusLines[0].Pay));
                    // }
                }
            }

            Proceed();
        }
    }
}