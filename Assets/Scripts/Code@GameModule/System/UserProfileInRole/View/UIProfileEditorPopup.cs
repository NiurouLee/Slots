using System.Collections.Generic;
using DragonU3DSDK.Account;
using DragonU3DSDK.Network.API.ILProtocol;
using Google.ilruntime.Protobuf.Collections;
using Sirenix.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameModule
{
    [AssetAddress("UIProfileEditor")]
    public class UIProfileEditorPopup : Popup
    {
        [ComponentBinder("Root/BasicInformationGroup/PlayerHeadGroup/NameGroup/InputField")]
        public InputField tmpInputFieldName;

        [ComponentBinder("Root/ScrollView/Viewport/Content")]
        public Transform transformHeadRoot;

        [ComponentBinder("Root/ScrollView/Viewport/Content")]
        public Transform toggleGroupAvatar;

        [ComponentBinder("Root/ScrollView/Viewport/Content/PlayerHeadCell")]
        public Transform transformHeadPrefab;

        [ComponentBinder("Root/BasicInformationGroup/SaveButton")]
        public Button buttonSave;

        [ComponentBinder("Root/BasicInformationGroup/PlayerHeadGroup/PlayerHeadButton/LevelGroup/LevelText")]
        public TMP_Text tmpTextLevel;

        [ComponentBinder("Root/BasicInformationGroup/PlayerHeadGroup/PlayerHeadButton/AvatarMask/Icon")]
        public RawImage rawImageAvatar;

        private int avatarCount = 9;

        private UIProfilePlayerHeadCell[] _headCells;

        private UserController _userController;

        private List<uint> _ownedAvatar;

        public UIProfileEditorPopup(string address) : base(address)
        {
            contentDesignSize = new Vector2(1365, 768);
        }

        protected void UpdateSortedAvatars()
        {
            var avatars = _userController.GetUserAvailableHeadPortrait();
            _ownedAvatar = new List<uint>();

            bool hasFb = false;
            for (var i = 0; i < avatars.Count; i++)
            {
                if(avatars[i] != 9999 && avatars[i] > 7)
                    _ownedAvatar.Add(avatars[i]);
                if (avatars[i] == 9999)
                {
                    hasFb = true;
                }
            }
            
            if(_ownedAvatar.Count > 0)
                _ownedAvatar.Sort();

            for (uint i = 0; i <= 7; i++)
            {
                _ownedAvatar.Add(i);
            }

            if (hasFb)
            {
                _ownedAvatar.Insert(0,9999);
            }
        }
        
        protected override void BindingComponent()
        {
            base.BindingComponent();

            _userController = Client.Get<UserController>();

            UpdateSortedAvatars();
             
            avatarCount = _ownedAvatar.Count;
            
            _headCells = new UIProfilePlayerHeadCell[avatarCount];
             
            transformHeadPrefab.gameObject.SetActive(false);

            for (int i = 0; i < avatarCount; i++)
            {
                var obj = GameObject.Instantiate(transformHeadPrefab.gameObject, transformHeadRoot);
                obj.transform.localScale = Vector3.one;
                obj.SetActive(true);
                var headCell = AddChild<UIProfilePlayerHeadCell>(obj.transform);

                _headCells[i] = headCell;

                var avatarID = _ownedAvatar[i];

                if (headCell != null)
                {
                    headCell.Set(AvatarController.defaultAvatar, avatarID);
                    headCell.avatarId = avatarID;
                    
                    AvatarController.GetSelfAvatar(avatarID, (t) =>
                    {
                        if (headCell.transform)
                        {
                            headCell.Set(t,avatarID);
                        }
                    });
                }

                headCell.SetOnValueChanged(OnAvatarSelectionChanged);
            }
            tmpInputFieldName.onValueChanged.AddListener(OnNameChanged);
            buttonSave.onClick.AddListener(OnSaveButtonClick);
            buttonSave.interactable = ShouldSaveBottomInteractive();

            tmpInputFieldName.characterLimit = 14;
        }

        public override void OnOpen()
        {
            base.OnOpen();
            
            BiManagerGameModule.Instance.SendGameEvent(BiEventFortuneX.Types.GameEventType.GameEventAvatarOpen);
 
            Refresh();
        }

        private uint AvatarIndexToID(int avatarIndex)
        {
            var avatarID = (avatarIndex == 0) ? 9999 : avatarIndex - 1;
            return (uint)avatarID;
        }

        private int AvatarIDToIndex(uint avatarID)
        {
            var avatarIndex = (avatarID == 9999) ? 0 : avatarID + 1;
            return (int)avatarIndex;
        }

        public int GetSelectedIndex()
        {
            for (int i = 0; i < avatarCount; i++)
            {
                var headCell = _headCells[i];
                if (headCell.isOn == true)
                {
                    return i;
                }
            }
            return 0;
        }

        private uint GetAvatarID()
        {
            var selected = GetSelectedIndex();
            return _headCells[selected].avatarId;
        }

        private void OnNameChanged(string text)
        {
            buttonSave.interactable = ShouldSaveBottomInteractive();
        }

        private bool ShouldSaveBottomInteractive()
        {
            return (IsAvatarModified() || IsNameModified()) && string.IsNullOrWhiteSpace(tmpInputFieldName.text) == false;
        }

        private bool IsAvatarModified()
        {
            var selectedAvatarID = GetAvatarID();
            var avatarID = _userController.GetUserAvatarID();
            return selectedAvatarID != avatarID;
        }

        private bool IsNameModified()
        {
            var userName = _userController.GetUserName();
            return userName != tmpInputFieldName.text;
        }

        private void OnAvatarSelectionChanged(bool isOn)
        {
            if (isOn)
            {
                var selectedAvatarID = GetAvatarID();

                if (rawImageAvatar != null)
                {

                    AvatarController.GetSelfAvatar(selectedAvatarID, (t) =>
                    {
                        if (rawImageAvatar != null && selectedAvatarID == GetAvatarID())
                        {
                            rawImageAvatar.texture = t;
                        }
                    });
                }

                buttonSave.interactable = ShouldSaveBottomInteractive();
            }
        }

        private async void OnSaveButtonClick()
        {
            var selectedAvatarID = GetAvatarID();

            var modifiedName = tmpInputFieldName.text;

            if (IsAvatarModified() || IsNameModified())
            {
                
                BiManagerGameModule.Instance.SendGameEvent(BiEventFortuneX.Types.GameEventType.GameEventAvatarChange, ("AvatarId",selectedAvatarID.ToString()));
                
                var result = await Client.Get<UserProfileInRoleController>().RequestCUpdateUserProfile(modifiedName, selectedAvatarID);
                if (result != null)
                {
                    PopupStack.ClosePopup<UIProfileEditorPopup>();

                    EventBus.Dispatch(new EventUserProfileUpdate(result.UserProfile));
                }
            }
            else
            {
                PopupStack.ClosePopup<UIProfileEditorPopup>();
            }
        }

        public void Refresh()
        {
            var controller = Client.Get<UserController>();

            if (tmpInputFieldName != null)
            {
                tmpInputFieldName.text = controller.GetUserName();
            }

            if (tmpTextLevel != null)
            {
                tmpTextLevel.text = $"LV:{controller.GetUserLevel()}";
            }

            var avatarID = controller.GetUserAvatarID();
 
            if (rawImageAvatar != null)
            {
                rawImageAvatar.texture = AvatarController.defaultAvatar;

                AvatarController.GetSelfAvatar(avatarID, (t) =>
                {
                    if (rawImageAvatar != null && controller != null && controller.GetUserAvatarID() == avatarID)
                    {
                        rawImageAvatar.texture = t;
                    }
                });
            }
            
            var avatarIndex = _ownedAvatar.IndexOf(avatarID);

            _headCells[avatarIndex].isOn = true;

            var fbAvatarIndex = _ownedAvatar.IndexOf(9999);
            
            if(fbAvatarIndex >= 0)
                _headCells[fbAvatarIndex].transform.gameObject.SetActive(AccountManager.Instance.HasBindFacebook());
        }

        public override void OnClose()
        {
            base.OnClose();
            Client.Get<UserController>().ResetNewAvatarState();
        }
    }
}
