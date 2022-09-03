// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/07/21/13:54
// Ver : 1.0.0
// Description : UserController.cs
// ChangeLog :
// **********************************************

using System.Linq;
using DragonU3DSDK.Network.API.ILProtocol;
using Google.ilruntime.Protobuf.Collections;
using UnityEditor;

namespace GameModule
{
    public class UserController : LogicController
    {
        private UserProfileModel _model;

        public UserController(Client client) : base(client)
        {
            _model = new UserProfileModel();
        }

        public RepeatedField<uint> GetUserAvailableHeadPortrait()
        {
            return _model.avatarIdsOwned;
        }

        public void AddNewAvatar(uint avatar)
        {
            if (!_model.avatarIdsOwned.Contains(avatar))
            {
                _model.avatarIdsOwned.Add(avatar);
                var newAvatars = Client.Storage.GetItem("NewAvatars", "");

                if (newAvatars == "")
                {
                    newAvatars = $"{avatar}";
                }
                else
                {
                    newAvatars = newAvatars + "|" + avatar;
                }
                
                Client.Storage.SetItem("NewAvatar", newAvatars);
                
                EventBus.Dispatch(new EventUserNewAvatarStateChanged());
            }
        }
        
        public bool HasNewAvatar()
        {
             return Client.Storage.GetItem("NewAvatar", "") != "";
        }
        
        public void ResetNewAvatarState()
        {
            Client.Storage.SetItem("NewAvatar", "");
            EventBus.Dispatch(new EventUserNewAvatarStateChanged());
        }
        
        public bool IsNewAvatar(uint avatar)
        {
            var newAvatars = Client.Storage.GetItem("NewAvatar", "");
            if (newAvatars != "")
            {
                var avatars = newAvatars.Split('|');
                return avatars.Contains(avatar.ToString());
            }

            return false;
        }
        
        /// <summary>
        /// 提供一个空的虚接口统一订阅游戏内的事件
        /// </summary>
        protected override void SubscribeEvents()
        {
            SubscribeEvent<EventUserProfileUpdate>(OnUserProfileUpdate);
        }

        private void OnUserProfileUpdate(EventUserProfileUpdate evt)
        {
            _model.UpdateModelData(evt.userProfile);
            isValid = true;
            EventBus.Dispatch(new EventUpdateRoleInfo());
        }

        protected bool isValid = false;
        public bool IsValid()
        {
            return isValid;
        }

        public string GetUserName()
        {
            return _model.GetUserName();
        }

        public uint GetUserAvatarID()
        {
            return _model.GetUserAvatarID();
        }

        public ulong GetUserId()
        {
            return _model.GetUserId();
        }

        public ulong GetUserLevel()
        {
            return _model.GetUserLevel();
        }
        
        public UserLevelInfo GetUserLevelInfo()
        {
            return _model.GetUserLevelInfo();
        }
        
        public ulong GetDiamondCount()
        {
            return _model.GetDiamondCount();
        }

        public ulong GetCoinsCount()
        {
            return _model.GetCoinsCount();
        }

        public UserVipLevelInfo GetVipInfo()
        {
            return _model.GetVipInfo();
        }
    }
}