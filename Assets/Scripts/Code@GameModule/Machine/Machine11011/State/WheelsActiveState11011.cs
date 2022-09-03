//  **********************************************
//  Copyright(c) 2021 by com.ustar
//  All right reserved
// 
//  Author : Besure.Chen
//  Date :2021-10-19 10:47
//  Ver : 1.0.0
//  Description : WheelsActiveState11011.cs
//  ChangeLog :
//  **********************************************

using System.Collections.Generic;
using DragonU3DSDK.Network.API.ILProtocol;

namespace GameModule
{
    public class WheelsActiveState11011:WheelsActiveState
    {
        private bool _isLinkWheel;
        public bool IsLinkWheel => _isLinkWheel;
        public WheelsActiveState11011(MachineState machineState)
            : base(machineState)
        {
        }
        
        public override void UpdateRunningWheelState(GameResult gameResult)
        {
            if (gameResult.IsReSpin)
            {
                UpdateLinkWheelState();
            }
            else if (gameResult.IsFreeSpin)
            {
                UpdateFreeWheelState();
            }
            else
            {
                UpdateBaseWheelState();
            }
        }
        public void UpdateLinkWheelState()
        {
            _isLinkWheel = true;
            ToggleJackpotPanel(false);
            UpdateRunningWheel(new List<string>() {"WheelLinkGame"});
        }
        public void UpdateBaseWheelState()
        {
            _isLinkWheel = false;
            ToggleJackpotPanel(true);
            UpdateRunningWheel(new List<string>() {"WheelBaseGame"});
        }
        public void UpdateFreeWheelState()
        {
            _isLinkWheel = false;
            ToggleJackpotPanel(false);
            var freeGameWinRate = machineState.Get<ExtraState11011>().FreeGameWinRate;
            var freeGameWinRateTotal = machineState.Get<ExtraState11011>().FreeGameCoinTotalWinRate;
            UpdateRunningWheel(new List<string>() {"WheelFreeGame"});
            machineState.Get<FreeSpinState11011>().LastTotalWinRate = freeGameWinRateTotal;
            machineState.machineContext.view.Get<FeatureView11011>(1).UpdateEachWin(freeGameWinRate);
            machineState.machineContext.view.Get<FeatureView11011>(1).UpdateNextWin(freeGameWinRateTotal);
        }

        private void ToggleJackpotPanel(bool visible)
        {
            machineState.machineContext.view.Get<JackPotPanel>().transform.gameObject.SetActive(visible);    
        }
        
        public override string GetReelNameForWheel(Wheel wheel)
        {
            if (wheel.wheelName.Contains("Link"))
            {
                return Constant11010.LinkReels;
            }
            if (wheel.wheelName.Contains("Free"))
            {
                return Constant11010.FreeReels;
            }
            return "Reels";
        }
    }
}