//  **********************************************
//  Copyright(c) 2021 by com.ustar
//  All right reserved
// 
//  Author : Besure.Chen
//  Date :2021-08-20 10:53
//  Ver : 1.0.0
//  Description : ReSpinState11007.cs
//  ChangeLog :
//  **********************************************


using DragonU3DSDK.Network.API.ILProtocol;

namespace GameModule
{
    public class ReSpinState11007: ReSpinState
    {
        public ReSpinState11007(MachineState state) : base(state)
        {
            
        }
        
        protected override void SetReSpinInfo(ReSpinInfo reSpinInfo,GameResult gameResult, bool isEnterRoom)
        {
            base.SetReSpinInfo(reSpinInfo, gameResult,isEnterRoom);

            var extraInfo = ProtocolUtils.GetAnyStruct<ColossalPigsGameResultExtraInfo>(gameResult.ExtraInfoPb);
            ReSpinNeedSettle = !NextIsReSpin && !reSpinInfo.IsOver && !extraInfo.Finished;
            IsInRespin = IsInRespin || ReSpinNeedSettle;
        }

        public override ulong GetRespinTotalWin()
        {
            return machineState.Get<ExtraState11007>().TotalWin;
        }
    }
}