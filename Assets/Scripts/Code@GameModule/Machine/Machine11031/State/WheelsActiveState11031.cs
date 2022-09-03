using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.Remoting.Contexts;
using DragonU3DSDK.Network.API.ILProtocol;
using GameModule;

namespace GameModule
{
    public class WheelsActiveState11031 : WheelsActiveState
    {
        public WheelsActiveState11031(MachineState machineState)
            : base(machineState)
        {
        }

        public void ShowRollsMasks(Wheel wheel)
        {
            var roll = wheel.transform.Find("Rolls");
            if (roll == null)
            {
                return;
            }
            roll.Find("spiningMask").gameObject.SetActive(true);
        }

        public void FadeOutRollMask(Wheel wheel)
        {
            var roll = wheel.transform.Find("Rolls");
            if (roll == null)
            {
                return;
            }
	        roll.Find("spiningMask").gameObject.SetActive(false);
        }

        public bool IsInLink { get; protected set; }

        public override void UpdateRunningWheelState(GameResult gameResult)
        {
            UpdateRunningWheelState(gameResult.IsReSpin, gameResult.IsFreeSpin, false);
        }

        public void UpdateRunningWheelState(bool isLink, bool isFree, bool updateReelSequence = true)
        {
            IsInLink = false;
            if (isLink)
            {
                IsInLink = true;
                var oneLinkMode = machineState.machineContext.state.Get<ExtraState11031>().IsOneLinkMode();
                if (oneLinkMode)
                {
                    UpdateRunningWheel(new List<string>() {"WheeRespinGame"}, updateReelSequence);
                }
                else
                {
                    UpdateRunningWheel(Constant11031.ListMultiplrLink, updateReelSequence);
                }
            }
            else
            {
                UpdateRunningWheel(new List<string>() {"WheelBaseGame"}, updateReelSequence);
            }
        }
        
        public override string GetReelNameForWheel(Wheel wheel)
        {
            if (wheel.wheelName == "WheelBaseGame")
            {
                return "Reels";
            }

            if (wheel.wheelName == "WheeRespinGame")
            {
                return "ReSpinReels";
            }

            if (wheel.wheelName == "WheeRespinGame1" || wheel.wheelName == "WheeRespinGame2" ||
                wheel.wheelName == "WheeRespinGame3"
                || wheel.wheelName == "WheeRespinGame4")
            {
                return "BlastReSpinReels";
            }

            return "Reels";
        }
    }
}