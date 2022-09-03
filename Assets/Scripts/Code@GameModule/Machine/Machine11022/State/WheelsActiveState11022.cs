
using System;
using System.Collections.Generic;
using DragonU3DSDK.Network.API.ILProtocol;

namespace GameModule
{
    public class WheelsActiveState11022: WheelsActiveState
    {
        public bool IsBaseWheel;
        public bool IsFreeWheel;
        public bool IsLinkWheel;
        public bool IsSingleWheel;
        public WheelsActiveState11022(MachineState machineState)
            : base(machineState)
        {
            spinningOrder = WheelSpinningOrder.SAME_TIME_START_ONE_BY_ONE_STOP;
            IsBaseWheel = true;
            IsFreeWheel = false;
            IsLinkWheel = false;
            IsSingleWheel = false;
        }

        public void UpdateSingleWheelState(string singleWheelName)
        {
            IsFreeWheel = false;
            IsLinkWheel = false;
            IsSingleWheel = true;
            IsBaseWheel = false;
            ToggleJackpotPanel(true);
            // UpdateRunningWheel(new List<string>() {singleWheelName});

            bool updateReelSequence = true;
            var runningWheelsName = new List<string>() {singleWheelName};
            var machineContext = machineState.machineContext;
            for (var i = runningWheel.Count - 1; i >= 0; i--)
            {
                var wheel = runningWheel[i];
                if (wheel.wheelName == "WheelLinkGame")
                {
                    wheel.SetActive(false,true);
                }
                else
                {
                    wheel.SetActive(false);
                }
                runningWheel.Remove(wheel);
            }

            for (var i = 0; i < runningWheelsName.Count; i++)
            {
                var wheel = machineContext.view.Get<Wheel>(runningWheelsName[i]);
                if (wheel != null)
                {
                    AddRunningWheel(wheel, -1, updateReelSequence);
                }
            }
            if (machineContext.state.Get<ReelStopSoundState>() != null)
            {
                var maxRollCount = 0;
                for (int i = 0; i < runningWheel.Count; i++)
                {
                    var wheel = runningWheel[i];
                    if (wheel!=null)
                    {
                        maxRollCount = Math.Max(maxRollCount, wheel.rollCount);
                    }
                }
                machineContext.state.Get<ReelStopSoundState>().ResetRollCount(maxRollCount);   
            }
            
            // machineState.machineContext.view.Get<IndependentWheel>().transform.gameObject.SetActive(true);
        }
        public void UpdateLinkWheelState()
        {
            IsFreeWheel = false;
            IsLinkWheel = true;
            IsSingleWheel = false;
            IsBaseWheel = false;
            ToggleJackpotPanel(false);
            UpdateRunningWheel(new List<string>() {"WheelLinkGame"});
            machineState.machineContext.view.Get<BackgroundView11022>().OpenBonus();
        }
        public void UpdateBaseWheelState()
        {
            IsFreeWheel = false;
            IsLinkWheel = false;
            IsSingleWheel = false;
            IsBaseWheel = true;
            ToggleJackpotPanel(true);
            UpdateRunningWheel(new List<string>() {"WheelBaseGame"});
            machineState.machineContext.view.Get<BackgroundView11022>().OpenBase();
        }
        public void UpdateFreeWheelState()
        {
            IsFreeWheel = true;
            IsLinkWheel = false;
            IsSingleWheel = false;
            IsBaseWheel = false;
            ToggleJackpotPanel(false);
            UpdateRunningWheel(new List<string>() {"WheelFreeGame"});
            machineState.machineContext.view.Get<BackgroundView11022>().OpenFree();
        }
        
        private void ToggleJackpotPanel(bool visible)
        {
            machineState.machineContext.view.Get<JackPotPanel>().transform.gameObject.SetActive(visible);    
        }

        public override void UpdateRunningWheelState(GameResult gameResult)
        {
            var extraInfo = ProtocolUtils.GetAnyStruct<AmalgamationGameResultExtraInfo>(gameResult.ExtraInfoPb);

            if (!gameResult.IsFreeSpin && !gameResult.IsBonusGame)
            {
                UpdateRunningWheel(new List<string>() {"WheelBaseGame"},false);
                
            }
            else if (machineState.Get<ExtraState11022>().IsChoosingGameType())
            {
                UpdateRunningWheel(new List<string>() {"WheelBaseGame"},false);
            }
            else if (gameResult.IsFreeSpin)
            {
                UpdateRunningWheel(new List<string>() {"WheelFreeGame"},false);
            }
            else if (gameResult.IsBonusGame)
            {
                UpdateRunningWheel(new List<string>() {"WheelLinkGame"},false);
            }
        }

        public override string GetReelNameForWheel(Wheel wheel)
        {
            if (wheel.wheelName == "WheelFreeGame")
                return Constant11022.FreeReelsNameDictionary[machineState.Get<ExtraState11022>().GetFreeSpinTriggerCount()];

            if (wheel.wheelName == "WheelLinkGame")
            {
                return machineState.Get<ExtraState11022>().IsLinkFromChoose() ? ( machineState.Get<ExtraState11022>().IsLinkNeedInitialized()?Constant11022.SelectInitialLinkReels:Constant11022.SelectLinkReels):Constant11022.NormalLinkReels;
            }

            if (wheel.wheelName.Contains("X"))
            {
                return Constant11022.ShapeLinkReels;
            }

            return "Reels";
        }
    }
}