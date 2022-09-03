using System.Collections.Generic;
using BestHTTP.SecureProtocol.Org.BouncyCastle.Crypto.Digests;
using DragonU3DSDK.Network.API.ILProtocol;
using Google.ilruntime.Protobuf.Collections;
using UnityEngine;

namespace GameModule
{
    public class ExtraState11031 : ExtraState<ChilliFrenzyGameResultExtraInfo>
    {
        public Dictionary<Wheel, List<int>> LinkLockElementIdsList = new Dictionary<Wheel, List<int>>();

        public Dictionary<int, int> FlyJackpotNumList = new Dictionary<int, int>();

        public List<int> selectedLetterList = new List<int>();

        public Dictionary<uint, uint> jackpotCurrentNumList;

        public bool needTransView = false;

        public List<GameObject> flyChilliList = new List<GameObject>();

        public uint oldActivePanelConut = 0;

        public ExtraState11031(MachineState state) : base(state)
        {
            jackpotCurrentNumList = new Dictionary<uint, uint>();
        }

        public override bool HasSpecialEffectWhenWheelStop()
        {
            return true;
        }

        public override bool HasBonusGame()
        {
            return GetMapIsStarted();
        }

        public void SetLockElementIdsList(Wheel wheel, int id)
        {
            LinkLockElementIdsList.Add(wheel, new List<int>() {id});
        }

        public void SetSelectedLetterList(int i)
        {
            selectedLetterList.Add(i);
        }

        public void ClearSelectedLetterList()
        {
            selectedLetterList.Clear();
        }

        public void SetFlyChilliList(GameObject chilliIcon)
        {
            flyChilliList.Add(chilliIcon);
        }

        public void ClearFlyChlliList()
        {
            flyChilliList.Clear();
        }

        public void DestroyFlyChilliList()
        {
            for (var i = 0; i < flyChilliList.Count; i++)
            {
                GameObject.Destroy(flyChilliList[i]);
            }
        }

        public void SetLockElementId(Wheel wheel, int id)
        {
            List<int> idList = LinkLockElementIdsList[wheel];
            idList.Add(id);
        }

        public void ClearLockElementIdsList()
        {
            LinkLockElementIdsList.Clear();
        }

        public bool IsTriggerLinkGreenPepper()
        {
            return extraInfo.LinkTriggeredBy6PepperAndGreen && !extraInfo.LinkTriggeredBy3YellowPepper;
        }

        public override bool HasSpecialBonus()
        {
            if (IsMapLevelUp() && GetMapRound() == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool IsMapLevelUp()
        {
            var isReSpin = machineState.Get<ReSpinState>().IsInRespin;
            return extraInfo.MapBonus.LevelUp && !isReSpin;
        }

        public bool IsTriggerLinkYellowPepper()
        {
            return extraInfo.LinkTriggeredBy3YellowPepper && !extraInfo.LinkTriggeredBy6PepperAndGreen;
        }

        public bool IsOneLinkMode()
        {
            return extraInfo.LinkTriggeredBy6PepperAndGreen && !extraInfo.LinkTriggeredBy3YellowPepper;
        }

        public bool FromOneLinkModeToFourLinkMode()
        {
            return extraInfo.LinkTriggeredBy3YellowPepper && extraInfo.LinkTriggeredBy6PepperAndGreen;
        }

        public bool IsFourLinkMode()
        {
            return extraInfo.LinkTriggeredBy3YellowPepper;
        }

        public void SetNeedTransView(bool isShow)
        {
            needTransView = isShow;
        }

        public bool GetNeedTransView()
        {
            return needTransView;
        }

        public uint GetLinkPepperCount()
        {
            return extraInfo.LinkPepperCount;
        }

        public ulong GetLinkPepperWinRate()
        {
            return extraInfo.LinkPepperWinRate;
        }

        public uint GetLinkPepperJackpotId()
        {
            return extraInfo.LinkPepperJackpotId;
        }

        public ulong GetLinkPepperJackpotPay()
        {
            return extraInfo.LinkPepperJackpotPay;
        }

        public uint GetLinkActivePanelCount()
        {
            return extraInfo.LinkActivePanelCount;
        }

        public uint GetLinkOldActivePanelCount()
        {
            return oldActivePanelConut;
        }

        public void SetLinkOldPanelCount(uint num)
        {
            oldActivePanelConut = num;
        }

        public RepeatedField<ChilliFrenzyGameResultExtraInfo.Types.LinkData> GetLastLinkData()
        {
            return lastExtraInfo.LinkDataList;
        }

        public RepeatedField<ChilliFrenzyGameResultExtraInfo.Types.LinkData> GetLinkData()
        {
            return extraInfo.LinkDataList;
        }

        public List<ChilliFrenzyGameResultExtraInfo.Types.LinkData.Types.LinkItem> GetNewLinkItem(int linkIndex)
        {
            var linkData = extraInfo.LinkDataList[linkIndex];
            var newLinkItemList = new List<ChilliFrenzyGameResultExtraInfo.Types.LinkData.Types.LinkItem>();

            for (var i = 0; i < linkData.Items.count; i++)
            {
                if (IsNewLinkItem(linkIndex, linkData.Items[i]))
                {
                    newLinkItemList.Add(linkData.Items[i]);
                }
            }

            return newLinkItemList;
        }

        public List<ChilliFrenzyGameResultExtraInfo.Types.LinkData.Types.LinkItem> GetLinkItem(int linkIndex)
        {
            var linkData = extraInfo.LinkDataList[linkIndex];
            var newLinkItemList = new List<ChilliFrenzyGameResultExtraInfo.Types.LinkData.Types.LinkItem>();

            for (var i = 0; i < linkData.Items.count; i++)
            {
                if (linkData.Items[i].SymbolId > 0)
                    newLinkItemList.Add(linkData.Items[i]);
            }

            return newLinkItemList;
        }

        public bool IsNewLinkItem(int intLinkIndex,
            ChilliFrenzyGameResultExtraInfo.Types.LinkData.Types.LinkItem linkItem)
        {
            if (lastExtraInfo != null && lastExtraInfo.LinkDataList.Count > intLinkIndex)
            {
                var items = lastExtraInfo.LinkDataList[intLinkIndex].Items;
                if (linkItem.SymbolId > 0)
                {
                    if (items[(int) linkItem.PositionId].PositionId == linkItem.PositionId)
                    {
                        if (items[(int) linkItem.PositionId].SymbolId == 0)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        public void ResetJackpotAccNum()
        {
            jackpotCurrentNumList.Clear();
            if (extraInfo != null && extraInfo.JackpotTitleList != null)
            {
                var count = extraInfo.JackpotTitleList.Count;
                for (int i = 0; i < count; i++)
                {
                    var info = extraInfo.JackpotTitleList[i];
                    jackpotCurrentNumList[info.JackpotId] = info.OldNum;
                }
            }
        }

        public void AddJackpotNum(uint jackpotId)
        {
            jackpotCurrentNumList[jackpotId]++;

            if (extraInfo.JackpotTitleList[(int) jackpotId - 1].Num < jackpotCurrentNumList[jackpotId])
            {
                XDebug.LogError("AddJackpotNum");
            }
        }

        public uint GetJackpotAccNum(uint jackpotId)
        {
            return jackpotCurrentNumList[jackpotId];
        }

        public ulong GetJackpotPay(uint jackpotId)
        {
            var accNum = GetJackpotAccNum(jackpotId);
            var payIndex = accNum / 3 - 1;
            for (var j = 0; j < extraInfo.JackpotTitleList.count; j++)
            {
                var jackpotInfo = extraInfo.JackpotTitleList[j];
                if (jackpotInfo.JackpotId == jackpotId)
                {
                    if (jackpotInfo.PayList.Count > payIndex)
                        return jackpotInfo.PayList[(int) payIndex];
                    XDebug.LogError("PayNotFond........");
                }
            }

            return 0;
        }

        public RepeatedField<ChilliFrenzyGameResultExtraInfo.Types.JackpotTitle> GetJackpotTitleList()
        {
            return extraInfo.JackpotTitleList;
        }

        //地图等级
        public uint GetMapLevel()
        {
            return extraInfo.MapBonus.Level;
        }

        //地图
        public uint GetMapPoint()
        {
            return extraInfo.MapBonus.MapCount;
        }


        //地图
        public uint GetMapMaxPoint()
        {
            return extraInfo.MapBonus.MapMaxCount;
        }

        //Map分值
        public RepeatedField<ChilliFrenzyGameResultExtraInfo.Types.MapBonus.Types.Multiplier> GetMapMultipliers()
        {
            return extraInfo.MapBonus.Multipliers;
        }

        //Map字母
        public RepeatedField<ChilliFrenzyGameResultExtraInfo.Types.MapBonus.Types.Letter> GetMapLetters()
        {
            return extraInfo.MapBonus.Letters;
        }

        //一上来就被选中的字母
        public uint GetHoldenLetter()
        {
            return extraInfo.MapBonus.HoldenLetter;
        }

        //当前被开奖的字母
        public uint GetCurrentLetter()
        {
            return extraInfo.MapBonus.CurrentLetter;
        }

        //当前被开奖的字母的分值
        public uint GetCurrentMultiplier()
        {
            return extraInfo.MapBonus.CurrentMultiplier;
        }

        //当前被开奖的字母的假分值:如果没有这把不忽悠则等于0
        public uint GetFakeMultiplier()
        {
            return extraInfo.MapBonus.FakeMultiplier;
        }

        //当前被开奖的字母的假分值:如果没有这把不忽悠则等于0
        public uint GetMapRound()
        {
            return extraInfo.MapBonus.Round;
        }

        //1-5轮一开始要选几个字母
        public uint GetNumOfLetterToSelect()
        {
            return extraInfo.MapBonus.NumOfLetterToSelect;
        }

        //当前轮是否刚进入select状态
        public bool GetMapIsSelect()
        {
            return extraInfo.MapBonus.IsSelect;
        }

        //当前轮是否进入offer状态
        public bool GetMapIsOffer()
        {
            return extraInfo.MapBonus.IsOffer;
        }

        //历史所有offer，最后一个就是当前轮的offer
        public RepeatedField<uint> GetAllOffers()
        {
            return extraInfo.MapBonus.Offers;
        }

        //是否是final轮，也可用round==6判断
        public bool GetMapIsFinal()
        {
            return extraInfo.MapBonus.IsFinal;
        }

        // 最终结果的分值，来自offer的时候从了 或者 final二选一
        public uint GetMapFinalMultiplier()
        {
            return extraInfo.MapBonus.FinalMultiplier;
        }

        //开始奖金， start_prize*final_multiplier等于totalWin
        public ulong GetMapStartPrize()
        {
            return extraInfo.MapBonus.StartPrize;
        }

        //BonusBet
        public ulong GetMapBet()
        {
            return extraInfo.MapBonus.Bet;
        }

        //BonusWin
        public ulong GetMapPreWin()
        {
            return extraInfo.MapBonus.PreWin;
        }

        //BonusTotalWin
        public ulong GetMapTotalWin()
        {
            return extraInfo.MapBonus.TotalWin;
        }

        public bool GetMapIsStarted()
        {
            return extraInfo.MapBonus.IsStarted;
        }

        public bool GetMapToSettle()
        {
            return extraInfo.MapBonus.ToSettle;
        }

        public int GetPayIndex(int pay)
        {
            int index = 0;
            if (Constant11031.WinRateLists.Contains(pay))
            {
                for (var i = 0; i < Constant11031.WinRateLists.Count; i++)
                {
                    if (Constant11031.WinRateLists[i] == pay)
                    {
                        index = i;
                    }
                }
            }

            return index;
        }
    }
}