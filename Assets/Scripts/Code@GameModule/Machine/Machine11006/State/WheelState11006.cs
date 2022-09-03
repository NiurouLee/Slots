using System.Collections.Generic;
using DragonU3DSDK.Network.API.ILProtocol;
using UnityEngine;

namespace GameModule
{
    public class WheelState11006: WheelState
    {
        public WheelState11006(MachineState state) : base(state)
        {
            
        }


        public override void UpdateStateOnRoomSetUp(SEnterGame gameEnterInfo)
        {
            base.UpdateStateOnRoomSetUp(gameEnterInfo);
        }


        public override List<SequenceElement> GetActiveSequenceElement(int rollIndex)
        {
            if (machineState.Get<FreeSpinState>().IsInFreeSpin)
            {
                return GetBuffaloList(base.GetActiveSequenceElement(rollIndex), rollIndex);
            }
            else
            {
                return base.GetActiveSequenceElement(rollIndex);
            }
        }

        public override List<SequenceElement> GetActiveSequenceElement(Roll roll)
        {
            if (machineState.Get<FreeSpinState>().IsInFreeSpin)
            {
                return GetBuffaloList( base.GetActiveSequenceElement(roll), roll.rollIndex);
            }
            else
            {
                return base.GetActiveSequenceElement(roll);
            }
            
        }

        private SequenceElement seqElementBuffalo;
        protected List<SequenceElement> GetBuffaloList(List<SequenceElement> list,int rollIndex)
        {
            List<SequenceElement> listBuffaloList = null;
            ExtraState11006 extraState = machineState.Get<ExtraState11006>();
            var listSubtitutes = extraState.GetSubstitutes();
            foreach (var reelPosition in listSubtitutes)
            {
                if (reelPosition != null)
                {
                    if (reelPosition.Col == rollIndex)
                    {
                        if (listBuffaloList == null)
                        {
                            listBuffaloList = new List<SequenceElement>(list);
                        }

                        if (seqElementBuffalo == null)
                        {
                            seqElementBuffalo  = new SequenceElement(this.machineState.machineConfig.elementConfigSet.GetElementConfig(Constant11006.normalElementId), machineState.machineContext);
                        }

                        if (reelPosition.Index < listBuffaloList.Count)
                        {
                            listBuffaloList[reelPosition.Index] = seqElementBuffalo;
                        }
                        else
                        {
                            Debug.LogError($"====ExtraSubstitutes出错 listBuffaloList.Count:{listBuffaloList.Count} reelPosition.Index:{reelPosition.Index}");
                        }


                    }
                }
            }

            if (listBuffaloList == null)
            {
                return list;
            }
            else
            {
                return listBuffaloList;
            }
        }


        private bool isSkipAnticipation = false;

        public void SetSkipAnticipation(bool isSkip)
        {
            isSkipAnticipation = isSkip;
        }
        
        public override int GetAnticipationAnimationStartRollIndex()
        {
            if (isSkipAnticipation)
            {
                return rollCount;
            }
            
            return base.GetAnticipationAnimationStartRollIndex();
        }

        public override bool HasAnticipationAnimationInRollIndex(int rollIndex)
        {
            if (isSkipAnticipation)
            {
                return false;
            }
            else
            {
                return base.HasAnticipationAnimationInRollIndex(rollIndex);

            }

        }
        
        public virtual bool HasAnticipationAnimationInRollIndex()
        {
            
            for (int i = 0; i < rollCount; i++)
            {
                if (HasAnticipationAnimationInRollIndex(i))
                {
                    return true;
                }
            }
            return false;
        }
    }
}