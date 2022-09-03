using System.Collections.Generic;
using DragonU3DSDK.Network.API.ILProtocol;

namespace GameModule
{
    public class WheelsActiveState11004: WheelsActiveState
    {
        public WheelsActiveState11004(MachineState machineState) : base(machineState)
        {
        }
        
        
        public override void UpdateRunningWheelState(GameResult gameResult)
        {
            UpdateRunningWheelState(gameResult.IsReSpin,gameResult.IsFreeSpin);
        }


        public bool IsLink
        {
            get;
            protected set;
        }

        public  void UpdateRunningWheelState(bool isLink,bool isFree)
        {
            if (isLink)
            {
                IsLink = true;
                UpdateRunningWheel(new List<string>() {"WheelLinkGame"});
                machineState.machineContext.view.Get<BaseTitleView11004>().Close();
                machineState.machineContext.view.Get<LinkTitleView11004>().Open();
                machineState.machineContext.view.Get<BackgroundView11004>().OpenLink();
                machineState.machineContext.state.Get<JackpotInfoState>().LockJackpot = true;
            }
            else
            {
                IsLink = false;
                UpdateRunningWheel(new List<string>() {"WheelBaseGame"});
                machineState.machineContext.view.Get<BaseTitleView11004>().Open();
                machineState.machineContext.view.Get<LinkTitleView11004>().Close();
                machineState.machineContext.state.Get<JackpotInfoState>().LockJackpot = false;
                
                if (isFree)
                {
                    machineState.machineContext.view.Get<BackgroundView11004>().OpenFree();
                }
                else
                {
                    machineState.machineContext.view.Get<BackgroundView11004>().OpenBase();
                }

            }
            
            machineState.machineContext.view.Get<BaseTitleView11004>().RefreshUI();
            
            machineState.machineContext.view.Get<LockElementView11004>().ClearLock();

        }
        

        public override string GetReelNameForWheel(Wheel wheel)
        {
            if (wheel.wheelName == "WheelBaseGame")
            {
                if (machineState.machineContext.state.Get<FreeSpinState>().IsInFreeSpin)
                {
                    return "Reels";
                }
                else
                {
                    return "FreeReels";
                }
            }
            else
            {
                return "SuperFreeReels";
            }

            return "Reels";
        }
    }
}