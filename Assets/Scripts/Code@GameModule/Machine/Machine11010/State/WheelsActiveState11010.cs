//  **********************************************
//  Copyright(c) 2021 by com.ustar
//  All right reserved
// 
//  Author : Besure.Chen
//  Date :2021-09-01 15:55
//  Ver : 1.0.0
//  Description : WheelActiveState11010.cs
//  ChangeLog :
//  **********************************************

using System.Collections.Generic;
using DragonU3DSDK.Network.API.ILProtocol;

namespace GameModule
{
    public class WheelsActiveState11010: WheelsActiveState
    {
        public bool isLinkWheel;
        private string[] strBgWheelSuffix = {"BGBaseGame", "BGLinkGame", "BGFreeGame"};
        public WheelsActiveState11010(MachineState machineState)
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
            isLinkWheel = true;
            UpdateWheelBg("BGLinkGame");
            UpdateRunningWheel(new List<string>() {"WheelLinkGame"});
        }
        public void UpdateBaseWheelState()
        {
            isLinkWheel = false;
            UpdateWheelBg("BGBaseGame");
            UpdateRunningWheel(new List<string>() {"WheelBaseGame"});
        }
        public void UpdateFreeWheelState()
        {
            isLinkWheel = false;
            UpdateWheelBg("BGFreeGame");
            UpdateRunningWheel(new List<string>() {"WheelFreeGame"});
        }

        private void UpdateWheelBg(string strWheel)
        {
            for (int i = 0; i < strBgWheelSuffix.Length; i++)
            {
                var suffix = strBgWheelSuffix[i];
                var goWheelBg = machineState.machineContext.transform.Find("Background/" + suffix);
                goWheelBg.gameObject.SetActive(strWheel == suffix);
            }
    
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