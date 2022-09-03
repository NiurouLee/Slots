using System.Collections.Generic;
using DragonU3DSDK.Network.API.ILProtocol;

namespace GameModule
{
    public class WheelsActiveState11005: WheelsActiveState
    {
        public WheelsActiveState11005(MachineState machineState) : base(machineState)
        {
            spinningOrder = WheelSpinningOrder.SAME_TIME_START_ONE_BY_ONE_STOP;
            
        }
        
        
        
        public override void UpdateRunningWheelState(GameResult gameResult)
        {
            
           var  tranFreeBG = machineState.machineContext.transform.Find("ZhenpingAnim/Background/SceneBackGroundFree");

            
            bool isFreeTrigger = gameResult.FreeSpinInfo.FreeSpinCount != 0 &&
                                 gameResult.FreeSpinInfo.FreeSpinCount == gameResult.FreeSpinInfo.FreeSpinTotalCount;
            if (gameResult.IsFreeSpin && !isFreeTrigger)
            {
                ExtraState11005 extraState = machineState.machineContext.state.Get<ExtraState11005>();
                uint luckCount = extraState.GetLuckCount();
                
                
                UpdateRunningWheel(Constant11005.GetListFree(luckCount),false);
                tranFreeBG.gameObject.SetActive(true);
            }
            else
            {
                
                UpdateRunningWheel(new List<string>() {"WheelBaseGame"},false);
                tranFreeBG.gameObject.SetActive(false);
            }
        }

        public override string GetReelNameForWheel(Wheel wheel)
        {
            if (wheel.wheelName == "WheelBaseGame")
                return "Reels";

            if (wheel.wheelName.Contains("WheelFreeGame"))
            {
                return $"Reels{wheel.wheelName.Replace("WheelFreeGame","")}";
            }

            return "Reels";
        }
    }
}