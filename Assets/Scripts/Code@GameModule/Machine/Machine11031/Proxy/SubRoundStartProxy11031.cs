using System.Collections.Generic;
using UnityEngine;
using System;
using GameModule;

namespace GameModule
{
    public class SubRoundStartProxy11031 : SubRoundStartProxy
    {
        private ReSpinState _reSpinState;
        private ExtraState11031 _extraState11031;

        public SubRoundStartProxy11031(MachineContext context)
            : base(context)
        {
            _reSpinState = machineContext.state.Get<ReSpinState>();
            _extraState11031 = machineContext.state.Get<ExtraState11031>();
        }

        protected override void HandleCommonLogic()
        {
            machineContext.view.Get<LinkRemaining11031>().ChangeReSpinCount();
            if (!_reSpinState.IsInRespin && !_reSpinState.ReSpinTriggered)
            {
                machineContext.view.Get<WinGroupView11031>().HideAllHighLight();
                machineContext.view.Get<WinGroupView11031>().PlayBiggerNumIdle();
            }

            base.HandleCommonLogic();
        }

        protected override void PlayBgMusic()
        {
            if (machineContext.state.Get<ReSpinState>().NextIsReSpin)
            {
                if (_extraState11031.IsFourLinkMode())
                {
                    AudioUtil.Instance.PlayMusic("Bg_SuperRespin_11031", true);
                }
                else
                {
                    AudioUtil.Instance.PlayMusic("Bg_Respin_11031", true);
                }
            }
            else
            {
                AudioUtil.Instance.PlayMusic(machineContext.machineConfig.audioConfig.GetBaseBackgroundMusicName());
            }
        }
    }
}