// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/07/21/12:15
// Ver : 1.0.0
// Description : LoginScene.cs
// ChangeLog :
// **********************************************

// using DragonU3DSDK.Network.API.ILProtocol;
//
// using TMPro;
// using UnityEngine;
// using UnityEngine.EventSystems;
// using UnityEngine.UI;
// namespace GameModule
// {
//     public class LoginScene : SceneView<LoginViewController>
//     {
//         public override bool IsPortraitScene { get; } = false;
//         public override SceneType SceneType { get; } = SceneType.TYPE_LOGIN;
//
//         
//         [ComponentBinder("PolicyText")] 
//         public TextMeshProUGUI policyText;
//
//         [ComponentBinder("FBButton")]
//         public Button fbButton;
//         
//         [ComponentBinder("GuestButton")]
//         public Button guestButton;
//         public LoginScene(string address)
//             : base(address, SceneType.TYPE_LOGIN)
//         {
//             
//         }
//     }
//
//     public class LoginViewController : ViewController<LoginScene>
//     {
//         public override void OnViewDidLoad()
//         {
//             base.OnViewDidLoad();
//               
//             view.fbButton.onClick.AddListener(OnFacebookLoginClicked);
//             view.guestButton.onClick.AddListener(OnGuestLoginClicked);
//             var pointerEventCustomHandler = view.policyText.gameObject.AddComponent<PointerEventCustomHandler>();
//             pointerEventCustomHandler.BindingPointerClick(OnPointerClick);
//
//             BiManagerGameModule.Instance.SendGameEvent(BiEventFortuneX.Types.GameEventType.GameEventEnterLoginScreen);
//         }
//  
//         
//         protected void OnFacebookLoginClicked()
//         {
//             EventBus.Dispatch(new EventFbLogin());
//         }
//
//         protected void OnGuestLoginClicked()
//         {
//             EventBus.Dispatch(new EventGuestLogin());
//         }
//         
//         public void OnPointerClick(PointerEventData pointerEventData)
//         {
//             int linkIndex =
//                 TMP_TextUtilities.FindIntersectingLink(view.policyText, pointerEventData.position,
//                     Camera.main); // If you are not in a Canvas using Screen Overlay, put your camera instead of null
//             if (linkIndex != -1)
//             {
//                 // was a link clicked?
//                 TMP_LinkInfo linkInfo = view.policyText.textInfo.linkInfo[linkIndex];
//                 if (linkInfo.GetLinkID() == "POLICY")
//                 {
//                     Application.OpenURL(ConfigurationController.Instance.PrivacyPolicyURL);
//                     BiManagerGameModule.Instance.SendGameEvent(BiEventFortuneX.Types.GameEventType.GameEventPrivacyClick);
//                 }
//                 else if (linkInfo.GetLinkID() == "SERVICE")
//                 {
//                     Application.OpenURL(ConfigurationController.Instance.PrivacyPolicyURL);
//                     BiManagerGameModule.Instance.SendGameEvent(BiEventFortuneX.Types.GameEventType.GameEventTermServiceClick);
//                 }
//             }
//         }
//     }
// }