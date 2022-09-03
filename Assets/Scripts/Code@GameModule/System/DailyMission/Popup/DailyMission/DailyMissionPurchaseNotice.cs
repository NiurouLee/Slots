//  **********************************************
//  Copyright(c) 2021 by com.ustar
//  All right reserved
// 
//  Author : Besure.Chen
//  Date :2021-09-23 15:41
//  Ver : 1.0.0
//  Description : DailyMissionPurchaseNotice.cs
//  ChangeLog :
//  **********************************************

using System;
using TMPro;
using UnityEngine.UI;

namespace GameModule
{
    [AssetAddress("UIDailyMissionPurchaseNoticeH","UIDailyMissionPurchaseNoticeV")]
    public class DailyMissionPurchaseNotice: Popup
    {
        [ComponentBinder("Root/BottomGroup/GetButton")]
        private Button btnCompleteNow;
        [ComponentBinder("Root/BottomGroup/GetButton/CurrencyGroup/CountText")]
        private TextMeshProUGUI txtCompleteNeedCost;
        
        private int nMissionIndex;
        private ulong ulCostDiamond;
        public Action<int, Action> actionCompleteNow;
        
        public Action actionFinish;

        public DailyMissionPurchaseNotice(string address)
            : base(address)
        {
            
        }

        public void Initialize(int index, ulong costDiamond)
        {
            nMissionIndex = index;
            ulCostDiamond = costDiamond;
            RefreshUI(costDiamond);
        }

        private void RefreshUI(ulong costDiamond)
        {
            txtCompleteNeedCost.text = costDiamond.ToString();
        }

        public void SubscribeFinishAction(Action action)
        {
            actionFinish = action;
        }

        [ComponentBinder("Root/BottomGroup/GetButton")]
        private void OnBtnComplete()
        {
            btnCompleteNow.interactable = false;
            Close();
            if (Client.Get<UserController>().GetDiamondCount() < ulCostDiamond)
            {
                EventBus.Dispatch(new EventShowPopup(new PopupArgs(typeof(StorePopup), StoreCommodityView.CommodityPageType.Diamond,"DailyMission")));
                return;
            }
            actionCompleteNow?.Invoke(nMissionIndex, actionFinish);
        }
    }
}