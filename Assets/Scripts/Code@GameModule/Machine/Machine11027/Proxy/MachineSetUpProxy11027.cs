namespace GameModule
{
    public class MachineSetUpProxy11027:MachineSetUpProxy
    {
        public MachineSetUpProxy11027(MachineContext context) : base(context)
        {
        }
        
        protected override void UpdateViewWhenRoomSetUp()
        {
	        base.UpdateViewWhenRoomSetUp();
            UpdateCollectGroupView();
            UpdateWheelBonusView();
        }
        
		//收集区
		private void UpdateCollectGroupView()
        { 
             machineContext.view.Get<CollectionGroup11027>().ShowCollectionGroup(false);
        }
        private void UpdateWheelBonusView()
        {
            machineContext.view.Get<WheelRollingView11027>().InitializeWheelView(true,false);
        }
    }
}