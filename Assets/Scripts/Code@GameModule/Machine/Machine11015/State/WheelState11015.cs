using System.Collections.Generic;
using System.Linq;
using DragonU3DSDK.Network.API.ILProtocol;

namespace GameModule
{
    public class WheelState11015: WheelState
    {
        private List<bool> hasShieldList;
        
        public WheelState11015(MachineState state) : base(state)
        {
            
        }

        public override void InitializeWheelState(string inWheelName)
        {
            base.InitializeWheelState(inWheelName);
           
            hasShieldList = new List<bool>(wheelConfig.rollCount);

            for (var i = 0; i < wheelConfig.rollCount; i++)
            {
                hasShieldList.Add(false);
            }
        }

        public override List<SequenceElement> GetActiveSequenceElement(int rollIndex)
        {
            return base.GetActiveSequenceElement(rollIndex);
        }

        public override void UpdateWheelStateInfo(Panel panel)
        {
            base.UpdateWheelStateInfo(panel);
            
            for (var i = 0; i < panel.Columns.Count; i++)
            {
                var column = panel.Columns;
                if (column[i].Symbols.Contains(Constant11015.ShieldElementId))
                {
                    hasShieldList[i] = true;
                }

                hasShieldList[i] = false;
            }
        }

        public bool HasShieldInRoll(int rollIndex)
        {
            return hasShieldList[rollIndex];
        }
        
        public override List<SequenceElement> GetActiveSequenceElement(Roll roll)
        {
            var listElement = roll.GetVisibleSequenceElement();
            bool hasShield = false;
            for (int i = 0; i < listElement.Count; i++)
            {
                if (listElement[i]!=null && listElement[i].config.id == Constant11015.ShieldElementId)
                {
                    hasShield = true;
                    break;
                }
            }

            if (hasShield)
            {
                var freeState = machineState.Get<FreeSpinState>();
                string reelName = "Reels";
                if (freeState.IsInFreeSpin && !freeState.IsOver)
                {
                    reelName = "FreeWildReels";
                }
                else
                {
                    reelName = "BaseWildReels";
                }
                var wildReels = sequenceElementConstructor.GetReelSequences(reelName);
                return wildReels[0].sequenceElements;
            }
            else
            {
                return base.GetActiveSequenceElement(roll);
            }
        }
    }
}