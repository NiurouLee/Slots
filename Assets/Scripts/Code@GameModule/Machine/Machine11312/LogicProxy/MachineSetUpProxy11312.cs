using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace GameModule{
    public class MachineSetUpProxy11312 : MachineSetUpProxy
    {
        public ExtraState11312 ExtraSatte;
        public MachineSetUpProxy11312(MachineContext context) : base(context)
        {
        }
        protected override void HandleCustomLogic(){
            base.HandleCustomLogic();

            var freeSpin = machineContext.state.Get<FreeSpinState>();
            var respin = machineContext.state.Get<ReSpinState11312>();
            if(freeSpin.IsOver && !respin.IsInRespin){
                Constant11312.UIRespinFeature = PopUpManager.Instance.ShowPopUp<UIRespinFeature11312>("UIRespinFeature" + machineContext.assetProvider.AssetsId);
            }
                
            
            // 初始化设置bet是否锁定
            var betState = machineContext.state.Get<BetState>();
            machineContext.view.Get<JackpotView11312>().InitSetJackpotStatus(betState.IsFeatureUnlocked(0));
            if(respin.IsInRespin && !freeSpin.IsInFreeSpin){
                ulong totalChips = respin.GetRespinTotalWin();
                machineContext.view.Get<ControlPanel>().UpdateWinLabelChipsWithAnimation((long)totalChips,0,false);
            }
            else if(freeSpin.IsInFreeSpin && !respin.IsInRespin){
                ulong totalChips = freeSpin.TotalWin;
                machineContext.view.Get<ControlPanel>().UpdateWinLabelChipsWithAnimation((long)totalChips,0,false);
            }else if(freeSpin.IsInFreeSpin && respin.IsInRespin){
                ulong totalChips = respin.GetRespinTotalWin();
                totalChips += freeSpin.TotalWin;
                machineContext.view.Get<ControlPanel>().UpdateWinLabelChipsWithAnimation((long)totalChips,0,false);
                machineContext.view.Get<ControlPanel>().UpdateControlPanelState(false,false);
            }

        }

        protected override void UpdateRunningWheelElement()
        {
            var wheelsRunningStatusState = machineContext.state.Get<WheelsActiveState>() as WheelsActiveState11312;
            
            var extraState = machineContext.state.Get<ExtraState11312>();
            var respinState = machineContext.state.Get<ReSpinState11312>();
            wheelsRunningStatusState.SetSpinningOder(WheelSpinningOrder.SAME_TIME_START_ONE_BY_ONE_STOP);
            // link 特殊处理 不在这做恢复轮盘逻辑
            if(respinState.IsInRespin){
                return;
            } 
            var wheels = wheelsRunningStatusState.GetRunningWheel();
            for (var i = 0; i < wheels.Count; i++)
            {
                wheels[i].ForceUpdateElementOnWheel();
            }
        }

    }
}

