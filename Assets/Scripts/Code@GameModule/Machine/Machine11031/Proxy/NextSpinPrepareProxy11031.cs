using DragonU3DSDK.Network.API.ILProtocol;

namespace GameModule
{
    public class NextSpinPrepareProxy11031 : NextSpinPrepareProxy
    {
        public NextSpinPrepareProxy11031(MachineContext machineContext) : base(machineContext)
        {
        }

        protected override void HandleCommonLogic()
        {
            base.HandleCommonLogic();

            if (!machineContext.state.Get<AutoSpinState>().IsAutoSpin)
            {
                machineContext.view.Get<BowlView11031>().EnableButtonResponse(true);
                machineContext.view.Get<CollectGroupView11031>().EnableButtonResponse(true);
            }

            if (IsFromMachineSetup())
            {
                machineContext.view.Get<CollectGroupView11031>().ShowTipView();
            }
        }

        public override void OnAutoSpinStopClicked()
        {
            base.OnAutoSpinStopClicked();

            if (!machineContext.state.Get<AutoSpinState>().IsAutoSpin)
            {
                machineContext.view.Get<BowlView11031>().EnableButtonResponse(true);
                machineContext.view.Get<CollectGroupView11031>().EnableButtonResponse(true);
            }
        }

        public override void StartNextSpin()
        {
            machineContext.view.Get<BowlView11031>().EnableButtonResponse(false);
            machineContext.view.Get<CollectGroupView11031>().EnableButtonResponse(false);
            base.StartNextSpin();
        }

        protected override void OnBetChange()
        {
            base.OnBetChange();
            var wheel = machineContext.state.Get<WheelsActiveState11031>().GetRunningWheel()[0];
            wheel.GetContext().state.Get<WheelsActiveState11031>().FadeOutRollMask(wheel);
            machineContext.view.Get<WinGroupView11031>().ShowWinGroup();
            machineContext.view.Get<WinGroupView11031>().HideAllHighLight();
            machineContext.view.Get<WinGroupView11031>().PlayBiggerNumIdle();
        }
    }
}