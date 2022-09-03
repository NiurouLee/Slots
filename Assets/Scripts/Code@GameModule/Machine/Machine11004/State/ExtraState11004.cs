using System.Collections.Generic;
using DragonU3DSDK.Network.API.ILProtocol;
using Google.ilruntime.Protobuf.Collections;

namespace GameModule
{
    public class ExtraState11004 : ExtraState<LionGoldGameResultExtraInfo>
    {
        public ExtraState11004(MachineState state) : base(state)
        {
        }


        // public override bool HasSpecialBonus()
        // {
        //     return machineState.machineContext.state.Get<ReSpinState>().IsInRespin;
        // }

        public override bool HasSpecialEffectWhenWheelStop()
        {
            return true;
        }

        public LionGoldGameResultExtraInfo.Types.LockData GetLockData()
        {
            var nowBet = machineState.machineContext.state.Get<BetState>().totalBet;
            LionGoldGameResultExtraInfo.Types.LockData lockData = null;
            extraInfo.LockStateMap.TryGetValue(nowBet, out lockData);
            return lockData;
        }




        public int GetRedLockNum()
        {
            var lockData = GetLockData();
            if (lockData != null)
            {
                int count = 0;
                for (int i = 0; i < lockData.Items.Count; i++)
                {
                    if (lockData.Items[i].Colour == Constant11004.RedFrameIndex)
                    {
                        count++;
                    }
                }

                return count;
            }

            return 0;
        }


        public int GetGreenLockNum()
        {
            var lockData = GetLockData();
            if (lockData != null)
            {
                int count = 0;
                for (int i = 0; i < lockData.Items.Count; i++)
                {
                    if (lockData.Items[i].Colour == Constant11004.GreenFrameIndex)
                    {
                        count++;
                    }
                }

                return count;
            }

            return 0;
        }

        public Dictionary<(int, int), LionGoldGameResultExtraInfo.Types.LockData.Types.Item> ChangeLockData(
            RepeatedField<LionGoldGameResultExtraInfo.Types.LockData.Types.Item> lockData)
        {
            Dictionary<(int, int), LionGoldGameResultExtraInfo.Types.LockData.Types.Item> dicLockData =
                new Dictionary<(int, int), LionGoldGameResultExtraInfo.Types.LockData.Types.Item>();
            foreach (var item in lockData)
            {
                dicLockData[((int) item.X, (int) item.Y)] = item;
            }
            
            
            return dicLockData;
        }
        
        
      


        public LionGoldGameResultExtraInfo.Types.LinkData GetLinkData()
        {
            return extraInfo.LinkData;
        }

        public Dictionary<int, LionGoldGameResultExtraInfo.Types.LinkData.Types.Item> GetLinkItems()
        {
            Dictionary<int, LionGoldGameResultExtraInfo.Types.LinkData.Types.Item> dicLink =
                new Dictionary<int, LionGoldGameResultExtraInfo.Types.LinkData.Types.Item>();

            if (extraInfo.LinkData != null)
            {
                foreach (var item in extraInfo.LinkData.Items)
                {
                    if (item.SymbolId != 0)
                    {
                        dicLink[(int)item.PositionId] = item;
                    }
                }
            }

            return dicLink;
        }
        
        public Dictionary<int, LionGoldGameResultExtraInfo.Types.LinkData.Types.Item> GetTriggerLinkItems()
        {
            Dictionary<int, LionGoldGameResultExtraInfo.Types.LinkData.Types.Item> dicLink =
                new Dictionary<int, LionGoldGameResultExtraInfo.Types.LinkData.Types.Item>();

            if (extraInfo.LinkData != null)
            {
                foreach (var item in extraInfo.LinkData.DragonTriggeringItems)
                {
                    if (item.SymbolId != 0)
                    {
                        dicLink[(int)item.PositionId] = item;
                    }
                }
            }

            return dicLink;
        }
        
        
        


        public LionGoldGameResultExtraInfo.Types.LinkData.Types.Item GetLinkItem(int positionId)
        {

            if (extraInfo.LinkData != null)
            {
                for (int i = 0; i < extraInfo.LinkData.Items.Count; i++)
                {
                    var item = extraInfo.LinkData.Items[i];
                    if (item.PositionId == positionId)
                    {
                        return item;
                    }
                }
            }
            return null;
        }
        
        
        public long GetRespinTotalWin()
        {
            long winNum = 0;
            var linkData = GetLinkData();
            if (linkData.GrandJackpot > 0)
            {
                
                var jackpotWin = this.machineState.machineContext.state.Get<BetState>().GetPayWinChips(linkData.GrandJackpot);
                winNum += (long)jackpotWin;
            }
                
            var dicLink = GetLinkItems();
            foreach (var linkKV in dicLink)
            {
                var itemLink = linkKV.Value;
                long linkWin = 0;
                if (itemLink.JackpotId > 0)
                {
                    linkWin = this.machineState.machineContext.state.Get<BetState>().GetPayWinChips((long) itemLink.JackpotPay);
                }
                else
                {
                    linkWin = this.machineState.machineContext.state.Get<BetState>().GetPayWinChips((long)itemLink.WinRate);
                }

                winNum += linkWin;
            }

            return winNum;
        }

    }
}