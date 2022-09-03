using System.Threading.Tasks;
using DragonU3DSDK.Network.API.ILProtocol;
using Google.ilruntime.Protobuf.Collections;

namespace GameModule
{
    public class ReSpinState:SubState
    {
        public bool NextIsReSpin;
        public bool ReSpinTriggered;
        public uint ReSpinCount;
        public bool ReSpinNeedSettle;
        public bool IsInRespin;
        public uint ReSpinLimit;
        public ulong ReSpinTototalWin;
        public RepeatedField<Panel> triggerPanels;
        
        public ReSpinState(MachineState state) : base(state)
        {
            
        }

        public override void UpdateStatePreRoomSetUp(SEnterGame gameEnterInfo)
        {
            base.UpdateStatePreRoomSetUp(gameEnterInfo);
            SetReSpinInfo(gameEnterInfo.GameResult.ReSpinInfo,gameEnterInfo.GameResult, true);
        }


        public override void UpdateStateOnRoomSetUp(SEnterGame gameEnterInfo)
        {
            base.UpdateStateOnRoomSetUp(gameEnterInfo);
            SetReSpinInfo(gameEnterInfo.GameResult.ReSpinInfo,gameEnterInfo.GameResult, true);
        }


        public override void UpdateStateOnReceiveSpinResult(SSpin spinResult)
        {
            base.UpdateStateOnReceiveSpinResult(spinResult);
            SetReSpinInfo(spinResult.GameResult.ReSpinInfo,spinResult.GameResult, false);
        }

        public override void UpdateStateOnBonusProcess(SBonusProcess sBonusProcess)
        {
            base.UpdateStateOnBonusProcess(sBonusProcess);
            SetReSpinInfo(sBonusProcess.GameResult.ReSpinInfo,sBonusProcess.GameResult, false);
        }
        public override void UpdateStateOnSettleProcess(SSettleProcess settleProcess)
        {
            base.UpdateStateOnSettleProcess(settleProcess);
            SetReSpinInfo(settleProcess.GameResult.ReSpinInfo,settleProcess.GameResult, false);
        }

        protected virtual void SetReSpinInfo(ReSpinInfo reSpinInfo,GameResult gameResult, bool isEnterRoom)
        {
            triggerPanels = reSpinInfo.TriggeringPanels;
            ReSpinLimit = reSpinInfo.ReSpinLimit;
            ReSpinCount = reSpinInfo.ReSpinCount;
            NextIsReSpin = reSpinInfo.ReSpinCount > 0;
            ReSpinNeedSettle = !NextIsReSpin && !reSpinInfo.IsOver;

            IsInRespin = gameResult.IsReSpin || isEnterRoom && (ReSpinNeedSettle || NextIsReSpin);
            ReSpinTriggered = !isEnterRoom && !IsInRespin && NextIsReSpin;
            ReSpinTototalWin = reSpinInfo.ReSpinTotalWin;
        }
        
        public async Task SettleReSpin()
        {
            var settleProcess = await machineState.machineContext.serviceProvider.SettleGameProgress();
            
            if (settleProcess != null)
            {
                machineState.UpdateStateOnSettleProcess(settleProcess);
            }
        }
        //
        // //解决航海报错 临时添加 
        // public override void UpdateStateOnSettleProcess(SSettleProcess settleProcess)
        // {
        //     base.UpdateStateOnSettleProcess(settleProcess);
        //     SetReSpinInfo(settleProcess.GameResult.ReSpinInfo,settleProcess.GameResult,false);
        // }

        public virtual ulong GetRespinTotalWin()
        {
            return 0;
        }
    }
}