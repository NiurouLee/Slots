using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using DragonU3DSDK.Network.API.ILProtocol;
using UnityEngine;
namespace GameModule{
    public class ReSpinState11312 : ReSpinState
    {
        public string curSmallWheelName = "";
        public ReSpinState11312(MachineState state) : base(state)
        {
        }
         private bool IsCoinInFrames(){
            var ExtraState = machineState.Get<ExtraState11312>();
            if(ExtraState.CoinInFrame.Count!=0)
                Constant11312.LastRespinIsHasSmall = true;
            else
                Constant11312.LastRespinIsHasSmall = false;
            return ExtraState.CoinInFrame!=null && ExtraState.CoinInFrame.Count!=0;
        }
        public bool JudgeIsEarlySettle(){
            var ExtraState = machineState.Get<ExtraState11312>();
            if (ExtraState.LinkItems.Count != 0 && ExtraState.LinkItems.Count >= ExtraState.PanelHeight * 5 && !IsCoinInFrames())
                return true;
            return false;
        }
        protected override void SetReSpinInfo(ReSpinInfo reSpinInfo,GameResult gameResult, bool isEnterRoom)
        {
            triggerPanels = reSpinInfo.TriggeringPanels;
            ReSpinLimit = reSpinInfo.ReSpinLimit;
            ReSpinCount = reSpinInfo.ReSpinCount;
            NextIsReSpin = reSpinInfo.ReSpinCount < ReSpinLimit;
            ReSpinNeedSettle = (!NextIsReSpin && !reSpinInfo.IsOver) || JudgeIsEarlySettle();
            ReSpinTototalWin = reSpinInfo.ReSpinTotalWin;

            IsInRespin = gameResult.IsReSpin || isEnterRoom && (ReSpinNeedSettle || NextIsReSpin);
            ReSpinTriggered = !isEnterRoom && !IsInRespin && NextIsReSpin;
            if(JudgeIsEarlySettle()){
                NextIsReSpin = false;
            }
        }

        public override ulong GetRespinTotalWin()
        {
            return (ulong)machineState.Get<ExtraState11312>().GetRespinTotalWin() + ReSpinTototalWin;
        }
    }
}
