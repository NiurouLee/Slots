// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2022/02/15/11:28
// Ver : 1.0.0
// Description : AdStrategyState.cs
// ChangeLog :
// **********************************************

using System.Threading.Tasks;
using DragonU3DSDK.Network.API.ILProtocol;
using Google.ilruntime.Protobuf.Collections;

namespace GameModule
{
    public class AdStrategyState:SubState
    {
        private AdStrategy _adStrategy = null;
        
        public AdStrategyState(MachineState machineState) : base(machineState)
        {
            
        }
        protected T GetSystemData<T>(RepeatedField<AnyStruct> systemContent, string filter) where T : Google.ilruntime.Protobuf.IMessage
        {
            if (systemContent!=null && systemContent.Count>0)
            {
                for (int i = 0; i < systemContent.Count; i++)
                {
                    var extraData = systemContent[i];
                    if (extraData.Type == filter)
                    {
                        return ProtocolUtils.GetAnyStruct<T>(extraData);
                    }
                }
            }
            return default;
        }
        
        public override void UpdateStateOnReceiveSpinResult(SSpin spinResult)
        {
            base.UpdateStateOnReceiveSpinResult(spinResult);
            _adStrategy = GetSystemData<AdStrategy>(spinResult.SystemContent, "AdStrategy");
        }

        public override void UpdateStateOnBonusProcess(SBonusProcess bonusProcess)
        {
            base.UpdateStateOnBonusProcess(bonusProcess);
            _adStrategy = GetSystemData<AdStrategy>(bonusProcess.SystemContent, "AdStrategy");
        }
        
        public override void UpdateStateOnSpecialProcess(SSpecialProcess specialProcess)
        {
            base.UpdateStateOnSpecialProcess(specialProcess);
            _adStrategy = GetSystemData<AdStrategy>(specialProcess.SystemContent, "AdStrategy");
        }

        public override void UpdateStateOnSettleProcess(SSettleProcess settleProcess)
        {
            base.UpdateStateOnSettleProcess(settleProcess);
            _adStrategy = GetSystemData<AdStrategy>(settleProcess.SystemContent, "AdStrategy");
        }

        public bool HasWatchAdWinExtraFreeSpin()
        {
            if (_adStrategy != null)
            {
                 XDebug.Log($"AdStrategy.Type:{_adStrategy.Type.ToString()}");
                 return _adStrategy.Type == AdStrategy.Types.Type.ExtraFreeSpin;
            }
            
            XDebug.Log($"No AdStrategy Data");
            
            return false;
        }
        
        public bool HasWinWheelMultiple()
        {
            if (_adStrategy != null)
            {
                XDebug.Log($"AdStrategy.Type:{_adStrategy.Type.ToString()}");
                return _adStrategy.Type == AdStrategy.Types.Type.MultipleBigWin;
            }
            
            XDebug.Log($"No AdStrategy Data");

            return false;
        }

        public AdStrategy GetAdStrategy()
        {
            if (_adStrategy != null)
            {
                XDebug.Log("HasServerAdStrategy");
            }
            else
            {
                XDebug.Log("NoServerAdStrategy");
            }

            return _adStrategy;
        }
 
        public async Task ClaimMultipleFreeSpin()
        {
            var response = await machineState.machineContext.serviceProvider.SendClaimRvExtraFreeSpin();

            if (response != null && response.GameResult != null)
            {
                if (response.GameResult.FreeSpinInfo != null)
                {
                    machineState.Get<FreeSpinState>().UpdateFreeSpinStateAfterClaimRvReward(response.GameResult.FreeSpinInfo);
                }
            }
        }
    }
}