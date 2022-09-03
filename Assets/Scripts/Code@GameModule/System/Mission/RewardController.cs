//  **********************************************
//  Copyright(c) 2021 by com.ustar
//  All right reserved
// 
//  Author : Besure.Chen
//  Date :2021-09-26 20:00
//  Ver : 1.0.0
//  Description : RewardController.cs
//  ChangeLog :
//  **********************************************

using DragonU3DSDK.Network.API.ILProtocol;

namespace GameModule
{
    public class RewardController
    {
        private Reward _reward;
        public Reward Reward => _reward;
        public RewardController(Reward reward)
        {
            _reward = reward;
        }

        public long GetCoinAmount()
        {
            var item = XItemUtility.GetItem(_reward.Items, Item.Types.Type.Coin);
            if (item != null)
            {
                return (long)item.Coin.Amount;
            }
            return 0;
        }
        public long GetCoinsAmount()
        {
            ulong amount = 0;
            var items = XItemUtility.GetItems(_reward.Items, Item.Types.Type.Coin);
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i] != null)
                {
                    amount += items[i].Coin.Amount;
                }
            }
            return (long)amount;
        }
        public ulong GetDiamondAmount()
        {
            var item = XItemUtility.GetItem(_reward.Items, Item.Types.Type.Emerald);
            if (item != null)
            {
                return item.Emerald.Amount;
            }
            return 0;
        }
        public ulong GetMissionPointAmount()
        {
            var item = XItemUtility.GetItem(_reward.Items, Item.Types.Type.MissionPoints);
            if (item != null)
            {
                return item.MissionPoints.Amount;
            }
            return 0;
        }
        public ulong GetMissionStarAmount()
        {
            var item = XItemUtility.GetItem(_reward.Items, Item.Types.Type.MissionStar);
            if (item != null)
            {
                return item.MissionStar.Amount;
            }
            return 0;
        }

        public ulong GetGoldHammerAmount()
        {
            var item = XItemUtility.GetItem(_reward.Items, Item.Types.Type.GoldHammer);
            if (item != null)
            {
                return item.GoldHammer.Amount;
            }
            return 0;
        }
    }
}