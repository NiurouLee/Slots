// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/08/17/19:36
// Ver : 1.0.0
// Description : UserProfileModel.cs
// ChangeLog :
// **********************************************


using System.Collections.Generic;
using DragonU3DSDK.Network.API.ILProtocol;
using DragonU3DSDK.Storage;
using Google.ilruntime.Protobuf.Collections;

namespace GameModule
{
    public class UserProfileModel : Model<UserProfile>
    {
        public RepeatedField<uint> avatarIdsOwned;
        
        public UserProfileModel() :
            base(ModelType.TYPE_USPER_PROFILE)
        {
            
        }

        public override void UpdateModelData(UserProfile inModelData)
        {
            base.UpdateModelData(inModelData);
            
            if (inModelData != null 
                && inModelData.UserBasicInfo != null 
                && inModelData.UserBasicInfo.AvatarIdsOwned.Count > 0)
            {
                if (avatarIdsOwned == null)
                {
                    avatarIdsOwned = inModelData.UserBasicInfo.AvatarIdsOwned;

                    var newAvatars = Client.Storage.GetItem("NewAvatar", "");
                    var validNewAvatars = new List<string>();
                     
                    if (newAvatars != "")
                    {
                        var avatars = newAvatars.Split('|');
                        for (var i = 0; i < avatars.Length; i++)
                        {
                            if (avatarIdsOwned.Contains(avatars[i].ToUInt()))
                            {
                                validNewAvatars.Add(avatars[i]);
                            }
                        }

                        if (validNewAvatars.Count > 0)
                        {
                            newAvatars = "";

                            for (int i = 0; i < validNewAvatars.Count; i++)
                            {
                                newAvatars += validNewAvatars[i] + ((i != (validNewAvatars.Count - 1)) ? "|" : "");
                            }
                            
                            Client.Storage.SetItem("NewAvatar", newAvatars);
                        }
                        else
                        {
                            Client.Storage.SetItem("NewAvatar", "");
                        }
                    }
                }
            }
        }

        public string GetUserName()
        {
            return modelData.UserBasicInfo.UserName;
        }

        public uint GetUserAvatarID()
        {
            return modelData.UserBasicInfo.UserAvatarId;
        }
        
        public ulong GetUserId()
        {
            return StorageManager.Instance.GetStorage<StorageCommon>().PlayerId;
        }

        public ulong GetCoinsCount()
        {
            return modelData.Coins;
        }

        public ulong GetUserLevel()
        {
            return modelData.UserLevelInfo.Level;
        }
        
        public UserLevelInfo GetUserLevelInfo()
        {
            return modelData.UserLevelInfo;
        }
        
        public ulong GetDiamondCount()
        {
            return modelData.Diamonds;
        }

        public UserVipLevelInfo GetVipInfo()
        {
            return modelData.UserVipLevelInfo;
        }
    }
}