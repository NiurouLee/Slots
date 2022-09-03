// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/07/08/20:36
// Ver : 1.0.0
// Description : ExtraState11002.cs
// ChangeLog :
// **********************************************

using System.Collections.Generic;
using System.Threading.Tasks;
using DragonU3DSDK.Network.API.ILProtocol;
using Google.ilruntime.Protobuf.Collections;

namespace GameModule
{
    public class ExtraState11002 : ExtraState<WinsOPlentyGameResultExtraInfo>
    {

        private RepeatedField<ulong> titleState;

        public SSettleProcess sSettleProcess;

        public ExtraState11002(MachineState state) : base(state)
        {
        }

        public WinsOPlentyGameResultExtraInfo GetExtraInfo()
        {
            return extraInfo;
        }

        public bool HasExtraInfo()
        {
            return extraInfo != null;
        }

        public override async Task SettleBonusProgress()
        {
            sSettleProcess = await machineState.machineContext.serviceProvider.SettleGameProgress();

            if (sSettleProcess != null)
            {
                machineState.UpdateStateOnSettleProcess(sSettleProcess);
            }
        }

        public override bool HasSpecialEffectWhenWheelStop()
        {
            var totalBet = machineState.Get<BetState>().totalBet;
            var chillState = GetChillState(totalBet);
            var lastChillState = GetChillLastState(totalBet);

            for (var i = 0; i < chillState.Count; i++)
            {
                if (chillState[i] != lastChillState[i])
                {
                    return true;
                }
            }
            return false;
        }

        public override bool HasBonusGame()
        {
            var wheelBonusInfo = extraInfo?.WheelBonusInfo;

            if (wheelBonusInfo == null) { return false; }

            return wheelBonusInfo.Chosen == false || wheelBonusInfo.Settled == false;
        }

        public RepeatedField<uint> GetChillState(ulong totalBet)
        {
            if (!extraInfo.WildTitleMap.ContainsKey(totalBet))
            {
                extraInfo.WildTitleMap.Add(totalBet, CreateWildTitle());

            }
            return extraInfo.WildTitleMap[totalBet].CurrentTitle;
        }

        public RepeatedField<uint> GetChillLastState(ulong totalBet)
        {
            if (!extraInfo.WildTitleMap.ContainsKey(totalBet))
            {
                extraInfo.WildTitleMap.Add(totalBet, CreateWildTitle());

            }
            return extraInfo.WildTitleMap[totalBet].LastTitle;
        }

        public WinsOPlentyGameResultExtraInfo.Types.WildTitle CreateWildTitle()
        {
            var wildTitle = new WinsOPlentyGameResultExtraInfo.Types.WildTitle();
            wildTitle.ActiveTitle.Add(new List<bool>() { false, false, false, false, false });
            wildTitle.CurrentTitle.Add(new List<uint>() { 0, 0, 0, 0, 0, 0 });
            wildTitle.LastTitle.Add(new List<uint>() { 0, 0, 0, 0, 0, 0 });
            return wildTitle;
        }

        public bool HaveStarWild(ulong totalBet)
        {
            WinsOPlentyGameResultExtraInfo.Types.WildTitle wildTitle = null;
            if (extraInfo.WildTitleMap.TryGetValue(totalBet, out wildTitle))
            {
                for (int i = 0; i < 5; i++)
                {
                    if (wildTitle.ActiveTitle[i] && wildTitle.CurrentTitle[i] == 2)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public override bool IsBlinkFeatureTriggered(uint elementId)
        {
            if (elementId == 12 || elementId == 17)
            {
                return machineState.Get<FreeSpinState>().IsTriggerFreeSpin;
            }
            return false;
        }


        public RepeatedField<bool> GetChillActive(ulong totalBet)
        {
            if (!extraInfo.WildTitleMap.ContainsKey(totalBet))
            {
                extraInfo.WildTitleMap.Add(totalBet, CreateWildTitle());

            }
            return extraInfo.WildTitleMap[totalBet].ActiveTitle;
        }

        public bool Have5ChillWild()
        {
            int num = 0;
            var totalBet = machineState.Get<BetState>().totalBet;
            var wildTitle = extraInfo.WildTitleMap[totalBet];
            for (int i = 0; i < 5; i++)
            {
                if (wildTitle.ActiveTitle[i] || wildTitle.CurrentTitle[i] == 3)
                {
                    num++;
                }
            }

            return num == 5;
        }
    }
}