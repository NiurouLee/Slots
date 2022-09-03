// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/07/05/15:03
// Ver : 1.0.0
// Description : JackpotInfo.cs
// ChangeLog :
// **********************************************

using System;
using DragonU3DSDK.Network.API.ILProtocol;
using UnityEngine;

namespace GameModule
{
    public class Jackpot
    {
        protected JackpotConfig jackpotConfig;
        protected JackpotState jackpotState;
        protected float lastUpdateTime;
 
        public Jackpot(JackpotConfig jackpotConfig, JackpotState jackpotState)
        {
            this.jackpotConfig = jackpotConfig;
            this.jackpotState = jackpotState;
            lastUpdateTime = Time.realtimeSinceStartup - jackpotState.JackpotProgress * jackpotConfig.Interval;
        }

        public ulong GetJackpotValue(ulong totalBet)
        {
            var deltaTime = (Time.realtimeSinceStartup - lastUpdateTime) % jackpotConfig.Interval;
            if (jackpotConfig.Interval.Equals(0) || jackpotConfig.MaxPay == jackpotConfig.MinPay)
            {
                ulong jackpotValueDecimal = jackpotConfig.MinPay * totalBet / 100;
                return jackpotValueDecimal;
                //return (ulong)Mathf.Floor(jackpotConfig.MinPay * totalBet * 0.01f);
            }
            var jackpotValue = Mathf.Lerp(jackpotConfig.MinPay, jackpotConfig.MaxPay,
                (deltaTime % jackpotConfig.Interval)/jackpotConfig.Interval);
            return (ulong) Mathf.Floor(jackpotValue * totalBet * 0.01f);
        }

        public ulong GetLastServerJackpotValue(ulong totalBet)
        {
            return totalBet * jackpotState.JackpotValue / 100;
        }

        public void SyncJackpotState(JackpotState inJackpotState)
        {
            jackpotState = inJackpotState;
            lastUpdateTime = Time.realtimeSinceStartup - jackpotState.JackpotProgress * jackpotConfig.Interval;
        }
        
        //===================added by james====================
        //某些信息被服务器放到了jackpotConfig中,例如jackpot解锁的bet数据
        public JackpotConfig JackpotConfig
        {
            get { return jackpotConfig; }
        }

        public JackpotState JackpotState
        {
            get { return jackpotState; }
        }
        //===================added by james====================

    }
}
