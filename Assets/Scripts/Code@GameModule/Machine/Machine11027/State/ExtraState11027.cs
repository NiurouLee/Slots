using DragonU3DSDK.Network.API.ILProtocol;
using Google.ilruntime.Protobuf.Collections;

namespace GameModule
{
    public class ExtraState11027 : ExtraState<LuckyRabbitsGameResultExtraInfo>
    {
        public ExtraState11027(MachineState state) : base(state)
        {
        }

        public override bool HasSpecialEffectWhenWheelStop()
        {
            return true;
        }
        
        public override bool HasBonusGame()
        {
            return GetIsPicking() || GetIsRolling();
        }

        public RepeatedField<LuckyRabbitsGameResultExtraInfo.Types.PickItem> GetPickItems()
        {
            return extraInfo.PickItems;
        }
        
        public RepeatedField<LuckyRabbitsGameResultExtraInfo.Types.WheelItem> GetWheelItems()
        {
            return extraInfo.WheelItems;
        }

        public bool GetIsPicking()
        {
            return extraInfo.IsPicking;
        }
        
        public bool GetIsPickingOver()
        {
            return extraInfo.IsPickingOver;
        }
        
        public bool GetIsRolling()
        {
            return extraInfo.IsRolling;
        }
        
        public bool GetIsRollingOver()
        {
            return extraInfo.IsRollingOver;
        }
        
        public uint GetWheelEndIndex()
        {
            return extraInfo.WheelEndIndex;
        }
        
        public uint GetWheelNudgeIndex()
        {
            return extraInfo.WheelNudgeIndex;
        }
        
        public bool NeedRollingSettle()
        {
            return GetIsRolling() && GetIsRollingOver();
        }
        
        public bool NeedPickSettle()
        {
            return GetIsPicking() && GetIsPickingOver();
        }
        
        public ulong GetBonusTotalWin()
        {
            return extraInfo.TotalWin;
        }

        public ulong GetPanelWin()
        {
            return extraInfo.TriggeringPanels[0].WinRate * extraInfo.Bet / 100;
        }

        public ulong GetWheelWin()
        {
            return extraInfo.WheelWin;
        }
        
        public ulong GetPickWin()
        {
            return extraInfo.PickWin;
        }
        
        public uint GetPrizeLevel()
        {
            return extraInfo.PrizeLevel;
        }
        
        public uint GetPickWinJackpotId()
        {
            return extraInfo.PickJackpotId;
        }

        public override void UpdateStateOnReceiveSpinResult(SSpin spinResult)
        {
            base.UpdateStateOnReceiveSpinResult(spinResult);
            var extraInfo =  ProtocolUtils.GetAnyStruct<AmalgamationGameResultExtraInfo>(spinResult.GameResult.ExtraInfoPb);

        }

        // public uint SetOldPrizeLevel()
        // {
        //     return extraInfo.PrizeLevel;
        // }
        //
        // public uint GetOldPrizeLevel()
        // {
        //     return extraInfo.PrizeLevel;
        // }
    }
}