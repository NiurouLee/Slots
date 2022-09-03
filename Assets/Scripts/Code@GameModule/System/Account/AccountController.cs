using System;
using System.Threading.Tasks;
using DragonU3DSDK;
using DragonU3DSDK.Account;
using DragonU3DSDK.Network.API.ILProtocol;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.SDKEvents;
using Facebook.Unity;
using UnityEngine;
using BiEventFortuneX = DragonU3DSDK.Network.API.ILProtocol.BiEventFortuneX;

namespace GameModule
{
    public class AccountController : LogicController
    {
        public AccountController(Client client) : base(client) { }

        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();
            EventManager.Instance.Subscribe<ThirdAlreadyBindErrorEvent>(OnThirdAlreadyBindErrorEvent);
        }

        public async void OnThirdAlreadyBindErrorEvent(ThirdAlreadyBindErrorEvent message)
        {
            XDebug.Log($"111111111111 ThirdAlreadyBindErrorEvent, type={message.type}");
            if (message.type == "facebook")
            {
                var token = message.token;
                var c = new CGetUserInfoByFacebook() { FacebookToken = token };
                var handler = await APIManagerGameModule.Instance.SendAsync<CGetUserInfoByFacebook, SGetUserInfoByFacebook>(c);
                if (handler.ErrorCode == ErrorCode.Success)
                {
                    var popup = PopupStack.GetPopup<UISwitchAccount>();
                    if (popup == null)
                    {
                        popup = await PopupStack.ShowPopup<UISwitchAccount>();
                    }
                    var data = new FacebookSwitchAccountData()
                    {
                        facebookToken = token,
                        facebookName = handler.Response.FacebookName,
                        userAvatarId = handler.Response.UserProfile.UserBasicInfo.UserAvatarId,
                        facebookID = handler.Response.FacebookId
                    };

                    popup.SetFacebook(data);
                }
                else
                {
                    Debug.LogError($"1111111111 GetFacebookInfo fail : {handler.ErrorCode} {handler.ErrorInfo}");
                }
            }
            else if (message.type == "apple")
            {
                var popup = PopupStack.GetPopup<UISwitchAccount>();
                if (popup == null)
                {
                    popup = await PopupStack.ShowPopup<UISwitchAccount>();
                }
                popup.SetApple();
            }
        }
        public static async void UpdateUserProfile()
        {
            CGetUserProfile cUserProfile = new CGetUserProfile();

            var response =
                await APIManagerGameModule.Instance.SendAsync<CGetUserProfile, SGetUserProfile>(cUserProfile);

            EventBus.Dispatch(new EventUserProfileUpdate(response.Response.UserProfile));
        }

        public static async void SendCClaimFacebookBindingReward()
        {
            var c = new CClaimFacebookBindingReward();

            var handle = await APIManagerGameModule.Instance.SendAsync<CClaimFacebookBindingReward, SClaimFacebookBindingReward>(c);

            if (handle != null && handle.ErrorCode == 0)
            {
                var s = handle.Response;
                if (s != null)
                {
                    var popup = PopupStack.GetPopup<UICongratulations>();
                    if (popup == null)
                    {
                        popup = await PopupStack.ShowPopup<UICongratulations>();
                    }
                    popup.Set(s.Rewards, s.UserProfile, "facebook");
                }
                XDebug.Log("111111111 receive SClaimFacebookBindingReward success");
            }
            else { XDebug.LogError("111111111 receive SClaimFacebookBindingReward failed"); }
        }

        public static async void SendCClaimAppleBindingReward()
        {
            var c = new CClaimAppleBindingReward();

            var handle = await APIManagerGameModule.Instance.SendAsync<CClaimAppleBindingReward, SClaimAppleBindingReward>(c);

            if (handle != null && handle.ErrorCode == 0)
            {
                var s = handle.Response;
                if (s != null)
                {
                    var popup = PopupStack.GetPopup<UICongratulations>();
                    if (popup == null)
                    {
                        popup = await PopupStack.ShowPopup<UICongratulations>();
                    }

                    if (s != null)
                    {
                        popup.Set(s.Rewards, s.UserProfile, "apple");
                    }
                }
                XDebug.Log("111111111 receive SClaimAppleBindingReward success");
            }
            else { XDebug.LogError("111111111 receive SClaimAppleBindingReward failed"); }
        }

        public static async void FacebookLogin()
        {
            var hasBindFacebook = AccountManager.Instance.HasBindFacebook();

            if (hasBindFacebook == false)
            {
                if (FacebookManager.Instance.IsLoggedIn())
                {
                    FacebookManager.Instance.LogOut(async (s, info, arg3) =>
                    {
                        XDebug.Log("11111111111111 facebook LogOut success!!!");
                        await BindFaceBook();
                    });
                }
                else
                {
                    await BindFaceBook();
                }
            }
        }

        public static async void AppleLogin()
        {
            var hasBindApple = AccountManager.Instance.HasBindApple();

            if (hasBindApple == false)
            {
                if (AppleLoginManager.Instance.IsLoggedIn())
                {
                    AppleLoginManager.Instance.LogOut(async (s, info, arg3) =>
                    {
                        XDebug.Log("11111111111111 apple LogOut success!!!");
                        await BindApple();
                    });
                }
                else
                {
                    await BindApple();
                }
            }
        }

        private static async Task BindFaceBook()
        {
            await Task.Yield();

            AccountManager.Instance.BindFacebook(
                                async (succeed) =>
                                {
                                    if (succeed)
                                    {
                                        XDebug.Log("11111111111 Bind facebook success");
                                        EventBus.Dispatch(new EventAccountUpdate());
                                        SendCClaimFacebookBindingReward();
                                        BiManagerGameModule.Instance.SendGameEvent(BiEventFortuneX.Types.GameEventType.GameEventBindFacebookSuccess);
                                    }
                                    else
                                    {
                                        var popup = PopupStack.GetPopup<UIWhatsUp>();
                                        if (popup == null)
                                        {
                                            popup = await PopupStack.ShowPopup<UIWhatsUp>();
                                        }
                                        popup.Set("facebook");
                                        XDebug.LogError("11111111111 Bind facebook failed");
                                        BiManagerGameModule.Instance.SendGameEvent(BiEventFortuneX.Types.GameEventType.GameEventBindFacebookFail, ("reason", "sdk 未提供原因"));
                                    }
                                    UpdateUserProfile();
                                },
                                () =>
                                {
                                    XDebug.Log("1111111111 User canceled facebook login");
                                },
                                true);
        }

        private static async Task BindApple()
        {
            await Task.Yield();

            AccountManager.Instance.BindApple(
                                async (succeed) =>
                                {
                                    if (succeed)
                                    {
                                        XDebug.Log("11111111111 Bind apple success");
                                        EventBus.Dispatch(new EventAccountUpdate());
                                        SendCClaimAppleBindingReward();
                                    }
                                    else
                                    {
                                        var popup = PopupStack.GetPopup<UIWhatsUp>();
                                        if (popup == null)
                                        {
                                            popup = await PopupStack.ShowPopup<UIWhatsUp>();
                                        }
                                        popup.Set("apple");
                                        XDebug.LogError("11111111111 Bind apple failed");
                                    }
                                    UpdateUserProfile();
                                },
                                () =>
                                {
                                    XDebug.Log("1111111111 User canceled apple login");
                                },
                                true);
        }
    }
}
