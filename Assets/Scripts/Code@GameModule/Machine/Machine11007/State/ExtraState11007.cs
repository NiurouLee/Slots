//  **********************************************
//  Copyright(c) 2021 by com.ustar
//  All right reserved
// 
//  Author : Besure.Chen
//  Date :2021-08-13 13:55
//  Ver : 1.0.0
//  Description : ExtraState11007.cs
//  ChangeLog :
//  **********************************************

using System.Collections.Generic;
using DragonU3DSDK.Network.API.ILProtocol;
using DragonU3DSDK.Network.API.Protocol;
using Google.ilruntime.Protobuf.Collections;

namespace GameModule
{
    public class ExtraState11007: ExtraState<ColossalPigsGameResultExtraInfo>
    {
        public ExtraState11007(MachineState machineState)
            : base(machineState)
        {

        }

        public bool HasLinkGame()
        {
            if (!ReferenceEquals(extraInfo, null) && extraInfo.Items.count>0)
            {
                return true;
            }
            return false;
        }
        
        //当前结算到的item id
        public uint CurrentItemId {
            get
            {
                if (!ReferenceEquals(CurrentItem, null))
                {
                    return CurrentItem.Id;
                }
                return 0;
            }
        }
        public RepeatedField<Position> GetWorldPosition()
        {
            return extraInfo.WildReplacePositions;
        }

        public ulong TotalWin => extraInfo.Win;
        //所有的Items结算完成
        public bool IsLinkRewardFinish()
        {
            if (Items.count>0)
            {
                for (int v = 0; v < Items.count; v++)
                {
                    if (!Items[v].Finished)
                    {
                        return false;
                    }
                }
                return true;
            }
            return false;
        }
        public bool IsClaimFinished()
        {
            return extraInfo.Finished;
        }
        public bool IsTriggerGrand()
        {
            return Items.count == Constant11007.MAX_LINK_COUNT;
        }
        public ColossalPigsGameResultExtraInfo.Types.Item CurrentItem {
            get
            {
                if (Items.count > 0)
                {
                    if (IsLinkRewardFinish())
                        return null;
                    for (int i = 0; i < Items.count; i++)
                    {
                        var item = Items[i];
                        if (!ReferenceEquals(item,null) && !item.Finished)
                        {
                            return item;
                        }
                    }
                }
                return null;   
            }
        }
        
        public RepeatedField<ColossalPigsGameResultExtraInfo.Types.Item> Items => extraInfo.Items;
        public override bool HasSpecialEffectWhenWheelStop()
        {
            return GetWorldPosition().Count > 0;
        }
    }
}