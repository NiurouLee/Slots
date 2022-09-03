using System.Collections.Generic;
using UnityEngine;
using System;
using DragonU3DSDK.Network.API.ILProtocol;
using GameModule;
using Google.ilruntime.Protobuf.Collections;

namespace GameModule
{
    public class ExtraState11015 : ExtraState<ZeusGameResultExtraInfo>
    {
        public ExtraState11015(MachineState state)
            :base(state)
        {

	
        }


        public override bool HasSpecialEffectWhenWheelStop()
        {
            return true;
        }

        public bool HasWildReel(int rollIndex)
        {

            if (extraInfo.WildReels.Count == 0)
            {
                return false;
            }

            var column = extraInfo.WildReels[rollIndex];
            if (column.Symbols.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        
        
        public bool HasWildReel()
        {

            if (extraInfo.WildReels.Count == 0)
            {
                return false;
            }

            foreach (var column in extraInfo.WildReels)
            {
                if (column != null)
                {
                    if (column.Symbols.Count > 0)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
        
        

        public RepeatedField<ZeusGameResultExtraInfo.Types.Position> GetWildsPos()
        {
           return extraInfo.StickyWildPositions;
        }


        public RepeatedField<ZeusGameResultExtraInfo.Types.Position> GetZeusPos()
        {
            return extraInfo.StickyZeusPositions;
        }

        public uint GetReTriggerCount()
        {
            var freeSpinState = machineState.Get<FreeSpinState>();
            if (freeSpinState.NewCount == 0)
            {
                var reSpinState = machineState.Get<ReSpinState>();
                var triggerPanels = reSpinState.triggerPanels;
                if (triggerPanels != null && triggerPanels.Count > 0 && triggerPanels[0] != null)
                {
                    long reTriggerCount = 0;
                    for (int i = 0; i < triggerPanels[0].WinLines.Count; i++)
                    {
                        var winLine = triggerPanels[0].WinLines[i];
                        reTriggerCount = reTriggerCount + winLine.FreeSpinCount;
                    }

                    if (reTriggerCount > 0)
                    {
                        return (uint)reTriggerCount;
                    }
                }
            }


            return freeSpinState.NewCount;
        }

    }
}