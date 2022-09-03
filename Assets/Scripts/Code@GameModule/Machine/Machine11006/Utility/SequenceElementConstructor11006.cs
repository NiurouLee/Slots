using System.Collections.Generic;
using DragonU3DSDK.Network.API.ILProtocol;
using Google.ilruntime.Protobuf.Collections;

namespace GameModule
{
    public class SequenceElementConstructor11006: SequenceElementConstructor
    {
        public SequenceElementConstructor11006(MachineContext inMachineContext) : base(inMachineContext)
        {
        }


        public override void ConstructReelSequenceFromServerData(ElementConfigSet inElementConfigSet, MapField<string, Reels> reelsMap)
        {
            base.ConstructReelSequenceFromServerData(inElementConfigSet, reelsMap);
            
            var listFree = cachedSequence[Constant11006.freeSeqName];

            List<List<ReelSequence>> listBuffalo = new List<List<ReelSequence>>();
            for (int i = 0; i < 4; i++)
            {
                listBuffalo.Add(new List<ReelSequence>());
            }
            SequenceElement seqElementBuffalo  = new SequenceElement(elementConfigSet.GetElementConfig(Constant11006.normalElementId), machineContext);
            foreach (var reelSequence in listFree)
            {
                for (int i = 0; i < listBuffalo.Count; i++)
                {
                    ReelSequence reelSequenceBuffalo = new ReelSequence(reelSequence.sequenceElements,Constant11006.listBuffaloLevel2ChangeId[i],seqElementBuffalo);
                    listBuffalo[i].Add(reelSequenceBuffalo);
                }
            }

            for (int i = 0; i < listBuffalo.Count; i++)
            {
                cachedSequence.Add(Constant11006.listBuffaloLevel2SequenceName[i],listBuffalo[i]);
            }

        }


        protected override ReelSequence GetFixedReelSequence(List<ReelSequence> sequences, int column, WheelState wheelState)
        {
            ReelSequence reelSequence = new ReelSequence(wheelState.GetActiveSequenceElement(column), null, null);
            return reelSequence;
        }
    }
}