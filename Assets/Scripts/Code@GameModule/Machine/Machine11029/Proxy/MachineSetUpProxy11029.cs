using System.Collections.Generic;
using UnityEngine;
using System;
using GameModule;

namespace GameModule
{
    public class MachineSetUpProxy11029 : MachineSetUpProxy
    {
        public MachineSetUpProxy11029(MachineContext context)
            : base(context)
        {
        }

        protected override void UpdateViewWhenRoomSetUp()
        {
            base.UpdateViewWhenRoomSetUp();
            UpdateSuperBonusProgressView();
            UpdateCollectGroupView();
            UpdateBackGroupView();
        }

        //钱袋区
        private void UpdateCollectGroupView()
        {
            machineContext.view.Get<MoneyBag11029>().ShowCollectionGroup(false);
        }

        private void UpdateSuperBonusProgressView()
        {
            var progressBar11029 = machineContext.view.Get<ProgressBar11029>();
            progressBar11029.LockSuperFree(!machineContext.state.Get<BetState>().IsFeatureUnlocked(0), false);
            progressBar11029.ChangeFill(false,false);
            progressBar11029.ChangeMapRightBtn();
            // machineContext.view.Get<JackpotPanel11026>().UpdateJackpotLockState(false,true);
        }

        private void UpdateBackGroupView()
        {
            if (machineContext.state.Get<FreeSpinState>().IsInFreeSpin)
            {
                uint freeSpinId = machineContext.state.Get<FreeSpinState>().freeSpinId;
                if (freeSpinId <= 1)
                {
                    machineContext.view.Get<MeiDuSha11029>().ShowMeiDuSha(true);
                    machineContext.view.Get<BackGroundView11029>().ShowFreeBackground(true, false, false);
                }
                else if (freeSpinId == 2)
                {
                    machineContext.view.Get<MeiDuSha11029>().ShowMeiDuSha(false);
                    machineContext.view.Get<BackGroundView11029>().ShowFreeBackground(false, false, true);
                }
                else if (freeSpinId > 2 && freeSpinId <= 8)
                {
                    machineContext.view.Get<MeiDuSha11029>().ShowMeiDuSha(false);
                    machineContext.view.Get<BackGroundView11029>().ShowFreeBackground(false, true, false);
                }
            }
            else
            {
                machineContext.view.Get<MeiDuSha11029>().ShowMeiDuSha(true);
                machineContext.view.Get<BackGroundView11029>().ShowFreeBackground(false, false, false);
            }
        }
    }
}