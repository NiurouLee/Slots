/**********************************************

Copyright(c) 2021 by com.ustar
All right reserved

Author : Jian.Wang 
Date : 2020-09-30 21:07:03
Ver : 1.0.0
Description : 
ChangeLog :  
**********************************************/


using System;
using System.Threading.Tasks;
using DragonU3DSDK.Network.API.ILProtocol;

using GameModule.UI;
using UnityEngine;

namespace GameModule
{
    public class LoadingSceneSwitchAction:SceneSwitchAction
    {
        public LoadingSceneSwitchAction()
            :base(SceneType.TYPE_LOADING)
        {
        }
        
        public override void InitializeLeaveMask()
        {
            readyLeaveMask =  (int) SwitchMask.MASK_SERVER_READY;
            readyLeaveMask |=  (int) SwitchMask.MASK_RESOURCE_READY;
        }
        
        // public override string GetTargetSceneSwitchViewAddress()
        // {
        //     return "SceneSwitchView";
        // }

        public override void DoLeaveSceneAction()
        {
            
        }

        public override bool ShowLoadingScreenView(SceneType inSceneType)
        {
            return false;
        }

        public override async Task CreateAndRunTargetScene()
        {
            BiManagerGameModule.Instance.SendGameEvent(BiEventFortuneX.Types.GameEventType.GameEventStartLoadLoading);

            var loadingInstanceGo = GameObject.Find("Launcher/SceneContainer/CanvasScene/AssetsLoading");

            if (loadingInstanceGo)
            {
                await ViewManager.Instance.BindingSceneView<LoadingScene>(loadingInstanceGo.transform, false);
            }
            else
            {
                await ViewManager.Instance.ShowSceneView<LoadingScene>("AssetsLoading", false);
            }
        }
    }
}