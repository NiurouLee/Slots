// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/10/21/17:45
// Ver : 1.0.0
// Description : BuffDataModel.cs
// ChangeLog :
// **********************************************

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DragonU3DSDK.Network.API.ILProtocol;
using DragonU3DSDK.Network.API.Protocol;
using Google.ilruntime.Protobuf;
using UnityEngine;

namespace GameModule
{
    
    /// <summary>
    /// 客户端对服务器Buffer的一个封装，方便客户端对Buffer的操作
    /// </summary>
    public class ClientBuff: Controller
    {
        protected ulong timestampLeft;
        protected float updateTime;
        protected string buffNameDesc;

        protected CancelableCallback buffExpireHandler;
        
        public ClientBuff()
        {
        }

        public float GetBuffLeftTimeInSecond()
        {
            return timestampLeft - (Time.realtimeSinceStartup - updateTime);
        }

        public string GetDescText()
        {
            return buffNameDesc;
        }

        public virtual void ParseServerBuff(AnyStruct anyStruct, float inUpdateTime)
        {
            updateTime = inUpdateTime;
        }

        protected virtual void SetUpBuffExpireHandler()
        {
            
        }
    }

    public class ClientBuff<T> : ClientBuff where T : IMessage
    {
        protected T serverBuffer;
         
        public override void ParseServerBuff(AnyStruct buff, float inUpdateTime)
        {
            base.ParseServerBuff(buff,inUpdateTime);
            serverBuffer = ProtocolUtils.GetAnyStruct<T>(buff);
        }

        public T GetServerBuff()
        {
            return serverBuffer;
        }
    }

    public class LevelUpBurstBuff : ClientBuff<BuffLevelUpBurst>
    {
        public LevelUpBurstBuff()
        {
            buffNameDesc = "Level Up Burst";
        }
        
        public override void ParseServerBuff(AnyStruct buff, float inUpdateTime)
        {
            base.ParseServerBuff(buff,inUpdateTime);
            timestampLeft = serverBuffer.TimestampLeft;
        }
    }

    public class SuperWheelBuff : ClientBuff<BuffSuperWheel>
    {
        public SuperWheelBuff()
        {
            buffNameDesc = "Super Wheel";
        }
        
        public override void ParseServerBuff(AnyStruct buff, float inUpdateTime)
        {
            base.ParseServerBuff(buff,inUpdateTime);
            timestampLeft = serverBuffer.TimestampLeft;
        }
    }
    public class TimerbonusSpinBuff : ClientBuff<BuffTimerbonusSpin>
    {
        public ulong spinBuffLevel;
        public ulong exp;
        public TimerbonusSpinBuff()
        {
            buffNameDesc = "Time Bonus";
        }
        
        public override void ParseServerBuff(AnyStruct buff, float inUpdateTime)
        {
            base.ParseServerBuff(buff,inUpdateTime);
            timestampLeft = ulong.MaxValue;
            spinBuffLevel = serverBuffer.SpinBuffLevel;
            exp = serverBuffer.Exp;
        }

        public void UpdateBuff(BuffTimerbonusSpin inServerBuffer)
        {
            serverBuffer = inServerBuffer;
            timestampLeft = ulong.MaxValue;
            spinBuffLevel = serverBuffer.SpinBuffLevel;
            exp = serverBuffer.Exp;
        }
    }

    public class DoubleExpBuff : ClientBuff<BuffDoubleExp>
    {
        public DoubleExpBuff()
        {
            buffNameDesc = "Double XP";
        }
        
        public override void ParseServerBuff(AnyStruct buff, float inUpdateTime)
        {
            base.ParseServerBuff(buff,inUpdateTime);
            timestampLeft = serverBuffer.TimestampLeft;
        }
    }
    
    public class SeasonQuestStarBoostBuff : ClientBuff<BuffSeasonQuestStarBoost>
    {
        public SeasonQuestStarBoostBuff()
        {
            buffNameDesc = "Season Quest Speed Boost";
        }
        
        public override void ParseServerBuff(AnyStruct buff, float inUpdateTime)
        {
            base.ParseServerBuff(buff,inUpdateTime);
            timestampLeft = serverBuffer.TimestampLeft;
        }
    }
    
    public class NewbieQuestBoostBuff : ClientBuff<BuffNewbieQuestBoost>
    {
        public NewbieQuestBoostBuff()
        {
            buffNameDesc = "Season Quest Speed Boost";
        }
        
        public override void ParseServerBuff(AnyStruct buff, float inUpdateTime)
        {
            base.ParseServerBuff(buff,inUpdateTime);
            timestampLeft = serverBuffer.TimestampLeft;
        }
    }
    
    public class MonopolyBoosterBuff : ClientBuff<BuffMonopolyBooster>
    {
        public MonopolyBoosterBuff()
        {
            buffNameDesc = "Monopoly Boost";
        }
        
        public override void ParseServerBuff(AnyStruct buff, float inUpdateTime)
        {
            base.ParseServerBuff(buff,inUpdateTime);
            var activityTreasureRaid = Client.Get<ActivityController>().GetDefaultActivity(ActivityType.TreasureRaid) as Activity_TreasureRaid;
            if (activityTreasureRaid != null)
            {
                activityTreasureRaid.UpdateMonopolyBuff(serverBuffer);
            }
        }
    }
    
    public class CashBackBuff : ClientBuff<BuffCashBack>
    {
        public CashBackBuff()
        {
            buffNameDesc = "CashBack";
        }

        public override void ParseServerBuff(AnyStruct buff, float inUpdateTime)
        {
            base.ParseServerBuff(buff, inUpdateTime);
            timestampLeft = (ulong) XUtility.GetLeftTime((ulong)serverBuffer.Expire * 1000);
            
            SetUpBuffExpireHandler();
        }

        public bool CheckBuffIsValid()
        {
            if (serverBuffer != null)
            {
                return XUtility.GetLeftTime(serverBuffer.Expire * 1000) > 0;
            }

            return false;
        }

        public long GetCoin()
        {
            return (long)serverBuffer.BackCoin;
        }

        public bool IsFull()
        {
            return serverBuffer.BackCoin >= serverBuffer.ReturnLimited;
        }

        protected override void SetUpBuffExpireHandler()
        {
            if (serverBuffer != null && CheckBuffIsValid())
            {
                if (buffExpireHandler != null)
                {
                    buffExpireHandler.CancelCallback();
                    buffExpireHandler = null;
                }
                var leftTime =  XUtility.GetLeftTime((ulong) serverBuffer.RewardAt * 1000);

                if (leftTime > 0)
                {
                    buffExpireHandler = new CancelableCallback(CheckAndClaimReward);
                    XUtility.WaitSeconds(leftTime, buffExpireHandler);
                }
            }
        }

        protected void CheckAndClaimReward()
        {
            if (serverBuffer != null)
            {
               var leftTime =  XUtility.GetLeftTime((ulong) serverBuffer.RewardAt * 1000);
               if (leftTime <= 0)
               {
                   ClaimBuffReward();
               }
            }
        }

        public async void RefreshCashBackBuff()
        {
            await RefreshCashBackBuffAsync();
        }
        public async Task RefreshCashBackBuffAsync()
        {
            CGetUserCashBackBuffs c = new CGetUserCashBackBuffs();
            var s = await APIManagerGameModule.Instance.SendAsync<CGetUserCashBackBuffs, SGetUserCashBackBuffs>(c);
            
            if (s.ErrorCode == 0)
            {
                var newServerBuffs = s.Response.CashBackBuffs;
                if (newServerBuffs.Count > 0)
                {
                    for (var i = 0; i < newServerBuffs.Count; i++)
                    {
                        if (newServerBuffs[i].Key == serverBuffer.Key)
                        {
                            serverBuffer.BackCoin = newServerBuffs[i].BackCoin;
                        }
                    }
                }
            }
        }

        public uint GetRewardMode()
        {
            return serverBuffer.RewardMode;
        }
        
        public async void ClaimBuffReward()
        {
            var c = new CGetCashBackBuffRewardMail();
            c.Key = serverBuffer.Key;
            var s = await APIManagerGameModule.Instance
                .SendAsync<CGetCashBackBuffRewardMail, SGetCashBackBuffRewardMail>(c);
            if (s.ErrorCode != 0)
            {
                XDebug.LogError("CGetCashBackBuffRewardMail:Failed" + s.ErrorInfo);
            }
        }
    }

    public class BuffDataModel : Model<SGetBuff>
    {
        private List<ClientBuff> _clientBuffs;
        
        public BuffDataModel()
            : base(ModelType.TYPE_BUFF)
        {
        }

        public override async Task FetchModalDataFromServerAsync()
        {
            CGetBuff cGetBuff = new CGetBuff();
            var sGetBuff = await APIManagerGameModule.Instance.SendAsync<CGetBuff, SGetBuff>(cGetBuff);

            if (sGetBuff.ErrorCode == ErrorCode.Success)
            {
                UpdateModelData(sGetBuff.Response);
            }
        }

        public override void UpdateModelData(SGetBuff inModelData)
        {
            base.UpdateModelData(inModelData);

            if (modelData != null && modelData.Buffs.Count > 0)
            {
                var buff = modelData.Buffs;
             
                _clientBuffs = new List<ClientBuff>();

                for (var i = 0; i < buff.Count; i++)
                {
                    var clientBuffTypeName = buff[i].Type.Substring(4) + "Buff";

                    var buffType = Type.GetType($"GameModule.{clientBuffTypeName}");
                    
                    if (buffType != null)
                    {
                        var clientBuff = Activator.CreateInstance(buffType) as ClientBuff;
                        if (clientBuff != null)
                        {
                            clientBuff.ParseServerBuff(modelData.Buffs[i], lastTimeUpdateData);
                            _clientBuffs.Add(clientBuff);
                        }
                    }
                }
            }
            else
            {
                _clientBuffs = null;
            }
        }

        public T GetBuff<T>() where T : ClientBuff
        {
            if (_clientBuffs == null)
                return null;
            
            for (var i = 0; i < _clientBuffs.Count; i++)
            {
                if (_clientBuffs[i].GetType() == typeof(T) && _clientBuffs[i].GetBuffLeftTimeInSecond() > 0)
                {
                    return _clientBuffs[i] as T;
                }
            }
            
            return null;
        }
        
        public bool HasBuff<T>() where T : ClientBuff
        {
            if (_clientBuffs == null)
                return false;
            
            for (var i = 0; i < _clientBuffs.Count; i++)
            {
                if (_clientBuffs[i].GetType() == typeof(T))
                {
                    return true;
                }
            }

            return false;
        }
    }
}