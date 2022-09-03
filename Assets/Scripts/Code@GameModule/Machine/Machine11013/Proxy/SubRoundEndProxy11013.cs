namespace GameModule
{
    public class SubRoundEndProxy11013 : SubRoundFinishProxy
    {
        public SubRoundEndProxy11013(MachineContext context) : base(context)
        {
        }


        protected override void RegisterInterestedWaitEvent()
        {
            waitEvents.Add(WaitEvent.WAIT_BLINK_ALL_WIN_LINE);
        }

        protected override bool CheckIsAllWaitEventComplete()
        {
            var freeSpinState = machineContext.state.Get<FreeSpinState>();

            if (freeSpinState.NewCount > 0)
            {
                var wheel = machineContext.state.Get<WheelsActiveState>().GetRunningWheel()[0];

                var list = wheel.GetElementMatchFilter((container) =>
                {
                    if (container.sequenceElement.config.id == Constant11013.PinkScatterElement ||
                        container.sequenceElement.config.id == Constant11013.GoldenScatterElement)
                    {
                        return true;
                    }

                    return false;
                });

                if (list.Count > 0)
                {
                    return base.CheckIsAllWaitEventComplete();
                }
            }
            else if (freeSpinState.IsTriggerFreeSpin)
            {
                return base.CheckIsAllWaitEventComplete();
            }

            return true;
        }

        protected override async void HandleCustomLogic()
        {
            await machineContext.view.Get<LockView11013>().TriggerFreeCount();
            base.HandleCustomLogic();
        }
    }
}