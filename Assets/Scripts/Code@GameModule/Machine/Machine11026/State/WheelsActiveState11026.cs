using System.Collections.Generic;
using UnityEngine;
using System;
using DragonU3DSDK.Network.API.ILProtocol;
using DG.Tweening;
using GameModule;

namespace GameModule
{
	public class WheelsActiveState11026 : WheelsActiveState
	{
		public WheelsActiveState11026(MachineState machineState)
		:base(machineState)
		{

	
		}
		
		
		public override void UpdateRunningWheelState(GameResult gameResult)
		{
			UpdateRunningWheelState(gameResult.IsReSpin, gameResult.IsFreeSpin,false);
		}

		public bool IsInLink
		{
			get;
			protected set;
		}

		public void UpdateRunningWheelState(bool isLink,bool isFree, bool isTriggerBegin = false, bool updateReelSequence = true)
		{
			IsInLink = false;
			if (isLink)
			{
				IsInLink = true;
				ExtraState11026 extraState = machineState.machineContext.state.Get<ExtraState11026>();
				uint rowCount = isTriggerBegin ? 0: extraState.GetRowsMore();
                UpdateRunningWheel(Constant11026.GetListLink(rowCount),updateReelSequence);
			}
			else if(isFree)
			{
				UpdateRunningWheel(new List<string>() {"WheelFreeGame"},updateReelSequence);
			}
			else
			{
				UpdateRunningWheel(new List<string>() {"WheelBaseGame"},updateReelSequence);
			}
		}

		public override string GetReelNameForWheel(Wheel wheel)
		{
			if (wheel.wheelName == "WheelBaseGame")
			{
				return "Reels";
			}
			else if(wheel.wheelName == "WheelLinkGame1")
			{
				return "MultiLinkReels";

			}
			else if(wheel.wheelName == "WheelLinkGame6")
			{
				 return "BoostLinkReels";
				
			}
			else if(wheel.wheelName == "WheelLinkGame2")
			{
				 return "Coin12LinkReels";
				
			}
			else if(wheel.wheelName == "WheelLinkGame3")
			{
				 return "Coin15LinkReels";
				
			}
			else if(wheel.wheelName == "WheelLinkGame4")
			{
				 return "Coin18LinkReels";
				
			}
			else if(wheel.wheelName == "WheelLinkGame5")
			{
				 return "Coin21LinkReels";
				
			}
			else if(wheel.wheelName == "WheelFreeGame")
			{
				return "FreeReels";
			}
			return "Reels";
		}
		
		     public void ShowRollsMasks(Wheel wheel)
        {
            var roll = wheel.transform.Find("Rolls");
            if (roll == null)
            {
                return;
            }

            if (!IsInLink)
            {
	             var rollCount = wheel.rollCount; 
	             for (var i = 0; i < rollCount; ++i) 
	             { 
		             roll.Find("spiningMask").gameObject.SetActive(true); 
		             var rollColorMask = roll.Find("spiningMask").Find("BlackMask" + i).gameObject; 
		             rollColorMask.SetActive(true); 
		             var render = rollColorMask.GetComponent<SpriteRenderer>(); 
		             DOTween.Kill(render); 
		             render.DOFade(0f, 0f); 
		             render.DOFade(0.6f, 1.0f); 
	             } 
            }
        }

        public void FadeOutRollMask(Wheel wheel, int rollIndex)
        {
            var roll = wheel.transform.Find("Rolls");
            if (roll == null)
            {
                return;
            }

            if (!IsInLink)
            { 
	            var render = roll.Find("spiningMask").Find("BlackMask" + rollIndex).gameObject.GetComponent<SpriteRenderer>();
	            DOTween.Kill(render);
                render.DOFade(0, 0.3f);
            }
        }
        
		public virtual void UpdateRunningLinkWheel(List<string> runningWheelsName, bool updateReelSequence = true)
        {
	        var machineContext = machineState.machineContext;
	        for (var i = runningWheel.Count - 1; i >= 0; i--)
	        {
		        if (runningWheel[i].wheelName == "WheelLinkGame2" || runningWheel[i].wheelName == "WheelLinkGame3" ||
		            runningWheel[i].wheelName == "WheelLinkGame4" || runningWheel[i].wheelName == "WheelLinkGame5")
		        {
			         RemoveRunningWheel(runningWheel[i]);
		        }
	        }

	        for (var i = 0; i < runningWheelsName.Count; i++)
	        {
		        var wheel = machineContext.view.Get<Wheel>(runningWheelsName[i]);
		        if (wheel != null)
		        { 
			        if (wheel.wheelName == "WheelLinkGame2" || wheel.wheelName == "WheelLinkGame3" ||
		             wheel.wheelName == "WheelLinkGame4" || wheel.wheelName == "WheelLinkGame5") 
			        { 
				        AddRunningWheel(wheel, i, updateReelSequence); 
			        }
		        }
	        }
	        if (machineContext.state.Get<ReelStopSoundState>() != null)
	        {
		        var maxRollCount = 0;
		        for (int i = 0; i < runningWheel.Count; i++)
		        {
			        var wheel = runningWheel[i];
			        if (wheel!=null)
			        { 
				        if (wheel.wheelName == "WheelLinkGame2" || wheel.wheelName == "WheelLinkGame3" ||
			              wheel.wheelName == "WheelLinkGame4" || wheel.wheelName == "WheelLinkGame5") 
				        { 
					        maxRollCount = Math.Max(maxRollCount, wheel.rollCount); 
				        } 
			        }
		        }
		        machineContext.state.Get<ReelStopSoundState>().ResetRollCount(maxRollCount); 
	        }
        }
	}
}