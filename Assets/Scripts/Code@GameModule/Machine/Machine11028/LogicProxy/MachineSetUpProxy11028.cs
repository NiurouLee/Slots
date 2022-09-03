//  **********************************************
//  Copyright(c) 2021 by com.ustar
//  All right reserved
// 
//  Author : Besure.Chen
//  Date :2022-01-20 15:18
//  Ver : 1.0.0
//  Description : MachineSetUpProxy11028.cs
//  ChangeLog :
//  **********************************************

namespace GameModule
{
    public class MachineSetUpProxy11028:MachineSetUpProxy
    {
        public MachineSetUpProxy11028(MachineContext context) : base(context)
        {
        }
        protected override void UpdateViewWhenRoomSetUp()
        {
            base.UpdateViewWhenRoomSetUp();
            machineContext.view.Get<JackpotPotPanel11028>().UpdateJackpotLockState(false,true);
        }
    }
}