// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/07/07/17:10
// Ver : 1.0.0
// Description : BonusGameState.cs
// ChangeLog :
// **********************************************

using System;
using System.Threading.Tasks;
using DragonU3DSDK.Network.API.ILProtocol;
using Google.ilruntime.Protobuf;

namespace GameModule
{
    public class ExtraState: SubState
    {
        protected bool isBonusGame;
        protected uint bonusGame;
        
        /// <summary>
        /// SpecialBonus 是在轮盘停下，BLINK WIN LINE的时候，结算的特殊玩法，具体玩法由每个关卡自己确定
        /// </summary>
        /// <returns></returns>
        public virtual bool HasSpecialBonus()
        {
            return false;
        }

        /// <summary>
        /// SpecialBonus是否结算完成
        /// </summary>
        /// <returns></returns>
        public virtual bool IsSpecialBonusFinish()
        {
            return isBonusGame && bonusGame == 0;
        }
        
        /// <summary>
        /// 轮盘停下后是否有一些特殊的效果，如收集玩法，会播放收集效果
        /// </summary>
        /// <returns></returns>
        public virtual bool HasSpecialEffectWhenWheelStop()
        {
            return false;
        }

        /// <summary>
        /// 所有轮盘赢钱结算完成之后，触发的特殊玩法，如果Scatter触发了，转盘玩法等 FreeSpin等
        /// </summary>
        /// <returns></returns>
        public virtual bool HasBonusGame()
        {
            return false;
        }
         
        public ExtraState(MachineState state) : base(state)
        {
            
        }
        
        /// <summary>
        /// elementId对应的图标是否触发了对应的玩法，这个接口需要关卡自己实现
        /// 作用是用于Blink动画的播放逻辑
        /// 目前策划定义BLink动画播放有三种方式，详情参考BlinkAnimationPlayStyleType中的说明
        /// 这个接口用于BlinkAnimationPlayStyleType.IdleCondition判断是否需要停止 idle 动画
        /// </summary>
        /// <param name="elementId"></param>
        /// <returns></returns>
        public virtual bool IsBlinkFeatureTriggered(uint elementId)
        {
            return false;
        }

        public virtual async Task<SBonusProcess> SendBonusProcess(Action<SBonusProcess> processAction = null)
        {
            var bonusResponse =
                await machineState.machineContext.serviceProvider.SendBonusResult(null, machineState.machineContext);

            if (bonusResponse != null)
            {
                if (machineState.machineContext.transform != null)
                {
                    processAction?.Invoke(bonusResponse);
                    machineState.UpdateStateOnBonusProcess(bonusResponse);
                }
            }

            return bonusResponse;
        }
        
        public virtual async Task<SBonusProcess> SendBonusProcess<TBonusRequest>(TBonusRequest bonusRequest)
            where TBonusRequest : IMessage
        {
            var bonusResponse = await machineState.machineContext.serviceProvider.SendBonusResult(bonusRequest,machineState.machineContext);

            if (machineState.machineContext.transform != null)
            {
                if (bonusResponse != null)
                {
                    machineState.UpdateStateOnBonusProcess(bonusResponse);
                }
            }

            return bonusResponse;
        }

        public virtual async Task<SSpecialProcess> SendSpecialProcess(string jsonData = null)
        {
            var specialResponse = await machineState.machineContext.serviceProvider.SendSpecialProcess(jsonData);
            if (machineState.machineContext.transform != null)
            {
                if (specialResponse != null)
                {
                    machineState.UpdateStateOnSpecialProcess(specialResponse);
                }
            }

            return specialResponse;
        }
        
        public virtual async Task SettleBonusProgress()
        {
            var bonusResponse = await machineState.machineContext.serviceProvider.SettleGameProgress();

            if (bonusResponse != null)
            {
                if (machineState.machineContext.transform != null)
                {
                    machineState.UpdateStateOnSettleProcess(bonusResponse);
                }
            }
        }
    }

    public class ExtraState<T> : ExtraState where T:IMessage
    {
        protected T extraInfo;
        
        protected T lastExtraInfo;

        public ExtraState(MachineState state) : base(state)
        {
            
        }


        public override void UpdateStatePreRoomSetUp(SEnterGame gameEnterInfo)
        {
            UpdateBonusGameState(gameEnterInfo.GameResult);
            extraInfo = ProtocolUtils.GetAnyStruct<T>(gameEnterInfo.GameResult.ExtraInfoPb);
            lastExtraInfo = extraInfo;
        }

        public override void UpdateStateOnRoomSetUp(SEnterGame gameEnterInfo)
        {
            lastExtraInfo = extraInfo;
            extraInfo = ProtocolUtils.GetAnyStruct<T>(gameEnterInfo.GameResult.ExtraInfoPb);
        }

        public override void UpdateStateOnReceiveSpinResult(SSpin spinResult)
        {
            lastExtraInfo = extraInfo;
            UpdateBonusGameState(spinResult.GameResult);
            extraInfo = ProtocolUtils.GetAnyStruct<T>(spinResult.GameResult.ExtraInfoPb);
        }

        public override void UpdateStateOnBonusProcess(SBonusProcess sBonusProcess)
        {
            lastExtraInfo = extraInfo;
            UpdateBonusGameState(sBonusProcess.GameResult);
            extraInfo = ProtocolUtils.GetAnyStruct<T>(sBonusProcess.GameResult
                .ExtraInfoPb);
        }
        
        public override void UpdateStateOnSpecialProcess(SSpecialProcess specialProcess)
        {
            lastExtraInfo = extraInfo;
            UpdateBonusGameState(specialProcess.GameResult);
            extraInfo = ProtocolUtils.GetAnyStruct<T>(specialProcess.GameResult
                .ExtraInfoPb);
        }
        
        public override void UpdateStateOnSettleProcess(SSettleProcess settleProcess)
        {
            lastExtraInfo = extraInfo;
            UpdateBonusGameState(settleProcess.GameResult);
            extraInfo = ProtocolUtils.GetAnyStruct<T>(settleProcess.GameResult
                .ExtraInfoPb);
        }
        
        private void UpdateBonusGameState(GameResult gameResult)
        {
            isBonusGame = gameResult.IsBonusGame;
            bonusGame = gameResult.BonusGame;
        }
    }
}