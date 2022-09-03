
using UnityEngine;

namespace GameModule
{
    public class MachineSetUpProxy11020 : MachineSetUpProxy
    {
        public MachineSetUpProxy11020(MachineContext context)
            : base(context)
        {

        }

        protected override void UpdateViewWhenRoomSetUp()
        {
            base.UpdateViewWhenRoomSetUp();

            var wheelActiveState = machineContext.state.Get<WheelsActiveState11020>();
            var wheel = wheelActiveState.GetRunningWheel()[0];
            // wheelActiveState.ForceWheelRandomElement(wheel);

            var lockedFramesView = machineContext.view.Get<LockedFramesView11020>();

            if (lockedFramesView != null)
            {
                lockedFramesView.currentWheel = wheel;
                lockedFramesView.StartWheel(lockedFramesView.currentWheel, false);
            }
        }

        protected override void UpdateRunningWheelElement()
        {
            // base.UpdateRunningWheelElement();
        }

        protected override async void HandleCustomLogic()
        {
            Constant11020.hasSpined = true;

            AudioUtil.Instance.PlayAudioFx("Fire_Welocome");

            var wheelActiveState = machineContext.state.Get<WheelsActiveState11020>();
            var wheel = wheelActiveState.GetRunningWheel()[0];
            wheelActiveState.ForceWheelRandomElement(wheel);

            var freeSpinState = machineContext.state.Get<FreeSpinState>();
            if (!freeSpinState.IsOver)
            {
                // var wheelActiveState = machineContext.state.Get<WheelsActiveState11020>();
            
                // wheelActiveState.UpdateRunningWheelState(null);

                // await machineContext.WaitSeconds(1.0f);

                machineContext.JumpToLogicStep(LogicStepType.STEP_FREE_GAME, LogicStepType.STEP_MACHINE_SETUP);

                return;
            }
            else
            {
                PopUpManager.Instance.ShowPopUp<LoadingFinishNoticePopUp11020>("UIHowToPlayTip" + machineContext.assetProvider.MachineId);
                base.HandleCustomLogic();
            }   
        }
    }
}