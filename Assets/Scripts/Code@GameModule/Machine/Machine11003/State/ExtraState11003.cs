using DragonU3DSDK.Network.API.ILProtocol;
using Google.ilruntime.Protobuf.Collections;
using UnityEngine;

namespace GameModule
{
    public class ExtraState11003 : ExtraState<PiggyBankGameResultExtraInfo>
    {
        private PiggyBankGameResultExtraInfo lastExtraInfo;


        public ExtraState11003(MachineState state) : base(state)
        {
        }

        public bool NextIsSuperFreeGame()
        {
            return extraInfo.SuperFreeSpinInfo.Left > 0;
        }
         
        public bool IsSuperFreeGame()
        {
            return !extraInfo.SuperFreeSpinInfo.IsOver && extraInfo.SuperFreeSpinInfo.Left != extraInfo.SuperFreeSpinInfo.Total;
        }
        
        public bool IsSuperFreeGameNeedSettle()
        {
            return extraInfo.SuperFreeSpinInfo.Left == 0 && !extraInfo.SuperFreeSpinInfo.IsOver;
        }

        
        public bool LastIsSuperFreeGame()
        {
            return !lastExtraInfo.SuperFreeSpinInfo.IsOver;
        }

        public PiggyBankGameResultExtraInfo.Types.FreeSpinInfo GetSuperFreeSpinInfo()
        {
            return extraInfo.SuperFreeSpinInfo;
        }

        public string GetSuperFreeWheelName(PiggyBankGameResultExtraInfo inExtraInfo)
        {
            string[] superWheelName = {"WheelFreeGameSuper3x5", "WheelFreeGameSuper4x5", "WheelFreeGameSuper3x6", "WheelFreeGameSuper4x6"};
           
            var index = 0;          
           
            for (int i = 0; i < inExtraInfo.Buffs.count; i++)
            {
                if (inExtraInfo.Buffs[i].Acquired)
                {
                    if (inExtraInfo.Buffs[i].Type == PiggyBankGameResultExtraInfo.Types.Buff.Types.Type.AddColumn)
                    {
                        index |= 2;;
                    }
                    else if (inExtraInfo.Buffs[i].Type == PiggyBankGameResultExtraInfo.Types.Buff.Types.Type.AddRow)
                    {
                        index |= 1;
                    }
                }
            }
            
            return superWheelName[index];
        }   
        
        public Wheel GetSuperFreeGameRunningWheel()
        {
            Wheel wheel = null;
            
            if (!extraInfo.SuperFreeSpinInfo.IsOver)
            {
                wheel = machineState.machineContext.view.Get<Wheel>(GetSuperFreeWheelName(extraInfo));
            }
            
            return wheel;
        }
        
        public bool IsAddSymbolsBuffer()
        {
            if (extraInfo.SuperFreeSpinInfo.Left > 0)
            {
                for (int i = 0; i < extraInfo.Buffs.count; i++)
                {
                    if (extraInfo.Buffs[i].Acquired)
                    {
                        if (extraInfo.Buffs[i].Type == PiggyBankGameResultExtraInfo.Types.Buff.Types.Type.AddSymbols)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }
        
        public override void UpdateStateOnRoomSetUp(SEnterGame gameEnterInfo)
        {
            lastExtraInfo = extraInfo;
            base.UpdateStateOnRoomSetUp(gameEnterInfo);
        }
        
        public override void UpdateStateOnReceiveSpinResult(SSpin spinResult)
        {
            lastExtraInfo = extraInfo;
            base.UpdateStateOnReceiveSpinResult(spinResult);
        }

        public override void UpdateStateOnBonusProcess(SBonusProcess sBonusProcess)
        {
            lastExtraInfo = extraInfo;
            base.UpdateStateOnBonusProcess(sBonusProcess);
      
        }

        public override bool HasSpecialBonus()
        {
            if (extraInfo?.Wheel != null)
            {
                return extraInfo.Wheel.Chosen == false;
            }

            return false;
        }

        public uint GetPotLevel()
        {
            return extraInfo.Pot.Level;
        }

        public RepeatedField<PiggyBankGameResultExtraInfo.Types.Buff> GetBuffers()
        {
            return extraInfo.Buffs;
        }
        
        public RepeatedField<PiggyBankGameResultExtraInfo.Types.Buff> GetLastBuffers()
        {
            return lastExtraInfo?.Buffs;
        } 
        
        public PiggyBankGameResultExtraInfo.Types.Wheel GetBonusWheelInfo()
        {
            return extraInfo.Wheel;
        }

        public long GetEachPigWinChipInFree()
        {
            var freeBet = machineState.Get<BetState>().totalBet;
            return (long) freeBet * extraInfo.NormalPiggySafeRate / 100;
        }
        
        public long GetEachPigWinChipsInSuperFree()
        {
            return (long)extraInfo.Pot.AvgBet * extraInfo.SuperWinRate / 100;
        }

        public float GetCoinCollectProgress()
        {
            if (extraInfo.Pot.Progress == 0)
                return 0;
            return extraInfo.Pot.Progress/ (float) extraInfo.Pot.Amount;
        }

        public ulong GetFreeGamePigCoins()
        {
           return (ulong)extraInfo.NormalPiggySafeRate * machineState.Get<BetState>().totalBet / 100;
        }
        
        public ulong GetSuperFreeGamePigCoins()
        {
            return (ulong)extraInfo.SuperWinRate * extraInfo.Pot.AvgBet / 100;
        }
    }
}