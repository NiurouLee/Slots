/**********************************************

Copyright(c) 2021 by com.ustar
All right reserved

Author : Jian.Wang 
Date : 2020-09-30 21:07:03
Ver : 1.0.0
Description : 
ChangeLog :  
**********************************************/

using GameModule.UI;
using UnityEngine;

namespace GameModule
{
    public class LaunchSceneSwitchAction:SceneSwitchAction
    {
        public LaunchSceneSwitchAction()
            :base(SceneType.TYPE_LAUNCH)
        {
            
        }
        
        public override void DoLeaveSceneAction()
        {
        }
    }
}