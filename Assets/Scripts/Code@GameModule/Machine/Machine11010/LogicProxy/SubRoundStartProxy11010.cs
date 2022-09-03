//  **********************************************
//  Copyright(c) 2021 by com.ustar
//  All right reserved
// 
//  Author : Besure.Chen
//  Date :2021-09-29 13:19
//  Ver : 1.0.0
//  Description : SubRoundStartProxy11010.cs
//  ChangeLog :
//  **********************************************

using UnityEngine;

namespace GameModule
{
    public class SubRoundStartProxy11010:SubRoundStartProxy
    {
        public SubRoundStartProxy11010(MachineContext context)
            : base(context)
        {
            
        }

        protected override async void SendSpinResultRequest()
        {
            if (machineContext.state.Get<FreeSpinState>().IsTriggerFreeSpin)
            {
                await machineContext.WaitSeconds(0.2f);
                Wheel wheel = machineContext.state.Get<WheelsActiveState>().GetRunningWheel()[0];
                Animator animator = wheel.transform.Find("UIFreeGameExtraDiamondAdded11010").GetComponent<Animator>();
                animator.gameObject.SetActive(true);
                XUtility.PlayAnimation(animator, "ExtraDiamondAdd");
                await machineContext.WaitSeconds(1.5f);
                AudioUtil.Instance.PlayAudioFx("FreeSpin_ExtraAdd");
                await machineContext.WaitSeconds(2f);
                animator.gameObject.SetActive(false);
                AudioUtil.Instance.PlayMusic(machineContext.machineConfig.audioConfig.GetFreeBackgroundMusicName());
            }
            base.SendSpinResultRequest();
        }
        
        protected override void PlayBgMusic()
        {
            if (machineContext.state.Get<ReSpinState>().NextIsReSpin)
            {
                AudioUtil.Instance.PlayMusic(machineContext.machineConfig.audioConfig.GetLinkBackgroundMusicName());
            }
            else if (machineContext.state.Get<FreeSpinState>().NextIsFreeSpin && !machineContext.state.Get<FreeSpinState>().IsTriggerFreeSpin)
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