using System.Collections;
using System.Collections.Generic;
using DragonU3DSDK.Network.API.ILProtocol;
using UnityEngine;
namespace GameModule{
    public class WheelsActiveState11312 : WheelsActiveState
    {
        SSpin currentSpinResult;
        public WheelsActiveState11312(MachineState machineState) : base(machineState)
        {
             spinningOrder = WheelSpinningOrder.SAME_TIME_START_ONE_BY_ONE_STOP;
        }
        public override void UpdateStateOnReceiveSpinResult(SSpin spinResult)
        {
            currentSpinResult = spinResult;

            base.UpdateStateOnReceiveSpinResult(spinResult);
        }
        public Panel GetPanel()
        {
            return currentSpinResult != null ? currentSpinResult.GameResult.Panels[0] : null;
        }
        public override void UpdateRunningWheelState(GameResult gameResult)
        {
            var freeSpinState = machineState.Get<FreeSpinState>();
            var reSpinState = machineState.Get<ReSpinState>();
            
            SetSpinningOder(WheelSpinningOrder.SAME_TIME_START_ONE_BY_ONE_STOP);
            if (reSpinState.NextIsReSpin || reSpinState.ReSpinNeedSettle){
                UpdateRunningWheel(new List<string>() { Constant11312.WheelName[2] }, false);
                var respinWheel = machineState.machineContext.view.Get<IndependentWheel>(0);
                respinWheel.wheelState.UpdateCurrentActiveSequence(GetReelNameForWheel(respinWheel));
                respinWheel.ForceUpdateElementOnWheel(false,true);
                machineState.machineContext.state.Get<JackpotInfoState>().LockJackpot = true;
            }else if (freeSpinState.NextIsFreeSpin){
                UpdateRunningWheel(new List<string>() { Constant11312.WheelName[1] }, true);
                machineState.machineContext.state.Get<JackpotInfoState>().LockJackpot = false;
            }  
            else{
                UpdateRunningWheel(new List<string>() { Constant11312.WheelName[0] }, true);
                machineState.machineContext.state.Get<JackpotInfoState>().LockJackpot = false;
            }
                

        }
        /// <summary>
        /// respin里切换转盘
        /// </summary>
        public void UpdateSmallRunningWheel(bool isSmall){
            var closeId = 2;
            var targetId = 3;
            if(!isSmall){
                closeId = 3;
                targetId = 2;
            }

            var respinWheel = machineState.machineContext.view.Get<Wheel>(Constant11312.WheelName[closeId]);
            runningWheel.Remove(respinWheel);

            var wheel = machineState.machineContext.view.Get<Wheel>(Constant11312.WheelName[targetId]);
            AddRunningWheel(wheel,-1,false);
            wheel.wheelState.UpdateCurrentActiveSequence(GetReelNameForWheel(wheel));
            wheel.wheelState.InitializeWheelState(wheel.wheelName);
        }
        /// <summary>
        /// respin初始化小转盘
        /// </summary>
        public void InitSmallWheel(bool isShow=true){
            var wheel = machineState.machineContext.view.Get<Wheel>(Constant11312.WheelName[3]);
            if(wheel==null) return;
            if(isShow){
                wheel.SetActive(true);
                wheel.wheelState.UpdateCurrentActiveSequence(GetReelNameForWheel(wheel));
                wheel.ForceUpdateElementOnWheel(true,true);
            }
            else{
                RemoveRunningWheel(wheel);
            }
        }
        /// <summary>
        /// 切换卷轴 是为了过渡，小轮盘玩法会根据panel.rellsId来切换对应的卷轴
        /// </summary>
        /// <param name="wheel"></param>
        /// <returns></returns>
        public override string GetReelNameForWheel(Wheel wheel)
        {
            if(wheel.wheelName == "WheelLinkGame")
			{
                var extraInfo = machineState.Get<ExtraState11312>();
                if(extraInfo.AddedToReels>0)
				    return "YellowFrameReSpinReels";
				return "BlueFrameReSpinReels";
			}
			else if(wheel.wheelName == "WheelFreeGame")
			{
				return "FreeReels";
			}
            else if(wheel.wheelName == "WheelSmallGame")
			{
				return "BlueCoinReels";
			}

            return "Reels";
        }

    }
}

