//  **********************************************
//  Copyright(c) 2021 by com.ustar
//  All right reserved
// 
//  Author : Besure.Chen
//  Date :2021-11-03 11:07
//  Ver : 1.0.0
//  Description : WheelsActiveState11016.cs
//  ChangeLog :
//  **********************************************

using System;
using System.Collections.Generic;
using DragonU3DSDK.Network.API.ILProtocol;

namespace GameModule
{
    public class WheelsActiveState11016: WheelsActiveState
    {
        public int LastPanelCount;
        public WheelsActiveState11016(MachineState machineState)
            :base(machineState)
        {
            spinningOrder = WheelSpinningOrder.SAME_TIME;
        }
        public override void UpdateRunningWheelState(GameResult gameResult)
        {
            if (gameResult.IsFreeSpin)
            {
                UpdateFreeWheelState();
            }
            else
            {
                UpdateBaseWheelState();
            }
        }
        public void UpdateFreeWheelState()
        {
            UpdateFreeBg(true);
          //  EventBus.Dispatch(new EventSystemWidgetActive(false));
            machineState.machineContext.view.Get<JackPotPanel>().Hide();
            machineState.machineContext.view.Get<FreeTopView11016>().Show();
            machineState.machineContext.view.Get<CollectView11016>().Hide();
            UpdateFreePanels();
            var extraState = machineState.Get<ExtraState11016>();
            var nextPanelCount = extraState.GetNextPanelCount(extraState.CurrentPanelCount);
            machineState.machineContext.view.Get<FreeTopView11016>().UpdateCollectCount(extraState.BombLeftNextLevel, nextPanelCount);

        }

        private void UpdateFreeBg(bool isFree)
        {
            machineState.machineContext.transform.Find("Background/BGBaseGame/BG0").gameObject.SetActive(true);
            machineState.machineContext.transform.Find("Background/BGBaseGame/BG1").gameObject.SetActive(isFree);
        }

        public void UpdateFreePanels(bool updatePosition =true)
        {
            var wheelNames = new List<string>();
            var currentPanelCount = machineState.Get<ExtraState11016>().CurrentPanelCount;
            for (int i = 0; i < currentPanelCount; i++)
            {
                wheelNames.Add($"WheelFreeGame{i}");
            }
            UpdateRunningWheel(wheelNames);
            UpdatePosition(updatePosition);
        }

        public void UpdatePosition(bool update)
        {
            var wheels = GetRunningWheel();
            for (int i = 0; i < wheels.Count; i++)
            {
                var wheel = wheels[i];
                for (int j = 0; j < wheel.rollCount; j++)
                {
                    var roll = wheel.GetRoll(j);
                    for (int k = 0; k < roll.rowCount; k++)
                    {
                        var elementContainer = roll.GetVisibleContainer(k);
                        var element = elementContainer.GetElement() as Element11016;
                        element.HideTrail();
                    }
                }
            }
            if (!update) return;
            var currentPanelCount = machineState.Get<ExtraState11016>().CurrentPanelCount;
            var multiFreeView = machineState.machineContext.view.Get<MultiFreePanelView11016>();
            var panelPosition = multiFreeView.GetPanelsPositions(currentPanelCount);
            for (int i = 0; i < panelPosition.Count; i++)
            {
                var transPos = panelPosition[i];
                wheels[i].transform.position = transPos.position;
                wheels[i].transform.localScale = transPos.localScale;
            } 
        }
        public void UpdateBaseWheelState()
        {
            UpdateFreeBg(false);
           // EventBus.Dispatch(new EventSystemWidgetActive(true));
            machineState.machineContext.view.Get<JackPotPanel>().Show();
            machineState.machineContext.view.Get<FreeTopView11016>().Hide();
            machineState.machineContext.view.Get<CollectView11016>().Show();
            
            var extraState = machineState.Get<ExtraState11016>();
            machineState.machineContext.view.Get<CollectView11016>().UpdateFreeCount(extraState.FreeGameLevel);
            UpdateRunningWheel(new List<string>() {"WheelBaseGame"});
        }
        public override string GetReelNameForWheel(Wheel wheel)
        {
            if (wheel.wheelName.Contains("Link"))
            {
                return Constant11010.LinkReels;
            }
            if (wheel.wheelName.Contains("Free"))
            {
                return Constant11010.FreeReels;
            }
            return "Reels";
        }

        public override void RemoveRunningWheel(Wheel wheel)
        {
            wheel.winLineAnimationController.StopAllElementAnimation();
            base.RemoveRunningWheel(wheel);
        }
    }
}