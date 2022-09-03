//  **********************************************
//  Copyright(c) 2021 by com.ustar
//  All right reserved
// 
//  Author : Besure.Chen
//  Date :2021-08-31 19:14
//  Ver : 1.0.0
//  Description : WheelAnimationController11007.cs
//  ChangeLog :
//  **********************************************

using UnityEngine;

namespace GameModule
{
    public class WheelAnimationController11007: WheelAnimationController
    {
        private GameObject goDrumModer;
        public override void ShowAnticipationAnimation(int rollIndex)
        {
            base.ShowAnticipationAnimation(rollIndex);
            if (goDrumModer == null)
            {
                goDrumModer = wheel.GetContext().transform.Find("Wheels/WheelDrummder").gameObject;   
            }
            if ( wheel.GetAttachedGameObject("AnticipationAnimation") && goDrumModer != null)
            {
                goDrumModer.SetActive(true);
            }
        }
        public override void StopAnticipationAnimation(bool playStopSound = true)
        {
            base.StopAnticipationAnimation(playStopSound);
            if (goDrumModer != null)
            {
                goDrumModer.SetActive(false);
            }
        }
    }
}