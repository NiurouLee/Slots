using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace GameModule
{
    public class SubRoundFinishProxy11006 : SubRoundFinishProxy
    {
        private ExtraState11006 extraState;
        private FreeSpinState freeSpinState;
        private WheelState11006 wheelState;
        private SequenceElementConstructor11006 sequenceElementConstructor;

        public SubRoundFinishProxy11006(MachineContext context) : base(context)
        {
            extraState = context.state.Get<ExtraState11006>();
            freeSpinState = context.state.Get<FreeSpinState>();
            wheelState = context.state.Get<WheelState11006>();
            sequenceElementConstructor = context.sequenceElementConstructor as SequenceElementConstructor11006;
        }

        protected override void RegisterInterestedWaitEvent()
        {
            waitEvents.Add(WaitEvent.WAIT_BLINK_ALL_WIN_LINE);
        }

        protected override bool CheckIsAllWaitEventComplete()
        {
            if (!freeSpinState.IsInFreeSpin)
            {
                return true;
            }

            if (extraState.GetOldBuffaloLevel() != extraState.GetBuffaloLevel())
            {
                var highElement = GetHighElementContainers();
                if (waitEvents.Count > 0 && highElement.Count > 0)
                    return !machineContext.HasWaitEvent(waitEvents);
            }

            return true;
        }
        protected override async void HandleCustomLogic()
        {
            await RefreshBuffaloElement();

            //RefreshAllBuffalo();

            base.HandleCustomLogic();
        }

        List<ElementContainer> GetHighElementContainers()
        {
            List<uint> listBuffaloId = new List<uint>();
            for (int i = extraState.GetOldBuffaloLevel(); i < extraState.GetBuffaloLevel(); i++)
            {
                if (i < Constant11006.listBuffaloLevel2Upgrade.Count)
                {
                    listBuffaloId.Add(Constant11006.listBuffaloLevel2ElementId[i]);
                }
            }

            var wheel = machineContext.view.Get<Wheel>();
            var highElementContainers = wheel.GetElementMatchFilter((container) =>
            {
                if (listBuffaloId.Contains(container.sequenceElement.config.id))
                {
                    return true;
                }

                return false;
            });

            return highElementContainers;
        }
        
        
        private SequenceElement sequenceElement;
        protected async Task RefreshBuffaloElement()
        {
            if (!freeSpinState.IsInFreeSpin)
            {
                return;
            }

            if (extraState.GetOldBuffaloLevel() != extraState.GetBuffaloLevel())
            {
                var highElementContainers = GetHighElementContainers();

                if (highElementContainers.Count > 0)
                {
                    AudioUtil.Instance.PlayAudioFx("ChangeToS01");
                    TaskCompletionSource<bool> taskCompletionSource = new TaskCompletionSource<bool>();
                    int completeNum = 0;
                    for (int i = 0; i < highElementContainers.Count; i++)
                    {
                        var container = highElementContainers[i];
                        //Debug.LogError($"======config.name:{container.sequenceElement.config.name}");
                        container.PlayElementAnimation("Change", false, () =>
                        {
                            //container.UpdateElementMaskInteraction(false);
                            
                            
                            if (sequenceElement == null)
                            {
                                var elementConfigSet = machineContext.state.machineConfig.GetElementConfigSet();
                                sequenceElement = new SequenceElement(elementConfigSet.GetElementConfig(Constant11006.normalElementId),
                                    machineContext);
                            }

                            container.UpdateElement(sequenceElement);
                            
                            
                            completeNum++;
                            //Debug.LogError($"=======completeNum:{completeNum}  highElementContainers.Count:{highElementContainers.Count}");
                            if (completeNum >= highElementContainers.Count)
                            {
                                taskCompletionSource.SetResult(true);
                            }
                        });
                    }

                    await taskCompletionSource.Task;
                }
            }
        }

        protected void RefreshAllBuffalo()
        {
            if (freeSpinState.IsInFreeSpin && freeSpinState.NextIsFreeSpin)
            {
                int buffaloLevel = extraState.GetBuffaloLevel();
                if (buffaloLevel > 0 && buffaloLevel <= Constant11006.listBuffaloLevel2Upgrade.Count)
                {
                    var listFreeSequences = sequenceElementConstructor.GetReelSequences(Constant11006.freeSeqName);

                    var listSubstitutes = extraState.GetSubstitutes();

                    //wheelState.UpdateCurrentActiveSequence(Constant11006.listBuffaloLevel2SequenceName[buffaloLevel-1]);
                }
                else
                {
                    wheelState.UpdateCurrentActiveSequence(Constant11006.freeSeqName);
                }
            }
            else
            {
                wheelState.UpdateCurrentActiveSequence(Constant11006.freeSeqName);
            }
        }
    }
}