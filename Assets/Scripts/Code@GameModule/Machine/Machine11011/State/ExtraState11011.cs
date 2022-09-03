//  **********************************************
//  Copyright(c) 2021 by com.ustar
//  All right reserved
// 
//  Author : Besure.Chen
//  Date :2021-10-19 10:42
//  Ver : 1.0.0
//  Description : ExtraState11011.cs
//  ChangeLog :
//  **********************************************


using DragonU3DSDK.Network.API.ILProtocol;
using Google.Protobuf.Collections;

namespace GameModule
{
    public class ExtraState11011:ExtraState<RisingFortuneGameResultExtraInfo>
    {
        public ExtraState11011(MachineState state) : base(state)
        {
            
        }
        
        public override bool HasBonusGame()
        {
            return !extraInfo.Chosen;
        }
        
        public ulong GetGrandJackpotWinRate()
        {
            return extraInfo.LinkData.FullWinRate;
        }
        
        public override bool HasSpecialBonus()
        {
            return !IsPicked();
        }
        
        public override bool HasSpecialEffectWhenWheelStop()
        {
            return machineState.Get<WheelsActiveState>().GetRunningWheel()[0].wheelState.HasBonusLine();
        }

        public bool IsPicked()
        {
            return extraInfo.Picked;
        }
        
        public uint GetGreenMultiplier()
        {
            return extraInfo.GreenMultiplier;
        }
        
        public  int PickJackpotId=>(int)extraInfo.PickJackpotId;

        public Google.ilruntime.Protobuf.Collections.RepeatedField<RisingFortuneGameResultExtraInfo.Types.LinkData.Types.Item> GetLinkItems()
        {
            return extraInfo.LinkData.Items;
        }

        public ulong FreeGameWinRate=>extraInfo.FreeGameWinRate;
        public bool Exaggerated=>extraInfo.Exaggerated;
        public ulong FreeGameCoinTotalWinRate=>extraInfo.FreeGameCoinTotalWinRate;

        public ulong GetEachWinRate()
        {
            ulong totalWinRate = 0;
            var items = GetLinkItems();
            for (int i = 0; i < items.Count; i++)
            {
                var item = items[i];
                if (!Constant11011.IsWrapElement(item.SymbolId) && item.WinRate>0)
                {
                    totalWinRate += item.WinRate;
                }
            }
            return totalWinRate;
        }

        public ulong GetNextWinRate()
        {
            ulong totalWinRate = 0;
            var items = GetLinkItems();
            for (int i = 0; i < items.Count; i++)
            {
                var item = items[i];
                if (item.SymbolId > 0 && item.WinRate > 0)
                {
                    totalWinRate += item.WinRate;
                }
            }
            
            return totalWinRate;
        }
    }
}