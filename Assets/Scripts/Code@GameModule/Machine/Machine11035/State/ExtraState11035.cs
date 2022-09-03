using UnityEngine;
using DragonU3DSDK.Network.API.ILProtocol;
using static DragonU3DSDK.Network.API.ILProtocol.HamburgerKingGameResultExtraInfo.Types;

namespace GameModule
{
    public class ExtraState11035 : ExtraState<HamburgerKingGameResultExtraInfo>
    {
        public ExtraState11035(MachineState state)
        : base(state)
        {
        }

        public uint GetCombo()
        {
            var betInfos = extraInfo.BetInfos;
            if (betInfos == null || betInfos.Count == 0) { return 0; }

            var betState = machineState.Get<BetState>();
            var bet = betState.totalBet;
            BetInfo betInfo;

            betInfos.TryGetValue(bet, out betInfo);

            return betInfo == null ? 0 : betInfo.Combo;
        }

        public bool HasJackpotWheel()
        {
            return extraInfo.JackpotInfo != null;
        }

        public bool JackpotUncollected()
        {
            return extraInfo.JackpotInfo != null && extraInfo.JackpotInfo.TotalWin > 0;
        }
    }
}