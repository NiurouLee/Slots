// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/08/04/10:10
// Ver : 1.0.0
// Description : MachineContextBuilder11004.cs
// ChangeLog :
// **********************************************

using System;
using UnityEngine;

namespace GameModule
{
    public class MachineContextBuilder11014:MachineContextBuilder11001
    {
        public MachineContextBuilder11014(string inMachineId)
            :base(inMachineId)
        {
        }
        
        public override void BindingExtraView(MachineContext machineContext)
        {
            var bingoMapView = machineContext.transform.Find("Wheels/WheelBaseGame/Bingo");
            machineContext.view.Add<BingoMapView11001>(bingoMapView);

            var bonusProgress = machineContext.transform.Find("Wheels/WheelBaseGame/BonusProgress");
            machineContext.view.Add<SuperFreeProgressView11001>(bonusProgress);
            
            var wheelBonusView = machineContext.transform.Find("Wheels/WheelBonusView");
            machineContext.view.Add<BonusWheelView11014>(wheelBonusView);
        }
        
        public override void AdaptMachineView(MachineContext machineContext)
        {
            var wheels = machineContext.transform.Find("Wheels");
            
            int uiTotalHeight = Constant11014.bingoWheelDesignHeight + Constant11014.jackpotPanelHeight;
            var systemUIHeight = MachineConstant.controlPanelVHeight + MachineConstant.topPanelHeight;

            var gameTotalHeight = systemUIHeight + uiTotalHeight;
            
            var deviceHeight = ViewResolution.referenceResolutionPortrait.y;

            int wheelsOffsetY = 0;
            float scale = 1.0f;
            if (gameTotalHeight <= deviceHeight)
            {
                var deltaHeight = deviceHeight - gameTotalHeight;

                if (deltaHeight > 75)
                {
                    wheelsOffsetY = 30;
                }
            }
            else
            {
                scale = 1 - (gameTotalHeight - deviceHeight) / uiTotalHeight;
            }
            
            wheels.localPosition = new Vector3(0, (-deviceHeight * 0.5f + MachineConstant.controlPanelVHeight + wheelsOffsetY) * MachineConstant.pixelPerUnitInv, 0);
            wheels.localScale = new Vector3(scale, scale, scale);

            if (scale < 1)
            {
                var jackpotPanel = machineContext.view.Get<JackPotPanel>();
                jackpotPanel.transform.localScale = new Vector3(scale, scale, scale);
                
                var jackpotIdealPos =
                    Constant11014.bingoWheelDesignHeight * scale + MachineConstant.controlPanelVHeight +
                    Constant11014.jackpotPanelHeight * scale * 0.5f;

                    jackpotPanel.transform.localPosition =
                    new Vector3(0, (jackpotIdealPos -deviceHeight * 0.5f)* MachineConstant.pixelPerUnitInv, 0);
                    
                machineContext.view.Get<BackGroundView11001>().HideLogo();    
            }
            else
            {
                float jackpotPanelStartPos = wheelsOffsetY + 
                    Constant11014.bingoWheelDesignHeight + MachineConstant.controlPanelVHeight +
                    Constant11014.jackpotPanelHeight;

                var availableHeight = deviceHeight - jackpotPanelStartPos - MachineConstant.topPanelVHeight;

                //var jackpotOffset =  Mathf.Min(10, availableHeight / 2);
                
                if(availableHeight > Constant11001.logoHeight - 50)
                {
                    var logoStartPos = jackpotPanelStartPos - deviceHeight * 0.5f +  Constant11001.logoHeight  * 0.5f;
                    
                    machineContext.view.Get<BackGroundView11001>().SetLogoYPosition(logoStartPos * MachineConstant.pixelPerUnitInv);
                }
                else
                {
                    machineContext.view.Get<BackGroundView11001>().HideLogo();
                }
                
                // if(availableHeight > 120)
                // {
                //     jackpotPanelStartPos = jackpotPanelStartPos - Constant11014.jackpotPanelHeight * 0.5f + 40 - deviceHeight * 0.5f;
                //     
                //     machineContext.view.Get<JackPotPanel>().transform.localPosition =
                //         new Vector3(0, jackpotPanelStartPos * MachineConstant.pixelPerUnitInv, 0);
                // }
                // else
                {
                    jackpotPanelStartPos = (jackpotPanelStartPos) - Constant11014.jackpotPanelHeight * 0.5f - deviceHeight * 0.5f;
                    machineContext.view.Get<JackPotPanel>().transform.localPosition =
                        new Vector3(0, jackpotPanelStartPos * MachineConstant.pixelPerUnitInv, 0);
                }
            }
        }
    }
}