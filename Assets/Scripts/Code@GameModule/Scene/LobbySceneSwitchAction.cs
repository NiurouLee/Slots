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
using GameModule.UI;

namespace GameModule
{
    public class LobbySceneSwitchAction : SceneSwitchAction
    {
        public LobbySceneSwitchAction()
            : base(SceneType.TYPE_LOBBY)
        {
            
        }
        public override async Task CreateAndRunTargetScene()
        {
            PerformanceTracker.AddTrackPoint("CreateLobbyScene");
          
            var lobbyScene = await ViewManager.Instance.ShowSceneView<LobbyScene>("LobbyScene", false);
            
            await lobbyScene.PrepareAsyncAssets();
           
            SoundController.PlayBgMusic("LobbyBgMusic");
            PerformanceTracker.FinishTrackPoint("CreateLobbyScene");
        }

        public override void DoLeaveSceneAction()
        {
            Client.Get<SoundController>().Clear();
            base.DoLeaveSceneAction();
        }
        
        public override bool ShowLoadingScreenView(SceneType inSceneType)
        {
            return false;
        }

        public override string GetTargetSceneSwitchViewAddress(SceneType fromSceneType)
        {
            if (fromSceneType == SceneType.TYPE_MACHINE)
                return "AssetsLoading";
            
            return null;
        }
    }
}