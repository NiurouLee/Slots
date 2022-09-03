// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/08/27/13:44
// Ver : 1.0.0
// Description : NextSpinPrepareProxy11001.cs
// ChangeLog :
// **********************************************

namespace GameModule
{
    public class NextSpinPrepareProxy11003:NextSpinPrepareProxy
    {
        public NextSpinPrepareProxy11003(MachineContext machineContext) : base(machineContext)
        {
            
        }


        protected override  void HandleCommonLogic()
        {
            base.HandleCommonLogic();

            if (!machineContext.state.Get<AutoSpinState>().IsAutoSpin)
            {
                machineContext.view.Get<BaseSpinMapTitleView>().EnableButtonResponse(true);
                machineContext.view.Get<JackpotPanel11003>().EnableButtonResponse(true);
            }
        }
        
        public override void OnAutoSpinStopClicked()
        {
            base.OnAutoSpinStopClicked();
            
            if (!machineContext.state.Get<AutoSpinState>().IsAutoSpin)
            {
                machineContext.view.Get<BaseSpinMapTitleView>().EnableButtonResponse(true);
                machineContext.view.Get<JackpotPanel11003>().EnableButtonResponse(true);
            }
        }

        public override void StartNextSpin()
        {
            machineContext.view.Get<BaseSpinMapTitleView>().EnableButtonResponse(false);
            machineContext.view.Get<JackpotPanel11003>().EnableButtonResponse(false);
            machineContext.view.Get<MapView11003>().HideMap();
            base.StartNextSpin();
        }
        
        protected override void OnBetChange()
        {
            base.OnBetChange();
            machineContext.view.Get<BaseSpinMapTitleView>().LockFeatureUI(!machineContext.state.Get<BetState>().IsFeatureUnlocked(2),true,false);

            UpdateJackpotWheelElementState();
            
            machineContext.view.Get<JackpotPanel11003>().UpdateJackpotLockState(true,false);
        }
        
        // protected void UpdateCoinElementState()
        // {
        //     var baseGameWheel = machineContext.view.Get<Wheel>("WheelBaseGame");
        //
        //     var coinElements = baseGameWheel.GetElementMatchFilter((container) =>
        //     {
        //         if (Constant11003.coinElement.Contains(container.sequenceElement.config.id))
        //         {
        //             return true;
        //         }
        //         
        //         return false;
        //     });
        //
        //     for (var i = 0; i < coinElements.Count; i++)
        //     {
        //         var coinElement11003 = coinElements[i].GetElement() as CoinElement11003;
        //         if (coinElement11003 != null)
        //         {
        //             coinElement11003.UpdateElementContent();
        //         }
        //     }
        // }

        protected void UpdateJackpotWheelElementState()
        {
            var jackpotWheel = machineContext.view.Get<Wheel>("WheelBaseJackpot");

            var jackpotElements = jackpotWheel.GetElementMatchFilter((container) =>
            {
                if (container.sequenceElement != null)
                {
                    if (Constant11003.jackPotElement.Contains(container.sequenceElement.config.id))
                    {
                        return true;
                    }
                }

                return false;
            });

            for (var i = 0; i < jackpotElements.Count; i++)
            {
                var jackpotElement = jackpotElements[i].GetElement() as JackpotElement11003;
                if (jackpotElement != null)
                {
                    jackpotElement.UpdateElementContent();
                }
            }
        }
        
        protected override void OnUnlockBetFeatureConfigChanged()
        {
            machineContext.view.Get<BaseSpinMapTitleView>().LockFeatureUI(!machineContext.state.Get<BetState>().IsFeatureUnlocked(2),false,false);

            UpdateJackpotWheelElementState();
            
            machineContext.view.Get<JackpotPanel11003>().UpdateJackpotLockState(false,false);
        }
    }
}