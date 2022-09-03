/**********************************************

Copyright(c) 2021 by com.ustar
All right reserved

Author : Jian.Wang 
Date : 2021-07-30 21:07:03
Ver : 1.0.0
Description : MachineSceneSwitchAction
ChangeLog :  
**********************************************/


 
using System.Threading.Tasks;
 

namespace GameModule
{
    public class MachineSceneSwitchAction: SceneSwitchAction
    {
        private MachineAssetProvider machineAssetProvider;

        private string _machineId = "";
        private string _assetId = "";
        private string _enterType = "";
        public MachineSceneSwitchAction()
            : base(SceneType.TYPE_MACHINE)
        {
            
        }

        /// <summary>
        /// 切换场景前, 初始化标记位
        /// </summary>
        public override void InitializeEnterMask()
        {
            readyEnterMask = 0;
            readyEnterMask |= (int)SwitchMask.MASK_SERVER_READY;
            readyEnterMask |= (int) SwitchMask.MASK_RESOURCE_READY;
        }
        
        /// <summary>
        /// 切换场景前, 初始化标记位
        /// </summary>
        public override void InitializeLeaveMask()
        {
            readyLeaveMask = 0;
            readyLeaveMask |= (int) SwitchMask.MASK_RESOURCE_READY;
        }
 
        public override void CleanUpAfterLeaveScene()
        {
            
        }
        
        public override bool IsPortraitScene()
        {
           return Client.Get<MachineLogicController>().IsPortraitMachine(_assetId);
        }

        public override async void DoLeaveSceneAction()
        {
            Client.Get<SoundController>().Clear();
            
            var machineScene = ViewManager.Instance.GetSceneView<MachineScene>();
            
            if (machineScene != null)
            {
                machineScene.viewController.PauseScene();
            }

            await XUtility.WaitNFrame(1);
            readyLeaveMask = 0;
            CheckReadyLeaveMask();
        }
        
        public override void SetUpSwitchEventData(EventSwitchScene inEventSwitchScene)
        {
            base.SetUpSwitchEventData(inEventSwitchScene);
            
            _machineId = eventSwitchScene.machineId;
            _assetId = eventSwitchScene.assetId;
            _enterType = eventSwitchScene.enterType;

            if (string.IsNullOrEmpty(_assetId))
            {
                _assetId = _machineId;
            }
        }

        public override void DoEnterSceneAction()
        {
            machineAssetProvider = new MachineAssetProvider(_machineId, _assetId);
          
            //Log.LogStateEvent(State.LoadingSlotAssets, null, StepId.ID_LOADING_SLOT);
            machineAssetProvider.LoadMachineAsset();
            
            SoundController.PlaySfx("Machine_Loading");
            
            EventBus.Dispatch(new EventRequestEnterGame(_machineId, _enterType));
        }

        public override string GetTargetSceneSwitchViewAddress(SceneType fromSceneType)
        {
            return $"Loading{_assetId}";
        }

        public override bool OnSceneSwitchMask(SwitchMask mask)
        {
            if (mask == SwitchMask.MASK_RESOURCE_READY)
            { 
                
            }
            return base.OnSceneSwitchMask(mask);
        }

        public override void OnAbortSwitchScene()
        {
            if (machineAssetProvider != null)
            {
                machineAssetProvider.ReleaseAssets();
                machineAssetProvider = null;
                SoundController.StopSfx("Machine_Loading");
            }

            base.OnAbortSwitchScene();
        }
        
        public override  float GetSwitchProgress()
        {
            if (machineAssetProvider != null)
            {
               return machineAssetProvider.GetCompleteProgress();
            }
            return 0;
        }

        public override async  Task CreateAndRunTargetScene()
        {
            
            MachineConstant.UpdateConstant();
            
            var machineSceneGameObject = machineAssetProvider.InstantiateGameObject("MachineScene" + _assetId);

            var machineScene = View.CreateView<MachineScene>(machineSceneGameObject.transform);

            ViewManager.Instance.ShowSceneView(machineScene, true);
            
            Client.Get<MachineLogicController>().UpdateNeedDownloadSizeInfo(_assetId);

            await machineScene.viewController.InitMachineContext(Client.Get<MachineLogicController>(), machineAssetProvider);
        }
    }
}