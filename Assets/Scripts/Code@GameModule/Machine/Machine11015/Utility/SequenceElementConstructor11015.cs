using System.Collections.Generic;
using UnityEngine;

namespace GameModule
{
    public class SequenceElementConstructor11015: SequenceElementConstructor
    {
        public SequenceElementConstructor11015(MachineContext inMachineContext) : base(inMachineContext)
        {
        }


        protected override ReelSequence AppendExtraSequenceElement(ReelSequence reelSequence, ReelSequence resultSequence, int stopPosition,
            int extraElementAppend,int column,int extraTopElementAppend)
        {
            ReelSequence results = null;

            var wheel = machineContext.state.Get<WheelsActiveState>().GetRunningWheel()[0];
           // var roll = wheel.GetRoll(column);
           // var listElement = roll.GetVisibleSequenceElement();
            bool hasShield = ((WheelState11015)wheel.wheelState).HasShieldInRoll(column);
            
            // for (int i = 0; i < listElement.Count; i++)
            // {
            //     if (listElement[i]!=null && listElement[i].config.id == Constant11015.ShieldElementId)
            //     {
            //         hasShield = true;
            //         break;
            //     }
            // }

            var extraState = machineContext.state.Get<ExtraState11015>();
            var reSpinState = machineContext.state.Get<ReSpinState>();
            
            if ( reSpinState.IsInRespin &&
                (hasShield || extraState.HasWildReel(column)))
            {

                

                var freeState = machineContext.state.Get<FreeSpinState>();
                string reelName = "Reels";
                if (freeState.IsInFreeSpin && !freeState.IsOver)
                {
                    reelName = "FreeWildReels";
                }
                else
                {
                    reelName = "BaseWildReels";
                }

                var reelSequences = GetReelSequences(reelName);

                reelSequence = reelSequences[0];
                results =
                    base.AppendExtraSequenceElement(reelSequence, resultSequence, stopPosition, extraElementAppend,column,extraTopElementAppend);
                

            }
            else
            {
                results =
                    base.AppendExtraSequenceElement(reelSequence, resultSequence, stopPosition, extraElementAppend,column,extraTopElementAppend);

            }

            

            


            int num = 14 - results.sequenceElements.Count;
            
            

            for (int i = 0; i < num; i++)
            {
                //var length = Constant11015.ListLowLevelElementIds.Count;
                //var index = Random.Range(0, length);
                //var element = new SequenceElement(elementConfigSet.GetElementConfig(Constant11015.ListLowLevelElementIds[index]), machineContext);

                //第一个1是原本上面都补了一个
                int index = (stopPosition - 1  - i -1);
                if (index < 0)
                {
                    index = reelSequence.sequenceElements.Count + index;
                }
                

                XDebug.Log("LR:" + reelSequence.sequenceElements.Count + ":Index" + index);
                uint id = reelSequence.sequenceElements[index].config.id;
                
                var element = new SequenceElement(elementConfigSet.GetElementConfig(id), machineContext);

                results.sequenceElements.Insert(0,element);
            }

  
            
            return results;
        }
    }
}