using System.Collections.Generic;
using DragonU3DSDK.Network.API.ILProtocol;
using UnityEngine;

namespace GameModule
{
    public class WheelState11002 : WheelState
    {
        // private ReelSequence reelSequence;

        public bool spoiler = false;

        public WheelState11002(MachineState state) : base(state)
        {

        }

        protected override void UpdateStartAndStopIndex(Panel panel)
        {
            var random = Random.Range(0, 100);
            spoiler = random > 50;

            currentSequenceName = panel.ReelsId;
            currentActiveSequence = sequenceElementConstructor.GetReelSequences(currentSequenceName);

            for (var i = 0; i < rollCount; i++)
            {

                ReelSequence reelSequence = i >= currentActiveSequence.Count
                    ? currentActiveSequence[0]
                    : currentActiveSequence[i];

                var maxIndex = reelSequence.sequenceElements.Count;
                rollStopReelIndex[i] = (int)panel.Columns[i].StopPosition;
                rollStartReelIndex[i] = ((int)panel.Columns[i].StopPosition - 1 + maxIndex) % maxIndex;

                anticipationInColumn[i] = spoiler ? false : panel.Columns[i].DrumMode;
            }

            DumpAnticipationInfo();
        }

        public override void InitializeWheelState(string inWheelName)
        {
            base.InitializeWheelState(inWheelName);

            currentSequenceName = inWheelName == "Wheel0" ? "Reels" : "FreeReels";
        }

        // public override void UpdateStateOnRoomSetUp(SEnterGame gameEnterInfo)
        // {
        //     base.UpdateStateOnRoomSetUp(gameEnterInfo);
        //     if (reelSequence == null)
        //     {
        //         elementConfigSet = machineState.machineConfig.GetElementConfigSet();
        //         reelSequence = new ReelSequence(elementConfigSet.GetElementConfig(18), 10, machineState.machineContext);
        //     }
        // }

        // public override List<SequenceElement> GetActiveSequenceElement(Roll roll)
        // {
        //     if (IsInChillActive(roll.rollIndex))
        //     {
        //         //小辣椒满的时候显示固定卷轴
        //         return reelSequence.sequenceElements;
        //     }
        //     return base.GetActiveSequenceElement(roll);
        // }

        // public List<SequenceElement> GetWildSequenceElements()
        // {
        //     return reelSequence.sequenceElements;
        // }


        // public override List<SequenceElement> GetSpinResultSequenceElement(Roll roll)
        // {

        //     var extraState = machineState.Get<ExtraState11002>();
        //     var betState = machineState.Get<BetState>();
        //     bool isActive = extraState.GetChillActive(betState.totalBet)[roll.rollIndex];
        //     uint numChill = extraState.GetChillState(betState.totalBet)[roll.rollIndex];

        //     if (isActive)
        //     {
        //         return reelSequence.sequenceElements;
        //     }
        //     return base.GetSpinResultSequenceElement(roll);
        // }

        // public override int GetRollStartReelIndex(int rollIndex)
        // {
        //     if (IsInChillActive(rollIndex))
        //     {
        //         //小辣椒满的时候显示固定卷轴
        //         return 0;
        //     }

        //     return base.GetRollStartReelIndex(rollIndex);
        // }


        protected bool IsInChillActive(int rollIndex)
        {
            var extraState = machineState.Get<ExtraState11002>();
            var betState = machineState.Get<BetState>();
            bool isActive = extraState.GetChillActive(betState.totalBet)[rollIndex];
            uint numChill = extraState.GetChillState(betState.totalBet)[rollIndex];

            if ((isActive && numChill != 0) ||
                (isActive && numChill == 3))
            {
                return true;
            }
            return false;
        }
    }
}