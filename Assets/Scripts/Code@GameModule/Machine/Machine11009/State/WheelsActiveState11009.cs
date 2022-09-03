using System.Collections.Generic;
using DragonU3DSDK.Network.API.ILProtocol;
using GameModule.Utility;
using UnityEngine;

namespace GameModule
{
    public class WheelsActiveState11009: WheelsActiveState
    {
        public WheelsActiveState11009(MachineState machineState) : base(machineState)
        {
        
        
        
        
        }


        public override void UpdateRunningWheelState(GameResult gameResult)
        {
            FreeSpinState11009 freeSpinState = machineState.machineContext.state.Get<FreeSpinState11009>();
            UpdateRunningWheelState(!freeSpinState.IsOver);
        }
        
        
        public virtual void UpdateRunningWheelState(bool isFreeSpin)
        {
            
            string wheelName = "WheelBaseGame";
            
            if (isFreeSpin)
            {
                var extraState = machineState.machineContext.state.Get<ExtraState11009>();
                if (extraState.GetFreeGreenState())
                {
                    wheelName = "WheelFreeGame5X6";
                }
                else
                {
                    wheelName = "WheelFreeGame5X4";
                }
                
                
                
            }
            
            UpdateRunningWheel(new List<string>() {wheelName},true);

            if (isFreeSpin)
            {
                RefreshDefultWheel(this.runningWheel[0]);
            }

            CloseElementMask();
            

            this.machineState.machineContext.view.Get<BoxesView11009>().AttachPoint(this.runningWheel[0]);

            var jackpotView = machineState.machineContext.view.Get<JackpotView11009>();
            jackpotView.RefreshJackpotState();
            jackpotView.RefreshJackpotNoAnim();
            
            var boxesView = machineState.machineContext.view.Get<BoxesView11009>();
            boxesView.RefreshBoxStateNoAnim();
            boxesView.SetCountNoticeState();
            
            
        }
        
        
        public void RefreshDefultWheel(Wheel wheel)
        {
            var wheelState = wheel.wheelState;
            List<int> listIndex = new List<int>();
            for (int i = 0; i < wheelState.rollCount; i++)
            {
                listIndex.Add(0);
            }
            
            wheelState.UpdateCurrentActiveSequence(GetReelNameForWheel(wheel),listIndex);
            wheel.ForceUpdateElementOnWheel(true,true);
        }


        public void CloseElementMask()
        {
            
            var listContainer = this.runningWheel[0].GetElementMatchFilter((container) =>
            {
                uint id = container.sequenceElement.config.id;
                if (Constant11009.ListCollectElementId.Contains(id) ||
                    Constant11009.ListElementIdGoldenVaraint.Contains(id) ||
                    Constant11009.ListElementIdPurpleVaraint.Contains(id))
                {
                    return true;
                }

                return false;
            });
            
          
            for (int i = 0; i < listContainer.Count; i++)
            {
                var container =  listContainer[i];
                container.GetElement().UpdateMaskInteraction(SpriteMaskInteraction.None);
                
                var listText = container.GetElement().transform.GetComponentsInChildren<TextMesh>();
                // foreach (var itemText in listText)
                // {
                //     itemText.font.material.SetFloat("_Stencil",8);
                // }
            }
        }


        public override string GetReelNameForWheel(Wheel wheel)
        {

            var extraState = machineState.machineContext.state.Get<ExtraState11009>();
            
            string reelName = "Reels";
            switch (wheel.wheelName)
            {
                case "WheelBaseGame" :
                    reelName = "Reels";
                    break;
                case "WheelFreeGame5X4" ://无绿色

                    if (extraState.GetFreePurpleState() && extraState.GetFreeRedState())
                    {
                        //紫红
                        reelName = "SuperFree1Reels";
                    }
                    else if(extraState.GetFreeRedState())
                    {
                        //红
                        reelName = "Free2Reels";
                    }
                    else if(extraState.GetFreePurpleState())
                    {
                        //紫
                        reelName = "Free3Reels";
                    }

                    break;
                case "WheelFreeGame5X6" : //有绿色

                    if (extraState.GetFreeRedState() && extraState.GetFreePurpleState())
                    {
                        //紫绿红
                        reelName = "ExtraSuperFreeReels";
                    }
                    else if (extraState.GetFreeRedState())
                    {
                        //绿红
                        reelName = "SuperFree3Reels";
                    }
                    else if(extraState.GetFreePurpleState())
                    {
                        //绿紫
                        reelName = "SuperFree2Reels"; 
                    }
                    else
                    {
                        //绿
                        reelName = "Free1Reels";
                    }
                    
                    break;
            }
            
            return reelName;
        }
    }
}