// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date :2021-07-05 12:05 PM
// Ver : 1.0.0
// Description : BetState.cs
// ChangeLog :
// **********************************************

using System.Collections.Generic;
using DragonU3DSDK.Network.API.ILProtocol;
using Google.ilruntime.Protobuf.Collections;

namespace GameModule
{
    public class BetState : SubState
    {
        //当前的使用的TotalBet
        public ulong totalBet;
        
        //上一次的使用的TotalBet
        public ulong lastTotalBet;
         
        //玩家可以的BET列表
        protected List<ulong> betList;
       
        //玩家当前BET在List中的索引
        public int betLevel;
        
        protected List<ulong> currentUnlockBetConfig;
        
        protected RepeatedField<GameUnlockConfig> gameUnlockConfig;
        protected RepeatedField<GameBetConfig> betConfigs;
        protected RepeatedField<ulong> ultraBets;
       
        //玩家当前解锁能玩的最大BET
        protected ulong normalMaxBet = 0;

        
        public bool IsMaxBet()
        {
            return totalBet == betList[betList.Count - 1];
        }
        
        public bool IsMinBet()
        {
            return totalBet == betList[0];;
        }

        public bool IsExtraBet()
        {
            return totalBet > normalMaxBet;
        }
        
        public BetState(MachineState state) : base(state)
        {
            
        }

        public override void UpdateStatePreRoomSetUp(SEnterGame gameEnterInfo)
        {
           // base.UpdateStateOnRoomSetUp();
           betConfigs = gameEnterInfo.GameConfigs[0].Bets;

           gameUnlockConfig = gameEnterInfo.GameConfigs[0].Unlocks;

           ultraBets = gameEnterInfo.UltraBets;
           
           var bets =  machineState.machineContext.serviceProvider.GetAvailableBetList(betConfigs);
           
           InitializeBetList(bets);
           InitializeRecommendBetLevel();
           
           UpdateGameUnlockConfig();
            
           if (gameEnterInfo.GameResult.IsFreeSpin)
           {
               totalBet = gameEnterInfo.GameResult.FreeSpinInfo.FreeSpinBet;
           }

           if (gameEnterInfo.GameResult.BonusGame > 0)
           {
               totalBet = gameEnterInfo.GameResult.Bet;
           }

           if (gameEnterInfo.GameResult.IsReSpin)
           {
               totalBet = gameEnterInfo.GameResult.ReSpinInfo.ReSpinBet;
           }

        }

        public void CheckAndApplyNewUnlockBetList()
        {
            var bets =  machineState.machineContext.serviceProvider.GetAvailableBetList(betConfigs);

            List<ulong> newUnlockedBets = new List<ulong>();
           
            for (var i = 0; i < bets.Count; i++)
            {
                if (!betList.Contains(bets[i]))
                {
                    newUnlockedBets.Add(bets[i]);
                }
                
                if (bets[i] > normalMaxBet)
                {
                    normalMaxBet = bets[i];
                }
            }

            if (newUnlockedBets.Count > 0)
            {
                newUnlockedBets = GetValidNewUnlockedBets(newUnlockedBets);

                for (var i = 0; i < newUnlockedBets.Count; i++)
                {
                    betList.Add(newUnlockedBets[i]);
                }
                
                betList.Sort();

                for (var i = 0; i < betList.Count; i++)
                {
                    if (betList[i] == totalBet)
                    {
                        betLevel = i;
                        break;
                    }
                }
            }

            UpdateGameUnlockConfig();
        }

        protected virtual List<ulong> GetValidNewUnlockedBets(List<ulong> list)
        {
            return list;
        }

        public void InitializeBetList(RepeatedField<ulong> inBetList)
        {
            betList = new List<ulong>();
            for (var i = 0; i < inBetList.Count; i++)
            {
                betList.Add(inBetList[i]);

                if (inBetList[i] > normalMaxBet)
                {
                    normalMaxBet = inBetList[i];
                }
            }
            
            for (var i = 0; i < ultraBets.Count; i++)
            {
                if (!betList.Contains(ultraBets[i]))
                {
                    betList.Add(ultraBets[i]);
                }
            }
            
            betList.Sort();
        }
        
        protected void UpdateGameUnlockConfig()
        {
            if (gameUnlockConfig != null && gameUnlockConfig.Count > 0)
            {
                currentUnlockBetConfig = new List<ulong>();
                var unlock = machineState.machineContext.serviceProvider.GetAvailableUnlockFeature(gameUnlockConfig);
                for (int i = 0; i < unlock.Count; i++)
                {
                    currentUnlockBetConfig.Add(unlock[i]);
                }
            }
        }

        public bool IsFeatureUnlocked(int index)
        {
            if (currentUnlockBetConfig != null && index < currentUnlockBetConfig.Count)
            {
                return totalBet >= currentUnlockBetConfig[index];
            }
            
            return false;
        }

        public bool SetBetBigEnoughToUnlockFeature(int index)
        {
            if (currentUnlockBetConfig != null && index < currentUnlockBetConfig.Count)
            {
                if (totalBet < currentUnlockBetConfig[index])
                {
                    for (var i = betLevel; i < betList.Count; i++)
                    {
                        if (betList[i] >= currentUnlockBetConfig[index])
                        {
                            SetBetLevel(i);
                            return true;
                        }
                    }
                }
            }

            return false;
        }
 
        public void InitializeRecommendBetLevel()
        {
            var machineId = machineState.machineContext.assetProvider.MachineId;
            betLevel = machineState.machineContext.serviceProvider.GetRecommendBetLevel(betList, machineId);
            totalBet = betList[betLevel];
        }

        public void AppendBetList()
        {
            
        }

        public bool IsBalanceSufficient()
        {
            return machineState.machineContext.serviceProvider.IsBalanceSufficient(totalBet);
        }

        public bool SetBetLevel(int inBetLevel)
        {
            if (inBetLevel < betList.Count && totalBet != betList[inBetLevel])
            {
                betLevel = inBetLevel;
                totalBet = betList[betLevel];

                var machineId = machineState.machineContext.assetProvider.MachineId;
                machineState.machineContext.serviceProvider.StoreRecommendBetLevel(totalBet, machineId);
                
                return true;
            }
            return false;
        }

        public bool SetTotalBet(ulong inTotalBet,bool refreshBetLevel = false)
        {
            if (totalBet != inTotalBet)
            {
                totalBet = inTotalBet;

                if (refreshBetLevel)
                {
                   betLevel = betList.IndexOf(inTotalBet);
                }

                return true;
            }
            
            return false;
        }
        
        public bool AlterBetLevel(int deltaLevel)
        {
            var betLevelCount =  betList.Count;
            var targetLevel = (betLevel + deltaLevel + betLevelCount) % betLevelCount;
            
            return SetBetLevel(targetLevel);
        }

        public bool UseMaxBetLevel()
        {
            return SetBetLevel(betList.Count - 1);
        }

        public int GetMexBetLevel()
        {
            return betList.Count - 1;
        }
        
        public bool RestoreTotalBet()
        {
            if (totalBet != betList[betLevel])
            {
                totalBet = betList[betLevel];
                return true;
            }
            
            return false;
        }

        public ulong GetPayWinChips(ulong payRate)
        {
            return payRate * totalBet / 100;
        }

        public long GetPayWinChips(long payRate)
        {
            return (payRate * (long) totalBet / 100);
        }
        
        public WinLevel GetSmallWinLevel(long winChips)
        {
            var winRate = (float) winChips / (long) totalBet;

            if (winChips <= 0)
            {
                return WinLevel.NoWin;
            }
            //small win
             if (winRate < 1)
                 return WinLevel.SmallWin;
            
             //middle win
             if (winRate < 3)
                 return WinLevel.Win;

            //nice win
            if (winRate < 8)
                return WinLevel.NiceWin;
                
            return WinLevel.NoWin;
        }

        public static long GetWinChips(long bet, long payRate)
        {
            return (long) (payRate * bet / 100);
        }
        
   
    }
}