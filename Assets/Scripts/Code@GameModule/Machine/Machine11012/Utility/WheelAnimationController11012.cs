using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GameModule
{
    public class WheelAnimationController11012: WheelAnimationController
    {

        protected DoorView11012 doorView;
        public override async void OnRollSpinningStopped(int rollIndex, Action rollLogicEnd)
        {
            
            var machineContext = ViewManager.Instance.GetSceneView<MachineScene>().viewController.machineContext;
            if (doorView == null)
            {
                doorView = machineContext.view.Get<DoorView11012>();
            }
            
            
            var freeSpinState = machineContext.state.Get<FreeSpinState>();
            if (freeSpinState.IsInFreeSpin)
            {
                List<Task> listTask = new List<Task>();
                var roll = wheel.GetRoll(rollIndex);
                int count = roll.rowCount;
                for (int i = 0; i < count; i++)
                {
                    var container =  roll.GetVisibleContainer(i);
                    uint id = container.sequenceElement.config.id;
                    listTask.Add(doorView.RollingStopLockFreeDoor(rollIndex,i,id));
                }

                if (listTask.Count > 0)
                {
                    AudioUtil.Instance.PlayAudioFxOneShot("FreeSpin_DoorBlink");
                    await Task.WhenAll(listTask);
                }
            }

            
            
            base.OnRollSpinningStopped(rollIndex, rollLogicEnd);
        }
    }
}