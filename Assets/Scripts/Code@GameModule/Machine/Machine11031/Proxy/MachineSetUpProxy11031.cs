using System.Collections.Generic;
using UnityEngine;
using System;
using GameModule;

namespace GameModule
{
    public class MachineSetUpProxy11031 : MachineSetUpProxy
    {
        public MachineSetUpProxy11031(MachineContext context)
            : base(context)
        {
        }

        protected override void UpdateViewWhenRoomSetUp()
        {
            base.UpdateViewWhenRoomSetUp();
            UpdateWinGroupView();
            UpdateJackPotPanel();
            UpdateSuperBonusProgressView();
            UpdateBackGroupView();
        }

        private void UpdateBackGroupView()
        {
            if (machineContext.state.Get<ReSpinState>().IsInRespin)
            {
                machineContext.view.Get<BackGroundView11031>().ShowBackground(true);
            }
            else
            {
                machineContext.view.Get<BackGroundView11031>().ShowBackground(false);
            }
        }

        //奖励值
        private void UpdateWinGroupView()
        {
            machineContext.view.Get<WinGroupView11031>().ShowWinGroup();
        }

        private void UpdateJackPotPanel()
        {
            machineContext.view.Get<JackpotPanel11031>().PlayJackPotRemindIdle();
        }

        //进度条
        private void UpdateSuperBonusProgressView()
        {
            var progressBar11031 = machineContext.view.Get<CollectGroupView11031>();
            progressBar11031.ChangeFill(false, false,true);
        }
    }
}