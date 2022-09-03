// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/08/03/14:38
// Ver : 1.0.0
// Description : MachineEditorHelper.cs
// ChangeLog :
// **********************************************


#if UNITY_EDITOR

namespace GameModule
{
    public static class MachineEditorHelper
    {
        public static void ApplyMachineConfig(string machineConfig)
        {
            var machineScene = ViewManager.Instance.GetSceneView<MachineScene>();
            if (machineScene == null)
            {
                return;
            }

            var viewController = machineScene.viewController;
            var machineContext = viewController.GetMachineContext();

            var newMachineConfig = LitJson.JsonMapper.ToObject<MachineConfig>(machineConfig);

            viewController.GetMachineContext().machineConfig.UpdateConfig(newMachineConfig);

            var wheelStates = machineContext.state.GetAll<WheelState>();

            for (var i = 0; i < wheelStates.Count; i++)
            {
                wheelStates[i].RefreshWheelConfig();
            }
        }
    }
}

#endif