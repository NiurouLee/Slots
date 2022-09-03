using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using DragonU3DSDK.Network.API.ILProtocol;
using UnityEngine;

namespace GameModule
{
    public class UserProfileInRoleController : LogicController
    {
        public UserProfileInRoleController(Client client) : base(client) { }

        public SGetUserProfileInRole sGetUserProfileInRoleData;

        public async Task<SUpdateUserProfile> RequestCUpdateUserProfile(string name, uint avatarID)
        {
            var c = new CUpdateUserProfile();
            c.UserProfile = new UserProfile();
            c.UserProfile.UserBasicInfo = new UserBasicInfo();
            c.UserProfile.UserBasicInfo.UserAvatarId = avatarID;
            c.UserProfile.UserBasicInfo.UserName = name;

            var s = await APIManagerGameModule.Instance.SendAsync<CUpdateUserProfile, SUpdateUserProfile>(c);

            if (s == null || s.ErrorCode != 0 || s.Response == null)
            {
                XDebug.LogError("1111111111 Modify user profile failed");
            }else{
                sGetUserProfileInRoleData.RoleInfo.UserBaseInfo.UserBasicInfo.UserName = name;
                sGetUserProfileInRoleData.RoleInfo.UserBaseInfo.UserBasicInfo.UserAvatarId = avatarID;
                XDebug.Log("1111111111 Modify user profile success");
            }

            return s.Response;
        }


        public async Task RequestCGetUserProfileInRole()
        {
            var c = new CGetUserProfileInRole();
            var s = await APIManagerGameModule.Instance.SendAsync<CGetUserProfileInRole, SGetUserProfileInRole>(c);
            if (s == null) { return; }


            if (s.ErrorCode == 0)
            {
                sGetUserProfileInRoleData = s.Response;

                if (sGetUserProfileInRoleData == null) { return; }

                var popup = PopupStack.GetPopup<UIProfilePopup>();
                if (popup == null)
                {
                    popup = await PopupStack.ShowPopup<UIProfilePopup>();
                }
                EventBus.Dispatch(new EventReceiveUserProfileInRole());
            }
            else
            {
                XDebug.LogError("GetUserProfileInRoleResponseError:" + s.ErrorInfo);
            }
        }
    }
}
