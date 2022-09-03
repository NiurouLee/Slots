using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;

namespace GameModule
{
    public class LockView11015: TransformHolder
    {
        protected LockPrefabLayer lockPrefabLayer;
        ElementConfigSet elementConfigSet = null;
        
        public LockView11015(Transform inTransform) : base(inTransform)
        {
           
        }

        public override void Initialize(MachineContext inContext)
        {
            base.Initialize(inContext);
            lockPrefabLayer = new LockPrefabLayer(context.view.Get<Wheel>(),context);
            elementConfigSet = context.state.machineConfig.elementConfigSet;
        }


        
        public async Task RefreshLock()
        {
            var wheel = context.state.Get<WheelsActiveState>().GetRunningWheel()[0];

            lockPrefabLayer.RefreshWheel(wheel);

            var freeSpinState = this.context.state.Get<FreeSpinState>();
            bool isFree = freeSpinState.IsInFreeSpin && !freeSpinState.IsOver;

            List<Task> listTask = new List<Task>();
            
            var listRoll = wheel.GetVisibleElementInfo();
            int index = 0;
            for (int x = 0; x < listRoll.Count; x++)
            {
                var roll = listRoll[x];
                for (int y = 0; y < roll.Count; y++)
                {
                    var element = roll[y];
                    if (element.config.id == Constant11015.ZeusElementId)
                    {
                        var objLock = lockPrefabLayer.ReplaceOrAttachToElement("S01Lock",y,x);

                        if (isFree)
                        {
                            objLock.transform.localScale =  Constant11015.ElementFreeScale;
                        }
                        else
                        {
                            objLock.transform.localScale = Vector3.one;
                        }

                        index = index + 1;
                        SortingGroup sortingGroup = objLock.GetComponent<SortingGroup>();
                        sortingGroup.sortingOrder = 9000 + index;
                        
                        Animator animatorLock = objLock.GetComponent<Animator>();
                        if (animatorLock != null)
                        {
                            listTask.Add(XUtility.PlayAnimationAsync(animatorLock,"Lock",context));
                        }
                    }
                }
            }

            if (listTask.Count > 0)
            {
                
                await Task.WhenAll(listTask);
            }
        }


        public void ClearLock()
        {
            if (lockPrefabLayer != null)
            {
                var wheel = context.state.Get<WheelsActiveState>().GetRunningWheel()[0];
                for (int x = 0; x < 5; x++)
                {
                    for (int y = 0; y < 12; y++)
                    {
                       var obj = lockPrefabLayer.GetElement(y, x);
                       if (obj != null)
                       {
                           //替换到wheel上
                           var elementConfig =
                               elementConfigSet.GetElementConfig(Constant11015.ZeusElementId);

                           var container = wheel.GetRoll(x).GetVisibleContainer(y);
                           container.UpdateElement(new SequenceElement(elementConfig,context));
                           container.PlayElementAnimation("LockIdle");
                           container.ShiftSortOrder(true);
                           var element = container.GetElement() as ElementZeus11015;
                           element.IsLock = true;
                       }
                    }
                }
                
                lockPrefabLayer.Clear(); 
            }

            
        }
    }
}