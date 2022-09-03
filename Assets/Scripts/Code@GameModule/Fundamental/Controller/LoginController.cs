/**********************************************

Copyright(c) 2021 by com.ustar
All right reserved

Author : Jian.Wang 
Date : 2020-9-29 20:15:03
Ver : 1.0.0
Description : 登陆逻辑管理
ChangeLog :  
**********************************************/

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using DragonU3DSDK.Account;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Network.API.ILProtocol;
using DragonU3DSDK.Network.PlayerProperties;
using DragonU3DSDK.Storage;

using GameModule.UI;
using SimpleJson;
using UnityEngine;

namespace GameModule
{
    public class LoginController : LogicController
    {
        private AccountType lastLoginAccountType;

        public LoginController(Client client)
            : base(client)
        {

        }

        protected override void SubscribeEvents()
        {
            SubscribeEvent<EventStartAutoLogin>(OnStartAutoLogin);
            SubscribeEvent<EventFbLogin>(OnFbLoginClick);
            SubscribeEvent<EventAppleLogin>(OnAppleLoginClick);
            SubscribeEvent<EventGuestLogin>(OnGuestLoginClick);
            SubscribeEvent<EventBindingFaceBook>(OnFaceBookBinding);
        }

        protected void OnStartAutoLogin(EventStartAutoLogin evt)
        {
            CheckAndStartAutoLogin();
        }


        public void CheckAndStartAutoLogin()
        {
            if (AccountManager.Instance.HasBindFacebook())
                LoginToServer(AccountType.FACEBOOK);
            else if (AccountManager.Instance.HasBindApple())
                LoginToServer(AccountType.APPLE);
            else if (AccountManager.Instance.HasLogin)
            {
                LoginToServer(AccountType.GUEST);
            }
        }
        
        public bool CanAutoLogin()
        {
            if (AccountManager.Instance.HasLogin)
                return true;
            if (AccountManager.Instance.HasBindFacebook())
                return true;
            if (AccountManager.Instance.HasBindApple())
                return true;
            
            return false;
        }

        public void OnFbLoginClick(EventFbLogin ev)
        {
            BiManagerGameModule.Instance.SendGameEvent(BiEventFortuneX.Types.GameEventType.GameEventUserLogin, ("loginType", "FaceBook"));
            LoginToServer(AccountType.FACEBOOK);

        }

        public void OnAppleLoginClick(EventAppleLogin ev)
        {
            BiManagerGameModule.Instance.SendGameEvent(BiEventFortuneX.Types.GameEventType.GameEventUserLogin, ("loginType", "Apple"));
            LoginToServer(AccountType.APPLE);

        }

        public void OnGuestLoginClick(EventGuestLogin ev)
        {
            XDebug.Log("OnGuestLoginClick");
            BiManagerGameModule.Instance.SendGameEvent(BiEventFortuneX.Types.GameEventType.GameEventUserLogin, ("loginType", "Guest"));
            LoginToServer(AccountType.GUEST);

        }

        private void ForceRefreshNetWorkStatus()
        {
            BindingFlags flag = BindingFlags.Instance | BindingFlags.NonPublic;
            Type type = APIManager.Instance.GetType();
            FieldInfo field = type.GetField("m_hasNetwork", flag);
            field?.SetValue(APIManager.Instance, Application.internetReachability != NetworkReachability.NotReachable);
            
            field = type.GetField("m_NetworkStatus", flag);
            field?.SetValue(APIManager.Instance, Application.internetReachability);
        }

        private async void LoginToServer(AccountType accountType)
        {
            //APIManager底层刷新网络状态是在每次发送心跳的时候才刷新，而心跳刷新时间是60S，这个在等了的时候强制刷新一次，避免实际有网络缺被判定为无网络的情况
            ForceRefreshNetWorkStatus();
            
            ViewManager.Instance.BlockingUserClick(true, "LoginToServer");
            await ViewManager.Instance.ShowScreenLoadingView();
            ViewManager.Instance.BlockingUserClick(false, "LoginToServer");
            
            PerformanceTracker.AddTrackPoint("LoginToServer");
            
            //已经登陆，就不再登陆了
            if (AccountManager.Instance.HasLogin && AccountManager.Instance.RefreshToken != null)
            {
                AccountManager.Instance.Relogin(AccountManager.Instance.RefreshToken, (success) =>
                {
                    if (success)
                        OnLoginBack();
                    else
                    {
                        CommonNoticePopup.ShowCommonNoticePopUp(
                            () => { EventBus.Dispatch(new EventRequestGameRestart(true)); }, "Login To Server Failed");
                    }
                });

                return;
            }

            var storageCommon = StorageManager.Instance.GetStorage<StorageCommon>();

            storageCommon.FacebookId = String.Empty;
            storageCommon.AppleAccountId = String.Empty;
            storageCommon.DeviceId = String.Empty;

            //如果要用FB登陆，但是玩家没有绑定FB，就用BindingFB登陆
            if (accountType == AccountType.FACEBOOK && !AccountManager.Instance.HasBindFacebook())
            {
                AccountManager.Instance.BindFacebook((succeed) =>
                {
                    if (succeed && AccountManager.Instance.HasBindFacebook())
                    {
                        OnLoginBack();
                    }
                    else
                    {
                        CommonNoticePopup.ShowCommonNoticePopUp(
                            () =>
                            {
                                EventBus.Dispatch(new EventRequestGameRestart(true));

                            }, "Login To Server Failed");
                    }
                }, () =>
                {
                    ViewManager.Instance.HideScreenLoadingView();

                    CommonNoticePopup.ShowCommonNoticePopUp(
                        () =>
                        { }, "User Canceled");
                });

                return;
            }

            if (accountType == AccountType.APPLE && !AccountManager.Instance.HasBindApple())
            {
                AccountManager.Instance.BindApple((succeed) =>
                {
                    if (succeed && AccountManager.Instance.HasBindApple())
                    {
                        OnLoginBack();
                    }
                    else
                    {
                        CommonNoticePopup.ShowCommonNoticePopUp(
                            () =>
                            {
                                EventBus.Dispatch(new EventRequestGameRestart(true));

                            }, "Login To Server Failed");
                    }
                }, () =>
                {
                    ViewManager.Instance.HideScreenLoadingView();

                    CommonNoticePopup.ShowCommonNoticePopUp(
                        () =>
                        { }, "User Canceled");
                });

                return;
            }

            lastLoginAccountType = accountType;

            XDebug.Log("DOLoin");

            AccountManager.Instance.Login(loginSuccess =>
            {
                if (loginSuccess)
                {
                    OnLoginBack();
                }
                else
                {
                    CommonNoticePopup.ShowCommonNoticePopUp(
                        () =>
                        {
                            EventBus.Dispatch(new EventRequestGameRestart(true));

                        }, "Login To Server Failed");
                }
            });
        }

        protected async Task<bool> InitWebsocket()
        {
            APIManagerGameModule.Instance.InitWebsocket();

            float timeOut = 0;

            while (!APIManagerGameModule.Instance.WebsocketConnected)
            {
                await Task.Delay(100);
                timeOut += 0.1f;
                if (timeOut >= 50)
                {
                    break;
                }
            }

            return APIManagerGameModule.Instance.WebsocketConnected;
        }


        void SendLoginEvent()
        {

            // send login to adjust
            var dataProvider = Dlugin.SDK.GetInstance().adjustPlugin;
            if (dataProvider != null)
            {
                var storageCommon = StorageManager.Instance.GetStorage<StorageCommon>();
                if (storageCommon.PlayerId > 0)
                {
                    dataProvider.SetSessionParameter("playerId", storageCommon.PlayerId.ToString());
                }

                dataProvider.TrackEvent("social_login", 0, "{}");
            }

            PlayerPropertiesManager.Instance.Init();
        }


        protected async void OnLoginBack()
        {
            XDebug.Log("LoginBack");
            PerformanceTracker.FinishTrackPoint("LoginToServer");
           
            SendLoginEvent();
            
            PerformanceTracker.AddTrackPoint("InitWebsocket");
            
            bool success = await InitWebsocket();
            
            PerformanceTracker.FinishTrackPoint("InitWebsocket");
           
            if (success)
            {
                PerformanceTracker.AddTrackPoint("CGetUserProfile");
                
                CGetUserProfile cUserProfile = new CGetUserProfile();

                var response =
                    await APIManagerGameModule.Instance.SendAsync<CGetUserProfile, SGetUserProfile>(cUserProfile);

                PerformanceTracker.FinishTrackPoint("CGetUserProfile");
               
                if (response != null && response.Response != null && response.ErrorCode == 0)
                {
                    XDebug.Log("<color=yellow>UserId:" + Client.Get<UserController>().GetUserId() + "</color>");

                    EventBus.Dispatch(new EventUserProfileUpdate(response.Response.UserProfile));
                    
                    PerformanceTracker.AddTrackPoint("SwitchToLobbyScene");
                    
                 //   EventBus.Dispatch(new EventSwitchScene(SceneType.TYPE_LOBBY));

                    EventBus.Dispatch(new EventLoginSuccess());
                }
                else
                {
#if !PRODUCTION_PACKAGE
                    CommonNoticePopup.ShowCommonNoticePopUp($"GetUser Profile From Server Failed[{response.ErrorCode}/{response.ErrorInfo}]");
#endif
                    XDebug.LogError("GetUserProfileFailed");
                }
            }
            else
            {
                CommonNoticePopup.ShowCommonNoticePopUp(
                    () =>
                    {
                        EventBus.Dispatch(new EventRequestGameRestart(true));

                    }, "Connect To Server Failed");
            }
        }


        // private void AfterLogin(ProtocolLogin protocol)
        // {
        //     EventBus.Dispatch(new EventGetUserFromServer(protocol.resolvedData.player));
        //     EventBus.Dispatch(new EventSwitchScene(SceneType.TYPE_LOBBY));
        //
        //     PTS.OnAfterLogin();
        //
        //     GetServerConfig();
        // }

        // private void GetServerConfig()
        // {
        //     var protocol = new ProtocolGetConfig();
        //     protocol.Send(OnGetServerConfig);
        // }

        // private void OnGetServerConfig(Protocol protocol)
        // {
        //     XDebug.Log("OnGetServerConfig");
        //     EventBus.Dispatch(new EventOnGetServerConfig(protocol as ProtocolGetConfig));
        // }

        public void SetAutoLogin(bool autoLogin)
        {
            XDebug.Log("SetAutoLogin:" + autoLogin);
            Client.Storage.SetItem("AutoLogin", autoLogin ? 1 : 0);
        }

        public bool NeedShowLoginView()
        {
            return !CanAutoLogin();
        }

        public void OnFaceBookBinding(EventBindingFaceBook evt)
        {
            // if (PTS.FaceBook.IsLoginIn() && Client.Player.AccountType == AccountType.FACEBOOK)
            //     return;
            //
            // PTS.FaceBook.Login((userId) =>
            // {
            //     if (userId != null)
            //     {
            //         var protocol = new ProtocolBindAccount()
            //         {
            //             accountId = userId,
            //             clientId = SystemInfo.deviceUniqueIdentifier,
            //             accountType = (int) AccountType.FACEBOOK
            //         };
            //
            //         protocol.accountInfo = new JsonObject();
            //         protocol.accountInfo.Add("id", userId);
            //
            //         var name = PTS.FaceBook.GetUserInfoField<string>("name");
            //         var email = PTS.FaceBook.GetUserInfoField<string>("email");
            //         if (email != null)
            //             protocol.accountInfo.Add("email", email);
            //         if (name != null)
            //             protocol.accountInfo.Add("name", name);
            //
            //         protocol.Send(OnBindAccountResponse);
            //     }
            // });
        }

        // private void OnBindAccountResponse(Protocol inProtocol)
        // {
        //     var protocol = inProtocol as ProtocolBindAccount;
        //     if (protocol != null)
        //     {
        //         Client.Storage.SetItem("AccountType", (int) protocol.accountType);
        //         if (protocol.errorCode == ErrorCode.ACCOUNT_PLAYER_BOUND || protocol.errorCode == 0)
        //         {
        //           //  Client.Player.UpdatePlayerInfoMap(protocol.accountInfoMap, (AccountType) protocol.accountType);
        //
        //             EventBus.Dispatch(new EventBindStatusChanged());
        //         }
        //         else
        //         {
        //             EventBus.Dispatch(new EventRequestGameRestart());
        //         }
        //     }
        // }

        // private bool CanAutoLogin()
        // {
        //
        //     return true;
        //     var accountType = (AccountType)Client.Storage.GetItem("AccountType", 0);
        //
        //     //新账号
        //     if (accountType == AccountType.UNDEFINED)
        //         return true;
        //
        //
        //     //如果上次登录是FB登录，并且FB是登录状态，就自动登录
        //
        //
        //     //如果上次登录是Guest 登录
        //     if (accountType == AccountType.GUEST)
        //     {
        //         var count = Client.Storage.GetItem("AccountLoginCount", 0);
        //         //如果连续用Guest登录了5次
        //         if (count >= 4)
        //         {
        //             var timeStamp = Convert.ToInt64(Client.Storage.GetItem("AccountTypeLastStoreTime",
        //                 DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString()));
        //             var dataOffset = DateTimeOffset.FromUnixTimeMilliseconds(timeStamp);
        //             var offset = DateTimeOffset.UtcNow - dataOffset;
        //             //并且距离最开始登录的那一天时间少于30天，就走自动登录
        //             if (offset.Days < 30)
        //             {
        //                 return true;
        //             }
        //         }
        //     }
        //
        //     //     XDebug.Log("Can't Auto Login:" + accountType);
        //
        //     return false;
        // }

        private void UpdateLoginStatus()
        {
            //更新玩家登录次数，玩家登录账号类型，登录时间
            var storeAccountType = (AccountType)Client.Storage.GetItem("AccountType", 0);

            //非FB账号，处于FB登录状态，登出FB账号

            if (lastLoginAccountType == storeAccountType && lastLoginAccountType == AccountType.GUEST)
            {
                var loginCount = Client.Storage.GetItem("AccountLoginCount", 0);
                Client.Storage.SetItem("AccountLoginCount", loginCount + 1);

                var timeStamp = Convert.ToInt64(Client.Storage.GetItem("AccountTypeLastStoreTime",
                    DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString()));
                var dataOffset = DateTimeOffset.FromUnixTimeMilliseconds(timeStamp);
                var offset = DateTimeOffset.UtcNow - dataOffset;

                if (offset.Days >= 30)
                {
                    Client.Storage.SetItem("AccountTypeLastStoreTime",
                        DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString());
                }
            }
            else
            {
                //   Client.Storage.SetItem("AccountType", (int) Client.Player.AccountType);

                if (lastLoginAccountType == AccountType.GUEST)
                {
                    Client.Storage.SetItem("AccountTypeLastStoreTime",
                        DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString());

                    //首次登录不算次数
                    if (storeAccountType == AccountType.UNDEFINED)
                        Client.Storage.SetItem("AccountLoginCount", 0);
                    else
                    {
                        Client.Storage.SetItem("AccountLoginCount", 1);
                    }
                }
            }
        }
    }
}