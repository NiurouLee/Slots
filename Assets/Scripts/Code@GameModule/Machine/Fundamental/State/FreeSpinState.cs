using System.Threading.Tasks;
using DragonU3DSDK.Network.API.ILProtocol;
using Google.ilruntime.Protobuf.Collections;
using UnityEngine;

namespace GameModule
{
    public class FreeSpinState : SubState
    {
        /// <summary>
        /// 当前是否在FreeSpin中
        /// </summary>
        public bool IsInFreeSpin { get; private set; } = false;
        
        /// <summary>
        /// 当前Spin是否触发的FreeSpin
        /// </summary>
        public bool IsTriggerFreeSpin { get; private set; } = false;
        
        /// <summary>
        /// 下一次Spin是否是FreeSpin
        /// </summary>
        public bool NextIsFreeSpin { get; private set; } = false;
         
        /// <summary>
        /// FreeSpin总次数
        /// </summary>
        public uint TotalCount { get; private set; } = 0;
        
        /// <summary>
        /// 当前FreeSpin的次数
        /// </summary>
        public uint CurrentCount { get; private set; }= 0;
        
        /// <summary>
        /// ReTrigger触发获得新的次数
        /// </summary>
        public uint NewCount { get; protected set; }= 0;
        
        /// <summary>
        /// 剩余的Spin次数
        /// </summary>
        public uint LeftCount { get; private set; } = 0;

        /// <summary>
        /// FreeSpin总赢钱
        /// </summary>
        public ulong TotalWin { get; private set; } = 0;


        public bool FreeNeedSettle { get; private set; } = false;

        public bool IsOver { get; private set; } = true;
        
        public bool backFromFree{ get; set; } = false;

        public ulong freeSpinBet;


        public RepeatedField<Panel> triggerPanels;
        
        /// <summary>
        /// freeSpinId
        /// </summary>
        public uint freeSpinId { get; private set; } = 0;

        public FreeSpinState(MachineState state) : base(state)
        {
        }
        
        public override void UpdateStatePreRoomSetUp(SEnterGame gameEnterInfo)
        {
            SetFreeSpinInfo(gameEnterInfo.GameResult.FreeSpinInfo,gameEnterInfo.GameResult);
        }
 

        public override void UpdateStateOnReceiveSpinResult(SSpin spinResult)
        {
            base.UpdateStateOnReceiveSpinResult(spinResult);
            SetFreeSpinInfo(spinResult.GameResult.FreeSpinInfo,spinResult.GameResult);
        }


        protected virtual void SetFreeSpinInfo(FreeSpinInfo freeSpinInfo, GameResult gameResult)
        {
            IsInFreeSpin = gameResult.IsFreeSpin;
            NewCount = gameResult.FreeSpinCount;
            freeSpinId = freeSpinInfo.FreeSpinId;
            
            if (freeSpinInfo.FreeSpinCount != 0 &&
                freeSpinInfo.FreeSpinCount == freeSpinInfo.FreeSpinTotalCount)
            {
                IsTriggerFreeSpin = true;
            }
            else
            {
                IsTriggerFreeSpin = false;
            }
 
            if (freeSpinInfo.FreeSpinTotalCount != 0 &&
                freeSpinInfo.FreeSpinCount > 0)
            {
                NextIsFreeSpin = true;
            }
            else
            {
                NextIsFreeSpin = false;
            }

            TotalCount = freeSpinInfo.FreeSpinTotalCount;
            CurrentCount = freeSpinInfo.FreeSpinTotalCount - freeSpinInfo.FreeSpinCount;
            LeftCount = freeSpinInfo.FreeSpinCount;
            TotalWin = freeSpinInfo.FreeSpinTotalWin;
            triggerPanels = freeSpinInfo.TriggeringPanels;

            FreeNeedSettle = !freeSpinInfo.IsOver && freeSpinInfo.FreeSpinCount == 0;

            freeSpinBet = freeSpinInfo.FreeSpinBet;
            
            IsOver = freeSpinInfo.IsOver;
        }

        public override void UpdateStateOnBonusProcess(SBonusProcess sBonusProcess)
        {
            SetFreeSpinInfo(sBonusProcess.GameResult.FreeSpinInfo,sBonusProcess.GameResult);
        }
        
        public override void UpdateStateOnSpecialProcess(SSpecialProcess specialProcess)
        {
            SetFreeSpinInfo(specialProcess.GameResult.FreeSpinInfo,specialProcess.GameResult);
        }
        
        public override void UpdateStateOnSettleProcess(SSettleProcess settleProcess)
        {
            SetFreeSpinInfo(settleProcess.GameResult.FreeSpinInfo, settleProcess.GameResult);
        }
        
        public async Task SettleFreeSpin()
        {
            var settleProcess = await machineState.machineContext.serviceProvider.SettleGameProgress();
            
            if (settleProcess != null)
            {
                machineState.UpdateStateOnSettleProcess(settleProcess);
            }
        }

        //
        public void UpdateFreeSpinStateAfterClaimRvReward(FreeSpinInfo freeSpinInfo)
        { 
            TotalCount = freeSpinInfo.FreeSpinTotalCount;
            CurrentCount = freeSpinInfo.FreeSpinTotalCount - freeSpinInfo.FreeSpinCount;
            LeftCount = freeSpinInfo.FreeSpinCount;
            freeSpinId = freeSpinInfo.FreeSpinId;
        }
        
        public override void UpdateStateOnRoundStart()
        {
            backFromFree = false;
        }
    }
}