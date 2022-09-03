using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;

namespace GameModule
{
    public class DoorView11012: TransformHolder
    {
        protected LockPrefabLayer lockElementLayer;
        public DoorView11012(Transform inTransform) : base(inTransform)
        {
        }


        private ElementConfigSet elementConfigSet;
        public override void Initialize(MachineContext inContext)
        {
            base.Initialize(inContext);
            lockElementLayer = new LockPrefabLayer(context.view.Get<Wheel>(),context);
            elementConfigSet = context.state.machineConfig.elementConfigSet;

        }

        public async Task OpenDoor()
        {
            await OpenBaseDoor();
            await OpenFreeDoor();
        }


        protected async Task OpenBaseDoor()
        {
            var freeSpinState = context.state.Get<FreeSpinState>();
            var reSpinState = context.state.Get<ReSpinState>();
            if (!freeSpinState.IsInFreeSpin && !reSpinState.IsInRespin)
            {
                var wheel = context.view.Get<Wheel>();
                lockElementLayer.RefreshWheel(wheel);
                lockElementLayer.Clear();
                
                
                List<Task> listTask = new List<Task>();
                int XCount = wheel.rollCount;
                int YCount = wheel.GetRoll(0).lastVisibleRowIndex - wheel.GetRoll(0).firstVisibleRowIndex;
                int indexDoor = 0;
                for (int x = 0; x < XCount; x++)
                {
                    for (int y = 0; y < YCount; y++)
                    {
                        var container = wheel.GetRoll(x).GetVisibleContainer(y);
                        if (Constant11012.ListDoorElementIds.Contains(container.sequenceElement.config.id))
                        {
                            //container.PlayElementAnimation("Idle");
                            //listTask.Add(Constant11012.OpenDoor(element, context));
                            uint newId = Constant11012.ListDoorToNoElementIds[container.sequenceElement.config.id];
                            var elementConfig = elementConfigSet.GetElementConfig(newId);
                            container.UpdateElement(new SequenceElement(elementConfig,context));
                            var obj = lockElementLayer.ReplaceOrAttachToElement("DoorGroup", y, x);
                            RefreshDoorLayer(obj.transform,indexDoor);
                            
                            listTask.Add(Constant11012.OpenDoor(obj,context));

                            indexDoor++;
                        }
                    }
                }
                
                if (listTask.Count > 0)
                {
                    Constant11012.PlayOpenDoorAudio();
                    await Task.WhenAll(listTask);
                    lockElementLayer.Clear();
                }


            }
        }


        protected void RefreshDoorLayer(Transform tranDoor,int index)
        {
            SortingGroup sortingGroup = tranDoor.Find("B02").GetComponent<SortingGroup>();
            SpriteMask mask = tranDoor.Find("Mask").GetComponent<SpriteMask>();
            mask.frontSortingOrder = 6000 + (index + 1) * 10;
            mask.backSortingOrder = 6000 + index*10;
            sortingGroup.sortingOrder = 6000 + index * 10 + 1;
        }

        protected async Task OpenFreeDoor()
        {
            var freeSpinState = context.state.Get<FreeSpinState>();
            var reSpinState = context.state.Get<ReSpinState>();
            if (freeSpinState.IsInFreeSpin && !reSpinState.IsInRespin)
            {
                var wheel = context.view.Get<Wheel>(2);
                lockElementLayer.RefreshWheel(wheel);
                lockElementLayer.Clear();

                
                
                int XCount = wheel.rollCount;
                int YCount = wheel.GetRoll(0).lastVisibleRowIndex - wheel.GetRoll(0).firstVisibleRowIndex;
                
                var extraState = context.state.Get<ExtraState11012>();
                if (extraState.IsBigDoor() && extraState.LastIsBigDoor())
                {
                    //先替换element
                    for (int x = 0; x < XCount; x++)
                    {
                        for (int y = 0; y < YCount; y++)
                        {
                            var container = wheel.GetRoll(x).GetVisibleContainer(y);
                            if (Constant11012.ListDoorElementIds.Contains(container.sequenceElement.config.id))
                            {
                                uint newId = Constant11012.ListDoorToNoElementIds[container.sequenceElement.config.id];
                                var elementConfig = elementConfigSet.GetElementConfig(newId);
                                container.UpdateElement(new SequenceElement(elementConfig, context));
                            }
                        }
                    }
                    
                    //全屏大门
                   await context.view.Get<TransitionView11012>().OpenBigDoor();
                }
                else
                {
                    List<Task> listTask = new List<Task>();
                    int indexDoor = 0;
                    for (int x = 0; x < XCount; x++)
                    {
                        for (int y = 0; y < YCount; y++)
                        {
                            var container = wheel.GetRoll(x).GetVisibleContainer(y);
                            if (Constant11012.ListDoorElementIds.Contains(container.sequenceElement.config.id))
                            {

                                uint newId = Constant11012.ListDoorToNoElementIds[container.sequenceElement.config.id];
                                var elementConfig = elementConfigSet.GetElementConfig(newId);
                                container.UpdateElement(new SequenceElement(elementConfig, context));



                                var obj = lockElementLayer.ReplaceOrAttachToElement("DoorGroup", y, x);
                                RefreshDoorLayer(obj.transform, indexDoor);

                                listTask.Add(Constant11012.OpenLockDoor(obj, context));



                                indexDoor++;
                            }
                        }
                    }

                    if (listTask.Count > 0)
                    {
                        Constant11012.PlayOpenDoorAudio();
                        await Task.WhenAll(listTask);
                        lockElementLayer.Clear();
                    }
                }
            }
        }


        public async Task RollingStopLockFreeDoor(int x,int y,uint elementId)
        {
            var wheel = context.state.Get<WheelsActiveState>().GetRunningWheel()[0];

            lockElementLayer.RefreshWheel(wheel);
            
            var extraState = context.state.Get<ExtraState11012>();

            if (extraState.IsBigDoor() && extraState.LastIsBigDoor())
            {
                //播放大门的时候不播放锁小门
            }
            else
            {
                if (Constant11012.ListDoorElementIds.Contains(elementId))
                {
                    string lastName = lockElementLayer.GetContainsName(y, x);
                    if (string.IsNullOrEmpty(lastName))
                    {
                        var obj = lockElementLayer.ReplaceOrAttachToElement("DoorGroup", y, x);
                        
                        
                        await Constant11012.CloseLock(obj, context);
                    }
                }
            }
        }


        public async Task RollingStartLockFreeDoor()
        {
            var freeSpinState = context.state.Get<FreeSpinState>();
            var reSpinState = context.state.Get<ReSpinState>();
            if (freeSpinState.IsInFreeSpin && !reSpinState.IsInRespin)
            {
                var wheel = context.view.Get<Wheel>(2);
                lockElementLayer.RefreshWheel(wheel);
                lockElementLayer.Clear();

                var extraState = context.state.Get<ExtraState11012>();
                if (extraState.IsBigDoor())
                {
                    //全屏大门
                    await context.view.Get<TransitionView11012>().CloseBigDoor();
                }
                else
                {
                    List<Task> listTask = new List<Task>();
                    int XCount = wheel.rollCount;
                    int YCount = wheel.GetRoll(0).lastVisibleRowIndex - wheel.GetRoll(0).firstVisibleRowIndex;
                
                
                    var linkDoor = extraState.GetFreeLockDoorIds();
                    int indexDoor = 0;
                    foreach (var posId in linkDoor)
                    {
                        int x = (int)Mathf.Floor(posId / (float)YCount);
                        int y = (int)(posId % YCount);

                        var objDoor = lockElementLayer.ReplaceOrAttachToElement("DoorGroup", y, x);
                        listTask.Add(Constant11012.CloseLockDoor(objDoor,context));
                    
                        indexDoor++;
                    }
                
                    if (listTask.Count > 0)
                    {
                        Constant11012.PlayCloseDoorAudio();
                        await Task.WhenAll(listTask);
                        //lockElementLayer.Clear();
                    }
                }
            }
        }
        
        


        public void OpenLinkAnticipation()
        {
            var reSpinState = context.state.Get<ReSpinState>();
            if (reSpinState.IsInRespin && reSpinState.ReSpinCount > 0)
            {
                var extraState = context.state.Get<ExtraState11012>();
                if (extraState.IsLastLinkLock())
                {
                    var wheel = context.state.Get<WheelsActiveState>().GetRunningWheel()[0];
                    lockElementLayer.RefreshWheel(wheel);

                    int lockPos = extraState.GetLastLinkLockPosition();
                    AudioUtil.Instance.PlayAudioFx("Link_J01_Anticipation", true);
                    lockElementLayer.ReplaceOrAttachToElement("AnticipationSingle", 0, lockPos);
                }
            }
        }
        
        
        public void CloseLinkAnticipation()
        {
            var reSpinState = context.state.Get<ReSpinState>();
            if (reSpinState.IsInRespin)
            {
                AudioUtil.Instance.StopAudioFx("Link_J01_Anticipation");
                if (reSpinState.ReSpinCount <= 0)
                    lockElementLayer.Clear();
            }
        }
    }
}