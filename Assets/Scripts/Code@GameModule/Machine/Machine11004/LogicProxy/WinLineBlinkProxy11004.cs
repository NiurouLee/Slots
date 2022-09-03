namespace GameModule
{
    public class WinLineBlinkProxy11004: WinLineBlinkProxy
    {
        public WinLineBlinkProxy11004(MachineContext context) : base(context)
        {
        }
        
        protected override void HandleCommonLogic()
        {
            var wheels = wheelsRunningStatusState.GetRunningWheel();
            
            if (wheels != null && wheels.Count > 0)
            {
                machineContext.AddWaitEvent(WaitEvent.WAIT_BLINK_ALL_WIN_LINE);
               
                for (var i = 0; i < wheels.Count; i++)
                {
                    wheels[i].winLineAnimationController.BlinkAllWinLine(() =>
                    {

                        RestoreFreeReTriggerWheelElement();
                        
                        machineContext.RemoveWaitEvent(WaitEvent.WAIT_BLINK_ALL_WIN_LINE);
                    });
                }
                
                CheckAndShowFiveOfKindAnimation(wheels);
            }
        }
        
        
        protected virtual void RestoreFreeReTriggerWheelElement()
        {
            var freeSpinState = machineContext.state.Get<FreeSpinState>();
            if (freeSpinState.NewCount > 0 && !freeSpinState.IsTriggerFreeSpin)
            {
                // var triggerPanels = machineContext.state.Get<FreeSpinState>().triggerPanels;
                var nowSpinResult = machineContext.state.Get<WheelState11004>().GetSpinResult();

                if (nowSpinResult != null)
                {
                    Wheel wheel = machineContext.state.Get<WheelsActiveState>().GetRunningWheel()[0];
                    
                    var wheelSpinResult = nowSpinResult.GameResult.Panels[0];
                    int xCount = wheelSpinResult.Columns.Count;
                    
                    var elementConfigSet = machineContext.state.machineConfig.elementConfigSet;
                    var scatterConfig = elementConfigSet.GetElementConfig(Constant11004.ScatterElementId);
                    
                    for (int x = 0;  x< xCount; x++)
                    {
                        var column = wheelSpinResult.Columns[x];
                        int yCount = column.Symbols.Count;
                        for (int y = 0; y < yCount; y++)
                        {
                            uint elementId = column.Symbols[y];
                            if (Constant11004.ScatterElementId == elementId)
                            {
                                wheel.GetRoll(x).GetVisibleContainer(y).UpdateElement(new SequenceElement(scatterConfig,machineContext));
                            }
                        }
                    }
                
            
                    //machineContext.view.Get<Wheel>().ForceUpdateElementOnWheel();
                    
                }

                
            }
        }
    }
}