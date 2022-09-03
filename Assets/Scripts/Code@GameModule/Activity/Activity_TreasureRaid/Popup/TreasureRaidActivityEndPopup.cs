using System;
using DragonU3DSDK.Network.API.ILProtocol;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameModule
{
    [AssetAddress("UITreasureRaidActivityEnd")]
    public class TreasureRaidActivityEndPopup : Popup
    {
        [ComponentBinder("Root/BottomGroup/ConfirmButton")]
        private Button okBtn;

        [ComponentBinder("Root/BottomGroup/ConfirmButton/ClickMask")]
        private Transform btnMask;

        public TreasureRaidActivityEndPopup(string address)
            : base(address)
        {
            contentDesignSize = ViewResolution.designSize;
        }
        
        protected override void BindingComponent()
        {
            base.BindingComponent();
            okBtn.onClick.AddListener(OnOkBtnClicked);
        }

        private void SetBtnState(bool interactable)
        {
            closeButton.interactable = interactable;
            okBtn.interactable = interactable;
            btnMask.gameObject.SetActive(!interactable);
        }

        private void OnOkBtnClicked()
        {
            SetBtnState(false);
            Close();
        }

        public override void Close()
        {
            base.Close();
            EventBus.Dispatch(new EventTreasureRaidOnExpire());
        }
    }
}