namespace GameModule
{
    public class MachineSetUpProxy11021: MachineSetUpProxy
    {
        public MachineSetUpProxy11021(MachineContext context) : base(context)
        {
        }


        protected override void SeekLogicTypeToJump()
        {

            if (machineContext.state.Get<ExtraState>().HasBonusGame())
            {
                logicStepToJump = LogicStepType.STEP_BONUS;
                return;
            }
 
            var freeSpinState = machineContext.state.Get<FreeSpinState>();
            var reSpinState = machineContext.state.Get<ReSpinState>();
             
            if (reSpinState != null && (reSpinState.NextIsReSpin || reSpinState.ReSpinNeedSettle))
            {
                logicStepToJump = LogicStepType.STEP_RE_SPIN;
                return;
            }

            if (freeSpinState != null && (freeSpinState.NextIsFreeSpin || freeSpinState.FreeNeedSettle))
            {
                logicStepToJump = LogicStepType.STEP_FREE_GAME;
                return;
            }
        }


        protected override void UpdateViewWhenRoomSetUp()
        {
            base.UpdateViewWhenRoomSetUp();
            machineContext.view.Get<JackpotPanel11021>().UpdateJackpotLockState(false,true);
        }

        protected override void HandleCustomLogic()
        {
            base.HandleCustomLogic();
            
            machineContext.view.Get<TitlePrizeView>().RefreshInfo();
        }
    }
}