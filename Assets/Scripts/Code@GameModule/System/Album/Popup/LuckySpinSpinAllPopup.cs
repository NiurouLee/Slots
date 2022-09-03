// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2022/03/19/13:40
// Ver : 1.0.0
// Description : LuckySpinSpinAllPopup.cs
// ChangeLog :
// **********************************************

using System;
using DragonU3DSDK.Network.API.ILProtocol;
using UnityEngine.UI;

namespace GameModule
{
    [AssetAddress("UICardSpinAll")]
    public class LuckySpinSpinAllPopup : Popup<ViewController>
    {
        [ComponentBinder("ConfirmButton")] public Button confirmButton;

        public Action confirmAction;

        public LuckySpinSpinAllPopup(string address)
            : base(address)
        {
            
        }

        protected void OnEventAlbumSeasonEnd(EventAlbumSeasonEnd evt)
        {
            Close();
        }

        protected override void OnViewSetUpped()
        {
            confirmButton.onClick.AddListener(OnConfirmButtonClicked);
            base.OnViewSetUpped();
            viewController.SubscribeEvent<EventAlbumSeasonEnd>(OnEventAlbumSeasonEnd);
        }

        protected override void OnCloseClicked()
        {
            base.OnCloseClicked();
            BiManagerGameModule.Instance.SendGameEvent(BiEventFortuneX.Types.GameEventType.GameEventCollectionLuckySpinStart,("Operation:", "SpinAllCloseButton"),("OperationId","4"));
        }

        protected void OnConfirmButtonClicked()
        {
            Close();
            BiManagerGameModule.Instance.SendGameEvent(BiEventFortuneX.Types.GameEventType.GameEventCollectionLuckySpinStart,("Operation:", "SpinAllConfirmButton"),("OperationId","3"));
            confirmAction?.Invoke();
        }

        public void SetConfirmAction(Action inConfirmAction)
        {
            confirmAction = inConfirmAction;
        }
    }
}