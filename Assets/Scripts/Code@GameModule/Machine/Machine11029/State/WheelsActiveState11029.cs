using System.Collections.Generic;
using UnityEngine;
using System;
using DragonU3DSDK.Network.API.ILProtocol;
using DG.Tweening;
using GameModule;

namespace GameModule
{
    public class WheelsActiveState11029 : WheelsActiveState
    {
        public WheelsActiveState11029(MachineState machineState)
            : base(machineState)
        {
        }
        public void ShowRollsMasks(Wheel wheel)
        {
            var roll = wheel.transform.Find("Rolls");
            if (roll == null)
            {
                return;
            }
            roll.Find("spiningMask").gameObject.SetActive(true);
        }
	    
	    public void FadeOutRollMask(Wheel wheel)
        {
            var roll = wheel.transform.Find("Rolls");
            if (roll == null)
            {
                return;
            }
	        roll.Find("spiningMask").gameObject.SetActive(false);
        }
	    
	    public void HideTwoRoll(Wheel wheel)
        {
	        for (int j = 0; j < wheel.rollCount; j++)
	        {
		        var roll = wheel.GetRoll(j);
		        if (j == 2)
		        {
			        roll.transform.gameObject.SetActive(false);
		        }
	        }
        }

	    public void ShowTwoRoll(Wheel wheel)
	    {
		    for (int j = 0; j < wheel.rollCount; j++)
		    {
			    var roll = wheel.GetRoll(j);
			    if (j == 2)
			    {
				    roll.transform.gameObject.SetActive(true);
			    }
		    }
	    }

	    public void ShowRollsAniMasks(Wheel wheel)
        {
            var roll = wheel.transform.Find("Rolls");
            if (roll == null)
            {
                return;
            }
            roll.Find("spiningMaskAni").gameObject.SetActive(true);
        }
	    
	    public void FadeOuAnitRollMask(Wheel wheel)
        {
            var roll = wheel.transform.Find("Rolls");
            if (roll == null)
            {
                return;
            }
	        roll.Find("spiningMaskAni").gameObject.SetActive(false);
        }
	    
	    public void ShowMapRollsMasks(Wheel wheel)
        {
            var roll = wheel.transform.Find("Rolls");
            if (roll == null)
            {
                return;
            }
            roll.Find("spiningMask").gameObject.SetActive(true);
        }
	    
	    public void FadeOutMapRollMask(Wheel wheel)
        {
            var roll = wheel.transform.Find("Rolls");
            if (roll == null)
            {
                return;
            }
	        roll.Find("spiningMask").gameObject.SetActive(false);
        }
	    public override void UpdateRunningWheelState(GameResult gameResult)
        {
            UpdateRunningWheelState(gameResult.IsReSpin, gameResult.IsFreeSpin, false);
        }

        public bool IsInLink { get; protected set; }

        public void UpdateRunningWheelState(bool isLink, bool isFree, bool updateReelSequence = true)
        {
            IsInLink = false;
            if (isLink)
            {
                IsInLink = true;
                UpdateRunningWheel(new List<string>() {"WheelLinkGame"}, updateReelSequence);
                UpdateElementContainerSize(false);
            }
            else if (isFree)
            {
	            uint freeSpinId = machineState.machineContext.state.Get<FreeSpinState>().freeSpinId;
	            if (freeSpinId < 3)
	            {
		            UpdateRunningWheel(new List<string>(){Constant11029.GetListFree(freeSpinId)},updateReelSequence);
		            UpdateElementContainerSize(false);
	            }
	            else
	            {
		            UpdateRunningWheel(Constant11029.ListFreeNodeGame,updateReelSequence);
		            UpdateElementContainerSize(true);
	            }
            }
            else
            {
                UpdateRunningWheel(new List<string>() {"WheelBaseGame"}, updateReelSequence);
                UpdateElementContainerSize(false);
                machineState.machineContext.view.Get<WheelBonus11029>().transform.gameObject.SetActive(false);
            }
        }

        public void UpdateElementContainerSize(bool change)
        {
	        var wheels = GetRunningWheel();
	        for (int i = 0; i < wheels.Count; i++)
	        {
		        var wheel = wheels[i];
		        for (int j = 0; j < wheel.rollCount; j++)
		        {
			        var roll = wheel.GetRoll(j);
			        for (int k = 0; k < roll.containerCount; k++)
			        {
				        var elementContainer = roll.GetContainer(k);
				        if (change)
				        {
					        elementContainer.transform.localScale = new Vector3(0.81f, 0.81f, 0.81f);
				        }
				        else
				        {
					        elementContainer.transform.localScale = new Vector3(1f, 1f, 1f);
				        }
			        }
		        }
	        }
        }

	    public virtual void UpdateRunningLinkWheel(List<string> runningWheelsName, bool updateReelSequence = true)
        {
	        var machineContext = machineState.machineContext;
	        for (var i = runningWheel.Count - 1; i >= 0; i--)
	        {
		        if (runningWheel[i].wheelName == "WheelMagicBonusGame")
		        {
			         RemoveRunningWheel(runningWheel[i]);
		        }
	        }

	        for (var i = 0; i < runningWheelsName.Count; i++)
	        {
		        var wheel = machineContext.view.Get<Wheel>(runningWheelsName[i]);
		        if (wheel != null)
		        { 
			        if (wheel.wheelName == "WheelMagicBonusGame" || wheel.wheelName == "WheelMagicBonusGameBig") 
			        { 
				        AddRunningWheel(wheel, -1, updateReelSequence); 
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
				        if (wheel.wheelName == "WheelMagicBonusGame" || wheel.wheelName == "WheelMagicBonusGameBig") 
				        { 
					        maxRollCount = Math.Max(maxRollCount, wheel.rollCount); 
				        } 
			        }
		        }
		        machineContext.state.Get<ReelStopSoundState>().ResetRollCount(maxRollCount); 
	        }
        }

        public override string GetReelNameForWheel(Wheel wheel)
        {
	        uint freeSpinId = machineState.machineContext.state.Get<FreeSpinState>().freeSpinId;
	        if (wheel.wheelName == "WheelBaseGame")
	        {
		        return "Reels";
	        }
	        else if (wheel.wheelName == "WheelMiniGame")
	        {
		        return "MapClassicReels";
	        }
	        else if (wheel.wheelName == "WheelBigPointGame1" || wheel.wheelName == "WheelBigPointGame2" || wheel.wheelName == "WheelBigPointGame3")
	        {
		        switch (freeSpinId)
		        {
			        case 3:
				        return "MapFree1Reels";
			        case 4:
				        return "MapFree2Reels";
			        case 5:
				        return "MapFree5Reels";
			        case 6:
				        return "MapFree2Reels";
			        case 7:
				        return "MapFree3Reels";
			        case 8:
				        return "MapFree2Reels";
		        }
	        }
	        else if (wheel.wheelName == "WheelFreeGame")
	        {
		        return "FreeReels";
	        }
	        else if (wheel.wheelName == "WheelMagicBonusGame")
	        {
		        return "BelleNormalReels";
	        }
	        else if (wheel.wheelName == "WheelMagicBonusGameBig")
	        {
		        return "BelleGemReels";
	        }

	        return "Reels";
        }
    }
}