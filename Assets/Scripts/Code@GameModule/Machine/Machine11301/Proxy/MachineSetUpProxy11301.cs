using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace GameModule{
    public class MachineSetUpProxy11301 : MachineSetUpProxy
    {
        public MachineSetUpProxy11301(MachineContext context) : base(context)
        {
        }
        protected override void UpdateViewWhenRoomSetUp()
        {
            base.UpdateViewWhenRoomSetUp();
            var extraState = machineContext.state.Get<ExtraState11301>();
            machineContext.view.Get<ShopEntranceView11301>().SetBoxTokensNum(extraState.GetCollectItems());
            Constant11301.IsSpining = false;
        }
    }
}
