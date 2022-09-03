// /**********************************************
//
// Copyright(c) 2021 by com.ustar
// All right reserved
//
// Author : Jian.Wang 
// Date : 2020-09-30 21:07:03
// Ver : 1.0.0
// Description : 
// ChangeLog :  
// **********************************************/
// using System.Threading.Tasks;
// using DragonU3DSDK.Account;
//
// namespace GameModule
// {
//     public class LoginSceneSwitchAction : SceneSwitchAction
//     {
//         public LoginSceneSwitchAction()
//             : base(SceneType.TYPE_LOGIN)
//         {
//         }
//
//         public override void InitializeLeaveMask()
//         {
//             readyLeaveMask =  (int) SwitchMask.MASK_SERVER_READY;
//         }
//
//         public override void DoLeaveSceneAction()
//         {
//         }
//
//         public async override Task CreateAndRunTargetScene()
//         {
//             XDebug.Log("ShowLoginView");
//           //  var loginLogic = null;// Client.Get<LoginLogic>();
//           // 自动登录不需要显示登录界面
//            if (!AccountManager.Instance.HasLogin)
//            {
//                await ViewManager.Instance.ShowSceneView<LoginScene>("Login", false);
//            }
//            else
//            {
//                EventBus.Dispatch(new EventEnterLoginScene());
//              
//                await Task.CompletedTask;
//                // Log.LogStateEvent(State.EnterLoginScene, new Dictionary<string, object>() {{"AutoLogin:", true}},
//                //     StepId.ID_ENTER_LOGIN);
//            }
//         }
//     }
// }