// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date :2021-07-06 6:59 PM
// Ver : 1.0.0
// Description : IMachineServiceProvider.cs
// ChangeLog :
// **********************************************
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using DragonU3DSDK.Network.API.ILProtocol;
using Google.ilruntime.Protobuf;
using Google.ilruntime.Protobuf.Collections;

namespace GameModule
{
    public interface IMachineServiceProvider
    {
        //获取进入房间的状态信息
        //JsonObject GetMachineSetUpInfo();
        //获取Spin结果
        SEnterGame GetEnterGameInfo();
        void GetSpinResult(ulong totalBet,bool isAuto, MachineContext context,Action<SSpin> spinCallback);
         
        //领取RV额外获取的FreeSpin次数          
        Task<SFulfillExtraFreeSpin> SendClaimRvExtraFreeSpin();
         
        Task<SBonusProcess> SendBonusResult(IMessage bonusPb, MachineContext context);

        Task<SSpecialProcess> SendSpecialProcess(string jsonData);
        //获取关卡Jackpot的信息
        //Dictionary<string, JackpotInfo> GetJackpotInfo(int subjectId);

        // int GetCurrentSubjectId();
        //
        RepeatedField<ulong> GetAvailableBetList(RepeatedField<GameBetConfig> bets);

        RepeatedField<ulong> GetAvailableUnlockFeature(RepeatedField<GameUnlockConfig> unlockConfigs);
        
        Task<SSettleProcess> SettleGameProgress();

        //
        bool IsBalanceSufficient(ulong totalBet);


        int GetRecommendBetLevel(List<ulong> bets, string machineId);
        void StoreRecommendBetLevel(ulong totalBet, string machineId);
        //
        // int GetNextUnlockBetLevel();
        //
        bool IsBetListNeedUpdate();
        //
        // List<int> GetWinLevelRange();
        //
        // void StoreRecommendBetLevel(long totalBet);
        //
        // int GetRecommendBetLevel();
        //
        // bool NeedShowNewBetUnlockTip
        // {
        //     get;
        //     set;
        // }
    }
}