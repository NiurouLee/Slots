using DragonU3DSDK.Network.API.ILProtocol;
using Google.ilruntime.Protobuf.Collections;
using UnityEngine;
using System;
using System.Collections.Generic;

namespace GameModule
{
    public class ExtraState11020 : ExtraState<LavaBountyGameResultExtraInfo>
    {
        // private LavaBountyGameResultExtraInfo lastExtraInfo;

        public bool isAfterSettle = false;

        public ExtraState11020(MachineState state) 
			: base(state)
        {

        }
        
        public override void UpdateStateOnRoomSetUp(SEnterGame gameEnterInfo)
        {
            lastExtraInfo = extraInfo;
            isAfterSettle = false;
            base.UpdateStateOnRoomSetUp(gameEnterInfo);
        }
        
        public override void UpdateStateOnReceiveSpinResult(SSpin spinResult)
        {
            lastExtraInfo = extraInfo;
            isAfterSettle = false;
            base.UpdateStateOnReceiveSpinResult(spinResult);
        }

        public override void UpdateStateOnBonusProcess(SBonusProcess sBonusProcess)
        {
            lastExtraInfo = extraInfo;
            isAfterSettle = false;
            base.UpdateStateOnBonusProcess(sBonusProcess);
        }
        
        public override void UpdateStateOnSettleProcess(SSettleProcess settleProcess)
        {
            isAfterSettle = true;
            base.UpdateStateOnSettleProcess(settleProcess);
        }

        public override bool HasSpecialBonus()
        {
            // if (extraInfo != null)
            // {
            //     return extraInfo.Sequences != null && extraInfo.BonusGameCompleted == false && extraInfo.Sequences.Count > 0;
            // }

            return false;
        }

        public override bool IsSpecialBonusFinish()
        {
            return false;
        }

        public int GetRouletteWheelId()
        {
            return extraInfo != null ? extraInfo.RouletteId : 0;
        }

        public uint GetBonusSymbolCount()
        {
            return extraInfo != null ? extraInfo.BonusSymbolCount : 0;
        }

        public int GetNewFrameCount()
        {
            return extraInfo != null ? extraInfo.RandomLions.Count : 0;
        }

        // 普通锁定的框
        public RepeatedField<uint> GetBetLockedFrames()
        {
            if (extraInfo != null && extraInfo.Frames != null)
            {
                var bet = machineState.Get<BetState>().totalBet;

                if (extraInfo.Frames.ContainsKey(bet))
                {
                    return extraInfo.Frames[bet].Positions;
                }
            }
            
            return null;
        }
        
        // super bonus锁定的框
        public RepeatedField<uint> GetSuperBonusLockedFrames()
        {
            if (extraInfo != null && extraInfo.SuperBonusLockedFrames != null)
            {
                return extraInfo.SuperBonusLockedFrames.Positions;
            }

            return null;
        }

        // 起点wild
		public RepeatedField<uint> GetStartWildFrames()
        {
            if (extraInfo != null)
            {
                return extraInfo.StartWildFrames;
            }

            return null;
        }

        // 变成wild图标的框
        public RepeatedField<uint> GetWildFrames()
        {
            if (extraInfo != null)
            {
                return extraInfo.WildFrames;
            }

            return null;
        }
        
        public RepeatedField<uint> GetLastWildFrames()
        {
            if (lastExtraInfo != null)
            {
                return lastExtraInfo.WildFrames;
            }

            return null;
        }

        // 本次spin随机出现的框
        public RepeatedField<uint> GetRandomFrames()
        {
            if (extraInfo != null)
            {
                return extraInfo.RandomFrames;
            }

            return null;
        }

        // 随机出现的狮子
        public RepeatedField<uint> GetRandomLions()
        {
            if (extraInfo != null)
            {
                return extraInfo.RandomLions;
            }

            return null;
        }

        public bool IsFrame(uint id)
        {
            var frames = GetBetLockedFrames();
            if (frames != null && frames.Contains(id))
            {
                return true;
            }

            return false;
        }

        public bool IsWildFrame(uint id)
        {

            var frames = GetWildFrames();
            if (frames != null && frames.Contains(id))
            {
                return true;
            }
            
            return false;
        }

        public bool IsRandomLion(uint id)
        {
            var frames = GetRandomLions();
            if (frames != null && frames.Contains(id))
            {
                return true;
            }
            
            return false;
        }

        public void dump()
        {
            Debug.Log("============================================================================================================================");
            
            var f = GetBetLockedFrames();
            Debug.Log("普通锁定的框 : " + (f != null ? f.Count : 0));

            f = GetWildFrames();
            Debug.Log("变成wild图标的框 : " + (f != null ? f.Count : 0));

            f = GetRandomFrames();
            Debug.Log("本次spin随机出现的框 : " + (f != null ? f.Count : 0));

            f = GetRandomLions();
            Debug.Log("随机出现的狮子 : " + (f != null ? f.Count : 0));

            f = GetSuperBonusLockedFrames();
            Debug.Log("super bonus锁定的框 : " + (f != null ? f.Count : 0));

            Debug.Log("============================================================================================================================");
        }
    }
}