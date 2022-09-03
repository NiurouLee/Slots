//  **********************************************
//  Copyright(c) 2021 by com.ustar
//  All right reserved
// 
//  Author : Besure.Chen
//  Date :2021-11-06 16:10
//  Ver : 1.0.0
//  Description : UIFreeGameStart11016.cs
//  ChangeLog :
//  **********************************************

using UnityEngine;
using UnityEngine.UI;

namespace GameModule
{
    public class UIFreeGameStart11016:FreeSpinStartPopUp
    {
        [ComponentBinder("CountText1")]
        protected Text txtFreeReelCount;
        [ComponentBinder("CountText2")]
        protected Text txtNextReelCount;
        [ComponentBinder("CountTextDouble")]
        protected Text freeSpinCountTextDouble;
        
        public UIFreeGameStart11016(Transform transform)
            : base(transform)
        {
            
        }

        public override void Initialize(MachineContext inContext)
        {
            base.Initialize(inContext);

            if (freeSpinCountTextDouble)
            {
                SetTransformActive(freeSpinCountTextDouble.transform,false);
                if (context.state.Get<FreeSpinState>().LeftCount > 9 && freeSpinCountTextDouble)
                {
                    freeSpinCountTextDouble.text = freeSpinCountText.text;
                    SetTransformActive(freeSpinCountText.transform,false);
                    SetTransformActive(freeSpinCountTextDouble.transform,true);
                }   
            }

            var extraState = inContext.state.Get<ExtraState11016>();
            txtFreeReelCount.text = extraState.CurrentPanelCount.ToString();
            txtNextReelCount.text = "9";
        }
    }
}