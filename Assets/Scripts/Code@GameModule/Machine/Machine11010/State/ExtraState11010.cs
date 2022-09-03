//  **********************************************
//  Copyright(c) 2021 by com.ustar
//  All right reserved
// 
//  Author : Besure.Chen
//  Date :2021-08-31 17:42
//  Ver : 1.0.0
//  Description : ExtraState11010.cs
//  ChangeLog :
//  **********************************************

using DragonU3DSDK.Network.API.ILProtocol;
using Google.ilruntime.Protobuf.Collections;


/*================= LockItLinkDiamondMine ================*/
/*message LockItLinkDiamondGameResultExtraInfo {
    map<uint32, uint32> drag_reel_position_map = 1; // key: col 0-4, value: postionId; drag wild
    message LinkRegion {
        repeated uint32 connected_position_ids = 1;
    }
    message  LinkJackpot {
        uint32 jackpot_id = 1;
        uint64 jackpot_pay = 2;
    }
    message LinkItem {
        uint32 position_id = 1; // 位置
        uint32 symbol_id = 2; // 现实图标
        uint64 win_rate = 3; // 赢钱倍率
    }
    message LinkData {
        repeated LinkItem items = 1;  // 0 - 14
        repeated LinkRegion regions = 2; // 联通域
        repeated LinkJackpot jackpots = 3; // 初始jackpots
    }
    LinkData link_data = 2; // link data
}*/
namespace GameModule
{
    public class ExtraState11010: ExtraState<LockItLinkDiamondGameResultExtraInfo>
    {
        public ExtraState11010(MachineState state) : base(state)
        {
            
        }

        public RepeatedField<LockItLinkDiamondGameResultExtraInfo.Types.LinkRegion> GetLinkRegions()
        {
            return extraInfo.LinkData.Regions;
        }

        public  RepeatedField<LockItLinkDiamondGameResultExtraInfo.Types.LinkItem> GetLinkItems()
        {
            return extraInfo.LinkData.Items;
        }

        public LockItLinkDiamondGameResultExtraInfo.Types.LinkItem GetLinkItemByRollId(int id)
        {
            for (int i = 0; i < extraInfo.LinkData.Items.Count; i++)
            {
                if (id == extraInfo.LinkData.Items[i].PositionId)
                {
                    return extraInfo.LinkData.Items[i];
                }
            }
            return null;
        }

        public RepeatedField<LockItLinkDiamondGameResultExtraInfo.Types.LinkJackpot> GetLinkJackpots()
        {
            return extraInfo.LinkData.Jackpots;
        }

        public MapField<uint, int> GetDragStackWildPos()
        {
            return extraInfo.DragReelPositionMap;
        }
        
        public override bool HasSpecialEffectWhenWheelStop()
        {
            return GetDragStackWildPos().Keys.Count > 0;
        }

        public bool IsTriggerGrand()
        {
            var items = GetLinkItems();
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].SymbolId<=0)
                {
                    return false;
                }
            }
            return true;
        }
        
        public ulong GetJackpotsWinChips()
        {
            ulong jackpotWin = 0;
            var jackpots = GetLinkJackpots();
            if (jackpots.Count > 0)
            {
                for (int i = 0; i < jackpots.Count; i++)
                {
                    jackpotWin += machineState.Get<BetState>().GetPayWinChips(jackpots[i].JackpotPay);
                }
            }
            return jackpotWin;
        }
    }
}