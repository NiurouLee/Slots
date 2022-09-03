using System.Collections.Generic;
using UnityEngine;
using System;
using GameModule;

namespace GameModule
{
    public class MachineSetUpProxy11017 : MachineSetUpProxy
    {
        public MachineSetUpProxy11017(MachineContext context)
            : base(context)
        {
        }

        protected override void UpdateViewWhenRoomSetUp()
        {
            base.UpdateViewWhenRoomSetUp();

            UpdateSuperFreeGameLockView();
        }

        protected void UpdateSuperFreeGameLockView()
        {
            var superFreeProgressView11001 = machineContext.view.Get<SuperFreeGameLock11017>();
            superFreeProgressView11001.LockSuperFree(!machineContext.state.Get<BetState>().IsFeatureUnlocked(0), false);
        }
    }
}