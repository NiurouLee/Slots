using System.Collections.Generic;
using DragonU3DSDK.Network.API.ILProtocol;

namespace GameModule
{
    public class JackpotState11009: JackpotInfoState
    {
        public JackpotState11009(MachineState machineState) : base(machineState)
        {
        }


        protected Dictionary<uint, JackpotWinInfo> dicJackpotWins = new Dictionary<uint, JackpotWinInfo>();
        public override void UpdateStateOnReceiveSpinResult(SSpin spinResult)
        {
            base.UpdateStateOnReceiveSpinResult(spinResult);

            dicJackpotWins.Clear();
            //BetState betState = machineState.machineContext.state.Get<BetState>();
            var winLines = spinResult.GameResult.Panels[0].WinLines;
            for (int i = 0; i < winLines.Count; i++)
            {
                if (winLines[i].JackpotId > 0)
                {
                    JackpotWinInfo winInfo = new JackpotWinInfo(winLines[i].JackpotId,winLines[i].Pay);
                    dicJackpotWins[winInfo.jackpotId] = winInfo;
                }
            }
        }


        public virtual ulong GetJackpotWinPay(uint jackpotId)
        {
            ulong jackpotWin = 0;
            JackpotWinInfo jackpotWinInfo = null;
            if (dicJackpotWins.TryGetValue(jackpotId, out jackpotWinInfo))
            {
                jackpotWin = jackpotWinInfo.jackpotPay;
            }


            return jackpotWin;
        }
    }
}