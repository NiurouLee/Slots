// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date :2021-07-05 12:15 PM
// Ver : 1.0.0
// Description : LogicProxySubRoundStart.cs
// ChangeLog :
// **********************************************

using UnityEngine;

namespace GameModule
{
    public class SubRoundStartProxy : LogicStepProxy
    {
        public SubRoundStartProxy(MachineContext context)
            : base(context)
        {
        }
      
        protected override void HandleCommonLogic()
        {
            machineContext.state.UpdateStateOnSubRoundStart();
             
            PlayBgMusic();
        }

        protected virtual void SendSpinResultRequest()
        {
            bool isAutoSpin = machineContext.state.Get<AutoSpinState>().IsAutoSpin;
            machineContext.serviceProvider.GetSpinResult(machineContext.state.Get<BetState>().totalBet, isAutoSpin,machineContext,(sSpin) =>
            {
                if (machineContext.transform != null)
                {
                    XDebug.Log(
                        $"<color=yellow>=======SpinGameResult: Response: {LitJson.JsonMapper.ToJsonField(sSpin.GameResult)}</color>");
                    machineContext.state.UpdateMachineStateOnSpinResultReceived(sSpin);
                    machineContext.DispatchInternalEvent(MachineInternalEvent.EVENT_SERVER_SPIN_DATA_RECEIVED);
                }
            });
        }

        protected override void Proceed()
        {
            SendSpinResultRequest();
            base.Proceed();
        }
        
        protected virtual void PlayBgMusic()
        {

            //Debug.LogError($"=======NextIsReSpin:{machineContext.state.Get<ReSpinState>().NextIsReSpin}");
            if (machineContext.state.Get<ReSpinState>().NextIsReSpin)
            {
                //Debug.LogError("====play");
                AudioUtil.Instance.PlayMusic(machineContext.machineConfig.audioConfig.GetLinkBackgroundMusicName());
            }
            else if (machineContext.state.Get<FreeSpinState>().NextIsFreeSpin)
            {
                AudioUtil.Instance.PlayMusic(machineContext.machineConfig.audioConfig.GetFreeBackgroundMusicName());
            }
            else
            {
                AudioUtil.Instance.PlayMusic(machineContext.machineConfig.audioConfig.GetBaseBackgroundMusicName());
            }
        }
    }
}